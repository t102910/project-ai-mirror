Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Net
Imports System.IO
Imports System.Threading.Tasks

Friend NotInheritable Class PortalDatachargeWorker


#Region "Enum"

    ''' <summary>
    ''' データチャージログの種別を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ActionTypeEnum As Byte

        ''' <summary>
        ''' 未指定です。
        ''' </summary>
        ''' <remarks></remarks>
        None = QsApiDatachargeActionTypeEnum.None

        ''' <summary>
        ''' リクエストです。
        ''' </summary>
        ''' <remarks></remarks>
        Request = QsApiDatachargeActionTypeEnum.Request

        ''' <summary>
        ''' レスポンスです。
        ''' </summary>
        ''' <remarks></remarks>
        Response = QsApiDatachargeActionTypeEnum.Response

        ''' <summary>
        ''' エラーです。
        ''' </summary>
        ''' <remarks></remarks>
        [Error] = QsApiDatachargeActionTypeEnum.Error


    End Enum

#End Region

#Region "Constant"
    'todo 検証サーバーで試す、リクエストIDの発行
    '加盟店ID
    Public Shared ReadOnly KAMEITENID As String = ConfigurationManager.AppSettings("AuPaymentKameitenId")
    '要求URI
    Public Shared ReadOnly REQUESTURI As String = ConfigurationManager.AppSettings("AuDatachargeUri")
    'エラーでメールを送る
    Public Shared ReadOnly IsSendMail As String = ConfigurationManager.AppSettings("IsSendMailDatacharge")

    'テストサーバーの仮想日
    Public Shared ReadOnly TESTSERVER_VIRTUALDATE As String = ConfigurationManager.AppSettings("AuPaymentTestServerVirtualDate")

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

    Private Shared Function PostDataChargeRequest(mainModel As QolmsYappliModel, eventId As String, serialCode As String) As Boolean

        Dim now As Date = Date.Now
        'Log
        Dim result As QhYappliPortalDatachageLogWriteApiResults = PortalDatachargeWorker.ExecuteDatachargeLogWriteApi(mainModel, ActionTypeEnum.Request, "", eventId, now.ToString("yyyyMMddhhmmss"), "", 0, "", serialCode, "")
        Dim requestId As String = result.RequestId

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Try
            'リクエストデータ
            Dim data As NameValueCollection = HttpUtility.ParseQueryString(String.Empty)

            If REQUESTURI.Contains("test.") Then
                'テストは未来日に
                '検証サーバの日付ずれてるため
                If String.IsNullOrEmpty(TESTSERVER_VIRTUALDATE) = False Then
                    Dim virtualdate As Date = now.Date
                    Date.TryParseExact(TESTSERVER_VIRTUALDATE, "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None, virtualdate)
                    data("RequestDate") = virtualdate.ToString("yyyyMMddHHmmss")
                End If
            Else
                data("RequestDate") = now.ToString("yyyyMMddHHmmss")
            End If
            data("RequestId") = requestId '16桁
            data("Command") = "DTC00201"
            data("AuId") = PortalDatachargeWorker.GetAuSystemID(mainModel.AuthorAccount.OpenId)
            data("MemberId") = AuPaymentAccessWorker.KAMEITENID
            data("EventId") = HttpUtility.UrlEncode(eventId, System.Text.Encoding.UTF8)
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(data.ToString())

            ''''''''''''''''''''''''''''''''''''
            DebugLog("Postパラメータ")
            For i As Integer = 0 To data.Count - 1
                DebugLog(String.Format("{0}:{1}", data.Keys(i), data.Get(i)))
            Next
            '''''''''''''''''''''''''''''

            'リクエスト
            Dim req As HttpWebRequest = CType(WebRequest.Create(REQUESTURI), HttpWebRequest)
            req.Method = "POST"
            req.ContentType = "application/x-www-form-urlencoded"
            'req.AcceptCharset = "UTF-8"
            req.ContentLength = byteArray.Length

            'レスポンス
            Using reqStream As Stream = req.GetRequestStream()
                reqStream.Write(byteArray, 0, byteArray.Length)
                Using res As HttpWebResponse = CType(req.GetResponse(), HttpWebResponse)

                    Dim status As HttpStatusCode = res.StatusCode
                    'response Header 
                    DebugLog("status =" & status)
                    DebugLog("Response　ヘッダ")
                    Dim headerCol As WebHeaderCollection = res.Headers
                    For i As Integer = 0 To headerCol.Count - 1
                        DebugLog(String.Format("{0}={1} ", headerCol.Keys(i), headerCol.Get(i)))
                    Next
                    ''''''''''''''''''''''''''''''''''''
                    'response body
                    Dim responseKeyValue As New Dictionary(Of String, String)()
                    Using resStream As Stream = res.GetResponseStream()
                        Using reader As StreamReader = New StreamReader(resStream)
                            DebugLog("Response　Body")
                            responseKeyValue = GetKeyValue(reader.ReadToEnd())
                            For i As Integer = 0 To responseKeyValue.Count - 1
                                DebugLog(String.Format("{0}={1} ", responseKeyValue.Keys(i), responseKeyValue.Item(responseKeyValue.Keys(i))))
                            Next
                        End Using
                    End Using
                    ''''''''''''''''''''''''''''''''''''
                    'Log
                    If responseKeyValue.ContainsKey("ResponseDate") Then
                        PortalDatachargeWorker.ExecuteDatachargeLogWriteApi(mainModel, ActionTypeEnum.Response, data("RequestId"), data("EventId"), data("RequestDate"), responseKeyValue.Item("ResponseDate"), Convert.ToInt32(status), responseKeyValue.Item("Result"), serialCode, String.Empty)
                    ElseIf responseKeyValue.ContainsKey("err") AndAlso responseKeyValue.Item("err") = "6" Then
                        PortalDatachargeWorker.ExecuteDatachargeLogWriteApi(mainModel, ActionTypeEnum.Response, data("RequestId"), data("EventId"), String.Empty, String.Empty, Convert.ToInt32(status), String.Empty, serialCode, "KDDIシステムメンテナンス中です。")
                    Else
                        PortalDatachargeWorker.ExecuteDatachargeLogWriteApi(mainModel, ActionTypeEnum.Response, data("RequestId"), data("EventId"), String.Empty, String.Empty, Convert.ToInt32(status), String.Empty, serialCode, String.Empty)
                    End If
                    ''''''''''''''''''''''''''''''''''''
                    'status = RequestTimeout
                    If (status = HttpStatusCode.OK AndAlso responseKeyValue.Item("Result") = "0000") OrElse status = HttpStatusCode.RequestTimeout Then

                        If status = HttpStatusCode.RequestTimeout Then

                            Dim bodyString As New StringBuilder()
                            bodyString.AppendLine("データチャージのリクエストがタイムアウトです。")
                            bodyString.AppendLine("RequestId")
                            bodyString.AppendLine(requestId)

                            Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())

                        End If

                        Return True
                    End If

                End Using
            End Using
        Catch ex As WebException
            Dim response As String = ""
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                response = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(response)
            End If
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            PortalDatachargeWorker.ExecuteDatachargeLogWriteApi(mainModel, ActionTypeEnum.Error, requestId, "", "", "", 0, "", serialCode, ex.Message)
        Catch ex As Exception
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            PortalDatachargeWorker.ExecuteDatachargeLogWriteApi(mainModel, ActionTypeEnum.Error, requestId, "", "", "", 0, "", serialCode, ex.Message)
        End Try

        Return False
    End Function
    Private Shared Function ExecuteDatachargeEventIdReadApi(mainModel As QolmsYappliModel, actionDate As Date, capacity As Integer) As QhYappliPortalDatachargeEventIdReadApiResults

        Dim apiArgs As New QhYappliPortalDatachargeEventIdReadApiArgs(
         QhApiTypeEnum.YappliPortalDatachargeEventIdRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
             .ActionDate = actionDate.ToApiDateString(),
             .DataSize = capacity.ToString()
        }
        Dim apiResults As QhYappliPortalDatachargeEventIdReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalDatachargeEventIdReadApiResults)(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey
                )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                If String.IsNullOrEmpty(.EventId) Then
                    Throw New Exception(String.Format("{0}MB の データチャージのイベントIDマスタがありません。", capacity)) 'とりあえずわかればいいので
                Else
                    Return apiResults
                End If
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecuteDatachargeReadApi(mainModel As QolmsYappliModel) As QhYappliPortalDatachargeReadApiResults

        Dim apiArgs As New QhYappliPortalDatachargeReadApiArgs(
         QhApiTypeEnum.YappliPortalDatachargeRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }
        Dim apiResults As QhYappliPortalDatachargeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalDatachargeReadApiResults)(
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
    Private Shared Function ExecuteDatachargeLogWriteApi(mainModel As QolmsYappliModel, actionType As Integer, requestId As String, eventId As String, _
                                                         requestDate As String, responseDate As String, httpStatusCode As Integer, result As String, _
                                                         serialCode As String, comment As String) As QhYappliPortalDatachageLogWriteApiResults

        Dim apiArgs As New QhYappliPortalDatachageLogWriteApiArgs(
         QhApiTypeEnum.YappliPortalDatachargeLogWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .ActionDate = Date.Now.ToApiDateString(),
             .ActionType = actionType.ToString(),
             .RequestId = requestId,
             .EventId = eventId,
             .RequestDate = requestDate,
             .ResponseDate = responseDate,
             .HttpStatusCode = httpStatusCode.ToString(),
             .Result = result,
             .SerialCode = serialCode,
             .Comment = comment
        }
        Dim apiResults As QhYappliPortalDatachageLogWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalDatachageLogWriteApiResults)(
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

    'ログイン時のOpenIDとして取得するIDはUri形式でその最後の要素だけをAuSystemIDとして利用する
    Private Shared Function GetAuSystemID(ByVal openid As String) As String
        Dim tmp As String() = openid.Split("/"c)
        Return tmp.Last()
    End Function

    'テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "auDataCharge.txt")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception

        End Try

    End Sub

    'Bodyに入ってくるKeyValue値をDictionaryに展開
    Private Shared Function GetKeyValue(bodyText As String) As Dictionary(Of String, String)
        Dim results As New Dictionary(Of String, String)
        Dim pair() As String = Split(bodyText, vbLf)
        If pair IsNot Nothing AndAlso pair.Count > 0 Then
            For Each keyValueStr As String In pair
                Dim kv() As String = keyValueStr.Split("="c)
                If kv.Length = 2 Then
                    results.Add(kv(0), kv(1))
                End If
            Next
        End If
        Return results
    End Function
#End Region

#Region "Public Method"
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum) As PortalDatachargeViewModel

        Dim result As New PortalDatachargeViewModel()
        Dim apiResult As QhYappliPortalDatachargeReadApiResults = PortalDatachargeWorker.ExecuteDatachargeReadApi(mainModel)
        result.EventIdN = apiResult.EventIdN.ConvertAll(Function(i) i.ToDatachargeEventIdItem())
        result.DatachargeHistN = apiResult.DatachargeHistN.ConvertAll(Function(i) i.ToDatachargeHistItem())
        result.FromPageNoType = pageNo

        'ポイント数の表示
        Try
            result.Point = QolmsPointWorker.GetQolmsPoint(
                            mainModel.ApiExecutor,
                            mainModel.ApiExecutorName,
                            mainModel.SessionId,
                            mainModel.ApiAuthorizeKey,
                            mainModel.AuthorAccount.AccountKey
                            )
        Catch ex As Exception
            result.Point = 0
        End Try

        Return result

    End Function

    Public Shared Function Charge(mainModel As QolmsYappliModel, capacity As Integer) As Boolean

        Dim actionDate As Date = Date.Now
        'チャージマスタ
        Dim result As QhYappliPortalDatachargeEventIdReadApiResults = PortalDatachargeWorker.ExecuteDatachargeEventIdReadApi(mainModel, actionDate, capacity)

        'point減算
        Dim limit As Date = Date.MinValue

        Dim pointResult As Dictionary(Of String, Integer) = QolmsPointWorker.AddQolmsPoints(
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey,
            mainModel.AuthorAccount.AccountKey,
            {
                New QolmsPointGrantItem() With {
                        .ActionDate = actionDate,
                        .SerialCode = Guid.NewGuid().ToApiGuidString(),
                        .PointItemNo = QyPointItemTypeEnum.Datacharge,
                        .Point = Integer.Parse(result.Point) * -1,
                        .PointTargetDate = actionDate,
                        .PointExpirationDate = Date.MaxValue,
                        .Reason = "auデータと交換"
                        }
            }.ToList()
        )
        If pointResult.First().Value = 0 Then
            '成功したらチャージ
            If PortalDatachargeWorker.PostDataChargeRequest(mainModel, result.EventId, pointResult.First().Key) Then
                Return True
            Else
                Dim pointLimitDate As Date = New Date(
                         actionDate.Year,
                         actionDate.Month,
                         1
                     ).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）
                'チャージ失敗
                Dim removePointResult As Dictionary(Of String, Integer) = QolmsPointWorker.AddQolmsPoints(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey,
                    {
                        New QolmsPointGrantItem() With {
                                .ActionDate = actionDate,
                                .SerialCode = Guid.NewGuid().ToApiGuidString(),
                                .PointItemNo = QyPointItemTypeEnum.RecoveryPoint,
                                .Point = Integer.Parse(result.Point),
                                .PointTargetDate = actionDate,
                                .PointExpirationDate = pointLimitDate,
                                .Reason = "auデータと交換失敗のためポイント復元"
                                }
                    }.ToList()
                )

                If removePointResult.First().Value <> 0 Then
                    Dim bodyString As New StringBuilder()
                    bodyString.AppendLine("データチャージポイント修正のエラーです。")
                    bodyString.AppendLine(String.Format("error:{0}", removePointResult.First().Key))

                    Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())
                End If

                'メール
                If (Not String.IsNullOrWhiteSpace(IsSendMail)) AndAlso Boolean.Parse(IsSendMail) Then

                    Dim bodyString As New StringBuilder()
                    bodyString.AppendLine("データチャージのエラーです。")
                    bodyString.AppendLine("SerialCode:")
                    bodyString.AppendLine(pointResult.First().Key)

                    Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())
                End If

                Throw New InvalidOperationException(String.Format("データチャージに失敗しました。SerialCode:{0}", pointResult.First().Key))
            End If
        Else
            Throw New InvalidOperationException(String.Format("ポイントの減算に失敗しました。"))
        End If

    End Function
#End Region

End Class
