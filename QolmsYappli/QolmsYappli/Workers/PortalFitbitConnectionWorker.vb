Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PortalFitbitConnectionWorker

#Region "Constant"

    '連携システム番号
    Private Const FITBIT_LINKAGESYSTEMNO As Integer = 47011

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

    Private Shared Function ExecuteFitbitConnectionReadApi(mainModel As QolmsYappliModel) As QhYappliPortalFitbitConnectionReadApiResults

        Dim apiArgs As New QhYappliPortalFitbitConnectionReadApiArgs(
            QhApiTypeEnum.YappliPortalFitbitConnectionRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = FITBIT_LINKAGESYSTEMNO.ToString()
        }
        Dim apiResults As QhYappliPortalFitbitConnectionReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalFitbitConnectionReadApiResults)(
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

    Private Shared Function ExecuteFitbitConnectionWriteApi(mainModel As QolmsYappliModel, token As FitbitTokenSet, deleteFlag As Boolean) As QhYappliPortalFitbitConnectionWriteApiResults

        Dim apiArgs As New QhYappliPortalFitbitConnectionWriteApiArgs(
            QhApiTypeEnum.YappliPortalFitbitConnectionWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = FITBIT_LINKAGESYSTEMNO.ToString(),
            .LinkageSystemId = token.user_id,
            .DeleteFlag = deleteFlag.ToString(),
            .TokenSet = New QhApiFitbitTokenSetItem() With {
            .Token = token.Token,
            .TokenExpires = token.TokenExpires.ToApiDateString(),
            .RefreshToken = token.RefreshToken,
            .RefreshTokenExpires = token.RefreshTokenExpires.ToApiDateString()
        }
        }
        Dim apiResults As QhYappliPortalFitbitConnectionWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalFitbitConnectionWriteApiResults)(
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

    Private Shared Function ExecuteFitbitConnectionTokenReadApi(mainModel As QolmsYappliModel) As QhYappliPortalFitbitConnectionTokenReadApiResults

        Dim apiArgs As New QhYappliPortalFitbitConnectionTokenReadApiArgs(
            QhApiTypeEnum.YappliPortalFitbitConnectionTokenRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = FITBIT_LINKAGESYSTEMNO.ToString()
        }
        Dim apiResults As QhYappliPortalFitbitConnectionTokenReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalFitbitConnectionTokenReadApiResults)(
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

    'Private Shared Function EncriptString(str As String) As String

    '    If String.IsNullOrWhiteSpace(str) Then Return String.Empty

    '    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
    '        Return crypt.EncryptString(str)
    '    End Using

    'End Function

    'Private Shared Function DecriptString(str As String) As String

    '    If String.IsNullOrWhiteSpace(str) Then Return String.Empty

    '    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
    '        Return crypt.DecryptString(str)
    '    End Using

    'End Function

    'Private Shared Sub SendMail(message As String)

    '    Dim br As New StringBuilder()
    '    br.AppendLine(String.Format("タニタ接続のエラーです。"))
    '    br.AppendLine(message)
    '    Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(br.ToString())

    'End Sub


    ''テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "fitbit.log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Public Method"


    ''' <summary>
    ''' 連携設定画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, fromPageNo As QyPageNoTypeEnum) As PortalFitbitConnectionViewModel

        Dim result As New PortalFitbitConnectionViewModel() With {
            .FromPageNoType = fromPageNo
            }

        With PortalFitbitConnectionWorker.ExecuteFitbitConnectionReadApi(mainModel)

            If .IsSuccess.TryToValueType(False) AndAlso .LinkageSystemNo.TryToValueType(Integer.MinValue) > 0 AndAlso Not String.IsNullOrWhiteSpace(.LinkageSystemId) Then

                ' 連携あり
                result.FitbitConnectedFlag = True

            End If

        End With

        Return result
    End Function

    ''' <summary>
    ''' 連携認証用のURLを取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAuthorizationUrl() As String

        Dim url As String = FitbitWorker.GetFitbitAuthorizationUrl()

        Return url

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="token"></param>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Register(mainModel As QolmsYappliModel, token As FitbitTokenSet, ByRef message As String) As Boolean

        With PortalFitbitConnectionWorker.ExecuteFitbitConnectionWriteApi(mainModel, token, False)

            message = .ErrorMessage
            Return .IsSuccess.TryToValueType(False)
        End With

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Cancel(mainModel As QolmsYappliModel, ByRef message As String) As Boolean

        Dim tokenSet As New FitbitTokenSet()
        'getToken
        With PortalFitbitConnectionWorker.ExecuteFitbitConnectionTokenReadApi(mainModel)

            tokenSet.Token = .TokenSet.Token
            tokenSet.TokenExpires = .TokenSet.TokenExpires.TryToValueType(Date.MinValue)
            tokenSet.RefreshToken = .TokenSet.RefreshToken
            tokenSet.RefreshTokenExpires = .TokenSet.RefreshTokenExpires.TryToValueType(Date.MinValue)

        End With

        'cancel
        ' 解除は投げるだけ。成功の可否関係なく解除する
         FitbitWorker.RevokeFitbitToken(tokenSet)
        
        ' cancel Register
        With PortalFitbitConnectionWorker.ExecuteFitbitConnectionWriteApi(mainModel, tokenSet, True)
            message = .ErrorMessage
            Return .IsSuccess.TryToValueType(False)

        End With

        Return False

    End Function


    ''' <summary>
    ''' Fitbit連携があるかどうかを取得します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsFitbitConnected(mainModel As QolmsYappliModel) As Boolean

        With PortalFitbitConnectionWorker.ExecuteFitbitConnectionReadApi(mainModel)

            Return .IsSuccess.TryToValueType(False) AndAlso .LinkageSystemNo.TryToValueType(Integer.MinValue) > 0 AndAlso Not String.IsNullOrWhiteSpace(.LinkageSystemId)

        End With

    End Function

#End Region


End Class