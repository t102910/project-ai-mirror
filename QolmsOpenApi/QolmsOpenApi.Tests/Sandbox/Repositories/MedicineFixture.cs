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
    public class MedicineFixture
    {
        IMedicineRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new MedicineRepository();
        }

        [TestMethod]
        public void 医薬品が検索できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var result = _repo.SearchMedicine(accountKey, "ロキソニン", 0, 4);
        }

        [TestMethod]
        public void 医薬品の詳細をできる()
        {
            //var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var yjCode = "2649735Q1020";

            var result = _repo.ReadDetail(yjCode);
        }

        [TestMethod]
        public void 医薬品の画像を取得できる()
        {
            var fileKey = Guid.Parse("94c5da04-dd3f-4100-b333-0980ad8bee00");

            var entity = _repo.ReadEthDrugFileEntity(fileKey);

            var fileInfo = _repo.ReadEthDrugFileBlob(entity);
        }
    }
}
