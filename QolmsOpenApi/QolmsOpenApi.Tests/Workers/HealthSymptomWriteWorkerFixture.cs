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
    public class HealthSymptomWriteWorkerFixture
    {
        HealthSymptomWriteWorker _worker;
        Mock<ISymptomRepository> _symptomRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initilaize()
        {
            _symptomRepo = new Mock<ISymptomRepository>();
            _worker = new HealthSymptomWriteWorker(_symptomRepo.Object);
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
        public void RecordDateが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.RecordDate = "hoge";

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.RecordDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 症状タイプが不正でエラーとなる()
        {
            var args = GetValidArgs();
            // 存在しないタイプ
            args.Symptoms = new List<int> { 900, 901, 902 };

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("症状の値が不正です").IsTrue();
        }

        [TestMethod]
        public void 症状が未設定の場合はエラーとなる()
        {
            var args = GetValidArgs();
            // 症状未設定
            args.Symptoms = new List<int>();

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("症状が設定されていません").IsTrue();
        }

        [TestMethod]
        public void 書き込み処理で例外が発生するとエラーとなる()
        {
            var args = GetValidArgs();

            // Insert処理で例外を起こす
            _symptomRepo.Setup(m => m.InsertEntity(It.IsAny<QH_SYMPTOM_DAT>())).Throws(new Exception());

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("症状の書き込み処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 新規登録で正常に終了する()
        {
            var args = GetValidArgs();

            _symptomRepo.Setup(m => m.InsertEntity(It.IsAny<QH_SYMPTOM_DAT>())).Callback((QH_SYMPTOM_DAT entity) =>
            {
                // 正しくentityが設定されている
                entity.ACCOUNTKEY.Is(_accountKey);
                entity.RECORDDATE.Is(new DateTime(2023, 12, 22, 10, 30, 15));
                // 連携システム番号に変換された
                entity.LINKAGESYSTEMNO.Is(47016);
                // IDが生成された
                entity.ID.IsNot(Guid.Empty);
                entity.OTHERDETAIL.Is("Detail");
                entity.MEMO.Is("Memo");

                var symptoms = new QsJsonSerializer().Deserialize<List<QsDbSymptomTypeEnum>>(entity.SYMPTOMS);

                // 重複が排除されて3件
                symptoms.Count.Is(3);
                symptoms[0].Is(QsDbSymptomTypeEnum.ShortOfBreath);
                symptoms[1].Is(QsDbSymptomTypeEnum.ChronicFatigue);
                symptoms[2].Is(QsDbSymptomTypeEnum.Other);
            });

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // Empty以外のIDが割り当てられた
            results.Id.IsNot(Guid.Empty);
        }

        [TestMethod]
        public void 更新処理で正常に終了する()
        {
            var args = GetValidArgs();
            args.Id = Guid.NewGuid();

            _symptomRepo.Setup(m => m.UpdateEntity(It.IsAny<QH_SYMPTOM_DAT>())).Callback((QH_SYMPTOM_DAT entity) =>
            {
                // 正しくentityが設定されている
                entity.ACCOUNTKEY.Is(_accountKey);
                entity.RECORDDATE.Is(new DateTime(2023, 12, 22, 10, 30, 15));
                // 連携システム番号に変換された
                entity.LINKAGESYSTEMNO.Is(47016);
                // IDは引数のIDとなっている
                entity.ID.Is(args.Id);
                entity.OTHERDETAIL.Is("Detail");
                entity.MEMO.Is("Memo");

                var symptoms = new QsJsonSerializer().Deserialize<List<QsDbSymptomTypeEnum>>(entity.SYMPTOMS);

                // 重複が排除されて3件
                symptoms.Count.Is(3);
                symptoms[0].Is(QsDbSymptomTypeEnum.ShortOfBreath);
                symptoms[1].Is(QsDbSymptomTypeEnum.ChronicFatigue);
                symptoms[2].Is(QsDbSymptomTypeEnum.Other);
            });

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 更新対象のIDがセットされている
            results.Id.Is(args.Id);
        }

        QoHealthSymptomWriteApiArgs GetValidArgs()
        {
            var symptoms = new List<QsDbSymptomTypeEnum>
            {
                QsDbSymptomTypeEnum.ShortOfBreath,
                QsDbSymptomTypeEnum.ChronicFatigue,
                QsDbSymptomTypeEnum.ShortOfBreath, // 重複させる
                QsDbSymptomTypeEnum.Other
            };
           

            return new QoHealthSymptomWriteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                Id = Guid.Empty,
                Memo = "Memo",
                OtherDetail = "Detail",
                RecordDate = new DateTime(2023,12,22,10,30,15).ToApiDateString(),
                Symptoms = symptoms.ConvertAll(x => (int)x)
            };
        }
    }
}
