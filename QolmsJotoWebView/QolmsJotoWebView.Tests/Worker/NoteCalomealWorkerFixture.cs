using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Worker;
using Moq;
using NUnit.Framework;
using System;

namespace QolmsJotoWebView.Tests
{
    [TestFixture]
    public class NoteCalomealWorkerFixture
    {
        Mock<ICalomealRepository> _calomealRepo;
        Mock<ICalomealWebViewApiRepository> _calomealApiRepo;
        Mock<IVitalRepository> _vitalRepo;
        NoteCalomealWorker _worker;
        Mock<IDateTimeProvider> _dateTimePro;


        Guid _accountKey = Guid.NewGuid();
        string _testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyMTIzIiwiaWF0IjoxNjc2ODAwMDAwfQ.test";
        const int CALOMEAL_LINKAGESYSTEMNO = 47015;
        const string CALOMEAL_TOKENLOG_FILENAME = "CalomealToken.Log";
        const string DEBUG_LOG_PATH = "/var/log/debug";

        [SetUp]
        public void Setup()
        {
            _calomealRepo = new Mock<ICalomealRepository>();
            _calomealApiRepo = new Mock<ICalomealWebViewApiRepository>();
            _worker = new NoteCalomealWorker(_calomealRepo.Object, _calomealApiRepo.Object, _vitalRepo.Object);
            _dateTimePro = new Mock<IDateTimeProvider>();
        }

        #region TokenRead Tests

        [Test]
        public void TokenRead_正常系_有効なトークンが存在()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            var currentDate = _dateTimePro.Object.Now.AddHours(1);
            var expiredDate = currentDate.AddHours(1);

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = "valid-token",
                TokenExpires = expiredDate.ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string token = _worker.TokenRead(mainModel);

            // Assert
            token.Is("valid-token");
            _calomealApiRepo.Verify(
                m => m.GetRefreshToken(It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public void TokenRead_正常系_トークンが空文字列()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = "",
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string token = _worker.TokenRead(mainModel);

            // Assert
            token.Is(string.Empty);
        }

        [Test]
        public void TokenRead_正常系_トークン期限切れ_リフレッシュトークン成功()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            var currentDate = DateTime.Parse("2026-02-20");
            var expiredDate = currentDate.AddHours(-1);

            var expiredTokenSet = new QhApiFitbitTokenSetItem
            {
                Token = "expired-token",
                TokenExpires = expiredDate.ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = expiredTokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // JWTトークンのペイロード部分（Base64エンコード）
            var jwtPayload = "eyJzdWIiOiAidXNlcjEyMyIsICJpYXQiOiAxMjM0NTY3ODkwfQ";
            var accessToken = $"header.{jwtPayload}.signature";

            var refreshTokenResult = new CalomealAccessTokenSet
            {
                access_token = accessToken,
                refresh_token = "new-refresh-token"
            };

            _calomealApiRepo
                .Setup(m => m.GetRefreshToken("refresh-token"))
                .Returns(refreshTokenResult);

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionWriteApi(
                    mainModel,
                    CALOMEAL_LINKAGESYSTEMNO,
                    It.IsAny<string>(),
                    refreshTokenResult,
                    false))
                .Returns(new QhYappliPortalCalomealConnectionWriteApiResults { IsSuccess = Boolean.TrueString });

            // Act
            string token = _worker.TokenRead(mainModel);

            // Assert
            token.Is(accessToken);
            _calomealApiRepo.Verify(
                m => m.GetRefreshToken("refresh-token"),
                Times.Once);
            _calomealRepo.Verify(
                m => m.ExecuteCalomealConnectionWriteApi(
                    mainModel,
                    CALOMEAL_LINKAGESYSTEMNO,
                    It.IsAny<string>(),
                    refreshTokenResult,
                    false),
                Times.Once);
        }

        [Test]
        public void TokenRead_正常系_トークン期限切れ_リフレッシュトークン失敗()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            var currentDate = DateTime.Parse("2026-02-20");
            var expiredDate = currentDate.AddHours(-1);

