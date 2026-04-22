using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppLive2DInitWorkerFixture
    {
        Mock<IPasswordManagementRepository> _passwordRepo;
        Mock<IAccountRepository> _accountRepo;
        Mock<IIdentityApiRepository> _identityApi;
        Mock<IAppSettingsRepository> _appSettingsRepo;
        Mock<IQkRandomAdviceRepository> _adviceRepo;

        AppLive2DInitWorker _worker;
        Guid _accountKey = Guid.NewGuid();
        Guid _photoKey = Guid.NewGuid();
        const string UserId = "MN0123456789";
        const string Password = "abc1234_";

        [TestInitialize]
        public void Initialize()
        {
            _passwordRepo = new Mock<IPasswordManagementRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _identityApi = new Mock<IIdentityApiRepository>();
            _appSettingsRepo = new Mock<IAppSettingsRepository>();
            _adviceRepo = new Mock<IQkRandomAdviceRepository>();

            _worker = new AppLive2DInitWorker(_passwordRepo.Object, _accountRepo.Object, _appSettingsRepo.Object, _adviceRepo.Object, _identityApi.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.LoginAndLoadData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void Executorが未設定でエラー()
        {
            var args = GetValidArgs();
            args.Executor = "";

            var results = _worker.LoginAndLoadData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("不正").IsTrue();
            results.Result.Detail.Contains(nameof(args.Executor)).IsTrue();
        }

        [TestMethod]
        public void ユーザーが存在しない場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // entityをnullで返す
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Returns(default(QH_PASSWORDMANAGEMENT_DAT));

            var results = _worker.LoginAndLoadData(args);

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

            var results = _worker.LoginAndLoadData(args);

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

            var results = _worker.LoginAndLoadData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("ログインできませんでした").IsTrue();
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

            var results = _worker.LoginAndLoadData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("ログインできませんでした").IsTrue();
        }

        [TestMethod]
        public void ログイン処理で例外が発生したらエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _identityApi.Setup(m => m.ExecuteLoginApi(string.Empty, UserId, Password, string.Empty, false, string.Empty)).Throws(new Exception());

            var results = _worker.LoginAndLoadData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("ログイン処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ユーザー情報が存在しない場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 存在しない
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(default(QH_ACCOUNTINDEX_DAT));

            var results = _worker.LoginAndLoadData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("対象アカウントが存在しません").IsTrue();
        }

        [TestMethod]
        public void ユーザー情報取得で例外が発生した場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Throws(new Exception());

            var results = _worker.LoginAndLoadData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void アドバイス取得処理で例外がでたらエラーが返る()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _adviceRepo.Setup(m => m.ReadSeasonList("0",DateTime.Now.Month)).Throws(new Exception());

            var results = _worker.LoginAndLoadData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アドバイス情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void アプリ設定の取得に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 設定取得に失敗
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, It.IsAny<int>())).Throws(new Exception());

            var results = _worker.LoginAndLoadData(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アプリ設定取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 正常に処理が完了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.LoginAndLoadData(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // ユーザー情報が正しく取得できている
            results.User.FamilyName.Is("田中");
            results.User.FamilyNameKana.Is("タナカ");
            results.User.GivenName.Is("一郎");
            results.User.GivenNameKana.Is("イチロウ");
            results.User.NickName.Is("イチロ");
            results.User.Birthday.Is(new DateTime(2000, 1, 1).ToApiDateString());
            results.User.AccountKeyReference.Is(_accountKey.ToEncrypedReference());
            results.User.PersonPhotoReference.Is(_photoKey.ToEncrypedReference());

            // アドバイスが正しく取得できている
            results.AdviceList[0].ID.Is(1);
            results.AdviceList[0].Advice.Is("hoge");
            results.AdviceList[1].ID.Is(2);
            results.AdviceList[1].Advice.Is("fuga");

            // 利用可能なモデルと背景が正しく設定されている
            // 未実装なので0件で正常とする
            results.AvailableModels.Count.Is(0);
            results.AvailableBackgrounds.Count.Is(0);

            // 設定が正しく取得できている
            results.SettingsJson.Is("someJson");

            // アクセスキーが設定されている
            results.AccessKey.IsNotNull();
        }

        void SetupValidMethods(QoAppLive2DInitApiArgs args)
        {            

            // アカウント情報取得
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

            // ユーザー情報
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                ACCOUNTKEY = _accountKey,
                FAMILYNAME = "田中",
                GIVENNAME = "一郎",
                FAMILYKANANAME = "タナカ",
                GIVENKANANAME = "イチロウ",
                NICKNAME = "イチロ",
                BIRTHDAY = new DateTime(2000,1,1),
                PHOTOKEY = _photoKey,
                SEXTYPE = 1,
            });

            // アドバイス取得
            _adviceRepo.Setup(m => m.ReadSeasonList("0", DateTime.Now.Month)).Returns(new List<QK_RANDOMADVICE_MST>
            {
                new QK_RANDOMADVICE_MST
                {
                    ID = 1,
                    ADVICE = "hoge"                    
                },
                new QK_RANDOMADVICE_MST
                {
                    ID = 2,
                    ADVICE = "fuga"
                }
            });

            // 設定取得
            _appSettingsRepo.Setup(m => m.ReadEntity(_accountKey, It.IsAny<int>())).Returns(new QH_APPSETTINGS_DAT
            {
                ACCOUNTKEY = _accountKey,
                VALUE = "someJson"
            });
        }

        QoAppLive2DInitApiArgs GetValidArgs()
        {
            return new QoAppLive2DInitApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Live2DApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsLive2DWeb}",
            };
        }
    }
}
