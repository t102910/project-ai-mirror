using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
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
    /// <summary>
    /// LinkageRegisterWorkerのテスト
    /// 基本的にソースの順番で異常系をテストしていき最後に正常系を確認しています。
    /// 
    /// </summary>
    [TestClass]
    public class LinkageRegisterWorkerFixture
    {
        LinkageRegisterWorker _worker;
        Mock<IIdentityApiRepository> _identityApi;
        Mock<ISignUpRepository> _signUpRepo;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<INoticeApiRepository> _noticeApi;
        Mock<IAccountRepository> _accountRepo;

        Guid _newAccountKey;
        Guid _newFamilyAccountKey;
        Guid _newLinkageSystemId = Guid.Empty;
        

        [TestInitialize]
        public void Initialize()
        {
            _identityApi = new Mock<IIdentityApiRepository>();
            _signUpRepo = new Mock<ISignUpRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _noticeApi = new Mock<INoticeApiRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _newAccountKey = Guid.NewGuid();
            _newFamilyAccountKey = Guid.NewGuid();

            _worker = new LinkageRegisterWorker(_linkageRepo.Object, _identityApi.Object, _signUpRepo.Object, _noticeApi.Object, _accountRepo.Object);
        }

        [TestMethod]
        public void LinkageSystemNoが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.LinkageSystemNo = "X0001";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.LinkageSystemNo));
        }

        [TestMethod]
        public void 仮アカウントキーが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.AccountKeyReference = "Hoge";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AccountKeyReference));
        }

        [TestMethod]
        public void 施設キー参照が不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.FacilityKeyReference = "Hoge";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FacilityKeyReference));
        }

        [TestMethod]
        public void 生年月日の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.Birthday = "Hoge";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.Birthday)}が不正です");
        }

        [TestMethod]
        public void 性別の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.Sex = "Hoge";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.Sex)}が不正です");
        }

        [TestMethod]
        public void その他の引数が未設定でエラー終了する()
        {
            // FamilyNameが未入力
            var args = GetValidArgs();
            args.FamilyName = "";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FamilyName)).IsTrue();

            // GivenNameが未入力
            args = GetValidArgs();
            args.GivenName = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.GivenName)).IsTrue();

            // FamilyNameKanaが未入力
            args = GetValidArgs();
            args.FamilyNameKana = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FamilyNameKana)).IsTrue();

            // GivenNameKana が未入力
            args = GetValidArgs();
            args.GivenNameKana = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.GivenNameKana)).IsTrue();

            // Password が未入力
            args = GetValidArgs();
            args.Password = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Password)).IsTrue();

            // Sex が未入力
            args = GetValidArgs();
            args.Sex = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Sex)).IsTrue();

            // Birthday が未入力
            args = GetValidArgs();
            args.Birthday = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Birthday)).IsTrue();

            // LinkageSystemId が未入力
            args = GetValidArgs();
            args.LinkageSystemId = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.LinkageSystemId)).IsTrue();
        }

        [TestMethod]
        public void 規約の同意がないとエラー終了する()
        {
            // プライバシーポリシー が同意されていない
            var args = GetValidArgs();
            args.IsAgreePrivacyPolicy = bool.FalseString;

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("プライバシーポリシー").IsTrue();

            // 利用規約 が同意されていない
            args = GetValidArgs();
            args.IsAgreeTermsOfService = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("利用規約").IsTrue();
        }

        [TestMethod]
        public void 家族情報が設定されていれば家族関連引数エラーで終了する()
        {
            // 漢字姓が未設定
            var args = GetValidArgs();
            args.FamilyAccountInfo.FamilyName = "";

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("漢字姓が不正です").IsTrue();

            // 漢字名が未設定
            args = GetValidArgs();
            args.FamilyAccountInfo.GivenName = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("漢字名が不正です").IsTrue();

            // カナ姓が未設定
            args = GetValidArgs();
            args.FamilyAccountInfo.FamilyNameKana = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("カナ姓が不正です").IsTrue();

            // カナ名が未設定
            args = GetValidArgs();
            args.FamilyAccountInfo.GivenNameKana = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("カナ名が不正です").IsTrue();

            // 性別が未設定
            args = GetValidArgs();
            args.FamilyAccountInfo.Sex = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("性別が不正です").IsTrue();

            // 生年月日が未設定
            args = GetValidArgs();
            args.FamilyAccountInfo.Birthday = "";

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("生年月日が不正です").IsTrue();
        }

        [TestMethod]
        public void 家族情報が未設定であれば家族情報チェックは通らない()
        {
            var args = GetValidArgs();
            args.FamilyAccountInfo = null;

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 施設キー変換失敗
            _linkageRepo.Setup(m => m.GetLinkageNo(facilityKey)).Returns(-1);

            var results = _worker.RegisterNewUser(args);

            // 家族情報のエラーではひかからず施設キー変換処理でこける
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("施設キーを連携システム番号に変換できませんでした").IsTrue();
        }

        [TestMethod]
        public void 施設キーから連携システム番号に変換できなければエラー終了()
        {
            var args = GetValidArgs();

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 変換失敗
            _linkageRepo.Setup(m => m.GetLinkageNo(facilityKey)).Returns(-1);

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("施設キーを連携システム番号に変換できませんでした").IsTrue();
        }

        [TestMethod]
        public void 仮登録データの取得失敗でエラー終了()
        {
            var args = GetValidArgs();
            var preAccountkey = args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            _identityApi.Setup(m => m.ExecuteRegisterReadApi(preAccountkey)).Returns(new QiQolmsAccountRegisterReadApiResults
            {
                IsSuccess = bool.FalseString
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("仮登録データの取得に失敗").IsTrue();
        }

        [TestMethod]
        public void 仮登録データのメールアドレスが未設定だとエラー終了()
        {
            // 

            var args = GetValidArgs();
            var preAccountkey = args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            _identityApi.Setup(m => m.ExecuteRegisterReadApi(preAccountkey)).Returns(new QiQolmsAccountRegisterReadApiResults
            {
                IsSuccess = bool.TrueString,
                MailAddress = "",
                Expires = DateTime.Now.AddDays(30).ToApiDateString()
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3001");
            results.Result.Detail.Contains("仮登録データが見つかりませんでした").IsTrue();
        }

        [TestMethod]
        public void 仮登録データの期限切れでエラー終了()
        {
            // シンプルに期限切れの場合
            var args = GetValidArgs();
            var preAccountkey = args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            _identityApi.Setup(m => m.ExecuteRegisterReadApi(preAccountkey)).Returns(new QiQolmsAccountRegisterReadApiResults
            {
                IsSuccess = bool.TrueString,
                MailAddress = "hoge@abc.com",
                Expires = DateTime.Now.AddSeconds(-1).ToApiDateString()
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("2004");
            results.Result.Detail.Contains("仮登録の期限").IsTrue();


            // 期限に空文字の場合
            args = GetValidArgs();

            _identityApi.Setup(m => m.ExecuteRegisterReadApi(preAccountkey)).Returns(new QiQolmsAccountRegisterReadApiResults
            {
                IsSuccess = bool.TrueString,
                MailAddress = "hoge@abc.com",
                Expires = ""
            });

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("2004");
            results.Result.Detail.Contains("仮登録の期限").IsTrue();
        }

        [TestMethod]
        public void 退会後に規定時間以内で再登録の場合はエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            var interval = TimeSpan.FromMinutes(QoApiConfiguration.NewUserRegisterInterval - 1);
            var updated = DateTime.Now - interval;

            _linkageRepo.Setup(m => m.GetLinkageUpdated(facilityKey, args.LinkageSystemId)).Returns(updated);

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("2999");
            // 詳細にはConfigの値が入る
            results.Result.Detail.Contains(QoApiConfiguration.NewUserRegisterInterval.ToString()).IsTrue();
        }

        [TestMethod]
        public void 退会後規定時間チェック内でエラーが発生したらエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            var interval = TimeSpan.FromMinutes(QoApiConfiguration.NewUserRegisterInterval - 1);
            var updated = DateTime.Now - interval;

            _linkageRepo.Setup(m => m.GetLinkageUpdated(facilityKey, args.LinkageSystemId)).Throws(new Exception("hoge"));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void 診察券が既に誰かに登録されていればエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 重複ありと判定される
            _linkageRepo.Setup(m => m.IsAvailableCard(facilityKey, args.LinkageSystemId)).Returns(false);

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3000");
            results.Result.Detail.Contains("この診察券番号は既に使われています").IsTrue();
        }

        [TestMethod]
        public void 診察券の重複チェック処理内部エラーでエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 内部エラー
            _linkageRepo.Setup(m => m.IsAvailableCard(facilityKey, args.LinkageSystemId)).Throws(new Exception("Hoge"));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void 既に同一情報で登録されているユーザーが存在すればエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            var sex = args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);

            // 1件存在する
            _accountRepo.Setup(m => m.GetRegisteredAccountId("hoge@abc.com", args.FamilyName, args.GivenName, birthDate, sex)).Returns((1, "12345678"));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3002"); // ユーザー重複エラー
            results.Result.Detail.Contains("同一登録情報のユーザーが既に存在します").IsTrue();
        }

        [TestMethod]
        public void 同一情報ユーザーチェックで異常が返るとエラー終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            var sex = args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);

            // 異常が返る設定
            _accountRepo.Setup(m => m.GetRegisteredAccountId("hoge@abc.com", args.FamilyName, args.GivenName, birthDate, sex)).Returns((int.MinValue, string.Empty));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("同一登録情報重複チェックでエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ユーザーID10桁を自動生成できる()
        {
            var ids = new List<string>();
            // 10万回生成する
            for(var i = 0;i < 100000; i++)
            {
                var id = _worker.GenerateUserId();

                // IOZを除く英数10桁で構成されている
                Regex.IsMatch(id, @"[0-9A-HJ-NP-Y]{10}").IsTrue();

                ids.Add(id);
            }

            // 重複なしで10万件生成できている
            ids.Distinct().Count().Is(100000);
        }

        [TestMethod]
        public void 本登録で10回重複するとエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 重複エラーを設定
            _identityApi.Setup(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, "hoge@abc.com")).Returns(new IdentityRegisterApiResults
            {
                IsSuccess = false,
                ErrorList = new List<string> { "UserId" }
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("2003"); // 重複エラー
            results.Result.Detail.Contains("このユーザーIDは既に使用").IsTrue();
        }

        [TestMethod]
        public void 本登録で重複以外のエラーの場合はリトライせずにエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 重複以外のエラー
            _identityApi.Setup(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, "hoge@abc.com")).Returns(new IdentityRegisterApiResults
            {
                IsSuccess = false,
                ErrorList = new List<string> { "Hoge","Fuga" }
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            // エラーリストの文字列が含まれている

            results.Result.Detail.Contains("Hoge").IsTrue(); 
            results.Result.Detail.Contains("Fuga").IsTrue();
        }

        [TestMethod]
        public void 本登録処理でアカウントキーが設定されなかったらエラー()
        {
            // 通常ありえないパターン
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 成功するがアカウントキーが設定されていない
            _identityApi.Setup(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, "hoge@abc.com")).Returns(new IdentityRegisterApiResults
            {
                IsSuccess = true,
                AccountKey = Guid.Empty.ToApiGuidString()
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("本登録処理でアカウントキーが返されませんでした").IsTrue();
        }

        [TestMethod]
        public void 本登録処理内で例外が発生したらエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 成功するがアカウントキーが設定されていない
            _identityApi.Setup(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, "hoge@abc.com")).Throws(new Exception("Hoge"));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void Push通知IDの更新で例外が発生したらエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.ReadEntity(_newAccountKey, QoLinkage.TIS_LINKAGE_SYSTEM_NO)).Throws(new Exception());

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("Push通知IDの更新に失敗しました").IsTrue();


            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
        }

        [TestMethod]
        public void 本登録後の家族登録処理で失敗するとエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // 家族登録失敗
            _identityApi.Setup(m => m.ExecuteAccountConnectFamilyAccountEditWriteApi(_newAccountKey, args.ExecutorName, args.FamilyAccountInfo, args.FamilyAccountInfo.Sex, args.FamilyAccountInfo.Birthday)).Returns(new QiQolmsAccountConnectFamilyAccountEditWriteApiResults { IsSuccess = bool.FalseString });

            // 退会処理成功
            _linkageRepo.Setup(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理")).Returns(true);

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("家族アカウントの追加に失敗しました").IsTrue();            

            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
        }

        [TestMethod]
        public void 本登録後の家族登録処理で例外が発生するとエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // 家族登録例外
            _identityApi.Setup(m => m.ExecuteAccountConnectFamilyAccountEditWriteApi(_newAccountKey, args.ExecutorName, args.FamilyAccountInfo, args.FamilyAccountInfo.Sex, args.FamilyAccountInfo.Birthday)).Throws(new Exception("Hoge"));

            // 退会処理失敗
            _linkageRepo.Setup(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理")).Returns(false);

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");

            // 退会処理が実行された(失敗しても何もしない）
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
        }

        [TestMethod]
        public void 親施設の連携処理で失敗するとエラー終了しアカウントも削除される()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 親連携番号として33333を返す
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = 33333
            });
            // Insert処理で例外を起こす
            _linkageRepo.Setup(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>())).Callback((QH_LINKAGE_DAT entity) =>
            {                
                entity.ACCOUNTKEY.Is(_newAccountKey); 
                entity.LINKAGESYSTEMNO.Is(33333); // 親連携IDがentityにセットされた
            }).Throws(new Exception());

            var results = _worker.RegisterNewUser(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("上位施設の連携処理でエラーが発生しました").IsTrue();

            // 退会処理が実行された(失敗しても何もしない）
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
        }

        [TestMethod]
        public void 親施設の連携処理で親施設がない場合は処理がスキップされる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 親施設無し
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(default(QH_LINKAGESYSTEM_MST));
                         
            var results = _worker.RegisterNewUser(args);

            // 親連携登録処理を実行されなかった。
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never);
            // スキップされたことだけ確認できれば良いのでこれ以降はスルーする
        }


        [TestMethod]
        public void 親施設の連携処理で親施設と引数の連携システム番号が同じなら処理がスキップされる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            var rootLinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO;

            // 親施設が引数の連携システム番号と同じになるように
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = rootLinkageSystemNo
            });

            var results = _worker.RegisterNewUser(args);

            // 親連携登録処理を実行されなかった。
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never);
            // スキップされたことだけ確認できれば良いのでこれ以降はスルーする
        }

        [TestMethod]
        public void 本登録後に家族が未指定なら本人の診察券が登録されるが失敗時はエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 家族無し
            args.FamilyAccountInfo = null;

            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 退会処理失敗
            _linkageRepo.Setup(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理")).Returns(false);

            // 重複エラーを返す
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Returns(("指定された種類のカードは、すでに登録があります。",null));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3000"); // 重複エラー
            results.Result.Detail.Contains("指定された種類のカードは、すでに登録があります").IsTrue();
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
            _linkageRepo.Invocations.Clear();

            // その他のエラー
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Returns(("Hoge",null));


            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("Hoge").IsTrue(); // 内部エラーメッセージ
            results.Result.Detail.Contains("診察券の登録に失敗しました").IsTrue();
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
            _linkageRepo.Invocations.Clear();

            // 例外発生
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Throws(new Exception("Fuga"));

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
            _linkageRepo.Invocations.Clear();
        }

        [TestMethod]
        public void 本登録後に家族情報があれば家族の診察券が登録されるが失敗時はエラー終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 退会処理成功
            _linkageRepo.Setup(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理")).Returns(true);

            // 重複エラーを返す
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newFamilyAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Returns(("指定された種類のカードは、すでに登録があります。",null));

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3000"); // 重複エラー
            results.Result.Detail.Contains("指定された種類のカードは、すでに登録があります").IsTrue();
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
            _linkageRepo.Invocations.Clear();

            // その他のエラー
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newFamilyAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Returns(("Hoge",null));

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("Hoge").IsTrue(); // 内部エラーメッセージ
            results.Result.Detail.Contains("診察券の登録に失敗しました").IsTrue();
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
            _linkageRepo.Invocations.Clear();

            // 例外発生
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newFamilyAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Throws(new Exception("Fuga"));

            results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            // 退会処理が実行された
            _linkageRepo.Verify(m => m.WithdrawAccountOnRegister(executor, authorKey, _newAccountKey, 27003, 255, "新規連携ユーザー登録処理中での退会処理"), Times.Once);
            _linkageRepo.Invocations.Clear();
        }

        [TestMethod]
        public void 正常に本登録が完了する_HOSPA会員_本人の診察券()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            args.FamilyAccountInfo = null;
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);            

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // TIS会員 TS{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"TS[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();

            // 家族追加処理は行われなかった
            _identityApi.Verify(m => m.ExecuteAccountConnectFamilyAccountEditWriteApi(It.IsAny<Guid>(), args.ExecutorName, It.IsAny<QoApiAccountFamilyInputItem>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            // 親施設がTISで引数の連携システム番号と同じなので親連携はスキップされた
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never);

            // 本人の診察券が登録された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(authorKey, _newAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false), Times.Once);

            // 仮登録データは削除された
            _signUpRepo.Verify(m => m.DeleteSignUpData(args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty)), Times.Once);

            // メールが送信された
            _noticeApi.Verify(m => m.SendIdNotificationMail("hoge@abc.com", results.UserId, args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)), Times.Once);
        }

        [TestMethod]
        public void 正常に本登録が完了する_HOSPA会員_家族の診察券()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // TIS会員 TS{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"TS[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();

            // 家族追加処理が行われた
            _identityApi.Verify(m => m.ExecuteAccountConnectFamilyAccountEditWriteApi(_newAccountKey, args.ExecutorName, args.FamilyAccountInfo, args.FamilyAccountInfo.Sex, args.FamilyAccountInfo.Birthday), Times.Once);

            // 親施設がTISで引数の連携システム番号と同じなので親連携はスキップされた
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never);

            // 家族の診察券が登録された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(authorKey, _newFamilyAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false), Times.Once);

            // 仮登録データは削除された
            _signUpRepo.Verify(m => m.DeleteSignUpData(args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty)), Times.Once);

            // メールが送信された
            _noticeApi.Verify(m => m.SendIdNotificationMail("hoge@abc.com", results.UserId, args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)), Times.Once);
        }

        [TestMethod]
        public void 正常に本登録が完了する_医療ナビ会員_本人()
        {
            var args = GetValidArgs();
            // 医療ナビに変更
            args.LinkageSystemNo = QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO.ToString();
            SetupValidMethods(args);

            args.FamilyAccountInfo = null; // 家族なしに変更
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            
            // 親連携番号を返すように
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = 33333
            });
            _linkageRepo.Setup(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>())).Callback((QH_LINKAGE_DAT entity) =>
            {
                // 家族のアカウントで親施設に連携された
                entity.ACCOUNTKEY.Is(_newFamilyAccountKey);
                entity.LINKAGESYSTEMNO.Is(33333);
            });


            _linkageRepo.Setup(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>())).Callback((QH_LINKAGE_DAT entity) =>
            {
                // 本人のアカウントで親施設に連携された
                entity.ACCOUNTKEY.Is(_newAccountKey);
                entity.LINKAGESYSTEMNO.Is(33333);
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 医療ナビ会員 MN{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"MN[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();

            // 親施設とも連携された
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Once);

            // 仮登録データは削除された
            _signUpRepo.Verify(m => m.DeleteSignUpData(args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty)), Times.Once);

            // メールが送信された
            _noticeApi.Verify(m => m.SendIdNotificationMail("hoge@abc.com", results.UserId, args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)), Times.Once);

        }

        [TestMethod]
        public void 正常に本登録が完了する_医療ナビ会員_家族()
        {
            var args = GetValidArgs();
            // 医療ナビに変更
            args.LinkageSystemNo = QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO.ToString();
            SetupValidMethods(args);

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
           
            // 親連携番号を返すように
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = 33333
            });
            _linkageRepo.Setup(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>())).Callback((QH_LINKAGE_DAT entity) =>
            {
                // 家族のアカウントで親施設に連携された
                entity.ACCOUNTKEY.Is(_newFamilyAccountKey);
                entity.LINKAGESYSTEMNO.Is(33333);
            });

            var results = _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 医療ナビ会員 MN{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"MN[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();

            // 親施設とも連携された
            _linkageRepo.Verify(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Once);

            // 仮登録データは削除された
            _signUpRepo.Verify(m => m.DeleteSignUpData(args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty)), Times.Once);

            // メールが送信された
            _noticeApi.Verify(m => m.SendIdNotificationMail("hoge@abc.com", results.UserId, args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)), Times.Once);

        }

        [TestMethod]
        public void メールの送信に失敗しても正常扱いとする()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // メール送信処理内部で例外を起こす
            _noticeApi.Setup(m => m.SendIdNotificationMail("hoge@abc.com", It.IsAny<string>(), args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None))).ThrowsAsync(new Exception());

            var results = _worker.RegisterNewUser(args);


            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // TIS会員 TS{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"TS[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();

            // 仮登録データは削除された
            _signUpRepo.Verify(m => m.DeleteSignUpData(args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty)), Times.Once);

            // メール送信処理は実行されたが例外が起きた。Task内なので何も影響せず
            _noticeApi.Verify(m => m.SendIdNotificationMail("hoge@abc.com", results.UserId, args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)), Times.Once);
        }

        // 正常系を通過するようにMockを設定する
        void SetupValidMethods(QoLinkageNewUserRegisterApiArgs args)
        {
            // 仮登録チェック
            var preAccountkey = args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            
            _identityApi.Setup(m => m.ExecuteRegisterReadApi(preAccountkey)).Returns(new QiQolmsAccountRegisterReadApiResults
            {
                IsSuccess = bool.TrueString,
                MailAddress = "hoge@abc.com",
                Expires = DateTime.Now.AddDays(1).ToApiDateString()
            });

            // 退会後規定時間チェック
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            var interval = TimeSpan.FromMinutes(QoApiConfiguration.NewUserRegisterInterval + 30);
            var updated = DateTime.Now - interval;

            _linkageRepo.Setup(m => m.GetLinkageUpdated(facilityKey, args.LinkageSystemId)).Returns(updated);

            // 施設キー変換
            _linkageRepo.Setup(m => m.GetLinkageNo(facilityKey)).Returns(27004);

            // 診察券重複チェック
            _linkageRepo.Setup(m => m.IsAvailableCard(facilityKey, args.LinkageSystemId)).Returns(true);

            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            var sex = args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);
            
            // 同一情報登録ユーザーチェック
            _accountRepo.Setup(m => m.GetRegisteredAccountId("hoge@abc.com", args.FamilyName, args.GivenName, birthDate, sex)).Returns((0, string.Empty));

            // 本登録
            _identityApi.Setup(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, "hoge@abc.com")).Returns(new IdentityRegisterApiResults
            {
                IsSuccess = true,
                AccountKey = _newAccountKey.ToApiGuidString()
            })
            .Callback((string a,string linkageSystemId,string c,string d,string e,string f,string g,string h,string i,string j, string mailAddr) =>
            {
                mailAddr.Is("hoge@abc.com");
                _newLinkageSystemId = Guid.Parse(linkageSystemId);
            });

            var targetEntityForPushId = new QH_LINKAGE_DAT();
            // 連携システムデータを返す
            _linkageRepo.Setup(m => m.ReadEntity(_newAccountKey, args.LinkageSystemNo.TryToValueType(0))).Returns(targetEntityForPushId);
            _linkageRepo.Setup(m => m.UpdateEntity(targetEntityForPushId)).Callback((QH_LINKAGE_DAT dat) =>
            {
                // アカウントキーがPush通知IDとして登録された
                dat.LINKAGESYSTEMID.Is(_newAccountKey.ToString("N"));
            });

            // 家族登録
            _identityApi.Setup(m => m.ExecuteAccountConnectFamilyAccountEditWriteApi(_newAccountKey, args.ExecutorName, args.FamilyAccountInfo, args.FamilyAccountInfo.Sex, args.FamilyAccountInfo.Birthday)).Returns(new QiQolmsAccountConnectFamilyAccountEditWriteApiResults { IsSuccess = bool.TrueString, Accountkey = _newFamilyAccountKey });

            // 親連携番号としてTISを返す
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = QoLinkage.TIS_LINKAGE_SYSTEM_NO
            });

            // 親連携レコードは常に無し
            //_linkageRepo.Setup(m => m.ReadEntity(It.IsAny<Guid>(), It.IsAny<int>())).Returns(default(QH_LINKAGE_DAT));

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // 診察券登録（家族）
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newFamilyAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Returns(("",new QH_PATIENTCARD_DAT { SEQUENCE = 1}));
            // 診察券登録（本人）
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(authorKey, _newAccountKey, 27004, facilityKey, args.LinkageSystemId, int.MinValue, false)).Returns(("", new QH_PATIENTCARD_DAT { SEQUENCE = 1 }));

            _noticeApi.Setup(m => m.SendIdNotificationMail("hoge@abc.com", It.IsAny<string>(), args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None))).ReturnsAsync(true);
        }

        QoLinkageNewUserRegisterApiArgs GetValidArgs()
        {
            return new QoLinkageNewUserRegisterApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(), 
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                LinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO.ToString(),
                LinkageSystemId = "88880001",
                AccountKeyReference = "fdc0b64b3f5bfc32f9e23dedc7c777a285fa9f0e94662249513c1abcd0b3d6510c3291114c7e59a66bb0c760470f4c38a95fff7f7a85e109e3ed716bfe4db545",
                Birthday = new DateTime(2000, 1, 1).ToApiDateString(),
                FacilityKeyReference = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec",
                FamilyName = "患者",
                GivenName = "太郎",
                FamilyNameKana = "カンジャ",
                GivenNameKana = "タロウ",
                IsAgreePrivacyPolicy = bool.TrueString,
                IsAgreeTermsOfService = bool.TrueString,
                Password = "abc1234_",
                Sex = "1",             
                FamilyAccountInfo = new QoApiAccountFamilyInputItem
                {
                    Birthday = new DateTime(2010, 1, 1).ToApiDateString(),
                    FamilyName = "患者",
                    GivenName = "花子",
                    FamilyNameKana = "カンジャ",
                    GivenNameKana = "ハナコ",
                    Sex = $"{(int)QsDbSexTypeEnum.Female}"
                }
            };
        }
    }

   
}
