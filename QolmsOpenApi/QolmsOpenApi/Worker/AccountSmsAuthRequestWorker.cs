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

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// SMS認証リクエスト処理
    /// </summary>
    public class AccountSmsAuthRequestWorker
    {
        ISmsAuthCodeRepository _smsAuthCodeRepo;
        IAccountRepository _accountRepo;
        IQoSmsClient _smsClient;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="smsAuthCodeRepository"></param>
        /// <param name="smsClient"></param>
        public AccountSmsAuthRequestWorker(
            ISmsAuthCodeRepository smsAuthCodeRepository, 
            IAccountRepository accountRepository,
            IQoSmsClient smsClient)
        {
            _smsAuthCodeRepo = smsAuthCodeRepository;
            _accountRepo = accountRepository;
            _smsClient = smsClient;
        }

        /// <summary>
        ///  SMS認証リクエスト処理実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoAccountSmsAuthRequestApiResults> SmsAuthRequest(QoAccountSmsAuthRequestApiArgs args)
        {
            var results = new QoAccountSmsAuthRequestApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 電話番号 必須チェック
            if (!args.PhoneNumber.CheckArgsRequired(nameof(args.PhoneNumber),results))
            {
                return results;
            }

            // 電話番号 数字だけで構成されているかチェック
            if(!Regex.IsMatch(args.PhoneNumber, @"^[0-9]+$"))
            {
                results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.PhoneNumber)}は数字だけで構成されている必要があります。");
                return results;
            }

            // 電話番号登録チェックが必須になっていればチェックする
            if (args.RequirePhoneNumberRegistration)
            {
                if(!TryCheckNumberRegistration(args.PhoneNumber, results))
                {
                    return results;
                }
            }

            // 認証キー発行
            var authKey = Guid.NewGuid();
            // 6桁の認証コード発行
            var authCode = QoSmsClient.GenerateAuthCode(6);

            // 認証コード情報を保存
            if(!SaveAuthCode(authKey, authCode, results))
            {
                return results;
            }

            // SMSで認証コードを送信
            if(!(await SendSms(args.ExecuteSystemType, args.PhoneNumber, authCode, results)))
            {
                return results;
            }

            // 成功
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.Success);
            // 認証キーを暗号化して返す
            results.AuthKeyReference = authKey.ToEncrypedReference();

            return results;
        }

        bool TryCheckNumberRegistration(string phoneNumber, QoApiResultsBase results)
        {
            try
            {
                var entity = _accountRepo.ReadPhoneEntityByNumber(phoneNumber);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.AccountPhoneNotFound, "電話番号が未登録です。");
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "電話番号登録確認処理でエラーが発生しました。");
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
            catch(Exception ex)
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
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "SMSの送信に失敗しました。");
                return false;
            }
        }
    }
}