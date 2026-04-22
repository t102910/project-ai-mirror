using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
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
    public class SignUpFixture
    {
        ISignUpRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new SignUpRepository();
        }

        [TestMethod]
        public void 仮登録データを削除できる()
        {
            var accountkey = Guid.Parse("3d3e63ed-c4ad-454d-935a-1fda0710eb56");
            
            var ret = _repo.DeleteSignUpData(accountkey);
            ret.IsTrue();
        }
    }
}
