using MGF.QOLMS.QolmsAzureStorageCoreV1;
namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
     internal sealed class UploadEditedFileBlobEntityReaderResults<TEntity> : QsAzureBlobStorageReaderResultsBase<TEntity> where TEntity : QsUploadFileBlobEntityBase
    {
        /// <summary>
        /// <see cref="UploadEditedFileBlobEntityReaderResults &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadEditedFileBlobEntityReaderResults() : base()
        {
        }
    }


}