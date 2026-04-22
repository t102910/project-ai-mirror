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
    /// 家族ユーザー追加処理
    /// </summary>
    public class UserFamilyAddWorker: UserWorkerBase
    {
        IStorageRepository _storageRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="storageRepository"></param>
        public UserFamilyAddWorker(IAccountRepository accountRepository, IFamilyRepository familyRepository,IStorageRepository storageRepository):base(accountRepository, familyRepository)
        {
            _storageRepo = storageRepository;
        }

        /// <summary>
        /// 追加処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserFamilyAddApiResults Add(QoUserFamilyAddApiArgs args)
        {
            var results = new QoUserFamilyAddApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            var user = args.User;

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
            if(!user.FamilyName.CheckArgsRequired(nameof(user.FamilyName),results) ||
               !user.GivenName.CheckArgsRequired(nameof(user.GivenName), results) ||
               !user.FamilyNameKana.CheckArgsRequired(nameof(user.FamilyNameKana), results) ||
               !user.GivenNameKana.CheckArgsRequired(nameof(user.GivenNameKana), results))
            {
                return results;
            }

            // 親アカウント確認
            if(!CheckParentAccount(accountKey, results))
            {
                return results;
            }

            // 家族追加
            if(!TryAddFamilyAccount(accountKey, args.User, birthDate, photoKey, results, out var childAccountKey))
            {
                return results;
            }

            // 画像処理
            if(photoKey != Guid.Empty)
            {
                // メタデータの更新(成功の有無は問わない)
                _storageRepo.UpdateImageMetaDataAccountKey(photoKey, childAccountKey);
            }

            // アカウントキー参照のセット
            args.User.AccountKeyReference = childAccountKey.ToEncrypedReference();

            // アクセスキーの生成
            var tokenProvider = new QsJwtTokenProvider();
            var encExeCutor = args.Executor.TryToValueType(Guid.Empty).ToString("N").TryEncrypt();
            args.User.AccessKey = tokenProvider.CreateOpenApiJwtAccessKey(encExeCutor, childAccountKey, accountKey, (int)QoApiFunctionTypeEnum.All);

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.User = args.User;

            return results;
        }


        bool TryAddFamilyAccount(Guid parentAccountKey, QoApiUserItem user, DateTime birthDate, Guid photoKey, QoApiResultsBase results, out Guid childAccountKey)
        {
            childAccountKey = Guid.Empty;
            try
            {
                childAccountKey = _familyRepo.AddFamily(parentAccountKey, user, birthDate, photoKey);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "家族アカウント追加処理でエラーが発生しました。");
                return false;
            }
        }
    }
}