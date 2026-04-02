Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Net
Imports System.IO
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsDbEntityV1

Friend NotInheritable Class AuWalletPointExchangeWorker

#Region "Constant"
    'todo 検証サーバーで試す、リクエストIDの発行
    '加盟店ID
    Public Shared ReadOnly KAMEITENID As String = ConfigurationManager.AppSettings("AuWalletPointKameitenId")

    'サービスID
    Public Shared ReadOnly SERVICEID As String = ConfigurationManager.AppSettings("AuPaymentServiceId")
    'セキュリティキー
    Public Shared ReadOnly SECUREKEY As String = ConfigurationManager.AppSettings("AuPaymentSecureKey")

    '要求URI
    Public Shared ReadOnly REQUESTURI As String = ConfigurationManager.AppSettings("AuWalletPointExchangeUri")

    Public Shared ReadOnly APIKEY As String = ConfigurationManager.AppSettings("AuWalletPointApiKey")
#End Region

#Region "private Class"

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

    Private Shared Function PostAuWalletPointExchangeRequest(mainModel As QolmsYappliModel, pointId As Integer, point As Integer, serialCode As String) As Boolean

        Dim actionDate As Date = Date.Now

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim requestId As String = String.Empty
        Try
            'Log
            Dim apiResult As QhYappliPortalAuWalletPointExchangeWriteApiResults = AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, point, serialCode, "", "", "", "", "", 0, "", False)
            requestId = apiResult.RequestId

            'Dim data As New NameValueCollection()
            'data("memberId") = AuWalletPointExchangeWorker.KAMEITENID
            'data("serviceId") = AuWalletPointExchangeWorker.SERVICEID
            'data("secureKey") = AuWalletPointExchangeWorker.SECUREKEY
            'data("authKbn") = "2"
            'data("auId") = AuWalletPointExchangeWorker.GetAuSystemID(mainModel.AuthorAccount.OpenId)
            'data("memberAskNo") = requestId  'こちらで一意の番号　文字列20桁まで あとでチェックするのでDBへ保持する
            'data("dispKbn") = "0"
            'data("commodity") = "ポイント交換（ＪＯＴＯポイント）" 'JOTOではこの文言を使用することで合意している
            'data("useAuIdPoint") = "0" '利用Walletポイント（JOTOからは利用しないので0）
            'data("useCmnPoint") = "0" '利用Auポイント（JOTOからは利用しないので0）
            'data("obtnPoint") = point.ToString() '獲得Walletポイント（JOTOからはここに交換の値を入れる）
            'data("tmpObtnKbn") = "1"      '獲得予定区分。予定ではなく即時獲得なので1
            'data("pointEffTimlmtKbn") = "1" '有効期限指定区分　0指定1最長

            DebugLog(mainModel.AuthorAccount.OpenId)
            'DebugLog("テスト用 https://connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_823b7s4nwa65rfv9rvpdgu9njel")
            
            Dim data As New auPointRequestOfJson()
            data.BPIFMerchantPoint_I.memberId =  AuWalletPointExchangeWorker.KAMEITENID
            data.BPIFMerchantPoint_I.serviceId =  AuWalletPointExchangeWorker.SERVICEID
            data.BPIFMerchantPoint_I.secureKey =  AuWalletPointExchangeWorker.SECUREKEY
            data.BPIFMerchantPoint_I.authKbn =  "2"
            data.BPIFMerchantPoint_I.auId = AuWalletPointExchangeWorker.GetAuSystemId(mainModel.AuthorAccount.OpenId)
                'AuWalletPointExchangeWorker.GetAuSystemID("https://connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_823b7s4nwa65rfv9rvpdgu9njel") 
                'AuWalletPointExchangeWorker.GetAuSystemI(mainModel.AuthorAccount.OpenId)
            data.BPIFMerchantPoint_I.memberAskNo =  requestId  'こちらで一意の番号　文字列20桁まで あとでチェックするのでDBへ保持する
            data.BPIFMerchantPoint_I.dispKbn =  "0"
            data.BPIFMerchantPoint_I.commodity =  "ポイント交換（ＪＯＴＯポイント）" 'JOTOではこの文言を使用することで合意している
            data.BPIFMerchantPoint_I.useAuIdPoint =  "0" '利用Walletポイント（JOTOからは利用しないので0）
            data.BPIFMerchantPoint_I.useCmnPoint =  "0" '利用Walletポイント（JOTOからは利用しないので0）
            data.BPIFMerchantPoint_I.obtnPoint =  point.ToString() '獲得Walletポイント（JOTOからはここに交換の値を入れる）
            data.BPIFMerchantPoint_I.tmpObtnKbn =  "1"      '獲得予定区分。予定ではなく即時獲得なので1
            data.BPIFMerchantPoint_I.pointEffTimlmtKbn =  "1" '有効期限指定区分　0指定1最長

            Dim delimiter As String = String.Empty
            Dim values As New StringBuilder()
            'For Each name As String In data.AllKeys
            '    values.Append(delimiter)
            '    values.Append(HttpUtility.UrlEncode(name, Encoding.UTF8))
            '    values.Append("=")
            '    values.Append(HttpUtility.UrlEncode(data(name)))
            '    delimiter = "&"

            'Next
            dim jsr1 As New QsJsonSerializer()
            Dim json As String = jsr1.Serialize(Of auPointRequestOfJson)(data)
            DebugLog(json)
            values.Append(json)

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(values.ToString())

            '''''''''''''''''''''''''''''
            DebugLog(AuWalletPointExchangeWorker.REQUESTURI)
            'リクエスト
            Dim req As HttpWebRequest = CType(WebRequest.Create(AuWalletPointExchangeWorker.REQUESTURI), HttpWebRequest)
            req.Method = "POST"
            req.ContentType = "application/json;charset=UTF-8"
            req.Headers.Add("Accept-Charset", "UTF-8")
            req.Headers.Add("X-Kddi-Api-Key", AuWalletPointExchangeWorker.APIKEY)
            req.Headers.Add("X-Conect-MemberId",　AuWalletPointExchangeWorker.KAMEITENID)
            'req.ContentLength = byteArray.Length

            DebugLog("-----------開始-------------")
            DebugLog("Request　ヘッダ")
            Dim reqheaderCol As WebHeaderCollection = req.Headers
            For i As Integer = 0 To reqheaderCol.Count - 1
                DebugLog(String.Format("{0}={1} ", reqheaderCol.Keys(i), reqheaderCol.Get(i)))
            Next

            ''DebugLog("Postパラメータ")
            ' ''DebugLog(Encoding.UTF8.GetString(reqStream.ToArray()))
            ''For i As Integer = 0 To data.Count - 1
            ''    DebugLog(String.Format("{0}:{1}", data.Keys(i), data.Get(i)))
            ''Next
            ''''''''''''''''''''''''''''''''''''

            'レスポンス
            Dim result As auPointResponseOfJson

            Using newStream As Stream = req.GetRequestStream()
                newStream.Write(byteArray, 0, byteArray.Length)
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
                    Using resStream As Stream = res.GetResponseStream()
                        Using reader As StreamReader = New StreamReader(resStream)
                            ' デシリアライズする
                            'Dim xmlSerializer2 As New XmlSerializer(GetType(auPointResponceXml))
                            'Dim xmlSettings As New System.Xml.XmlReaderSettings() With {
                            '    .CheckCharacters = False
                            '}
                            'Using xmlReader As System.Xml.XmlReader = System.Xml.XmlReader.Create(reader, xmlSettings)
                            '    result = CType(xmlSerializer2.Deserialize(xmlReader), auPointResponceXml)

                            'Jsonにデシリアライズする
                            dim jsr As New QsJsonSerializer()
                            Dim jsonStr As String = reader.ReadToEnd

                            result = jsr.Deserialize(Of auPointResponseOfJson)(jsonStr)

                            DebugLog(jsonStr)
                            DebugLog("Response　Body")
                            DebugLog(String.Format("resultCd :{0}", result.pointIf.control.resultCd))
                            DebugLog(String.Format("pointReceiptNo :{0}", result.pointIf.processResult.pointInfo.pointReceiptNo))
                            'DebugLog(String.Format("obtnHpnDay :{0}", result.processResult.pointInfo.obtnHpnDay))
                            'DebugLog(String.Format("processDay :{0}", result.processResult.pointInfo.processDay))
                            'DebugLog(String.Format("processTime :{0}", result.processResult.pointInfo.processTime))
                            'DebugLog(String.Format("useAuIDHpnDay :{0}", result.processResult.pointInfo.useAuIDHpnDay))
                            'DebugLog(String.Format("useCmnHpnDay :{0}", result.processResult.pointInfo.useCmnHpnDay))

                        End Using
                    End Using
                    ''''''''''''''''''''''''''''''''''''

                    If (status = HttpStatusCode.OK AndAlso result.pointIf.control.resultCd = "PUD100000") Then

                        '登録処理
                        Dim info As PointInfo = result.pointIf.processResult.pointInfo
                        AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, point, serialCode, requestId, "A1", info.pointReceiptNo, info.processDay, info.processTime, Convert.ToInt32(status), result.pointIf.control.resultCd, False)

                        Call DebugLog("通信成功、ポイント付与成功")
                        Return True
                    ElseIf (status = HttpStatusCode.OK AndAlso Not String.IsNullOrWhiteSpace(result.pointIf.control.resultCd)) Then
                        '登録処理
                        Dim info As PointInfo = result.pointIf.processResult.pointInfo
                        AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", info.pointReceiptNo, info.processDay, info.processTime, Convert.ToInt32(status), result.pointIf.control.resultCd, False)

                    ElseIf status = HttpStatusCode.RequestTimeout Then

                        '登録処理
                        AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, point, serialCode, requestId, "A1", String.Empty, String.Empty, String.Empty, Convert.ToInt32(status), String.Empty, False)

                        Dim bodyString As New StringBuilder()
                        bodyString.AppendLine("Pontaポイント交換のリクエストがタイムアウトです。")
                        bodyString.AppendLine("RequestId")
                        bodyString.AppendLine(requestId)

                        Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())
                        Call DebugLog("通信タイムアウト")

                    Else
                        '登録処理
                        AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", String.Empty, String.Empty, String.Empty, Convert.ToInt32(status), String.Empty, False)

                        Dim bodyString As New StringBuilder()
                        bodyString.AppendLine(String.Format("Pontaポイント交換のリクエスト({0})", Convert.ToInt32(status)))
                        bodyString.AppendLine("RequestId")
                        bodyString.AppendLine(requestId)

                        Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())

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
            AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", String.Empty, String.Empty, String.Empty, Convert.ToInt32(ex.Status), String.Empty, False)
        Catch ex As Exception
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            AuWalletPointExchangeWorker.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", String.Empty, String.Empty, String.Empty, 0, String.Empty, False)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("PostAuWalletPointExchangeRequest:{0}", ex.Message))
        End Try
        Call DebugLog("通信成功、ポイント付与失敗")
        Return False
    End Function
    Private Shared Function ExecuteAuPointExchangeItemReadApi(mainModel As QolmsYappliModel, actionDate As Date, id As Integer) As QhYappliPortalAuWalletPointExchangeItemReadApiResults

        Dim apiArgs As New QhYappliPortalAuWalletPointExchangeItemReadApiArgs(
         QhApiTypeEnum.YappliPortalAuWalletPointMasterRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
             .ActionDate = actionDate.ToApiDateString(),
             .AuWalletPointItemId = id.ToString()
        }
        Dim apiResults As QhYappliPortalAuWalletPointExchangeItemReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAuWalletPointExchangeItemReadApiResults)(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey
                )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                If String.IsNullOrEmpty(.AuWalletPointItemId) Then
                    Call DebugLog(String.Format("Pontaの交換マスタがありません。id:{0}", id))
                    Throw New Exception(String.Format("Pontaの交換マスタがありません。id:{0}", id)) 'とりあえずわかればいいので
                Else
                    Return apiResults
                End If
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecuteAuWalletPointReadApi(mainModel As QolmsYappliModel) As QhYappliPortalAuWalletPointExchangeReadApiResults

        Dim apiArgs As New QhYappliPortalAuWalletPointExchangeReadApiArgs(
         QhApiTypeEnum.YappliPortalAuWalletPointRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }
        Dim apiResults As QhYappliPortalAuWalletPointExchangeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAuWalletPointExchangeReadApiResults)(
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
    Private Shared Function ExecuteAuWalletPointWriteApi(mainModel As QolmsYappliModel, actionDate As Date, AuWalletPointItemId As Integer, demandPoint As Integer, serialCode As String, requestId As String, _
                                                         actionType As String, pointReceiptNo As String, responseDate As String, responseTime As String, httpStatusCode As Integer, _
                                                         result As String, deleteFlag As Boolean) As QhYappliPortalAuWalletPointExchangeWriteApiResults
        Dim matchResultType As Byte = 0 'Enum
        Dim apiArgs As New QhYappliPortalAuWalletPointExchangeWriteApiArgs(
         QhApiTypeEnum.YappliPortalAuWalletPointWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .ActionDate = actionDate.ToApiDateString(),
             .AuWalletPointItemId = AuWalletPointItemId.ToString(),
             .SerialCode = serialCode,
             .DemandPoint = demandPoint.ToString(),
             .RequestId = requestId,
             .ActionType = actionType,
             .PointReceiptNo = pointReceiptNo,
             .ResponseDate = responseDate,
             .ResponseTime = responseTime,
             .HttpStatusCode = httpStatusCode.ToString(),
             .Result = result,
             .MatchResultType = matchResultType.ToString(),
             .DeleteFlag = deleteFlag.ToString()
        }
        Dim apiResults As QhYappliPortalAuWalletPointExchangeWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAuWalletPointExchangeWriteApiResults)(
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
    <Conditional("DEBUG")>
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/Log"), "auPoint.log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Date.Now, message, vbCrLf))
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum) As PortalAuWalletPointExchangeViewModel

        DebugLog("CreateViewModel")
        Dim result As New PortalAuWalletPointExchangeViewModel()
        Dim apiResult As QhYappliPortalAuWalletPointExchangeReadApiResults = AuWalletPointExchangeWorker.ExecuteAuWalletPointReadApi(mainModel)
        result.AuWalletPointItemN = apiResult.AuWalletPointItemN.ConvertAll(Function(i) i.ToAuWalletPointItem())
        result.AuWalletPointHistN = apiResult.AuWalletPointHistN.ConvertAll(Function(i) i.ToAuWalletPointHistItem())
        result.FromPageNoType = pageNo
        '履歴に表示する→result:成功コード matchresult:0,1

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

    Shared Function Exchange(mainModel As QolmsYappliModel, itemid As Integer) As Boolean

        DebugLog("Exchange")
        Dim actionDate As Date = Date.Now
        'チャージマスタ
        Dim result As QhYappliPortalAuWalletPointExchangeItemReadApiResults = AuWalletPointExchangeWorker.ExecuteAuPointExchangeItemReadApi(mainModel, actionDate, itemid)

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
                        .PointItemNo = QyPointItemTypeEnum.AuPoint,
                        .Point = Integer.Parse(result.Point) * -1,
                        .PointTargetDate = actionDate,
                        .PointExpirationDate = Date.MaxValue,
                        .Reason = "Pontaポイントと交換"
                        }
            }.ToList()
        )
        If pointResult.First().Value = 0 Then

            '成功したらチャージ
            If AuWalletPointExchangeWorker.PostAuWalletPointExchangeRequest(mainModel, Integer.Parse(result.AuWalletPointItemId), Integer.Parse(result.AuWalletPoint), pointResult.First().Key) Then
                Return True
            Else
                Dim pointLimitDate As Date = New Date(
                      actionDate.Year,
                      actionDate.Month,
                      1
                  ).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

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
                                .Reason = "Pontaポイントと交換失敗のためポイントを復元"
                                }
                    }.ToList()
                )

                If removePointResult.First().Value <> 0 Then
                    Dim bodyString1 As New StringBuilder()
                    bodyString1.AppendLine("Pontaポイント修正のエラーです。")
                    bodyString1.AppendLine(String.Format("error:{0}", removePointResult.First().Key))

                    Dim task1 As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString1.ToString())
                End If

                'メー
                Dim bodyString As New StringBuilder()
                bodyString.AppendLine("Pontaポイント交換のエラーです。")
                bodyString.AppendLine("SerialCode:")
                bodyString.AppendLine(pointResult.First().Key)

                Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())

                Throw New InvalidOperationException(String.Format("Pontaポイント交換に失敗しました。SerialCode:{0}", pointResult.First().Key))

            End If
        Else
            Call DebugLog("ポイントの減算に失敗しました")
            Throw New InvalidOperationException(String.Format("ポイントの減算に失敗しました。{0}", pointResult.First().Value))
        End If

    End Function

#End Region



End Class
