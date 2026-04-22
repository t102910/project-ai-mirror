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

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestQoIdentityClient
    {
        // 連携ユーザ作成のIdentityApi呼び出しのテスト
        [TestMethod]
        public void QiQolmsManagementLinkageUserRegisterApiResults_Success()
        {
            string linkageSytemNo = "27003";
            string linkageSystemId = Guid.NewGuid().ToString("N");
            string userId = Guid.NewGuid().ToString("N");
            string userPass = "pass@12345678";
            string familyName = "てすと";
            string givenName = "なまえ";
            string familyKananame = "テスト";
            string givenKanaName = "ナマエ";
            string sexType = "1";
            string birthDay = DateTime.Now.ToApiDateString();
            string mailAddress = "";
            QiQolmsManagementLinkageUserRegisterApiResults results = QoIdentityClient.ExecuteLinkageUserRegisterApi(
             linkageSytemNo, linkageSystemId, userId, userPass, familyName, givenName, familyKananame, givenKanaName,
             sexType, birthDay, mailAddress);

            Assert.IsNotNull(results);            //null　でなければOK
            Assert.AreNotEqual(results.IsSuccess.TryToValueType(false),false);  //0 でなければOK
            Assert.AreNotEqual(results.AccountKey.TryToValueType(Guid.Empty), Guid.Empty); //EmptyでなければOK
        }
        

    }
}
