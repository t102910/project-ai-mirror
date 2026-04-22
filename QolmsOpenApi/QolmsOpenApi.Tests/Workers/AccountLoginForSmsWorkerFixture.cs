using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
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
    [TestClass]
    public class AccountLoginForSmsWorkerFixture
    {
        AccountLoginForSmsWorker _worker;
        Mock<IAccountRepository> _accountRepo;
        Mock<IPasswordManagementRepository> _passwordRepo;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<IQoSmsClient> _smsClient;
        Guid _accountKey;
        Guid _authKey;
        string _authCode;
        string _encPassword;

        [TestInitialize]
        public void Initialize()
        {
            _accountRepo = new Mock<IAccountRepository>();
            _passwordRepo = new Mock<IPasswordManagementRepository>();
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _smsClient = new Mock<IQoSmsClient>();

            _accountKey = Guid.NewGuid();
            

            _worker = new AccountLoginForSmsWorker(_accountRepo.Object, _passwordRepo.Object, _smsAuthCodeRepo.Object, _smsClient.Object);
        }

        [TestMethod]
        public async Task 必須項目_電話番号_パスワードが未入力ならエラー()
        {
            var args = GetValidArgs();
            args.PhoneNumber = "";

            var results = await _worker.LoginForSms(args);

            // 電話番号未入力により失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.PhoneNumber)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            args = GetValidArgs();
            args.Password = "";

            results = await _worker.LoginForSms(args);

            // パスワード未入力により失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.Password)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();
        }

        [TestMethod]
        public async Task 電話番号の形式が不正でエラー()
        {
            var args = GetValidArgs();
            args.PhoneNumber = "06A99991000";

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("は数字だけで構成されている必要があります").IsTrue();
            results.Result.Detail.Contains("PhoneNumber").IsTrue();
        }

        [TestMethod]
        public async Task Executorが未設定でエラー()
        {
            var args = GetValidArgs();
            args.Executor = "";

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("不正").IsTrue();
            results.Result.Detail.Contains(nameof(args.Executor)).IsTrue();
        }

        [TestMethod]
        public async Task 電話番号に対応するアカウントが無ければエラー()
        {
            var args = GetValidArgs();

            // entityはnullで返す
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Returns(default(QH_ACCOUNTPHONE_MST));

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("電話番号に対応するアカウントが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public async Task 電話番号からアカウントを取得する処理で例外が出たらエラー()
        {
            var args = GetValidArgs();

            // DBアクセスで例外
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Throws(new Exception());

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント照合処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public async Task パスワード照合でアカウントが存在しない場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // entityはnullで返す
            _passwordRepo.Setup(m => m.ReadEntity(_accountKey)).Returns(default(QH_PASSWORDMANAGEMENT_DAT));

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("アカウントが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public async Task パスワード照合で不一致だった場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // テーブル側のパスワードが異なる
            _passwordRepo.Setup(m => m.ReadEntity(_accountKey)).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERPASSWORD = "hoge"
            });

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1001");
            results.Result.Detail.Contains("パスワードが一致しませんでした").IsTrue();
        }

        [TestMethod]
        public async Task パスワード照合で例外が発生した場合はエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 例外発生
            _passwordRepo.Setup(m => m.ReadEntity(_accountKey)).Throws(new Exception());

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("パスワード照合処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public async Task 認証コードの保存に失敗するとエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // DB登録時にエラー
            _smsAuthCodeRepo.Setup(m => m.InsertEntity(It.IsAny<QH_SMSAUTHCODE_DAT>())).Throws(new Exception());

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("SMS認証コードの登録に失敗しました").IsTrue();
        }

        [TestMethod]
        public async Task 認証コードのSMS送信に失敗するとエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // SMS送信で例外発生
            _smsClient.Setup(m => m.SendSms(args.PhoneNumber, It.IsAny<string>())).Throws(new Exception());

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("SMSの送信に失敗しました").IsTrue();
        }

        [TestMethod]
        public async Task 処理に成功して正常に終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = await _worker.LoginForSms(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // AuthKeyが返されている
            results.AuthKeyReference.ToDecrypedReference<Guid>().Is(_authKey);
            // Tokenに値が設定されている(JWTの検証はここではしない)
            results.Token.IsNotNull();
            
        }

        void SetupValidMethods(QoAccountLoginForSmsApiArgs args)
        {
            _encPassword = args.Password.TryEncrypt();

            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Returns(new QH_ACCOUNTPHONE_MST
            {
                ACCOUNTKEY = _accountKey,                
            });

            _passwordRepo.Setup(m => m.ReadEntity(_accountKey)).Returns(new QH_PASSWORDMANAGEMENT_DAT
            {
                USERPASSWORD = _encPassword
            });

            _smsAuthCodeRepo.Setup(m => m.InsertEntity(It.IsAny<QH_SMSAUTHCODE_DAT>())).Callback((QH_SMSAUTHCODE_DAT entity) =>
            {
                entity.AUTHKEY.IsNot(Guid.Empty);
                // DateTime.Nowは操作できないのでおおよそ15分後かどうかを検証
                (entity.EXPIRES > DateTime.Now.AddMinutes(10)).IsTrue();
                (entity.EXPIRES < DateTime.Now.AddMinutes(20)).IsTrue();

                _authKey = entity.AUTHKEY;                
                _authCode = entity.AUTHCODE;                
            });

            _smsClient.Setup(m => m.SendSms(args.PhoneNumber, It.IsAny<string>())).Callback((string phone, string message) =>
            {
                phone.Is(args.PhoneNumber);
                message.Contains("医療ナビ").IsTrue();
                message.Contains("認証コード").IsTrue();
            });
        }

        QoAccountLoginForSmsApiArgs GetValidArgs()
        {
            return new QoAccountLoginForSmsApiArgs
            {
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.QolmsNaviiOsApp}",
                PhoneNumber = "0699998888",
                Password = "abc1234_"
            };
        }
    }
}
