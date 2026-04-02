Imports MGF
Imports MGF.QOLMS
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Runtime.Serialization.Json


''' <summary>
''' OpenApi QolmsPoint呼び出しに関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class QolmsPointWorker
    ''' <summary>
    ''' ロック オブジェクトを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly _lockObject As New Object()
    Private Shared _checkAccount As New Dictionary(Of String, DateTime)

#Region "Constant"
    Private Const SERVICENO As Integer = 47003
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
    ''' <param name="serviceNo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteQolmsPointReadApi(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                                     ByVal accountKey As Guid, ByVal serviceNo As Integer) As QoQolmsPointReadApiResults
        Dim apiArgs As New QoQolmsPointReadApiArgs(
            QoApiTypeEnum.QolmsPointRead,
            QsApiSystemTypeEnum.Qolms,
            apiExecutor,
            apiExecutorName
        ) With {
            .AccountKey = accountKey.ToApiGuidString(),
            .ActionServiceNo = serviceNo.ToString()
            }
        Dim apiResults As QoQolmsPointReadApiResults = QsApiManager.ExecuteQolmsOpenApi(Of QoQolmsPointReadApiResults)(
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

    ''' <summary>
    ''' ポイント履歴を取得するAPIを実行します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="accountKey"></param>
    ''' <param name="serviceNo"></param>
    ''' <param name="fromDate"></param>
    ''' <param name="toDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteQolmsPointHistoryReadApi(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                                            ByVal accountKey As Guid, ByVal serviceNo As Integer,
                                                            ByVal fromDate As Date, ByVal toDate As Date) As QoQolmsPointHistoryReadApiResults
        Dim apiArgs As New QoQolmsPointHistoryReadApiArgs(
            QoApiTypeEnum.QolmsPointHistoryRead,
            QsApiSystemTypeEnum.Qolms,
            apiExecutor,
            apiExecutorName
        ) With {
            .ActionServiceNo = serviceNo.ToString(),
            .Filter = New QoApiQolmsPointHistoryFilter() With {.AccountKey = accountKey.ToApiGuidString(),
                                                              .ContainDeletedItem = Boolean.FalseString,
                                                              .StartDate = fromDate.ToApiDateString,
                                                              .EndDate = toDate.ToApiDateString
                                                              }
            }
        Dim apiResults As QoQolmsPointHistoryReadApiResults = QsApiManager.ExecuteQolmsOpenApi(Of QoQolmsPointHistoryReadApiResults)(
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

    ''' <summary>
    ''' ポイント付与APIを実行します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="accountKey"></param>
    ''' <param name="serviceNo"></param>
    ''' <param name="grantList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteQolmsPointWriteApi(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                                    ByVal accountKey As Guid, ByVal serviceNo As Integer,
                                                    ByVal grantList As List(Of QolmsPointGrantItem), Optional updateFlag As Integer = 0) As QoQolmsPointWriteApiResults
        Dim apiArgs As New QoQolmsPointWriteApiArgs(
            QoApiTypeEnum.QolmsPointWrite,
            QsApiSystemTypeEnum.Qolms,
            apiExecutor,
            apiExecutorName
            ) With {
                .ActionServiceNo = serviceNo.ToString(),
                .ActionDataList = New List(Of QoApiQolmsPointGrantActionItem)
                }

        If grantList IsNot Nothing Then
            For Each item As QolmsPointGrantItem In grantList
                apiArgs.ActionDataList.Add(
                        New QoApiQolmsPointGrantActionItem() With {
                            .AccountKey = accountKey.ToApiGuidString(),
                            .SerialNo = item.SerialCode,
                            .ActionDate = item.ActionDate.ToApiDateString(),
                            .ActionPoint = item.Point.ToString(),
                            .PointTargetDate = item.PointTargetDate.ToApiDateString(),
                            .ActionReason = item.Reason,
                            .PointExpirationDate = String.Format("{0:yyyy/MM/dd}", item.PointExpirationDate),
                            .PointItemNo = item.PointItemNo.ToString(),
                            .UpdateFlag = updateFlag.ToString()
                        })
            Next
        End If

        Dim actionDate As Date = grantList.First().ActionDate
        Dim pointItemNo As Integer = grantList.First().PointItemNo
        Dim errorMessage As String = String.Empty
        Dim statusCode As String = String.Empty
        Dim mailMessage As String = String.Empty
        Dim isRequested As Boolean = False
        Dim checkKey As String = String.Empty

        Try
            With apiArgs.ActionDataList.First()
                checkKey = String.Format("{0}{1}", .AccountKey, .PointItemNo)
            End With
            If grantList.First().Point > 0 Then
                '加算のみリクエストを重ねないようにする
                SyncLock QolmsPointWorker._lockObject
                    If QolmsPointWorker._checkAccount.ContainsKey(checkKey) Then
                        If QolmsPointWorker._checkAccount(checkKey) > DateTime.Now.AddMinutes(-1) Then
                            isRequested = True
                            Throw New Exception("既にリクエスト中")
                        Else
                            QolmsPointWorker._checkAccount(checkKey) = DateTime.Now '終わったら消されてるはずだけど
                        End If
                    Else
                        QolmsPointWorker._checkAccount.Add(checkKey, DateTime.Now)
                    End If
                End SyncLock
            End If
            Dim apiResult As QoQolmsPointWriteApiResults = QsApiManager.ExecuteQolmsOpenApi(Of QoQolmsPointWriteApiResults)(
                apiArgs,
                sessionId,
                apiAuthorizeKey
                )

            With apiResult
                If .IsSuccess.TryToValueType(False) Then
                    Return apiResult
                Else

                    Dim resultCode As String = IIf(.Result Is Nothing, "不明", .Result.Code).ToString()

                    statusCode = resultCode
                    mailMessage = String.Format("{0:yyyy/MM/dd HH:mm}: {1} APIの実行に失敗しました。{2}{3}{2} ResultCode={4}", Now, QsApiManager.GetQolmsOpenApiName(apiArgs), vbCrLf, MakeCyptDataString(apiArgs), resultCode)

                End If
            End With

        Catch ex As HttpException

            Dim code As Integer = ex.GetHttpCode()
            statusCode = code.ToString()
            errorMessage = ex.Message
            mailMessage = String.Format("{0:yyyy/MM/dd HH:mm}: {1} APIの実行に失敗しました。{2}{3}{2} ex.Message={4}", Now, QsApiManager.GetQolmsOpenApiName(apiArgs), vbCrLf, MakeCyptDataString(apiArgs), ex.Message)

        Catch ex As Exception

            errorMessage = ex.Message
            mailMessage = String.Format("{0:yyyy/MM/dd HH:mm}: {1} APIの実行に失敗しました。{2}{3}{2} ex.Message={4}", Now, QsApiManager.GetQolmsOpenApiName(apiArgs), vbCrLf, MakeCyptDataString(apiArgs), ex.Message)

        Finally
            If grantList.First().Point > 0 AndAlso isRequested = False Then
                SyncLock QolmsPointWorker._lockObject
                    If String.IsNullOrEmpty(checkKey) = False AndAlso QolmsPointWorker._checkAccount.ContainsKey(checkKey) Then QolmsPointWorker._checkAccount.Remove(checkKey)
                End SyncLock
            End If
            If Not String.IsNullOrWhiteSpace(statusCode) OrElse Not String.IsNullOrWhiteSpace(errorMessage) Then

                QolmsPointWorker.ExecuteQolmsPointRetryLogWriteApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, accountKey, actionDate, "", statusCode, MakeCyptString(errorMessage), pointItemNo, MakeCyptDataString(apiArgs))
                NoticeMailWorker.Send(mailMessage)

            End If

        End Try

        Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)))
        Return Nothing
    End Function

    ''' <summary>
    ''' ポイント付与失敗のログを登録するAPIを実行します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="accountKey"></param>
    ''' <param name="actionDate"></param>
    ''' <param name="CallerSystemName"></param>
    ''' <param name="statusCode"></param>
    ''' <param name="message"></param>
    ''' <param name="pointItemNo"></param>
    ''' <param name="pointRequestString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteQolmsPointRetryLogWriteApi(apiExecutor As Guid, apiExecutorName As String, sessionId As String, apiAuthorizeKey As Guid,
                                                            accountKey As Guid, actionDate As Date, CallerSystemName As String,
                                                            statusCode As String, message As String, pointItemNo As Integer, pointRequestString As String) As QhYappliQolmsPointRetryWriteApiResults

        Dim apiArgs As New QhYappliQolmsPointRetryWriteApiArgs(
            QhApiTypeEnum.YappliQolmsPointRetryWrite,
            QsApiSystemTypeEnum.Qolms,
            apiExecutor,
            apiExecutorName
            ) With {
                .accountKey = accountKey.ToApiGuidString(),
                .actionDate = actionDate.ToApiDateString(),
                .CallerSystemName = "JotoSite",
                .message = message,
                .pointItemNo = pointItemNo.ToString(),
                .PointRequest = pointRequestString,
                .statusCode = statusCode
            }

        Dim apiResults As QhYappliQolmsPointRetryWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliQolmsPointRetryWriteApiResults)(
            apiArgs,
            sessionId,
            apiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else

                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If

        End With
    End Function

    Private Shared Function MakeCyptString(str As String) As String
        Try
            Using crypt As New QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem)
                Return crypt.EncryptString(str)
            End Using
        Catch ex As Exception
            '
        End Try
        Return ""
    End Function

    Private Shared Function MakeCyptDataString(args As QoQolmsPointWriteApiArgs) As String
        Try
            Using crypt As New QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem)
                Using ms As New IO.MemoryStream()
                    With New DataContractJsonSerializer(args.GetType)
                        .WriteObject(ms, args)
                        Return crypt.EncryptString(Encoding.UTF8.GetString(ms.ToArray()))
                    End With
                End Using
            End Using
        Catch ex As Exception
            '
        End Try
        Return ""
    End Function

    Private Shared Function MakeCyptDataString(args As QoQolmsPointWriteApiResults) As String
        Try
            Using crypt As New QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem)
                Using ms As New IO.MemoryStream()
                    With New DataContractJsonSerializer(args.GetType)
                        .WriteObject(ms, args)
                        Return crypt.EncryptString(Encoding.UTF8.GetString(ms.ToArray()))
                    End With
                End Using
            End Using
        Catch ex As Exception
            '
        End Try
        Return ""
    End Function

