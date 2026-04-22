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
    public class AccountSmsAuthRequestWorkerFixture
    {
        AccountSmsAuthRequestWorker _worker;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<IAccountRepository> _accountRepo;
        Mock<IQoSmsClient> _smsClient;
        Guid _authKey;
        string _authCode;

        [TestInitialize]
        public void Initialize()
        {
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _smsClient = new Mock<IQoSmsClient>();
            _worker = new AccountSmsAuthRequestWorker(_smsAuthCodeRepo.Object,_accountRepo.Object ,_smsClient.Object);
        }

        [TestMethod]
        public async Task 電話番号が未設定でエラー()
        {
            var args = GetValidArgs();
            args.PhoneNumber = string.Empty;

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("必須").IsTrue();
            results.Result.Detail.Contains("PhoneNumber").IsTrue();
        }

        [TestMethod]
        public async Task 電話番号の形式が不正でエラー()
        {
            var args = GetValidArgs();
            args.PhoneNumber = "06A99991000";

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("は数字だけで構成されている必要があります").IsTrue();
            results.Result.Detail.Contains("PhoneNumber").IsTrue();
        }

        [TestMethod]
        public async Task 電話番号登録必須の場合はチェックを行い未登録はエラー()
        {
            var args = GetValidArgs();
            args.RequirePhoneNumberRegistration = true;

            // 未登録設定
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Returns(default(QH_ACCOUNTPHONE_MST));

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3008");
            results.Result.Detail.Contains("電話番号が未登録です").IsTrue();
        }

        [TestMethod]
        public async Task 電話番号登録必須の場合はチェックを行い例外はエラー()
        {
            var args = GetValidArgs();
            args.RequirePhoneNumberRegistration = true;

            // 未登録設定
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Throws(new Exception());

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("電話番号登録確認処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public async Task 認証コードの保存に失敗するとエラー()
        {
            var args = GetValidArgs();

            // DB登録時にエラー
            _smsAuthCodeRepo.Setup(m => m.InsertEntity(It.IsAny<QH_SMSAUTHCODE_DAT>())).Throws(new Exception());

            var results = await _worker.SmsAuthRequest(args);

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

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("SMSの送信に失敗しました").IsTrue();
        }

        [TestMethod]
        public async Task 処理に成功して正常に終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // AuthKeyが返されている
            results.AuthKeyReference.ToDecrypedReference<Guid>().Is(_authKey);

            // 認証コード6桁数字が発行されている            
            Regex.IsMatch(_authCode, @"^[0-9]{6}$");
        }

        [TestMethod]
        public async Task 処理に成功して正常に終了する_電話番号登録必須の場合()
        {
            var args = GetValidArgs();
            args.RequirePhoneNumberRegistration = true;
            SetupValidMethods(args);

            var results = await _worker.SmsAuthRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // AuthKeyが返されている
            results.AuthKeyReference.ToDecrypedReference<Guid>().Is(_authKey);

            // 認証コード6桁数字が発行されている            
            Regex.IsMatch(_authCode, @"^[0-9]{6}$");
        }

        void SetupValidMethods(QoAccountSmsAuthRequestApiArgs args)
        {
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

            // 存在チェック通過
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Returns(new QH_ACCOUNTPHONE_MST());
        }

        QoAccountSmsAuthRequestApiArgs GetValidArgs()
        {
            return new QoAccountSmsAuthRequestApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsNaviiOsApp}",
                PhoneNumber = "0699998888"
            };
        }
    }
}
