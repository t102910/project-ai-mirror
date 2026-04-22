using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    
    /// <summary>
    /// アップロード ファイル ブロブ へ メタデータ を設定するための基本 クラス を表します。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル ブロブ エンティティ クラス の型。</typeparam>
    /// <typeparam name="TArgs">Azure ブロブ ストレージ へ メタデータ を設定するための情報を格納する クラス の型。</typeparam>
    /// <typeparam name="TResults">Azure ブロブ ストレージ へ メタデータ を設定した結果を格納する戻り値 クラス の型。</typeparam>
    /// <remarks></remarks>
    internal abstract class UploadFileBlobStorageMetadataWriterBase<TEntity, TArgs, TResults> : QsAzureBlobStorageMetadataWriterBase<TEntity>, IQsAzureBlobStorageMetadataWriter<TEntity, TArgs, TResults>
        where TEntity : QsUploadFileBlobEntityBase, new()
        where TArgs : QsAzureBlobStorageMetadataWriterArgsBase<TEntity>
        where TResults : QsAzureBlobStorageMetadataWriterResultsBase
    {


        /// <summary>
        ///  インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadFileBlobStorageMetadataWriterBase() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ へ メタデータ を設定します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        /// <remarks></remarks>
        public abstract TResults Execute(TArgs args);
    }


}