#End Region

#Region "Public Method"
    ''' <summary>
    ''' 複数のポイントをリストで追加します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="targetAccountKey"></param>
    ''' <param name="pointList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AddQolmsPoints(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                 ByVal targetAccountKey As Guid,
                                 ByVal pointList As List(Of QolmsPointGrantItem)) As Dictionary(Of String, Integer)

        Dim apiResult As QoQolmsPointWriteApiResults = ExecuteQolmsPointWriteApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey,
                                                SERVICENO, pointList)
        Dim results As New Dictionary(Of String, Integer)
        Dim errorExists As Boolean = False
        If apiResult.IsSuccess.TryToValueType(False) Then
            For Each result As QoApiQolmsPointGrantResultItem In apiResult.ResultList
                results.Add(result.SerialNo, result.ErrorCode.TryToValueType(0))
                If result.ErrorCode.TryToValueType(0) > 0 _
                    AndAlso result.ErrorCode.TryToValueType(MGF.QOLMS.QolmsApiCoreV1.QoApiPointGrantActionErrorTypeEnum.None) <> MGF.QOLMS.QolmsApiCoreV1.QoApiPointGrantActionErrorTypeEnum.FrequencyLimit Then
                    errorExists = True
                End If
            Next
            '頻度チェック以外のエラー発生したらメール
            If errorExists Then
                NoticeMailWorker.Send(String.Format("{0:yyyy/MM/dd HH:mm}: AddQolmsPointsポイント付与に失敗しました。{1}result:{2}",
                                                    Now, vbCrLf, MakeCyptDataString(apiResult)))
            End If
            Return results
        End If
        Return Nothing

    End Function

    ''' <summary>
    ''' 複数のポイントをリストで追加します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="targetAccountKey"></param>
    ''' <param name="pointList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function removeQolmsPoints(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                 ByVal targetAccountKey As Guid,
                                 ByVal pointList As List(Of QolmsPointGrantItem)) As Dictionary(Of String, Integer)
        Dim errorExists As Boolean = False
        Dim apiResult As QoQolmsPointWriteApiResults = ExecuteQolmsPointWriteApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey,
                                                SERVICENO, pointList, 9)
        Dim results As New Dictionary(Of String, Integer)
        If apiResult.IsSuccess.TryToValueType(False) Then
            For Each result As QoApiQolmsPointGrantResultItem In apiResult.ResultList
                results.Add(result.SerialNo, result.ErrorCode.TryToValueType(0))
                If result.ErrorCode.TryToValueType(0) > 0 Then
                    errorExists = True
                End If
            Next
            'エラー発生したらメール
            If errorExists Then
                NoticeMailWorker.Send(String.Format("{0:yyyy/MM/dd HH:mm}: AddQolmsPointsポイント付与に失敗しました。{1}result:{2}",
                                                    Now, vbCrLf, MakeCyptDataString(apiResult)))
            End If
            Return results
        End If
        Return Nothing

    End Function
    ' ''' <summary>
    ' ''' ポイントを追加しして成否を返します。追加に成功した場合は直近の有効期限と期限切れになるポイントを取得できるので必要なら戻り値の違うFunctionを追加してください。
    ' ''' </summary>
    ' ''' <param name="apiExecutor"></param>
    ' ''' <param name="apiExecutorName"></param>
    ' ''' <param name="sessionId"></param>
    ' ''' <param name="apiAuthorizeKey"></param>
    ' ''' <param name="targetAccountKey"></param>
    ' ''' <param name="actionDate"></param>
    ' ''' <param name="serialCode"></param>
    ' ''' <param name="pointItemNo"></param>
    ' ''' <param name="point"></param>
    ' ''' <param name="pointExpirationDate"></param>
    ' ''' <param name="reason"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function AddQolmsPoint(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
    '                                 ByVal targetAccountKey As Guid, actionDate As Date,
    '                                 ByVal serialCode As String, ByVal pointItemNo As Integer, ByVal point As Integer, ByVal pointExpirationDate As Date,
    '                                 Optional ByVal reason As String = "") As Dictionary(Of String, Integer)
    '    Dim apiResult As QoQolmsPointWriteApiResults = ExecuteQolmsPointWriteApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey,
    '                                     SERVICENO, New List(Of QolmsPointGrantItem) From {
    '                                                    New QolmsPointGrantItem() With {
    '                                                                        .SerialCode = serialCode,
    '                                                                        .ActionDate = actionDate,
    '                                                                        .Point = point,
    '                                                                        .Reason = reason,
    '                                                                        .PointExpirationDate = pointExpirationDate,
    '                                                                        .PointItemNo = pointItemNo
    '                                                                    }
    '                                                })
    '    Dim results As New Dictionary(Of String, Integer)
    '    If apiResult.IsSuccess.TryToValueType(False) Then
    '        For Each result As QoApiQolmsPointGrantResultItem In apiResult.ResultList
    '            results.Add(result.SerialNo, result.ErrorCode.TryToValueType(0))
    '        Next
    '        Return results
    '    End If
    '    Return Nothing
    'End Function



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
    Public Shared Function GetQolmsPoint(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                         ByVal targetAccountKey As Guid) As Integer
        Return ExecuteQolmsPointReadApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey, SERVICENO).Point.TryToValueType(0)
    End Function

    ''' <summary>
    ''' 現在のポイントと直近の有功期限、
    ''' その有効期限で失効するポイントを取得します。
    ''' </summary>
    ''' <param name="apiExecutor">Web API の実行者アカウント キー。</param>
    ''' <param name="apiExecutorName">Web API の実行者名。</param>
    ''' <param name="sessionId">セッション ID。</param>
    ''' <param name="apiAuthorizeKey">API 認証キー。</param>
    ''' <param name="targetAccountKey">対象者アカウント キー。</param>
    ''' <param name="refClosestExprirationDate">直近の有効期限が格納される変数。</param>
    ''' <param name="refClosestExprirationPoint">直近の有効期限で失効するポイントが格納される変数。</param>
    ''' <returns>
    ''' 現在のポイント。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetQolmsPointWithClosestExpriration(
        apiExecutor As Guid,
        apiExecutorName As String,
        sessionId As String,
        apiAuthorizeKey As Guid,
        targetAccountKey As Guid,
        ByRef refClosestExprirationDate As Date,
        ByRef refClosestExprirationPoint As Integer) As Integer

        ' TODO: IsSuccess ＝ False と例外を分けて判断する必要あり
        With ExecuteQolmsPointReadApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey, QolmsPointWorker.SERVICENO)
            If .IsSuccess.TryToValueType(False) Then
                ' QO_QOLMSPOINT_DAT にデータ有り
                refClosestExprirationDate = .ClosestExprirationDate.TryToValueType(Date.MinValue)
                refClosestExprirationPoint = .ColsestExprirationPoint.TryToValueType(0) ' 綴りミス

                Return .Point.TryToValueType(0)
            Else
                ' QO_QOLMSPOINT_DAT にデータ無し
                refClosestExprirationDate = Date.MinValue
                refClosestExprirationPoint = 0

                Return 0
            End If
        End With

    End Function

    Public Shared Function GetTargetPointFromHistory(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                         ByVal targetAccountKey As Guid, ByVal targetPointItemType As QyPointItemTypeEnum, ByVal targetDate As Date) As Integer
        Dim apiResult As QoQolmsPointHistoryReadApiResults = ExecuteQolmsPointHistoryReadApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey, SERVICENO, Date.MinValue, Date.Now)
        If apiResult.IsSuccess.TryToValueType(False) Then
            Return apiResult.PointHistoryList.Where(Function(m) m.PointTargetDate.TryToValueType(Date.MinValue).Date = targetDate.Date AndAlso CInt(targetPointItemType) = m.PointItemNo.TryToValueType(Integer.MinValue)).Sum(Function(m) m.PointValue.TryToValueType(0))
        End If
        Return Integer.MinValue
    End Function

    ''' <summary>
    ''' 指定した期間のポイント履歴のリストを取得します。
    ''' </summary>
    ''' <param name="apiExecutor"></param>
    ''' <param name="apiExecutorName"></param>
    ''' <param name="sessionId"></param>
    ''' <param name="apiAuthorizeKey"></param>
    ''' <param name="targetAccountKey"></param>
    ''' <param name="targetPointItemType"></param>
    ''' <param name="fromDate"></param>
    ''' <param name="toDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetTargetPointFromHistoryList(ByVal apiExecutor As Guid, ByVal apiExecutorName As String, ByVal sessionId As String, ByVal apiAuthorizeKey As Guid,
                                         ByVal targetAccountKey As Guid, ByVal targetPointItemType As QyPointItemTypeEnum, ByVal fromDate As Date, ByVal toDate As Date) As List(Of QoApiQolmsPointHistoryResultItem)
        Dim apiResult As QoQolmsPointHistoryReadApiResults = ExecuteQolmsPointHistoryReadApi(apiExecutor, apiExecutorName, sessionId, apiAuthorizeKey, targetAccountKey, SERVICENO, fromDate, toDate)
        If apiResult.IsSuccess.TryToValueType(False) Then
            Return apiResult.PointHistoryList
        End If
        Return Nothing
    End Function

#End Region


End Class



