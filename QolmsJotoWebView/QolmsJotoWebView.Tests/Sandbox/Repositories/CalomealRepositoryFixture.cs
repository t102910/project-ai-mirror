using MGF.QOLMS.QolmsJotoWebView;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QolmsJotoWebView.Tests.Sandbox.Repositories
{
    [TestFixture]
    class CalomealRepositoryFixture
    {
        private ICalomealRepository _repository;
        Mock<IDateTimeProvider> _dateTimePro;

        private Mock<HttpSessionStateBase> _mockSession;
        private Mock<HttpContextBase> _mockHttpContext;
        //private Mock<IAccessLogWorker> _mockAccessLogWorker;
        //private Mock<IQsCalomealWebViewApiManager> _mockApiManager;

        Guid _accountKey = Guid.Parse("81417A6F-1BAC-48A5-B691-750B003529A8");
        string _sessionid = Guid.NewGuid().ToString("N");
        int _linkage = 47015;

        [SetUp]
        public void SetUp()
        {
            _repository = new CalomealRepository();
            _dateTimePro = new Mock<IDateTimeProvider>();

            _dateTimePro.Setup(m => m.Now).Returns(DateTime.Now);

            _mockSession = new Mock<HttpSessionStateBase>();
            _mockSession.Setup(m => m.SessionID).Returns(_sessionid);
            _mockHttpContext = new Mock<HttpContextBase>();
            _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);
        }

        [Test]
        public void トークンが取得できる()
        {
            //テスト通らないけど後回しにします。
            var ret = _repository.ExecuteCalomealConnectionTokenReadApi(CreateValidQolmsJotoModel(), _linkage);

            (ret.IsSuccess == bool.TrueString).IsTrue(); ;
            string.IsNullOrWhiteSpace(ret.TokenSet.Token).IsFalse();
        }

        private QolmsJotoModel CreateValidQolmsJotoModel()
        {
            _dateTimePro.Setup(m => m.Now).Returns(DateTime.Now);

            var refAuthorAccount = new AuthorAccountItem()
            {
                UserId = "userid",
                LoginAt = DateTime.MinValue,
                AccountKey = _accountKey,
                FamilyName = string.Empty,
                MiddleName = string.Empty,
                GivenName = string.Empty,
                FamilyKanaName = string.Empty,
                MiddleKanaName = string.Empty,
                GivenKanaName = string.Empty,
                FamilyRomanName = string.Empty,
                MiddleRomanName = string.Empty,
                GivenRomanName = string.Empty,
                SexType = QjSexTypeEnum.Male,
                Birthday = DateTime.MinValue,
                EncryptedAccountKey = string.Empty
            };

            return new QolmsJotoModel(refAuthorAccount, _mockHttpContext.Object.Session.SessionID, Guid.NewGuid(), _dateTimePro.Object.Now.AddMinutes(15), Guid.NewGuid(), _dateTimePro.Object.Now.AddMinutes(15));
        }
    }
}
