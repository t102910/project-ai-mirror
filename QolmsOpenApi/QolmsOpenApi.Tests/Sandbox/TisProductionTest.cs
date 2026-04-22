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
    public class TisProductionTest
    {
        [TestMethod]
        public void 受診履歴書き込み()
        {
            var eventDate = new DateTime(2022,10,1);
            var start = eventDate.AddHours(10);
            var end = start.AddHours(1);
            var facilityKeyRef = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb").ToEncrypedReference();

            var eventSet = new QoApiCalendarEventSetItem
            {
                Address = "",
                Detail = "メモメモ",
                FacilityKeyReference = facilityKeyRef,
                Location = new QoApiLocationItem(),
                MedicalEvent = new QoApiMedicalEventItem()
                {
                    MedicalEventType = "1",
                    DepartmentName = "呼吸器内科",
                },
            };

            var item = new QoApiCalendarEventItem
            {
                LinkageSystemId = "99999001",
                LinkageSystemNo = "27004",
                EventDate = eventDate.ToApiDateString(),
                Sequence = "-1",
                StartDate = start.ToApiDateString(),
                EndDate = end.ToApiDateString(),
                EventType = "1",
                Name = "受診履歴",
                AllDayFlag = bool.FalseString,
                FinishFlag = bool.TrueString,
                NoticeFlag = bool.FalseString,
                OpenFlag = bool.FalseString,
                Importance = "1",
                CustomStampNo = string.Empty,
                EventSetTypeName = "QhMedicalEventSetOfJson",
                EventSet = eventSet,
                CategoryNoN = new List<string>() { "11" },      //通院
                SystemTagNoN = new List<string> { "1", "3", "4" },
                DeleteFlag = bool.FalseString
            };

            var args = new QoCalendarEventWriteApiArgs
            {
                ApiType = QoApiTypeEnum.CalendarEventWrite.ToString(),
                Event = item,
                Executor = "FFB6CAB6-0040-4101-0000-000000027003"
            };







            var ret = CalendarWorker.EventWrite(args);
        }
    }
}
