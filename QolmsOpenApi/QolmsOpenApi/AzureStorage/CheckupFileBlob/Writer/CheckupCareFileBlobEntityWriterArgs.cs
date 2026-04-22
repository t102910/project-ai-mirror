
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    internal sealed class CheckupCareFileBlobEntityWriterArgs<TEntity> : QsAzureBlobStorageWriterArgsBase<TEntity> where TEntity : QsAzureBlobStorageEntityBase
    {
        /// <summary>
        /// <see cref="CheckupCareFileBlobEntityWriterArgs &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupCareFileBlobEntityWriterArgs() : base()
        {
        }
    }


}