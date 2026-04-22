using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{   

    [TestClass]
    public class SmapaApiFixture
    {
        ISmapaApiRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new SmapaApiRepository();
        }

        [TestMethod]
        public async Task API01を実行する()
        {
            var ret = await _repo.ExecuteSmapaApi("9999910006", "00001300", QoApiConfiguration.SmapaApiHomeUrl);
        }

        [TestMethod]
        public async Task API09を実行する()
        {
            var ret = await _repo.ExecuteSmapaApi("9999910006", "00001300", QoApiConfiguration.SmapaApiRevokeUrl);
        }
    }
}
