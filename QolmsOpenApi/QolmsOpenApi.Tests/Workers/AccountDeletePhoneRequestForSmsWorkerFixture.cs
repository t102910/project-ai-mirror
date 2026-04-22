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
    public class AccountDeletePhoneRequestForSmsWorkerFixture
    {
        AccountDeletePhoneRequestForSmsWorker _worker;
        Mock<ISmsAuthCodeRepository> _smsAuthCodeRepo;
        Mock<IAccountRepository> _accountRepo;
        Guid _authKey;
        Guid _accountKey;

        [TestInitialize]
        public void Initialize()
        {
            _smsAuthCodeRepo = new Mock<ISmsAuthCodeRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _worker = new AccountDeletePhoneRequestForSmsWorker(_accountRepo.Object, _smsAuthCodeRepo.Object);

            _authKey = Guid.NewGuid();
            _accountKey = Guid.NewGuid();
        }

        [TestMethod]
        public void アカウントキーが不正でエラー()
        {
            var args = GetValidArgs();
            args.ActorKey = "Hoge";

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("アカウントキーが不正です");
        }

        [TestMethod]
        public void 認証キーが不正でエラー終了する()
        {
            var args = GetValidArgs();
            args.AuthKeyReference = "Hoge";

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AuthKeyReference));
        }

        [TestMethod]
        public void 認証コード期限切れでエラー()
        {
            var args = GetValidArgs();

            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(-15),
            });

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3004");
            results.Result.Detail.Contains("認証コードの期限が切れています");
        }

        [TestMethod]
        public void 認証コード試行回数オーバーでエラー()
        {
            var args = GetValidArgs();

            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
                FAILURECOUNT = 2
            });

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3005");
            results.Result.Detail.Contains("認証コードが一定回数間違えたため無効となっています");
        }

        [TestMethod]
        public void 認証コード_コード不一致でエラー()
        {
            var args = GetValidArgs();

            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123450",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("3006");
            results.Result.Detail.Contains("認証コードが一致しませんでした");
        }

        [TestMethod]
        public void 認証コード照合処理で例外でエラー()
        {
            var args = GetValidArgs();

            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Throws(new Exception());

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("認証コード照合処理に失敗しました");
        }

        [TestMethod]
        public void 電話番号未登録でエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 電話番号レコードなし
            _accountRepo.Setup(m => m.ReadPhoneEntity(_accountKey)).Returns(default(QH_ACCOUNTPHONE_MST));

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1006");
            results.Result.Detail.Contains("電話番号は未登録です");
        }

        [TestMethod]
        public void 電話番号削除処理で例外発生でエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _accountRepo.Setup(m => m.PhysicalDeletePhoneEntity(_accountKey)).Throws(new Exception());

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("電話番号削除処理に失敗しました");
        }

        [TestMethod]
        public void 正常終了()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.DeletePhoneRequest(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 電話番号物理削除が実行された
            _accountRepo.Verify(m => m.PhysicalDeletePhoneEntity(_accountKey), Times.Once);
        }

        

        void SetupValidMethods(QoAccountDeletePhoneRequestForSmsApiArgs args)
        {
            // 認証コード通過
            _smsAuthCodeRepo.Setup(m => m.ReadEntity(_authKey)).Returns(new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = _authKey,
                AUTHCODE = "123456",
                EXPIRES = DateTime.Now.AddMinutes(15),
            });
            

            // AccountPhoneマスタを取得
            _accountRepo.Setup(m => m.ReadPhoneEntity(_accountKey)).Returns(new QH_ACCOUNTPHONE_MST           
            {
                PHONENUMBER = "99999999"
            });
        }

        QoAccountDeletePhoneRequestForSmsApiArgs GetValidArgs()
        {
            return new QoAccountDeletePhoneRequestForSmsApiArgs
            {
                AuthCode = "123456",
                AuthKeyReference = _authKey.ToEncrypedReference(),
                ActorKey = _accountKey.ToApiGuidString(), 
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsNaviiOsApp}",
            };
        }
    }
}
