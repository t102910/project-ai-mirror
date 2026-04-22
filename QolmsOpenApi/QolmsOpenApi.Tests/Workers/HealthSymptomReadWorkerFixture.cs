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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthSymptomReadWorkerFixture
    {
        HealthSymptomReadWorker _worker;
        Mock<ISymptomRepository> _symptomRepository;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _symptomRepository = new Mock<ISymptomRepository>();
            _worker = new HealthSymptomReadWorker(_symptomRepository.Object);
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
        public void データ取得で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();

            // 例外を発生させる
            _symptomRepository.Setup(m => m.ReadEntities(_accountKey, DateTime.MinValue, DateTime.Today.AddDays(1), 0, 10, 47016)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("症状のリストの取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常にデータを取得できる()
        {
            var args = GetValidArgs();

            var entityList = new List<QH_SYMPTOM_DAT>
            {
                new QH_SYMPTOM_DAT
                {
                    RECORDDATE = new DateTime(2024,1,9,10,30,0),
                    SYMPTOMS = "[1,2]",
                    OTHERDETAIL = "JIYZl/3NNSx7T6gFbjfmsw3u5GF1yvdiiqz0YWaSJSY=",
                    MEMO = "P3oCOjuKnUnJlcmS8DMswg=="
                },
                new QH_SYMPTOM_DAT
                {
                    RECORDDATE = new DateTime(2024,1,8,11,30,0),
                    SYMPTOMS = "[3]",
                    OTHERDETAIL = "JIYZl/3NNSx7T6gFbjfms72T96Kqc69+PaMm2Fl/7kw=",
                    MEMO = "SEvd/tkXcedTTwHXHve7VA=="
                },
            };

            // DBよりEntityList取得(ToDateは未指定なので明日が指定される)
            _symptomRepository.Setup(m => m.ReadEntities(_accountKey, DateTime.MinValue, DateTime.Today.AddDays(1), 0, 10, 47016)).Returns(entityList);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 2件取得できている
            results.ItemList.Count.Is(2);
            var rec1 = results.ItemList[0];

            // レコード1が正しく変換されている
            rec1.RecordDate.Is(entityList[0].RECORDDATE.ToApiDateString());
            rec1.Symptoms[0].Is(1);
            rec1.Symptoms[1].Is(2);
            rec1.OtherDetail.Is("その他の症状１");
            rec1.Memo.Is("メモ１");

            var rec2 = results.ItemList[1];

            // レコード2が正しく変換されている
            rec2.RecordDate.Is(entityList[1].RECORDDATE.ToApiDateString());
            rec2.Symptoms[0].Is(3);
            rec2.OtherDetail.Is("その他の症状２");
            rec2.Memo.Is("メモ２");
        }

        [TestMethod]
        public void 正常に取得できる_開始日終了日指定の場合()
        {
            var args = GetValidArgs();
            // 明示的に開始日終了日を指定する
            var toDate = new DateTime(2024, 5, 28, 10, 30, 0);
            var fromDate = new DateTime(2024, 5, 27, 10, 30, 0);
            args.ToDate = toDate.ToApiDateString();
            args.FromDate = fromDate.ToApiDateString();

            // DBよりEntityList取得
            _symptomRepository.Setup(m => m.ReadEntities(_accountKey, fromDate, toDate, 0, 10, 47016)).Returns(new List<QH_SYMPTOM_DAT>());

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 0件
            results.ItemList.Count.Is(0);

            // DB取得処理が正しく呼ばれた
            _symptomRepository.Verify(m => m.ReadEntities(_accountKey, fromDate, toDate, 0, 10, 47016), Times.Once);
        }

        QoHealthSymptomReadApiArgs GetValidArgs()
        {
            return new QoHealthSymptomReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitor",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                Offset = 0,
                Fetch = 10,
            };
        }
    }
}
