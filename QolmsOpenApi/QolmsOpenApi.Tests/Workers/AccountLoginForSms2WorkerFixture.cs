using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AccountLoginForSms2WorkerFixture
    {
        static string UserId = "MN0123456789";
        static string Password = "abc1234_";

        AccountLoginForSms2Worker _worker;
        Mock<IPasswordManagementRepository> _passwordRepo;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IIdentityApiRepository> _identityApi;
        Guid _accountKey;
        Guid _authKey;

        [TestInitialize]
        public void Initialize()
        {
            _passwordRepo = new Mock<IPasswordManagementRepository>();
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _identityApi = new Mock<IIdentityApiRepository>();

            _worker = new AccountLoginForSms2Worker(_passwordRepo.Object, _smsAuthCodeRepo.Object, _linkageRepo.Object, _identityApi.Object);

            _accountKey = Guid.NewGuid();
            _authKey = Guid.NewGuid();
        }

        [TestMethod]
        public void アカウントキーが正しくない場合はエラー()
        {
            var args = GetValidArgs();
            args.ActorKey = "";

            var results = _worker.LoginForSms2(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void Executorが未設定でエラー()
        {
            var args = GetValidArgs();
            args.Executor = "";

            var results = _worker.LoginForSms2(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("不正").IsTrue();
            results.Result.Detail.Contains(nameof(args.Executor)).IsTrue();
        }

        [TestMethod]
        public void AuthKeyReferenceが不正でエラー()
        {
            var args = GetValidArgs();
            args.AuthKeyReference = "";

            var results = _worker.LoginForSms2(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("不正").IsTrue();
            results.Result.Detail.Contains(nameof(args.AuthKeyReference)).IsTrue();
        }

        [TestMethod]
        public void 認証コードの期限切れでエラー()
        {
            var args = GetValidArgs();
            var expires = DateTime.Now.AddMinutes(-16); // 15分以上経っている
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = expires,
            });

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3004"); // 期限切れ
            results.Result.Detail.Contains("認証コードの期限が切れています").IsTrue();
        }

        [TestMethod]
        public void 認証コード照合失敗が規定回数以上でエラー()
        {
            var args = GetValidArgs();
            var expires = DateTime.Now.AddMinutes(15);
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = expires,
                FAILURECOUNT = 2 // 規定回数の2回失敗済み
            });

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3005"); // 照合処理規定回数オーバー
            results.Result.Detail.Contains("認証コードが一定回数間違えたため無効となっています").IsTrue();
        }

        [TestMethod]
        public void 認証コードが不一致でエラー()
        {
            var args = GetValidArgs();
            args.AuthCode = "999999"; // 正しくない認証コード
            var expires = DateTime.Now.AddMinutes(15);
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = expires,
            });

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3006"); // 認証コード不一致
            results.Result.Detail.Contains("認証コードが一致しませんでした").IsTrue();
        }

        [TestMethod]
        public void 認証コード照合処理で例外が発生するとエラー()
        {
            var args = GetValidArgs();

            // 例外発生
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Throws(new Exception());

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("認証コード照合処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void ユーザーが存在しない場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // entityをnullで返す
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Returns(default(QH_PASSWORDMANAGEMENT_DAT));

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("ユーザーが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void ユーザー情報取得処理で例外が発生した場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Throws(new Exception());

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("ユーザー情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void ログイン処理でロックダウン中の場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // ロックダウン中
            _identityApi.Setup(m => m.ExecuteLoginApi(string.Empty, UserId, Password, string.Empty, false, string.Empty)).Returns(new QiQolmsLoginApiResults
            {
                LoginResultType = QsApiLoginResultTypeEnum.Lockdown.ToString(),
            });

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");            
            results.Result.Detail.Contains("ログインできませんでした").IsTrue();
            // ロックダウンという情報は返さずリトライとして返された
            results.LoginResultType.Is(QsApiLoginResultTypeEnum.Retry.ToString());
        }

        [TestMethod]
        public void ログイン処理でSuccess以外はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 二段階認証要求
            _identityApi.Setup(m => m.ExecuteLoginApi(string.Empty, UserId, Password, string.Empty, false, string.Empty)).Returns(new QiQolmsLoginApiResults
            {
                LoginResultType = QsApiLoginResultTypeEnum.TwoFactorRequire.ToString(),
            });

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("ログインできませんでした").IsTrue();
            results.LoginResultType.Is(QsApiLoginResultTypeEnum.TwoFactorRequire.ToString());

            // OpenApiでは二段階認証は現時点では対応せず
        }

        [TestMethod]
        public void ログイン処理で例外が発生したらエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _identityApi.Setup(m => m.ExecuteLoginApi(string.Empty, UserId, Password, string.Empty, false, string.Empty)).Throws(new Exception());

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("ログイン処理でエラーが発生しました").IsTrue();
            results.LoginResultType.Is(QsApiLoginResultTypeEnum.None.ToString());
        }

        [TestMethod]
        public void 通知ID取得処理で例外が発生したらエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 99001)).Throws(new Exception());

            var results = _worker.LoginForSms2(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("通知用IDの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 通知ID取得処理で対応する医療ナビ_HOSPA以外はアカウントキー参照をIDとする()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);
            args.ExecuteSystemType = $"{QsApiSystemTypeEnum.KagaminoiOSApp}";

            var results = _worker.LoginForSms2(args);

            // 成功扱い
            results.IsSuccess.Is(bool.TrueString);
            // 通知IDはアカウントキー参照
            results.LinkageIdReference.Is(_accountKey.ToEncrypedReference());
        }

        [TestMethod]
        public void 通知ID取得処理で情報が存在しなくてもエラーにはしない()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // entityはnullを返す
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 99001)).Returns(default(QH_LINKAGE_DAT));

            var results = _worker.LoginForSms2(args);

            // 成功扱い
            results.IsSuccess.Is(bool.TrueString);
            // 通知IDは空文字
            results.LinkageIdReference.Is(string.Empty);
        }

        [TestMethod]
        public void 正常に終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.LoginForSms2(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.LinkageIdReference.Is("Hoge".ToEncrypedReference());
            results.LoginResultType.Is(QsApiLoginResultTypeEnum.Success.ToString());
            // 何かしらのトークンが生成されている
            results.Token.IsNotNull();
        }

        

        void SetupValidMethods(QoAccountLoginForSms2ApiArgs args)
        {
            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });

            // ユーザー情報取得
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                ACCOUNTKEY = _accountKey,
                USERPASSWORD = Password,
                USERID = UserId
            });

            // ログイン通過
            _identityApi.Setup(m => m.ExecuteLoginApi(string.Empty, UserId, Password, string.Empty, false, string.Empty)).Returns(new QiQolmsLoginApiResults
            {
                LoginResultType = QsApiLoginResultTypeEnum.Success.ToString(),
            });

            // 通知用ID
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 99001)).Returns(new QH_LINKAGE_DAT
            {
                LINKAGESYSTEMID = "Hoge"
            });
        }

        QoAccountLoginForSms2ApiArgs GetValidArgs()
        {
            return new QoAccountLoginForSms2ApiArgs
            {
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.QolmsNaviiOsApp}",
                ActorKey = _accountKey.ToApiGuidString(),
                AuthCode = "123456",
                AuthKeyReference = _authKey.ToEncrypedReference(),
            };
        }
    }
}
