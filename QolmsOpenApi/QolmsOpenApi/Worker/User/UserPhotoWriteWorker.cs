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
    /// ユーザー画像更新処理
    /// </summary>
    public class UserPhotoWriteWorker : UserWorkerBase
    {
        IStorageRepository _storageRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="storageRepository"></param>
        public UserPhotoWriteWorker(IAccountRepository accountRepository, IFamilyRepository familyRepository, IStorageRepository storageRepository) : base(accountRepository, familyRepository)
        {
            _storageRepo = storageRepository;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserPhotoWriteApiResults Write(QoUserPhotoWriteApiArgs args)
        {
            var results = new QoUserPhotoWriteApiResults
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

            // 画像引数チェック
            if(!CheckPhotoArgs(args.Photo, results))
            {
                return results;
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

            // ファイルをStorageとDBに保存
            if (!TrySaveStorage(targetAccountKey, authorKey, args.Photo, results, out var fileKey)){
                return results;
            }

            // ファイルキー更新
            entity.PHOTOKEY = fileKey;

            // アカウント情報更新
            if(!TryWriteAccoutIndex(entity, results))
            {
                // 失敗時ファイルは削除
                DeleteStorage(authorKey, fileKey);
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.PhotoKeyReference = fileKey.ToEncrypedReference();

            return results;
        }

        bool CheckPhotoArgs(QoApiFileItem file, QoApiResultsBase results)
        {
            if (file == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "利用者画像情報が不正です");
                return false;
            }

            if (string.IsNullOrWhiteSpace(file.ContentType))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "利用者画像MIMEタイプが不正です");
                return false;
            }

            if (string.IsNullOrWhiteSpace(file.Data))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "利用者画像データが不正です");
                return false;
            }

            return true;
        }

        bool TrySaveStorage(Guid accountKey, Guid authorKey, QoApiFileItem file, QoApiResultsBase results, out Guid fileKey)
        {
            fileKey = Guid.Empty;
            try
            {
                fileKey = _storageRepo.WriteImage(accountKey, authorKey, file);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ファイル保存処理でエラーが発生しました。");
                return false;
            }
        }

        void DeleteStorage(Guid authorKey, Guid fileKey)
        {
            try
            {
                _storageRepo.DeleteFile(authorKey, fileKey);
            }
            catch(Exception ex)
            {
                // エラーとしては返さずログに残すのみとする
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    authorKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"Blobファイル削除エラー:例外: {ex.InnerException.Message}"
                );
            }
        }
    }
}