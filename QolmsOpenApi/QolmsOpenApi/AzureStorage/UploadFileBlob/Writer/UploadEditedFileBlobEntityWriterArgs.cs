
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    internal sealed class UploadEditedFileBlobEntityWriterArgs<TEntity> : QsAzureBlobStorageWriterArgsBase<TEntity> where TEntity : QsUploadFileBlobEntityBase
    {
        /// <summary>
        /// <see cref="UploadEditedFileBlobEntityWriterArgs &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadEditedFileBlobEntityWriterArgs() : base()
        {
        }
    }


}