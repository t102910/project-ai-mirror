Imports System.Threading.Tasks
Imports System.Web.Configuration
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsNoticeApiCoreV1

Friend NotInheritable Class NoticeMailWorker

#Region "Constant"

    Private Const SETTING_NOTICE_SETTINGS_NAME As String = "SysNoticeSettingsName"

    Private Const SETTING_NOTICE_SUBJECT As String = "SysNoticeSubject"

    Private Const SETTING_NOTICE_TO As String = "SysNoticeTo"

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

    Private Shared Function GetAppSettings() As Dictionary(Of String, String)

        Dim result As New Dictionary(Of String, String)()

        For Each key As String In {
            NoticeMailWorker.SETTING_NOTICE_SETTINGS_NAME,
            NoticeMailWorker.SETTING_NOTICE_SUBJECT,
            NoticeMailWorker.SETTING_NOTICE_TO
        }

            Dim value As String = String.Empty

            Try
                value = ConfigurationManager.AppSettings(key).Trim()
            Catch ex As Exception
                Debug.Print(ex.Message)
            End Try

            If Not String.IsNullOrWhiteSpace(value) Then result.Add(key, value)
        Next

        Return result

    End Function

    Private Shared Function ExecuteQolmsNoticeApi(args As QnMailSendApiArgs) As QnMailSendApiResults

        Dim result As New QnMailSendApiResults() With {.IsSuccess = Boolean.FalseString}

        Try
            result = QsNoticeApiManager.ExecuteQolmsNoticeApi(Of QnMailSendApiResults)(args)
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        Return result

    End Function

#End Region

#Region "Public Method"

    <Obsolete("実装中")>
    Public Shared Function Send(body As String) As Boolean

        Dim result As Boolean = False

        If Not String.IsNullOrWhiteSpace(body) Then
            Dim settings As Dictionary(Of String, String) = NoticeMailWorker.GetAppSettings()

            If settings.ContainsKey(NoticeMailWorker.SETTING_NOTICE_SETTINGS_NAME) _
                AndAlso settings.ContainsKey(NoticeMailWorker.SETTING_NOTICE_SUBJECT) _
                AndAlso settings.ContainsKey(NoticeMailWorker.SETTING_NOTICE_TO) Then

                Dim toN As List(Of String) = settings(NoticeMailWorker.SETTING_NOTICE_TO).Split({","}, StringSplitOptions.RemoveEmptyEntries).Where(Function(i) Not String.IsNullOrWhiteSpace(i)).Select(Function(i) i.Trim()).ToList()

                If toN.Any() Then
                    Dim args As New QnMailSendApiArgs() With {
                        .ApiType = QnApiTypeEnum.MailSend.ToString(),
                        .ExecuteSystemType = QsApiSystemTypeEnum.Qolms.ToString(),
                        .Executor = Guid.Empty.ToApiGuidString(),
                        .ExecutorName = "JOTOホームドクター",
                        .ExecuteApplicationType = "None",
                        .NoticeSettingsName = settings(NoticeMailWorker.SETTING_NOTICE_SETTINGS_NAME),
                        .ToN = toN,
                        .Subject = settings(NoticeMailWorker.SETTING_NOTICE_SUBJECT),
                        .Body = body.Trim()
                    }

                    result = NoticeMailWorker.ExecuteQolmsNoticeApi(args).IsSuccess.TryToValueType(False)
                End If
            End If
        End If

        Return result

    End Function

    <Obsolete("実装中")>
    Public Shared Async Function SendAsync(body As String) As Task(Of Boolean)

        Return Await Task.Run(Of Boolean)(
            Function()
                Return NoticeMailWorker.Send(body)
            End Function
        )

    End Function

      ''' <summary>
      ''' メールアドレスとタイトルと本文を指定して、メールを送信します。
      ''' </summary>
      ''' <param name="toMailAddress"></param>
      ''' <param name="subject"></param>
      ''' <param name="body"></param>
      ''' <returns></returns>
    <Obsolete("実装中")>
    Public Shared Function SendUser(toMailAddress As string,subject As String ,body As String) As Boolean

        Dim result As Boolean = False

        If Not String.IsNullOrWhiteSpace(body) Then
            Dim settings As Dictionary(Of String, String) = NoticeMailWorker.GetAppSettings()

            If settings.ContainsKey(NoticeMailWorker.SETTING_NOTICE_SETTINGS_NAME) _
                AndAlso settings.ContainsKey(NoticeMailWorker.SETTING_NOTICE_SUBJECT) _
                AndAlso settings.ContainsKey(NoticeMailWorker.SETTING_NOTICE_TO) Then

                Dim toN As List(Of String) = settings(NoticeMailWorker.SETTING_NOTICE_TO).Split({","}, StringSplitOptions.RemoveEmptyEntries).Where(Function(i) Not String.IsNullOrWhiteSpace(i)).Select(Function(i) i.Trim()).ToList()

                If toN.Any() Then
                    Dim args As New QnMailSendApiArgs() With {
                        .ApiType = QnApiTypeEnum.MailSend.ToString(),
                        .ExecuteSystemType = QsApiSystemTypeEnum.Qolms.ToString(),
                        .Executor = Guid.Empty.ToApiGuidString(),
                        .ExecutorName = "JOTOホームドクター",
                        .ExecuteApplicationType = "None",
                        .NoticeSettingsName = settings(NoticeMailWorker.SETTING_NOTICE_SETTINGS_NAME),
                        .ToN = {toMailAddress}.ToList(),
                        .Subject = subject,
                        .Body = body.Trim()
                    }

                    result = NoticeMailWorker.ExecuteQolmsNoticeApi(args).IsSuccess.TryToValueType(False)
                End If
            End If
        End If

        Return result

    End Function

    <Obsolete("実装中")>
    Public Shared Async Function SendUserAsync(toMailAddress As string,subject As String ,body As String) As Task(Of Boolean)

        Return Await Task.Run(Of Boolean)(
            Function()
                Return NoticeMailWorker.SendUser(toMailAddress,subject,body)
            End Function
        )

    End Function
#End Region

End Class
