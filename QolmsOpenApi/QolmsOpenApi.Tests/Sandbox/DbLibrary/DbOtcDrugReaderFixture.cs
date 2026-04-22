using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsDbCoreV1;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class DbOtcDrugReaderFixture
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void 市販薬情報取得()
        {
            var reader = new DbOtcDrugReader();
            var args = new DbOtcDrugReaderArgs
            {
                ItemCode = "4987306054769",
                //ItemCodeType = "J"
            };

            var result = QsDbManager.Read(reader, args);
        }
    }
}
