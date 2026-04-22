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
    public class HealthRecordAlertFixture
    {
        IHealthRecordAlertRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new HealthRecordAlertRepository();
        }

        [TestMethod]
        public void 次のAlertNo取得()
        {
            var ret = _repo.GetNewAlertNo();
        }

        [TestMethod]
        public void 主キー指定で取得()
        {
            var accountKey = Guid.Parse("dda5c3df-a868-49b4-a469-a9fe96be5753");
            var recordDate = new DateTime(2022, 8, 16,0,0,0);

            var ret = _repo.ReadEntity(accountKey, recordDate, 8, 47011);
        }

        [TestMethod]
        public void AlertNo指定で取得()
        {
            var ret = _repo.ReadEntity(7);
        }

        [TestMethod]
        public void 新規追加()
        {
            var accountKey = Guid.Parse("dda5c3df-a868-49b4-a469-a9fe96be5753");

            using (var tran = new QoTransaction())
            {
                var no = _repo.GetNewAlertNo();
                _repo.InsertEntity(new QH_HEALTHRECORDALERT_DAT
                {
                    ACCOUNTKEY = accountKey,
                    RECORDDATE = new DateTime(2023, 11, 3, 12, 15, 0),
                    VITALTYPE = 8,
                    LINKAGESYSTEMNO = 47016,
                    VALUE1 = 100,
                    VALUE2 = -1,
                    ALERTNO = no,
                    ABNORMALTYPE = 2,
                    ALERTTYPE = 1,
                    EMERGENCYTYPE = 2,
                    MESSAGE = "hoge"
                });

                tran.Commit();
            }           
        }

        [TestMethod]
        public void 更新()
        {
            var entity = _repo.ReadEntity(8);

            entity.VALUE1 = 45;
            entity.ABNORMALTYPE = 1;

            _repo.UpdateEntity(entity);
        }

        [TestMethod]
        public void 削除()
        {
            var entity = _repo.ReadEntity(9);

            _repo.DeleteEntity(entity);
        }

        [TestMethod]
        public void 物理削除()
        {
            var accountKey = Guid.Parse("dda5c3df-a868-49b4-a469-a9fe96be5753");
            _repo.PhysicalDeleteEntity(accountKey, new DateTime(2023, 11, 1, 12, 15, 0), 8, 47016);
        }

        [TestMethod]
        public void アラート症状VIEWの取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var ret = _repo.ReadAlertSymptomView(accountKey, 47016, DateTime.Now.AddDays(-60), DateTime.Now);
        }

        [TestMethod]
        public void アラートの取得()
        {
            var accountKey = Guid.Parse("EF7259D6-93FC-4F8A-9E7F-88BB24F781FB");

            // 10件取得
            var ret1 = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 8, 0, 10, 47016);

            // 20件取得
            var ret2 = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 8, 0, 20, 47016);

            // 11件目以降取得（ページング確認）
            var ret3 = _repo.ReadEntities(accountKey, DateTime.MinValue, new DateTime(2024, 5, 30, 23, 59, 59), 8, 10, 10, 47016);

            // 開始日付指定
            var ret4 = _repo.ReadEntities(accountKey, new DateTime(2024, 5, 31, 0, 0, 0), new DateTime(2024, 5, 31, 23, 59, 59), 8, 0, 10, 47016);
        }
    }
}
