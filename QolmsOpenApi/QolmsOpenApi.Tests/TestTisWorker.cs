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
    public class TestTisWorker
    {
        
        // 有効な期限チェックの結果を返すプライベートメソッドのテスト
        [TestMethod]
        public void CheckExpiration_Success()
        {
            // staticなprivateメソッドのテストはPrivateTypeを作成して、GetLinkageNoメソッドを呼び出す
            var privateType = new PrivateType(typeof(TisWorker));
            var results = (bool)privateType.InvokeStatic("CheckExpiration", string.Format("{0:yyyyMMddHHmm}", DateTime.Now.AddMinutes(-179))) ;
//            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results,false); //falseでなければOK
            
        }
        
    }
}
