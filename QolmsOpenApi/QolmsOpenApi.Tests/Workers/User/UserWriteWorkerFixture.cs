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
    public class UserWriteWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        Mock<IFamilyRepository> _familyRepo;
        Mock<IStorageRepository> _storageRepo;

        UserWriteWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _childKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _familyRepo = new Mock<IFamilyRepository>();
            _storageRepo = new Mock<IStorageRepository>();

            _worker = new UserWriteWorker(_accountRepo.Object, _familyRepo.Object, _storageRepo.Object);
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
            args.User.AccountKeyReference = "invalidGuid";

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.User.AccountKeyReference)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 生年月日の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.User.Birthday = "Hoge";

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.Birthday)}が不正です");
        }

        [TestMethod]
        public void 性別の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.User.Sex = 99;

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.Sex)}が不正です");
        }

        [TestMethod]
        public void 必須項目未設定でエラーとなる()
        {
            // 漢字姓なし
            var args = GetValidArgs();
            args.User.FamilyName = string.Empty;

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.FamilyName)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // 漢字名なし
            args = GetValidArgs();
            args.User.GivenName = string.Empty;

            results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.GivenName)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // カナ名なし
            args = GetValidArgs();
            args.User.FamilyNameKana = string.Empty;

            results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.FamilyNameKana)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // カナ姓なし
            args = GetValidArgs();
            args.User.GivenNameKana = string.Empty;

            results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.GivenNameKana)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();
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
        public void 親子関係チェックで例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外を発生させる
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, _childKey)).Throws(new Exception());

            var results = _worker.Write(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント関連情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 対象アカウントが存在しない場合はエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 対象アカウントなし
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_childKey)).Returns(default(QH_ACCOUNTINDEX_DAT));

            var results = _worker.Write(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("対象アカウントが存在しません").IsTrue();
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
        }

        [TestMethod]
        public void 正常にアカウントを更新できる_子_画像あり()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // QH_ACCOUNTINDEX_DATの内容が引数の内容に書き換わっている
            var user = args.User;
            entity.FAMILYNAME.Is(user.FamilyName);
            entity.GIVENNAME.Is(user.GivenName);
            entity.FAMILYKANANAME.Is(user.FamilyNameKana);
            entity.GIVENKANANAME.Is(user.GivenNameKana);
            entity.NICKNAME.Is(user.NickName);
            entity.PHOTOKEY.Is(user.PersonPhotoReference.ToDecrypedReference<Guid>());
            entity.BIRTHDAY.Is(DateTime.Parse(user.Birthday));
            entity.SEXTYPE.Is(user.Sex);

            var photoKey = args.User.PersonPhotoReference.ToDecrypedReference<Guid>();
            // 画像のメタデータ更新が行われた
            _storageRepo.Verify(m => m.UpdateImageMetaDataAccountKey(photoKey, _childKey), Times.Once);
        }

        [TestMethod]
        public void 正常にアカウントを更新できる_親自身_画像なし()
        {
            var args = GetValidArgs();
            // 対象を親自身にする
            args.User.AccountKeyReference = _accountKey.ToEncrypedReference();
            // 画像なし
            args.User.PersonPhotoReference = string.Empty;

            var entity = SetupValidMethods(args);

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // QH_ACCOUNTINDEX_DATの内容が引数の内容に書き換わっている
            var user = args.User;
            entity.FAMILYNAME.Is(user.FamilyName);
            entity.GIVENNAME.Is(user.GivenName);
            entity.FAMILYKANANAME.Is(user.FamilyNameKana);
            entity.GIVENKANANAME.Is(user.GivenNameKana);
            entity.NICKNAME.Is(user.NickName);
            // 本人の場合生年月日と性別は変更できないので元のまま
            entity.BIRTHDAY.Is(new DateTime(2000, 1, 1));
            entity.SEXTYPE.Is((byte)2);

            // 親子関係チェックは通過しない
            _familyRepo.Verify(m => m.IsParentChildRelation(_accountKey, _accountKey), Times.Never);

            // 画像のメタデータ更新は行われなかった
            _storageRepo.Verify(m => m.UpdateImageMetaDataAccountKey(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        QH_ACCOUNTINDEX_DAT SetupValidMethods(QoUserWriteApiArgs args)
        {
            // 親アカウントチェック通過用
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                PRIVATEACCOUNTFLAG = false
            });

            var targetAccountKey = args.User.AccountKeyReference.ToDecrypedReference<Guid>();

            // 親子関係成立
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, targetAccountKey)).Returns(true);

            var entity = new QH_ACCOUNTINDEX_DAT
            {
                FAMILYNAME = "田中",
                GIVENNAME = "優子",
                FAMILYKANANAME = "タナカ",
                GIVENKANANAME = "ユウコ",
                BIRTHDAY = new DateTime(2000, 1, 1),
                SEXTYPE = 2,
                NICKNAME = "タナ",
                PHOTOKEY = Guid.NewGuid()
            };

            // 対象アカウント取得
            _accountRepo.Setup(m => m.ReadAccountIndexDat(targetAccountKey)).Returns(entity);

            return entity;
        }

        QoUserWriteApiArgs GetValidArgs()
        {
            return new QoUserWriteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",                
                User = new QoApiUserItem
                {
                    AccountKeyReference = _childKey.ToEncrypedReference(),
                    FamilyName = "山田",
                    GivenName = "一郎",
                    FamilyNameKana = "ヤマダ",
                    GivenNameKana = "イチロウ",
                    NickName = "イチロー",
                    Birthday = new DateTime(2010, 1, 1).ToApiDateString(),
                    Sex = 1,
                    PersonPhotoReference = Guid.NewGuid().ToEncrypedReference()
                }
            };
        }
    }
}
