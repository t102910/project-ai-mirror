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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class LinkagePatientCardAddWorkerFixture
    {
        LinkagePatientCardAddWorker _worker;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IAccountRepository> _accountRepo;
        Mock<IPatientCardRepository> _cardRepo;
        Mock<IStorageRepository> _storageRepo;
        Mock<IFamilyRepository> _familyRepo;

        Guid _accountKey = Guid.NewGuid();
        Guid _childAccountKey = Guid.NewGuid();
        Guid _executor = Guid.NewGuid();
        Guid _facilityKey = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec".ToDecrypedReference().TryToValueType(Guid.Empty);
        Guid _parentFacilityKey = Guid.NewGuid();

        [TestInitialize]
        public void Initlaize()
        {
            _linkageRepo = new Mock<ILinkageRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _cardRepo = new Mock<IPatientCardRepository>();
            _familyRepo = new Mock<IFamilyRepository>();
            _storageRepo = new Mock<IStorageRepository>();

            _worker = new LinkagePatientCardAddWorker(_linkageRepo.Object, _accountRepo.Object,_familyRepo.Object,_cardRepo.Object,_storageRepo.Object);
        }

        [TestMethod]
        public void AccountKeyが不正でエラー()
        {
            var args = GetValidArgs();
            args.ActorKey = "";

            var results = _worker.Add(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
        }

        [TestMethod]
        public void 生年月日が不正でエラー()
        {
            var args = GetValidArgs();
            args.Birthday = "";

            var results = _worker.Add(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Birthday)).IsTrue();
        }

        [TestMethod]
        public void 施設キーが不正でエラー()
        {
            var args = GetValidArgs();
            args.FacilityKeyReference = "";

            var results = _worker.Add(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FacilityKeyReference)).IsTrue();
        }

        [TestMethod]
        public void 性別が不正でエラー()
        {
            var args = GetValidArgs();
            args.SexType = "";

            var results = _worker.Add(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.SexType)).IsTrue();
        }

        [TestMethod]
        public void その他の引数が未設定でエラー終了する()
        {
            // FamilyNameが未入力
            var args = GetValidArgs();
            args.FamilyName = "";

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FamilyName));

            // GivenNameが未入力
            args = GetValidArgs();
            args.GivenName = "";

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.GivenName));

            // FamilyNameKanaが未入力
            args = GetValidArgs();
            args.FamilyKanaName = "";

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FamilyKanaName));

            // GivenNameKana が未入力
            args = GetValidArgs();
            args.GivenKanaName = "";

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.GivenKanaName));            

            // LinkageSystemId が未入力
            args = GetValidArgs();
            args.LinkUserId = "";

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.LinkUserId));
        }        

        [TestMethod]
        public void 施設キーから連携システム番号に変換できなければエラー終了()
        {
            var args = GetValidArgs();

            // 変換失敗
            _linkageRepo.Setup(m => m.GetLinkageNo(_facilityKey)).Returns(-1);

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("施設キーを連携システム番号に変換できませんでした").IsTrue();
        }

        [TestMethod]
        public void ユーザー情報が不一致でエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 生年月日が不一致
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = new DateTime(2000, 1, 2),
                SEXTYPE = 1
            });

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3003");
            results.Result.Detail.Contains("生年月日が登録情報と一致しません").IsTrue();

            // 性別が不一致
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = new DateTime(2000, 1, 1),
                SEXTYPE = 2
            });

            results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3003");
            results.Result.Detail.Contains("性別が登録情報と一致しません").IsTrue();
        }

        [TestMethod]
        public void ユーザー情報照合処理内部で例外発生したらエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 内部で例外発生させる
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("ユーザー情報照合処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ユーザー情報照合で姓名カナが違っても正常と判定する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = new DateTime(2000, 1, 1),
                SEXTYPE = 1,
                FAMILYNAME = "Hoge",
                GIVENNAME = "Fuga",
                FAMILYKANANAME = "フガ",
                GIVENKANANAME = "ホゲ"
            });

            var results = _worker.Add(args);

            // カード重複チェックが実行された
            _linkageRepo.Verify(m => m.IsAvailableCard(_facilityKey, args.LinkUserId), Times.Once);
            // ユーザー情報照合が通過したことだけ確認できれば良いので以降の処理はスルー
        }

        [TestMethod]
        public void 家族が対象であればユーザー情報照合は通らない()
        {
            var args = GetValidArgs();
            args.WithFamilyAccountRegistration = bool.TrueString;
            SetupValidMethods(args);            

            var results = _worker.Add(args);

            // アカウント照会処理は実行されなかった
            _accountRepo.Verify(m => m.ReadAccountIndexDat(_accountKey), Times.Never);
            // ユーザー情報照合が行われないことが確認できれば良いので以降の処理はスルー
        }

        [TestMethod]
        public void 家族アカウント作成で親アカウントが存在しない場合はエラー()
        {
            var args = GetValidArgs();
            args.WithFamilyAccountRegistration = bool.TrueString;
            SetupValidMethods(args);

            // nullを返すように設定
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(default(QH_ACCOUNT_MST));

            var results = _worker.Add(args);            

            results.IsSuccess.Is(bool.FalseString);

            // UserFamilyAddWorker内でのエラー
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("このアカウントは存在しません").IsTrue();
        }

        [TestMethod]
        public void 家族アカウント作成で親アカウントがプライベイトアカウントであればエラー()
        {
            var args = GetValidArgs();
            args.WithFamilyAccountRegistration = bool.TrueString;
            SetupValidMethods(args);

            // プライベートアカウントを返すように設定
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST { PRIVATEACCOUNTFLAG = true });

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            // UserFamilyAddWorker内でのエラー
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("このアカウントに子を追加することはできません").IsTrue();
        }

        [TestMethod]
        public void 家族アカウント作成で追加処理に失敗したらエラー()
        {
            var args = GetValidArgs();
            args.WithFamilyAccountRegistration = bool.TrueString;
            SetupValidMethods(args);

            // 家族追加処理で例外発生
            var birthDay = args.Birthday.TryToValueType(DateTime.MinValue);
            _familyRepo.Setup(m => m.AddFamily(_accountKey, It.IsAny<QoApiUserItem>(), birthDay, Guid.Empty)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            // UserFamilyAddWorker内でのエラー
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("家族アカウント追加処理でエラーが発生しました").IsTrue();
        }        

        

        [TestMethod]
        public void 診察券が既に誰かに登録されていればエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 重複ありと判定される
            _linkageRepo.Setup(m => m.IsAvailableCard(_facilityKey, args.LinkUserId)).Returns(false);

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3000");
            results.Result.Detail.Contains("この診察券番号は既に使われています").IsTrue();
        }

        [TestMethod]
        public void 診察券の重複チェック処理内部エラーでエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 内部エラー
            _linkageRepo.Setup(m => m.IsAvailableCard(_facilityKey, args.LinkUserId)).Throws(new Exception("Hoge"));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void 親施設連携処理で例外が発生するとエラー終了()
        {
            var args = GetValidArgs();            
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("上位施設の連携処理でエラーが発生しました").IsTrue();            
        }

        [TestMethod]
        public void 連携システム番号が指定されていなければ親施設連携はスキップされる
            ()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            args.LinkageSystemNo = "";

            var results = _worker.Add(args);

            // 親施設連携処理は行われなかった
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Never);

            // 次の診察券登録処理が実行された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false), Times.Once);
            // 上記2点でスキップが確認できれば良いので他はスルー
        }

        [TestMethod]
        public void 親施設連携処理で既に親連携済みであればスキップされる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 連携レコードあり
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 33333)).Returns(new QH_LINKAGE_DAT());

            var results = _worker.Add(args);

            // 親連携データ登録されなかった
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never);

            // 次の診察券登録処理が実行された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false), Times.Once);
            // 上記2点でスキップが確認できれば良いので他はスルー
        }        

        [TestMethod]
        public void 診察券登録処理で例外が発生するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void 診察券登録処理に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false)).Returns(("hoge",new QH_PATIENTCARD_DAT()));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("診察券の登録に失敗しました").IsTrue();
            results.Result.Detail.Contains("hoge").IsTrue();
        }

        [TestMethod]
        public void 登録した診察券の情報の取得に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 登録したはずの診察券が取得できない
            _cardRepo.Setup(m => m.ReadPatientCard(_accountKey, 27004, 1, 0)).Returns(default(QH_PATIENTCARD_FACILITY_VIEW));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("追加した診察券情報が取得できませんでした").IsTrue();
        }

        [TestMethod]
        public void 登録した診察券の情報の取得で例外が発生するとエラーとなる_家族登録込み()
        {
            var args = GetValidArgs();
            args.WithFamilyAccountRegistration = bool.TrueString;
            SetupValidMethods(args);

            // 診察券取得処理で例外
            _cardRepo.Setup(m => m.ReadPatientCard(_childAccountKey, 27004, 1, 0)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("診察券情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 全ての処理に成功する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.Add(args);
            
            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 親連携データが登録された
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Once);
            // 診察券データが登録された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false), Times.Once);

            // 家族作成は指定していないので何も入らない
            results.User.IsNull();
            results.Account.IsNull();
        }

        [TestMethod]
        public void 家族情報作成フラグありで全ての処理に成功する()
        {
            var args = GetValidArgs();
            args.WithFamilyAccountRegistration = bool.TrueString;
            SetupValidMethods(args);

            var results = _worker.Add(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");


            // 親連携データが登録された
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Once);
            // 診察券データが登録された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(_executor, _childAccountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false), Times.Once);
            // 家族データが作成された
            var birthDay = args.Birthday.TryToValueType(DateTime.MinValue);
            _familyRepo.Verify(m => m.AddFamily(_accountKey, It.IsAny<QoApiUserItem>(), birthDay,Guid.Empty), Times.Once);

            // ユーザー情報が正しく設定されている
            var user = results.User;
            user.FamilyName.Is(args.FamilyName);
            user.GivenName.Is(args.GivenName);
            user.FamilyNameKana.Is(args.FamilyKanaName);
            user.GivenNameKana.Is(args.GivenKanaName);
            user.NickName.Is(args.NickName);
            user.Birthday.Is(args.Birthday);
            user.Sex.Is(args.SexType.TryToValueType((byte)0));
            user.AccessKey.IsNotNull(); // 何か生成されていれば良しとする
            user.AccountKeyReference.Is(_childAccountKey.ToEncrypedReference());

            // アカウント情報(互換用)が正しく設定されている
            var acc = results.Account;
            acc.FamilyName.Is(args.FamilyName);
            acc.GivenName.Is(args.GivenName);
            acc.FamilyNameKana.Is(args.FamilyKanaName);
            acc.GivenNameKana.Is(args.GivenKanaName);
            acc.Birthday.Is(args.Birthday);
            acc.Sex.Is(args.SexType);
            acc.AccessKey.IsNotNull(); // 何か生成されていれば良しとする
            acc.AccountKeyReference.Is(_childAccountKey.ToEncrypedReference());
            acc.QolmsSsoAccessKey.Is(""); // Ssoキーを家族情報に乗せるのは廃止
        }

        [TestMethod]
        public void 連携システム番号が未指定でも正常終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            args.LinkageSystemNo = "";

            var results = _worker.Add(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 親連携データは登録されない
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never);
            // 診察券データが登録された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false), Times.Once);
        }

        void SetupValidMethods(QoLinkagePatientCardAddApiArgs args)
        {
            // 施設キー変換
            _linkageRepo.Setup(m => m.GetLinkageNo(_facilityKey)).Returns(27004);

            // ユーザー情報
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = new DateTime(2000,1,1),
                SEXTYPE = 1
            });

            // 子アカウント情報
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_childAccountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                BIRTHDAY = new DateTime(2000, 1, 1),
                SEXTYPE = 1
            });

            // アカウント情報を返す
            _accountRepo.Setup(m => m.ReadMasterEntity(_accountKey)).Returns(new QH_ACCOUNT_MST
            {
                ACCOUNTKEY = _accountKey,
                PRIVATEACCOUNTFLAG = false,
                DELETEFLAG = false
            });

            // 家族アカウント作成成功
            var birthDay = args.Birthday.TryToValueType(DateTime.MinValue);
            _familyRepo.Setup(m => m.AddFamily(_accountKey, It.IsAny<QoApiUserItem>(), birthDay, Guid.Empty)).Returns(_childAccountKey);

            // 重複チェックOK
            _linkageRepo.Setup(m => m.IsAvailableCard(_facilityKey, args.LinkUserId)).Returns(true);

            // 親連携マスタ情報
            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = 33333,
                FACILITYKEY = _parentFacilityKey
            });

            // 親は未連携
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 33333)).Returns(default(QH_LINKAGE_DAT));
            _linkageRepo.Setup(m => m.ReadEntity(_childAccountKey, 33333)).Returns(default(QH_LINKAGE_DAT));

            // 診察券登録成功
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false)).Returns(("",new QH_PATIENTCARD_DAT
            {
                SEQUENCE = 1
            }));
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _childAccountKey, 27004, _facilityKey, args.LinkUserId, int.MinValue, false)).Returns(("", new QH_PATIENTCARD_DAT
            {
                SEQUENCE = 1
            }));

            // 診察券情報取得（取得処理自体のテストは LinkagePatientCardListWorkerFixture の方で行うのでここでは省略）
            _cardRepo.Setup(m => m.ReadPatientCard(_accountKey, 27004, 1, 0)).Returns(new MGF.QOLMS.QolmsOpenApi.Sql.QH_PATIENTCARD_FACILITY_VIEW());

            _cardRepo.Setup(m => m.ReadPatientCard(_childAccountKey, 27004, 1, 0)).Returns(new MGF.QOLMS.QolmsOpenApi.Sql.QH_PATIENTCARD_FACILITY_VIEW());
        }

        QoLinkagePatientCardAddApiArgs GetValidArgs()
        {
            return new QoLinkagePatientCardAddApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                LinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO.ToString(),
                Birthday = new DateTime(2000, 1, 1).ToApiDateString(),
                FacilityKeyReference = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec",
                FamilyName = "患者",
                GivenName = "太郎",
                FamilyKanaName = "カンジャ",
                GivenKanaName = "タロウ",
                NickName = "カン",
                SexType = "1",
                LinkUserId = "88880001",
                WithFamilyAccountRegistration = bool.FalseString
            };
        }
    }
}
