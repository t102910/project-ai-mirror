Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

<Serializable()>
Public NotInheritable Class PasswordResetRecoverSMSInputModel
    Inherits QyPasswordResetPageViewModelBase
    Implements IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 電話番号の最小文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_10 As Integer = 10

    ''' <summary>
    ''' 電話番号の最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_11 As Integer = 11

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



#End Region

#Region "Public Property"

    ''' <summary>
    ''' 登録電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("登録電話番号")>
    Public Property PhoneNumber As String = String.Empty


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

        If String.IsNullOrWhiteSpace(Me.PhoneNumber) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "電話番号"), {"model.PhoneNumber"}))
        Else

            If Not Regex.IsMatch(Me.PhoneNumber, "^[0-9]+$") Then result.Add(New ValidationResult("数値のみで入力してください。", {"model.PhoneNumber"}))
            If Me.PhoneNumber.Length < 10 OrElse Me.PhoneNumber.Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {"model.PhoneNumber"}))

        End If
        Return result

    End Function

#End Region

End Class