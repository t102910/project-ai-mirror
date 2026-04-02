Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

<Serializable()>
Public NotInheritable Class PortalLocalIdVerificationRegisterInputModel
    Inherits QyHealthPageViewModelBase
    Implements IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 診察券番号の入力文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_8 As Integer = 8

    ''' <summary>
    ''' 診察券番号の入力文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_12 As Integer = 12

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
    ''' ユーザーIDの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_100 As Integer = 100

    ''' <summary>
    ''' メールアドレスの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_256 As Integer = 256

    ' ''' <summary>
    ' ''' パスワードの最小長を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_8 As Integer = 8

    ' ''' <summary>
    ' ''' パスワードの最大長を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_32 As Integer = 32

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
    ''' メールアドレスとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_PATTERN As String = "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z"
    'Private Const MAILADDRESS_PATTERN As String = "^[A-Za-z0-9]*$"

    ''' <summary>
    ''' 必須項目のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERRORMESSAGE As String = "{0}は必須項目です。"


    ''' <summary>
    ''' 文字列長のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ID_LENGTH_ERRORMESSAGE As String = "{0}は{1}文字で入力してください。{1}文字未満の場合は先頭を0で埋めて入力してください。"

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
    ''' 連携システム番号 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携システム ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemId As String = String.Empty

    ''' <summary>
    ''' 連携ステータス を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Status As Byte = Byte.MinValue

    ''' <summary>
    ''' 電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("電話番号")>
    Public Property PhoneNumber As String = String.Empty

    ''' <summary>
    ''' 生年月日の日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("メールアドレス")>
    Public Property MailAddress As String = String.Empty

    ''' <summary>
    ''' 個人情報の更新を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IdentityUpdateFlag As Boolean = False

    ''' <summary>
    ''' 連携種別フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("開示許可")>
    Public Property RelationContentFlags As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalLocalIdVerificationRegisterInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region "Public Method"


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

        '電話番号
        If String.IsNullOrWhiteSpace(Me.PhoneNumber) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "電話番号"), {"model.PhoneNumber"}))
        Else
            '数値＋桁数（10か11）

            If Not Regex.IsMatch(PhoneNumber, "^[0-9]+$") Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "電話番号", "数字のみ"), {"model.PhoneNumber"}))
            If PhoneNumber.Length < 10 OrElse PhoneNumber.Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {"model.PhoneNumber"}))
        End If

        'メールアドレス
        If String.IsNullOrWhiteSpace(Me.MailAddress) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "メールアドレス"), {"model.MailAddress"}))
        Else
            Dim mail As New EmailAddressAttribute()

            If Not mail.IsValid(Me.MailAddress) OrElse Not Regex.IsMatch(Me.MailAddress, MAILADDRESS_PATTERN, RegexOptions.IgnoreCase) Then
                result.Add(New ValidationResult(String.Format(MAILADDRESS_ERRORMESSAGE), {"model.MailAddress"}))
            End If

            If Me.MailAddress.Length > LENGTH_256 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "メールアドレス", LENGTH_256), {"model.MailAddress"}))
        End If

        If Me.RelationContentFlags = Nothing OrElse Me.RelationContentFlags = QyRelationContentTypeEnum.None Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "基本情報の開示"), {"model.RelationContentFlags"}))
        End If

        Return result

    End Function

#End Region

End Class