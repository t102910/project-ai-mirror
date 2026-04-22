using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class IdentityApiFixture
    {
        IIdentityApiRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new IdentityApiRepository();
        }

        [TestMethod]
        public void 新規連携ユーザー登録テスト()
        {
            var accountKeyRef = "fdc0b64b3f5bfc32f9e23dedc7c777a285fa9f0e94662249513c1abcd0b3d6510c3291114c7e59a66bb0c760470f4c38a95fff7f7a85e109e3ed716bfe4db545";
            var accountKey = accountKeyRef.ToDecrypedReference().TryToValueType(Guid.Empty);

            //var result = _repo.ExecuteLinkageUserRegisterApi(
            //    "27003",
            //    "66666009",
            //    "TS1234AA5678",
            //    "abc1234-",
            //    "患者",
            //    "太郎",
            //    "カンジャ",
            //    "タロウ",
            //    "1",
            //    new DateTime(2000,1,1).ToApiDateString(),
            //    "kamuxxx@hotmail.com",
            //    accountKey
            // );
        }
    }
}
