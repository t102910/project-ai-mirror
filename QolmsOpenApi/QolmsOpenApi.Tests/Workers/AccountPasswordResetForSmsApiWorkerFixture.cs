using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;


namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AccountPasswordResetForSmsApiWorkerFixture
    {
        AccountPasswordResetForSmsApiWorker _worker;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<IPasswordManagementRepository> _passwordManagementRepo;
        Mock<IAccountRepository> _accountRepo;
        Guid _authKey;
        Guid _accountKey;
        DateTime _birthDate = new DateTime(1990, 6, 15);

        [TestInitialize]
        public void Initialize()
        {
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _passwordManagementRepo = new Mock<IPasswordManagementRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _worker = new AccountPasswordResetForSmsApiWorker(_passwordManagementRepo.Object, _smsAuthCodeRepo.Object, _accountRepo.Object);

            _authKey = Guid.NewGuid();
            _accountKey = Guid.NewGuid();
        }        

        [TestMethod]
        public void パスワードが未設定でエラー()
        {
            var args = GetValidArgs();
            args.Password = string.Empty;

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("必須").IsTrue();
            results.Result.Detail.Contains("Password").IsTrue();
        }

        [TestMethod]
        public void 認証キーが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.AuthKeyReference = "Hoge";

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AuthKeyReference));
        }

        [TestMethod]
        public void 認証コードチェックエラー_期限切れ()
        {
            var args = GetValidArgs();

            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(-10),
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3004");
            results.Result.Detail.Contains("認証コードの期限が切れています");
        }

        [TestMethod]
        public void 認証コードチェックエラー_試行回数オーバー()
        {
            var args = GetValidArgs();

            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
                FAILURECOUNT = 2,
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3005");
            results.Result.Detail.Contains("認証コードが一定回数間違えたため無効となっています");
        }

        [TestMethod]
        public void 認証コードチェックエラー_認証コード不一致()
        {
            var args = GetValidArgs();

            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "65432",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3006");
            results.Result.Detail.Contains("認証コードが一致しませんでした");
        }

        [TestMethod]
        public void 認証コードチェックエラー_例外発生()
        {
            var args = GetValidArgs();

            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });

            _smsAuthCodeRepo.Setup(m => m.ReadEntity(It.IsAny<Guid>())).Throws(new Exception("例外発生"));

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("認証コード照合処理に失敗しました");
        }

        [TestMethod]
        public void 生年月日が不正でエラー()
        {
            var args = GetValidArgs();
            args.BirthDate = "hoge"; // 日付以外の不正フォーマット
            SetupValidMethods(args);

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("生年月日が不正です").IsTrue();
        }

        [TestMethod]
        public void 生年月日が不一致でエラー()
        {
            var args = GetValidArgs();
            args.BirthDate = new DateTime(1980, 1, 1).ToApiDateString();
            SetupValidMethods(args);

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3009");
            results.Result.Detail.Contains("生年月日が一致しませんでした").IsTrue();
        }

        [TestMethod]
        public void 生年月日取得処理で例外発生でエラー()
        {
            var args = GetValidArgs();
            args.BirthDate = _birthDate.ToApiDateString();
            SetupValidMethods(args);

            // アカウント取得処理で例外
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Throws(new Exception());


            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("生年月日取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 新しいパスワードの有効性チェック_UserIDとパスワードが同じです()
        {
            var args = GetValidArgs();
            args.Password = "123456";
            SetupValidMethods(args);

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERID = "123456",
                USERPASSWORD = "abc1234_",
                LASTUPDATEPASSWORDDATE = new DateTime(2024, 2, 6, 17, 24, 0),
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("UserIDとパスワードが同じです").IsTrue();
        }

        [TestMethod]
        public void 新しいパスワードの有効性チェック_登録済みパスワードが空白です()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERID = "123456",
                USERPASSWORD = "",
                LASTUPDATEPASSWORDDATE = new DateTime(2024, 2, 6, 17, 24, 0),
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("登録済みパスワードが空白です").IsTrue();
        }

        [TestMethod]
        public void 新しいパスワードの有効性チェック_登録済みパスワードの更新日時が不正です()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERID = "123456",
                USERPASSWORD = "abc1234_",
                LASTUPDATEPASSWORDDATE = DateTime.MinValue,
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("登録済みパスワードの更新日時が不正です").IsTrue();
        }

        [TestMethod]
        public void 新しいパスワードの有効性チェック_現在のパスワードと新しいパスワードが同じです()
        {
            var args = GetValidArgs();
            args.Password = "abc1234_";
            SetupValidMethods(args);

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERID = "123456",
                USERPASSWORD = "abc1234_",
                LASTUPDATEPASSWORDDATE = new DateTime(2024,2,6,17,24,0),
            });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("現在のパスワードと新しいパスワードが同じです").IsTrue();
        }

        [TestMethod]
        public void 新しいパスワードの有効性チェック_パスワードが無効です()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns((QH_PASSWORDMANAGEMENT_DAT)null);

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("パスワードが無効です").IsTrue();
        }

        [TestMethod]
        public void 新しいパスワードの有効性チェック_UnknownError()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERID = "123456",
                USERPASSWORD = "abc1234_",
            });

            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(It.IsAny<string>())).Returns(new QH_ACCOUNTPHONE_MST() { ACCOUNTKEY = new Guid() });

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("9999");
        }

        [TestMethod]
        public void 処理に成功して正常に終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 処理に成功して正常に終了する_生年月日あり()
        {
            var args = GetValidArgs();
            args.BirthDate = _birthDate.ToApiDateString();
            SetupValidMethods(args);

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void パスワード変更処理中に例外が発生する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _passwordManagementRepo.Setup(m => m.EditPassword(It.IsAny<Guid>(), It.IsAny<string>())).Throws(new Exception("例外発生"));

            var results = _worker.PasswordResetRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("パスワード変更処理に失敗しました").IsTrue();
        }

        void SetupValidMethods(QoAccountPasswordResetForSmsApiArgs args)
        {
            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });

            // 戻り値をセット
            _passwordManagementRepo.Setup(m => m.ReadDecryptedEntity(It.IsAny<Guid>())).Returns(GetPasswordManagementDat());

            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(It.IsAny<string>())).Returns(new QH_ACCOUNTPHONE_MST() { ACCOUNTKEY = _accountKey });

            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = _birthDate
            });
        }

        QoAccountPasswordResetForSmsApiArgs GetValidArgs()
        {
            return new QoAccountPasswordResetForSmsApiArgs
            {
                AuthCode = "123456",
                AuthKeyReference = _authKey.ToEncrypedReference(),
                ActorKey = _authKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsNaviiOsApp}",
                Password = "abc12345"
            };
        }

        QH_PASSWORDMANAGEMENT_DAT GetPasswordManagementDat()
        {
            return new QH_PASSWORDMANAGEMENT_DAT
            {
                USERID = "Hoge",
                USERPASSWORD = "abc1234_",
                LASTUPDATEPASSWORDDATE = new DateTime(2024, 2, 6, 16, 56, 0),
            };
        }
    }
}
