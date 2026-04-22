using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    
    internal sealed class UploadThumbnailFileBlobEntityWriterArgs<TEntity> : QsAzureBlobStorageWriterArgsBase<TEntity> where TEntity : QsUploadFileBlobEntityBase
    {

        /// <summary>
        /// <see cref="UploadThumbnailFileBlobEntityWriterArgs &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadThumbnailFileBlobEntityWriterArgs() : base()
        {
        }
    }


}