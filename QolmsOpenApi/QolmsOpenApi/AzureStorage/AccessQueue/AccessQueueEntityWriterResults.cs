using MGF.QOLMS.QolmsAzureStorageCoreV1;

/// <summary>
/// アクセス ログ キュー の末尾に、
/// メッセージ を登録した結果を格納する戻り値 クラス を表します。
/// この クラス は継承できません。
/// </summary>
/// <remarks></remarks>
internal sealed class AccessQueueEntityWriterResults : QsAzureQueueStorageWriterResultsBase
{
    /// <summary>
    /// <see cref="AccessQueueEntityWriterResults" /> クラス の新しい インスタンス を初期化します。
    /// </summary>
    /// <remarks></remarks>
    public AccessQueueEntityWriterResults() : base()
    {
    }
}
