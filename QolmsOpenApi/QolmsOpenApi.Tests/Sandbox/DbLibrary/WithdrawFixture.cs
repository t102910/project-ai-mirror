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

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class WithdrawFixture
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void 各種テーブル削除()
        {
            // 実行すると実際にDBから削除されるので細心の注意の元実行すること
            var writer = new DbUnsubscribeWriterCore(Guid.NewGuid(),Guid.NewGuid());

            var accountKey = Guid.Parse("7517aa9b-d534-4ebf-b234-7d1e2a2d1311");
            writer.DeleteTableData(DateTime.Now, accountKey, 99001);
        }
    }
}
