Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute

Public NotInheritable Class PortalTanitaConnectionInputModel
    Implements  _
        IQyModelUpdater(Of PortalTanitaConnectionInputModel), 
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
    ''' パスワードとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const PASSWORD_PATTERN As String = "^[A-Za-z0-9!-/:-@≠\[-`{-~]*$"

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
            Return PortalTanitaConnectionInputModel.SESSION_ID
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
            Return PortalTanitaConnectionInputModel.API_AUTHORIZE_KEY
        End Get

    End Property

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    ''' <summary>
    ''' IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("ID")>
    Public Property ID As String = String.Empty

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    ''' <summary>
    ''' パスワードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("パスワード")>
    Public Property Password As String = String.Empty

    ''' <summary>
    ''' 連携IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("連携ID")>
    Public Property ConnectionID As String = String.Empty

    ''' <summary>
    ''' デバイスのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("デバイス")>
    Public Property Devices As New List(Of DeviceItem)()

    ''' <summary>
    ''' 体組成計のデータ取得するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BodyCompositionMeter As Boolean = False

    ''' <summary>
    ''' 歩数のデータ取得するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sphygmomanometer As Boolean = False

    ''' <summary>
    ''' 血圧のデータ取得するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Pedometer As Boolean = False

    ''' <summary>
    ''' ALKOOの連携があるかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AlkooConnectedFlag As Boolean = False


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalTanitaConnectionInputModel" /> クラスの新しいインスタンスを初期化します。
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
    Public Sub UpdateByInput(inputModel As PortalTanitaConnectionInputModel) Implements IQyModelUpdater(Of PortalTanitaConnectionInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.ID = If(String.IsNullOrWhiteSpace(inputModel.ID), String.Empty, .ID.Trim())
                Me.Password = If(String.IsNullOrWhiteSpace(inputModel.Password), String.Empty, .Password.Trim())
                'Me.DataFalgs = inputModel.DataFalgs
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

        If String.IsNullOrWhiteSpace(Me.ConnectionID) Then

            If String.IsNullOrWhiteSpace(Me.ID) Then
                result.Add(New ValidationResult("IDを入力してください。", {"ID"}))
            Else
                If Me.ID.Length > 64 Then
                    result.Add(New ValidationResult("IDは64文字文字以下で入力してください。", {"ID"}))
                End If

            End If
            If String.IsNullOrWhiteSpace(Me.Password) Then
                result.Add(New ValidationResult("パスワードを入力してください。", {"Password"}))
            Else
                If Me.Password.Length > 16 Then
                    result.Add(New ValidationResult("パスワードは16文字文字以下で入力してください。", {"Password"}))
                End If

            End If
        End If

        Return result

    End Function

#End Region

End Class