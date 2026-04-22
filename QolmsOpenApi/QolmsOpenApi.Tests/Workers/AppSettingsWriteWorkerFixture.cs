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
    public class AppSettingsWriteWorkerFixture
    {
        AppSettingsWriteWorker _worker;
        Mock<IAppSettingsRepository> _appSettingsRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _appSettingsRepo = new Mock<IAppSettingsRepository>();
            _worker = new AppSettingsWriteWorker(_appSettingsRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Write(args);

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

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("アプリ種別が不正").IsTrue();
        }

        [TestMethod]
        public void 設定値Jsonが復元不能な場合はエラーとなる()
        {
            var args = GetValidArgs();

            args.SettingsJson = @"abcdef";

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("設定値が不正です").IsTrue();
        }

        [TestMethod]
        public void 設定値書き込み処理で例外発生したらエラー()
        {
            var args = GetValidArgs();

            // データ無し
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Returns(default(QH_APPSETTINGS_DAT));

            // 新規追加でエラー
            _appSettingsRepo.Setup(m => m.InsertEntity(It.IsAny<QH_APPSETTINGS_DAT>())).Throws(new Exception());

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アプリ設定書き込み処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 設定値新規追加で正常に終了する()
        {
            var args = GetValidArgs();

            // データ無し
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Returns(default(QH_APPSETTINGS_DAT));
            
            _appSettingsRepo.Setup(m => m.InsertEntity(It.IsAny<QH_APPSETTINGS_DAT>())).Callback((QH_APPSETTINGS_DAT e) => 
            {
                // Json値が反映されている
                e.VALUE.Is(args.SettingsJson);
            });

            var results = _worker.Write(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 新規追加処理が実行された
            _appSettingsRepo.Verify(m => m.InsertEntity(It.IsAny<QH_APPSETTINGS_DAT>()), Times.Once);
        }

        [TestMethod]
        public void 設定値更新で正常に終了する()
        {
            var args = GetValidArgs();

            // データあり
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Returns(new QH_APPSETTINGS_DAT 
            { 
                VALUE = "{}"
            });

            _appSettingsRepo.Setup(m => m.UpdateEntity(It.IsAny<QH_APPSETTINGS_DAT>())).Callback((QH_APPSETTINGS_DAT e) =>
            {
                // Json値が変更されている
                e.VALUE.Is(args.SettingsJson);
            });

            var results = _worker.Write(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 更新処理が実行された
            _appSettingsRepo.Verify(m => m.UpdateEntity(It.IsAny<QH_APPSETTINGS_DAT>()), Times.Once);
        }

        QoAppSettingsWriteApiArgs GetValidArgs()
        {
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
                        RequireNotifyMail = false,
                        PhoneNumber = "09099998888",
                        RequireNotifyPhone = true
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

            return new QoAppSettingsWriteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                SettingsJson = json
            };
        }
    }
}
