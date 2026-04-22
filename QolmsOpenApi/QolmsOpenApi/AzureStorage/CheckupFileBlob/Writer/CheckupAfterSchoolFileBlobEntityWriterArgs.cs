
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    internal sealed class CheckupAfterSchoolFileBlobEntityWriterArgs<TEntity> : QsAzureBlobStorageWriterArgsBase<TEntity> where TEntity : QsAzureBlobStorageEntityBase
    {
        /// <summary>
        /// <see cref="CheckupAfterSchoolFileBlobEntityWriterArgs &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupAfterSchoolFileBlobEntityWriterArgs() : base()
        {
        }
    }


}