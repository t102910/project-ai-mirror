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
    public class SmsAuthCodeFixture
    {
        ISmsAuthCodeRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new SmsAuthCodeRepository();
        }

        [TestMethod]
        public void レコード取得()
        {
            var authKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var entity = _repo.ReadEntity(authKey);
        }

        [TestMethod]
        public void レコード追加()
        {
            var authKey = Guid.NewGuid();
            var entity = new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = authKey,
                AUTHCODE = "999888",
                EXPIRES = DateTime.Now.AddMinutes(15)
            };

            _repo.InsertEntity(entity);

            var ret = _repo.ReadEntity(authKey);
        }

        [TestMethod]
        public void レコード更新()
        {
            var authKey = Guid.Parse("e3509271-ff6c-4220-aef3-498cc65afad2");
            var entity = _repo.ReadEntity(authKey);

            entity.AUTHCODE = "987654";

            _repo.UpdateEntity(entity);

            entity = _repo.ReadEntity(authKey);
        }

        [TestMethod]
        public void レコード論理削除()
        {
            var authKey = Guid.Parse("e3509271-ff6c-4220-aef3-498cc65afad2");
            var entity = _repo.ReadEntity(authKey);

            _repo.DeleteEntity(entity);
        }

        [TestMethod]
        public void レコード物理追加()
        {
            var authKey = Guid.NewGuid();
            var entity = new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = authKey,
                AUTHCODE = "033444",
                EXPIRES = DateTime.Now.AddMinutes(15)
            };

            _repo.InsertEntity(entity);

            _repo.PhysicalDeleteEntity(authKey);
        }
    }
}
