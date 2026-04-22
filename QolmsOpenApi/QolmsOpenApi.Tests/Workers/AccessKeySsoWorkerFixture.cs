using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AccessKeySsoWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        AccessKeySsoWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _parentAccountKey = Guid.NewGuid();
        Guid _executor = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _worker = new AccessKeySsoWorker(_accountRepo.Object);
        }

        [TestMethod]
        public void アカウントマスタに存在しない場合はエラー()
        {
            var args = GetValidArgs();

            // アカウントマスタ null返却
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(default(QH_ACCOUNT_MST));

            var results = _worker.Generate(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("アカウントが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void アカウントマスタ取得失敗でエラー()
        {
            var args = GetValidArgs();

            // アカウントマスタ取得時 例外発生
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Throws(new Exception());

            var results = _worker.Generate(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void パスワードマネジメント情報取得失敗でエラー()
        {
            var args = GetValidArgs();

            // アカウントマスタは正常に取得できる
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                ACCOUNTKEY = _accountKey,
                PRIVATEACCOUNTFLAG = false
            });

            // パスワードマネジメント情報取得時 例外発生
            _accountRepo.Setup(m => m.ReadPasswordManagementEntity(_accountKey)).Throws(new Exception());

            var results = _worker.Generate(args);

            // TryGetPasswordManagementでエラーが発生しても、SSOキー生成は成功する（QolmsSsoKeyは生成されないが、SsoKeyは生成される）
            results.IsSuccess.Is(bool.TrueString);
            results.SsoKey.IsNotNull();
        }

        [TestMethod]
        public void 子のSSOキー指定で親アカウントが存在しない場合はエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(true);

            // 親アカウントが存在しない
            _accountRepo.Setup(m => m.ReadParentMasterEntity(_accountKey)).Returns(default(QH_ACCOUNT_MST));

            var results = _worker.Generate(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("親アカウントが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void 子のSSOキー指定で親アカウント取得失敗でエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(true);

            // 親アカウント取得時エラー
            _accountRepo.Setup(m => m.ReadParentMasterEntity(_accountKey)).Throws(new Exception());

            var results = _worker.Generate(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("親アカウント情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 親アカウントのSSOキー指定で正常終了()
        {
            var args = GetValidArgs();
            SetUpValidMethods(false);

            var results = _worker.Generate(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.SsoKey.IsNotNull(); // null以外なら生成されているものとする

            // アカウントマスタ取得処理が実行された
            _accountRepo.Verify(m => m.ReadMasterEntity(_accountKey), Times.Once);
            // 親アカウントマスタ取得処理は実行されなかった
            _accountRepo.Verify(m => m.ReadParentMasterEntity(_accountKey), Times.Never);
            // パスワードマネジメント情報取得処理が実行された
            _accountRepo.Verify(m => m.ReadPasswordManagementEntity(_accountKey), Times.Once);
        }

        [TestMethod]
        public void 子アカウントのSSOキー指定で正常終了()
        {
            var args = GetValidArgs();
            SetUpValidMethods(true);

            var results = _worker.Generate(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.SsoKey.IsNotNull(); // null以外なら生成されているものとする

            // アカウントマスタ取得処理が実行された
            _accountRepo.Verify(m => m.ReadMasterEntity(_accountKey), Times.Once);
            // 親アカウントマスタ取得処理が実行された
            _accountRepo.Verify(m => m.ReadParentMasterEntity(_accountKey), Times.Once);
            // パスワードマネジメント情報取得処理が実行された
            _accountRepo.Verify(m => m.ReadPasswordManagementEntity(_accountKey), Times.Once);
        }

        void SetUpValidMethods(bool isFamily = false)
        {
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                ACCOUNTKEY = _accountKey,
                PRIVATEACCOUNTFLAG = isFamily,                
            });

            _accountRepo.Setup(m => m.ReadParentMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                ACCOUNTKEY = _parentAccountKey,
                PRIVATEACCOUNTFLAG = false
            });

            _accountRepo.Setup(m => m.ReadPasswordManagementEntity(_accountKey)).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                ACCOUNTKEY = _accountKey,
                USERID = "testuser",
                USERPASSWORD = "hashedpassword123"
            });
        }

        QoAccessKeySsoApiArgs GetValidArgs()
        {
            return new QoAccessKeySsoApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.TisiOSApp}",
                QolmsPageNo = "1",
                QolmsSsoShowMenu = "1",
            };
        }
    }
}
