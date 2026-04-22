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
    public class PasswordManagementFixture
    {
        IPasswordManagementRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new PasswordManagementRepository();
        }

        [TestMethod]
        public void 生のレコードを取得()
        {
            var accountKey = Guid.Parse("b776cb87-9b26-464a-b80a-51b98e1a50fa");

            var ret = _repo.ReadEntity(accountKey);
        }

        [TestMethod]
        public void 復号化されたレコードを取得()
        {
            var accountKey = Guid.Parse("b776cb87-9b26-464a-b80a-51b98e1a50fa");

            var ret = _repo.ReadDecryptedEntity(accountKey);
        }

        [TestMethod]
        public void パスワード変更()
        {
            // USERID = "MN2GRYRK2SVE"
            // PASSWORDRECOVERYMAILADDRESS = "ohuchi.mg@gmail.com"
            var accountKey = Guid.Parse("1FE7AA63-9F3C-4FC3-AF3F-3977D2EB009D");

            // 生のレコードを取得
            //ACCOUNTKEY = { 1fe7aa63 - 9f3c - 4fc3 - af3f - 3977d2eb009d}
            //PASSWORDRECOVERYMAILADDRESS = "6CuKaf2x4y3S1zs3iz3pAkFM0DDmpHNAXOTe1JoMja4="
            //USERID = "FCQXSdJWFjFWBkFSGbYoaw=="
            //USERPASSWORD = "Y5Rf9tIDZ7q9DHOTOFy2Sw=="
            var ret1 = _repo.ReadEntity(accountKey);

            // 復号化されたレコードを取得
            //ACCOUNTKEY = { 1fe7aa63 - 9f3c - 4fc3 - af3f - 3977d2eb009d}
            //PASSWORDRECOVERYMAILADDRESS = "ohuchi.mg@gmail.com"
            //USERID = "MN2GRYRK2SVE"
            //USERPASSWORD = "abc1234_"
            var ret2 = _repo.ReadDecryptedEntity(accountKey);
            var oldPassword = ret2.USERPASSWORD;

            // パスワード変更
            var newPassword = "abc12345";
            _repo.EditPassword(accountKey, newPassword);

            // 変更されたか確認
            // USERPASSWORD = "qoRLGpkOYf0oFgRLvoROnQ=="
            var ret3 = _repo.ReadEntity(accountKey);
            // USERPASSWORD = "abc12345"
            var ret4 = _repo.ReadDecryptedEntity(accountKey);
            var changedPassword = ret4.USERPASSWORD;
            changedPassword.Is(newPassword);

            // パスワードを戻す
            _repo.EditPassword(accountKey, oldPassword);

            // 戻ったか確認
            var ret5 = _repo.ReadDecryptedEntity(accountKey);
            var returnedPassword = ret5.USERPASSWORD;
            returnedPassword.Is(oldPassword);
        }
    }
}
