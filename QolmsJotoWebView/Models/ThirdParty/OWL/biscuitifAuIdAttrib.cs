using System.Xml.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// AuID属性
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://www.kddi.com/cocoa")]
    public class biscuitifAuIdAttrib
    {
        private string auIdField;
        private string idIdntKbnField;
        private string idIdntKbnRegstDtimeField;
        private string idIdntKbnChgDtimeField;
        private string auIdLinkField;

        /// <summary>
        /// AUIDシステムID。
        /// </summary>
        public string auId
        {
            get { return this.auIdField; }
            set { this.auIdField = value; }
        }

        /// <summary>
        /// ID識別区分。au ID か、Wow!IDか。
        /// </summary>
        /// <returns>0：Au、1:Wow</returns>
        public string idIdntKbn
        {
            get { return this.idIdntKbnField; }
            set { this.idIdntKbnField = value; }
        }

        /// <summary>
        /// 登録日。
        /// </summary>
        public string idIdntKbnRegstDtime
        {
            get { return this.idIdntKbnRegstDtimeField; }
            set { this.idIdntKbnRegstDtimeField = value; }
        }

        /// <summary>
        /// 区分変更日。これが入る場合はAu⇔Wowでの区分変更があった人だとわかる。
        /// </summary>
        public string idIdntKbnChgDtime
        {
            get { return this.idIdntKbnChgDtimeField; }
            set { this.idIdntKbnChgDtimeField = value; }
        }

        /// <summary>
        /// 携帯と連動しているIDか
        /// </summary>
        /// <returns>0:連動なし、1:連動あり</returns>
        /// <remarks>連動ありかつ加入者コードがある場合に、AUの携帯契約者だと判別できる</remarks>
        public string auIdLink
        {
            get { return this.auIdLinkField; }
            set { this.auIdLinkField = value; }
        }
    }
}