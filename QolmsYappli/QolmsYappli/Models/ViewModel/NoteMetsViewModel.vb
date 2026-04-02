''' <summary>
''' 「運動強度」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteMetsViewModel
    Inherits QyNotePageViewModelBase

#Region "Variable"

    ''' <summary>
    ''' ビューに展開するバイタル情報のディクショナリを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _itemList As New SortedDictionary(Of Date, SortedDictionary(Of Date, VitalValueItem))()

    ''' <summary>
    ''' ビューに展開するバイタル評価情報のリストを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _evaluationItemList As New List(Of VitalEvaluationItem)()

    ''' <summary>
    ''' ビューに展開する表の行数を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _rowCount As Integer = 0

    ''' <summary>
    ''' ビューに展開する Y 軸の最小値を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _yAxisMin As Decimal = Decimal.Zero

    ''' <summary>
    ''' ビューに展開する Y 軸の最大値を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _yAxisMax As Decimal = Decimal.Zero

    ''' <summary>
    ''' ビューに展開するバイタル目標値を表す文字列を保持します。
    ''' 血圧は"[（下）下限,（下）上限,（上）下限,（上）上限]"、
    ''' 体重は"[（体重）下限, （体重）上限, （BMI）下限,（BMI）上限]"、
    ''' 血糖値は"[（空腹時）下限,（空腹時）上限,（その他）下限,（その他）上限]"、
    ''' その他は"[下限,上限]"、
    ''' の形式です。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _targetValue As String = "[]"

    ''' <summary>
    ''' ビューに展開するグラフ値を表す文字列を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _graphData As String = "[]"

    ''' <summary>
    ''' ビューに展開するグラフ ラベルを表す文字列を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _graphLabel As String = "[]"

#End Region


#Region "Public Property"

    ''' <summary>
    ''' バイタル情報の有効性のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AvailableVitalN As New List(Of AvailableVitalItem)()

    ''' <summary>
    ''' ビューに展開するバイタル情報のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ItemList As SortedDictionary(Of Date, SortedDictionary(Of Date, VitalValueItem))

        Get
            Return Me._itemList
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開するバイタル評価情報のリストを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EvaluationItemList As List(Of VitalEvaluationItem)

        Get
            Return Me._evaluationItemList
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する表の行数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RowCount As Integer

        Get
            Return Me._rowCount
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する Y 軸の最小値を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisMin As Decimal

        Get
            Return Me._yAxisMin
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する Y 軸の最大値を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisMax As Decimal

        Get
            Return Me._yAxisMax
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開するバイタル目標値を表す文字列を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TargetValue As String

        Get
            Return Me._targetValue
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開するグラフ値を表す文字列を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GraphData As String

        Get
            Return Me._graphData
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開するグラフ ラベルを表す文字列を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GraphLabel As String

        Get
            Return Me._graphLabel
        End Get

    End Property

    ''' <summary>
    ''' 表示期間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PeriodType As QyPeriodTypeEnum = QyPeriodTypeEnum.OneDay

    ''' <summary>
    ''' 開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartDate As Date = Date.MinValue

    ''' <summary>
    ''' 終了日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EndDate As Date = Date.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMetsViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteMetsViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMets)

        ' TODO 

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteMetsViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="items">情報のコレクション。</param>
    ''' <param name="targetValueLower">目標下限値。</param>
    ''' <param name="targetValueUpper">目標上限値。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel,
                   items As IEnumerable(Of VitalValueItem),
                   targetValueLower As Decimal,
                   targetValueUpper As Decimal,
                   periodType As QyPeriodTypeEnum,
                   startDate As DateTime,
                   endDate As DateTime)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMets)

        Me.PeriodType = PeriodType
        Me.StartDate = StartDate
        Me.EndDate = EndDate


        Dim targetValues() As Decimal = {targetValueLower, targetValueUpper}

        ' 評価情報リストの初期化
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower, targetValueUpper, Decimal.Zero))

        ' Y 軸の最小値と最大値の初期化
        Me._yAxisMin = targetValues.Min()
        Me._yAxisMax = targetValues.Max()

        ' 目標値
        Me._targetValue = String.Format("[{0}]", String.Join(",", targetValues))

        Me.InitializeBy(items)

    End Sub

#End Region

#Region "Protected Method"

    ''' <summary>
    ''' <see cref="NoteMetsViewModel" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="items">心拍情報のコレクション。</param>
    ''' <remarks></remarks>
    Protected Sub InitializeBy(items As IEnumerable(Of VitalValueItem))

        ' 日付のディクショナリを作成
        Dim labels As New List(Of String)()

        If Me.PeriodType = QyPeriodTypeEnum.OneDay Then
            ' 一日は15分間隔一日分
            Dim tomorrowDate As Date = Me.EndDate.AddDays(1)
            Dim targetDate As Date = Me.EndDate
            While targetDate < tomorrowDate

                Dim key As Date = targetDate
                If Not Me._itemList.ContainsKey(key) Then Me._itemList.Add(key, New SortedDictionary(Of Date, VitalValueItem)())

                labels.Add(String.Format("'{0:HH:mm}'", targetDate))

                targetDate = targetDate.AddMinutes(15)

            End While
        Else
            ' その他は範囲内の日数分
            Dim targetDate As Date = Me.StartDate
            While targetDate <= Me.EndDate

                Dim key As Date = targetDate
                If Not Me._itemList.ContainsKey(key) Then Me._itemList.Add(key, New SortedDictionary(Of Date, VitalValueItem)())

                labels.Add(String.Format("'{0:MM月dd日}'", targetDate))

                targetDate = targetDate.AddDays(1)

            End While
        End If

        Me._graphLabel = String.Format("[{0}]", String.Join(","c, labels))

        ' 各日付に測定値を追加
        For Each item As VitalValueItem In items

            Dim key As Date = New Date(item.RecordDate.Year, item.RecordDate.Month, item.RecordDate.Day)

            ' 一日モードは15分間隔
            If Me.PeriodType = QyPeriodTypeEnum.OneDay Then
                key = New Date(item.RecordDate.Year,
                               item.RecordDate.Month,
                               item.RecordDate.Day,
                               item.RecordDate.Hour,
                               Convert.ToInt32(Math.Floor(item.RecordDate.Minute / 15.0) * 15.0),
                               0)
                'key = New Date(item.RecordDate.Year, item.RecordDate.Month, item.RecordDate.Day, item.RecordDate.Hour, item.RecordDate.Minute, 0)
            End If

            If Me._itemList.ContainsKey(key) AndAlso Not Me._itemList(key).ContainsKey(item.RecordDate) Then
                Me._itemList(key).Add(item.RecordDate, item)

                ' 最大行数
                If Me._rowCount < Me._itemList(key).Count Then Me._rowCount = Me._itemList(key).Count
            End If

        Next

        ' 平均値を算出
        For a As Integer = 0 To Me._itemList.Count - 1
            Dim values As SortedDictionary(Of Date, VitalValueItem) = Me._itemList.Values(a)

            Dim average As Decimal = 0
            Dim count As Integer = 0

            If values.Count > 0 Then
                For Each kv As KeyValuePair(Of Date, VitalValueItem) In values
                    ' 0 以上を平均の対象とする。
                    If kv.Value.Value1 >= 0 Then
                        average += kv.Value.Value1
                        count += 1
                    End If
                Next

                If count > 0 Then
                    average = Math.Truncate(average / count * 10) / 10

                    ' 評価情報を新しい日付の平均値で置き換え
                    Me._evaluationItemList(0).Value = average
                Else
                    ' 平均の対象すべてが負の数
                    ' 0 はプロットするので無効値は MinValue
                    average = Decimal.MinValue
                End If
            Else
                ' 日付範囲内にデータがない
                ' 0 はプロットするので無効値は MinValue
                average = Decimal.MinValue
            End If

            Me._itemList.Values(a).Add(
                Date.MinValue,
                New VitalValueItem() With {
                    .RecordDate = Date.MinValue,
                    .Value1 = average,
                    .Value2 = Decimal.MinValue
                }
            )
        Next

        ' 有効な評価情報だけ残す
        If Not Me._evaluationItemList(0).IsEvaluable Then Me._evaluationItemList.RemoveAt(0)

        ' グラフ用データを作成
        Me._graphData = Me.CreateGraphData()

    End Sub

    ''' <summary>
    ''' ビューに展開するグラフ値を表す文字列を作成します。
    ''' </summary>
    ''' <returns>
    ''' バイタル値データ。
    ''' [値1, 値2, ...] の形式（値が無いときは値として null もしくは空白を指定）。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Function CreateGraphData() As String

        Dim values As New List(Of String)()

        Dim yAxisMin As Decimal = Decimal.MaxValue
        Dim yAxisMax As Decimal = Decimal.MinValue

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            ' 0 はプロットするが、0 以下の場合は null 表示(中抜け)
            If kv.Value.Count > 0 AndAlso kv.Value.Values(0).RecordDate = Date.MinValue AndAlso kv.Value.Values(0).Value1 >= 0 Then
                Dim value As Decimal = kv.Value.Values(0).Value1

                values.Add(value.ToString())

                ' Y 軸の最小値と最大値を更新
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                values.Add("null")
            End If
        Next

        ' Y 軸の範囲を少し広げる
        If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
            Me._yAxisMin = Math.Max(yAxisMin - 1, 0)
            Me._yAxisMax = Math.Max(yAxisMax + 1, 0)
        End If

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

#End Region

#Region "Public Method"

#End Region

End Class
