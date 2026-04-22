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
    public class UserListReadWorkerFixture
    {
        Mock<IFamilyRepository> _familyRepo;
        UserListReadWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _childKey1 = Guid.NewGuid();
        Guid _childKey2 = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _familyRepo = new Mock<IFamilyRepository>();
            _worker = new UserListReadWorker(_familyRepo.Object);
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
        public void 家族リスト結果0件の場合はエラーとなる()
        {
            var args = GetValidArgs();

            // 0件を返す設定
            _familyRepo.Setup(m => m.ReadFamilyList(_accountKey)).Returns(new List<QH_ACCOUNTINDEX_DAT>());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005"); 
            results.Result.Detail.Contains("アカウント情報が存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void 家族リスト取得で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();

            // 取得処理で例外
            _familyRepo.Setup(m => m.ReadFamilyList(_accountKey)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("DBからのアカウント情報リストの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常に家族リストが取得できる()
        {
            var args = GetValidArgs();
            var family = SetupValidMethods();

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            
            var user1 = results.UserList[0];
            // 1人目のデータが一致している
            user1.AccountKeyReference.Is(_accountKey.ToEncrypedReference());
            user1.FamilyName.Is("山田");
            user1.GivenName.Is("太郎");
            user1.FamilyNameKana.Is("ヤマダ");
            user1.GivenNameKana.Is("タロウ");
            user1.NickName.Is("タロ");
            user1.Birthday.Is(new DateTime(1990, 1, 1).ToApiDateString());
            user1.Sex.Is((byte)1);
            user1.PersonPhotoReference.Is(string.Empty);
            user1.AccessKey.Is(string.Empty); // 親はアクセスキーは指定されない

            var user2 = results.UserList[1];
            // 2人目のデータが一致している
            user2.AccountKeyReference.Is(_childKey1.ToEncrypedReference());
            user2.FamilyName.Is("山田");
            user2.GivenName.Is("一郎");
            user2.FamilyNameKana.Is("ヤマダ");
            user2.GivenNameKana.Is("イチロウ");
            user2.NickName.Is("イチ");
            user2.Birthday.Is(new DateTime(2010, 1, 1).ToApiDateString());
            user2.Sex.Is((byte)1);
            user2.PersonPhotoReference.Is(family[1].PHOTOKEY.ToEncrypedReference());
            user2.AccessKey.IsNot(string.Empty); // アクセスキーが設定されている

            var user3 = results.UserList[2];
            // 3人目のデータが一致している
            user3.AccountKeyReference.Is(_childKey2.ToEncrypedReference());
            user3.FamilyName.Is("山田");
            user3.GivenName.Is("花子");
            user3.FamilyNameKana.Is("ヤマダ");
            user3.GivenNameKana.Is("ハナコ");
            user3.NickName.Is("ハナ");
            user3.Birthday.Is(new DateTime(2015, 1, 1).ToApiDateString());
            user3.Sex.Is((byte)2);
            user3.PersonPhotoReference.Is(family[2].PHOTOKEY.ToEncrypedReference());
            user3.AccessKey.IsNot(string.Empty); // アクセスキーが設定されている
        }

        List<QH_ACCOUNTINDEX_DAT> SetupValidMethods()
        {
            var family = new List<QH_ACCOUNTINDEX_DAT>
            {
                new QH_ACCOUNTINDEX_DAT
                {
                    ACCOUNTKEY = _accountKey,
                    FAMILYNAME =  "山田".TryEncrypt(),
                    GIVENNAME = "太郎".TryEncrypt(),
                    FAMILYKANANAME = "ヤマダ".TryEncrypt(),
                    GIVENKANANAME = "タロウ".TryEncrypt(),
                    NICKNAME = "タロ".TryEncrypt(),
                    BIRTHDAY = new DateTime(1990,1,1),
                    SEXTYPE = 1,
                    PHOTOKEY = Guid.Empty,
                },
                new QH_ACCOUNTINDEX_DAT
                {
                    ACCOUNTKEY = _childKey1,
                    FAMILYNAME =  "山田".TryEncrypt(),
                    GIVENNAME = "一郎".TryEncrypt(),
                    FAMILYKANANAME = "ヤマダ".TryEncrypt(),
                    GIVENKANANAME = "イチロウ".TryEncrypt(),
                    NICKNAME = "イチ".TryEncrypt(),
                    BIRTHDAY = new DateTime(2010,1,1),
                    SEXTYPE = 1,
                    PHOTOKEY = Guid.NewGuid(),
                },
                new QH_ACCOUNTINDEX_DAT
                {
                    ACCOUNTKEY = _childKey2,
                    FAMILYNAME =  "山田".TryEncrypt(),
                    GIVENNAME = "花子".TryEncrypt(),
                    FAMILYKANANAME = "ヤマダ".TryEncrypt(),
                    GIVENKANANAME = "ハナコ".TryEncrypt(),
                    NICKNAME = "ハナ".TryEncrypt(),
                    BIRTHDAY = new DateTime(2015,1,1),
                    SEXTYPE = 2,
                    PHOTOKEY = Guid.NewGuid(),
                },
            };

            _familyRepo.Setup(m => m.ReadFamilyList(_accountKey)).Returns(family);

            return family;
        }


        QoUserListReadApiArgs GetValidArgs()
        {
            return new QoUserListReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
            };
        }
    }
}
