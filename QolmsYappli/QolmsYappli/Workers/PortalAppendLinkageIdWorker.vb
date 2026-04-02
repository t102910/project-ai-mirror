Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1

''' <summary>
''' 「連携システム番号登録」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalAppendLinkageIdWorker

#Region "Constant"


    ''' <summary>
    ''' 設定値から  を取得します。
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property AppendLinkageSystemNoList As New Lazy(Of List(Of Integer))(Function() GetSetting())
    Public Shared ReadOnly Property statusApplied As New List(Of Integer) From {47900019} '設定に出すほどかどうかは検討。

    Private Shared ReadOnly ErrorMessageDictionary As New Dictionary(Of QjApiLinkageErrorTypeEnum, String)() From {
        {QjApiLinkageErrorTypeEnum.AlreadyExists, "この連携 はすでに登録されています。"},
        {QjApiLinkageErrorTypeEnum.DbRegisterFaild, ""},
        {QjApiLinkageErrorTypeEnum.UpdateFailed, "連携 ID がすでに登録されています。"},
        {QjApiLinkageErrorTypeEnum.NotRegistered, "連携 が登録されていません。"}
    }


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
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting() As List(Of Integer)

        Dim result As New List(Of Integer)()
        Dim value As String = ConfigurationManager.AppSettings("AppendLinkageSystemNoList")

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            Dim arr As String() = value.Split(","c)
            'If arr.Any() Then

            For Each item As String In arr
                Dim outint As Integer = Integer.MinValue

                If Integer.TryParse(item, outint) Then
                    result.Add(outint)
                End If

            Next
            'End If

        End If

        Return result

    End Function


    '''' <summary>
    '''' 「」画面取得 API を実行します。
    '''' </summary>
    '''' <param name="mainModel">ログイン済みモデル。</param>
    '''' <returns>
    '''' Web API 戻り値クラス。
    '''' </returns>
    '''' <remarks></remarks>
    'Private Shared Function ExecutePortalAppendLinkageIdReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeEditReadApiResults

    '    Dim apiArgs As New QhYappliPortalChallengeEditReadApiArgs(
    '         QhApiTypeEnum.YappliPortalChallengeEditRead,
    '         QsApiSystemTypeEnum.Qolms,
    '         mainModel.ApiExecutor,
    '         mainModel.ApiExecutorName
    '         ) With {
    '             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
    '             .Challengekey = challengekey.ToApiGuidString()
    '    }

    '    Dim apiResults As QhYappliPortalChallengeEditReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEditReadApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then
    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function


    Private Shared Function ExecutePortalAppendLinkageIdWriteApi(mainModel As QolmsYappliModel, LinkageSystemNo As Integer, LinkageSystemId As String, status As Byte) As QjPortalAppendLinkageIdWriteApiResults

        Dim apiArgs As New QjPortalAppendLinkageIdWriteApiArgs(
         QjApiTypeEnum.PortalAppendLinkageIdWrite,
         QsApiSystemTypeEnum.QolmsJoto,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .LinkageSystemNo = LinkageSystemNo.ToString(),
             .LinkageSystemId = LinkageSystemId,
             .Status = status.ToString()
         }

        Dim apiResults As QjPortalAppendLinkageIdWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalAppendLinkageIdWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"

    Public Shared Function Append(mainModel As QolmsYappliModel, LinkageSystemNo As String, LinkageSystemId As String, ByRef errorMessage As String) As Boolean

        Dim strNo As String = String.Empty
        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

            strNo = crypt.DecryptString(LinkageSystemNo)

        End Using

        Dim resNo As Integer = Integer.MinValue
        Dim status As Byte = 1 '初期値は申請中

        ' すでに作成済みのLinkgaeデータにLinkageSystemIDを追加する
        ' 許可されたLinageNoじゃない場合、
        If Integer.TryParse(strNo, resNo) AndAlso PortalAppendLinkageIdWorker.AppendLinkageSystemNoList.Value.IndexOf(resNo) >= 0 Then

            'ステータス連携済みにする例外処理
            If statusApplied.IndexOf(resNo) >= 0 Then
                status = 2
            End If

            With PortalAppendLinkageIdWorker.ExecutePortalAppendLinkageIdWriteApi(mainModel, resNo, LinkageSystemId, status)

                If .ErrorType.TryToValueType(QjApiLinkageErrorTypeEnum.None) = QjApiLinkageErrorTypeEnum.None Then
                    '成功
                    errorMessage = "成功"
                    Return True
                Else

                    Dim type As QjApiLinkageErrorTypeEnum = .ErrorType.TryToValueType(QjApiLinkageErrorTypeEnum.None)

                    If PortalAppendLinkageIdWorker.ErrorMessageDictionary.ContainsKey(type) Then
                        errorMessage = PortalAppendLinkageIdWorker.ErrorMessageDictionary(type)
                    Else
                        errorMessage = $"エラーが発生しました。({type.ToString("d")})"
                    End If

                End If

            End With
        Else
            errorMessage = "許可されていない連携です。"
        End If

        Return False

    End Function

    'Public Shared Function Edit(mainModel As QolmsYappliModel, preaentModel As PortalChallengeEditInputModel) As Boolean


    'End Function


    'Shared Function Cancel(mainModel As QolmsYappliModel, challengekey As Guid) As Boolean

    'End Function

#End Region

End Class
