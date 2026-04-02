
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 「市民確認 確認」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalLocalIdVerificationDetailWorker

#Region "Constant"

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
    ''' チャレンジエントリー画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalLocalIdVerificationDetailViewModel

        Dim result As New PortalLocalIdVerificationDetailViewModel()

        Dim LINKAGE_NO As Integer = 47900021
        '発行したIDを取得
        'ステータスを取得
        'リンクを設定

        Dim linkageItem As LinkageItem = LinkageWorker.GetLinkageItem(mainModel, LINKAGE_NO)

        If linkageItem.LinkageSystemNo = 47900021 Then
            result.LinkageSystemNo = linkageItem.LinkageSystemNo
            result.LinkageSystemName = linkageItem.LinkageSystemName
            result.Status = linkageItem.StatusType

            Select Case result.Status
                Case 1
                    'result.Reason = "JOTO連携IDが発行済みです。参加申請がお済みの方は、申請結果の通知をお待ちください。申請がお済みでない方は、「申請画面に戻る」より参加申請を行ってください。申請状況は、ぴったりサービスよりご確認いただけます。​"
                Case 2
                    'result.Reason = "ぎのわんスマート健康増進プロジェクトへのエントリーが完了しました。"

                Case 3
                    'result.Reason = "宜野湾市民であることの確認ができませんでした。本エントリーは宜野湾市民限定となっております。ご不明な点がございましたら、プロジェクト事務局（098-880-2469）までお問い合わせください。​"

            End Select

            Try
                Dim ser As New QsJsonSerializer
                Dim dataset As QhLinkageDataSetOfJson = ser.Deserialize(Of QhLinkageDataSetOfJson)(linkageItem.Dataset)

                result.Reason += dataset.DisapprovedReason

            Catch ex As Exception
            End Try
        End If

        Return result

    End Function

#End Region

End Class
