Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute

<Serializable()>
Public NotInheritable Class StartLoginEditInputModel
    Inherits QyPageViewModelBase
    Implements  _
        IQyModelUpdater(Of StartLoginEditInputModel), 
        IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' ダミーのセッションIDを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミーのAPI認証キーを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    ''' <summary>
    ''' 電話番号の最小長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_10 As Integer = 10

    ''' <summary>
    ''' 電話番号の最大長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_11 As Integer = 11

    ''' <summary>
    ''' ユーザーIDの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_100 As Integer = 100

    ''' <summary>
    ''' パスワードの最小長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_8 As Integer = 8

    ''' <summary>
    ''' パスワードの最大長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_32 As Integer = 32

    ''' <summary>
    ''' ユーザーIDとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const QOLMS_PASSWORD_PATTERN As String = "^[A-Za-z0-9!-/:-@≠\[-`{-~]*$"

    ''' <summary>
    ''' パスワードとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const PASSWORD_PATTERN As String = "^((?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])|(?=.*[a-z])(?=.*[A-Z])(?=.*[!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;])|(?=.*[A-Z])(?=.*[0-9])(?=.*[!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;])|(?=.*[a-z])(?=.*[0-9])(?=.*[!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;]))([a-zA-Z0-9!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;]){8,32}$"

#End Region

#Region "Public Property"

    ''' <summary>
    ''' ダミーのセッションIDを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SessionId As String

        Get
            Return StartLoginEditInputModel.SESSION_ID
        End Get

    End Property

    ''' <summary>
    ''' ダミーのAPI認証キーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ApiAuthorizeKey As Guid

        Get
            Return StartLoginEditInputModel.API_AUTHORIZE_KEY
        End Get

    End Property

    ''' <summary>
    ''' アカウントキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Accountkey As Guid = Guid.Empty

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    '<StringLength(StartLoginEditInputModel.LENGTH_256, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    '<RegularExpression(StartLoginEditInputModel.PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれています。")>

    ''' <summary>
    ''' ユーザーIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("ユーザーID")>
    Public Property UserId As String = String.Empty

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    '<StringLength(StartLoginEditInputModel.LENGTH_32, MinimumLength:=StartLoginEditInputModel.LENGTH_8, ErrorMessage:="{0}は{2}文字以上、{1}文字以下で入力してください。")>
    '<RegularExpression(StartLoginEditInputModel.PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれています。")>
    ''' <summary>
    ''' パスワードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("パスワード")>
    Public Property Password As String = String.Empty

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    '<StringLength(StartLoginEditInputModel.LENGTH_32, MinimumLength:=StartLoginEditInputModel.LENGTH_8, ErrorMessage:="{0}は{2}文字以上、{1}文字以下で入力してください。")>
    '<RegularExpression(StartLoginEditInputModel.PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれています。")>
    ''' <summary>
    ''' パスワード（確認）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("パスワード（確認）")>
    Public Property Password2 As String = String.Empty

    Public Property OpenId As String = String.Empty
    Public Property OpenIdType As Byte = Byte.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="StartLoginEditInputModel" /> クラスの新しいインスタンスを初期化します。
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
    Public Sub UpdateByInput(inputModel As StartLoginEditInputModel) Implements IQyModelUpdater(Of StartLoginEditInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel

                Me.UserId = If(String.IsNullOrWhiteSpace(inputModel.UserId), String.Empty, .UserId.Trim())
                Me.Password = If(String.IsNullOrWhiteSpace(inputModel.Password), String.Empty, .Password.Trim())
                Me.Password2 = If(String.IsNullOrWhiteSpace(inputModel.Password2), String.Empty, .Password.Trim())
                'Me.SendFlag = inputModel.SendFlag

            End With
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

        'IDの入力検証
        If Not String.IsNullOrWhiteSpace(Me.UserId) Then

            If Not Regex.IsMatch(Me.UserId, QOLMS_PASSWORD_PATTERN) Then result.Add(New ValidationResult("使用できない文字が含まれています。", {"UserId"}))
            If Me.UserId.Length < LENGTH_8 OrElse Me.UserId.Length > LENGTH_100 Then result.Add(New ValidationResult("IDは8文字以上、100文字以下で入力してください。", {"UserId"}))
        End If

        'パスワードの入力検証
        If Not String.IsNullOrWhiteSpace(Me.Password) Then
            If Not Regex.IsMatch(Me.Password, PASSWORD_PATTERN) Then result.Add(New ValidationResult("パスワードの形式が不正です。", {"Password"}))
            If Me.Password.Length < LENGTH_8 OrElse Me.Password.Length > LENGTH_32 Then result.Add(New ValidationResult("パスワードは8文字以上、32文字以下で入力してください。", {"Password"}))
        Else
            result.Add(New ValidationResult("パスワードに使用できない文字が含まれています。", {"Password"}))
        End If

        If Not String.IsNullOrWhiteSpace(Me.Password) AndAlso Not String.IsNullOrWhiteSpace(Me.Password2) Then
            If Me.Password.Trim().CompareTo(Me.Password2.Trim()) <> 0 Then
                result.Add(New ValidationResult("パスワードと、パスワード（確認）が異なります。", {"Password"}))
            End If
        Else
            result.Add(New ValidationResult("パスワードの形式が不正です。", {"Password"}))
        End If

        Return result

    End Function

#End Region

End Class