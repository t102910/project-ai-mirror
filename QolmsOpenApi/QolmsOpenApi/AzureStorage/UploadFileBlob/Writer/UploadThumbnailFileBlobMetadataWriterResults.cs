using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
 
    /// <summary>
    /// アップロード された ファイル の サムネイル 情報の メタデータ を、
    /// ブロブ ストレージ へ登録した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class UploadThumbnailFileBlobMetadataWriterResults : QsAzureBlobStorageMetadataWriterResultsBase
    {


        /// <summary>
        /// <see cref="UploadThumbnailFileBlobMetadataWriterResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadThumbnailFileBlobMetadataWriterResults() : base()
        {
        }
    }


}