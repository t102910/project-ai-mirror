using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
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
    public class DiaryStatusWorkerFixture
    {
        Mock<IEventRepository> _eventRepo;
        DiaryStatusWorker _worker;

        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _eventRepo = new Mock<IEventRepository>();
            _worker = new DiaryStatusWorker(_eventRepo.Object);
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
        public void いいね数取得処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);
            // 例外発生
            _eventRepo.Setup(m => m.GetLikeCount(_accountKey)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("いいね数取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 気分取得処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _eventRepo.Setup(m => m.GetDiaryPostCount(_accountKey)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("日記投稿数情報取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常終了_気分が良い()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 気分・いいね数が正しく設定されている
            results.StatusItem.FeelingType.Is(QoApiDiaryFeelingTypeEnum.Good);
            results.StatusItem.TodayLike.Is(30);
            results.StatusItem.TotalLike.Is(300);
        }

        [TestMethod]
        public void 正常終了_気分普通()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _eventRepo.Setup(m => m.GetDiaryPostCount(_accountKey)).Returns(new QH_EVENT_DIARY_POST_VIEW
            {
                TODAY = 0,
                DAY1TO2 = 2,
                DAY3TO6 = 3
            });

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 気分・いいね数が正しく設定されている
            results.StatusItem.FeelingType.Is(QoApiDiaryFeelingTypeEnum.Normal);
            results.StatusItem.TodayLike.Is(30);
            results.StatusItem.TotalLike.Is(300);
        }

        [TestMethod]
        public void 正常終了_気分が優れない()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _eventRepo.Setup(m => m.GetDiaryPostCount(_accountKey)).Returns(new QH_EVENT_DIARY_POST_VIEW
            {
                TODAY = 0,
                DAY1TO2 = 0,
                DAY3TO6 = 3
            });

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 気分・いいね数が正しく設定されている
            results.StatusItem.FeelingType.Is(QoApiDiaryFeelingTypeEnum.NotGood);
            results.StatusItem.TodayLike.Is(30);
            results.StatusItem.TotalLike.Is(300);
        }

        [TestMethod]
        public void 正常終了_カウントなしは通常扱い()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _eventRepo.Setup(m => m.GetDiaryPostCount(_accountKey)).Returns(new QH_EVENT_DIARY_POST_VIEW
            {
                TODAY = 0,
                DAY1TO2 = 0,
                DAY3TO6 = 0
            });

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 気分・いいね数が正しく設定されている
            results.StatusItem.FeelingType.Is(QoApiDiaryFeelingTypeEnum.Normal);
            results.StatusItem.TodayLike.Is(30);
            results.StatusItem.TotalLike.Is(300);
        }

        void SetupValidMethods(QoDiaryStatusApiArgs args)
        {
            _eventRepo.Setup(m => m.GetLikeCount(_accountKey)).Returns(new QH_EVENTREACTION_LIKE_VIEW
            {
                TOTAL = 300,
                TODAY = 30
            });

            _eventRepo.Setup(m => m.GetDiaryPostCount(_accountKey)).Returns(new QH_EVENT_DIARY_POST_VIEW
            {
                TODAY = 1,
                DAY1TO2 = 2,
                DAY3TO6 = 3
            });
        }

        QoDiaryStatusApiArgs GetValidArgs()
        {
            return new QoDiaryStatusApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
            };
        }
    }
}
