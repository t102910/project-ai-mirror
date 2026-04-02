Imports MGF.QOLMS.QolmsApiCoreV1
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports MGF.QOLMS

''' <summary>
''' KDDI のOwl（属性取得API)呼び出しに関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class AuOwlAccessWorker

#Region "Constant"

    Public Shared ReadOnly OWL_URI As String = If(String.IsNullOrEmpty(ConfigurationManager.AppSettings("AuOwlUri")), "https://p2.stg.openapi.au.com/proxy/k1-biscuit/v1/op/gs/cca_rt_op_api/property/getUserAttributeXml_allAu", ConfigurationManager.AppSettings("AuOwlUri"))
    Public Shared ReadOnly OWL_ACCESSKEY As String = If(String.IsNullOrEmpty(ConfigurationManager.AppSettings("AuOwlKey")), "f287fc61-ab8d-4b99-969f-cf30a0cc2341", ConfigurationManager.AppSettings("AuOwlKey")) 'KDDIの属性取得APIのアクセスキー
    Public Shared ReadOnly OWL_SID As String = If(String.IsNullOrEmpty(ConfigurationManager.AppSettings("AuOwlSid")), "99APESAUmPywPdgU", ConfigurationManager.AppSettings("AuOwlSid"))
    Public Shared ReadOnly OWL_FID As String = If(String.IsNullOrEmpty(ConfigurationManager.AppSettings("AuOwlFid")), "ATTR0139", ConfigurationManager.AppSettings("AuOwlFid"))

    '・sid
    '検証：99APESAUmPywPdgU
    '商用：99APESAUXhWt4FRK

    '・fid
    '検証：ATTR0139
    '商用：同上

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

    Private Class MyStringWriter
        Inherits StringWriter
        Public Overrides ReadOnly Property Encoding As Encoding
            Get
                Return Encoding.GetEncoding("shift-jis")
            End Get
        End Property

    End Class

