using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// アカウントに関する機能を提供する API コントローラ です。
    /// </summary>
    public sealed class AccountController : QoApiControllerBase
    {
        /// <summary>
        /// <see cref="AccountController" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AccountController() : base() { }
        
        /// <summary>
        /// ログイン認証を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("Login")]
        public QoAccountLoginApiResults PostLogin(QoAccountLoginApiArgs args)
        {
            return base.ExecuteWorkerMethod(args, AccountWorker.Login);
        }

        /// <summary>
        /// アカウント仮登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("SignUp")]
        public QoAccountSignUpApiResults PostSignUp(QoAccountSignUpApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.SignUp);
        }

        /// <summary>
        /// SMS認証リクエストを行います。
        /// 認証コードが発行されSMSが送信されます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("SmsAuthRequest")]
        public async Task<QoAccountSmsAuthRequestApiResults> PostSmsAuthRequest(QoAccountSmsAuthRequestApiArgs args)
        {
            var worker = new AccountSmsAuthRequestWorker(
                new SmsAuthCodeRepository(),
                new AccountRepository(),
                new QoSmsClient());
            return await this.ExecuteWorkerMethodAsync(args, worker.SmsAuthRequest);
        }

        /// <summary>
        /// アカウント本登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("Register")]
        public QoAccountRegisterApiResults PostRegister(QoAccountRegisterApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.Register);
        }

        /// <summary>
        /// パスワードリセットを行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("PasswordReset")]
        public QoAccountPasswordResetApiResults PostPasswordReset(QoAccountPasswordResetApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.PasswordReset);
        }

        /// <summary>
        /// パスワードリセットを行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("PasswordEdit")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All, true)]
        public QoAccountPasswordEditApiResults PostPasswordEdit(QoAccountPasswordEditApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.PasswordEdit);
        }

        /// <summary>
        /// アカウント情報を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("InformationRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountInformationReadApiResults PostInformationRead(QoAccountInformationReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.InformationRead);
        }

        /// <summary>
        /// アカウント情報を変更します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("InformationWrite")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountInformationWriteApiResults PostInformationWrite(QoAccountInformationWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.InformationWrite);
        }

        /// <summary>
        /// 退会理由マスタ情報を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("WithdrawMstRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountWithdrawMstReadApiResults PostWithdrawMstRead(QoAccountWithdrawMstReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.WithdrawMstRead);
        }

        /// <summary>
        /// アカウント退会処理を実行します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("Withdraw")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountWithdrawApiResults PostWithdraw(QoAccountWithdrawApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.Withdraw);
        }

        /// <summary>
        /// メールアドレス変更要求（メール認証）を実行します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("ChangeMailRequest")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountChangeMailRequestApiResults PostChangeMailRequest(QoAccountChangeMailRequestApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.ChangeMailRequest);
        }

        /// <summary>
        /// ID問い合わせ要求を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("IdForgetRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All | QoApiFunctionTypeEnum.Guest)]
        public QoAccountIdForgetReadApiResults PostIdForgetRead(QoAccountIdForgetReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountWorker.IdForgetRead);
        }

        /// <summary>
        /// SMS認証コードによるログイン1段階目を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("LoginForSms")]
        public async  Task<QoAccountLoginForSmsApiResults> PostLoginForSms(QoAccountLoginForSmsApiArgs args)
        {
            var worker = new AccountLoginForSmsWorker(
                new AccountRepository(),
                new PasswordManagementRepository(),
                new SmsAuthCodeRepository(),
                new QoSmsClient()
            );

            return await ExecuteWorkerMethodAsync(args, worker.LoginForSms);
        }

        /// <summary>
        /// SMS認証コードによるログイン2段階目を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("LoginForSms2")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtToken)]
        public QoAccountLoginForSms2ApiResults PostLoginForSms2(QoAccountLoginForSms2ApiArgs args)
        {
            var worker = new AccountLoginForSms2Worker(
                new PasswordManagementRepository(),
                new SmsAuthCodeRepository(),
                new LinkageRepository(),
                new IdentityApiRepository()
            );

            return ExecuteWorkerMethod(args, worker.LoginForSms2);
        }

        /// <summary>
        /// 電話番号変更（SMS認証用）を実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("ChangePhoneRequestForSms")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountChangePhoneRequestForSmsApiResults PostChangePhoneRequestForSms(QoAccountChangePhoneRequestForSmsApiArgs args)
        {
            var worker = new AccountChangePhoneRequestForSmsWorker(
                new AccountRepository(),
                new SmsAuthCodeRepository());

            return ExecuteWorkerMethod(args, worker.ChangePhoneRequest);
        }

        /// <summary>
        /// 電話番号削除(SMS認証用)を実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("DeletePhoneRequestForSms")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoAccountDeletePhoneRequestForSmsApiResults PostDeletePhoneRequestForSms(QoAccountDeletePhoneRequestForSmsApiArgs args)
        {
            var worker = new AccountDeletePhoneRequestForSmsWorker(
                new AccountRepository(),
                new SmsAuthCodeRepository());

            return ExecuteWorkerMethod(args, worker.DeletePhoneRequest);
        }

        /// <summary>
        /// パスワードリセット（SMS認証用）を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("PasswordResetForSms")]
        public QoAccountPasswordResetForSmsApiResults PostPasswordResetForSms(QoAccountPasswordResetForSmsApiArgs args)
        {
            var worker = new AccountPasswordResetForSmsApiWorker(
                new PasswordManagementRepository(),
                new SmsAuthCodeRepository(),
                new AccountRepository());

            return ExecuteWorkerMethod(args, worker.PasswordResetRequest);
        }

        /// <summary>
        /// SMS認証コードによるログイン1段階目を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("LoginForMail")]
        public async Task<QoAccountLoginForMailApiResults> PostLoginForMail(QoAccountLoginForMailApiArgs args)
        {
            var worker = new AccountLoginForMailWorker(
                new AccountRepository(),
                new PasswordManagementRepository(),
                new SmsAuthCodeRepository()
            );

            return await ExecuteWorkerMethodAsync(args, worker.LoginForMail);
        }


    }
}