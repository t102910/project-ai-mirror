Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「JOTO ポイント履歴」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalHistoryViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 現在のポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As Integer = Integer.MinValue

    ''' <summary>
    ''' 現在のポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Point As Integer = Integer.MinValue

    ''' <summary>
    ''' 直近の有効期限を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ClosestExprirationDate As Date = Date.MinValue

    ''' <summary>
    ''' 直近の有効期限で失効するポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ClosestExprirationPoint As Integer = Integer.MinValue

    ''' <summary>
    ''' 表示年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Year As Integer = Integer.MinValue

    ''' <summary>
    ''' 表示月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Month As Integer = Integer.MinValue

    ''' <summary>
    ''' au契約状態を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsMobileSubscriberOfAu As Boolean = False

    ''' <summary>
    ''' 日付毎の JOTO ポイント ログ情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PointDailyLogN As New List(Of JotoPointDailyLogItem)()

    ''' <summary>
    ''' auIDかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsAuId As Boolean = False
    
    ''' <summary>
    ''' プレミアム会員かどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsPremium As Boolean = False

    ''' <summary>
    ''' 法人連携かどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsforBiz As Boolean = False

    ''' <summary>
    ''' 病院連携済みかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsConnectedHospital As Boolean = False

    ''' <summary>
    ''' 参加中のチャレンジのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeEntryList As new Dictionary(Of Guid,String)

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHistoryViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="PortalHistoryViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="point">現在のポイント。</param>
    ''' <param name="closestExprirationDate">直近の有効期限。</param>
    ''' <param name="closestExprirationPoint">直近の有効期限で失効するポイント。</param>
    ''' <param name="year">表示年。</param>
    ''' <param name="month">表示月。</param>
    ''' <param name="items">QOLMS ポイント履歴情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, point As Integer, closestExprirationDate As Date, closestExprirationPoint As Integer, year As Integer, month As Integer, items As IEnumerable(Of QoApiQolmsPointHistoryResultItem), isMobileSubscriberOfAu As Boolean, fromPageNoType As QyPageNoTypeEnum, isAuId As Boolean, _
                   IsPremium As Boolean ,IsforBiz As Boolean,IsConnectedHospital As Boolean,challengeList As Dictionary(Of Guid,String))
        MyBase.New(mainModel, QyPageNoTypeEnum.PortalHistory)

        Me.InitializeBy(point, closestExprirationDate, closestExprirationPoint, year, month, items, isMobileSubscriberOfAu, fromPageNoType, isAuId,IsPremium,IsforBiz,IsConnectedHospital,challengeList)

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 有効期限を取得します。
    ''' </summary>
    ''' <param name="itemNo">ポイント項目番号。</param>
    ''' <param name="actionDate">操作日時。</param>
    ''' <param name="targetDate">対象日時。</param>
    ''' <param name="point">ポイント。</param>
    ''' <returns>
    ''' 有効期限。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function GetExpirationDate(itemNo As Integer, actionDate As Date, targetDate As Date, point As Integer) As Date

        Dim result As Date = Date.MinValue

        If itemNo <> QyPointItemTypeEnum.None AndAlso point > 0 Then
            ' 加算ポイント
            If itemNo <> QyPointItemTypeEnum.Examination Then
                ' 健診以外
                result = New Date(targetDate.Year, targetDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は測定日）
            Else
                ' 健診
                result = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="PortalHistoryViewModel" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="point">現在のポイント。</param>
    ''' <param name="closestExprirationDate">直近の有効期限。</param>
    ''' <param name="closestExprirationPoint">直近の有効期限で失効するポイント。</param>
    ''' <param name="year">表示年。</param>
    ''' <param name="month">表示月。</param>
    ''' <param name="items">QOLMS ポイント履歴情報のコレクション。</param>
    ''' <remarks></remarks>
    Private Sub InitializeBy(point As Integer, closestExprirationDate As Date, closestExprirationPoint As Integer, year As Integer, month As Integer, items As IEnumerable(Of QoApiQolmsPointHistoryResultItem), isMobileSubscriberOfAu As Boolean, fromPageNoType As QyPageNoTypeEnum, isAuId As Boolean, _
                   isPremium As Boolean ,isforBiz As Boolean,isConnectedHospital As Boolean,challengeList As Dictionary(Of Guid,String))
         

        Me.Point = point
        Me.ClosestExprirationDate = closestExprirationDate
        Me.ClosestExprirationPoint = closestExprirationPoint
        Me.Year = year
        Me.Month = month
        Me.IsMobileSubscriberOfAu = isMobileSubscriberOfAu
        Me.FromPageNoType = fromPageNoType
        Me.IsAuId = isAuId
        Me.IsPremium = IsPremium
        Me.IsforBiz = IsforBiz
        Me.IsConnectedHospital = IsConnectedHospital
        Me.ChallengeEntryList = challengeList

        If items IsNot Nothing AndAlso items.Any() Then
            ' 日付毎にまとめる
            Dim logN As New Dictionary(Of Date, List(Of QoApiQolmsPointHistoryResultItem))()

            For Each item As QoApiQolmsPointHistoryResultItem In items
                Dim key As Date = item.ActionDate.TryToValueType(Date.MinValue).Date

                If key.Year = year And key.Month = month Then
                    If Not logN.ContainsKey(key) Then logN.Add(key, New List(Of QoApiQolmsPointHistoryResultItem)())

                    logN(key).Add(item)
                End If
            Next

            ' 降順にソートされた日付毎の JOTO ポイント ログ情報のリストを作成する
            If logN.Any() Then
                For Each item As KeyValuePair(Of Date, List(Of QoApiQolmsPointHistoryResultItem)) In logN.OrderByDescending(Function(i) i.Key)
                    Me.PointDailyLogN.Add(
                        New JotoPointDailyLogItem() With {
                            .ActionDate = item.Key,
                            .PointLogN = item.Value _
                                .ConvertAll(
                                    Function(i)
                                        Return New JotoPointLogItem() With {
                                            .ActionDate = i.ActionDate.TryToValueType(Date.MinValue),
                                            .TargetDate = i.PointTargetDate.TryToValueType(Date.MinValue),
                                            .ItemNo = i.PointItemNo.TryToValueType(0),
                                            .ItemName = i.PointItemName,
                                            .Point = i.PointValue.TryToValueType(0),
                                            .Reason = i.PointReason,
                                            .ExpirationDate = i.ExprirationDate.TryToValueType(Date.MinValue)
                                        }
                                    End Function
                                ).OrderByDescending(Function(i) i.ActionDate).ThenByDescending(Function(i) i.ItemNo).ToList(),
                            .Point = .PointLogN.Sum(Function(i) i.Point)
                        }
                    )
                Next
            End If
        End If

    End Sub

#End Region

End Class
