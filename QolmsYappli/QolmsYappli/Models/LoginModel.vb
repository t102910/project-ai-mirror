Imports MGF.QOLMS.QolmsApiCoreV1

Public NotInheritable Class LoginModel
    Implements IQyModelUpdater(Of LoginModel)

#Region "Public Property"

    ''' <summary>
    ''' QOLMSの サイト名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property QolmsSiteName As String

        Get
            Return QyConfiguration.QolmsYappliSiteName
        End Get

    End Property

    Public Property UserId As String = String.Empty

    Public Property Password As String = String.Empty
    
    Public Property PasswordHash As String = String.Empty

    Public Property RememberId As Boolean = False

    Public Property RememberLogin As Boolean = False

    Public Property LoginResultType As QsApiLoginResultTypeEnum = QsApiLoginResultTypeEnum.None

    Public Property Message As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="LoginModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' インプット モデルの内容を現在のインスタンスに反映します。
    ''' </summary>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(inputModel As LoginModel) Implements IQyModelUpdater(Of LoginModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.UserId = If(String.IsNullOrWhiteSpace(.UserId), String.Empty, .UserId.Trim())
                Me.Password = If(String.IsNullOrWhiteSpace(.Password), String.Empty, .Password.Trim())
                Me.RememberId = .RememberId
                Me.RememberLogin = .RememberLogin
            End With
        End If

    End Sub

#End Region

End Class