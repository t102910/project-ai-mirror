Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1


''' <summary>
''' OpenApi 呼び出しに関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class OpenApiWorker

#Region "Constant"
#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"
    ''' <summary>
    ''' 現在のポイントと直近の有功期限、その有効期限で失効するポイントを取得するAPIを実行します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="accountKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteAccountAuthenticateApi(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                                     ByVal accountKey As Guid) As QoAccountAuthenticateApiResults
        Dim apiArgs As New QoAccountAuthenticateApiArgs(
            QoApiTypeEnum.AccountAuthenticate,
            QsApiSystemTypeEnum.Qolms,
            apiExecutor,
            apiExecutorName
        ) With {
            .AccountKey = accountKey.ToApiGuidString()
            }
        Dim apiResults As QoAccountAuthenticateApiResults = QsApiManager.ExecuteQolmsOpenApi(Of QoAccountAuthenticateApiResults)(
            apiArgs,
            sessionId,
            apiAuthorizeKey
            )
        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)))
            End If
        End With
    End Function



#End Region

#Region "Public Method"
 
    ''' <summary>
    ''' 現在のポイントを返します。呼び出すAPIで同時に直近の有効期限と期限切れになるポイントを取得できるので必要なら同様のFunctionを追加してください。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="targetAccountKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetApplicationLoginToken(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                         ByVal targetAccountKey As Guid) As String
        Return ExecuteAccountAuthenticateApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey).Token
    End Function


#End Region


End Class



