using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class SymptomFixture
    {
        ISymptomRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new SymptomRepository();
        }

        [TestMethod]
        public void 新規登録()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");

            var symptoms = new List<QsDbSymptomTypeEnum>
            {
                QsDbSymptomTypeEnum.ChronicFatigue,
                QsDbSymptomTypeEnum.Other
            };

            var json = new QsJsonSerializer().Serialize(symptoms);

            var entity = new QH_SYMPTOM_DAT
            {
                ID = Guid.NewGuid(),
                ACCOUNTKEY = accountKey,
                RECORDDATE = new DateTime(2023, 12, 22, 10, 10, 15),
                LINKAGESYSTEMNO = 47016,
                SYMPTOMS = json,
                OTHERDETAIL = "とにかくしんどい",
                MEMO = "特になし"
            };

            _repo.InsertEntity(entity);
        }

        [TestMethod]
        public void 主キーによる取得()
        {
            var id = Guid.Parse("33842b9b-8cd1-41d8-8e71-34756fc87170");
            var ret = _repo.ReadEntity(id);
        }

        [TestMethod]
        public void 更新()
        {
            var id = Guid.Parse("33842b9b-8cd1-41d8-8e71-34756fc87170");
            var entity = _repo.ReadEntity(id);

            var symptoms  = new QsJsonSerializer().Deserialize<List<QsDbSymptomTypeEnum>>(entity.SYMPTOMS);

            symptoms.Add(QsDbSymptomTypeEnum.LowerLimbsFatigue);

            entity.SYMPTOMS = new QsJsonSerializer().Serialize(symptoms);
            entity.OTHERDETAIL = "その他更新";
            entity.MEMO = "更新したよ";

            _repo.UpdateEntity(entity);
        }

        [TestMethod]
        public void 論理削除()
        {
            var id = Guid.Parse("74e9111b-3f03-40b7-8d1d-243143fb48fd");
            var entity = _repo.ReadEntity(id);

            _repo.DeleteEntity(entity);
        }

        [TestMethod]
        public void 物理削除()
        {
            // 実行注意
            var id = Guid.Parse("74e9111b-3f03-40b7-8d1d-243143fb48fd");
            _repo.PhysicalDeleteEntity(id);
        }

        [TestMethod]
        public void 違和感の取得()
        {
            var accountKey = Guid.Parse("EF7259D6-93FC-4F8A-9E7F-88BB24F781FB");

            var ret = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 0, 10, 47016);

            // 10件取得
            var ret1 = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 0, 10, 47016);

            // 20件取得
            var ret2 = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 0, 20, 47016);

            // 11件目以降取得（ページング確認）
            var ret3 = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 10, 10, 47016);

            // 開始日付指定
            var ret4 = _repo.ReadEntities(accountKey, new DateTime(2024, 5, 30, 0, 0, 0), new DateTime(2024, 5, 30, 23, 59, 59), 0, 10, 47016);
        }
    }
}
