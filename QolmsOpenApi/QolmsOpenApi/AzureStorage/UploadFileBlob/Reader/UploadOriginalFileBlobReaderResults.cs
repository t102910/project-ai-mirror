using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    internal sealed class UploadOriginalFileBlobEntityReaderResults<TEntity> : QsAzureBlobStorageReaderResultsBase<TEntity> where TEntity : QsUploadFileBlobEntityBase
    {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadOriginalFileBlobEntityReaderResults() : base()
        {
        }
    }
}