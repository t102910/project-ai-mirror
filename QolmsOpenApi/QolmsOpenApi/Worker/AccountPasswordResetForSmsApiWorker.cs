using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// パスワードリセット処理（SMS認証用）
    /// </summary>
    public class AccountPasswordResetForSmsApiWorker
    {
        IPasswordManagementRepository _passwordManagementRepository;
        ISmsAuthCodeRepository _smsAuthCodeRepo;
        IAccountRepository _accountRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="passwordManagementRepository"></param>
        /// <param name="smsAuthCodeRepository"></param>
        /// <param name="accountRepository"></param>
        public AccountPasswordResetForSmsApiWorker(
            IPasswordManagementRepository passwordManagementRepository,
            ISmsAuthCodeRepository smsAuthCodeRepository,
            IAccountRepository accountRepository)
        {
            _passwordManagementRepository = passwordManagementRepository;
            _smsAuthCodeRepo = smsAuthCodeRepository;
            _accountRepo = accountRepository;
        }

        /// <summary>
        ///  パスワードリセット処理（SMS認証用）実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAccountPasswordResetForSmsApiResults PasswordResetRequest(QoAccountPasswordResetForSmsApiArgs args)
        {
            var results = new QoAccountPasswordResetForSmsApiResults
            {
                IsSuccess = bool.FalseString
            };

            // パスワード 必須チェック
            if (!args.Password.CheckArgsRequired(nameof(args.Password), results))
            {
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

            // アカウントキー取得
            var accountKey = GetAccountKey(args.PhoneNumber);

            // 生年月日の入力チェック
            if (!string.IsNullOrWhiteSpace(args.BirthDate))
            {
                // 生年月日が設定されていれば生年月日も照合する
                if(!CheckBirthDate(accountKey, args.BirthDate, results))
                {
                    return results;
                }
            }

            // 新しいパスワードの有効性チェック
            if (!CheckNewPassword(accountKey, args.Password, results))
            {
                return results;
            }

            // 新しいパスワードを更新
            if (!ReplasePassword(accountKey, args.Password, results))
            {
                return results;
            }
        
            // 成功
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.Success);

            return results;
        }

        /// <summary>
        /// 認証コードチェック
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="authCode"></param>
        /// <param name="results"></param>
        /// <returns></returns>
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

        // 生年月日チェック
        bool CheckBirthDate(Guid accountKey, string birthDate, QoApiResultsBase results)
        {
            var birthDateTime = birthDate.TryToValueType(DateTime.MinValue);
            if(birthDateTime == DateTime.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "生年月日が不正です。");
                return false;
            }

            try
            {
                var entity = _accountRepo.ReadAccountIndexDat(accountKey);
                if(entity.BIRTHDAY.Date != birthDateTime.Date)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountBirthDateMismatch, "生年月日が一致しませんでした。");
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "生年月日取得処理でエラーが発生しました。");
                return false;
            }
        }

        /// <summary>
        /// パスワード変更処理
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="newPassword"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        bool ReplasePassword(Guid authorKey, string newPassword, QoApiResultsBase results)
        {
            try
            {
                // パスワード変更
                _passwordManagementRepository.EditPassword(authorKey, newPassword);

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "パスワード変更処理に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// 入力されたパスワードをチェックします。
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="newPassword"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        /// <remarks>チェック内容はQoPasswordManagementより移植</remarks>
        public bool CheckNewPassword(Guid authorKey, string newPassword, QoApiResultsBase results)
        {
            bool result = false;

            if (authorKey != Guid.Empty)
            {
                string strErrorMessage = string.Empty;

                // DB から取得
                var entity = _passwordManagementRepository.ReadDecryptedEntity(authorKey);
                if (entity != null)
                {
                    if (entity.USERID.Trim() == newPassword.Trim())
                    {
                        strErrorMessage = "UserIDとパスワードが同じです。";
                    }
                    else if (string.IsNullOrWhiteSpace(entity.USERPASSWORD))
                    {
                        strErrorMessage = "登録済みパスワードが空白です。";
                    }
                    else if (entity.LASTUPDATEPASSWORDDATE == DateTime.MinValue)
                    {
                        strErrorMessage = "登録済みパスワードの更新日時が不正です。";
                    }
                    else if (entity.USERPASSWORD.CompareTo(newPassword) == 0)
                    {
                        strErrorMessage = "現在のパスワードと新しいパスワードが同じです。";
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    strErrorMessage = "パスワードが無効です。";
                }

                if (!result)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, strErrorMessage);
                }
            }
            else
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.UnknownError);
            }

            return result;
        }

        /// <summary>
        /// アカウントキーの取得
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private Guid GetAccountKey(string phoneNumber)
        {
            var acccountMst = _accountRepo.ReadPhoneEntityByNumber(phoneNumber);

            return acccountMst.ACCOUNTKEY;
        }
    }
}