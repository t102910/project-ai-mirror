using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アクセス ログ テーブル ストレージ へ値を登録した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class AccessTableEntityWriterResults
        : QsAzureTableStorageWriterResultsBase
    {

        #region "Constructor"

        /// <summary>
        /// <see cref="AccessTableEntityWriterResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AccessTableEntityWriterResults() : base() { }

        #endregion

    }
}