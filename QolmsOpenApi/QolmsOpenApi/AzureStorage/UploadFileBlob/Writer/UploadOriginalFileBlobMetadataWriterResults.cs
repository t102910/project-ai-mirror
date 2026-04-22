using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{

    /// <summary>
    /// アップロード された ファイル の オリジナル 情報の メタデータ を、
    /// ブロブ ストレージ へ登録した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class UploadOriginalFileBlobMetadataWriterResults : QsAzureBlobStorageMetadataWriterResultsBase
    {


        /// <summary>
        /// <see cref="UploadOriginalFileBlobMetadataWriterResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadOriginalFileBlobMetadataWriterResults() : base()
        {
        }
    }


}