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
    /// 家族削除処理
    /// </summary>
    public class UserFamilyDeleteWorker: UserWorkerBase
    {
        IStorageRepository _storageRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="storageRepository"></param>
        public UserFamilyDeleteWorker(IAccountRepository accountRepository, IFamilyRepository familyRepository, IStorageRepository storageRepository) : base(accountRepository, familyRepository)
        {
            _storageRepo = storageRepository;
        }

        /// <summary>
        /// 削除実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserFamilyDeleteApiResults Delete(QoUserFamilyDeleteApiArgs args)
        {
            var results = new QoUserFamilyDeleteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var parentAccountKey))
            {
                return results;
            }

            // ターゲットアカウントキー変換
            if (!args.AccountKeyReference.ToDecrypedReference().CheckArgsConvert(nameof(args.AccountKeyReference), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 親アカウント妥当性チェック
            if (!CheckParentAccount(parentAccountKey, results))
            {
                return results;
            }

            // 自身を削除しようとしている場合はエラー
            if (parentAccountKey == accountKey)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "パブリックアカウントは削除できません。");
                return results;
            }

            // 親子関係チェック
            if (!TryCheckRelation(parentAccountKey, accountKey, results))
            {
                return results;
            }

            // 削除前に写真キーを取得しておく
            if(!TryGetPhotoKey(accountKey, results, out var photoKey))
            {
                return results;
            }

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // 削除実行
            if(!TryDeleteFamilyAccount(parentAccountKey, accountKey, authorKey, results))
            {
                return results;
            }

            // 画像が設定されていた場合は画像削除
            if(photoKey != Guid.Empty)
            {
                // 成否は問わない
                TryDeleteFile(authorKey, photoKey);
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryGetPhotoKey(Guid accountKey, QoApiResultsBase results, out Guid photoKey)
        {
            photoKey = Guid.Empty;
            try
            {
                var entity = _accountRepo.ReadAccountIndexDat(accountKey);
                photoKey = entity.PHOTOKEY;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "画像情報取得処理に失敗しました。");
                return false;
            }
        }

        bool TryDeleteFamilyAccount(Guid parentAccountKey, Guid accountKey, Guid authorKey, QoApiResultsBase results)
        {
            try
            {
                _familyRepo.DeleteFamily(parentAccountKey, accountKey, authorKey);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "子アカウントの削除に失敗しました。");
                return false;
            }
        }

        void TryDeleteFile(Guid authorKey, Guid fileKey)
        {
            try
            {                
                _storageRepo.DeleteFile(authorKey, fileKey);
            }
            catch(Exception ex)
            {
                // ログに残すのみでエラー扱いにはしない
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    authorKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"ファイル削除エラー :例外: {ex.InnerException.Message} AuthorKey: {authorKey}"
                );
            }
        }
    }
}