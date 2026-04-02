Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

<Serializable()>
Public NotInheritable Class PasswordResetRecoverInputModel
    Inherits QyPasswordResetPageViewModelBase
    Implements IValidatableObject

#Region "Constant"

        ''' <summary>
    ''' メールアドレスの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_256 As Integer = 256

    ''' <summary>
    ''' 入力値が必須であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERROR_MESSAGE As String = "{0}を入力してください。"
        
    ''' <summary>
    ''' 文字列長のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_ERRORMESSAGE As String = "{0}は{1}文字以下で入力してください。"

    ''' <summary>
    ''' メールアドレスの形式エラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_ERRORMESSAGE As String = "有効なメールアドレス形式で入力してください。"

    ''' <summary>
    ''' メールアドレスの正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_PATTERN As String = "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z"

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 登録メールアドレスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("登録メールアドレス")>
    Public Property MailAddress As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteExerciseInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 入力検証エラー メッセージを作成します。
    ''' </summary>
    ''' <param name="format">複合書式指定文字列。</param>
    ''' <param name="propertyName">プロパティ名。</param>
    ''' <param name="displayName">表示名。</param>
    ''' <param name="arg1">書式指定する第 1 オブジェクト。</param>
    ''' <param name="arg2">書式指定する第 2 オブジェクト（オプショナル、デフォルト = Nothing）。</param>
    ''' <returns>
    ''' 入力検証エラー メッセージ。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateErrorMessage(format As String, propertyName As String, displayName As String, Optional arg1 As Object = Nothing, Optional arg2 As Object = Nothing) As String

        Return String.Format(format, If(String.IsNullOrWhiteSpace(displayName), propertyName, displayName), arg1, arg2)

    End Function


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

        If String.IsNullOrWhiteSpace(Me.MailAddress) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "メールアドレス"), {"model.MailAddress"}))
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