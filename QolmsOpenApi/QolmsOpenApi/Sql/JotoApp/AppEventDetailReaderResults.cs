using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// アプリイベント詳細読み込み結果
    /// </summary>
    internal sealed class AppEventDetailReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// アプリイベント詳細
        /// </summary>
        public DbAppEventItem AppEventItem { get; set; } = null;
    }
}
