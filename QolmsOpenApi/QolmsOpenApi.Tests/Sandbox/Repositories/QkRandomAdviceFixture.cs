using MGF.QOLMS.QolmsApiEntityV1;
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
    public class QkRandomAdviceFixture
    {
        IQkRandomAdviceRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new QkRandomAdviceRepository();
        }

        [TestMethod]
        public void アドバイスリストが取得できる()
        {
            var result = _repo.ReadList("0");

            result.Any().IsTrue();

            result = _repo.ReadList("1");

            result.Any().IsFalse();
        }

        [TestMethod]
        public void IDを指定してアドバイスが取得できる()
        {
            var result = _repo.Read(39);

            result.IsNotNull();

            result = _repo.Read(999);

            result.IsNull();
        }

        [TestMethod]
        public void ランダムなアドバイスリストを取得できる()
        {
            // 1月 朝 対象
            var result = _repo.ReadRandomList("0", 1, QkAdviceTimeType.Morning, new List<int> { 39, 45 });

            // 2月 夕方 対象
            result = _repo.ReadRandomList("0", 2, QkAdviceTimeType.Evening);
        }

        [TestMethod]
        public void 季節に対応したアドバイスリストを取得できる()
        {
            // 1月に対応したアドバイス
            var result = _repo.ReadSeasonList("0", 1);
        }
    }
}
