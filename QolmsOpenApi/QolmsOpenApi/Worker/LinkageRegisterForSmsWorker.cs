using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 新規連携ユーザー登録処理
    /// </summary>
    public class LinkageRegisterForSmsWorker : LinkageRegisterWorkerBase
    {
        ISmsAuthCodeRepository _smsAuthCodeRepository;
        IQoSmsClient _smsClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkageRepository"></param>
        /// <param name="identityApiRepository"></param>
        /// <param name="accountRepository"></param>
        /// <param name="smsAuthCodeRepository"></param>
        /// <param name="smsClient"></param>
        public LinkageRegisterForSmsWorker(
            ILinkageRepository linkageRepository, 
            IIdentityApiRepository identityApiRepository, 
            IAccountRepository accountRepository,
            ISmsAuthCodeRepository smsAuthCodeRepository,
            IQoSmsClient smsClient) : base(linkageRepository, identityApiRepository, accountRepository)
        {
            _smsAuthCodeRepository = smsAuthCodeRepository;
            _smsClient = smsClient;
        }

        /// <summary>
        /// 登録処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoLinkageNewUserRegisterForSmsApiResults> RegisterNewUser(QoLinkageNewUserRegisterForSmsApiArgs args)
        {            
            if (args.WithPatientCard.TryToValueType(false))
            {
                // 診察券ありのシステムの場合
                return await RegisterWithPatientCard(args);
            }
            else
            {
                // 診察券なしのシステムの場合
                return await RegisterLinkageOnly(args);
            }
        }

        async Task<QoLinkageNewUserRegisterForSmsApiResults> RegisterWithPatientCard(QoLinkageNewUserRegisterForSmsApiArgs args)
        {
            var results = new QoLinkageNewUserRegisterForSmsApiResults
            {
                IsSuccess = bool.FalseString
            };

            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);
            if (linkageSystemNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.LinkageSystemNo)}情報が不正です。");
                return results;
            }

            // 認証キーのデコード
            var authKey = args.AuthKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (authKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.AuthKeyReference)}が不正です。");
                return results;
            }

            // 施設キー変換チェック
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FacilityKeyが不明です");
                return results;
            }

            // 生年月日変換チェック
            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            if (birthDate == DateTime.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Birthday)}が不正です。");
                return results;
            }

            // 性別変換チェック
            var sexType = (QsDbSexTypeEnum)args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);
            if (sexType == QsDbSexTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Sex)}が不正です。");
                return results;
            }

            // 必須チェック
            if (!ArgsValidationHelper.CheckArgsRequired(args.FamilyName, nameof(args.FamilyName), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.GivenName, nameof(args.GivenName), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.GivenNameKana, nameof(args.GivenNameKana), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.FamilyNameKana, nameof(args.FamilyNameKana), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Password, nameof(args.Password), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Sex, nameof(args.Sex), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Birthday, nameof(args.Birthday), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.LinkageSystemId, nameof(args.LinkageSystemId), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.AuthCode, nameof(args.AuthCode), results))
            {
                return results;
            }

            // 電話番号形式になっているかどうか
            if (!Regex.IsMatch(args.AccountPhoneNumber, @"^[0-9]+$"))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "電話番号が不正です。");
                return results;
            }

            // 規約同意チェック
            if (!args.IsAgreePrivacyPolicy.TryToValueType(false))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "プライバシーポリシーの同意がありません。");
                return results;
            }
            if (!args.IsAgreeTermsOfService.TryToValueType(false))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "利用規約の同意がありません。");
                return results;
            }

            // 家族情報がある場合は家族情報引数チェック
            if (args.FamilyAccountInfo != null)
            {
                var familyinfoErrors = new List<string>();
                if (!args.FamilyAccountInfo.Validate(ref familyinfoErrors))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"家族情報が不正です。{string.Join(",", familyinfoErrors)}");
                    return results;
                }
            }

            // 認証コード照合
            if (!CheckAuthCode(authKey, args.AuthCode, results))
            {
                return results;
            }

            // 診察券の連携システム番号を取得
            var cardLinkageNo = _linkageRepository.GetLinkageNo(facilityKey);
            if (cardLinkageNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "施設キーを連携システム番号に変換できませんでした。");
                return results;
            }

            // 退会後既定時間内の新規登録の場合エラー返却
            if (!CheckRegisterInterval(args.LinkageSystemId, facilityKey, results))
            {
                return results;
            }

            // 利用者カード重複チェック
            if (!CheckCardNo(facilityKey, args.LinkageSystemId, results))
            {
                return results;
            }

            // 同一情報のユーザーの存在チェック
            if (!CheckDuplicatePhoneNumber(args.AccountPhoneNumber, results))
            {
                return results;
            }

            // 仮連携ID生成
            var newLinkageSystemID = Guid.NewGuid();

            // ユーザー登録処理
            var registerResult = TryRegisterUser(linkageSystemNo, newLinkageSystemID.ToString("N"), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, string.Empty);

            if (!registerResult.isSuccess)
            {
                // エラー
                results.Result = registerResult.apiResult;
                return results;
            }

            // 仮で設定していたPush通知IDを更新する
            if(!TryUpdateNotificationId(registerResult.accountKey, linkageSystemNo, results))
            {
                // 失敗時アカウントも削除
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // NickNameが指定されていればNickNameを更新する
            if (!string.IsNullOrWhiteSpace(args.NickName))
            {
                if (!TryUpdateNickName(registerResult.accountKey, args.NickName, results))
                {
                    // 失敗時アカウントも削除
                    WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                    return results;
                }
            }

            // 診察券対象アカウントキー設定
            var cardAccountKey = registerResult.accountKey;
            // 家族ユーザー登録
            if (args.FamilyAccountInfo != null)
            {
                if (!AddFamilyUser(registerResult.accountKey, args.ExecutorName, args.FamilyAccountInfo, results, out var familyAccountKey))
                {
                    // 失敗時アカウントも削除
                    WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                    return results;
                }
                // 診察券対象アカウントキーを家族に
                cardAccountKey = familyAccountKey;
            }

            // 親施設の取得と連携登録
            if (!AddParentLinkage(cardAccountKey, facilityKey, linkageSystemNo, results))
            {
                // 失敗時アカウントも削除
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // 診察券登録処理
            if (!AddPatientCard(args.AuthorKey.TryToValueType(Guid.Empty), cardAccountKey, cardLinkageNo, facilityKey, args.LinkageSystemId, results, out var _))
            {
                // 失敗時アカウントも削除（子も削除されるはず）
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            if (!RegisterPhoneNumber(registerResult.accountKey, args.AccountPhoneNumber, results))
            {
                // 失敗時アカウントも削除（子も削除されるはず）
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // SMS認証コードを削除
            DeleteAuthCode(authKey);

            // ID案内SMS送信
            await SendSms(args.ExecuteSystemType, args.AccountPhoneNumber, registerResult.userId);

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.UserId = registerResult.userId;
            results.LinkageIdReference = registerResult.accountKey.ToEncrypedReference();
            results.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(GetEncryptExecutor(args.Executor), registerResult.accountKey);

            return results;
        }

        async Task<QoLinkageNewUserRegisterForSmsApiResults> RegisterLinkageOnly(QoLinkageNewUserRegisterForSmsApiArgs args)
        {
            var results = new QoLinkageNewUserRegisterForSmsApiResults
            {
                IsSuccess = bool.FalseString
            };

            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);
            if (linkageSystemNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.LinkageSystemNo)}情報が不正です。");
                return results;
            }

            // 認証キーのデコード
            var authKey = args.AuthKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (authKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.AuthKeyReference)}が不正です。");
                return results;
            }

            // 施設キー変換
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);            

            // 生年月日変換チェック
            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            if (birthDate == DateTime.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Birthday)}が不正です。");
                return results;
            }

            // 性別変換チェック
            var sexType = (QsDbSexTypeEnum)args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);
            if (sexType == QsDbSexTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Sex)}が不正です。");
                return results;
            }

            // 必須チェック
            if (!ArgsValidationHelper.CheckArgsRequired(args.FamilyName, nameof(args.FamilyName), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.GivenName, nameof(args.GivenName), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.GivenNameKana, nameof(args.GivenNameKana), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.FamilyNameKana, nameof(args.FamilyNameKana), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Password, nameof(args.Password), results) ||                
                !ArgsValidationHelper.CheckArgsRequired(args.Birthday, nameof(args.Birthday), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.AuthCode, nameof(args.AuthCode), results))
            {
                return results;
            }

            // 電話番号形式になっているかどうか
            if (!Regex.IsMatch(args.AccountPhoneNumber, @"^[0-9]+$"))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "電話番号が不正です。");
                return results;
            }

            // 規約同意チェック
            if (!args.IsAgreePrivacyPolicy.TryToValueType(false))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "プライバシーポリシーの同意がありません。");
                return results;
            }
            if (!args.IsAgreeTermsOfService.TryToValueType(false))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "利用規約の同意がありません。");
                return results;
            }

            // 認証コード照合
            if (!CheckAuthCode(authKey, args.AuthCode, results))
            {
                return results;
            }

            // 同一情報のユーザーの存在チェック
            if (!CheckDuplicatePhoneNumber(args.AccountPhoneNumber, results))
            {
                return results;
            }

            // 仮Push通知ID生成
            var newLinkageSystemID = Guid.NewGuid();

            // ユーザー登録処理
            var registerResult = TryRegisterUser(linkageSystemNo, newLinkageSystemID.ToString("N"), args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, $"{args.Sex}", args.Birthday, string.Empty);

            if (!registerResult.isSuccess)
            {
                // エラー
                results.Result = registerResult.apiResult;
                return results;
            }

            // 仮で設定していたPush通知IDを更新する
            if (!TryUpdateNotificationId(registerResult.accountKey, linkageSystemNo, results))
            {
                // 失敗時アカウントも削除
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // NickNameが指定されていればNickNameを更新する
            if (!string.IsNullOrWhiteSpace(args.NickName))
            {
                if (!TryUpdateNickName(registerResult.accountKey, args.NickName, results))
                {
                    // 失敗時アカウントも削除
                    WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                    return results;
                }
            }

            //  施設キーが指定されている場合は連携登録する
            if (facilityKey != Guid.Empty)
            {
                if (!TryAddLinkageSystem(facilityKey, registerResult.accountKey, results))
                {
                    // 失敗時アカウントも削除
                    WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                    return results;
                }
            }

            // 電話番号の登録
            if (!RegisterPhoneNumber(registerResult.accountKey, args.AccountPhoneNumber, results))
            {
                // 失敗時アカウントも削除（子も削除されるはず）
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // SMS認証コードを削除
            DeleteAuthCode(authKey);

            // ID案内SMS送信
            await SendSms(args.ExecuteSystemType, args.AccountPhoneNumber, registerResult.userId);

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.UserId = registerResult.userId;
            results.LinkageIdReference = registerResult.accountKey.ToEncrypedReference();
            results.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(GetEncryptExecutor(args.Executor), registerResult.accountKey);

            return results;
        }

        /// <summary>
        /// 認証コードチェック
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="authCode"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool CheckAuthCode(Guid authKey, string authCode, QoApiResultsBase results)
        {
            try
            {
                var entity = _smsAuthCodeRepository.ReadEntity(authKey);
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
                    _smsAuthCodeRepository.UpdateEntity(entity);
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

        /// <summary>
        /// アカウント電話番号登録処理
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool RegisterPhoneNumber(Guid accountKey, string phoneNumber, QoApiResultsBase results)
        {
            try
            {
                // 重複チェック
                var existEntity = _accountRepository.ReadPhoneEntityByNumber(phoneNumber);
                if (existEntity != null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountPhoneDuplicate, "この電話番号は既に使用されています。");
                    return false;
                }

                var entity = new QH_ACCOUNTPHONE_MST
                {
                    ACCOUNTKEY = accountKey,
                    PHONENUMBER = phoneNumber,
                };

                // 登録
                _accountRepository.InsertPhoneEntity(entity);

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "電話番号登録処理に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// SMS送信
        /// </summary>
        /// <param name="systemType"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="userId"></param>
        protected async Task SendSms(string systemType, string phoneNumber, string userId)
        {
            var system = systemType.TryToValueType(QsApiSystemTypeEnum.None);
            if(system == QsApiSystemTypeEnum.MeiNaviiOSApp ||
                system == QsApiSystemTypeEnum.MeiNaviAndroidApp || 
                system == QsApiSystemTypeEnum.MeiNaviApp)
            {
                // MEIナビ(ツーイン)はSMS通知は送信しない
                return;
            }

            // システム名称取得
            var systemName = systemType.TryToValueType(QsApiSystemTypeEnum.None).ToSystemName();
            // SMSメッセージ
            var message = $"【{systemName}】本登録が完了いたしました。\nあなたのユーザーID\n{userId}\nアプリを再インストールしたり、機種変更などされた場合は、ユーザーIDとアプリ内で設定したパスワードでログインして利用を再開することができます。";

            try
            {
                // SMS送信
                await _smsClient.SendSms(phoneNumber, message);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, "アカウント登録完了後のSMS送信に失敗しました。", Guid.Empty);
            }
        }

        /// <summary>
        /// SMSAuthCodeレコード削除
        /// </summary>
        /// <param name="authKey"></param>
        protected void DeleteAuthCode(Guid authKey)
        {
            try
            {
                _smsAuthCodeRepository.PhysicalDeleteEntity(authKey);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, "SMS認証コードの削除に失敗しました。", Guid.Empty);
            }
        }

        bool CheckDuplicatePhoneNumber(string phoneNumber, QoApiResultsBase results)
        {
            try
            {
                var entity = _accountRepository.ReadPhoneEntityByNumber(phoneNumber);
                if (entity != null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountPhoneDuplicate, "この電話番号は既に登録済みです。");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "電話番号登録確認処理でエラーが発生しました。");
                return false;
            }
        }

        bool TryAddLinkageSystem(Guid facilityKey, Guid accountKey, QoApiResultsBase results)
        {
            try
            {
                var linkageSystemNo = _linkageRepository.GetLinkageNo(facilityKey);

                var entity = new QH_LINKAGE_DAT
                {
                    ACCOUNTKEY = accountKey,
                    LINKAGESYSTEMNO = linkageSystemNo,
                    LINKAGESYSTEMID = "",
                    DATASET = "",
                    STATUSTYPE = 2, // 承認済みとして登録                    
                };

                _linkageRepository.InsertEntity(entity);

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "連携システムの登録に失敗しました");
                return false;
            }
        }
    }
}