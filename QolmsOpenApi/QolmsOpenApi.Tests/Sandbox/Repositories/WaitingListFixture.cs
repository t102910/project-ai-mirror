using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class WaitingListFixture
    {
        IWaitingRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new WaitingRepository();
        }

        [TestMethod]
        public void 待ち状態リストの取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");

            var ret = _repo.ReadLatestWaitingList(accountKey, facilityKey, new DateTime(2022, 9, 14), 0);
        }

        [TestMethod]
        public void 待ち人数の取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");

            var ret = _repo.GetWaitingCount(accountKey, facilityKey, new DateTime(2022, 9, 14), 3);
        }

        [TestMethod()]
        public void 待ち人数の取得_優先度未指定なら予約時刻優先()
        {
            var accountKey = Guid.Parse("bf7ea295-c64d-4ebf-b0ba-05efa6d78881");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");

            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();

            var ret = _repo.GetWaitingOrderNumber(27004, "03", "01", config, new DateTime(2025, 05, 30), "99990001", QsDbWaitingPriorityTypeEnum.None, 0);

            // 予約時刻は遅い方なので、2番目（返却値1）
            ret.Item1.Is(1);
        }

        [TestMethod()]
        public void 待ち人数の取得_予約時刻優先()
        {
            var accountKey = Guid.Parse("bf7ea295-c64d-4ebf-b0ba-05efa6d78881");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");

            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();

            var ret = _repo.GetWaitingOrderNumber(27004, "03", "01", config, new DateTime(2025, 05, 30), "99990001", QsDbWaitingPriorityTypeEnum.PriorityByReserve, 0);

            // 予約時刻は遅い方なので、2番目（返却値1）
            ret.Item1.Is(1);
        }

        [TestMethod()]
        public void 待ち人数の取得_受付時刻優先()
        {
            var accountKey = Guid.Parse("bf7ea295-c64d-4ebf-b0ba-05efa6d78881");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");

            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();

            var ret = _repo.GetWaitingOrderNumber(27004, "03", "01", config, new DateTime(2025, 05, 30), "99990001", QsDbWaitingPriorityTypeEnum.PriorityByArrival, 0);

            // 受付時刻は早い方なので、1番目（返却値0）
            ret.Item1.Is(0);
        }

        [TestMethod]
        public void 待ち人数リストの取得()
        {
            var ret = _repo.GetWaitingOrderListEntity(27004, "01", "9801",10, true, new DateTime(2023, 11, 6, 15, 0, 0));

            var ret2 = _repo.GetWaitingOrderListEntity(27004, "01", "9801", 10, false, new DateTime(2023, 11, 6, 15, 0, 0));
        }

        [TestMethod]
        public void 順番待ちの書き込み()
        {
            var facilityKey = Guid.NewGuid();
            var entity = new QH_WAITINGLIST_DAT
            {
                FACILITYKEY = facilityKey,
                WAITINGDATE = DateTime.Today,
                DATATYPE = 1,
                SEQUENCE = 0,
                LINKAGESYSTEMID = "99999006",
                LINKAGESYSTEMNO = 28001,
                DEPARTMENTCODE = "1",
                STATUSTYPE = 10,
                RECEPTIONDATE = DateTime.Now,
                RESERVATIONDATE = DateTime.Now,
                RECEPTIONNO = "100",
                RESERVATIONNO = "111",
                FOREIGNKEY = "9999009",
                VALUE = "",
                DELETEFLAG = false,
                CREATEDDATE = DateTime.Now,
                UPDATEDDATE = DateTime.Now
            };
            var ret = _repo.WriteList(new List<QH_WAITINGLIST_DAT> { entity });
        }

        [TestMethod]
        public void  新施設設定取得()
        {
            var entity = _repo.GetMedicalDepartmentAppConfig(27004, "01");
            var value = _repo.GetMedicalDepartmentAppValue(entity);

            // 存在しないレコード（エラーにならずに規定値が返る）
            var notFound = _repo.GetMedicalDepartmentAppConfig(27004, "999");
        }
    }
}
