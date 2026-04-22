using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// アプリイベント一覧読み込みパラメータ
    /// </summary>
    internal sealed class AppEventReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// ページ番号を取得または設定します。
        /// </summary>
        public int PageIndex { get; set; } = int.MinValue;

        /// <summary>
        /// ページサイズを取得または設定します。
        /// </summary>
        public int PageSize { get; set; } = int.MinValue;

        /// <summary>
        /// 対象者アカウントキーを取得または設定します。
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;
    }
}
