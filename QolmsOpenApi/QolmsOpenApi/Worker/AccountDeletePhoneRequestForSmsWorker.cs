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
    /// 電話番号削除処理(SMS認証用)
    /// </summary>
    public class AccountDeletePhoneRequestForSmsWorker
    {
        IAccountRepository _accountRepo;
        ISmsAuthCodeRepository _smsAuthCodeRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="smsAuthCodeRepository"></param>
        public AccountDeletePhoneRequestForSmsWorker(
            IAccountRepository accountRepository,
            ISmsAuthCodeRepository smsAuthCodeRepository)
        {
            _accountRepo = accountRepository;
            _smsAuthCodeRepo = smsAuthCodeRepository;
        }

        /// <summary>
        /// 電話番号削除処理実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAccountDeletePhoneRequestForSmsApiResults DeletePhoneRequest(QoAccountDeletePhoneRequestForSmsApiArgs args)
        {
            var results = new QoAccountDeletePhoneRequestForSmsApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーに変換
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アカウントキーが不正です。");
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

            // 電話番号削除
            if (!DeletePhoneNumber(accountKey, results))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool DeletePhoneNumber(Guid accountKey, QoApiResultsBase results)
        {
            try
            {
                // AccountPhoneマスタを取得
                var entity = _accountRepo.ReadPhoneEntity(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "電話番号は未登録です。");
                    return false;
                }                                  

                // AccountPhoneマスタレコードを物理削除
                _accountRepo.PhysicalDeletePhoneEntity(accountKey);                

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "電話番号削除処理に失敗しました。");
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