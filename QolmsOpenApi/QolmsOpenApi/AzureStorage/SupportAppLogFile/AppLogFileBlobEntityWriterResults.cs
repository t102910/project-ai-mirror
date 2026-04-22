using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
  
    /// <summary>
    /// アプリログ ファイル データ を、
    /// ブロブ ストレージ へ登録した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AppLogFileBlobEntityWriterResults : QsAzureBlobStorageWriterResultsBase
    {
        /// <summary>
        /// <see cref="AppLogFileBlobEntityWriterResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AppLogFileBlobEntityWriterResults() : base()
        {
        }
    }


}