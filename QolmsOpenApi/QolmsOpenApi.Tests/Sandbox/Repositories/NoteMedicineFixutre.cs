using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsApiCoreV1;
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
    public class NoteMedicineFixutre
    {
        INoteMedicineRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new NoteMedicineRepository();
        }

        [TestMethod]
        public void お薬手帳のリストを取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                DataType = "1",
                LastAccessDate = DateTime.Now.ToString(),
                PageIndex = "0",
                DaysPerPage = "1000"
            };           

            var ret =_repo.ReadMedicineList(args, out var isModified, out var lastAccessDate, out var pageIndex, out var maxPageIndex);
        }
    }
}
