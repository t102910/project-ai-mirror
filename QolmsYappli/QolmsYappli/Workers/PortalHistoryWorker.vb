Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「JOTO ポイント履歴」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalHistoryWorker

    ''' <summary>
    ''' auidの正規表現チェック
    ''' 他のIDで似たような形式があったら通ってしまうので移植時はopenidtypeを取得してチェックしてください。
    ''' </summary>
    Private Shared ReadOnly REGEX_AU_ID As New Regex("^https://.+/*$", RegexOptions.IgnoreCase)

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 「JOTO ポイント履歴」画面ビュー モデル作成します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="year">表示年。</param>
    ''' <param name="month">表示月。</param>
    ''' <returns>
    ''' 「JOTO ポイント履歴」画面ビュー モデル。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, year As Integer, month As Integer, FromPageNoType As QyPageNoTypeEnum) As PortalHistoryViewModel

        Dim point As Integer = Integer.MinValue
        Dim closestExprirationDate As Date = Date.MinValue
        Dim closestExprirationPoint As Integer = Integer.MinValue
        Dim targetYear As Integer = year
        Dim targetMonth As Integer = month
        Dim fromDate As Date = Date.MinValue
        Dim toDate As Date = Date.MinValue
        Dim items As New List(Of QoApiQolmsPointHistoryResultItem)()

        ' 保有ポイント、直近の失効ポイントを取得
        Try
            point = QolmsPointWorker.GetQolmsPointWithClosestExpriration(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                closestExprirationDate,
                closestExprirationPoint
            )
        Catch
            point = 0
            closestExprirationDate = Date.MinValue
            closestExprirationPoint = 0
        End Try

        ' ポイント履歴を取得
        If year < 1 _
            OrElse year > 9999 _
            OrElse month < 1 _
            OrElse month > 12 Then

            With Date.Now
                targetYear = year
                targetMonth = .Month
            End With

        End If

        fromDate = New Date(targetYear, targetMonth, 1)
        toDate = fromDate.AddMonths(1).AddDays(-1)

        Try
            items = QolmsPointWorker.GetTargetPointFromHistoryList(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                QyPointItemTypeEnum.None,
                fromDate,
                toDate
            )
            '減算ポイント
            Dim exprirat As Integer = items.Where(Function(i) i.PointItemNo = "0").ToList().Sum(Function(j) Integer.Parse(j.PointValue))
            If exprirat < 0 Then
                Dim expriratItem As QoApiQolmsPointHistoryResultItem = items.Where(Function(i) i.PointItemNo = "0").ToList().First()
                expriratItem.PointValue = exprirat.ToString()
                Dim newItems As New List(Of QoApiQolmsPointHistoryResultItem)()
                newItems.Add(expriratItem)
                newItems.AddRange(items.Where(Function(i) i.PointItemNo <> "0").ToList())

                items = newItems

            End If

        Catch
            items = New List(Of QoApiQolmsPointHistoryResultItem)()
        End Try

        Dim isAuId As Boolean = False
        If Not String.IsNullOrWhiteSpace(mainModel.AuthorAccount.OpenId) _
            AndAlso REGEX_AU_ID.IsMatch(mainModel.AuthorAccount.OpenId) Then
            isAuId = True
        End If

        'au契約があるかどうか
        Dim isMobileSubscriberOfAu As Boolean = False
        Try
            isMobileSubscriberOfAu = AuOwlAccessWorker.IsMobileSubscriberOfAu(mainModel.AuthorAccount.OpenId)
        Catch ex As Exception
            Dim message As String = ex.Message
        End Try

        ' 最新の会員ステータスを取得 
        mainModel.AuthorAccount.MembershipType =DirectCast([Enum].ToObject(GetType(QyMemberShipTypeEnum),  PremiumWorker.GetMemberShipType(mainModel)), QyMemberShipTypeEnum)

        'プレミアムかどうか
        Dim isPremium As Boolean = _
            mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime OrElse _
            mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Premium

                
        '法人連携かどうか
        Dim isforBiz As Boolean = _
            mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Business OrElse _
            mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.BusinessFree


        '病院連携があるかどうか
        Dim IsConnectedHospital As Boolean = PortalHomeWorker.GetMedicalLinkageList(mainModel).Where(Function(i) i.StatusType = "2").Count> 0


        Dim challengeList As Dictionary(Of Guid,String) = PortalChallengeWorker.GetChallengeEntryList(mainModel)

        ' ビュー モデルを返却
        Return New PortalHistoryViewModel(
            mainModel,
            point,
            closestExprirationDate,
            closestExprirationPoint,
            targetYear,
            targetMonth,
            items,
            isMobileSubscriberOfAu,
            FromPageNoType,
            isAuId,
            isPremium,
            isforBiz,
            IsConnectedHospital,
            challengeList
        )

    End Function

#End Region

End Class
