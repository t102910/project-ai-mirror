Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsKaradaKaruteApiCoreV1
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Threading

Friend NotInheritable Class PortalConnectionSettiongWorker

#Region "Constant"

    '連携システム番号
    Public Shared ReadOnly LinkageList As Integer() = {47005, 47006, 47010, 47011}
    Public Shared ReadOnly MedicineLinkageList As Integer() = {47009}
    Public Shared ReadOnly CompanyLinkageList As Integer() = {47100, 11111}

    'Dim url As String = "http://fox233d.dev.navitime.co.jp/iphone_walking/html/joto/redirect?to=cooperationPageJump&jotoId=xxx"
    'Private Shared ReadOnly JOB_URL As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJob")
    'Private Shared ReadOnly JOB_ACCOUNT As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJobAccount")
    'Private Shared ReadOnly JOB_PASSWORD As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJobPassword")
    'Dim jobAccount As String = "$devqolmsjotohdr"
    'Dim jobPassword As String = "TbMEDvyGgxl2wHG83ghuaCGgB8dT6xKy7Yeoa6yujr8YSeRe67uFs9vdlD6Q"


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

    Private Shared Function ExecuteConnectionSettingReadApi(mainModel As QolmsYappliModel) As QhYappliPortalConnectionSettingReadApiResults

        Dim apiArgs As New QhYappliPortalConnectionSettingReadApiArgs(
            QhApiTypeEnum.YappliPortalConnectionSettingRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = LinkageList.ToList().ConvertAll(Function(i) i.ToString()),
            .MedicineLinkageSystemNo = MedicineLinkageList.ToList().ConvertAll(Function(i) i.ToString()),
            .CompanyLinkageSystemNo = CompanyLinkageList.ToList().ConvertAll(Function(i) i.ToString())
        }
        Dim apiResults As QhYappliPortalConnectionSettingReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalConnectionSettingReadApiResults)(
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


    ' ''' <summary>
    ' ''' JOBを実行します。
    ' ''' </summary>
    ' ''' <param name="paramString"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function ExecuteQolmsKaradaKaruteFirstJob(jobUri As String, jobUser As String, jobPassword As String, paramString As String) As Boolean
    '    Try
    '        'HttpWebRequestの作成
    '        Dim webreq As System.Net.HttpWebRequest = _
    '            DirectCast(System.Net.WebRequest.Create(jobUri & "?arguments=" & paramString),  _
    '                System.Net.HttpWebRequest)
    '        'PreAuthenticateプロパティを設定
    '        webreq.PreAuthenticate = True

    '        'Basic認証の設定
    '        webreq.Credentials = New System.Net.NetworkCredential(jobUser, jobPassword)
    '        webreq.Method = "POST"
    '        webreq.ContentLength = 0
    '        'HttpWebResponseの取得
    '        Dim webres As System.Net.HttpWebResponse = DirectCast(webreq.GetResponse(), System.Net.HttpWebResponse)


    '    Catch exh As System.Net.WebException
    '        DebugLog(exh.Message)

    '        Dim str As New StringBuilder()
    '        str.AppendLine(EncriptString(exh.Message))
    '        str.AppendLine(paramString)
    '        'メール
    '        PortalConnectionSettiongWorker.SendMail(str.ToString())
    '        'アクセスログ
    '        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(exh.Message))

    '        Return False
    '    Catch ex As Exception
    '        DebugLog(ex.Message)
    '        'メール
    '        Dim str As New StringBuilder()
    '        str.AppendLine(EncriptString(ex.Message))
    '        str.AppendLine(paramString)
    '        PortalConnectionSettiongWorker.SendMail(str.ToString())

    '        'アクセスログ
    '        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))

    '        Return False
    '    End Try

    '    Return True

    'End Function

    Private Shared Function EncriptString(str As String) As String

        If String.IsNullOrWhiteSpace(str) Then Return String.Empty

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            Return crypt.EncryptString(str)
        End Using

    End Function

    Private Shared Function DecriptString(str As String) As String

        If String.IsNullOrWhiteSpace(str) Then Return String.Empty

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            Return crypt.DecryptString(str)
        End Using

    End Function

    Private Shared Sub SendMail(message As String)

        Dim br As New StringBuilder()
        br.AppendLine(String.Format("タニタ接続のエラーです。"))
        br.AppendLine(message)
        Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(br.ToString())

    End Sub


    ''テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "tanita.txt")
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, fromPageNo As QyPageNoTypeEnum, tabNo As Byte) As PortalConnectionSettingViewModel

        Dim result As New PortalConnectionSettingViewModel() With {
            .FromPageNoType = fromPageNo,
            .TabNoType = tabNo
            }
        '画面に必要根情報を取得する
        'アカウントキーを指定して連携情報を取得
        Dim apiResult As QhYappliPortalConnectionSettingReadApiResults = PortalConnectionSettiongWorker.ExecuteConnectionSettingReadApi(mainModel)
        With apiResult

            For Each item As QhApiLinkageItem In .LinkageItems
                result.ConnectionSettingItems.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingItem() With {
                                                  .LinkageNo = Integer.Parse(item.LinkageSystemNo),
                                                  .Devices = item.Devices,
                                                  .Tags = item.Tags,
                                                  .Status = Byte.Parse(item.StatusType)
                                              })
            Next

            For Each item As QhApiLinkageItem In .ConnectionSettingHospitalItemN
                result.ConnectionSettingHospitalItemN.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingHospitalItem() With {
                                                  .LinkageSystemNo = Integer.Parse(item.LinkageSystemNo),
                                                  .LinkageSysyemName = item.LinkageSystemName,
                                                  .Status = item.StatusType.TryToValueType(Byte.MinValue)
                                              })
            Next

            For Each item As QhApiLinkageItem In .ConnectionSettingMedicineItemN
                result.ConnectionSettingPharmacyItemN.Add(Guid.Parse(item.Facilitykey), New ConnectionSettingPharmacyItem() With {
                                                  .LinkageSystemNo = Integer.Parse(item.LinkageSystemNo),
                                                  .LinkageSysyemName = item.LinkageSystemName,
                                                  .Status = item.StatusType.TryToValueType(Byte.MinValue)
                                              })
            Next

            For Each item As QhApiLinkageItem In .ConnectionSettingCompanyItemN
                result.ConnectionSettingCompanyItemN.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingCompanyItem() With {
                                                  .LinkageSystemNo = Integer.Parse(item.LinkageSystemNo),
                                                  .LinkageSysyemName = item.LinkageSystemName,
                                                  .Status = item.StatusType.TryToValueType(Byte.MinValue)
                                              })
            Next

        End With

        Return result
    End Function
    
    ''' <summary>
    ''' 連携設定画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetLinkageLists(mainModel As QolmsYappliModel) As Dictionary(Of Integer,ConnectionSettingItem)

        Dim result As New Dictionary(Of Integer,ConnectionSettingItem)

        Dim apiResult As QhYappliPortalConnectionSettingReadApiResults = PortalConnectionSettiongWorker.ExecuteConnectionSettingReadApi(mainModel)
        With apiResult
            '後でConteinsKeyのチェックする
            For Each item As QhApiLinkageItem In .LinkageItems
                result.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingItem() With {
                                        .LinkageNo = Integer.Parse(item.LinkageSystemNo),
                                        .Devices = item.Devices,
                                        .Tags = item.Tags,
                                        .Status = Byte.Parse(item.StatusType)
                                    })
            Next

            For Each item As QhApiLinkageItem In .ConnectionSettingHospitalItemN
                result.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingItem() With {
                                        .LinkageNo = Integer.Parse(item.LinkageSystemNo),
                                        .Devices = item.Devices,
                                        .Tags = item.Tags,
                                        .Status = Byte.Parse(item.StatusType)
                                    })
            Next

            For Each item As QhApiLinkageItem In .ConnectionSettingMedicineItemN
                result.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingItem() With {
                                        .LinkageNo = Integer.Parse(item.LinkageSystemNo),
                                        .Devices = item.Devices,
                                        .Tags = item.Tags,
                                        .Status = Byte.Parse(item.StatusType)
                                    })
            Next

            For Each item As QhApiLinkageItem In .ConnectionSettingCompanyItemN
                result.Add(Integer.Parse(item.LinkageSystemNo), New ConnectionSettingItem() With {
                                        .LinkageNo = Integer.Parse(item.LinkageSystemNo),
                                        .Devices = item.Devices,
                                        .Tags = item.Tags,
                                        .Status = Byte.Parse(item.StatusType)
                                    })
            Next

        End With

        Return result
    End Function

#End Region

End Class