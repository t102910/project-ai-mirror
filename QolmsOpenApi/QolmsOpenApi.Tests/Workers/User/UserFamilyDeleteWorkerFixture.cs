using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
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
    public class UserFamilyDeleteWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        Mock<IFamilyRepository> _familyRepo;
        Mock<IStorageRepository> _storageRepo;

        UserFamilyDeleteWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _childKey = Guid.NewGuid();
        Guid _photoKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _familyRepo = new Mock<IFamilyRepository>();
            _storageRepo = new Mock<IStorageRepository>();

            _worker = new UserFamilyDeleteWorker(_accountRepo.Object, _familyRepo.Object, _storageRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 対象アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.AccountKeyReference = "invalidGuid";

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.AccountKeyReference)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 親アカウントが指定された場合はエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 自身のアカウントキーを指定
            args.AccountKeyReference = _accountKey.ToEncrypedReference();

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー            
            results.Result.Detail.Contains("パブリックアカウントは削除できません").IsTrue();
        }

        [TestMethod]
        public void 親子関係チェックで例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外を発生させる
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, _childKey)).Throws(new Exception());

            var results = _worker.Delete(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント関連情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 写真キー取得処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外を発生させる
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_childKey)).Throws(new Exception());

            var results = _worker.Delete(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("画像情報取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 削除処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外を発生させる
            _familyRepo.Setup(m => m.DeleteFamily(_accountKey, _childKey, Guid.Parse(args.AuthorKey))).Throws(new Exception());

            var results = _worker.Delete(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("子アカウントの削除に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常に削除される_画像あり()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // データ削除処理が実行された
            _familyRepo.Verify(m => m.DeleteFamily(_accountKey, _childKey, authorKey), Times.Once);

            // ファイル削除処理が実行された
            _storageRepo.Verify(m => m.DeleteFile(authorKey, _photoKey), Times.Once);
        }

        [TestMethod]
        public void 正常に削除される_画像なし()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 画像キーが空
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_childKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                PHOTOKEY = Guid.Empty
            });

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // データ削除処理が実行された
            _familyRepo.Verify(m => m.DeleteFamily(_accountKey, _childKey, authorKey), Times.Once);

            // ファイル削除処理はスキップされた
            _storageRepo.Verify(m => m.DeleteFile(authorKey, Guid.Empty), Times.Never);
        }

        void SetupValidMethods(QoUserFamilyDeleteApiArgs args)
        {
            // 親アカウントチェック通過用
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                PRIVATEACCOUNTFLAG = false
            });

            var targetAccountKey = args.AccountKeyReference.ToDecrypedReference<Guid>();

            // 親子関係成立
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, targetAccountKey)).Returns(true);

            // 画像キー
            _accountRepo.Setup(m => m.ReadAccountIndexDat(targetAccountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                PHOTOKEY = _photoKey
            });

            
        }

        QoUserFamilyDeleteApiArgs GetValidArgs()
        {
            return new QoUserFamilyDeleteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),               
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                AccountKeyReference = _childKey.ToEncrypedReference()
            };
        }
    }
}
