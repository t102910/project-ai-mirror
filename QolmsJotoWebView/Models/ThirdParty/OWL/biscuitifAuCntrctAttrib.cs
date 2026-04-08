using System.Xml.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// au契約属性
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://www.kddi.com/cocoa")]
    public class biscuitifAuCntrctAttrib
    {
        private string subscrCdField;
        private string mobileTelField;
        private string userFeeOptField;
        private string dualSingleKbnField;
        private string auKaiyakuDayField;

        /// <summary>
        /// 加入者コード
        /// </summary>
        public string subscrCd
        {
            get { return this.subscrCdField; }
            set { this.subscrCdField = value; }
        }

        /// <summary>
        /// au電話番号
        /// </summary>
        /// <returns>半角数字</returns>
        public string mobileTel
        {
            get { return this.mobileTelField; }
            set { this.mobileTelField = value; }
        }

        /// <summary>
        /// 料金オプション
        /// </summary>
        /// <returns>サービスコード(4バイト)×20オカレンス</returns>
        /// <remarks>設定なしの場合は空白80バイト</remarks>
        public string userFeeOpt
        {
            get { return this.userFeeOptField; }
            set { this.userFeeOptField = value; }
        }

        /// <summary>
        /// デュアルシングル区分
        /// </summary>
        /// <returns>0：デュアル、1:シングル</returns>
        public string dualSingleKbn
        {
            get { return this.dualSingleKbnField; }
            set { this.dualSingleKbnField = value; }
        }

        /// <summary>
        /// Au解約日
        /// </summary>
        public string auKaiyakuDay
        {
            get { return this.auKaiyakuDayField; }
            set { this.auKaiyakuDayField = value; }
        }
    }
}