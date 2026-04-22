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
    public class QkBackgroundFixture
    {
        IQkBackgroundRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new QkBackgroundRepository();
        }

        [TestMethod]
        public void 背景リストを取得できる()
        {
            var result = _repo.ReadList();
        }

        [TestMethod]
        public void 背景情報を取得できる()
        {
            var result = _repo.Read("mgf_bg0001");
        }

        [TestMethod]
        public void 背景情報を取得するが存在しない()
        {
            var result = _repo.Read("hogefuga");
            result.IsNull(); // 存在しない場合はnullが返る
        }

        [TestMethod]
        public void 削除された背景情報を取得できる()
        {
            var result = _repo.Read("mgf_bg_deleted01");
            result.DELETEFLAG.IsTrue();
        }

        [TestMethod]
        public void 背景ファイルを取得できる()
        {
            var result = _repo.ReadFile(new Guid("22d42912-c879-4b30-b646-f1cab93ec3ed"));
        }
    }
}
