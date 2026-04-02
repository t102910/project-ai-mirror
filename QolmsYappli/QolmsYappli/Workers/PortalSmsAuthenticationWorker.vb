Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PortalSmsAuthenticationWorker

#Region "Constant"

    '''' <summary>
    '''' ダミーのセッションIDを現します。
    '''' </summary>
    '''' <remarks></remarks>
    'Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    '''' <summary>
    '''' ダミーのAPI認証キーを表します。
    '''' </summary>
    '''' <remarks></remarks>
    'Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    '''' <summary>
    '''' SMSのタイトル
    '''' </summary>
    'Private Shared ReadOnly _smsSubject As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSAuthenticationSubject"))

    ''' <summary>
    ''' リトライカウント
    ''' </summary>
    Private Shared ReadOnly _retryCount As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSAuthenticationCount"))

    ''' <summary>
    ''' パスコードの桁数
    ''' </summary>
    Private Shared ReadOnly _passCodeLength As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSPassCodeLength"))

    ''' <summary>
    ''' パスコードの有効期限
    ''' </summary>
    Private Shared ReadOnly _passCodeExpiresMinutes As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSPassCodeExpiresMinutes"))


#End Region


#Region "Private Property"

    ''' <summary>
    ''' パスコード入力のリトライカウントを取得します。
    ''' </summary>
    Public Shared ReadOnly Property RetryCount As Integer
        Get

            Dim result As Integer = Integer.MinValue
            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_retryCount.Value) AndAlso Integer.TryParse(_retryCount.Value, result) Then

                result = If(result > 0, result, 1)
            Else
                result = 1
            End If

            Return result
        End Get

    End Property

    ''' <summary>
    ''' パスコードの桁数を取得します
    ''' </summary>
    Public Shared ReadOnly Property PassCodeLength As Integer
        Get
            Dim result As Integer = Integer.MinValue

            '設定があれば設定値、なければ初期値6
            If Not String.IsNullOrWhiteSpace(_passCodeLength.Value) AndAlso Integer.TryParse(_passCodeLength.Value, result) Then

                result = If(result > 0, result, 6)
            Else
                result = 6
            End If

            Return result
        End Get

    End Property

    ''' <summary>
    ''' パスコードの有効期限（分）を取得します。
    ''' </summary>
    Public Shared ReadOnly Property PassCodeExpiresMinutes As Integer
        Get
            Dim result As Integer = Integer.MinValue

            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_passCodeExpiresMinutes.Value) AndAlso Integer.TryParse(_passCodeExpiresMinutes.Value, result) Then

                result = If(result > 0, result, 1)
            Else
                result = 1
            End If

            Return result
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region


#Region "Private Method"


    ''' <summary>
    ''' SMS認証に使用するパスコードを発行
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    Private Shared Function ExecutePortalSmsAuthenticationPassCodeWriteApi(mainModel As QolmsYappliModel, phoneNumber As String, userAgent As String, userHostAddress As String, userHostName As String) As QjPortalSmsAuthenticationWriteApiResults

        Dim apiArgs As New QjPortalSmsAuthenticationWriteApiArgs(
         QjApiTypeEnum.PortalSmsAuthenticationWrite,
         QsApiSystemTypeEnum.QolmsJoto,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .PhoneNumber = phoneNumber,
             .PassCodeLength = PortalSmsAuthenticationWorker.PassCodeLength.ToString(),
             .ExpiresMinutes = PortalSmsAuthenticationWorker.PassCodeExpiresMinutes.ToString(),
             .UserAgent = userAgent,
             .UserHostAddress = userHostAddress,
             .UserHostName = userHostName
        }
        Dim apiResults As QjPortalSmsAuthenticationWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalSmsAuthenticationWriteApiResults)(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey2
                )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ExecutePortalSmsAuthenticationReadApi(mainModel As QolmsYappliModel) As QhYappliPortalTermsReadApiResults

        Dim apiArgs As New QhYappliPortalTermsReadApiArgs(
         QhApiTypeEnum.YappliPortalTermsRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString
        }
        Dim apiResults As QhYappliPortalTermsReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTermsReadApiResults)(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey
                )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

#End Region


#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalTermsViewModel




    End Function

    Friend Shared Function SendPassCode(mainModel As QolmsYappliModel, phoneNumber As String) As Boolean

        Dim userHostAddress As String = HttpContext.Current.Request.UserHostAddress
        Dim userHostName As String = HttpContext.Current.Request.UserHostName
        Dim userAgent As String = HttpContext.Current.Request.UserAgent

        ' パスコード
        With PortalSmsAuthenticationWorker.ExecutePortalSmsAuthenticationPassCodeWriteApi(mainModel, phoneNumber, userAgent, userHostAddress, userHostName)

            If .IsSuccess.TryToValueType(False) AndAlso String.IsNullOrWhiteSpace(.PassCode) Then

                ' SMS送信


                Return True
            End If

        End With

        Return False

    End Function
#End Region

End Class
