using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{

    /// <summary>
    /// アップロード された ファイル の サムネイル 情報の メタデータ を、
    /// ブロブ ストレージ から取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class UploadThumbnailFileBlobMetadataReaderArgs : QsAzureBlobStorageMetadataReaderArgsBase
    {
        /// <summary>
        /// <see cref="UploadThumbnailFileBlobMetadataReaderArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadThumbnailFileBlobMetadataReaderArgs() : base()
        {
        }
    }


}