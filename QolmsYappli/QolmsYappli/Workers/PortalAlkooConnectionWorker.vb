Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsKaradaKaruteApiCoreV1
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Threading
Imports MGF.QOLMS.QolmsAlkooApiCoreV1
Imports System.Globalization

Friend NotInheritable Class PortalAlkooConnectionWorker

#Region "Constant"

    '連携システム番号
    Private Const TANITA_SYSTEM_NO As Integer = 47005
    Private Const ALKOO_SYSTEM_NO As Integer = 47006

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


    ''' <summary>
    ''' 設定名を指定して設定値を取得します。
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting(name As String) As String

        Return ConfigurationManager.AppSettings(name)

    End Function


    Private Shared Function ExecuteAlkooConnectionCheckApi(id As String) As checkApiResults

        Dim apiArgs As New checkApiArgs() With {
            .joto_id_list = {id}.ToList()
        }
        Dim apiResults As checkApiResults = QsAlkooApiManager.Execute(Of checkApiArgs, checkApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "Alkoo"))
            End If
        End With

    End Function

    Public Shared Function ExecuteAlkooConnectionCancel(id As String) As cancelApiResults

        Dim apiArgs As New cancelApiArgs() With {
            .joto_id_list = {id}.ToList()
        }
        Dim apiResults As cancelApiResults = QsAlkooApiManager.Execute(Of cancelApiArgs, cancelApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "Alkoo"))
            End If
        End With

    End Function

    Public Shared Function ExecuteAlkooGetRecords(id As String, startDate As Date, endDate As Date) As recordsApiResults

        Dim apiArgs As New recordsApiArgs() With {
            .joto_id = id,
            .start_date = Integer.Parse(startDate.ToString("yyyyMMdd")),
            .end_date = Integer.Parse(endDate.ToString("yyyyMMdd")),
            .dimension = DimensionTypeEnum.date.ToDimensionString()
        }
        Dim apiResults As recordsApiResults = QsAlkooApiManager.Execute(Of recordsApiArgs, recordsApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "Alkoo"))
            End If
        End With

    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="targetStartDate">対象日。</param>
    ''' <param name="targetEndDate">対象日。</param>
    ''' <param name="onlyUpdateSteps">更新されたものが対象かどうか。</param>
    ''' <param name="jotoIdN">取得対象のJOTOIDのリスト</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function ExecuteAlkooBulkApi(targetStartDate As Integer, targetEndDate As Integer, onlyUpdateSteps As Boolean, jotoIdN As List(Of String)) As bulkApiResults

        'Dim targetStartDay As Integer = settings(ConstantDefinition.SETTING_TARGET_STARTDAY).TryToValueType(0)
        'Dim targetEndDay As Integer = settings(ConstantDefinition.SETTING_TARGET_ENDDAY).TryToValueType(-7)
        'Dim onlyUpdateSteps As Boolean = settings(ConstantDefinition.SETTING_ONLY_UPDATE_STEPS).TryToValueType(True)

        'ローカルデバッグ
        '#If DEBUG Then
        '        Return New bulkApiResults() With {
        '            .IsSuccess = True,
        '            .StatusCode = StatusTypeEnum.OK,
        '            .result = New StepsResultOfJson() With {
        '                .status = StatusTypeEnum.OK.ToStatusString(),
        '                .message_id = "S0101",
        '                .message = "歩数データの取得に成功しました。",
        '                .count_OK = 2,
        '                .count_NG = 0,
        '                .steps_list = New List(Of StepsDataOfJson)() From {
        '                    New StepsDataOfJson() With {
        '                        .status = StatusTypeEnum.OK.ToStatusString(),
        '                        .message_id = "S0101",
        '                        .message = "歩数データの取得に成功しました。",
        '                        .joto_id = "65c78ac46646432680a5e40991ab5dd7",
        '                    .users_steps_list = New List(Of StepsOfJson)() From {New StepsOfJson() With {.date = 20200618, .steps = 10000}, New StepsOfJson() With {.date = 20200619, .steps = 5000}}
        '                    },
        '                    New StepsDataOfJson() With {
        '                        .status = StatusTypeEnum.OK.ToStatusString(),
        '                        .message_id = "S0101",
        '                        .message = "歩数データの取得に成功しました。",
        '                        .joto_id = "12345",
        '                    .users_steps_list = New List(Of StepsOfJson)() From {New StepsOfJson() With {.date = 20200616, .steps = 6000}, New StepsOfJson() With {.date = 20200619, .steps = 5000}}
        '                    }
        '                }
        '            }
        '        }
        '#End If

        DebugLog("ExecuteAlkooBulkApi")
        DebugLog(jotoIdN.First())

        Dim apiArgs As New bulkApiArgs() With {
            .start_date = targetStartDate,
            .end_date = targetEndDate,
            .only_updated_steps = onlyUpdateSteps,
            .joto_id_list = jotoIdN
        }
        Dim apiResults As bulkApiResults = QsAlkooApiManager.Execute(Of bulkApiArgs, bulkApiResults)(apiArgs)

        With apiResults
            DebugLog("apiResults")
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .RequestString)
            DebugLog(.RequestString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            DebugLog(.ResponseString)
            If .IsSuccess Then
                'LogWorker.WriteLog(.RequestString)
                'LogWorker.WriteLog(.ResponseString)
                Return apiResults
            Else
                Return apiResults
            End If
        End With

    End Function


    Private Shared Function ExecuteAlkooConnectionReadApi(mainModel As QolmsYappliModel, linkageNoList As List(Of String)) As QhYappliPortalAlkooConnectionReadApiResults

        Dim apiArgs As New QhYappliPortalAlkooConnectionReadApiArgs(
            QhApiTypeEnum.YappliPortalAlkooConnectionRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = linkageNoList
        }
        Dim apiResults As QhYappliPortalAlkooConnectionReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAlkooConnectionReadApiResults)(
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


    Private Shared Function ExecuteAlkooConnectionWriteApi(mainModel As QolmsYappliModel, id As String, StatusType As Byte, deleteFlag As Boolean) As QhYappliPortalAlkooConnectionWriteApiResults

        Dim apiArgs As New QhYappliPortalAlkooConnectionWriteApiArgs(
            QhApiTypeEnum.YappliPortalAlkooConnectionWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = ALKOO_SYSTEM_NO.ToString(),
            .LinkageSystemId = id,
            .StatusType = StatusType.ToString(),
            .DeleteFlag = deleteFlag.ToString()
        }
        Dim apiResults As QhYappliPortalAlkooConnectionWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAlkooConnectionWriteApiResults)(
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
        br.AppendLine(String.Format("ALKOO接続のエラーです。"))
        br.AppendLine(message)
        Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(br.ToString())

    End Sub

    Private Shared Function ExceptionString(mainModel As QolmsYappliModel, ex As Exception) As String

        Dim message As New StringBuilder()
        message.AppendLine("ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString())
        message.AppendLine(ex.Source)

        Dim exp As Exception = ex
        For i As Integer = 0 To 3
            message.AppendLine(ex.Message)
            message.AppendLine(ex.StackTrace)

            If exp.InnerException Is Nothing Then
                Exit For
            Else
                exp = ex.InnerException
            End If
        Next

        Return message.ToString()

    End Function

    ''テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/Log"), "Alkoo.log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Public Method"

    Public Shared Function GetConnected(mainModel As QolmsYappliModel, ByRef id As String) As Boolean

        Dim apiResult As QhYappliPortalAlkooConnectionReadApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionReadApi(mainModel, {ALKOO_SYSTEM_NO.ToString()}.ToList())
        With apiResult
            If .LinkageItems.Count > 0 Then
                id = .LinkageItems.First().LinkageSystemId
                If .LinkageItems.First().LinkageSystemNo = ALKOO_SYSTEM_NO.ToString() AndAlso .LinkageItems.First().StatusType = "2" Then
                    Return True
                End If
            End If
        End With
        Return False

    End Function


    Public Shared Function GetSteps(mainModel As QolmsYappliModel, getDate As Date) As Integer

        'ID取得
        Dim apiResult As QhYappliPortalAlkooConnectionReadApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionReadApi(mainModel, {ALKOO_SYSTEM_NO.ToString()}.ToList())

        Dim id As String = String.Empty
        With apiResult
            If .LinkageItems.Count > 0 Then
                If .LinkageItems.First().LinkageSystemNo = ALKOO_SYSTEM_NO.ToString() AndAlso .LinkageItems.First().StatusType = "2" Then
                    id = .LinkageItems.First().LinkageSystemId
                End If
            End If
        End With

        DebugLog("ID:")
        DebugLog(id)

        '歩数取得
        If Not String.IsNullOrWhiteSpace(id) Then
            Try
                'api
                Dim records As recordsApiResults = PortalAlkooConnectionWorker.ExecuteAlkooGetRecords(id, getDate, getDate)
                DebugLog("ResponseString:")
                DebugLog(records.ResponseString)

                With records
                    If .result.status = StatusTypeEnum.OK.ToStatusString() AndAlso .result.steps_list.First().status = StatusTypeEnum.OK.ToStatusString() Then
                        Return .result.steps_list.First().steps
                    End If

                End With
                If records.result.message_id.StartsWith("E00") Then
                    'メール
                    PortalAlkooConnectionWorker.SendMail(EncriptString("GetSteps:" & records.ResponseString) & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString())
                    'アクセスログ
                    AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("GetSteps:" & records.ResponseString & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString()))
                End If
            Catch ex As Exception

                'メール
                PortalAlkooConnectionWorker.SendMail(EncriptString(PortalAlkooConnectionWorker.ExceptionString(mainModel, ex)))
                'アクセスログ
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, PortalAlkooConnectionWorker.ExceptionString(mainModel, ex))


            End Try
        End If

        Return 0


    End Function

    ''' <summary>
    ''' 指定された期間のALKOOの歩数を取得して日付ごとのディクショナリーを返します。。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="startDate"></param>
    ''' <param name="endDate"></param>
    ''' <param name="onlyUpdateSteps"></param>
    ''' <returns>key:日付 vlaue:歩数</returns>
    ''' <remarks></remarks>
    Public Shared Function GetBulkSteps(mainModel As QolmsYappliModel, id As String, startDate As Date, endDate As Date, onlyUpdateSteps As Boolean) As Dictionary(Of Date, Decimal)

        'ID取得
        Dim result As New Dictionary(Of Date, Decimal)()

        '歩数取得
        If Not String.IsNullOrWhiteSpace(id) Then
            Try

                Dim startValue As Integer = Integer.Parse(startDate.ToString("yyyyMMdd")) '日付をIntegerへ
                Dim endValue As Integer = Integer.Parse(endDate.ToString("yyyyMMdd")) '日付をIntegerへ
                Dim records As bulkApiResults = PortalAlkooConnectionWorker.ExecuteAlkooBulkApi(startValue, endValue, onlyUpdateSteps, {id}.ToList())

                With records
                    If .result.status = StatusTypeEnum.OK.ToStatusString() AndAlso .result.steps_list.First().status = StatusTypeEnum.OK.ToStatusString() Then

                        Dim ci As CultureInfo = CultureInfo.CurrentCulture
                        Dim dts As DateTimeStyles = DateTimeStyles.None

                        For Each item As StepsDataOfJson In .result.steps_list
                            For Each item2 As StepsOfJson In item.users_steps_list

                                Dim day As Date = Date.MinValue
                                If Date.TryParseExact(item2.date.ToString(), "yyyyMMdd", ci, dts, day) AndAlso item2.steps > 0 Then
                                    DebugLog(item2.date.ToString() + ":" + item2.steps.ToString)
                                    result.Add(day, item2.steps)
                                End If
                            Next
                        Next
                    End If

                End With
                If records.result.message_id.StartsWith("E00") Then
                    'メール
                    PortalAlkooConnectionWorker.SendMail(EncriptString("GetSteps:" & records.ResponseString) & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString())
                    'アクセスログ
                    AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("GetSteps:" & records.ResponseString & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString()))
                End If
            Catch ex As Exception

                'メール
                PortalAlkooConnectionWorker.SendMail(EncriptString(PortalAlkooConnectionWorker.ExceptionString(mainModel, ex)))
                'アクセスログ
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, PortalAlkooConnectionWorker.ExceptionString(mainModel, ex))


            End Try
        End If

        Return result


    End Function

    Public Shared Function AlkooConnectionCheck(mainModel As QolmsYappliModel, id As String) As Boolean
        Try
            'api
            Dim checkResult As checkApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionCheckApi(id)
            DebugLog(checkResult.ResponseString)
            DebugLog(checkResult.result.message_id)
            DebugLog(checkResult.result.status_list.First().message_id)
            If checkResult.result.message_id.StartsWith("S") AndAlso checkResult.result.status_list.First().message_id.StartsWith("S") Then
                '接続確認成功
                'update
                'DebugLog("成功")
                PortalAlkooConnectionWorker.ExecuteAlkooConnectionWriteApi(mainModel, id, 2, False)
                Return True
            Else
                If checkResult.result.message_id.StartsWith("E00") Then
                    'todo :ログを詳細にする
                    'メール
                    PortalAlkooConnectionWorker.SendMail(EncriptString("AlkooConnectionCheck:" & checkResult.ResponseString & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString()))
                    'アクセスログ
                    AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, "AlkooConnectionCheck:" & checkResult.ResponseString & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString())
                End If
                Return False
            End If
        Catch ex As Exception

            'メール
            PortalAlkooConnectionWorker.SendMail(EncriptString(PortalAlkooConnectionWorker.ExceptionString(mainModel, ex)))
            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, PortalAlkooConnectionWorker.ExceptionString(mainModel, ex))
            Return False

        End Try

    End Function

    ''' <summary>
    ''' ALKOO連携画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalAlkooConnectionViewModel

        Dim result As New PortalAlkooConnectionViewModel()
        '画面に必要根情報を取得する
        'アカウントキーを指定して連携情報を取得
        Dim apiResult As QhYappliPortalAlkooConnectionReadApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionReadApi(mainModel, {ALKOO_SYSTEM_NO.ToString(), TANITA_SYSTEM_NO.ToString()}.ToList())
        With apiResult

            For Each item As QhApiLinkageItem In .LinkageItems
                If item.LinkageSystemNo = ALKOO_SYSTEM_NO.ToString() Then
                    If item.StatusType = "2" Then
                        'ALKOO連携
                        result.AlkooConnectedFlag = True
                    Else
                        'Alkoo接続確認
                        'Dim checkResult As checkApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionCheckApi(item.LinkageSystemId)
                        'DebugLog(checkResult.ResponseString)
                        'If checkResult.result.message_id.StartsWith("S") AndAlso checkResult.result.status_list.First().message_id.StartsWith("S") Then
                        '    '接続確認成功
                        '    'update
                        '    PortalAlkooConnectionWorker.ExecuteAlkooConnectionWriteApi(mainModel, item.LinkageSystemId, 2, False)
                        '    result.AlkooConnectedFlag = True
                        'End If
                        result.AlkooConnectedFlag = PortalAlkooConnectionWorker.AlkooConnectionCheck(mainModel, item.LinkageSystemId)

                    End If

                ElseIf item.LinkageSystemNo = TANITA_SYSTEM_NO.ToString() AndAlso item.Devices.Contains(DirectCast(QsKaradaKaruteApiDeviceTypeEnum.Pedometer, Byte).ToString()) Then
                    result.TanitaWalkConnectedFlag = True
                End If
            Next
        End With

        Return result
    End Function

    ''' <summary>
    ''' ALKOO連携情報を登録する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ConnectionRegister(mainModel As QolmsYappliModel) As String ', ByRef message As String,


        Dim apiResult As QhYappliPortalAlkooConnectionWriteApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionWriteApi(mainModel, "", 1, False)
        With apiResult

            If .IsSuccess.TryToValueType(False) AndAlso Not String.IsNullOrWhiteSpace(.LinkageSystemId) Then
                Return .LinkageSystemId
            Else
                Return String.Empty
            End If
        End With

    End Function

    ''' <summary>
    ''' 連携を解除する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Cancel(mainModel As QolmsYappliModel, ByRef message As String) As Boolean

        '連携情報取得
        Dim apiResult As QhYappliPortalAlkooConnectionReadApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionReadApi(mainModel, {ALKOO_SYSTEM_NO.ToString()}.ToList())

        If apiResult.LinkageItems.Count = 0 Then
            Return True
        End If

        '連携を解除
        Dim cancelResult As New cancelApiResults()
        Try
            cancelResult = PortalAlkooConnectionWorker.ExecuteAlkooConnectionCancel(apiResult.LinkageItems.First().LinkageSystemId)
            DebugLog(cancelResult.ResponseString)

        Catch ex As Exception
            'メール
            PortalAlkooConnectionWorker.SendMail(EncriptString(PortalAlkooConnectionWorker.ExceptionString(mainModel, ex)))
            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, PortalAlkooConnectionWorker.ExceptionString(mainModel, ex))
            message = ex.Message
            Return False
        End Try
        DebugLog(cancelResult.result.message_id)
        DebugLog(cancelResult.result.status_list.First().message_id)
        If cancelResult.result.message_id.StartsWith("S") AndAlso cancelResult.result.status_list.First().message_id.StartsWith("S") Then
            '{"result": {"status": "NG", "message_id": "S0401", "message": "\u9023\u643a\u89e3\u9664\u306b\u6210\u529f\u3057\u307e\u3057\u305f\u3002", "count_OK": 0, "count_NG": 1, "status_list": [{"joto_id": "1c791aedb8c7439f8d3241c6c0bdc984", "status": "OK", "message_id": "E0402", "message": "\u672a\u9023\u643a\u306eID\u3067\u3059\u3002"}]}}
            '解除成功
            'DebugLog("成功")
            message = "成功"
            'Linkageを未連携に更新
            Dim writerResult As QhYappliPortalAlkooConnectionWriteApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionWriteApi(mainModel, String.Empty, 2, True)
            DebugLog("Delete" & writerResult.LinkageSystemId)
            Return writerResult.IsSuccess.TryToValueType(False)
        Else
            If cancelResult.result.message_id.StartsWith("E00") Then
                'メール
                PortalAlkooConnectionWorker.SendMail(EncriptString("Cancel:" & cancelResult.ResponseString & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString()))
                'アクセスログ
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("Cancel:" & cancelResult.ResponseString & "ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString()))
            End If
            'DebugLog("Not Delete")
            message = cancelResult.result.status_list.First().message
            Return False
        End If

    End Function

    ''' <summary>
    ''' ALKOO連携情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAlkooId(mainModel As QolmsYappliModel) As String

        Dim apiResult As QhYappliPortalAlkooConnectionReadApiResults = PortalAlkooConnectionWorker.ExecuteAlkooConnectionReadApi(mainModel, {ALKOO_SYSTEM_NO.ToString()}.ToList())
        With apiResult
            If .LinkageItems.Any() AndAlso Not String.IsNullOrWhiteSpace(.LinkageItems.First().LinkageSystemId) Then

                Return .LinkageItems.First().LinkageSystemId
            Else
                Return String.Empty

            End If

        End With

    End Function


    Public Shared Function DynamicLink(mainModel As QolmsYappliModel, urlSetting As String, code As String, ginowanjoin As String) As String

        Dim result As String = String.Empty

        '設定からURLを取得する
        Dim url As String = PortalAlkooConnectionWorker.GetSetting(urlSetting)

        If Not String.IsNullOrWhiteSpace(url) Then

            ' Alkoo連携と jotoidを取得する
            Dim jotoid As String = PortalAlkooConnectionWorker.GetAlkooId(mainModel)

            If Not String.IsNullOrWhiteSpace(jotoid) Then

                Dim str As String = $"{url}?jotoid={jotoid}"

                If Not String.IsNullOrWhiteSpace(code) Then
                    str += $"&code={code}"
                End If

                If Not String.IsNullOrWhiteSpace(ginowanjoin) AndAlso ginowanjoin.TryToValueType(False) Then

                    With LinkageWorker.GetLinkageItem(mainModel, 47900021)

                        str += $"&ginowan={ (.StatusType = 2).ToString().ToLower()}"

                    End With

                End If

                result = str
            Else
                result = "../portal/alkooconnection"
            End If

        Else
            '設定がない
            Throw New Exception("ALKOO DynamicLink の設定がありません。")

        End If

        Return result

    End Function

    ''' <summary>
    ''' ALKOO連携がある場合、ALKOOの歩数画面へ遷移するリダイレクトURLを生成します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <returns></returns>
    Public Shared Function CreateWalkRedirectUrl(mainModel As QolmsYappliModel) As String

        Dim id As String = String.Empty
        Dim url As String = ConfigurationManager.AppSettings("AlkooRedirectUrl")
        Dim alkooConnected As Boolean = PortalAlkooConnectionWorker.GetConnected(mainModel, id)

        If String.IsNullOrWhiteSpace(url) OrElse String.IsNullOrWhiteSpace(id) Then

            '連携なし/設定なしパターン
            Return String.Empty
        ElseIf Not alkooConnected AndAlso Not PortalAlkooConnectionWorker.AlkooConnectionCheck(mainModel, id) Then

            '未連携申請中パターン
            Return String.Empty

        End If

        '連携済みパターン
        Task.Run(
            Sub()
                'ナビタイムのサーバー側から歩数を取得して同期
                NoteWalkWorker.SynkAlkooSteps(mainModel, id)
            End Sub
        )

        With LinkageWorker.GetLinkageItem(mainModel, 47900022)

            Return $"{url}{id}&ginowan={(.StatusType = 2).ToString().ToLower()}" 'Redirect(url & id)

        End With

    End Function

#End Region

End Class