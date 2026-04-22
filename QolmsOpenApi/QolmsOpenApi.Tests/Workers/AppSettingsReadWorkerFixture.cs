using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppSettingsReadWorkerFixture
    {
        AppSettingsReadWorker _worker;
        Mock<IAppSettingsRepository> _appSettingsRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _appSettingsRepo = new Mock<IAppSettingsRepository>();
            _worker = new AppSettingsReadWorker(_appSettingsRepo.Object);
        }


        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void アプリ種別が正しくない場合はエラー()
        {
            var args = GetValidArgs();
            args.ExecuteSystemType = "999";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("アプリ種別が不正").IsTrue();
        }


        [TestMethod]
        public void アプリ設定取得処理で例外が発生したらエラー()
        {
            var args = GetValidArgs();

            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("アプリ設定取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void アプリ設定が存在しない場合は空データを返す()
        {
            var args = GetValidArgs();

            // DBにデータはない
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Returns(default(QH_APPSETTINGS_DAT));

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var settings = new QsJsonSerializer().Deserialize<QhSettingsHeartMonitorOfJson>(results.SettingsJson);

            // 全て初期値である
            settings.IsAutoHeartRateThreshold.IsFalse();
            settings.RestingHeartRate.Is(0);
            settings.HeartRateThresholdLow.MinValue.Is(0);
            settings.HeartRateThresholdLow.MaxValue.Is(0);
            settings.HeartRateThresholdMid.MinValue.Is(0);
            settings.HeartRateThresholdMid.MaxValue.Is(0);
            settings.HeartRateThresholdHigh.MinValue.Is(0);
            settings.HeartRateThresholdHigh.MaxValue.Is(0);
            settings.MonitorList.Count.Is(0);
        }

        [TestMethod]
        public void アプリ設定を正常に取得できる()
        {
            var args = GetValidArgs();

            var settings = new QhSettingsHeartMonitorOfJson
            {
                IsAutoHeartRateThreshold = true,
                HeartRateThresholdLow = new QhHeartRateThresholdItem
                {
                    MinValue = 30,
                    MaxValue = 100
                },
                HeartRateThresholdMid = new QhHeartRateThresholdItem
                {
                    MinValue = 35,
                    MaxValue = 110
                },
                HeartRateThresholdHigh = new QhHeartRateThresholdItem
                {
                    MinValue = 40,
                    MaxValue = 120
                },
                RestingHeartRate = 100,
                MonitorList = new List<QhHeartMonitorMemberItem>
                {
                    new QhHeartMonitorMemberItem
                    {
                        RelationshipType = QsDbFamilyRelationshipTypeEnum.Father,
                        MailAddress = "abc@def.com",
                        RequireNotifyMail = false
                    },
                    new QhHeartMonitorMemberItem
                    {
                        RelationshipType = QsDbFamilyRelationshipTypeEnum.Child,
                        MailAddress = "hoge@def.com",
                        RequireNotifyMail = true
                    }
                }
            };

            var json = new QsJsonSerializer().Serialize(settings);

            var entity = new QH_APPSETTINGS_DAT
            {
                ACCOUNTKEY = _accountKey,
                APPTYPE = (int)QsDbApplicationTypeEnum.HeartMonitorApp,
                VALUE = json
            };

            // 設定を返す
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Returns(entity);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // Json値が正しく設定されている
            results.SettingsJson.Is(json);
        }

        QoAppSettingsReadApiArgs GetValidArgs()
        {
            return new QoAppSettingsReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
            };
        }
    }
}
