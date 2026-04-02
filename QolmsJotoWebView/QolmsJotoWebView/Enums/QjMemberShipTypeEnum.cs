using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 沖縄セルラー会員種別を表します。
    /// </summary>
    public enum QjMemberShipTypeEnum : byte
    {

        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsDbMemberShipTypeEnum.None,

        /// <summary>
        /// 無料会員です。
        /// </summary>
        Free = QsDbMemberShipTypeEnum.Free,

        /// <summary>
        /// 期間限定プレミアム会員です。
        /// </summary>
        LimitedTime = QsDbMemberShipTypeEnum.LimitedTime,

        /// <summary>
        /// 有料プレミアム会員です。
        /// </summary>
        Premium = QsDbMemberShipTypeEnum.Premium,

        /// <summary>
        /// ビジネスプレミアム会員です。
        /// </summary>
        Business = QsDbMemberShipTypeEnum.Business,

        /// <summary>
        /// ビジネスプレミアム会員です。
        /// </summary>
        BusinessFree = QsDbMemberShipTypeEnum.BusinessFree,

        /// <summary>
        /// その他です。
        /// </summary>
        Other = QsDbMemberShipTypeEnum.Other
    }
}