using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using System.Linq;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestAccountControllers
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "xbcXXzuwm9GMF/bu8K9ujA==";
        /// <summary>
        /// ログイン成功を確認します。
        /// </summary>
        [TestMethod]
        public void PostLogin_Success()
        {
            //ログイン成功したらTokenが返ってくる
            var args = new QoAccountLoginApiArgs() {
                ApiType = QoApiTypeEnum.AccountLogin.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName =EXECUTORNAME ,
                Executor = EXECUTOR ,
                Password ="11111111",
                UserId ="dev_masuda",
                UseTwoFactorAuthentication ="false",
                SessionId ="nanika"
            };
            var controller = new AccountController();
            var result =controller.PostLogin(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), false);
        }
        [TestMethod]
        public void PostLogin_Error_CheckPassword()
        {
            //ログイン失敗
            var args = new QoAccountLoginApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountLogin.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME ,
                Executor = EXECUTORNAME ,
                Password = "?????",
                UserId = "dev_masuda",
                UseTwoFactorAuthentication = "false",
                SessionId = "nanika"
            };
            var controller = new AccountController();
            var result = controller.PostLogin(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), false);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), true);
            Assert.AreEqual(result.LoginResultType.TryToValueType(QsApiLoginResultTypeEnum.None),QsApiLoginResultTypeEnum.Retry );
        }
        [TestMethod]
        public void PostLogin_Error_CheckExecutor()
        {
            //ログイン失敗 Executor不正
            var args = new QoAccountLoginApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountLogin.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = "xbcXXzuwm9GMF/bu8K9ujA==",
                Executor = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/d",
                Password = "11111111",
                UserId = "dev_masuda",
                UseTwoFactorAuthentication = "false",
                SessionId = "nanika"
            };
            var controller = new AccountController();
            var result = controller.PostLogin(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), false);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), true);         
        }

        [TestMethod]
        public void PostSignup_Success()
        {
            //アカウントSignUp成功
            var args = new QoAccountSignUpApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountSignUp.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                Mail = "maki_masuda@mgfactory.co.jp"
            };
            var controller = new AccountController();
            var result = controller.PostSignUp(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }
        [TestMethod]
        public void PostSignup_Error()
        {
            //アカウントSignUpメルアドなくて失敗
            var args = new QoAccountSignUpApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountRegister.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                Mail = ""
            };
            var controller = new AccountController();
            var result = controller.PostSignUp(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), false);
        }

        [TestMethod]
        public void PostRegister_Success()
        {
            //アカウント登録成功したらTokenが返ってくる
            var args = new QoAccountRegisterApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountRegister.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME ,
                Executor = EXECUTOR ,
                Account = new QoApiAccountRegisterItem()
                {
                    AccountKeyReference = "ce76bfea49c215d6de53821fbf18e0bcc60c4fcf13677810a4b04acd76ced9652d1b3bb9334e41249b92fd118662500a84c75a5757790e93532cbe1a190e1f61",
                    Birthday = DateTime.Now.ToApiDateString(),
                    FamilyName = "てすと",
                    FamilyNameKana = "テスト",
                    GivenName = "てすとなまえ",
                    GivenNameKana = "テストナマエ",
                    IsAgreePrivacyPolicy = bool.TrueString,
                    IsAgreeTermsOfService = bool.TrueString,
                    Mail = "test@qolms.com",
                    Password = "test@1",
                    Sex = "1",
                    Tel = "09000000000",
                    UserId = string.Format("test@{0:MMdddhhmmss}", DateTime.Now)
                }
            };
            var controller = new AccountController();
            var result = controller.PostRegister(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), false);
        }
        [TestMethod]
        public void PostRegister_Error()
        {
            //アカウント登録失敗するはず
            var args = new QoAccountRegisterApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountRegister.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                Account = new QoApiAccountRegisterItem()
                {
                    AccountKeyReference = "",
                    Birthday = DateTime.Now.ToApiDateString(),
                    FamilyName = "てすと",
                    FamilyNameKana = "てすと",
                    GivenName = "てすとなまえ",
                    GivenNameKana = "テストナマエ",
                    IsAgreePrivacyPolicy = bool.TrueString,
                    IsAgreeTermsOfService = bool.TrueString,
                    Mail = "test@qolms.com",
                    Password = "test@1",
                    Sex = "1",
                    Tel = "09000000000",
                    UserId = string.Format("test@{0:MMdddhhmmss}", DateTime.Now)
                }
            };
            var controller = new AccountController();
            var result = controller.PostRegister(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), false);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), true);
        }



        [TestMethod]
        public void PostPasswordReset_Success()
        {
            //パスワードリセット(メールが飛ぶ）
            var args = new QoAccountPasswordResetApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountPasswordReset.ToString(),
                ExecuteSystemType ="40", // QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                Mail ="maki_masuda@mgfactory.co.jp",
                UserId ="dev_masuda",
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountController();
            var result = controller.PostPasswordReset(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }

        [TestMethod]
        public void PostPasswordEdit_Success()
        {
            //パスワードEdit
            var args = new QoAccountPasswordEditApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountPasswordEdit.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                UserIdReference = "d3aa64dfe81708ee3a04fbc9b43495eb3e8d6a7a4ea273f9d2691f15ec200f74",    //めーるから
                NewPassword ="dev_masuda@1",
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountController();
            var result = controller.PostPasswordEdit(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), false);
        }
        [TestMethod]
        public void PostPasswordEdit_Error()
        {
            //パスワード無効
            var args = new QoAccountPasswordEditApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountPasswordEdit.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                UserIdReference = "d3aa64dfe81708ee3a04fbc9b43495eb3e8d6a7a4ea273f9d2691f15ec200f74",    //めーるから
                NewPassword = "11111111",
                ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountController();
            var result = controller.PostPasswordEdit(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Token), false);
        }
        [TestMethod]
        public void PostInformationRead_Success()
        {
            //アカウント情報（メルアド・Tel）を取得
            var args = new QoAccountInformationReadApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountInformationRead.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountController();
            var result = controller.PostInformationRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.Information.Mail), false);
        }

        [TestMethod]
        public void PostInformationWrite_Success()
        {
            //アカウント情報（メルアド・Tel）を変更
            var args = new QoAccountInformationWriteApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountInformationWrite.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                Information=new QoApiAccountInformationItem() { Mail ="maki_masuda@mgfactory.co.jp",Tel="09000000000"},
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountController();
            var result = controller.PostInformationWrite(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }


        [TestMethod]
        public void PostWithdrawMstRead_Success()
        {
            //退会理由マスタが返ってくる
            var args = new QoAccountWithdrawMstReadApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountWithdrawMstRead.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                LinkageSystemNo ="99999",
                ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountController();
            var result = controller.PostWithdrawMstRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(result.WithdrawMstN.Any(), true);
        }

        [TestMethod]
        public void PostWithdraw_Success()
        {
            //退会処理
            var args = new QoAccountWithdrawApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountWithdraw.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "",       //入れたら一回しかテストできない・・・
                LinkageSystemNo = "99999",
                UnsubscribeItemNo = "1",
                Comment = "Dummiy"
            };
            var controller = new AccountController();
            var result = controller.PostWithdraw(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);

        }

        [TestMethod]
        public void PostChangeMailRequest_Success()
        {
            //メールアドレス変更要求
            var args = new QoAccountChangeMailRequestApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountChangeMailRequest.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS",                   //テスト用（本来はトークンからいれられる）
                Mail = "maki_masuda@mgfactory.co.jp"
            };
            var controller = new AccountController();
            var result = controller.PostChangeMailRequest(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);

        }
    }
}
