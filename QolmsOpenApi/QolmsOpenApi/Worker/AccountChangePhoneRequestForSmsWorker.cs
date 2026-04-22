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
using MGF.QOLMS.QolmsOpenApi.Enums;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 電話番号変更処理（SMS認証用）
    /// </summary>
    public class AccountChangePhoneRequestForSmsWorker
    {
        IAccountRepository _accountRepo;
        ISmsAuthCodeRepository _smsAuthCodeRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="smsAuthCodeRepository"></param>
        public AccountChangePhoneRequestForSmsWorker(
            IAccountRepository accountRepository,
            ISmsAuthCodeRepository smsAuthCodeRepository)
        {
            _accountRepo = accountRepository;
            _smsAuthCodeRepo = smsAuthCodeRepository;
        }

        /// <summary>
        ///  電話番号変更処理（SMS認証用）実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAccountChangePhoneRequestForSmsApiResults ChangePhoneRequest(QoAccountChangePhoneRequestForSmsApiArgs args)
        {
            var results = new QoAccountChangePhoneRequestForSmsApiResults
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

            // 認証キーのデコード
            var authKey = args.AuthKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (authKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.AuthKeyReference)}が不正です。");
                return results;
            }

            // 認証コード照合
            if (!CheckAuthCode(authKey, args.AuthCode, results))
            {
                return results;
            }

            // 新しい電話番号重複チェック
            var existEntity = _accountRepo.ReadPhoneEntityByNumber(args.PhoneNumber);
            if (existEntity != null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountPhoneDuplicate, "この電話番号は既に使用されています。");
                return results;
            }

            // アカウントキーに変換
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            // 新しい電話番号を更新
            if (!ReplasePhoneNumber(accountKey, args.PhoneNumber, results))
            {
                return results;
            }

            // 成功
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.Success);

            return results;
        }

        // アカウント電話番号更新処理
        bool ReplasePhoneNumber(Guid accountKey, string phoneNumber, QoApiResultsBase results)
        {
            try
            {
                // AccountPhoneマスタを取得
                var entity = _accountRepo.ReadPhoneEntity(accountKey);
                if (entity == null)
                {
                    entity = new QH_ACCOUNTPHONE_MST
                    {
                        ACCOUNTKEY = accountKey,
                        PHONENUMBER = phoneNumber,
                    };

                    // 電話番号新規登録
                    _accountRepo.InsertPhoneEntity(entity);
                }
                else
                {
                    // 電話番号変更
                    entity.PHONENUMBER = phoneNumber;

                    // AccountPhoneマスタを更新
                    _accountRepo.UpdatePhoneEntity(entity);
                }                

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "電話番号登録処理に失敗しました。");
                return false;
            }
        }

        bool CheckAuthCode(Guid authKey, string authCode, QoApiResultsBase results)
        {
            try
            {
                var entity = _smsAuthCodeRepo.ReadEntity(authKey);
                var now = DateTime.Now;
                if (entity.EXPIRES < now)
                {
                    // 期限切れ
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SmsAuthCodeExpired, "認証コードの期限が切れています。");
                    return false;
                }

                if (entity.FAILURECOUNT >= 2)
                {
                    // 試行回数オーバー
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SmsAuthCodeCountOver, "認証コードが一定回数間違えたため無効となっています。");
                    return false;
                }

                if (entity.AUTHCODE != authCode)
                {
                    // 認証コード不一致
                    // 失敗回数カウントアップ＆更新
                    entity.FAILURECOUNT++;
                    _smsAuthCodeRepo.UpdateEntity(entity);
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SmsAuthCodeInvalid, "認証コードが一致しませんでした。");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "認証コード照合処理に失敗しました。");
                return false;
            }
        }
    }
}