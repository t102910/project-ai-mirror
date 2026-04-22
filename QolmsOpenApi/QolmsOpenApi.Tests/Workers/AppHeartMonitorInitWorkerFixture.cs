using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppHeartMonitorInitWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        Mock<IAppSettingsRepository> _settingsRepo;

        AppHeartMonitorInitWorker _worker;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _settingsRepo = new Mock<IAppSettingsRepository>();

            _worker = new AppHeartMonitorInitWorker(_accountRepo.Object, _settingsRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.GetInitData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void アプリ設定の取得に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 設定取得に失敗
            _settingsRepo.Setup(m => m.ReadEntity(_accountKey, It.IsAny<int>())).Throws(new Exception());

            var results = _worker.GetInitData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("アプリ設定取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void アカウント情報が存在しない場合はエラーとなる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // アカウント情報がnull
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(default(QH_ACCOUNTINDEX_DAT));

            var results = _worker.GetInitData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("アカウントが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void アカウント情報取得で例外が発生したらエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // アカウント情報取得で例外
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Throws(new Exception());

            var results = _worker.GetInitData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 初期データが正常に取得できる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var results = _worker.GetInitData(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 生年月日が取得できている
            results.Birthday.Is(new DateTime(1999, 3, 3).ToApiDateString());
            // 設定Jsonが取得できている
            // 詳細な検証はAppSettingReadWorkerFixtureで行う）
            results.SettingsJson.IsNotNull();          
        }

        void SetUpValidMethods(QoAppHeartMonitorInitApiArgs args)
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

            var entity = new QH_APPSETTINGS_DAT
            {
                ACCOUNTKEY = _accountKey,
                APPTYPE = (int)QsDbApplicationTypeEnum.HeartMonitorApp,
                VALUE = json
            };

            // 設定を返す
            _settingsRepo.Setup(m => m.ReadEntity(_accountKey, (int)QsDbApplicationTypeEnum.HeartMonitorApp)).Returns(entity);

            // アカウント情報を返す
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = new DateTime(1999, 3, 3)
            });
        }

        QoAppHeartMonitorInitApiArgs GetValidArgs()
        {
            return new QoAppHeartMonitorInitApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
            };
        }
    }
}
