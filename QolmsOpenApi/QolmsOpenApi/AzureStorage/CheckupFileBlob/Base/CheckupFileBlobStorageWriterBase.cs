using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// 健診系データ用
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TArgs"></typeparam>
    /// <typeparam name="TResults"></typeparam>
    /// <remarks></remarks>
    public abstract class CheckupFileBlobStorageWriterBase<TEntity, TArgs, TResults> : QsAzureBlobStorageWriterBase<TEntity>, IQsAzureBlobStorageWriter<TEntity, TArgs, TResults>
        where TEntity : QsCheckupFileBlobEntityBase, new()
        where TArgs : QsAzureBlobStorageWriterArgsBase<TEntity>
        where TResults : QsAzureBlobStorageWriterResultsBase
    {


        /// <summary>
        /// <see cref="CheckupFileBlobStorageWriterBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupFileBlobStorageWriterBase() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        public abstract TResults Execute(TArgs args);
    }


}