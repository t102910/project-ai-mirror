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
using MGF.QOLMS.QolmsCryptV1;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestMasterWorker
    {
        // 有効なLinkageSystemNoを指定したら、その連携施設の子施設のリストを返すプライベートメソッドのテスト
        [TestMethod]
        public void GetFacilityList_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (List<QH_FACILITY_MST>)privateType.InvokeStatic("GetFacilityList", 27003, DateTime.MinValue);
            
            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.Count,0);  //0 でなければOK
            foreach (var item in results)
            {
                Assert.AreNotEqual(item.FACILITYKEY, Guid.Empty); //EmptyでなければOK
            }
        }
        [TestMethod]
        public void GetFacilityList_0()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (List<QH_FACILITY_MST>)privateType.InvokeStatic("GetFacilityList", int.MinValue, DateTime.MinValue);
            Assert.IsNull(results);            //nullでOK
        }

        //施設の電話情報
        [TestMethod]
        public void GetFacilityPhoneList_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (List<QoApiFacilityPhoneListItem>)privateType.InvokeStatic("GetFacilityPhoneList", Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));

            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.Count, 0);  //0 でなければOK
            foreach (var item in results)
            {
                Assert.AreNotEqual(item.PhoneNo, string.Empty); //EmptyでなければOK
                Assert.AreNotEqual(item.ReceptionTimeN, null); //nullでなければOK
                foreach (var time in item.ReceptionTimeN)
                {
                    Assert.AreNotEqual(time.TimeText , string.Empty); //EmptyでなければOK
                    Assert.AreNotEqual(item.Title , string.Empty); //EmptyでなければOK
                }
            }
        }
        //施設のURL情報
        [TestMethod]
        public void GetFacilityUrlList_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (List<QoApiFacilityUriListItem>)privateType.InvokeStatic("GetFacilityUrlList", Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));

            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.Count, 0);  //0 でなければOK
            foreach (var item in results)
            {
                Assert.AreNotEqual(item.Uri, string.Empty); //nullでなければOK
            }
        }
        //施設の診療科情報
        [TestMethod]
        public void GetMedicalDepartmentList_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (List<QoApiMedicalDepartmentItem>)privateType.InvokeStatic("GetMedicalDepartmentList", Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));

            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.Count, 0);  //0 でなければOK
            foreach (var item in results)
            {
                Assert.AreNotEqual(item.DepartmentName, string.Empty); //nullでなければOK
                Assert.AreNotEqual(item.DepartmentNo, string.Empty); //nullでなければOK
                Assert.AreNotEqual(item.LocalCode, string.Empty); //nullでなければOK
                Assert.AreNotEqual(item.LocalName, string.Empty); //nullでなければOK
            }
        }
        //施設の位置情報
        [TestMethod]
        public void GetFacilityAddition_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (QoApiLocationItem)privateType.InvokeStatic("GetFacilityAddition", Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));

            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.Latitude, string.Empty);  //EmptyでなければOK
            Assert.AreNotEqual(results.Longitude, string.Empty);  //EmptyでなければOK

        }
        //施設の画像情報
        [TestMethod]
        public void GetFacilityImageKey_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetTotalValueメソッドを呼び出す
            var privateType = new PrivateType(typeof(MasterWorker));
            var results = (List<QoApiFileKeyItem>)privateType.InvokeStatic("GetFacilityImageKey", Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));

            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.Count ,0);  //EmptyでなければOK
            Assert.AreNotEqual(results[0].FileKeyReference , string.Empty);  //EmptyでなければOK
            
        }
        [TestMethod]
        public void temp()
        {
            var FacilityKeyReference = new QsCrypt(QsCryptTypeEnum.QolmsWeb).EncryptString("4D0E31C37DDE4D9BA68E8149E1738B36");
            FacilityKeyReference.IsNotNull<string>();

            //json作るのしんどいからここで
            var json = new QhFacilityContactInformationCommentSetOfJson()
            {
                Comment = "平日の昼間しか受け付けません",
                Title = "予約受付センター",
                ReceptionTimeN = new List<QhFacilityContactInformationReceptionTimeOfJson>
                {
                    new QhFacilityContactInformationReceptionTimeOfJson() { Tag = "平日", TimeText = "9:00-12:00 / 14:00-17:00" },
                    new QhFacilityContactInformationReceptionTimeOfJson() { Tag = "土曜", TimeText = "9:00-12:00" }
                }
            };
            var str = new QsJsonSerializer().Serialize(json);
            str.IsNotNull<string>();
        }
        
    }
}