#Region "Private Method"

    Private Shared Function MakeRequestString(ByVal sid As String, ByVal fid As String, ByVal utype As String, ByVal uidtf As String) As String
        Dim result As String = String.Empty
        Dim reqObj As New QolmsYappli.OwlRequest.biscuitif() With {.fid = fid, .sid = sid, .utype = utype, .uidtf = uidtf}
        Using writer As New MyStringWriter()
            Dim serializer As New XmlSerializer(GetType(QolmsYappli.OwlRequest.biscuitif))
            Dim xsn As New XmlSerializerNamespaces()
            xsn.Add("cocoa", "http://www.kddi.com/cocoa")
            serializer.Serialize(writer, reqObj, xsn)
            result = writer.ToString()
        End Using
        Return result
    End Function

    ''' <summary>
    ''' Owlにユーザ属性を問い合わせます。
    ''' </summary>
    ''' <param name="openid">ユーザのシステムAuID</param>
    ''' <param name="retryCount">流量制限によってエラー時に、リトライを行う場合はリトライ数、リトライせず返す場合は０(省略可)。</param>
    ''' <returns>成功した場合(ステータス200)は取得したXMLをデシリアライズしたクラスを返却、失敗時はNothing</returns>
    ''' <remarks>ステータス200でもデータが取れているとは限らない(メンテナンス中なども含む)のでresultStatus(0：正常。3：異常、6：メンテナンス中)をチェックする必要があります</remarks>
    Private Shared Function OwlRequestByOpenid(ByVal openid As String, Optional ByVal retryCount As Integer = 0) As biscuitif

        Call DebugLog(String.Format("OwlRequest :{0}", OWL_URI))

        Dim results As biscuitif = Nothing
        Dim uriObj As New Uri(OWL_URI)
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Using wb As WebClient = New WebClient()
                '指定のヘッダを設定
                wb.Headers.Add(String.Format("X-KDDI-API-KEY:{0}", OWL_ACCESSKEY))
                wb.Headers(HttpRequestHeader.Host) = uriObj.Host
                wb.Headers(HttpRequestHeader.AcceptCharset) = "Windows-31J"
                wb.Headers(HttpRequestHeader.ContentType) = "text/xml"
                Dim enc As System.Text.Encoding = Encoding.GetEncoding("shift-jis")
                Dim postStr As String = MakeRequestString(OWL_SID, OWL_FID, "OPENID", openid)
                Call DebugLog(String.Format("PostData :{0}", postStr))
                Dim postData As Byte() = enc.GetBytes(postStr)
                Dim resData As Byte() = wb.UploadData(uriObj, postData)
                wb.Dispose()

                '受信したデータ
                Dim resText As String = enc.GetString(resData)
                Call DebugLog(String.Format("ResData :{0}", resText))
                Dim serializer As XmlSerializer = New XmlSerializer(GetType(biscuitif))
                results = DirectCast(serializer.Deserialize(New StringReader(resText)), biscuitif)

            End Using

        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                Dim body As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(body)
                For Each key As String In ex.Response.Headers
                    If key = "Retry-After" AndAlso IsNumeric(ex.Response.Headers.Get(key)) AndAlso retryCount > 0 Then '流量制限が発生した場合のリトライインターバル（秒）
                        '流量制限が発生した場合,指定の秒数待ってから再度リクエスト
                        System.Threading.Thread.Sleep(1000 * CInt(ex.Response.Headers.Get(key)))
                        Return OwlRequestByOpenid(openid, retryCount - 1)
                    End If
                    DebugLog(String.Format("Ex Header>{0}:{1}", key, ex.Response.Headers.Get(key)))
                Next
            End If
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("OwlRequest Error:{0}", ex.Message))

            Call DebugLog(String.Format("OwlRequest Error:{0}", ex.Message))
        Catch ex As Exception

            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("OwlRequest Error:{0}", ex.Message))
            Call DebugLog(String.Format("OwlRequest Error:{0}", ex.Message))
            'Throw ex
        End Try
        Return results
    End Function

    ''' <summary>
    ''' Owlにユーザ属性を問い合わせます。
    ''' </summary>
    ''' <param name="auid">ユーザのシステムAuID</param>
    ''' <param name="retryCount">流量制限によってエラー時に、リトライを行う場合はリトライ数、リトライせず返す場合は０(省略可)。</param>
    ''' <returns>成功した場合(ステータス200)は取得したXMLをデシリアライズしたクラスを返却、失敗時はNothing</returns>
    ''' <remarks>ステータス200でもデータが取れているとは限らない(メンテナンス中なども含む)のでresultStatus(0：正常。3：異常、6：メンテナンス中)をチェックする必要があります</remarks>
    Private Shared Function OwlRequest(ByVal auid As String, Optional ByVal retryCount As Integer = 0) As biscuitif
        Call DebugLog(String.Format("OwlRequest :{0}", OWL_URI))

        Dim results As biscuitif = Nothing
        Dim uriObj As New Uri(OWL_URI)
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Using wb As WebClient = New WebClient()
                '指定のヘッダを設定
                wb.Headers.Add(String.Format("X-KDDI-API-KEY:{0}", OWL_ACCESSKEY))
                wb.Headers(HttpRequestHeader.Host) = uriObj.Host
                wb.Headers(HttpRequestHeader.AcceptCharset) = "Windows-31J"
                wb.Headers(HttpRequestHeader.ContentType) = "text/xml"
                Dim enc As System.Text.Encoding = Encoding.GetEncoding("shift-JIS")
                Dim postStr As String = MakeRequestString(OWL_SID, OWL_FID, "AUID", auid)
                Call DebugLog(String.Format("PostData :{0}", postStr))
                Dim postData As Byte() = enc.GetBytes(postStr)
                Dim resData As Byte() = wb.UploadData(uriObj, postData)
                wb.Dispose()

                '受信したデータ
                Dim resText As String = enc.GetString(resData)
                Call DebugLog(String.Format("ResData :{0}", resText))
                Dim serializer As XmlSerializer = New XmlSerializer(GetType(biscuitif))
                results = DirectCast(serializer.Deserialize(New StringReader(resText)), biscuitif)

            End Using

        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                Dim body As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(body)
                For Each key As String In ex.Response.Headers
                    If key = "Retry-After" AndAlso IsNumeric(ex.Response.Headers.Get(key)) AndAlso retryCount > 0 Then '流量制限が発生した場合のリトライインターバル（秒）
                        '流量制限が発生した場合,指定の秒数待ってから再度リクエスト
                        System.Threading.Thread.Sleep(1000 * CInt(ex.Response.Headers.Get(key)))
                        Return OwlRequest(auid, retryCount - 1)
                    End If
                    DebugLog(String.Format("Ex Header>{0}:{1}", key, ex.Response.Headers.Get(key)))
                Next
            End If
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("OwlRequest Error:{0}", ex.Message))
            Call DebugLog(String.Format("OwlRequest Error:{0}", ex.Message))
        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("OwlRequest Error:{0}", ex.Message))
            Call DebugLog(String.Format("OwlRequest Error:{0}", ex.Message))
            'Throw ex
        End Try
        Return results



    End Function

    Private Shared Property logPath As String = String.Empty

    'テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Private Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "OwlsAccesslog.Log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub

    'データを暗号化してアクセスログに入れとく
    Private Shared Sub WriteAccessLog(ByVal apesData As String)
        If String.IsNullOrEmpty(apesData) Then Return
        Try
            Dim cryptData As String = ""
            Using crypt As New QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem)
                cryptData = crypt.EncryptString(apesData)
            End Using
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.None, cryptData)
        Catch ex As Exception

        End Try
    End Sub
    'ログイン時のOpenIDとして取得するIDはUri形式でその最後の要素だけをAuSystemIDとして利用する
    Private Shared Function GetAuSystemID(ByVal openid As String) As String
        Dim tmp As String() = openid.Split("/"c)
        Return tmp.Last()
    End Function

    'もしもスペースが含まれてたら、苗字と名前を分けてあげる(多分無理)
    Private Shared Function SeparateFirstAndLastNamesIfPossible(ByVal nameStr As String) As String()
        Dim result(1) As String

        If nameStr.Contains(" ") Then
            result = nameStr.Split(" "c)
        ElseIf nameStr.Contains("　") Then
            result = nameStr.Split("　"c)
        Else
            result(0) = nameStr
            result(1) = ""
        End If
        Return result
    End Function
