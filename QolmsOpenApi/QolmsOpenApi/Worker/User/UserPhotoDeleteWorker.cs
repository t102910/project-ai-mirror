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
    /// ユーザー画像削除処理
    /// </summary>
    public class UserPhotoDeleteWorker: UserWorkerBase
    {
        IStorageRepository _storageRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="storageRepository"></param>
        public UserPhotoDeleteWorker(IAccountRepository accountRepository, IFamilyRepository familyRepository, IStorageRepository storageRepository) : base(accountRepository, familyRepository)
        {
            _storageRepo = storageRepository;
        }

        /// <summary>
        /// 削除処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserPhotoDeleteApiResults Delete(QoUserPhotoDeleteApiArgs args)
        {
            var results = new QoUserPhotoDeleteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 親アカウントキー変換
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var parentAccountKey))
            {
                return results;
            }

            Guid targetAccountKey;
            // ターゲットが指定されていた場合
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
                // 未指定の場合は親アカウントを対象とする
                targetAccountKey = parentAccountKey;
            }

            // 自身のアカウントが対象でなければ
            if (parentAccountKey != targetAccountKey)
            {
                // 親子関係チェック
                if (!TryCheckRelation(parentAccountKey, targetAccountKey, results))
                {
                    return results;
                }
            }

            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);

            // アカウント情報取得
            if (!TryReadAccountIndex(targetAccountKey, results, out var entity))
            {
                return results;
            }            

            // ファイル削除
            // 削除に失敗してもエラーとしない
            DeleteStorage(authorKey, entity.PHOTOKEY);

            // ファイルキーを空に更新
            entity.PHOTOKEY = Guid.Empty;

            // アカウント情報更新
            if (!TryWriteAccoutIndex(entity, results))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        void DeleteStorage(Guid authorKey, Guid fileKey)
        {
            try
            {
                _storageRepo.DeleteFile(authorKey, fileKey);
            }
            catch (Exception ex)
            {
                // エラーとしては返さずログに残すのみとする
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    authorKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"Blobファイル削除エラー:例外: {ex.InnerException?.Message ?? string.Empty}"
                );
            }
        }
    }
}