'Imports MGF.QOLMS.QolmsApiCoreV1
'Imports MGF.QOLMS.QolmsApiEntityV1

'Public Class TestWorker

'    ''' <summary>
'    ''' ダミーのセッションIDを現します。
'    ''' </summary>
'    ''' <remarks></remarks>
'    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

'    ''' <summary>
'    ''' ダミーのAPI認証キーを表します。
'    ''' </summary>
'    ''' <remarks></remarks>
'    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

'    Private Shared Function ExecutePasswordResetRecoverWriteApi(mainmodel As QolmsYappliModel) As QjTestRead1ApiResults

'        Dim apiArgs As New QjTestRead1ApiArgs(
'            QjApiTypeEnum.TestRead1,
'            QsApiSystemTypeEnum.QolmsJoto,
'             mainModel.ApiExecutor,
'             mainModel.ApiExecutorName
'        ) With{
'            .ActorKey = mainmodel.AuthorAccount.AccountKey.ToString(),
'            .Message = "test"
'        }

'        Dim apiResults As QjTestRead1ApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjTestRead1ApiResults)(
'                    apiArgs,
'                    mainModel.SessionId,
'                    mainModel.ApiAuthorizeKey
'                )

'        If apiResults.IsSuccess.TryToValueType(False) Then

'            Return apiResults

'        Else

'            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
'        End If

'    End Function


'    Friend Shared Sub JotoApiCall(mainModel As QolmsYappliModel )
'        ExecutePasswordResetRecoverWriteApi(mainModel)

'    End Sub
'End Class
