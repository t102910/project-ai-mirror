Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1


''' <summary>
''' 「チャレンジ」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalChallengeDetailWorker

#Region "Constant"

    Private Const TANITA_SYSTEM_NO As Integer = 47005
    Private Const TANITA_SITE_ID As String = "830"
    '伊平野村の場合
    '暗号化
    Private Shared ReadOnly IHEYA_CATEGORYKEY As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieCategoryKeyIheya")
    '竹富町の場合
    '暗号化
    Private Shared ReadOnly TAKETOMI_CATEGORYKEY As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieCategoryKeyTaketomi")

    '暗号化
    Private Shared ReadOnly TANITA_SITE_KEY As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieSiteKey")

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
    ''' 「チャレンジ詳細」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeDetailReadApi(mainModel As QolmsYappliModel, challengekey As Guid, getLinkage As Integer) As QhYappliPortalChallengeDetailReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeDetailReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeDetailRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .Challengekey = challengekey.ToApiGuidString(),
             .LinkageSystemNo = getLinkage.ToString()
         }

        Dim apiResults As QhYappliPortalChallengeDetailReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeDetailReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function


#End Region

#Region "Public Method"
    ''' <summary>
    ''' チャレンジ画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, challengekey As Guid) As PortalChallengeDetailViewModel

        Dim result As New PortalChallengeDetailViewModel()
        ' API を実行
        Dim getLinkage As Integer = Integer.MinValue

        If challengekey = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1") OrElse challengekey = Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c") Then

            getLinkage = TANITA_SYSTEM_NO
        End If

        With PortalChallengeDetailWorker.ExecutePortalChallengeDetailReadApi(mainModel, challengekey, getLinkage)

            Dim item As QhApiChallengeItem = .ChallengeItem
            Dim imageStr As String = String.Empty
            'If PortalChallengeWorker.imageDic.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then
            If PortalChallengeWorker.ChallengeFolder.Value.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then
                imageStr = String.Format("../dist/img/challenge/{0}/", PortalChallengeWorker.ChallengeFolder.Value(item.Challengekey.TryToValueType(Guid.Empty)))
            End If

            Dim challenge As New ChallengeItem() With {
                .Challengekey = item.Challengekey.TryToValueType(Guid.Empty),
                .Description = item.Description,
                .EntryEndDate = item.EntryEndDate.TryToValueType(Date.MinValue),
                .EntryStartDate = item.EntryStartDate.TryToValueType(Date.MinValue),
                .Name = item.Name,
                .Period = item.Period.TryToValueType(Integer.MinValue),
                .StartDate = item.StartDate.TryToValueType(Date.MinValue),
                .StatusType = item.StatusType.TryToValueType(Byte.MinValue),
                .StatusTypeMaster = item.StatusTypeMaster.ConvertAll(Function(i) New ChallengeStatusItem() With {
                                                                         .Challengekey = i.Challengekey.TryToValueType(Guid.Empty),
                                                                         .StatusType = i.StatusType.TryToValueType(Byte.MinValue),
                                                                         .StatusName = i.StatusName
                                                                     }),
                .UserEndDate = item.UserEndDate.TryToValueType(Date.MinValue),
                .UserStartDate = item.UserStartDate.TryToValueType(Date.MinValue),
                .ExternalId = item.ExternalId,
                .ImageSrc = imageStr,
                .LinkageSystemNo = item.LinkageSystemNo.TryToValueType(Integer.MinValue)
                }

            result.ChallengeItem = challenge
            result.TargetAchievedType = Byte.Parse(.TargetAchievedType)

            Return result

        End With

    End Function

    'Shared Function GetChallengeDetail3PartialViewModel(model As PortalChallengeDetailViewModel) As PortalChallengeDetail3PartialPartialViewModel

    '    Dim result As New PortalChallengeDetail3PartialPartialViewModel()

    '    'If model.ChallengeItem.Challengekey = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1") OrElse model.ChallengeItem.Challengekey = Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c") Then

    '    Dim endPoint As String = "https://www.karadakarute.jp/hlp/login/sso"
    '    Dim uid As String = String.Empty
    '    Dim siteId As String = TANITA_SITE_ID
    '    Dim siteKey As String = String.Empty
    '    Dim categoryKey As String = String.Empty

    '    Try
    '        Using crypt As New QsCrypt(QsCryptTypeEnum.Default)
    '            siteKey = crypt.DecryptString(TANITA_SITE_KEY)

    '            If model.ChallengeItem.Challengekey = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1") Then
    '                ' 竹富
    '                categoryKey = crypt.DecryptString(TAKETOMI_CATEGORYKEY)
    '            Else
    '                ' 伊平屋
    '                categoryKey = crypt.DecryptString(IHEYA_CATEGORYKEY)

    '            End If
    '        End Using
    '    Catch ex As Exception
    '    End Try

    '    '暗号化処理
    '    Dim text As String = String.Format("{0},{1},{2}", siteId, model.LinkageSystemId, Date.Now.ToString("yyyyMMddHHmm"))
    '    Using crypt As New QsCrypt(QsCryptTypeEnum.JotoKaradakarute)
    '        uid = crypt.EncryptString(text)
    '    End Using

    '    '1:健康ショートドラマの場合()
    '    '　/video/health_support_drama/webview
    '    Dim healthUrl As String = "/video/health_support_drama/webview"

    '    '２．オンラインセミナー動画（動画版）の場合
    '    '　/video/seminar_drama/today/webview
    '    Dim seminarUrl As String = "/video/seminar_drama/today/webview"

    '    result.HealthSupportDrama = endPoint + "?" + String.Format("uid={0}&sid={1}&siteKey={2}&categoryKey={3}&url={4}", uid, siteId, siteKey, categoryKey, healthUrl)
    '    result.SeminarDrama = endPoint + "?" + String.Format("uid={0}&sid={1}&siteKey={2}&categoryKey={3}&url={4}", uid, siteId, siteKey, categoryKey, seminarUrl)

    '    result.PageViewModel = model

    '    Return result

    'End Function

#End Region

End Class
