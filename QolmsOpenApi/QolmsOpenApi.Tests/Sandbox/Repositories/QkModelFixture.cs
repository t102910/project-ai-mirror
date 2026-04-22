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
    public class QkModelFixture
    {
        IQkModelRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new QkModelRepository();
        }

        [TestMethod]
        public void モデルリストを取得できる_無料のみ()
        {
            var result = _repo.ReadList(false);
        }

        [TestMethod]
        public void モデルリストを取得できる_有料含む()
        {
            var result = _repo.ReadList(true);
        }

        [TestMethod]
        public void モデル情報を取得できる()
        {
            var result = _repo.Read("sorarin");
        }

        [TestMethod]
        public void モデル情報を取得するが存在しない()
        {
            var result = _repo.Read("hogefuga");
            result.IsNull(); // 存在しない場合はnullが返る
        }

        [TestMethod]
        public void 削除されたモデル情報を取得できる()
        {
            var result = _repo.Read("mgf_ava_deleted01");
            result.DELETEFLAG.IsTrue();
        }

        [TestMethod]
        public void モデルファイルを取得できる()
        {
            var result = _repo.ReadFile(new Guid("35726130-67bb-4e71-8a02-1b7b64f87af3"));
        }
    }
}
