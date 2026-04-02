<Serializable()>
Public NotInheritable Class PortalChallengeEntryPassResultPartialViewModel
    Inherits QyPartialViewModelBase(Of PortalChallengeEntryInputModel)

#Region "Public Property"

    ''' <summary>
    ''' Passコード入力を表示するかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PassCodeVisible As Boolean = False

    ''' <summary>
    ''' 必須入力項目のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PassCodes As New List(Of String)()

    ''' <summary>
    ''' 必須入力項目のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequiredN As New Dictionary(Of String, String)()

    ''' <summary>
    ''' 任意入力項目のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OptionalN As New Dictionary(Of String, String)()

    ''' <summary>
    ''' 連携システム番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' Viewer連携が必要かどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RelationFlag As Boolean = False

    ''' <summary>
    ''' メールアドレスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MailAddress As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeEntryPassResultPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="PortalChallengeEntryPassResultPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As PortalChallengeEntryInputModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
