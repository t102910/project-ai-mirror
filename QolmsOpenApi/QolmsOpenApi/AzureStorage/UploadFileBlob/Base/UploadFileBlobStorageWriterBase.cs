using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// とりあえずOpenApi側に作成（これでいいかなとは思うのでAzureStorageCoreV1へ持っていきたい）
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TArgs"></typeparam>
    /// <typeparam name="TResults"></typeparam>
    /// <remarks></remarks>
    public abstract class UploadFileBlobStorageWriterBase<TEntity, TArgs, TResults> : QsAzureBlobStorageWriterBase<TEntity>, IQsAzureBlobStorageWriter<TEntity, TArgs, TResults>
        where TEntity : QsUploadFileBlobEntityBase, new()
        where TArgs : QsAzureBlobStorageWriterArgsBase<TEntity>
        where TResults : QsAzureBlobStorageWriterResultsBase
    {


        /// <summary>
        /// <see cref="UploadFileBlobStorageWriterBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadFileBlobStorageWriterBase() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        public abstract TResults Execute(TArgs args);
    }


}