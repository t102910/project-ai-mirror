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
using MGF.QOLMS.QolmsCryptV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// ユーザー情報更新処理
    /// </summary>
    public class UserWriteWorker: UserWorkerBase
    {
        IStorageRepository _storageRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="storageRepository"></param>
        public UserWriteWorker(IAccountRepository accountRepository, IFamilyRepository familyRepository, IStorageRepository storageRepository):base(accountRepository, familyRepository)
        {
            _storageRepo = storageRepository;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserWriteApiResults Write(QoUserWriteApiArgs args)
        {
            var results = new QoUserWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 親アカウントキー変換
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var parentAccountKey))
            {
                return results;
            }                      

            var user = args.User;

            // ターゲットアカウントキー変換
            if(!user.AccountKeyReference.ToDecrypedReference().CheckArgsConvert(nameof(user.AccountKeyReference),Guid.Empty, results, out var accountKey)){
                return results;
            }

            // 写真キー取得
            var photoKey = user.PersonPhotoReference.ToDecrypedReference().TryToValueType(Guid.Empty);

            // 性別変換チェック
            var sexType = user.Sex.TryConvertOrDefault(QsDbSexTypeEnum.None);
            if (sexType == QsDbSexTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(user.Sex)}が不正です。");
                return results;
            }

            // 生年月日変換チェック
            var birthDate = user.Birthday.TryToValueType(DateTime.MinValue);
            if (birthDate == DateTime.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(user.Birthday)}が不正です。");
                return results;
            }

            // 必須チェック
            if (!user.FamilyName.CheckArgsRequired(nameof(user.FamilyName), results) ||
               !user.GivenName.CheckArgsRequired(nameof(user.GivenName), results) ||
               !user.FamilyNameKana.CheckArgsRequired(nameof(user.FamilyNameKana), results) ||
               !user.GivenNameKana.CheckArgsRequired(nameof(user.GivenNameKana), results))
            {
                return results;
            }

            // 親アカウント妥当性チェック
            if(!CheckParentAccount(parentAccountKey, results))
            {
                return results;
            }

            // 自身のアカウントが対象でなければ
            if (parentAccountKey != accountKey)
            {
                // 親子関係チェック
                if (!TryCheckRelation(parentAccountKey, accountKey, results))
                {
                    return results;
                }
            }

            // アカウント情報取得
            if(!TryReadAccountIndex(accountKey, results, out var entity))
            {
                return results;
            }

            // 情報更新
            entity.FAMILYNAME = user.FamilyName;
            entity.FAMILYKANANAME = user.FamilyNameKana;
            entity.GIVENNAME = user.GivenName;
            entity.GIVENKANANAME = user.GivenNameKana;
            entity.NICKNAME = user.NickName;
            entity.PHOTOKEY = photoKey;
            if(parentAccountKey != accountKey)
            {
                // 生年月日・性別は子アカウントのみ変更可能
                entity.SEXTYPE = user.Sex;
                entity.BIRTHDAY = birthDate;
            }

            // DB更新
            if(!TryWriteAccoutIndex(entity, results))
            {
                return results;
            }

            // 画像処理
            if (photoKey != Guid.Empty)
            {
                // メタデータの更新(成功の有無は問わない)
                _storageRepo.UpdateImageMetaDataAccountKey(photoKey, accountKey);
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

    }
}