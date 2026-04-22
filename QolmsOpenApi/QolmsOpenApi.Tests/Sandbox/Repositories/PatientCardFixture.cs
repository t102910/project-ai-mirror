using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Extension;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class PatientCardFixture
    {
        IPatientCardRepository _repo;

        Guid _testAccountKey = Guid.Parse("9c1c613d-af67-4fc5-809d-fdbdb930df66");

        [TestInitialize]
        public void Initialize()
        {
            _repo = new PatientCardRepository();
        }

        [TestMethod]
        public void アカウントキーを指定して施設情報を含む利用者カード情報リストを取得する()
        {
            var ret = _repo.ReadPatientCardList(_testAccountKey,1);
        }

        [TestMethod]
        public void 親連携番号で絞り込んで利用者カード情報リストを取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var ret = _repo.ReadPatientCardList(accountKey, 2, int.MinValue);
        }

        [TestMethod]
        public void 利用者カード情報を1件取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var ret = _repo.ReadPatientCard(accountKey, 27004, 1);
        }
    }
}
