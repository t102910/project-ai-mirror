using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// アプリイベント詳細読み込みパラメータ
    /// </summary>
    internal sealed class AppEventDetailReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// イベントキー
        /// </summary>
        public Guid EventKey { get; set; } = Guid.Empty;
    }
}
