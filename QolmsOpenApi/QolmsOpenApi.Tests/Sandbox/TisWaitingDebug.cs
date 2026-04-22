using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class TisWaitingDebug
    {
        [TestMethod]
        public void 外部キーの最大値を取得()
        {
            var reader = new DbWaitingListDebugReaderCore();
            var ret = reader.GetMaxDebugForeignKey();
        }

        [TestMethod]
        public void 受付番号の最大値を取得()
        {
            var reader = new DbWaitingListDebugReaderCore();
            var ret = reader.GetMaxReceptionNoInDay(new DateTime(2022,12,10));
        }

        [TestMethod]
        public void 待ち受けテーブルに書き込む()
        {
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var args = new QoTisWaitingListDebugWriteApiArgs
            {
                ApiType = QoApiTypeEnum.TisWaitingListDebugWrite.ToString(),
                DataType = "1",
                DepartmentName = "内科",
                DoctorName = "医者太郎",
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                LinkageSystemId = "99999001",
                LinkageSystemNo = "27004",
                //ReceptionTime = "1000",
                WaitingDate = new DateTime(2022,12,15).ToApiDateString(),
                //ReservationTime = "1030",
                StatusType = "10",
                ForeignKey = "",    
                ReceptionNo = "0200"
            };

            var result = TisWorker.WaitingListDebugWrite(args);
        }
    }
}
