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
    public class OtcDrugFixture
    {
        IOtcDrugRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new OtcDrugRepository();
        }

        [TestMethod]
        public void OTC医薬品を検索できる()
        {
            var ret = _repo.SearchDrug("ロキソニン", 0, 5);
        }

        [TestMethod]
        public void OTC医薬品詳細を取得できる()
        {
            var ret = _repo.ReadDrug("4987306054769", "J");
        }
    }
}
