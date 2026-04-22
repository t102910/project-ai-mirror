using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// SMSログイン1段階目処理
    /// </summary>
    public class AccountLoginForSmsWorker
    {
        IAccountRepository _accountRepo;
        IPasswordManagementRepository _passwordRepo;
        ISmsAuthCodeRepository _smsAuthCodeRepo;
        IQoSmsClient _smsClient;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="passwordManagementRepository"></param>
        /// <param name="smsAuthCodeRepository"></param>
        /// <param name="smsClient"></param>
        public AccountLoginForSmsWorker(
            IAccountRepository accountRepository,
            IPasswordManagementRepository passwordManagementRepository,
            ISmsAuthCodeRepository smsAuthCodeRepository,
            IQoSmsClient smsClient)
        {
            _accountRepo = accountRepository;
            _passwordRepo = passwordManagementRepository;
            _smsAuthCodeRepo = smsAuthCodeRepository;
            _smsClient = smsClient;
        }

        /// <summary>
        /// SMSログイン処理1段階目実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoAccountLoginForSmsApiResults> LoginForSms(QoAccountLoginForSmsApiArgs args)
        {
            var results = new QoAccountLoginForSmsApiResults
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
            if (!args.Executor.CheckArgsConvert(nameof(args.Executor),Guid.Empty, results,out var executor))
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

            // パスワード照合
            if(!TryVerifyPassword(accountKey, args.Password, results))
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
            if (!await SendSms(args.ExecuteSystemType, args.PhoneNumber, authCode, results))
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
                var entity = _accountRepo.ReadPhoneEntityByNumber(phoneNumber);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "電話番号に対応するアカウントが存在しませんでした。");
                    return false;
                }

                accountKey = entity.ACCOUNTKEY;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント照合処理でエラーが発生しました。");
                return false;
            }
        }

        bool TryVerifyPassword(Guid accountKey, string password, QoApiResultsBase results)
        {
            try
            {
                var entity = _passwordRepo.ReadEntity(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "アカウントが存在しませんでした。");
                    return false;
                }

                // パスワード不一致
                if(entity.USERPASSWORD != password.TryEncrypt())
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "パスワードが一致しませんでした。");
                    return false;
                }

                return true;
            }
            catch(Exception ex)
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
                _smsAuthCodeRepo.InsertEntity(entity);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "SMS認証コードの登録に失敗しました。");
                return false;
            }
        }

        async Task<bool> SendSms(string systemType, string phoneNumber, string authCode, QoApiResultsBase results)
        {
            // システム名称取得
            var systemName = systemType.TryToValueType(QsApiSystemTypeEnum.None).ToSystemName();
            // SMSメッセージ
            var message = $"【{systemName}】認証コード: {authCode}";

            try
            {
                // SMS送信
                await _smsClient.SendSms(phoneNumber, message);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "SMSの送信に失敗しました。");
                return false;
            }
        }
    }
}