using System;
using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// データ投稿（健診ファイル） データ を、
    /// ブロブ ストレージ から削除するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public class UploadCheckupPostingFileBlobEntityDeleter : QsAzureBlobStorageWriterBase<QmUploadCheckupPostingFileBlobEntity>, IQsAzureBlobStorageWriter<QmUploadCheckupPostingFileBlobEntity, UploadCheckupPostingFileBlobEntityDeleterArgs, UploadCheckupPostingFileBlobEntityDeleterResults>
    {
        /// <summary>
        /// <see cref="UploadCheckupPostingFileBlobEntityDeleter" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadCheckupPostingFileBlobEntityDeleter() : base(true, BlobContainerPublicAccessType.Off)
        {
        }

        /// <summary>
        /// Azure ブロブ ストレージ から値を削除します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        /// <remarks></remarks>
        public UploadCheckupPostingFileBlobEntityDeleterResults Execute(UploadCheckupPostingFileBlobEntityDeleterArgs args)
        {
            var result = new UploadCheckupPostingFileBlobEntityDeleterResults()
            {
                IsSuccess = false,
                Result = Guid.Empty
            };

            result.IsSuccess = base.Delete(args.Entity.Name);

            return result;
        }
    }
}