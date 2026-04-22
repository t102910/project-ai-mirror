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
    public class UserReadWorkerFixture
    {
        Mock<IAccountRepository> _accountRepo;
        Mock<IFamilyRepository> _familyRepo;
        Mock<IStorageRepository> _storageRepo;
        Mock<IPasswordManagementRepository> _passwordRepo;

        UserReadWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _childKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _familyRepo = new Mock<IFamilyRepository>();
            _storageRepo = new Mock<IStorageRepository>();
            _passwordRepo = new Mock<IPasswordManagementRepository>();

            _worker = new UserReadWorker(_accountRepo.Object, _familyRepo.Object, _storageRepo.Object, _passwordRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Read(args);

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

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.AccountKeyReference)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 親子関係が成立していない場合はエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 親子関係ではない
            _familyRepo.Setup(m => m.IsParentChildRelation(_accountKey, _childKey)).Returns(false);

            var results = _worker.Read(args);
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

            var results = _worker.Read(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント関連情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 対象アカウント情報処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外を発生させる
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_childKey)).Throws(new Exception());

            var results = _worker.Read(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 対象が親の時にメールアドレスの取得処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            // 親をターゲットに
            args.AccountKeyReference = _accountKey.ToEncrypedReference();
            SetupValidMethods(args);

            // 例外を発生させる
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Throws(new Exception());

            var results = _worker.Read(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("メールアドレス情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 対象が親の時にメールアドレスの取得処理でレコードが取得できなかったらエラーとなる()
        {
            var args = GetValidArgs();
            // 親をターゲットに
            args.AccountKeyReference = _accountKey.ToEncrypedReference();
            SetupValidMethods(args);

            // 対象レコードなし
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Returns(default(QH_PASSWORDMANAGEMENT_DAT));

            var results = _worker.Read(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("メールアドレス情報が存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void 対象が親の時に電話番号の取得処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();
            // 親をターゲットに
            args.AccountKeyReference = _accountKey.ToEncrypedReference();
            SetupValidMethods(args);

            // 例外を発生させる
            _accountRepo.Setup(m => m.ReadPhoneEntity(_accountKey)).Throws(new Exception());

            var results = _worker.Read(args);
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("電話番号の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常にユーザー情報を取得できる_子()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var user = results.User;
            // ユーザー情報が正しく設定されている
            user.AccountKeyReference.Is(_childKey.ToEncrypedReference());
            user.FamilyName.Is(entity.FAMILYNAME);
            user.GivenName.Is(entity.GIVENNAME);
            user.FamilyNameKana.Is(entity.FAMILYKANANAME);
            user.GivenNameKana.Is(entity.GIVENKANANAME);
            user.NickName.Is(entity.NICKNAME);
            user.Birthday.Is(entity.BIRTHDAY.ToApiDateString());
            user.Sex.Is(entity.SEXTYPE);
            user.AccessKey.IsNot(string.Empty);
            user.PersonPhotoReference.Is(entity.PHOTOKEY.ToEncrypedReference());
            user.Mail.Is(string.Empty); // 子にはメールは設定されない
            user.AccountPhoneNumber.Is(string.Empty); // 子には電話番号は設定されない
            user.LoginId.Is(string.Empty);

        }

        [TestMethod]
        public void 正常にユーザー情報を取得できる_親()
        {
            var args = GetValidArgs();
            // 対象を親にする
            args.AccountKeyReference = _accountKey.ToEncrypedReference();
            var entity = SetupValidMethods(args);            

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var user = results.User;
            // ユーザー情報が正しく設定されている
            user.AccountKeyReference.Is(_accountKey.ToEncrypedReference());
            user.FamilyName.Is(entity.FAMILYNAME);
            user.GivenName.Is(entity.GIVENNAME);
            user.FamilyNameKana.Is(entity.FAMILYKANANAME);
            user.GivenNameKana.Is(entity.GIVENKANANAME);
            user.NickName.Is(entity.NICKNAME);
            user.Birthday.Is(entity.BIRTHDAY.ToApiDateString());
            user.Sex.Is(entity.SEXTYPE);
            user.AccessKey.Is(string.Empty); // 親はアクセスキーは設定されない
            user.PersonPhotoReference.Is(entity.PHOTOKEY.ToEncrypedReference());
            user.Mail.Is("hoge@abc.com");
            user.AccountPhoneNumber.Is("09099996666");
            user.LoginId.Is("TaroYamada01");
        }

        [TestMethod]
        public void 正常にユーザー情報を取得できる_親_ターゲット省略()
        {
            var args = GetValidArgs();
            // 対象未設定にすることでも親をターゲットにすることができる
            args.AccountKeyReference = string.Empty;
            var entity = SetupValidMethods(args);

            _passwordRepo.Setup(m => m.ReadDecryptedEntity(_accountKey)).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                PASSWORDRECOVERYMAILADDRESS = "hoge@abc.com",
                USERID = "TaroYamada01"
            });

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var user = results.User;
            // ユーザー情報が正しく設定されている
            user.AccountKeyReference.Is(_accountKey.ToEncrypedReference());
            user.FamilyName.Is(entity.FAMILYNAME);
            user.GivenName.Is(entity.GIVENNAME);
            user.FamilyNameKana.Is(entity.FAMILYKANANAME);
            user.GivenNameKana.Is(entity.GIVENKANANAME);
            user.NickName.Is(entity.NICKNAME);
            user.Birthday.Is(entity.BIRTHDAY.ToApiDateString());
            user.Sex.Is(entity.SEXTYPE);
            user.AccessKey.Is(string.Empty); // 親はアクセスキーは設定されない
            user.PersonPhotoReference.Is(entity.PHOTOKEY.ToEncrypedReference());
            user.Mail.Is("hoge@abc.com");
            user.LoginId.Is("TaroYamada01");

        }


        QH_ACCOUNTINDEX_DAT SetupValidMethods(QoUserReadApiArgs args)
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

            // メール取得
            _passwordRepo.Setup(m => m.ReadDecryptedEntity(targetAccountKey)).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                PASSWORDRECOVERYMAILADDRESS = "hoge@abc.com",
                USERID = "TaroYamada01"
            });

            // 電話番号取得
            _accountRepo.Setup(m => m.ReadPhoneEntity(targetAccountKey)).Returns(new QH_ACCOUNTPHONE_MST
            {
                PHONENUMBER = "09099996666"
            });

            return entity;
        }

        QoUserReadApiArgs GetValidArgs()
        {
            return new QoUserReadApiArgs
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
