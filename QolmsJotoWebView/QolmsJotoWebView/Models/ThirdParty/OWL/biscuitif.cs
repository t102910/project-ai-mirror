using System.Xml.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// biscuitif ルートクラス
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://www.kddi.com/cocoa")]
    [XmlRoot(Namespace = "http://www.kddi.com/cocoa", IsNullable = false)]
    public class biscuitif
    {
        private string sidField;
        private string fidField;
        private string resultStatusField;
        private biscuitifAuIdAttrib auIdAttribField;
        private biscuitifCsAttrib csAttribField;
        private biscuitifAuCntrctAttrib[] auCntrctAttribField;

        /// <summary>
        /// フィルタリングサービスID
        /// </summary>
        public string sid
        {
            get { return this.sidField; }
            set { this.sidField = value; }
        }

        /// <summary>
        /// 機能ID
        /// </summary>
        public string fid
        {
            get { return this.fidField; }
            set { this.fidField = value; }
        }

        /// <summary>
        /// 処理結果
        /// </summary>
        /// <returns>0：成功、3：異常、6：メンテナンス中</returns>
        public string resultStatus
        {
            get { return this.resultStatusField; }
            set { this.resultStatusField = value; }
        }

        /// <summary>
        /// AuID属性(親要素)
        /// </summary>
        public biscuitifAuIdAttrib auIdAttrib
        {
            get { return this.auIdAttribField; }
            set { this.auIdAttribField = value; }
        }

        /// <summary>
        /// 顧客属性(親要素)
        /// </summary>
        public biscuitifCsAttrib csAttrib
        {
            get { return this.csAttribField; }
            set { this.csAttribField = value; }
        }

        /// <summary>
        /// Au契約属性(親要素)
        /// </summary>
        [XmlElement("auCntrctAttrib")]
        public biscuitifAuCntrctAttrib[] auCntrctAttrib
        {
            get { return this.auCntrctAttribField; }
            set { this.auCntrctAttribField = value; }
        }
    }

}