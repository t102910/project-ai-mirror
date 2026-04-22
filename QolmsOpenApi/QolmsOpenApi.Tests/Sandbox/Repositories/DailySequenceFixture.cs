using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class DailySequenceFixture
    {
        IDailySequenceRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new DailySequenceRepository();
        }

        [TestMethod]
        public void 連番の更新と取得()
        {
            var ret = _repo.GetDailySequence(DateTime.Now,MGF.QOLMS.QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsTisApp,1);
        }

        [TestMethod]
        public void 同時更新テスト()
        {
            var list = Enumerable.Range(0, 50);
            Parallel.ForEach(list, x =>
            {
                 Console.WriteLine(_repo.GetDailySequence());
            });
        }

        [TestMethod]
        public void 同時新規日時追加テスト()
        {
            var list = Enumerable.Range(0, 50);
            // まだDBに存在しない日付を指定する
            var targetDate = DateTime.Today.AddDays(-1);

            Parallel.ForEach(list, x =>
            {
                Console.WriteLine(_repo.GetDailySequence(targetDate,0,1));
            });
        }
    }
}
