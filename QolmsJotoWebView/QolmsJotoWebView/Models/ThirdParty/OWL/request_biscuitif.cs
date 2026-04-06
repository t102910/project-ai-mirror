using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.OwlRequest
{
    /// <remarks/>
    [System.Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.kddi.com/cocoa")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://www.kddi.com/cocoa", IsNullable = false)]
    public partial class biscuitif
    {
        private string sidField;
        private string fidField;
        private string utypeField;
        private string uidtfField;

        /// <remarks/>
        public string sid
        {
            get { return this.sidField; }
            set { this.sidField = value; }
        }

        /// <remarks/>
        public string fid
        {
            get { return this.fidField; }
            set { this.fidField = value; }
        }

        /// <remarks/>
        public string utype
        {
            get { return this.utypeField; }
            set { this.utypeField = value; }
        }

        /// <remarks/>
        public string uidtf
        {
            get { return this.uidtfField; }
            set { this.uidtfField = value; }
        }
    }

}