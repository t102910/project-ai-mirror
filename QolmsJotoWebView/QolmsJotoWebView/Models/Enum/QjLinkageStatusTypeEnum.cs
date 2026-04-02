using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 連携のステータスの種別を表します。
    /// </summary>
    public enum QjLinkageStatusTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsDbLinkageStatusTypeEnum.None,

        /// <summary>
        /// 申請中です。
        /// </summary>
        Applying = QsDbLinkageStatusTypeEnum.Applying,

        /// <summary>
        /// 承認済みです。
        /// </summary>
        Approved = QsDbLinkageStatusTypeEnum.Approved,

        /// <summary>
        ///  承認不可（連係解除）です。
        /// </summary>
        Refused = QsDbLinkageStatusTypeEnum.Refused

    }
}