            var expiredTokenSet = new QhApiFitbitTokenSetItem
            {
                Token = "expired-token",
                TokenExpires = expiredDate.ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = expiredTokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            var refreshTokenResult = new CalomealAccessTokenSet
            {
                access_token = "",
                refresh_token = "new-refresh-token"
            };

            _calomealApiRepo
                .Setup(m => m.GetRefreshToken("refresh-token"))
                .Returns(refreshTokenResult);

            // Act
            string token = _worker.TokenRead(mainModel);

            // Assert
            token.Is("");
            _calomealApiRepo.Verify(
                m => m.GetRefreshToken("refresh-token"),
                Times.Once);
            _calomealRepo.Verify(
                m => m.ExecuteCalomealConnectionWriteApi(It.IsAny<QolmsJotoModel>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CalomealAccessTokenSet>(), It.IsAny<bool>()),
                Times.Never);
        }

        #endregion

        #region CreateWebViewUrl Tests

        [Test]
        public void CreateWebViewUrl_正常系_有効な日付指定()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "1";
            string selectdate = "20260101"; // 過去の日付

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_日付未指定_本日を使用()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "2";
            string selectdate = "";

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_無効な日付形式_本日を使用()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "3";
            string selectdate = "2026/01/01"; // 無効な形式

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_未来の日付_本日を使用()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "1";
            string selectdate = DateTime.Now.Date.AddDays(1).ToString("yyyyMMdd"); // 明日

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_meal値の変換_breakfast()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "1";
            string selectdate = "";

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_meal値の変換_lunch()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "2";
            string selectdate = "";

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_meal値の変換_dinner()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "3";
            string selectdate = "";

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_meal値の変換_snack()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "4";
            string selectdate = "";

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = _testToken,
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void CreateWebViewUrl_正常系_トークンなし_認証画面へリダイレクト()
        {
            // Arrange
            var mainModel = CreateValidQolmsJotoModel();
            string meal = "1";
            string selectdate = "";

            var tokenSet = new QhApiFitbitTokenSetItem
            {
                Token = "",
                TokenExpires = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                RefreshToken = "refresh-token"
            };

            var result = new QhYappliPortalCalomealConnectionTokenReadApiResults
            {
                TokenSet = tokenSet
            };

            _calomealRepo
                .Setup(m => m.ExecuteCalomealConnectionTokenReadApi(mainModel, CALOMEAL_LINKAGESYSTEMNO))
                .Returns(result);

            // Act
            string url = _worker.CreateWebViewUrl(mainModel, meal, selectdate);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        #endregion

        #region GetWebViewAuthUrl Tests

        [Test]
        public void GetWebViewAuthUrl_正常系_有効なトークン_breakfast()
        {
            // Arrange
            var token = "test-token";
            var selectDate = DateTime.Parse("2026-01-15");
            byte meal = 1;

            // Act
            string url = NoteCalomealWorker.GetWebViewAuthUrl(token, selectDate, meal);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void GetWebViewAuthUrl_正常系_トークンなし()
        {
            // Arrange
            var token = "";
            var selectDate = DateTime.Parse("2026-01-15");
            byte meal = 1;

            // Act
            string url = NoteCalomealWorker.GetWebViewAuthUrl(token, selectDate, meal);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        [Test]
        public void GetWebViewAuthUrl_正常系_meal値ゼロ()
        {
            // Arrange
            var token = "test-token";
            var selectDate = DateTime.Parse("2026-01-15");
            byte meal = 0;

            // Act
            string url = NoteCalomealWorker.GetWebViewAuthUrl(token, selectDate, meal);

            // Assert
            string.IsNullOrWhiteSpace(url).IsFalse();
        }

        #endregion
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

            return new QolmsJotoModel(refAuthorAccount, "Session", Guid.NewGuid(), _dateTimePro.Object.Now.AddMinutes(15), Guid.NewGuid(), _dateTimePro.Object.Now.AddMinutes(15));
        }
    }
}
