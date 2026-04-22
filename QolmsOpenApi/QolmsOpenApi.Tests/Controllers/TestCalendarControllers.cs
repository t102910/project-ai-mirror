using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestCalendarControllers
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "B6C7DT25LMcRYu+RahIoCQ==";

        [TestMethod]
        public void PostEventWrite_Medichine_Success()
        {
            //お薬のイベントWrite
            var now = DateTime.Now;
            var eventSet = new QoApiCalendarEventSetItem() {
                Address = "東京都千代田区",
                Detail = String.Empty,
                FacilityKeyReference = String.Empty,
                Location = new QoApiLocationItem(),
                PhoneNumber = "03-0000-0000",
                //MedicalEvent= new
            };

            var item = new QoApiCalendarEventItem() {
                //LinkageSystemId ="99999999",
                LinkageSystemNo = "27001",
                EventDate = now.ToApiDateString(),
                Sequence = "-1",
                StartDate = now.ToApiDateString(),
                EndDate = now.ToApiDateString(),
                EventType = QsDbEventTypeEnum.System.ToString("d"),
                Name = "テストイベント",
                AllDayFlag = Boolean.TrueString,
                FinishFlag = Boolean.TrueString,
                NoticeFlag = Boolean.FalseString,
                OpenFlag = Boolean.FalseString,
                Importance = "1",
                CustomStampNo = String.Empty,
                EventSetTypeName = nameof(QhMedicineEventSetOfJson),
                EventSet = eventSet,
                CategoryNoN = new List<string>() { "1" },
                SystemTagNoN = new List<string>() { "1" },
                CustomTagNoN = new List<string>() { "1" },
                DeleteFlag = Boolean.FalseString
            };

            var args = new QoCalendarEventWriteApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                Executor =EXECUTOR,
                ExecutorName = EXECUTORNAME,
                ApiType = QoApiTypeEnum.CalendarEventWrite.ToString(),
                ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn",  //対象アカウント アプリならJWTから
                Event = item
            };
            
            var controller = new CalendarController();
            var result = controller.PostEventWrite(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            
        }
        [TestMethod]
        public void PostEventWrite_Medical_Success()
        {
            //通院のイベントWrite
            var now = DateTime.Now;
            var eventSet = new QoApiCalendarEventSetItem()
            {
                Address = "東京都千代田区",
                Detail = String.Empty,
                FacilityKeyReference = new QsCrypt(QsCryptTypeEnum.QolmsWeb).EncryptString("11dc3f5656524d0891471575c1723edb"),
                Location = new QoApiLocationItem(),
                PhoneNumber = "03-0000-0000",
                MedicalEvent= new QoApiMedicalEventItem() { 
                    MedicalEventType=QsDbCalendarMedicalEventTypeEnum.MedicalTreatment.ToString("d"),
                    DoctorName ="医師",
                }
            };

            var item = new QoApiCalendarEventItem()
            {
                LinkageSystemId = "00000051",
                LinkageSystemNo = "27004",
                EventDate = now.AddDays(7).ToApiDateString(),
                Sequence = "-1",
                StartDate = now.ToApiDateString(),
                EndDate = now.ToApiDateString(),
                EventType = QsDbEventTypeEnum.System.ToString("d"),
                Name = "予約",
                AllDayFlag = Boolean.TrueString,
                FinishFlag = Boolean.TrueString,
                NoticeFlag = Boolean.FalseString,
                OpenFlag = Boolean.FalseString,
                Importance = "1",
                CustomStampNo = String.Empty,
                EventSetTypeName = nameof(QhMedicalEventSetOfJson),
                EventSet = eventSet,
                CategoryNoN = new List<string>() { "11" },      //通院
                SystemTagNoN = new List<string>() { "3" },      //診察
               // CustomTagNoN = new List<string>() { "1" },
                DeleteFlag = Boolean.FalseString
            };

            var args = new QoCalendarEventWriteApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                Executor = EXECUTOR,
                ExecutorName = EXECUTORNAME,
                ApiType = QoApiTypeEnum.CalendarEventWrite.ToString(),
                ActorKey = string.Empty ,  //対象アカウント は、LinkageSystemIDからひいてもらう
                Event = item
            };

            var controller = new CalendarController();
            var result = controller.PostEventWrite(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);

        }
        
        [TestMethod]
        public void PostEventRead_Medicine_Success()
        {
            //お薬イベントRead
            var now = DateTime.Now;
            var args = new QoCalendarEventReadApiArgs()
            {
                ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn",  //対象アカウント アプリならJWTから,
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                Executor = EXECUTOR,
                ExecutorName = EXECUTORNAME,
                ApiType = QoApiTypeEnum.CalendarEventRead.ToString(),
                LinkageSystemNo = "27001",
                StartDate = now.ToApiDateString(),
                EndDate = now.ToApiDateString(),
                CategoryNoN = new List<string>() { "1" },
                SystemTagNoN = new List<string>() { "1" },
                CustomTagNoN = new List<string>() { "1" },
                FinishStateType = "3"
            };
            var controller = new CalendarController();
            var result = controller.PostEventRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }

        [TestMethod]
        public void PostEventReadFromJob_Medical_Success()
        {
            //通院イベントRead
            var now = DateTime.Now;
            var args = new QoCalendarEventReadApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.TisAndroidApp.ToString(),
                Executor = EXECUTOR,
                ExecutorName = EXECUTORNAME,
                ApiType = QoApiTypeEnum.CalendarEventRead.ToString(),
                LinkageSystemNo = "27004",
                LinkageSystemId= "00000051",
                StartDate = now.ToApiDateString(),
                EndDate = now.AddDays(30).ToApiDateString(),
                CategoryNoN = new List<string>() { "11" },
                SystemTagNoN = new List<string>() { "3" },
                FinishStateType = "3"
            };
                //ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn",  //対象アカウント アプリならJWTから,
            var controller = new CalendarController();
            var result = controller.PostEventRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }
        [TestMethod]
        public void PostEventReadFromJob_Medical_Success_中村さん指摘不具合確認用()
        {
            //通院イベントRead
            var now = DateTime.Now;
            var args = new QoCalendarEventReadApiArgs()
            {
                ActorKey = "vESvTpAlpnUzgj5HwRl5KwE4V37DAFilsd+Sjj/lT37zd/0uX1lvNZ6VhDgrwmAE",  //対象アカウント アプリならJWTから,
                ExecuteSystemType = QsApiSystemTypeEnum.TisAndroidApp.ToString(),
                Executor = EXECUTOR,
                ExecutorName = EXECUTORNAME,
                ApiType = QoApiTypeEnum.CalendarEventRead.ToString(),
                LinkageSystemNo = "27004",
                StartDate = now.AddMonths(-3).ToApiDateString(),
                EndDate = now.AddDays(30).ToApiDateString(),
                CategoryNoN = new List<string>() { "11" },
                SystemTagNoN = new List<string>() { "3","4","1" },
                FinishStateType = "2"
            };
            //ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn",  //対象アカウント アプリならJWTから,
            var controller = new CalendarController();
            var result = controller.PostEventRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }
        [TestMethod]
        public void PostEventReadFromMobileApp_Medical_Success()
        {
            //通院イベントRead
            var now = DateTime.Now;
            var args = new QoCalendarEventReadApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.TisAndroidApp.ToString(),
                Executor = EXECUTOR,
                ExecutorName = EXECUTORNAME,
                ApiType = QoApiTypeEnum.CalendarEventRead.ToString(),
                StartDate = now.ToApiDateString(),
                EndDate = now.AddDays(30).ToApiDateString(),
                CategoryNoN = new List<string>() { "11" },
                SystemTagNoN = new List<string>() { "3" },
                FinishStateType = "3",
                ActorKey= "GDQatN2U5kLXBHmUMMEBrljx1Hdhs0q+ZdBexHjZeM90UqXaYklX4R74srppjAc6"   //アプリからはトークンでくる
            };
            //ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn",  //対象アカウント アプリならJWTから,
            var controller = new CalendarController();
            var result = controller.PostEventRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }
    }
}
