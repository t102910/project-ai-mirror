
'''<remarks/>
<System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.kddi.com/cocoa"), _
 System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://www.kddi.com/cocoa", IsNullable:=False)> _
Public Class biscuitif

    Private sidField As String

    Private fidField As String

    Private resultStatusField As String

    Private auIdAttribField As biscuitifAuIdAttrib

    Private csAttribField As biscuitifCsAttrib

    Private auCntrctAttribField() As biscuitifAuCntrctAttrib

    ''' <summary>
    ''' フィルタリングサービスID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property sid() As String
        Get
            Return Me.sidField
        End Get
        Set(value As String)
            Me.sidField = value
        End Set
    End Property

    ''' <summary>
    ''' 機能ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property fid() As String
        Get
            Return Me.fidField
        End Get
        Set(value As String)
            Me.fidField = value
        End Set
    End Property

    ''' <summary>
    ''' 処理結果
    ''' </summary>
    ''' <value></value>
    ''' <returns>0：成功、3：異常、6：メンテナンス中</returns>
    ''' <remarks></remarks>
    Public Property resultStatus() As String
        Get
            Return Me.resultStatusField
        End Get
        Set(value As String)
            Me.resultStatusField = value
        End Set
    End Property

    ''' <summary>
    ''' AuID属性(親要素)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property auIdAttrib() As biscuitifAuIdAttrib
        Get
            Return Me.auIdAttribField
        End Get
        Set(value As biscuitifAuIdAttrib)
            Me.auIdAttribField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客属性(親要素)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property csAttrib() As biscuitifCsAttrib
        Get
            Return Me.csAttribField
        End Get
        Set(value As biscuitifCsAttrib)
            Me.csAttribField = value
        End Set
    End Property

    ''' <summary>
    ''' Au契約属性(親要素)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Xml.Serialization.XmlElementAttribute("auCntrctAttrib")> _
    Public Property auCntrctAttrib() As biscuitifAuCntrctAttrib()
        Get
            Return Me.auCntrctAttribField
        End Get
        Set(value As biscuitifAuCntrctAttrib())
            Me.auCntrctAttribField = value
        End Set
    End Property
End Class

''' <summary>
''' AuID属性
''' </summary>
''' <remarks></remarks>
<System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.kddi.com/cocoa")> _
Public Class biscuitifAuIdAttrib
    Private auIdField As String

    Private idIdntKbnField As String

    Private idIdntKbnRegstDtimeField As String

    Private idIdntKbnChgDtimeField As String

    Private auIdLinkField As String

    ''' <summary>
    ''' AUIDシステムID。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property auId() As String
        Get
            Return Me.auidField
        End Get
        Set(value As String)
            Me.auidField = value
        End Set
    End Property
    ''' <summary>
    ''' ID識別区分。au ID か、Wow!IDか。
    ''' </summary>
    ''' <value></value>
    ''' <returns>0：Au、1:Wow</returns>
    ''' <remarks></remarks>
    Public Property idIdntKbn() As String
        Get
            Return Me.idIdntKbnField
        End Get
        Set(value As String)
            Me.idIdntKbnField = value
        End Set
    End Property

    ''' <summary>
    ''' 登録日。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property idIdntKbnRegstDtime() As String
        Get
            Return Me.idIdntKbnRegstDtimeField
        End Get
        Set(value As String)
            Me.idIdntKbnRegstDtimeField = value
        End Set
    End Property

    ''' <summary>
    ''' 区分変更日。これが入る場合はAu⇔Wowでの区分変更があった人だとわかる。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property idIdntKbnChgDtime() As String
        Get
            Return Me.idIdntKbnChgDtimeField
        End Get
        Set(value As String)
            Me.idIdntKbnChgDtimeField = value
        End Set
    End Property

    ''' <summary>
    ''' 携帯と連動しているIDか
    ''' </summary>
    ''' <value></value>
    ''' <returns>0:連動なし、1:連動あり</returns>
    ''' <remarks>連動ありかつ加入者コードがある場合に、AUの携帯契約者だと判別できる</remarks>
    Public Property auIdLink() As String
        Get
            Return Me.auIdLinkField
        End Get
        Set(value As String)
            Me.auIdLinkField = value
        End Set
    End Property
End Class

''' <summary>
''' 顧客属性
''' </summary>
''' <remarks></remarks>
<System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.kddi.com/cocoa")> _
Public Class biscuitifCsAttrib

    Private rplstvBillToCntrctCdField As String

    Private rplstvBillToKbnField As String

    Private nameKanjiField As String

    Private nameKanaField As String

    Private nameKanjiFamilyField As String

    Private nameKanjiFirstField As String

    Private nameKanaFamilyField As String

    Private nameKanaFirstField As String

    Private csTypeField As String

    Private birthdayField As String

    Private sexField As String

    Private zipField As String

    Private addrField As String

    Private eMail1Field As String

    Private eMail1SendFlgField As String

    Private eMail2Field As String

    Private eMail2SendFlgField As String

    Private ezMailField As String

    Private ezMailSendFlgField As String

    ''' <summary>
    ''' 代表契約コード
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>auかんたん決済にて通信料合算でのお支払い方法を選択した際に、合算請求先となる通信サービスのコードです</remarks>
    Public Property rplstvBillToCntrctCd() As String
        Get
            Return Me.rplstvBillToCntrctCdField
        End Get
        Set(value As String)
            Me.rplstvBillToCntrctCdField = value
        End Set
    End Property

    ''' <summary>
    ''' 代表契約区分
    ''' </summary>
    ''' <value></value>
    ''' <returns>1:Au、2:BBC</returns>
    ''' <remarks></remarks>
    Public Property rplstvBillToKbn() As String
        Get
            Return Me.rplstvBillToKbnField
        End Get
        Set(value As String)
            Me.rplstvBillToKbnField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客の氏名(漢字)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>姓と名が一緒になっている。分けられない・・・</remarks>
    Public Property nameKanji() As String
        Get
            Return Me.nameKanjiField
        End Get
        Set(value As String)
            Me.nameKanjiField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客の氏名(カナ)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>姓と名が一緒になっている。</remarks>
    Public Property nameKana() As String
        Get
            Return Me.nameKanaField
        End Get
        Set(value As String)
            Me.nameKanaField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客の名字(漢字)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property nameKanjiFamily() As String
        Get
            Return Me.nameKanjiFamilyField
        End Get
        Set(value As String)
            Me.nameKanjiFamilyField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客の名前(漢字)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property nameKanjiFirst() As String
        Get
            Return Me.nameKanjiFirstField
        End Get
        Set(value As String)
            Me.nameKanjiFirstField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客の名字(カナ)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property nameKanaFamily() As String
        Get
            Return Me.nameKanaFamilyField
        End Get
        Set(value As String)
            Me.nameKanaFamilyField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客の名前(カナ)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property nameKanaFirst() As String
        Get
            Return Me.nameKanaFirstField
        End Get
        Set(value As String)
            Me.nameKanaFirstField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客区分
    ''' </summary>
    ''' <value></value>
    ''' <returns>1:個人、2:法人、3:業務用、4:マンションオーナー/管理組合</returns>
    ''' <remarks></remarks>
    Public Property csType() As String
        Get
            Return Me.csTypeField
        End Get
        Set(value As String)
            Me.csTypeField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客生年月日
    ''' </summary>
    ''' <value></value>
    ''' <returns>YYYYMMDD形式</returns>
    ''' <remarks></remarks>
    Public Property birthday() As String
        Get
            Return Me.birthdayField
        End Get
        Set(value As String)
            Me.birthdayField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客性別
    ''' </summary>
    ''' <value></value>
    ''' <returns>1:男、2:女、未設定はブランク</returns>
    ''' <remarks></remarks>
    Public Property sex() As String
        Get
            Return Me.sexField
        End Get
        Set(value As String)
            Me.sexField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客連絡先郵便番号
    ''' </summary>
    ''' <value></value>
    ''' <returns>半角数字</returns>
    ''' <remarks></remarks>
    Public Property zip() As String
        Get
            Return Me.zipField
        End Get
        Set(value As String)
            Me.zipField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客住所
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property addr() As String
        Get
            Return Me.addrField
        End Get
        Set(value As String)
            Me.addrField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客Eメールアドレス１
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property eMail1() As String
        Get
            Return Me.eMail1Field
        End Get
        Set(value As String)
            Me.eMail1Field = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客Eメールアドレス1に対して送信可否
    ''' </summary>
    ''' <value></value>
    ''' <returns>0:否、1:要、アドレスが設定されてない場合はNULL</returns>
    ''' <remarks></remarks>
    Public Property eMail1SendFlg() As String
        Get
            Return Me.eMail1SendFlgField
        End Get
        Set(value As String)
            Me.eMail1SendFlgField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客Eメールアドレス２
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property eMail2() As String
        Get
            Return Me.eMail2Field
        End Get
        Set(value As String)
            Me.eMail2Field = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客Eメールアドレス２に対して送信可否
    ''' </summary>
    ''' <value></value>
    ''' <returns>0:否、1:要、アドレスが設定されてない場合はNULL</returns>
    ''' <remarks></remarks>
    Public Property eMail2SendFlg() As String
        Get
            Return Me.eMail2SendFlgField
        End Get
        Set(value As String)
            Me.eMail2SendFlgField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客EZメールアドレス
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ezMail() As String
        Get
            Return Me.ezMailField
        End Get
        Set(value As String)
            Me.ezMailField = value
        End Set
    End Property

    ''' <summary>
    ''' 顧客EZメールアドレスに対して送信可否
    ''' </summary>
    ''' <value></value>
    ''' <returns>0:否、1:要、アドレスが設定されてない場合はNULL</returns>
    ''' <remarks></remarks>
    Public Property ezMailSendFlg() As String
        Get
            Return Me.ezMailSendFlgField
        End Get
        Set(value As String)
            Me.ezMailSendFlgField = value
        End Set
    End Property
End Class

''' <summary>
''' au契約属性
''' </summary>
''' <remarks></remarks>
<System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.kddi.com/cocoa")> _
Public Class biscuitifAuCntrctAttrib

    Private subscrCdField As String

    Private mobileTelField As String

    Private userFeeOptField As String

    Private dualSingleKbnField As String

    Private auKaiyakuDayField As String

    ''' <summary>
    ''' 加入者コード
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property subscrCd() As String
        Get
            Return Me.subscrCdField
        End Get
        Set(value As String)
            Me.subscrCdField = value
        End Set
    End Property

    ''' <summary>
    ''' au電話番号
    ''' </summary>
    ''' <value></value>
    ''' <returns>半角数字</returns>
    ''' <remarks></remarks>
    Public Property mobileTel() As String
        Get
            Return Me.mobileTelField
        End Get
        Set(value As String)
            Me.mobileTelField = value
        End Set
    End Property

    ''' <summary>
    ''' 料金オプション
    ''' </summary>
    ''' <value></value>
    ''' <returns>サービスコード(4バイト)×20オカレンス</returns>
    ''' <remarks>設定なしの場合は空白80バイト</remarks>
    Public Property userFeeOpt() As String
        Get
            Return Me.userFeeOptField
        End Get
        Set(value As String)
            Me.userFeeOptField = value
        End Set
    End Property

    ''' <summary>
    ''' デュアルシングル区分
    ''' </summary>
    ''' <value></value>
    ''' <returns>0：デュアル、1:シングル</returns>
    ''' <remarks></remarks>
    Public Property dualSingleKbn() As String
        Get
            Return Me.dualSingleKbnField
        End Get
        Set(value As String)
            Me.dualSingleKbnField = value
        End Set
    End Property

    ''' <summary>
    ''' Au解約日
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property auKaiyakuDay() As String
        Get
            Return Me.auKaiyakuDayField
        End Get
        Set(value As String)
            Me.auKaiyakuDayField = value
        End Set
    End Property

End Class
