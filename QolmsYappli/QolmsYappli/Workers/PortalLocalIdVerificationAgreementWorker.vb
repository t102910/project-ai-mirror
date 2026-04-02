Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1


''' <summary>
''' 「市民確認 同意」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalLocalIdVerificationAgreementWorker

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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalLocalIdVerificationAgreementViewModel

        Dim result As New PortalLocalIdVerificationAgreementViewModel()

        Dim TARMS_NO As Integer = 106

        '規約
        Dim str As String = TarmsWorker.GetTermsContent(mainModel, TARMS_NO)
        result.Terms = str

        Return result

    End Function

#End Region

End Class
