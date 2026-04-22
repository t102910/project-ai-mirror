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
    public class TestLinkageWorker
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "xbcXXzuwm9GMF/bu8K9ujA==";

        // 有効なFacilityKeyを指定したら、その連携施設の子施設のリストを返すプライベートメソッドのテスト
        [TestMethod]
        public void GetLinkageNo_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetLinkageNoメソッドを呼び出す
            var privateType = new PrivateType(typeof(LinkageWorker));
            var results = (int)privateType.InvokeStatic("GetLinkageNo", "11dc3f56-5652-4d08-9147-1575c1723edb".TryToValueType(Guid.Empty));
//            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results,int.MinValue); //MinvalueでなければOK
            
        }
        [TestMethod]
        public void GetLinkageNo_Error()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetLinkageNoメソッドを呼び出す
            var privateType = new PrivateType(typeof(LinkageWorker));
            var results = (int)privateType.InvokeStatic("GetLinkageNo", Guid.Empty);
            Assert.AreEqual(results, int.MinValue); //MinvalueでOK
        }

        //診察券と連携登録（２回実行すると重複で失敗する）
        [TestMethod]
        public void WriteLinkagePatientCard_Success()
        {
            Guid actorKey = "034AC78F-7149-481B-939A-1AAC834C79FA".TryToValueType(Guid.Empty);
            Guid authorKey = "034AC78F-7149-481B-939A-1AAC834C79FA".TryToValueType(Guid.Empty);
            int linkageSystemNo = 27004;
            Guid facilityKey = "11dc3f56-5652-4d08-9147-1575c1723edb".TryToValueType(Guid.Empty);
            string patientId = "88888888";
            var privateType = new PrivateType(typeof(LinkageWorker));
            string results = (string)privateType.InvokeStatic("WriteLinkagePatientCard",
                    new object[] { authorKey, actorKey, linkageSystemNo, facilityKey, patientId,int.MinValue,false } );
            Assert.AreEqual(results, string.Empty); 
        }

        //診察券リスト取得
        [TestMethod]
        public void GetPatientCardList_Success()
        {
            Guid actorKey = "034AC78F-7149-481B-939A-1AAC834C79FA".TryToValueType(Guid.Empty);
            Guid authorKey = "034AC78F-7149-481B-939A-1AAC834C79FA".TryToValueType(Guid.Empty);
            byte statusNo = 2;
            var privateType = new PrivateType(typeof(LinkageWorker));
            List<QoApiPatientCardItem> results = (List <QoApiPatientCardItem> )privateType.InvokeStatic("GetPatientCardList", new object[] { authorKey, actorKey, statusNo  });
            Assert.AreNotEqual(results, null);
        }

        //診察券と連携削除  
        [TestMethod]
        public void WriteLinkagePatientCardDelete_Success()
        {
            Guid actorKey = "034AC78F-7149-481B-939A-1AAC834C79FA".TryToValueType(Guid.Empty);
            Guid authorKey = "034AC78F-7149-481B-939A-1AAC834C79FA".TryToValueType(Guid.Empty);
            int linkageSystemNo = 27004;
            Guid facilityKey = "11dc3f56-5652-4d08-9147-1575c1723edb".TryToValueType(Guid.Empty);
            int seq = 1;
            var privateType = new PrivateType(typeof(LinkageWorker));
            string results = (string)privateType.InvokeStatic("WriteLinkagePatientCard",
                    new object[] { authorKey, actorKey, linkageSystemNo, facilityKey, string.Empty ,seq,true });
           
            Assert.AreEqual(results, string.Empty);
        }

    }
}
