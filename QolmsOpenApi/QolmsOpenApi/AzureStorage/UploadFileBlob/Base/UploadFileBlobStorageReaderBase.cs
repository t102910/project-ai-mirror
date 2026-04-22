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
    public abstract class UploadFileBlobStorageReaderBase<TEntity, TArgs, TResults> : QsAzureBlobStorageReaderBase<TEntity>, IQsAzureBlobStorageReader<TEntity, TArgs, TResults>
        where TEntity : QsUploadFileBlobEntityBase, new()
        where TArgs : QsAzureBlobStorageReaderArgsBase
        where TResults : QsAzureBlobStorageReaderResultsBase<TEntity>
    {


        /// <summary>
        /// ブロブ コンテナを作成するかを指定して、
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="createIfNotExists">
        /// ブロブ コンテナが存在しない場合に作成するなら True、
        /// 作成しないなら False を指定。
        /// </param>
        /// <param name="publicAccessType">作成したブロブ コンテナに設定するパブリック アクセス レベル。</param>
        /// <remarks></remarks>
        protected UploadFileBlobStorageReaderBase(bool createIfNotExists, BlobContainerPublicAccessType publicAccessType) : base(createIfNotExists, publicAccessType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract TResults Execute(TArgs args);
    }


}