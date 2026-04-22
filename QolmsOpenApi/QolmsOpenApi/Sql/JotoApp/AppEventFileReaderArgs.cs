using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// アプリイベントファイル読み込みパラメータ
    /// </summary>
    internal sealed class AppEventFileReaderArgs : QsDbReaderArgsBase<QH_APPEVENTFILE_DAT>
    {
        /// <summary>
        /// イベントキー
        /// </summary>
        public Guid EventKey { get; set; } = Guid.Empty;

        /// <summary>
        /// ファイルキー
        /// </summary>
        public Guid FileKey { get; set; } = Guid.Empty;
    }
}
