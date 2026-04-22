using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// アプリイベント一覧読み込み結果
    /// </summary>
    internal sealed class AppEventReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// アプリイベント一覧
        /// </summary>
        public List<DbAppEventItem> AppEventItemN { get; set; } = new List<DbAppEventItem>();
    }
}
