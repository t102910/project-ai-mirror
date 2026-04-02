''' <summary>
''' KDDIのAPESから取得できるユーザ情報のうち、新規登録に必要な情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class AuUserInf

#Region "Public Property"

    ''' <summary>
    ''' 漢字姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FamilyName As String = String.Empty

    ''' <summary>
    ''' 漢字名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GivenName As String = String.Empty

    ''' <summary>
    ''' カナ姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FamilyKanaName As String = String.Empty

    ''' <summary>
    ''' カナ名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GivenKanaName As String = String.Empty

    ''' <summary>
    ''' 性別（M|F）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sex As QySexTypeEnum = QySexTypeEnum.None

    ''' <summary>
    ''' 生年月日の年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BirthYear As String = String.Empty

    ''' <summary>
    ''' 生年月日の月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BirthMonth As String = String.Empty

    ''' <summary>
    ''' 生年月日の日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BirthDay As String = String.Empty

    ''' <summary>
    ''' メールアドレスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MailAddress As String = String.Empty

#End Region

#Region "Constructor"


#End Region

End Class
