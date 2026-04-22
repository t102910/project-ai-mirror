using MGF.QOLMS.QolmsApiEntityV1;
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
    public class QolmsSystemKeyTest
    {
        [TestMethod]
        public void Generate()
        {
            var args = new QoQolmsSystemKeyGenerateApiArgs
            {
            };
            var result = QolmsSystemKeyWorker.Generate(args);
        }
    }
}
