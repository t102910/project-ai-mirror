using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class FamilyFixture
    {
        IFamilyRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new FamilyRepository();
        }

        [TestMethod]
        public void 家族リストの取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var ret = _repo.ReadFamilyList(accountKey);

            var json = JsonConvert.SerializeObject(ret);
        }

        [TestMethod]
        public void 親子関係確認()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var child = Guid.Parse("fdab892a-c364-4d96-9d5a-cfb00bd1b4a1");

            var ret = _repo.IsParentChildRelation(accountKey, child);
        }

        [TestMethod]
        public void 家族の追加()
        {
            var user = new QoApiUserItem
            {
                FamilyName = "山田",
                GivenName = "次郎",
                FamilyNameKana = "ヤマダ",
                GivenNameKana = "ジロウ",
                Sex = 1,
            };

            var birthdate = new DateTime(2000, 1, 1);

            var photoKey = Guid.Empty;

            var parent = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var childAccountKey = _repo.AddFamily(parent, user, birthdate, photoKey);
        }

        [TestMethod]
        public void 家族の削除()
        {
            var parent = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var child = Guid.Parse("2eccc11e-043b-4346-b40d-d96551a87789");

            _repo.DeleteFamily(parent, child, parent);
        }
    }
}
