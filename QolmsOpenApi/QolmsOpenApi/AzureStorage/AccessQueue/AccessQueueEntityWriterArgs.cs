using MGF.QOLMS.QolmsAzureStorageCoreV1;

/// <summary>
/// アクセス ログ キュー の末尾に、
/// メッセージ を登録するための情報を格納する引数 クラス を表します。
/// この クラス は継承できません。
/// </summary>
/// <remarks></remarks>
internal sealed class AccessQueueEntityWriterArgs : QsAzureQueueStorageWriterArgsBase<QoAccessQueueEntity, QoAccessQueueMessage>
{
    /// <summary>
    /// <see cref="AccessQueueEntityWriterArgs" /> クラス の新しい インスタンス を初期化します。
    /// </summary>
    /// <remarks></remarks>
    public AccessQueueEntityWriterArgs() : base()
    {
    }
}