#End Region

#Region "Public Method"



    ''' <summary>
    ''' ユーザ属性を取得します。これは新規登録時のデフォルト値を用意する目的なので、エラー時リトライしません。
    ''' </summary>
    ''' <param name="openIdFormatAuId">Uri形式のAuIDもしくはWowID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetUserInf(openIdFormatAuId As String) As AuUserInf
        Dim biscuitInf As biscuitif = AuOwlAccessWorker.OwlRequest(GetAuSystemID(openIdFormatAuId))

        Dim results As New AuUserInf()

        If biscuitInf IsNot Nothing AndAlso biscuitInf.resultStatus.Trim() = "0" AndAlso biscuitInf.csAttrib IsNot Nothing Then
            With results
                '名前 
                Dim kanjiNames() As String = SeparateFirstAndLastNamesIfPossible(biscuitInf.csAttrib.nameKanji)
                .FamilyName = kanjiNames(0)
                .GivenName = kanjiNames(1)
                Dim kanaNames() As String = SeparateFirstAndLastNamesIfPossible(biscuitInf.csAttrib.nameKana)
                .FamilyKanaName = kanaNames(0)
                .GivenKanaName = kanaNames(1)
                '生年月日
                Dim yyyyMMddStr As String = biscuitInf.csAttrib.birthday
                If yyyyMMddStr.Length = 8 Then
                    .BirthYear = yyyyMMddStr.Substring(0, 4)
                    .BirthMonth = CInt(yyyyMMddStr.Substring(4, 2)).ToString() '"01"→"1"
                    .BirthDay = CInt(yyyyMMddStr.Substring(6, 2)).ToString()
                End If
                '性別
                Select Case biscuitInf.csAttrib.sex.Trim()
                    Case "1"
                        .Sex = QySexTypeEnum.Male
                    Case "2"
                        .Sex = QySexTypeEnum.Female
                    Case Else
                        .Sex = QySexTypeEnum.None
                End Select
                'メールアドレス
                If String.IsNullOrEmpty(biscuitInf.csAttrib.eMail1) = False AndAlso biscuitInf.csAttrib.eMail1SendFlg.Trim() = "1" Then
                    .MailAddress = biscuitInf.csAttrib.eMail1
                ElseIf String.IsNullOrEmpty(biscuitInf.csAttrib.eMail2) = False AndAlso biscuitInf.csAttrib.eMail2SendFlg.Trim() = "1" Then
                    .MailAddress = biscuitInf.csAttrib.eMail2
                ElseIf String.IsNullOrEmpty(biscuitInf.csAttrib.ezMail) = False AndAlso biscuitInf.csAttrib.ezMailSendFlg.Trim() = "1" Then
                    .MailAddress = biscuitInf.csAttrib.ezMail
                End If
            End With
        End If
        Return results

    End Function

    Public Shared Function IsMobileSubscriberOfAu(openIdFormatAuId As String) As Boolean
        Dim results As biscuitif = OwlRequest(GetAuSystemID(openIdFormatAuId))
        If results IsNot Nothing AndAlso results.resultStatus.Trim() = "0" AndAlso results.auCntrctAttrib IsNot Nothing AndAlso IsNumeric(results.auIdAttrib.auIdLink) AndAlso CInt(results.auIdAttrib.auIdLink) = 1 Then
            For Each item As biscuitifAuCntrctAttrib In results.auCntrctAttrib
                If String.IsNullOrEmpty(item.subscrCd.Trim()) = False Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    ''' <summary>
    ''' 指定日にAuの携帯契約者かどうかを返します。非契約者なら1,契約者なら2,取得に失敗したら9を返します。
    ''' AuIDを取得できた場合はAUIDを引数で返します。
    ''' </summary>
    ''' <param name="openId">他社のOpenID</param>
    ''' <param name="timming"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsMobileSubscriberOfAu(openId As String, timming As String, ByRef auId As String) As Integer
        Dim results As biscuitif
        'If String.IsNullOrEmpty(auId) = False Then
        '    'test
        '    results = AuApesAccessWorker.ApesRequest(auId)

        'Else
        results = OwlRequestByOpenid(openId)

        'End If

        If results Is Nothing OrElse results.resultStatus.Trim() <> "0" Then
            'error
            DebugLog("★契約情報取得失敗")
            Return 9
        Else
            auId = results.auIdAttrib.auId
            '携帯と連動しているIDか
            If results.auCntrctAttrib IsNot Nothing AndAlso IsNumeric(results.auIdAttrib.auIdLink) AndAlso CInt(results.auIdAttrib.auIdLink) = 1 Then   'auIdLink 携帯契約と紐づいているなら
                If results.auCntrctAttrib IsNot Nothing AndAlso results.auCntrctAttrib.Any(Function(m) String.IsNullOrEmpty(m.subscrCd.Trim()) = False) Then '加入者コードが存在する契約属性があるなら
                    '解約日がないか、指定日より未来日になってれば契約者とみなす
                    If results.auCntrctAttrib.Any(Function(m) String.IsNullOrWhiteSpace(m.auKaiyakuDay) OrElse m.auKaiyakuDay.TryToValueType(Of Integer)(0) > timming.TryToValueType(Of Integer)(0)) Then
                        DebugLog("★Auの契約者です")
                        Return 2
                    End If
                End If
            End If
        End If
        DebugLog("★Auの契約者ではないです")
        Return 1
    End Function
#End Region


End Class


