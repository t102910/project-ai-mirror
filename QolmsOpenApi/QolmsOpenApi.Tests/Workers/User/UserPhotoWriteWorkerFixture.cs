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
    public class UserPhotoWriteWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        Mock<IFamilyRepository> _familyRepo;
        Mock<IStorageRepository> _storageRepo;

        UserPhotoWriteWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _childKey = Guid.NewGuid();
        Guid _fileKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _familyRepo = new Mock<IFamilyRepository>();
            _storageRepo = new Mock<IStorageRepository>();

            _worker = new UserPhotoWriteWorker(_accountRepo.Object, _familyRepo.Object, _storageRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Write(args);

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

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.AccountKeyReference)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 画像引数のバリデーションでエラーとなる()
        {
            var args = GetValidArgs();
            // 画像がnull
            args.Photo = null;

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("利用者画像情報が不正です").IsTrue();

            args = GetValidArgs();
            // ContentTypeが未指定
            args.Photo.ContentType = string.Empty;

            results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("利用者画像MIMEタイプが不正です").IsTrue();

            args = GetValidArgs();
            // Dataが未指定
            args.Photo.Data = string.Empty;

            results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("利用者画像データが不正です").IsTrue();
        }

        [TestMethod]
        public void 親子関係が成立していない場合はエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 親子関係ではない
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, _childKey)).Returns(false);

            var results = _worker.Write(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("親子関係が成立していません").IsTrue();
        }

        [TestMethod]
        public void 対象アカウント情報処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外を発生させる
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_childKey)).Throws(new Exception());

            var results = _worker.Write(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void ストレージ保存で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // 例外を発生させる
            _storageRepo.Setup(m => m.WriteImage(_childKey, authorKey, args.Photo)).Throws(new Exception());

            var results = _worker.Write(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("ファイル保存処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void アカウント情報更新処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            // 例外を発生させる
            _accountRepo.Setup(m => m.UpdateIndexEntity(entity)).Throws(new Exception());

            var results = _worker.Write(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報の更新に失敗しました").IsTrue();

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // 失敗時にファイルの削除が実行された
            _storageRepo.Verify(m => m.DeleteFile(authorKey, _fileKey),Times.Once);
        }

        [TestMethod]
        public void 正常に画像情報が更新される_子()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 子のアカウントがターゲットである
            entity.ACCOUNTKEY.Is(_childKey);

            // DB情報のファイルキーが更新されている
            entity.PHOTOKEY.Is(_fileKey);
            
            // 更新されたファイルキーが返された
            results.PhotoKeyReference.Is(_fileKey.ToEncrypedReference());

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // ファイルの削除が実行されなかった
            _storageRepo.Verify(m => m.DeleteFile(authorKey, _fileKey), Times.Never);
        }

        [TestMethod]
        public void 正常に画像情報が更新される_親()
        {
            var args = GetValidArgs();
            args.AccountKeyReference = string.Empty;
            var entity = SetupValidMethods(args);

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 親のアカウントがターゲットである
            entity.ACCOUNTKEY.Is(_accountKey);

            // DB情報のファイルキーが更新されている
            entity.PHOTOKEY.Is(_fileKey);

            // 更新されたファイルキーが返された
            results.PhotoKeyReference.Is(_fileKey.ToEncrypedReference());

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // ファイルの削除が実行されなかった
            _storageRepo.Verify(m => m.DeleteFile(authorKey, _fileKey), Times.Never);
        }

        QH_ACCOUNTINDEX_DAT SetupValidMethods(QoUserPhotoWriteApiArgs args)
        {
            // 親アカウントチェック通過用
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                PRIVATEACCOUNTFLAG = false
            });

            var targetAccountKey = string.IsNullOrWhiteSpace(args.AccountKeyReference) ?
                _accountKey :
                args.AccountKeyReference.ToDecrypedReference<Guid>();

            // 親子関係成立
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, targetAccountKey)).Returns(true);

            var entity = new QH_ACCOUNTINDEX_DAT
            {
                ACCOUNTKEY = targetAccountKey,
                FAMILYNAME = "山田",
                GIVENNAME = "太郎",
                FAMILYKANANAME = "ヤマダ",
                GIVENKANANAME = "タロウ",
                BIRTHDAY = new DateTime(2000, 1, 1),
                SEXTYPE = 1,
                NICKNAME = "タロ",
                PHOTOKEY = Guid.NewGuid()
            };

            // 対象アカウント取得
            _accountRepo.Setup(m => m.ReadAccountIndexDat(targetAccountKey)).Returns(entity);

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // 画像保存成功
            _storageRepo.Setup(m => m.WriteImage(targetAccountKey, authorKey, args.Photo)).Returns(_fileKey);

            return entity;
        }

        



        QoUserPhotoWriteApiArgs GetValidArgs()
        {
            return new QoUserPhotoWriteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                AccountKeyReference = _childKey.ToEncrypedReference(),
                Photo = new QoApiFileItem
                {
                    ContentType = "image/jpeg",
                    OriginalName = "photo.jpg",
                    Data = "data"
                }
            };
        }
    }
}
