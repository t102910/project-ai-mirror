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

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class AccountFamilyWorkerFixture
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void 家族の削除実行テスト()
        {
            // 成功すると実際にDBから削除されるので注意して実行する

            var accountKey = Guid.Parse("B776CB87-9B26-464A-B80A-51B98E1A50FA");
            var childAccountKey = Guid.Parse("7F42644C-95C2-42CC-A6BB-207CB692E134");

            var args = new QoAccountFamilyDeleteApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                AccountKeyReference = childAccountKey.ToEncrypedReference(),
            };

            var results = AccountFamilyWorker.Delete(args);
        }
    }
}
