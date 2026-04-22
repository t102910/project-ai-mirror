using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// データ投稿（健診ファイル） データ を、
    /// ブロブ ストレージ から削除した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class UploadCheckupPostingFileBlobEntityDeleterResults : QsAzureBlobStorageWriterResultsBase
    {
        /// <summary>
        /// <see cref="UploadCheckupPostingFileBlobEntityDeleterResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadCheckupPostingFileBlobEntityDeleterResults() : base()
        {
        }
    }
}