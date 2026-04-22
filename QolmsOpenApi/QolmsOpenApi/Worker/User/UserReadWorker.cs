using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// ユーザー情報取得処理
    /// </summary>
    public class UserReadWorker: UserWorkerBase
    {
        IPasswordManagementRepository _passwordManagementRepo;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="storageRepository"></param>
        /// <param name="passwordManagementRepository"></param>
        public UserReadWorker(IAccountRepository accountRepository, IFamilyRepository familyRepository, IStorageRepository storageRepository, IPasswordManagementRepository passwordManagementRepository) : base(accountRepository, familyRepository)
        {
            _passwordManagementRepo = passwordManagementRepository;
        }

        /// <summary>
        /// 取得処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserReadApiResults Read(QoUserReadApiArgs args)
        {
            var results = new QoUserReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 親アカウントキー変換
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var parentAccountKey))
            {
                return results;
            }

            Guid targetAccountKey;
            if (!string.IsNullOrWhiteSpace(args.AccountKeyReference))
            {
                // ターゲットアカウントキー変換
                if (!args.AccountKeyReference.ToDecrypedReference().CheckArgsConvert(nameof(args.AccountKeyReference), Guid.Empty, results, out var childAccountKey))
                {
                    return results;
                }
                targetAccountKey = childAccountKey;
            }
            else
            {
                targetAccountKey = parentAccountKey;
            }


            var loginId = string.Empty;
            var mailAddress = string.Empty;
            var phoneNumber = string.Empty;
            // 自身のアカウントが対象でなければ
            if (parentAccountKey != targetAccountKey)
            {
                // 親子関係チェック
                if (!TryCheckRelation(parentAccountKey, targetAccountKey, results))
                {
                    return results;
                }
            }
            else
            {
                // 親アカウントが対象であればメールアドレスも取得する
                if(!TryReadMailAddress(parentAccountKey, results, out var mail, out var userId))
                {
                    return results;
                }
                mailAddress = mail;
                loginId = userId;

                // 認証用の電話番号も取得する
                if(!TryReadPhoneNumber(parentAccountKey, results, out var phone))
                {
                    return results;
                }
                phoneNumber = phone;
            }
            
            // アカウント情報取得
            if (!TryReadAccountIndex(targetAccountKey, results, out var entity))
            {
                return results;
            }

            var accessKey = CreateAccesskey(parentAccountKey, targetAccountKey, args.Executor);

            results.User = new QoApiUserItem
            {
                AccountKeyReference = targetAccountKey.ToEncrypedReference(),
                LoginId = loginId,
                FamilyName = entity.FAMILYNAME,
                GivenName = entity.GIVENNAME,
                FamilyNameKana = entity.FAMILYKANANAME,
                GivenNameKana = entity.GIVENKANANAME,
                NickName = entity.NICKNAME,
                Birthday = entity.BIRTHDAY.ToApiDateString(),
                PersonPhotoReference = entity.PHOTOKEY.ToEncrypedReference(),
                Sex = entity.SEXTYPE,
                AccessKey = accessKey,
                Mail = mailAddress,
                AccountPhoneNumber = phoneNumber,
            };

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        string CreateAccesskey(Guid parentAccountKey, Guid accountKey, string executor)
        {
            if(parentAccountKey == accountKey)
            {
                return string.Empty;
            }

            // アクセスキーの生成
            var tokenProvider = new QsJwtTokenProvider();
            var encExeCutor = executor.TryToValueType(Guid.Empty).ToString("N").TryEncrypt();
            var accessKey = tokenProvider.CreateOpenApiJwtAccessKey(encExeCutor, accountKey, parentAccountKey, (int)QoApiFunctionTypeEnum.All);

            return accessKey;
        }

        bool TryReadMailAddress(Guid accountKey, QoApiResultsBase results, out string mail, out string userId)
        {
            mail = string.Empty;
            userId = string.Empty;
            try
            {
                var entity = _passwordManagementRepo.ReadDecryptedEntity(accountKey);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "メールアドレス情報が存在しませんでした。");
                    return false;
                }

                mail = entity.PASSWORDRECOVERYMAILADDRESS;
                userId = entity.USERID;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "メールアドレス情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryReadPhoneNumber(Guid accountKey, QoApiResultsBase results, out string phoneNumber)
        {
            phoneNumber = string.Empty;
            try
            {
                var entity = _accountRepo.ReadPhoneEntity(accountKey);
                if(entity != null)
                {
                    phoneNumber = entity.PHONENUMBER;
                }                
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "電話番号の取得に失敗しました。");
                return false;
            }
        }
    }
}