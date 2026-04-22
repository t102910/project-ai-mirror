using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class HealthRecordQueueFixture
    {
        IHealthRecordQueueRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new HealthRecordQueueRepository();
        }

        [TestMethod]
        public void キューを登録できる()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var vitals = new List<QhApiVitalValueItem>
            {
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,20,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyWeight}",
                }
            };

            var ret = _repo.Enqueue(accountKey, vitals);

            ret.IsTrue();
        }
    }
}
