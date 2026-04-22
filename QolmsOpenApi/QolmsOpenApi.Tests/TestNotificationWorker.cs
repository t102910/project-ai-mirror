using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestNotificationWorker
    {
        
        // プライベートメソッドのテスト
        [TestMethod]
        public void Write_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetLinkageNoメソッドを呼び出す
            var privateType = new PrivateType(typeof(NotificationWorker));
            long noticeNo = long.MinValue;
            string title = "おしらせてすと";
            string contents = "XXXXXXXXXXXXXXXXXXXXXXXXX";
            byte categoryNo = 1;
            byte priorityNo=3;
            byte fromSystemType = 7;
            byte toSystemType = 100;
            Guid facilitykey = System.Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB");
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(1);
            Guid authorKey = Guid.Parse("034AC78F-7149-481B-939A-1AAC834C79FA");
            
            var results = (long )privateType.InvokeStatic("Write",  noticeNo, title,  contents,  categoryNo,  priorityNo,
             fromSystemType,  toSystemType,  facilitykey,  startDate,  endDate,
             authorKey) ;
//            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results,long.MinValue ); 
            
        }

        //Read
        [TestMethod]
        public void Read_Success()
        {
            QoNotificationReadApiArgs args = new QoNotificationReadApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.TisAndroidApp.ToString(),
                FromDate = DateTime.Now.ToApiDateString(),
                ToDate = DateTime.Now.ToApiDateString(),
                PageIndex = "0",
                PageSize = "20",
                ActorKey = "C2827437-1D4C-40D9-9D3D-961E21467C5C"
            };
            QoNotificationReadApiResults results = NotificationWorker.Read(args);
            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.IsSuccess.TryToValueType(false), false);
        }

        [TestMethod]
        public void ReadPersonal_Success()
        {
            QoNotificationPersonalReadApiArgs args = new QoNotificationPersonalReadApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.TisiOSApp.ToString(),
                NoticeNo = "35",
                ActorKey = "C2827437-1D4C-40D9-9D3D-961E21467C5C"
            };
            QoNotificationPersonalReadApiResults results = NotificationWorker.PersonalRead(args);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(results.IsSuccess.TryToValueType(false), false);
        }
        //Read
        [TestMethod]
        public void ImageRead_Success()
        {
            QoNotificationImageReadApiArgs args = new QoNotificationImageReadApiArgs()
            {
                ExecuteSystemType = QsApiSystemTypeEnum.TisAndroidApp.ToString(),
                FileKeyReference = "f83f451e60800e9e6aa209e114d21aa163348bb4ef8b9fbfd3ec804c735673d4e4adeb2459f67422c798ad11704a2529",
                FileType ="1",
                ActorKey = "034AC78F-7149-481B-939A-1AAC834C79FA"
            };
            QoNotificationImageReadApiResults results = NotificationWorker.ImageRead(args);
            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.IsSuccess.TryToValueType(false), false);
        }
    }
}
