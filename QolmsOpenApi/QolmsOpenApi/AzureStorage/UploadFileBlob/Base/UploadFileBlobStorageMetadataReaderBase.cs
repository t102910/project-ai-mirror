using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// アップロード ファイル ブロブ の メタデータ を取得するための基本 クラス を表します。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル ブロブ エンティティ クラス の型。</typeparam>
    /// <typeparam name="TArgs">Azure ブロブ ストレージ の メタデータ を取得するための情報を格納する引数 クラス の型。</typeparam>
    /// <typeparam name="TResults">Azure ブロブ ストレージ の メタデータ を取得した結果を格納する戻り値 クラス の型。</typeparam>
    /// <remarks></remarks>
    internal abstract class UploadFileBlobStorageMetadataReaderBase<TEntity, TArgs, TResults> : QsAzureBlobStorageMetadataReaderBase<TEntity>, IQsAzureBlobStorageMetadataReader<TEntity, TArgs, TResults>
        where TEntity : QsUploadFileBlobEntityBase, new()
        where TArgs : QsAzureBlobStorageMetadataReaderArgsBase
        where TResults : QsAzureBlobStorageMetadataReaderResultsBase<TEntity>
    {


        /// <summary>
        /// <see cref="UploadFileBlobStorageMetadataReaderBase &lt; TEntity,TArgs,TResults &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadFileBlobStorageMetadataReaderBase() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ から メタデータ を取得します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        /// <remarks></remarks>
        public abstract TResults Execute(TArgs args);
    }


}