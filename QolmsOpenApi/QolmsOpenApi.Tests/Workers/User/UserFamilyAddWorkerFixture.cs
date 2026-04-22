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
    public class UserFamilyAddWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        Mock<IFamilyRepository> _familyRepo;
        Mock<IStorageRepository> _storageRepo;

        UserFamilyAddWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _childKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _familyRepo = new Mock<IFamilyRepository>();
            _storageRepo = new Mock<IStorageRepository>();

            _worker = new UserFamilyAddWorker(_accountRepo.Object, _familyRepo.Object, _storageRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 生年月日の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.User.Birthday = "Hoge";

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.Birthday)}が不正です");
        }

        [TestMethod]
        public void 性別の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.User.Sex = 99;

            var results = _worker.Add(args);

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

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.FamilyName)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // 漢字名なし
            args = GetValidArgs();
            args.User.GivenName = string.Empty;

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.GivenName)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // カナ名なし
            args = GetValidArgs();
            args.User.FamilyNameKana = string.Empty;

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.FamilyNameKana)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // カナ姓なし
            args = GetValidArgs();
            args.User.GivenNameKana = string.Empty;

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.User.GivenNameKana)}").IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();
        }

        [TestMethod]
        public void 親アカウントが存在しない場合はエラーとなる()
        {
            var args = GetValidArgs();

            // アカウントが存在しない
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(default(QH_ACCOUNT_MST));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("このアカウントは存在しません").IsTrue();
        }

        [TestMethod]
        public void 親アカウントではなかった場合はエラーとなる()
        {
            var args = GetValidArgs();

            // 対象アカウントがプライベートアカウントだった
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                PRIVATEACCOUNTFLAG = true
            });

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("このアカウントに子を追加することはできません").IsTrue();
        }

        [TestMethod]
        public void 親アカウント確認処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();

            // 例外発生
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 家族追加処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var photoKey = args.User.PersonPhotoReference.ToDecrypedReference<Guid>();

            // 家族追加で例外を発生させる
            _familyRepo.Setup(m => m.AddFamily(_accountKey, args.User, new DateTime(2010, 1, 1), photoKey)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("家族アカウント追加処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 正常に家族が追加される_写真あり()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var user = results.User;

            user.FamilyName.Is("山田");
            user.GivenName.Is("一郎");
            user.FamilyNameKana.Is("ヤマダ");
            user.GivenNameKana.Is("イチロウ");
            user.NickName.Is("イチロー");
            user.Birthday.Is(new DateTime(2010, 1, 1).ToApiDateString());
            user.Sex.Is((byte)1);
            user.PersonPhotoReference.Is(args.User.PersonPhotoReference);
            user.AccountKeyReference.Is(_childKey.ToEncrypedReference());
            user.AccessKey.IsNot(string.Empty);

            var photoKey = args.User.PersonPhotoReference.ToDecrypedReference<Guid>();
            // 画像のメタデータ更新が行われた
            _storageRepo.Verify(m => m.UpdateImageMetaDataAccountKey(photoKey, _childKey), Times.Once);
        }

        [TestMethod]
        public void 正常に家族が追加される_写真なし()
        {
            var args = GetValidArgs();
            args.User.PersonPhotoReference = string.Empty;
            SetupValidMethods(args);

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var user = results.User;

            user.FamilyName.Is("山田");
            user.GivenName.Is("一郎");
            user.FamilyNameKana.Is("ヤマダ");
            user.GivenNameKana.Is("イチロウ");
            user.NickName.Is("イチロー");
            user.Birthday.Is(new DateTime(2010, 1, 1).ToApiDateString());
            user.Sex.Is((byte)1);
            user.PersonPhotoReference.Is(string.Empty);
            user.AccountKeyReference.Is(_childKey.ToEncrypedReference());
            user.AccessKey.IsNot(string.Empty);

            var photoKey = args.User.PersonPhotoReference.ToDecrypedReference<Guid>();
            // 画像のメタデータ更新は行われなかった
            _storageRepo.Verify(m => m.UpdateImageMetaDataAccountKey(photoKey, _childKey), Times.Never);
        }

        void SetupValidMethods(QoUserFamilyAddApiArgs args)
        {
            // 親アカウントチェック通過用
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                PRIVATEACCOUNTFLAG = false
            });

            var photoKey = args.User.PersonPhotoReference.ToDecrypedReference<Guid>();
            _familyRepo.Setup(m => m.AddFamily(_accountKey, args.User, new DateTime(2010, 1, 1), photoKey)).Returns(_childKey);
        }

        QoUserFamilyAddApiArgs GetValidArgs()
        {
            return new QoUserFamilyAddApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                User = new QoApiUserItem
                {
                    FamilyName = "山田",
                    GivenName = "一郎",
                    FamilyNameKana = "ヤマダ",
                    GivenNameKana = "イチロウ",
                    NickName = "イチロー",
                    Birthday = new DateTime(2010,1,1).ToApiDateString(),
                    Sex = 1,
                    PersonPhotoReference = Guid.NewGuid().ToEncrypedReference()
                }
            };
        }
    }
}
