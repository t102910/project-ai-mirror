using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    public class AccountLoginForMailWorker
    {
        private IAccountRepository _accountRepository;
        private IPasswordManagementRepository _passwordManagementRepository;
        private ISmsAuthCodeRepository _smsAuthCodeRepository;

        public AccountLoginForMailWorker(AccountRepository accountRepository, PasswordManagementRepository passwordManagementRepository, SmsAuthCodeRepository smsAuthCodeRepository)
        {
            this._accountRepository = accountRepository;
            this._passwordManagementRepository = passwordManagementRepository;
            this._smsAuthCodeRepository = smsAuthCodeRepository;
        }

        public async Task<QoAccountLoginForMailApiResults> LoginForMail(QoAccountLoginForMailApiArgs args)
        {
            var results = new QoAccountLoginForMailApiResults()
            {
                IsSuccess = bool.FalseString
            };

            // 必須チェック
            if (!args.PhoneNumber.CheckArgsRequired(nameof(args.PhoneNumber), results) ||
                !args.Password.CheckArgsRequired(nameof(args.Password), results))
            {
                return results;
            }

            // Executorチェック
            if (!args.Executor.CheckArgsConvert(nameof(args.Executor), Guid.Empty, results, out var executor))
            {
                return results;
            }

            // 電話番号 数字だけで構成されているかチェック
            if (!Regex.IsMatch(args.PhoneNumber, @"^[0-9]+$"))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.PhoneNumber)}は数字だけで構成されている必要があります。");
                return results;
            }

            // Executorを暗号化
            var encExecutor = executor.ToString("N").TryEncrypt();

            // 電話番号に紐づくアカウントを取得
            if (!TryCheckAccount(args.PhoneNumber, out var accountKey, results))
            {
                return results;
            }

            var mailaddress = string.Empty;

            // パスワード照合
            if (!TryVerifyPassword(accountKey, args.Password, results, ref mailaddress))
            {
                return results;
            }

            // 認証キー発行
            var authKey = Guid.NewGuid();
            // 6桁の認証コード発行
            var authCode = QoSmsClient.GenerateAuthCode(6);

            // 認証コード情報を保存
            if (!SaveAuthCode(authKey, authCode, results))
            {
                return results;
            }

            // SMSで認証コードを送信
            if (!await SendMail(args.ExecuteSystemType, mailaddress, authCode, results))
            {
                return results;
            }

            // 成功
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            // 認証キーを暗号化
            results.AuthKeyReference = authKey.ToEncrypedReference();
            // 二段階目のログインAPIアクセス用トークンを設定
            results.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(encExecutor, accountKey);

            return results;

        }

        bool TryCheckAccount(string phoneNumber, out Guid accountKey, QoApiResultsBase results)
        {
            accountKey = Guid.Empty;

            try
            {
                var entity = _accountRepository.ReadPhoneEntityByNumber(phoneNumber);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "電話番号に対応するアカウントが存在しませんでした。");
                    return false;
                }

                accountKey = entity.ACCOUNTKEY;
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント照合処理でエラーが発生しました。");
                return false;
            }
        }

        bool TryVerifyPassword(Guid accountKey, string password, QoApiResultsBase results, ref string mailaddress)
        {
            try
            {
                var entity = _passwordManagementRepository.ReadEntity(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "アカウントが存在しませんでした。");
                    return false;
                }

                // パスワード不一致
                if (entity.USERPASSWORD != password.TryEncrypt())
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "パスワードが一致しませんでした。");
                    return false;
                }

                mailaddress = entity.PASSWORDRECOVERYMAILADDRESS.TryDecrypt();

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "パスワード照合処理でエラーが発生しました。");
                return false;
            }
        }
        bool SaveAuthCode(Guid authkey, string authCode, QoApiResultsBase results)
        {
            var entity = new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = authkey,
                AUTHCODE = authCode,
                EXPIRES = DateTime.Now.AddMinutes(15),
            };

            try
            {
                _smsAuthCodeRepository.InsertEntity(entity);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "SMS認証コードの登録に失敗しました。");
                return false;
            }
        }

        async Task<bool> SendMail(string systemType, string mailAddress, string authCode, QoApiResultsBase results)
        {
            // システム名称取得
            var systemName = systemType.TryToValueType(QsApiSystemTypeEnum.None).ToSystemName();
            //// SMSメッセージ
            //var message = $"【{systemName}】認証コード: {authCode}";

            try
            {
                string p = AppWorker.GetUrlParam(systemType.TryToValueType(QsApiSystemTypeEnum.None));
                await SendAccountLoginForMail(mailAddress, authCode, p);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "メールの送信に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// ログインパスコードメールを送信します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="passCode"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private Task<bool> SendAccountLoginForMail(string mailAddress, string passCode, string param)
        {
            string bodyPath = string.Format("~/App_Data/MailBodyAccountLoginPass_{0}.txt", param);
            string footerPath = string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNameAccountLoginPass_{0}", param);
            string subject = string.Format("MailSubjectAccountLoginPass_{0}", param);
            QoAccessLog.WriteInfoLog(bodyPath);
            return new AccountLoginForMailClient(new AccountLoginForMailClientArgs(settingsName, subject, mailAddress, passCode, bodyPath, footerPath)).SendAsync();
        }

    }
}