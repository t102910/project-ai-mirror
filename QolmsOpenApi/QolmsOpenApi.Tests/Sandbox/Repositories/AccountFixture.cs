using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class AccountFixture
    {
        IAccountRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new AccountRepository();
        }

        [TestMethod]
        public void 複数の同一登録情報のユーザーが返る()
        {
            var ret = _repo.GetRegisteredAccountId("kamuxxx@hotmail.com", "中村", "聡", new DateTime(1977, 3, 27), 1);

            (ret.count >= 2).IsTrue();
        }

        [TestMethod]
        public void 同一登録情報のユーザーが返る()
        {
            var ret = _repo.GetRegisteredAccountId("naosuke_arai@mgfactory.co.jp", "テスト", "患者PHR①", new DateTime(1985, 10, 22), 1);

            ret.count.Is(1);
        }

        [TestMethod]
        public void Emailを取得できる()
        {
            var ret = _repo.GetAccountEmail(Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf"));
        }

        [TestMethod]
        public void 親アカウントを取得する()
        {
            var accountKey = Guid.Parse("ef2a37c3-8c78-4255-8413-a853714bcdfd");

            var ret = _repo.ReadParentMasterEntity(accountKey);

            accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            ret = _repo.ReadParentMasterEntity(accountKey);
            ret.IsNull();
        }

        [TestMethod]
        public void 親または本人アカウントを取得する()
        {
            // 子の場合
            var accountKey = Guid.Parse("d9c2cb2a-7701-4bae-9487-c39ab6300a0f");

            var ret = _repo.ReadParentOrSelfMasterEntiry(accountKey);

            // 親の場合
            accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            ret = _repo.ReadParentOrSelfMasterEntiry(accountKey);
        }

        [TestMethod]
        public void QH_ACCOUNTPHONE_MSTにレコードを追加()
        {
            var accountKey = Guid.NewGuid();

            var entity = new QH_ACCOUNTPHONE_MST
            {
                ACCOUNTKEY = accountKey,
                PHONENUMBER = "09099991234",
            };

            _repo.InsertPhoneEntity(entity);

            var ret = _repo.ReadPhoneEntity(accountKey);
        }

        [TestMethod]
        public void QH_ACCOUNTPHONE_MSTを電話番号で抽出()
        {
            var entity = _repo.ReadPhoneEntityByNumber("09099991234");
        }

        [TestMethod]
        public void QH_ACCOUNTPHONE_MSTのレコードを更新()
        {
            var accountKey = Guid.Parse("7517aa9b-d534-4ebf-b234-7d1e2a2d1311");

            var entity = _repo.ReadPhoneEntity(accountKey);

            entity.PHONENUMBER = "09000001234";

            _repo.UpdatePhoneEntity(entity);

            var ret = _repo.ReadPhoneEntity(accountKey);
        }

        [TestMethod]
        public void QH_ACCOUNTPHONE_MSTのレコードを論理削除()
        {
            var accountKey = Guid.Parse("7517aa9b-d534-4ebf-b234-7d1e2a2d1311");

            var entity = _repo.ReadPhoneEntity(accountKey);

            _repo.DeletePhoneEntity(entity);
        }

        [TestMethod]
        public void QH_ACCOUNTPHONE_MSTのレコードを物理削除()
        {
            var accountKey = Guid.NewGuid();

            var entity = new QH_ACCOUNTPHONE_MST
            {
                ACCOUNTKEY = accountKey,
                PHONENUMBER = "09099991235",
            };

            _repo.InsertPhoneEntity(entity);

            _repo.PhysicalDeletePhoneEntity(accountKey);
        }

        [TestMethod]
        public void QH_ACCOUNTINDEX_DATを更新()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var entity = _repo.ReadAccountIndexDat(accountKey);

            entity.NICKNAME = "サトシ";

            _repo.UpdateIndexEntity(entity);
        }

        [TestMethod]
        public void QH_ACCOUNT_MSTを挿入()
        {
            var entity = new QH_ACCOUNT_MST
            {
                ACCOUNTTYPE = 1,
                PRIVATEACCOUNTFLAG = false,
                TESTACCOUNTFLAG = false,
            };

            _repo.InsertMasterEntity(entity);
        }

        [TestMethod]
        public void QH_ACCOUNTINDEX_DATの挿入()
        {
            var entity = new QH_ACCOUNTINDEX_DAT
            {
                ACCOUNTKEY = Guid.NewGuid(),
                FAMILYNAME = "中村",
                GIVENNAME = "聡",
                FAMILYKANANAME = "ナカムラ",
                GIVENKANANAME = "サトシ",
                NICKNAME = "サト",
                BIRTHDAY = new DateTime(2000, 1, 1),
                SEXTYPE = 1,
                PHOTOKEY = Guid.Empty
            };

            _repo.InsertIndexEntity(entity);
        }

        [TestMethod]
        public void QH_ACCOUNTINDEX_DATのRead()
        {
            var accountKey = Guid.Parse("a0b20987-c0da-4c23-b018-be32f8fb4611");

            var ret = _repo.ReadAccountIndexDat(accountKey);
        }
    }
}
