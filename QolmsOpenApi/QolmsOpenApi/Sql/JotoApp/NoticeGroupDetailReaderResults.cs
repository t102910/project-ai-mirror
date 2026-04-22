using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// お知らせ（グループ）詳細情報の読み込み結果
    /// </summary>
    internal sealed class NoticeGroupDetailReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        #region "Private Property"
        #endregion

        #region "Public Property"

        /// <summary>
        /// お知らせ（グループ）詳細情報
        /// </summary>
        public DbNoticeGroupItem NoticeGroupItem { get; set; } = null;
        #endregion

        #region "Constructor"
        #endregion

        #region "Private Method"
        #endregion

        #region "Public Method"
        #endregion
    }
}
