using System.Xml.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 顧客属性
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://www.kddi.com/cocoa")]
    public class biscuitifCsAttrib
    {
        private string rplstvBillToCntrctCdField;
        private string rplstvBillToKbnField;
        private string nameKanjiField;
        private string nameKanaField;
        private string nameKanjiFamilyField;
        private string nameKanjiFirstField;
        private string nameKanaFamilyField;
        private string nameKanaFirstField;
        private string csTypeField;
        private string birthdayField;
        private string sexField;
        private string zipField;
        private string addrField;
        private string eMail1Field;
        private string eMail1SendFlgField;
        private string eMail2Field;
        private string eMail2SendFlgField;
        private string ezMailField;
        private string ezMailSendFlgField;

        /// <summary>
        /// 代表契約コード
        /// </summary>
        /// <remarks>auかんたん決済にて通信料合算でのお支払い方法を選択した際に、合算請求先となる通信サービスのコードです</remarks>
        public string rplstvBillToCntrctCd
        {
            get { return this.rplstvBillToCntrctCdField; }
            set { this.rplstvBillToCntrctCdField = value; }
        }

        /// <summary>
        /// 代表契約区分
        /// </summary>
        /// <returns>1:Au、2:BBC</returns>
        public string rplstvBillToKbn
        {
            get { return this.rplstvBillToKbnField; }
            set { this.rplstvBillToKbnField = value; }
        }

        /// <summary>
        /// 顧客の氏名(漢字)
        /// </summary>
        /// <remarks>姓と名が一緒になっている。分けられない・・・</remarks>
        public string nameKanji
        {
            get { return this.nameKanjiField; }
            set { this.nameKanjiField = value; }
        }

        /// <summary>
        /// 顧客の氏名(カナ)
        /// </summary>
        /// <remarks>姓と名が一緒になっている。</remarks>
        public string nameKana
        {
            get { return this.nameKanaField; }
            set { this.nameKanaField = value; }
        }

        /// <summary>
        /// 顧客の名字(漢字)
        /// </summary>
        public string nameKanjiFamily
        {
            get { return this.nameKanjiFamilyField; }
            set { this.nameKanjiFamilyField = value; }
        }

        /// <summary>
        /// 顧客の名前(漢字)
        /// </summary>
        public string nameKanjiFirst
        {
            get { return this.nameKanjiFirstField; }
            set { this.nameKanjiFirstField = value; }
        }

        /// <summary>
        /// 顧客の名字(カナ)
        /// </summary>
        public string nameKanaFamily
        {
            get { return this.nameKanaFamilyField; }
            set { this.nameKanaFamilyField = value; }
        }

        /// <summary>
        /// 顧客の名前(カナ)
        /// </summary>
        public string nameKanaFirst
        {
            get { return this.nameKanaFirstField; }
            set { this.nameKanaFirstField = value; }
        }

        /// <summary>
        /// 顧客区分
        /// </summary>
        /// <returns>1:個人、2:法人、3:業務用、4:マンションオーナー/管理組合</returns>
        public string csType
        {
            get { return this.csTypeField; }
            set { this.csTypeField = value; }
        }

        /// <summary>
        /// 顧客生年月日
        /// </summary>
        /// <returns>YYYYMMDD形式</returns>
        public string birthday
        {
            get { return this.birthdayField; }
            set { this.birthdayField = value; }
        }

        /// <summary>
        /// 顧客性別
        /// </summary>
        /// <returns>1:男、2:女、未設定はブランク</returns>
        public string sex
        {
            get { return this.sexField; }
            set { this.sexField = value; }
        }

        /// <summary>
        /// 顧客連絡先郵便番号
        /// </summary>
        /// <returns>半角数字</returns>
        public string zip
        {
            get { return this.zipField; }
            set { this.zipField = value; }
        }

        /// <summary>
        /// 顧客住所
        /// </summary>
        public string addr
        {
            get { return this.addrField; }
            set { this.addrField = value; }
        }

        /// <summary>
        /// 顧客Eメールアドレス１
        /// </summary>
        public string eMail1
        {
            get { return this.eMail1Field; }
            set { this.eMail1Field = value; }
        }

        /// <summary>
        /// 顧客Eメールアドレス1に対して送信可否
        /// </summary>
        /// <returns>0:否、1:要、アドレスが設定されてない場合はNULL</returns>
        public string eMail1SendFlg
        {
            get { return this.eMail1SendFlgField; }
            set { this.eMail1SendFlgField = value; }
        }

        /// <summary>
        /// 顧客Eメールアドレス２
        /// </summary>
        public string eMail2
        {
            get { return this.eMail2Field; }
            set { this.eMail2Field = value; }
        }

        /// <summary>
        /// 顧客Eメールアドレス２に対して送信可否
        /// </summary>
        /// <returns>0:否、1:要、アドレスが設定されてない場合はNULL</returns>
        public string eMail2SendFlg
        {
            get { return this.eMail2SendFlgField; }
            set { this.eMail2SendFlgField = value; }
        }

        /// <summary>
        /// 顧客EZメールアドレス
        /// </summary>
        public string ezMail
        {
            get { return this.ezMailField; }
            set { this.ezMailField = value; }
        }

        /// <summary>
        /// 顧客EZメールアドレスに対して送信可否
        /// </summary>
        /// <returns>0:否、1:要、アドレスが設定されてない場合はNULL</returns>
        public string ezMailSendFlg
        {
            get { return this.ezMailSendFlgField; }
            set { this.ezMailSendFlgField = value; }
        }
    }
}