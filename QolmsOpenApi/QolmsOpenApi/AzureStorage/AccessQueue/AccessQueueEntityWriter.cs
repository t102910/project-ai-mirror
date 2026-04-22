using MGF.QOLMS.QolmsAzureStorageCoreV1;

/// <summary>
/// アクセス ログ キュー の末尾に、
/// メッセージ を登録するための機能を提供します。
/// この クラス は継承できません。
/// </summary>
/// <remarks></remarks>
internal sealed class AccessQueueEntityWriter : QsAzureQueueStorageWriterBase<QoAccessQueueEntity, QoAccessQueueMessage>, IQsAzureQueueStorageWriter<QoAccessQueueEntity, QoAccessQueueMessage, AccessQueueEntityWriterArgs, AccessQueueEntityWriterResults>
{
    /// <summary>
    /// <see cref="AccessQueueEntityWriter" /> クラス の新しい インスタンス を初期化します。
    /// </summary>
    /// <remarks></remarks>
    public AccessQueueEntityWriter() : base(true)
    {
    }
    /// <summary>
    /// Azure キュー ストレージへ 値を設定します。
    /// </summary>
    /// <param name="args">引数 クラス。</param>
    /// <returns>
    /// 戻り値 クラス。
    /// </returns>
    /// <remarks></remarks>
    public AccessQueueEntityWriterResults Execute(AccessQueueEntityWriterArgs args)
    {
        // キュー の末尾に メッセージ 登録
        return new AccessQueueEntityWriterResults()
        {
            IsSuccess = this.Enqueue(args.Entity)
        };
    }
}

