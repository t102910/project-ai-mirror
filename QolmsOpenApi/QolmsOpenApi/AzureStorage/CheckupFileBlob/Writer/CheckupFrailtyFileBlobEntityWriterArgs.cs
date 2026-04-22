
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    internal sealed class CheckupFrailtyFileBlobEntityWriterArgs<TEntity> : QsAzureBlobStorageWriterArgsBase<TEntity> where TEntity : QsAzureBlobStorageEntityBase
    {
        /// <summary>
        /// <see cref="CheckupFrailtyFileBlobEntityWriterArgs &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupFrailtyFileBlobEntityWriterArgs() : base()
        {
        }
    }


}