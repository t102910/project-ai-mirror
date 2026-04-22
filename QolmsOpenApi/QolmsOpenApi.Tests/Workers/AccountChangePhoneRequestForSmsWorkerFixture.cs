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
    public class AccountChangePhoneRequestForSmsWorkerFixture
    {
        AccountChangePhoneRequestForSmsWorker _worker;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<IAccountRepository> _accountRepo;
        Guid _authKey;

        [TestInitialize]
        public void Initialize()
        {
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _worker = new AccountChangePhoneRequestForSmsWorker(_accountRepo.Object, _smsAuthCodeRepo.Object);

            _authKey = Guid.NewGuid();
        }

        [TestMethod]
        public void 電話番号が未設定でエラー()
        {
            var args = GetValidArgs();
            args.PhoneNumber = string.Empty;

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("必須").IsTrue();
            results.Result.Detail.Contains("PhoneNumber").IsTrue();
        }

        [TestMethod]
        public void 電話番号の形式が不正でエラー()
        {
            var args = GetValidArgs();
            args.PhoneNumber = "06A99991000";

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("は数字だけで構成されている必要があります").IsTrue();
            results.Result.Detail.Contains("PhoneNumber").IsTrue();
        }

        [TestMethod]
        public void 認証キーが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.AuthKeyReference = "Hoge";

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AuthKeyReference));
        }

        [TestMethod]
        public void 電話番号重複でエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Returns(new QH_ACCOUNTPHONE_MST { PHONENUMBER = args.PhoneNumber });

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3007");
            results.Result.Detail.Contains("この電話番号は既に使用されています").IsTrue();
        }

        [TestMethod]
        public void 処理に成功して正常に終了する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _accountRepo.Setup(m => m.UpdatePhoneEntity(It.IsAny<QH_ACCOUNTPHONE_MST>())).Callback((QH_ACCOUNTPHONE_MST entity) =>
            {
                // 電話番号が変更される      
                entity.PHONENUMBER.Is(args.PhoneNumber);
            });

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 処理に成功して正常に終了する_番号新規登録の場合()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            // 電話番号マスタに存在しない
            _accountRepo.Setup(m => m.ReadPhoneEntity(accountKey)).Returns(default(QH_ACCOUNTPHONE_MST));

            _accountRepo.Setup(m => m.InsertPhoneEntity(It.IsAny<QH_ACCOUNTPHONE_MST>())).Callback((QH_ACCOUNTPHONE_MST entity) =>
            {
                // アカウントキーと電話番号が設定されている
                entity.ACCOUNTKEY.Is(accountKey);   
                entity.PHONENUMBER.Is(args.PhoneNumber);
            });

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 電話番号更新処理中に例外が発生する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _accountRepo.Setup(m => m.UpdatePhoneEntity(It.IsAny<QH_ACCOUNTPHONE_MST>())).Throws(new Exception("例外発生"));

            var results = _worker.ChangePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("電話番号登録処理に失敗しました").IsTrue();
        }

        void SetupValidMethods(QoAccountChangePhoneRequestForSmsApiArgs args)
        {
            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });

            // 存在チェック
            _accountRepo.Setup(m => m.ReadPhoneEntityByNumber(args.PhoneNumber)).Returns((QH_ACCOUNTPHONE_MST)null);

            // AccountPhoneマスタを取得
            _accountRepo.Setup(m => m.ReadPhoneEntity(args.ActorKey.TryToValueType(Guid.Empty))).Returns(new QH_ACCOUNTPHONE_MST           
            {
                PHONENUMBER = "99999999"
            });
        }

        QoAccountChangePhoneRequestForSmsApiArgs GetValidArgs()
        {
            return new QoAccountChangePhoneRequestForSmsApiArgs
            {
                AuthCode = "123456",
                AuthKeyReference = _authKey.ToEncrypedReference(),
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS",                   //テスト用（本来はトークンからいれられる）
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsNaviiOsApp}",
                PhoneNumber = "0699998888"
            };
        }
    }
}
