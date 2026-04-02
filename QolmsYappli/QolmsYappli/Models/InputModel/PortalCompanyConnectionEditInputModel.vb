Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute
Imports System.Text.RegularExpressions

Public NotInheritable Class PortalCompanyConnectionEditInputModel
    Implements  _
        IQyModelUpdater(Of PortalCompanyConnectionEditInputModel), 
        IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 漢字姓、
    ''' 漢字名、
    ''' カナ姓、
    ''' カナ名、
    ''' の入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_25 As Integer = 25

    ''' <summary>
    ''' 社員番号の最大長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_40 As Integer = 40

    ''' <summary>
    ''' メールアドレスの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_256 As Integer = 256

    ''' <summary>
    ''' IDとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const NUMBER_PATTERN As String = "^[0-9]*$"

    ''' <summary>
    ''' IDとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ID_PATTERN As String = "^[A-Za-z0-9]*$"

    ''' <summary>
    ''' カナとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const KANA_PATTERN As String = "^[\p{IsKatakana}\s]*$"

    ''' <summary>
    ''' メールアドレスの正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_PATTERN As String = "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z"

    ''' <summary>
    ''' 必須項目のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERRORMESSAGE As String = "{0}は必須項目です。"

    ''' <summary>
    ''' 文字列長のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_ERRORMESSAGE As String = "{0}は{1}文字以下で入力してください。"

    ''' <summary>
    ''' 正規表現のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REGULAREXPRESSION_ERRORMESSAGE As String = "{0}は{1}で入力してください。"

    ''' <summary>
    ''' その他のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const OTHERS_ERRORMESSAGE As String = "{0}は{1}で入力してください。"

    ''' <summary>
    ''' メールアドレスの形式エラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_ERRORMESSAGE As String = "有効なメールアドレス形式で入力してください。"

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ''' <summary>
    ''' 企業番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    ''' <summary>
    ''' 社員番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemId As String = String.Empty

    ''' <summary>
    ''' 企業名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemName As String = String.Empty


    ' ''' <summary>
    ' ''' 氏名　姓　を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("姓")>
    'Public Property FamilyName As String = String.Empty

    ' ''' <summary>
    ' ''' 氏名　名　を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("名")>
    'Public Property GivenName As String = String.Empty


    ' ''' <summary>
    ' ''' 性別を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("性別")>
    'Public Property SexType As QySexTypeEnum = QySexTypeEnum.None

    ' ''' <summary>
    ' ''' 生年月日の年を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("生年月日")>
    'Public Property BirthYear As String = String.Empty

    ' ''' <summary>
    ' ''' 生年月日の月を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("生年月日")>
    'Public Property BirthMonth As String = String.Empty

    ' ''' <summary>
    ' ''' 生年月日の日を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("生年月日")>
    'Public Property BirthDay As String = String.Empty

    ''' <summary>
    ''' 連携種別フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("開示許可")>
    Public Property RelationContentFlags As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None

    ''' <summary>
    ''' 企業連絡用メールアドレスを取得または設定します。
    ''' QH_EMPLYEE_DAT.MailAddressのみ変更
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("企業連絡用メールアドレス")>
    <EmailAddress(ErrorMessage:=MAILADDRESS_ERRORMESSAGE)>
    Public Property MailAddress As String = String.Empty

    ''' <summary>
    ''' 連携があるかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CompanyConnectedFlag As Boolean = False


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalCompanyConnectionEditInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' インプットモデルの内容を現在のインスタンスに反映します。
    ''' </summary>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(inputModel As PortalCompanyConnectionEditInputModel) Implements IQyModelUpdater(Of PortalCompanyConnectionEditInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            'With inputModel
            '    Me.ID = If(String.IsNullOrWhiteSpace(inputModel.ID), String.Empty, .ID.Trim())
            '    Me.Password = If(String.IsNullOrWhiteSpace(inputModel.Password), String.Empty, .Password.Trim())
            '    'Me.DataFalgs = inputModel.DataFalgs
            'End With
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    ''' <summary>
    ''' 指定されたオブジェクトが有効かどうかを判断します。
    ''' </summary>
    ''' <param name="validationContext">検証コンテキスト。</param>
    ''' <returns>
    ''' 失敗した検証の情報を保持するコレクション。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()

        Dim rxNumber As New Regex(NUMBER_PATTERN, RegexOptions.Compiled)
        Dim rxId As New Regex(ID_PATTERN, RegexOptions.Compiled)

        If String.IsNullOrWhiteSpace(Me.MailAddress) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "メールアドレス"), {"model.MailAddress"}))
        Else
            Dim mail As New EmailAddressAttribute()

            If Not mail.IsValid(Me.MailAddress) OrElse Not Regex.IsMatch(Me.MailAddress, MAILADDRESS_PATTERN, RegexOptions.IgnoreCase) Then
                result.Add(New ValidationResult(String.Format(MAILADDRESS_ERRORMESSAGE), {"model.MailAddress"}))
            End If

            If Me.MailAddress.Length > LENGTH_256 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "メールアドレス", LENGTH_256), {"model.MailAddress"}))
        End If


        Return result

    End Function

#End Region

End Class