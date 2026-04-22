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
    /// LinkageRegisterForSmsWorkerの診察券なしバージョンのテスト
    /// 重複するテストは省略
    /// </summary>
    [TestClass]
    public class LinkageRegisterForSmsWorkerFixture2
    {
        LinkageRegisterForSmsWorker _worker;
        Mock<IIdentityApiRepository> _identityApi;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IAccountRepository> _accountRepo;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<IQoSmsClient> _smsClient;

        Guid _newAccountKey;
        Guid _newLinkageSystemId = Guid.Empty;
        Guid _authKey;
        Guid _facilityKey;

        [TestInitialize]
        public void Initialize()
        {
            _identityApi = new Mock<IIdentityApiRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _smsClient = new Mock<IQoSmsClient>();

            _newAccountKey = Guid.NewGuid();
            _authKey = Guid.NewGuid();

            _facilityKey = Guid.Parse("6EE88782-4192-4070-839F-D0E57F96BCD2");

            _worker = new LinkageRegisterForSmsWorker(_linkageRepo.Object, _identityApi.Object, _accountRepo.Object, _smsAuthCodeRepo.Object, _smsClient.Object);
        }

        [TestMethod]
        public async Task LinkageSystemNoが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.LinkageSystemNo = "X0001";

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.LinkageSystemNo));
        }

        [TestMethod]
        public async Task 認証キーが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.AuthKeyReference = "Hoge";

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AuthKeyReference));
        }

        [TestMethod]
        public async Task 生年月日の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.Birthday = "Hoge";

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.Birthday)}が不正です");
        }

        [TestMethod]
        public async Task 性別の変換エラーで終了する()
        {
            var args = GetValidArgs();
            args.Sex = "Hoge";

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains($"{nameof(args.Sex)}が不正です");
        }

        [TestMethod]
        public async Task その他の引数が未設定でエラー終了する()
        {
            // FamilyNameが未入力
            var args = GetValidArgs();
            args.FamilyName = "";

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FamilyName)).IsTrue();

            // GivenNameが未入力
            args = GetValidArgs();
            args.GivenName = "";

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.GivenName)).IsTrue();

            // FamilyNameKanaが未入力
            args = GetValidArgs();
            args.FamilyNameKana = "";

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FamilyNameKana)).IsTrue();

            // GivenNameKana が未入力
            args = GetValidArgs();
            args.GivenNameKana = "";

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.GivenNameKana)).IsTrue();

            // Password が未入力
            args = GetValidArgs();
            args.Password = "";

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Password)).IsTrue();
            

            // Birthday が未入力
            args = GetValidArgs();
            args.Birthday = "";

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Birthday)).IsTrue();            

            // 認証コードが未入力
            args = GetValidArgs();
            args.AuthCode = "";

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AuthCode)).IsTrue();
        }

        [TestMethod]
        public async Task 電話番号形式が正しくない場合はエラー()
        {
            var args = GetValidArgs();
            args.AccountPhoneNumber = "090A9998888"; // 数字以外がある

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("電話番号が不正です").IsTrue();

            args.AccountPhoneNumber = ""; // 空文字

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("電話番号が不正です").IsTrue();
        }

        [TestMethod]
        public async Task 規約の同意がないとエラー終了する()
        {
            // プライバシーポリシー が同意されていない
            var args = GetValidArgs();
            args.IsAgreePrivacyPolicy = bool.FalseString;

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("プライバシーポリシー").IsTrue();

            // 利用規約 が同意されていない
            args = GetValidArgs();
            args.IsAgreeTermsOfService = bool.FalseString;

            results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("利用規約").IsTrue();
        }

        [TestMethod]
        public async Task 正常に本登録が完了する_健康DIARY会員()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);            

            _smsClient.Setup(m => m.SendSms(args.AccountPhoneNumber, It.IsAny<string>())).Callback((string phone, string message) =>
            {
                message.Contains("健幸DX手帳").IsTrue();
                message.Contains("本登録が完了いたしました").IsTrue();
            });

            _accountRepo.Setup(m => m.UpdateIndexEntity(It.IsAny<QH_ACCOUNTINDEX_DAT>())).Callback((QH_ACCOUNTINDEX_DAT entity) =>
            {
                // ニックネームが登録された
                entity.NICKNAME.Is(args.NickName);
            });

            // 連携システム
            _linkageRepo.Setup(m => m.InsertEntity(It.IsAny<QH_LINKAGE_DAT>())).Callback((QH_LINKAGE_DAT entity) =>
            {
                // 仙北市と連携できた
                entity.LINKAGESYSTEMNO.Is(590003);
            });

            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 健康DIARY会員 HD{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"HD[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();
            

            // 認証コードデータは物理削除された
            _smsAuthCodeRepo.Verify(m => m.PhysicalDeleteEntity(_authKey), Times.Once);

            // SMSが送信された
            _smsClient.Verify(m => m.SendSms(args.AccountPhoneNumber, It.IsAny<string>()), Times.Once);

            // 連携なし本登録APIは呼ばれない
            _identityApi.Verify(m => m.ExecuteRegisterWriteApi(It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, string.Empty), Times.Never);
        }

        [TestMethod]
        public async Task 正常に本登録が完了する_KAGAMINO会員_子連携無し_ニックネームなし()
        {
            var args = GetValidArgs();
            args.FacilityKeyReference = "";
            args.NickName = "";
            args.ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.KagaminoiOSApp}";
            args.LinkageSystemNo = "99999";
            SetupValidMethods(args);

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            _smsClient.Setup(m => m.SendSms(args.AccountPhoneNumber, It.IsAny<string>())).Callback((string phone, string message) =>
            {
                message.Contains("KAGAMINO").IsTrue();
                message.Contains("本登録が完了いたしました").IsTrue();
            });


            var results = await _worker.RegisterNewUser(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // その他 QQ{10桁英数}のIDができている
            Regex.IsMatch(results.UserId, @"QQ[0-9A-HJ-NP-Y]{10}").IsTrue();
            results.LinkageIdReference.Is(_newAccountKey.ToEncrypedReference());
            // Tokenが生成されている
            results.Token.IsNotNull();


            // 認証コードデータは物理削除された
            _smsAuthCodeRepo.Verify(m => m.PhysicalDeleteEntity(_authKey), Times.Once);

            // SMSが送信された
            _smsClient.Verify(m => m.SendSms(args.AccountPhoneNumber, It.IsAny<string>()), Times.Once);

            // NickName処理は行われなかった
            _accountRepo.Verify(m => m.UpdateIndexEntity(It.IsAny<QH_ACCOUNTINDEX_DAT>()),Times.Never);

            // 子連携システム処理は行われなかった
            _linkageRepo.Verify(m => m.GetLinkageNo(_facilityKey), Times.Never);

            // 連携ユーザー登録APIは呼ばれない
            _identityApi.Verify(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, string.Empty), Times.Never);

            // 連携システムデータのPush通知IDの更新は通らない
            _linkageRepo.Verify(m => m.ReadEntity(_newAccountKey, 99999), Times.Never);
        }

        // 正常系を通過するようにMockを設定する
        void SetupValidMethods(QoLinkageNewUserRegisterForSmsApiArgs args)
        {
            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });


            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            var sex = args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);

            // 同一情報登録ユーザーチェック
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.AccountPhoneNumber)).Returns(default(QH_ACCOUNTPHONE_MST));

            // 本登録(連携あり)
            _identityApi.Setup(m => m.ExecuteLinkageUserRegisterApi(args.LinkageSystemNo, It.IsAny<string>(), It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, string.Empty)).Returns(new IdentityRegisterApiResults
            {
                IsSuccess = true,
                AccountKey = _newAccountKey.ToApiGuidString()
            })
            .Callback((string a, string linkageSystemId, string c, string d, string e, string f, string g, string h, string i, string j, string mailAddr) =>
            {
                mailAddr.Is(string.Empty);
                _newLinkageSystemId = Guid.Parse(linkageSystemId);
            });

            // 本登録(連携なし)
            _identityApi.Setup(m => m.ExecuteRegisterWriteApi(It.IsAny<string>(), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, string.Empty)).Returns(new IdentityRegisterApiResults
            {
                IsSuccess = true,
                AccountKey = _newAccountKey.ToApiGuidString()
            })
            .Callback((string c, string d, string e, string f, string g, string h, string i, string j, string mailAddr) =>
            {
                mailAddr.Is(string.Empty);
            });

            var targetEntityForPushId = new QH_LINKAGE_DAT();
            // 連携システムデータを返す
            _linkageRepo.Setup(m => m.ReadEntity(_newAccountKey, args.LinkageSystemNo.TryToValueType(0))).Returns(targetEntityForPushId);
            _linkageRepo.Setup(m => m.UpdateEntity(targetEntityForPushId)).Callback((QH_LINKAGE_DAT dat) =>
            {
                // アカウントキーがPush通知IDとして登録された
                dat.LINKAGESYSTEMID.Is(_newAccountKey.ToString("N"));
            });

            // アカウント情報取得
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_newAccountKey)).Returns(new QH_ACCOUNTINDEX_DAT());

            // 施設キーから仙北市連携番号に
            _linkageRepo.Setup(m => m.GetLinkageNo(_facilityKey)).Returns(590003);
        }


        QoLinkageNewUserRegisterForSmsApiArgs GetValidArgs()
        {
            return new QoLinkageNewUserRegisterForSmsApiArgs
            {
                WithPatientCard = bool.FalseString,
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HealthDiary",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.HealthDiaryiOSApp}",
                LinkageSystemNo = QoLinkage.HEALTHDIARY_LINKAGE_SYSTEM_NO.ToString(),
                FacilityKeyReference = _facilityKey.ToEncrypedReference(), // 仙北市
                AuthKeyReference = _authKey.ToEncrypedReference(),
                AuthCode = "123456",
                AccountPhoneNumber = "09099998888",
                Birthday = new DateTime(2000, 1, 1).ToApiDateString(),                
                FamilyName = "患者",
                GivenName = "太郎",
                FamilyNameKana = "カンジャ",
                GivenNameKana = "タロウ",
                NickName = "ニックネーム",
                IsAgreePrivacyPolicy = bool.TrueString,
                IsAgreeTermsOfService = bool.TrueString,
                Password = "abc1234_",
                Sex = "1",                
            };
        }
    }
}
