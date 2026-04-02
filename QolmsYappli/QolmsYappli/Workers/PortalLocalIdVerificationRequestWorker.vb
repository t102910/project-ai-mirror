
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 「市民確認 同意」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalLocalIdVerificationRequestWorker

#Region "Constant"

    ''' <summary>
    ''' 宜野湾用ピッタリサービスのURLを設定から取得
    ''' </summary>
    Private Shared GINOWAN_APPLY_URL As New Lazy(Of String)(Function() PortalLocalIdVerificationRequestWorker.GetSetting("GinowanApplyUrl"))

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
    ''' 設定を取得します。
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting(settingName As String) As String

        Dim result As String = String.Empty
        Dim value As String = ConfigurationManager.AppSettings(settingName)

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            result = value
        End If

        Return result

    End Function

    '''' <summary>
    '''' 「チャレンジエントリー」画面取得 API を実行します。
    '''' </summary>
    '''' <param name="mainModel">ログイン済みモデル。</param>
    '''' <returns>
    '''' Web API 戻り値クラス。
    '''' </returns>
    '''' <remarks></remarks>
    'Private Shared Function ExecutePortalLocalIdVerificationAgreementReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalLocalIdVerificationAgreementReadApiResults

    '    Dim apiArgs As New QhYappliPortalLocalIdVerificationAgreementReadApiArgs(
    '     QhApiTypeEnum.YappliPortalLocalIdVerificationAgreementRead,
    '     QsApiSystemTypeEnum.Qolms,
    '     mainModel.ApiExecutor,
    '     mainModel.ApiExecutorName
    '     ) With {
    '         .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
    '         .challengekey = challengekey.ToApiGuidString()
    '     }

    '    Dim apiResults As QhYappliPortalLocalIdVerificationAgreementReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalLocalIdVerificationAgreementReadApiResults)(
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

#End Region

#Region "Public Method"
    ''' <summary>
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalLocalIdVerificationRequestViewModel

        Dim result As New PortalLocalIdVerificationRequestViewModel()

        Dim LINKAGE_NO As Integer = 47900021
        '発行したIDを取得
        'ステータスを取得
        'リンクを設定

        Dim linkageItem As LinkageItem = LinkageWorker.GetLinkageItem(mainModel, LINKAGE_NO)

        If linkageItem.LinkageSystemNo = 47900021 Then '連携がすでにあるかどうか
            result.LinkageSystemNo = linkageItem.LinkageSystemNo
            result.LinkageSystemId = linkageItem.LinkageSystemId
            result.LinkageSystemName = linkageItem.LinkageSystemName
            result.Status = linkageItem.StatusType

            Try
                Dim ser As New QsJsonSerializer
                Dim dataset As QhLinkageDataSetOfJson = ser.Deserialize(Of QhLinkageDataSetOfJson)(linkageItem.Dataset)

                result.Reason = dataset.DisapprovedReason

            Catch ex As Exception
            End Try
        End If

        result.Url = HttpUtility.UrlEncode(GINOWAN_APPLY_URL.Value)

        Return result

    End Function

#End Region

End Class
