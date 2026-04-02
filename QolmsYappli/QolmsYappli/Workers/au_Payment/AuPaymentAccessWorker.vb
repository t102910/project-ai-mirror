Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Net
Imports System.IO
Imports System.Threading.Tasks
Imports System.Runtime.Serialization.Json


''' <summary>
''' KDDI のかんたん決済呼び出しに関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class AuPaymentAccessWorker

#Region "Constant"
    '加盟店ID
    Public Shared ReadOnly KAMEITENID As String = ConfigurationManager.AppSettings("AuPaymentKameitenId")
    'サービスID
    Public Shared ReadOnly SERVICEID As String = ConfigurationManager.AppSettings("AuPaymentServiceId")
    'セキュリティキー
    Public Shared ReadOnly SECUREKEY As String = ConfigurationManager.AppSettings("AuPaymentSecureKey")
    'サービス名
    Public Shared ReadOnly SERVICENAME As String = ConfigurationManager.AppSettings("AuPaymentServiceName")
    '要求URI
    Public Shared ReadOnly REQUESTURI As String = ConfigurationManager.AppSettings("AuPaymentRequestUri")
    '認証確認OKのときの戻りアクションUri
    Public Shared ReadOnly AUTHOKURL As String = ConfigurationManager.AppSettings("AuPaymentAuthOkUri")
    '認証確認NGの時の戻りアクションUri
    Public Shared ReadOnly AUTHNGURL As String = ConfigurationManager.AppSettings("AuPaymentAuthNgUri")
    'コンテンツID　これが重複すると登録できない。つまり二重登録防止になる。
    Public Shared ReadOnly CONTENTID As String = ConfigurationManager.AppSettings("AuPaymentContentId")
    '課金額
    Public Shared ReadOnly PREMIUMMEMBERAMOUNT As Integer = ConfigurationManager.AppSettings("AuPaymentPremiumMemberAmount").TryToValueType(300)

    '2019年10月消費税増税対応
    '古い課金額
    Public Shared ReadOnly OLD_PREMIUMMEMBERAMOUNT As Integer = ConfigurationManager.AppSettings("AuPaymentPremiumMemberOldAmount").TryToValueType(324)
    '新しい課金額
    Public Shared ReadOnly NEW_PREMIUMMEMBERAMOUNT As Integer = ConfigurationManager.AppSettings("AuPaymentPremiumMemberNewAmount").TryToValueType(330)
    '切り替え日
    Public Shared ReadOnly PREMIUMMEMBERAMOUNT_CHANGEDATE As String = ConfigurationManager.AppSettings("AuPaymentAmountChangeDate")
    'KDDIテストサーバの仮想日付
    Public Shared ReadOnly TESTSERVER_VIRTUALDATE As String = ConfigurationManager.AppSettings("AuPaymentTestServerVirtualDate")

    '成功を示す結果コード
    Private Const AUSUCCESSCODE As String = "MPL01000"
    Public Const AUREGISTEREDERRORCODE As String = "MPL40538"
    '結果コード名
    Private Const RESULTCODEHEADERNAME As String = "X-ResultCd"

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

    '(a)決済認可要求
    '   NG (b)決済認可エラー処理＞エラー画面表示
    '   OK (c)ユーザ認証リダイレクト
    ' 同意
    '       NG(d)認証NO画面表示
    '       OK
    '       暗証番号認証
    '           NG(e)認証NG画面
    '           OK
    ' 継続課金情報登録の場合における注意点 
    ' 課金タイミング区分 01:日指定のとき、課金タイミングには 1～28 の値が指定できます。（29 日以降の日を 指定することはできません。）  月末指定の場合は課金タイミング 02:月末日を指定してください。 
    ' 継続課金処理における初回課金が実行するタイミングと決済情報作成は、下記の通りとなります。 
    '・継続課金サービス登録日＝初回課金年月日（継続課金サービスへの登録時に初回課金を実行し決済情報を 作成します） 
    '・継続課金サービス登録日＜初回課金年月日（初回課金年月日当日 0 時に初回課金を実行し 9 時までに決済 情報を作成します） 
    ' 継続課金における決済認可要求電文パラメータ『コンテンツ ID』の管理により、同一サービス(商品)にお ける重複課金防止が可能となります。   
    '※『コンテンツ ID』パラメータの利用イメージは『【au かんたん決済】運用ガイド』をご参照ください。
    '  継続課金において、決済認可要求で一度設定した『課金タイミング』や『次回以降金額』等の全パラメー タは、あとから変更することはできません。  
    '           (a)継続課金情報 登録要求
    '               NG(b)継続課金情報登録エラー処理＞エラー画面
    '               OK(c)決済結果画面表示処理

    '決済認可要求
    Private Shared Function PostPayCertRequest(ByRef auPaymentModel As PaymentInf, firstAmount As Integer, nextAmount As Integer) As Boolean
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ' SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Using wb As WebClient = New WebClient()
                wb.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
                wb.Headers(HttpRequestHeader.AcceptCharset) = "UTF-8"
                Dim data As NameValueCollection = GetAuCommonParam()
                data("certType") = "03"
                data("auoneId") = auPaymentModel.AuId
                data("accountIntervalKbn") = "02"
                data("accountInterval") = "1"
                data("accountTimingKbn") = "01" '01日指定、02月末
                data("accountTiming") = "01" '日付
                If REQUESTURI.Contains("test.") Then
                    '検証サーバの日付ずれてるため
                    Dim difMonth As Long = DateDiff(DateInterval.Month, New Date(Now.Year, Now.Month, 1), auPaymentModel.ContinueAccountStartDate)
                    If String.IsNullOrEmpty(TESTSERVER_VIRTUALDATE) = False Then
                        Dim virtualdate As Date = Now.Date
                        Date.TryParseExact(TESTSERVER_VIRTUALDATE, "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None, virtualdate)
                        virtualdate = virtualdate.AddMonths(CInt(difMonth))
                        auPaymentModel.ContinueAccountStartDate = New Date(virtualdate.Year, virtualdate.Month, 1)
                    End If
                End If
                data("firstAccountDay") = String.Format("{0:yyyyMMdd}", auPaymentModel.ContinueAccountStartDate)  '課金開始日
                DebugLog(String.Format("本来の課金開始日{0:yyyy/MM/dd}", auPaymentModel.ContinueAccountStartDate))
                data("firstAmount") = firstAmount.ToString
                data("nextAmount") = nextAmount.ToString
                data("commodity") = "毎月一日課金" ' System.Web.HttpUtility.UrlEncode("毎月一日課金", Encoding.UTF8).ToUpper()   '概要"
                data("memberAuthOkUrl") = AUTHOKURL 'System.Web.HttpUtility.UrlEncode(AUTHOKURL, Encoding.UTF8) '認証OKURL
                data("memberAuthNgUrl") = AUTHNGURL 'System.Web.HttpUtility.UrlEncode(AUTHNGURL, Encoding.UTF8) '認証NGURL
                data("memberManageNo") = auPaymentModel.MemberManageNo.ToString() '加盟店管理番号
                data("contentsId") = CONTENTID  'コンテンツID
                ''''''''''''''''''''''''''''''''''''
                DebugLog("Postパラメータ")
                For i As Integer = 0 To data.Count - 1
                    DebugLog(String.Format("{0}:{1}", data.Keys(i), data.Get(i)))
                Next
                '''''''''''''''''''''''''''''
                Dim response() As Byte = wb.UploadValues(REQUESTURI & "?ID=PayCertForContBill", "POST", data)
                Dim responseText As String = System.Text.Encoding.UTF8.GetString(response)
                Dim responseKeyValue As Dictionary(Of String, String) = GetKeyValue(responseText)
                auPaymentModel.AuResultCode = wb.ResponseHeaders.Get(RESULTCODEHEADERNAME).Trim()
                If auPaymentModel.AuResultCode = AUSUCCESSCODE AndAlso responseKeyValue.Keys.Contains("transactionId") Then
                    auPaymentModel.TransctionId = responseKeyValue("transactionId")
                    Return True
                End If
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                DebugLog("Response　Body")
                DebugLog(responseText)
                DebugLog("Response　ヘッダ")
                Dim headerCol As WebHeaderCollection = wb.ResponseHeaders
                For i As Integer = 0 To headerCol.Count - 1
                    DebugLog(String.Format("{0}={1} ", headerCol.Keys(i), headerCol.Get(i)))
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''

            End Using

        Catch ex As WebException
            Dim response As String = ""
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                response = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(response)
            End If
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("PostPayCertRequest Error:{0}/Response:{1}", ex.Message, response))
        Catch ex As Exception
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("PostPayCertRequest Error:{0}", ex.Message))
        End Try
        Return False
    End Function

    '継続課金情報登録要求
    Private Shared Function ContBillRequest(ByRef auPaymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ' SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Using wb As WebClient = New WebClient()
                wb.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
                wb.Headers(HttpRequestHeader.AcceptCharset) = "UTF-8"

                Dim data As NameValueCollection = GetAuCommonParam()
                'トランザクションIDをセット
                data("transactionId") = auPaymentModel.TransctionId

                Dim response() As Byte = wb.UploadValues(REQUESTURI & "?ID=ContBill", "POST", data)
                Dim responseText As String = System.Text.Encoding.UTF8.GetString(response)
                Dim responseKeyValue As Dictionary(Of String, String) = GetKeyValue(responseText)
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                DebugLog("Response　Body")
                DebugLog(responseText)
                DebugLog("ContinueAccountId =" & auPaymentModel.ContinueAccountId)
                DebugLog("Response　ヘッダ")
                Dim headerCol As WebHeaderCollection = wb.ResponseHeaders
                For i As Integer = 0 To headerCol.Count - 1
                    DebugLog(String.Format("{0}={1} ", headerCol.Keys(i), headerCol.Get(i)))
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If wb.ResponseHeaders.Get(RESULTCODEHEADERNAME).Trim() = AUSUCCESSCODE AndAlso
                    responseKeyValue.Keys.Contains("continueAccountId") Then
                    auPaymentModel.ContinueAccountId = responseKeyValue("continueAccountId")
                    'auPaymentModel.ContinueAccountStartDate = StringToDate(responseKeyValue("processDay"), responseKeyValue("processTime"))
                    auPaymentModel.StartDate = StringToDate(responseKeyValue("processDay"), responseKeyValue("processTime"))
                    'auPaymentModel.StartDate = Now.Date
                    auPaymentModel.EndDate = Date.MaxValue
                    auPaymentModel.MemberManageNo = responseKeyValue("memberManageNo").TryToValueType(Long.MinValue)
                    Return True
                End If


            End Using

        Catch ex As WebException
            Dim response As String = ""
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                response = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(response)
            ElseIf ex.Status = WebExceptionStatus.Timeout Then
                'タイムアウトの時は成否不明なので処理取り消しを行う
                ' 118秒あけないとならないので非同期で
                Dim transId As String = auPaymentModel.TransctionId
                Task.Run(
                    Sub()
                        System.Threading.Thread.Sleep(15000)    'タイムアウトなら既に100秒たっているはずなので+15秒
                        Dim resultCode As String = AuPaymentAccessWorker.OperateCancelContBillRequest(transId)
                        If resultCode <> AUSUCCESSCODE Then
                            System.Threading.Thread.Sleep(15000)    'タイムアウトなら既に100秒たっているはずなので+15秒
                            resultCode = AuPaymentAccessWorker.OperateCancelContBillRequest(transId)
                        End If
                        Call DebugLog(String.Format("ContBillRequest Timeout> OperateCancelResultCode:{0}", resultCode))
                        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillRequest Timeout retry> OperateCancelResultCode:{0}", resultCode))
                    End Sub
                    )
                NoticeMailWorker.SendAsync(String.Format("ContBillRequest Timeout :{0}", MakeCyptDataString(auPaymentModel)))
            End If
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillRequest Error:{0}/Response{1}", ex.Message, response))
        Catch ex As Exception
            Call DebugLog(String.Format("ContBillRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillRequest Error:{0}", ex.Message))

        End Try
        Return False
    End Function

    '継続課金解約要求
    Private Shared Function ContBillCancelRequest(ByRef auPaymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ' SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Using wb As WebClient = New WebClient()
                wb.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
                wb.Headers(HttpRequestHeader.AcceptCharset) = "UTF-8"
                Dim data As NameValueCollection = GetAuCommonParam()
                data("continueAccountId") = auPaymentModel.ContinueAccountId

                Dim response() As Byte = wb.UploadValues(REQUESTURI & "?ID=ContBillCancel", "POST", data)
                Dim responseText As String = System.Text.Encoding.UTF8.GetString(response)
                Dim responseKeyValue As Dictionary(Of String, String) = GetKeyValue(responseText)
                auPaymentModel.AuResultCode = wb.ResponseHeaders.Get(RESULTCODEHEADERNAME).Trim()

                '''''''''''''''''''''''''''''''''''''''''''''''''''
                DebugLog("Response　Body")
                DebugLog(responseText)
                DebugLog("ContinueAccountId =" & auPaymentModel.ContinueAccountId)
                DebugLog("Response　ヘッダ")
                Dim headerCol As WebHeaderCollection = wb.ResponseHeaders
                For i As Integer = 0 To headerCol.Count - 1
                    DebugLog(String.Format("{0}={1} ", headerCol.Keys(i), headerCol.Get(i)))
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If auPaymentModel.AuResultCode = AUSUCCESSCODE AndAlso
                  responseKeyValue.Keys.Contains("processDay") Then
                    auPaymentModel.EndDate = StringToDate(responseKeyValue("processDay"), responseKeyValue("processTime"))
                    Return True

                End If

            End Using

        Catch ex As WebException
            Dim response As String = ""
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                response = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(response)
            End If
            If ex.Status = WebExceptionStatus.Timeout Then
                'タイムアウトの時は成否不明
                NoticeMailWorker.SendAsync(String.Format("ContBillRequest Timeout :{0}", MakeCyptDataString(auPaymentModel)))
            End If
            Call DebugLog(String.Format("ContBillCancelRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillCancelRequest Error:{0}/Response{1}", ex.Message, response))
        Catch ex As Exception
            Call DebugLog(String.Format("ContBillCancelRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillCancelRequest Error:{0}", ex.Message))

        End Try
        Return False
    End Function

    '継続課金処理取り消し要求
    Private Shared Function OperateCancelContBillRequest(transactionId As String) As String
        Dim result As String = ""
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ' SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Using wb As WebClient = New WebClient()
                wb.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
                wb.Headers(HttpRequestHeader.AcceptCharset) = "UTF-8"
                Dim data As NameValueCollection = GetAuCommonParam()
                data("continueAccountId") = transactionId

                Dim response() As Byte = wb.UploadValues(REQUESTURI & "?ID=OperateCancelContBill", "POST", data)
                Dim responseText As String = System.Text.Encoding.UTF8.GetString(response)
                Dim responseKeyValue As Dictionary(Of String, String) = GetKeyValue(responseText)
                result = wb.ResponseHeaders.Get(RESULTCODEHEADERNAME).Trim()


                '''''''''''''''''''''''''''''''''''''''''''''''''''
                DebugLog("Response　Body")
                DebugLog(responseText)

                DebugLog("Response　ヘッダ")
                Dim headerCol As WebHeaderCollection = wb.ResponseHeaders
                For i As Integer = 0 To headerCol.Count - 1
                    DebugLog(String.Format("{0}={1} ", headerCol.Keys(i), headerCol.Get(i)))
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''

            End Using

        Catch ex As WebException
            Dim response As String = ""
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                response = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(response)
            End If

            Call DebugLog(String.Format("ContBillCancelRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillCancelRequest Error:{0}/Response{1}", ex.Message, response))
        Catch ex As Exception
            Call DebugLog(String.Format("ContBillCancelRequest Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ContBillCancelRequest Error:{0}", ex.Message))

        End Try
        Return result
    End Function

    'かんたん決済関連API呼び出し共通パラメータを生成
    Private Shared Function GetAuCommonParam() As NameValueCollection
        Dim result As New NameValueCollection()
        result("memberId") = AuPaymentAccessWorker.KAMEITENID
        result("serviceId") = AuPaymentAccessWorker.SERVICEID
        result("secureKey") = AuPaymentAccessWorker.SECUREKEY
        Return result
    End Function

    'Bodyに入ってくるKeyValue値をDictionaryに展開
    Private Shared Function GetKeyValue(bodyText As String) As Dictionary(Of String, String)
        Dim results As New Dictionary(Of String, String)
        Dim pair() As String = bodyText.Split("&"c)
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

    '日付文字列を日付に変換
    Private Shared Function StringToDate(ByVal yyyyMMddStr As String, ByVal hhmmssStr As String) As DateTime

        If yyyyMMddStr.Length = 8 AndAlso IsNumeric(yyyyMMddStr) AndAlso hhmmssStr.Length = 6 AndAlso IsNumeric(hhmmssStr) Then
            Dim y As Integer = CInt(Left(yyyyMMddStr, 4))
            Dim m As Integer = CInt(Mid(yyyyMMddStr, 5, 2))
            Dim d As Integer = CInt(Right(yyyyMMddStr, 2))
            Dim h As Integer = CInt(Left(hhmmssStr, 2))
            Dim mm As Integer = CInt(Mid(hhmmssStr, 3, 2))
            Dim s As Integer = CInt(Right(hhmmssStr, 2))
            Return New DateTime(y, m, d, h, mm, s)

        End If
        Return DateTime.MinValue
    End Function


    Private Shared Function MakeCyptDataString(args As PaymentInf) As String
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
    'テスト用の手抜きログ吐き
    <Conditional("DEBUG")>
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "AuPaymentAccesslog.txt")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception

        End Try

    End Sub




#End Region

#Region "Public Method"



    ''' <summary>
    ''' '決済認可要求をして、成功したらユーザ認証要求URLを返す
    ''' </summary>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RequestPayCertAndGetRedirectUrl(ByRef auPaymentModel As PaymentInf) As String
        '消費税増税対策
        Dim firstAmount As Integer = PREMIUMMEMBERAMOUNT
        Dim nextAmount As Integer = PREMIUMMEMBERAMOUNT
        If String.Format("{0:yyyyMMdd}", auPaymentModel.ContinueAccountStartDate) >= PREMIUMMEMBERAMOUNT_CHANGEDATE Then
            firstAmount = NEW_PREMIUMMEMBERAMOUNT
            nextAmount = NEW_PREMIUMMEMBERAMOUNT
        Else
            If String.Format("{0:yyyyMMdd}", auPaymentModel.ContinueAccountStartDate.AddMonths(1)) >= PREMIUMMEMBERAMOUNT_CHANGEDATE Then
                nextAmount = NEW_PREMIUMMEMBERAMOUNT
            Else
                nextAmount = OLD_PREMIUMMEMBERAMOUNT
            End If
            firstAmount = OLD_PREMIUMMEMBERAMOUNT
        End If

        If AuPaymentAccessWorker.PostPayCertRequest(auPaymentModel, firstAmount, nextAmount) Then
            Return REQUESTURI & "?ID=UserPermitBridge&transactionId=" & auPaymentModel.TransctionId
        Else
            Return ""   'エラー
        End If
    End Function

    ''' <summary>
    ''' '課金情報登録をして成否を返す
    ''' </summary>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SetContinuedBilling(ByRef auPaymentModel As PaymentInf) As Boolean
        Return ContBillRequest(auPaymentModel)
    End Function

    ''' <summary>
    ''' 継続課金登録をキャンセルする
    ''' </summary>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RequestContBillCancel(ByRef auPaymentModel As PaymentInf) As Boolean
        Return ContBillCancelRequest(auPaymentModel)
    End Function

#End Region


End Class






