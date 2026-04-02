Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsKaradaKaruteApiCoreV1
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Threading
Imports System.Net
Imports System.IO

Friend NotInheritable Class PortalTanitaConnectionWorker

#Region "Constant"

    '連携システム番号
    Private Const TANITA_SYSTEM_NO As Integer = 47005
    Private Const ALKOO_SYSTEM_NO As Integer = 47006
    Private Shared ReadOnly JOB_URL As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJob")
    Private Shared ReadOnly JOB_ACCOUNT As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJobAccount")
    Private Shared ReadOnly JOB_PASSWORD As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJobPassword")
    'Dim url As String = "https://devqolmsjotohdr.scm.azurewebsites.net/api/triggeredwebjobs/QolmsKaradaKaruteFirstJob/run"
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

    Private Shared Function ExecuteKaradaKaruteConnectionApi(loginId As String, loginPassword As String) As MemberAuthApiResults

        Dim apiArgs As New MemberAuthApiArgs() With {
            .siteId = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSiteId,
            .sitePass = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSitePassword,
            .loginId = loginId,
            .loginPass = loginPassword,
            .mode = "0"
        }
        Dim apiResults As MemberAuthApiResults = QsKaradaKaruteApiManager.Execute(Of MemberAuthApiArgs, MemberAuthApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then
                Select Case .status

                    Case "0" ' "0"：正常。

                    Case "1" ' "1"：非会員。
                        .message = "IDまたはパスワードが間違っています。"
                    Case "-1" ' "-1"：パラメータ 異常。
                        .message = "パラメータ 異常"

                    Case "-2" ' "-2"：データベース 異常。
                        .message = "データベース 異常"

                    Case "-3" ' "-3"：認証 エラー。
                        .message = "IDまたはパスワードが間違っています。"

                    Case "-4" ' "-4"：その他 エラー。
                        .message = "その他 エラー"

                End Select
                Return apiResults

            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "KaradaKarute"))
            End If
        End With

    End Function

    Public Shared Function ExecuteKaradaKaruteCancel(memberNo As String) As MemberAuthApiResults

        Dim apiArgs As New MemberAuthApiArgs() With {
            .siteId = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSiteId,
            .sitePass = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSitePassword,
            .memberNo = memberNo,
            .mode = "1"
        }
        Dim apiResults As MemberAuthApiResults = QsKaradaKaruteApiManager.Execute(Of MemberAuthApiArgs, MemberAuthApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then
                Select Case .status
                    Case "0" ' "0"：正常。

                    Case "1" ' "1"：非会員。
                        .message = "IDまたはパスワードが間違っています。"
                    Case "-1" ' "-1"：パラメータ 異常。
                        .message = "パラメータ 異常"

                    Case "-2" ' "-2"：データベース 異常。
                        .message = "データベース 異常"

                    Case "-3" ' "-3"：認証 エラー。
                        .message = "IDまたはパスワードが間違っています。"

                    Case "-4" ' "-4"：その他 エラー。
                        .message = "その他 エラー"

                End Select
                Return apiResults

            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "KaradaKarute"))
            End If
        End With

    End Function

    Private Shared Function ExecuteTanitaConnectionReadApi(mainModel As QolmsYappliModel, SystemNoList As List(Of String)) As QhYappliPortalTanitaConnectionReadApiResults

        Dim apiArgs As New QhYappliPortalTanitaConnectionReadApiArgs(
            QhApiTypeEnum.YappliPortalTanitaConnectionRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = SystemNoList
        }
        Dim apiResults As QhYappliPortalTanitaConnectionReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTanitaConnectionReadApiResults)(
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


    Private Shared Function ExecuteTanitaConnectionWriteApi(mainModel As QolmsYappliModel, inputModel As PortalTanitaConnectionInputModel, linkageId As String, devices As List(Of Byte), tags As List(Of String), StatusType As Byte, deleteFlag As Boolean) As QhYappliPortalTanitaConnectionWriteApiResults

        Dim apiArgs As New QhYappliPortalTanitaConnectionWriteApiArgs(
            QhApiTypeEnum.YappliPortalTanitaConnectionWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = TANITA_SYSTEM_NO.ToString(),
            .LinkageSystemId = linkageId,
            .Tags = tags,
            .Devices = devices.ConvertAll(Function(i) i.ToString()),
            .StatusType = StatusType.ToString(),
            .DeleteFlag = deleteFlag.ToString()
        }
        Dim apiResults As QhYappliPortalTanitaConnectionWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTanitaConnectionWriteApiResults)(
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


    Private Shared Function ExecuteTanitaConnectionUpdateWriteApi(mainModel As QolmsYappliModel, device As Byte, tags As List(Of String), checked As Boolean) As QhYappliPortalTanitaConnectionDeviceWriteApiResults

        Dim apiArgs As New QhYappliPortalTanitaConnectionDeviceWriteApiArgs(
            QhApiTypeEnum.YappliPortalTanitaConnectionDeviceWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = TANITA_SYSTEM_NO.ToString(),
            .Device = device.ToString(),
            .Tags = tags,
            .Checked = checked.ToString()
        }
        Dim apiResults As QhYappliPortalTanitaConnectionDeviceWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTanitaConnectionDeviceWriteApiResults)(
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

    ''' <summary>
    ''' JOBを実行します。
    ''' </summary>
    ''' <param name="paramString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExecuteQolmsKaradaKaruteFirstJob(jobUri As String, jobUser As String, jobPassword As String, paramString As String) As Boolean
        Try
            'HttpWebRequestの作成
            Dim webreq As System.Net.HttpWebRequest = _
                DirectCast(System.Net.WebRequest.Create(jobUri & "?arguments=" & paramString),  _
                    System.Net.HttpWebRequest)
            'PreAuthenticateプロパティを設定
            webreq.PreAuthenticate = True

            'Basic認証の設定
            webreq.Credentials = New System.Net.NetworkCredential(jobUser, jobPassword)
            webreq.Method = "POST"
            webreq.ContentLength = 0
            'HttpWebResponseの取得
            Dim webres As System.Net.HttpWebResponse = DirectCast(webreq.GetResponse(), System.Net.HttpWebResponse)


        Catch exh As System.Net.WebException
            DebugLog(exh.Message)

            Dim str As New StringBuilder()
            str.AppendLine(EncriptString(exh.Message))
            str.AppendLine(paramString)
            'メール
            PortalTanitaConnectionWorker.SendMail(str.ToString())
            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(exh.Message))

            Return False
        Catch ex As Exception
            DebugLog(ex.Message)
            'メール
            Dim str As New StringBuilder()
            str.AppendLine(EncriptString(ex.Message))
            str.AppendLine(paramString)
            PortalTanitaConnectionWorker.SendMail(str.ToString())

            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))

            Return False
        End Try

        Return True

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

    Public Shared Function GetConnected(mainModel As QolmsYappliModel, ByRef id As String) As Boolean

        Dim apiResult As QhYappliPortalTanitaConnectionReadApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionReadApi(mainModel, {TANITA_SYSTEM_NO.ToString()}.ToList())
        With apiResult
            If .LinkageItems.Count > 0 Then
                id = .LinkageItems.First().LinkageSystemId
                If .LinkageItems.First().LinkageSystemNo = TANITA_SYSTEM_NO.ToString() AndAlso .LinkageItems.First().StatusType = "2" Then
                    Return True
                End If
            End If
        End With
        Return False

    End Function
    Public Shared Function GetConnected(mainModel As QolmsYappliModel) As Boolean

        Dim apiResult As QhYappliPortalTanitaConnectionReadApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionReadApi(mainModel, New List(Of String)() From {TANITA_SYSTEM_NO.ToString()})
        If apiResult.LinkageItems.Count = 0 OrElse String.IsNullOrWhiteSpace(apiResult.LinkageItems.First().LinkageSystemId) Then
            '未連携
            Return False
        Else
            Return True
        End If

    End Function

    Public Shared Function GetConnectedDevice(mainModel As QolmsYappliModel) As List(Of DeviceItem)

        Dim result As New List(Of DeviceItem)()
        Dim apiResult As QhYappliPortalTanitaConnectionReadApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionReadApi(mainModel, New List(Of String)() From {TANITA_SYSTEM_NO.ToString()})
        With apiResult

            result.Add(New DeviceItem() With {.DevicePropertyName = "BodyCompositionMeter", .DeviceName = "体重"})
            result.Add(New DeviceItem() With {.DevicePropertyName = "Sphygmomanometer", .DeviceName = "血圧"})
            result.Add(New DeviceItem() With {.DevicePropertyName = "Pedometer", .DeviceName = "歩数"})
            For Each linkageItem As QhApiLinkageItem In .LinkageItems


                If linkageItem.LinkageSystemNo = TANITA_SYSTEM_NO.ToString() Then
                    'tanita
                    If Not String.IsNullOrWhiteSpace(linkageItem.LinkageSystemId) Then
                        Dim linked As List(Of Byte) = linkageItem.Devices.ConvertAll(Function(i) Byte.Parse(i))
                        For Each item As DeviceItem In result
                            'デバイスのEnumをループさせるつもり
                            Dim device As QsKaradaKaruteApiDeviceTypeEnum = CType([Enum].Parse(GetType(QsKaradaKaruteApiDeviceTypeEnum), item.DevicePropertyName), QsKaradaKaruteApiDeviceTypeEnum)
                            If linked.Contains(device) Then
                                item.Checked = True
                            End If
                        Next
                    End If
                End If
            Next

        End With

        Return result

    End Function

    ''' <summary>
    ''' 連携している デバイス 情報の リスト を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="refMemberNo">
    ''' 会員識別番号が格納される変数。
    ''' 取得に失敗した場合は String.Empty が格納されます。
    ''' </param>
    ''' <returns>
    ''' 成功なら デバイス 情報の リスト、
    ''' 失敗なら空の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Shared Function GetConnectedDevice(mainModel As QolmsYappliModel, ByRef refMemberNo As String) As List(Of DeviceItem)

        refMemberNo = String.Empty

        Dim result As New List(Of DeviceItem)()

        Try
            Dim apiResult As QhYappliPortalTanitaConnectionReadApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionReadApi(mainModel, New List(Of String)() From {TANITA_SYSTEM_NO.ToString()})

            With apiResult
                result.AddRange(
                    {
                        New DeviceItem() With {
                            .DeviceName = "体重",
                            .DevicePropertyName = QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter.ToString(),
                            .Checked = False
                        },
                        New DeviceItem() With {
                            .DeviceName = "血圧",
                            .DevicePropertyName = QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer.ToString(),
                            .Checked = False
                        },
                        New DeviceItem() With {
                            .DeviceName = "歩数",
                            .DevicePropertyName = QsKaradaKaruteApiDeviceTypeEnum.Pedometer.ToString(),
                            .Checked = False
                        }
                    }
                )

                If .LinkageItems.Count = 1 Then
                    Dim linkage As QhApiLinkageItem = .LinkageItems.First()

                    If linkage.LinkageSystemNo.TryToValueType(Integer.MinValue) = TANITA_SYSTEM_NO _
                        AndAlso Not String.IsNullOrWhiteSpace(linkage.LinkageSystemId) Then

                        refMemberNo = linkage.LinkageSystemId

                        Dim devices As New HashSet(Of String)(linkage.Devices.ConvertAll(Function(i) i.TryToValueType(QsKaradaKaruteApiDeviceTypeEnum.None).ToString()))

                        result.ForEach(Sub(i) i.Checked = devices.Contains(i.DevicePropertyName))
                    End If
                End If
            End With
        Catch
            refMemberNo = String.Empty
            result = New List(Of DeviceItem)()
        End Try

        Return result

    End Function

    ''' <summary>
    ''' タニタ連携入力画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalTanitaConnectionInputModel

        Dim result As New PortalTanitaConnectionInputModel()
        '画面に必要根情報を取得する
        'アカウントキーを指定して連携情報を取得
        Dim apiResult As QhYappliPortalTanitaConnectionReadApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionReadApi(mainModel, New List(Of String)() From {TANITA_SYSTEM_NO.ToString(), ALKOO_SYSTEM_NO.ToString()})
        With apiResult
            result.Devices.Add(New DeviceItem() With {.DevicePropertyName = "BodyCompositionMeter", .DeviceName = "体重"})
            result.Devices.Add(New DeviceItem() With {.DevicePropertyName = "Sphygmomanometer", .DeviceName = "血圧"})
            result.Devices.Add(New DeviceItem() With {.DevicePropertyName = "Pedometer", .DeviceName = "歩数"})
            For Each linkageItem As QhApiLinkageItem In .LinkageItems

                If linkageItem.LinkageSystemNo = TANITA_SYSTEM_NO.ToString() Then
                    'tanita
                    result.ConnectionID = linkageItem.LinkageSystemId

                    If Not String.IsNullOrWhiteSpace(result.ConnectionID) Then
                        Dim linked As List(Of Byte) = linkageItem.Devices.ConvertAll(Function(i) Byte.Parse(i))
                        For Each item As DeviceItem In result.Devices
                            'デバイスのEnumをループさせるつもり
                            Dim device As QsKaradaKaruteApiDeviceTypeEnum = CType([Enum].Parse(GetType(QsKaradaKaruteApiDeviceTypeEnum), item.DevicePropertyName), QsKaradaKaruteApiDeviceTypeEnum)
                            If linked.Contains(device) Then
                                item.Checked = True
                            End If
                        Next
                    End If

                Else
                    Select Case linkageItem.LinkageSystemNo
                        Case ALKOO_SYSTEM_NO.ToString()
                            'tagのなかまで見るかは検討
                            result.AlkooConnectedFlag = True
                    End Select

                End If

            Next

            'tanitaなかったら初期値(true)
            If String.IsNullOrWhiteSpace(result.ConnectionID) Then
                For Each item As DeviceItem In result.Devices
                    'デバイスのEnumをループさせるつもり
                    item.Checked = True
                Next

            End If

        End With

        Return result
    End Function


    ''' <summary>
    ''' タニタ連携入力画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ConnectionRegister(mainModel As QolmsYappliModel, inputModel As PortalTanitaConnectionInputModel, ByRef message As String) As Boolean


        'タニタAPIで連携IDを取得
        Dim apiResult As New MemberAuthApiResults()
        Try
            apiResult = PortalTanitaConnectionWorker.ExecuteKaradaKaruteConnectionApi(inputModel.ID, inputModel.Password)
        Catch ex As Exception
            'テスト用
            message = "タニタとの連携の呼び出しに失敗しました。"
            Return False
        End Try

        Dim memberId As String = String.Empty

        If apiResult.status = "0" AndAlso Not String.IsNullOrWhiteSpace(apiResult.member_no) Then
            memberId = apiResult.member_no
        Else
            message = apiResult.message
            Return False
        End If
        inputModel.Devices.Add(New DeviceItem() With {.DevicePropertyName = "BodyCompositionMeter", .DeviceName = "体重", .Checked = inputModel.BodyCompositionMeter})
        inputModel.Devices.Add(New DeviceItem() With {.DevicePropertyName = "Sphygmomanometer", .DeviceName = "血圧", .Checked = inputModel.Sphygmomanometer})
        inputModel.Devices.Add(New DeviceItem() With {.DevicePropertyName = "Pedometer", .DeviceName = "歩数", .Checked = inputModel.Pedometer})

        Dim devices As New List(Of Byte)()
        Dim tags As New List(Of String)()
        'Linkage_Datを更新
        For Each item As DeviceItem In inputModel.Devices
            If item.Checked Then
                Dim deviceEnum As QsKaradaKaruteApiDeviceTypeEnum = CType([Enum].Parse(GetType(QsKaradaKaruteApiDeviceTypeEnum), item.DevicePropertyName), QsKaradaKaruteApiDeviceTypeEnum)
                devices.Add(deviceEnum)

                Select Case deviceEnum
                    Case QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter
                        tags.Add("6021")
                        '●体組成
                        '6021 : 体重 (xxx.xx)kg
                        '6022 : 体脂肪率% (xx.xx)%
                    Case QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer
                        tags.AddRange({"622E", "622F"}.ToList())
                        '●血圧
                        '622E : 最高血圧 (xxx)mmHg
                        '622F : 最低血圧 (xxx)mmHg
                        '6230 : 脈拍 (xxx)回/分

                    Case QsKaradaKaruteApiDeviceTypeEnum.Pedometer
                        tags.Add("6331")
                        '●歩数
                        '6331 : 歩数 (xxxxx)歩
                        '6338 : 総消費エネルギー量 (xxxx.x)kcal
                        '6339 : 活動エネルギー量(活動量計) (xxxx.x)kcal
                End Select
            End If
        Next
        Dim writerResult As New QhYappliPortalTanitaConnectionWriteApiResults()
        Try

            writerResult = PortalTanitaConnectionWorker.ExecuteTanitaConnectionWriteApi(mainModel, inputModel, memberId, devices, tags, 2, False)
        Catch ex As Exception
            message = "IDまたはパスワードが不正です。"

            Return False
        End Try

        '成功したらポイント呼び出し
        If writerResult.IsSuccess.TryToValueType(False) Then

            '過去1か月分のデータ取得Jobをキック

            'localの時は呼び出さないようにする
            Try

                Dim parameter As String = QolmsLibraryV1.QolmsKaradaKaruteFirstJobWorker.MakeParamString(mainModel.AuthorAccount.AccountKey, TANITA_SYSTEM_NO, QolmsLibraryV1.QhDeviceTypeEnum.BodyCompositionMeter Or QolmsLibraryV1.QhDeviceTypeEnum.Sphygmomanometer Or QolmsLibraryV1.QhDeviceTypeEnum.Pedometer)
                Dim result As Boolean = PortalTanitaConnectionWorker.ExecuteQolmsKaradaKaruteFirstJob(JOB_URL, JOB_ACCOUNT, JOB_PASSWORD, parameter)

            Catch ex As Exception
                'メール
                PortalTanitaConnectionWorker.SendMail(ex.Message)
                'アクセスログ
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))

            End Try

            Try
                ''ポイント付与
                'Dim task As Task
                '' プレミアム会員初登録時にポイント付与
                'Dim actionDate As Date = Now
                '' Dim limit As Date = Now.Date.AddMonths(7).AddDays(-1)
                'Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                'Dim pointItemList As New List(Of QolmsPointGrantItem)
                ''todo:ポイント数はITEMの中の配列で指定
                'pointItemList.Add(New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.TanitaConnection, limit)) '初回プレミアム登録
                'task.Run(
                '    Sub()
                '        QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
                '        mainModel.AuthorAccount.AccountKey, pointItemList)
                '    End Sub
                ')

            Catch ex As Exception
                'メール
                PortalTanitaConnectionWorker.SendMail(EncriptString(ex.Message))
                'アクセスログ
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))
            End Try

        End If
        Return writerResult.IsSuccess.TryToValueType(False)


    End Function

    ''' <summary>
    ''' 連携を開始
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DeviceRegister(mainModel As QolmsYappliModel, deviceName As String, checked As Boolean) As List(Of Byte)

        Dim result As New PortalTanitaConnectionInputModel()

        Dim device As Byte
        device = CType([Enum].Parse(GetType(QsKaradaKaruteApiDeviceTypeEnum), deviceName), QsKaradaKaruteApiDeviceTypeEnum)

        Dim tags As New List(Of String)()
        Select Case device
            Case QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter
                tags.Add("6021")
                '●体組成
                '6021 : 体重 (xxx.xx)kg
                '6022 : 体脂肪率% (xx.xx)%
            Case QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer
                tags.AddRange({"622E", "622F"}.ToList())
                '●血圧
                '622E : 最高血圧 (xxx)mmHg
                '622F : 最低血圧 (xxx)mmHg
                '6230 : 脈拍 (xxx)回/分

            Case QsKaradaKaruteApiDeviceTypeEnum.Pedometer
                tags.Add("6331")
                '●歩数
                '6331 : 歩数 (xxxxx)歩
                '6338 : 総消費エネルギー量 (xxxx.x)kcal
                '6339 : 活動エネルギー量(活動量計) (xxxx.x)kcal
        End Select

        Dim writerResult As QhYappliPortalTanitaConnectionDeviceWriteApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionUpdateWriteApi(mainModel, device, tags, checked)
        Return writerResult.Devices.ConvertAll(Function(i) Byte.Parse(i))

    End Function


    ''' <summary>
    ''' 連携を解除する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Cancel(mainModel As QolmsYappliModel, message As String) As Boolean

        '連携情報取得
        Dim apiResult As QhYappliPortalTanitaConnectionReadApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionReadApi(mainModel, New List(Of String)() From {TANITA_SYSTEM_NO.ToString()})

        '連携を解除
        Dim cancelResult As New MemberAuthApiResults()
        Try
            cancelResult = PortalTanitaConnectionWorker.ExecuteKaradaKaruteCancel(apiResult.LinkageItems.First().LinkageSystemId)

        Catch ex As Exception
            'メール
            PortalTanitaConnectionWorker.SendMail(EncriptString(ex.Message))
            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))

            Return False
        End Try

        If cancelResult.status = "0" Then
            '解除成功
            'Linkageを未連携に更新
            Dim writerResult As QhYappliPortalTanitaConnectionWriteApiResults = PortalTanitaConnectionWorker.ExecuteTanitaConnectionWriteApi(mainModel, New PortalTanitaConnectionInputModel(), String.Empty, New List(Of Byte)(), New List(Of String)(), 2, True)
            Return writerResult.IsSuccess.TryToValueType(False)
        Else
            message = cancelResult.message
            Return False
        End If

    End Function

#End Region

End Class