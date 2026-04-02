''' <summary>
''' 「バイタル」画面の、
''' 体重グラフ パーシャル ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class VitalWeightGraphPartialViewModel
    Inherits QyVitalGraphPartialViewModelBase

#Region "Variable"

    ''' <summary>
    ''' 身長（cm）を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private height As Decimal = Decimal.MinusOne

    ''' <summary>
    ''' ビューに展開する BMI 用 Y 軸の最小値を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _yAxisBmiMin As Decimal = Decimal.Zero

    ''' <summary>
    ''' ビューに展開する BMI 用 Y 軸の最大値を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _yAxisBmiMax As Decimal = Decimal.Zero

    ' 以下、健康年齢用

    ''' <summary>
    ''' ビューに展開する健康年齢情報のディクショナリを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _ageItemList As New SortedDictionary(Of Date, SortedDictionary(Of Date, VitalValueItem))()

    ' ''' <summary>
    ' ''' ビューに展開する健康年齢評価情報のリストを保持します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Protected _ageEvaluationItemList As New List(Of VitalEvaluationItem)()

    ''' <summary>
    ''' ビューに展開する健康年齢表の行数を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _ageRowCount As Integer = 0

    ''' <summary>
    ''' ビューに展開する健康年齢グラフの Y 軸の最小値を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _yAxisAgeMin As Decimal = Decimal.Zero

    ''' <summary>
    ''' ビューに展開する健康年齢グラフの Y 軸の最大値を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _yAxisAgeMax As Decimal = Decimal.Zero

    ''' <summary>
    ''' ビューに展開する健康年齢グラフの値を表す文字列を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _ageGraphData As String = "[]"

    ''' <summary>
    ''' ビューに展開する健康年齢グラフのラベルを表す文字列を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _ageGraphLabel As String = "[]"

#End Region

#Region "Constructor"

    ' ''' <summary>
    ' ''' 「バイタル」画面ビュー モデルおよび値を指定して、
    ' ''' <see cref="VitalWeightGraphPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ' ''' </summary>
    ' ''' <param name="model">「バイタル」画面ビュー モデル。</param>
    ' ''' <param name="startDate">表示開始日。</param>
    ' ''' <param name="endDate">表示終了日。</param>
    ' ''' <param name="items">体重情報のコレクション。</param>
    ' ''' <param name="height">身長（cm）。</param>
    ' ''' <param name="targetValueLower1">体重目標下限値。</param>
    ' ''' <param name="targetValueUpper1">体重目標上限値。</param>
    ' ''' <param name="targetValueLower2">BMI 目標下限値。</param>
    ' ''' <param name="targetValueUpper2">BMI 目標上限値。</param>
    ' ''' <remarks></remarks>
    'Public Sub New(
    '    model As NoteVitalViewModel,
    '    startDate As Date,
    '    endDate As Date,
    '    items As IEnumerable(Of VitalValueItem),
    '    height As Decimal,
    '    targetValueLower1 As Decimal,
    '    targetValueUpper1 As Decimal,
    '    targetValueLower2 As Decimal,
    '    targetValueUpper2 As Decimal
    ')

    '    MyBase.New(model, Decimal.Zero, Decimal.Zero)

    '    ' 評価情報リストの初期化
    '    Me._evaluationItemList.Clear()
    '    Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower1, targetValueUpper1, Decimal.Zero, "【体重】目標", "結果", "kg")) ' 体重
    '    Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower2, targetValueUpper2, Decimal.Zero, "【BMI】目標", "結果", String.Empty)) ' BMI

    '    ' Y 軸の最小値と最大値
    '    Me._yAxisMin = {targetValueLower1, targetValueUpper1}.Min()
    '    Me._yAxisMax = {targetValueLower1, targetValueUpper1}.Max()

    '    Me._yAxisBmiMin = {targetValueLower2, targetValueUpper2}.Min()
    '    Me._yAxisBmiMax = {targetValueLower2, targetValueUpper2}.Max()

    '    ' 目標値
    '    Me._targetValue = String.Format("[{0}]", String.Join(",", {targetValueLower1, targetValueUpper1, targetValueLower2, targetValueUpper2}.ToArray()))

    '    ' 身長（BMI 計算用）
    '    Me.height = height

    '    Me.InitializeBy(startDate, endDate, items)

    'End Sub

    ''' <summary>
    ''' 「バイタル」画面ビュー モデルおよび値を指定して、
    ''' <see cref="VitalWeightGraphPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">「バイタル」画面ビュー モデル。</param>
    ''' <param name="items">体重情報のコレクション。</param>
    ''' <param name="height">身長（cm）。</param>
    ''' <param name="targetValueLower1">体重目標下限値。</param>
    ''' <param name="targetValueUpper1">体重目標上限値。</param>
    ''' <param name="targetValueLower2">BMI 目標下限値。</param>
    ''' <param name="targetValueUpper2">BMI 目標上限値。</param>
    ''' <param name="ageItems">健康年齢情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(
        model As NoteVitalViewModel,
        items As IEnumerable(Of VitalValueItem),
        height As Decimal,
        targetValueLower1 As Decimal,
        targetValueUpper1 As Decimal,
        targetValueLower2 As Decimal,
        targetValueUpper2 As Decimal,
        ageItems As IEnumerable(Of VitalValueItem)
    )

        MyBase.New(model, Decimal.Zero, Decimal.Zero)

        ' 評価情報リストの初期化
        Me._evaluationItemList.Clear()
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower1, targetValueUpper1, Decimal.Zero, "【体重】目標", "結果", "kg")) ' 体重
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower2, targetValueUpper2, Decimal.Zero, "【BMI】目標", "結果", String.Empty)) ' BMI

        ' Y 軸の最小値と最大値
        Me._yAxisMin = {targetValueLower1, targetValueUpper1}.Min()
        Me._yAxisMax = {targetValueLower1, targetValueUpper1}.Max()

        Me._yAxisBmiMin = {targetValueLower2, targetValueUpper2}.Min()
        Me._yAxisBmiMax = {targetValueLower2, targetValueUpper2}.Max()

        ' 目標値
        Me._targetValue = String.Format("[{0}]", String.Join(",", {targetValueLower1, targetValueUpper1, targetValueLower2, targetValueUpper2}.ToArray()))

        ' 身長（BMI 計算用）
        Me.height = height

        Me.InitializeBy(items)

        ' 以下、健康年齢用

        ' Y 軸の最小値と最大値
        Me._yAxisAgeMin = {Byte.MinValue, Byte.MaxValue}.Min()
        Me._yAxisAgeMax = {Byte.MinValue, Byte.MaxValue}.Max()

        Me.InitializeByAge(ageItems)
    End Sub

#End Region

#Region "Public Property"

    ''' <summary>
    ''' ビューに展開する BMI 用 Y 軸の最小値を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisBmiMin As Decimal

        Get
            Return Me._yAxisBmiMin
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する BMI 用 Y 軸の最大値を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisBmiMax As Decimal

        Get
            Return Me._yAxisBmiMax
        End Get

    End Property

    ' 以下、健康年齢用

    ''' <summary>
    ''' ビューに展開する健康年齢情報のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AgeItemList As SortedDictionary(Of Date, SortedDictionary(Of Date, VitalValueItem))

        Get
            Return Me._ageItemList
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する健康年齢用 Y 軸の最小値を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisAgeMin As Decimal

        Get
            Return Me._yAxisAgeMin
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する健康年齢用 Y 軸の最大値を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisAgeMax As Decimal

        Get
            Return Me._yAxisAgeMax
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する健康年齢表の行数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AgeRowCount As Integer

        Get
            Return Me._ageRowCount
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する健康年齢グラフ値を表す文字列を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AgeGraphData As String

        Get
            Return Me._ageGraphData
        End Get

    End Property

    ''' <summary>
    ''' ビューに展開する健康年齢グラフ ラベルを表す文字列を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AgeGraphLabel As String

        Get
            Return Me._ageGraphLabel
        End Get

    End Property

#End Region

#Region "Protected Method"

    ' ''' <summary>
    ' ''' <see cref="VitalWeightGraphPartialViewModel" /> クラスのインスタンスを初期化します。
    ' ''' </summary>
    ' ''' <param name="startDate">表示開始日。</param>
    ' ''' <param name="endDate">表示終了日。</param>
    ' ''' <param name="items">体重情報のコレクション。</param>
    ' ''' <remarks></remarks>
    'Protected Overrides Sub InitializeBy(startDate As Date, endDate As Date, items As IEnumerable(Of VitalValueItem))

    '    If startDate <= endDate AndAlso items IsNot Nothing AndAlso items.Count() > 0 Then
    '        Dim st As Date = New Date(startDate.Year, startDate.Month, startDate.Day)
    '        Dim ed As Date = New Date(endDate.Year, endDate.Month, endDate.Day)
    '        Dim colCount As Integer = ed.Subtract(st).Days + 1

    '        ' 日付のディクショナリを作成
    '        For a As Integer = 0 To colCount - 1
    '            Me._itemList.Add(st.AddDays(a), New SortedDictionary(Of Date, VitalValueItem)())
    '        Next

    '        ' 各日付に測定値を追加
    '        For Each item As VitalValueItem In items
    '            Dim key As Date = New Date(item.RecordDate.Year, item.RecordDate.Month, item.RecordDate.Day)

    '            If Me._itemList.ContainsKey(key) AndAlso Not Me._itemList(key).ContainsKey(item.RecordDate) Then
    '                Me._itemList(key).Add(item.RecordDate, item)

    '                ' 最大行数
    '                If Me._rowCount < Me._itemList(key).Count Then Me._rowCount = Me._itemList(key).Count
    '            End If
    '        Next

    '        ' BMI と体重平均を算出
    '        For a As Integer = 0 To colCount - 1
    '            Dim values As SortedDictionary(Of Date, VitalValueItem) = Me._itemList.Values(a)
    '            Dim bmi As Decimal = 0 ' BMI
    '            Dim average As Decimal = 0 ' 体重平均
    '            Dim count As Integer = 0 ' 体重個数

    '            If values.Count > 0 Then
    '                For Each kv As KeyValuePair(Of Date, VitalValueItem) In values
    '                    ' 体重累計
    '                    If kv.Value.Value1 > 0 Then
    '                        average += kv.Value.Value1
    '                        count += 1
    '                    End If
    '                Next

    '                ' BMI と体重平均を算出（小数点第 2 位で切り捨て）
    '                If count > 0 Then
    '                    average = average / count

    '                    ' BMI
    '                    If average > 0 AndAlso Me.height > 0 Then
    '                        bmi = Math.Truncate(average / (Me.height * Me.height) * 100000) / 10

    '                        ' 評価情報を新しい日付の BMI で置き換え
    '                        Me._evaluationItemList(1).Value = bmi
    '                    End If

    '                    ' 平均
    '                    average = Math.Truncate(average * 10) / 10

    '                    ' 評価情報を新しい日付の平均値で置き換え
    '                    Me._evaluationItemList(0).Value = average
    '                End If
    '            End If

    '            ' BMI を先頭に挿入
    '            Me._itemList.Values(a).Add(
    '                Date.MinValue.AddSeconds(1),
    '                New VitalValueItem() With {
    '                    .RecordDate = Date.MinValue.AddSeconds(1),
    '                    .Value1 = bmi,
    '                    .Value2 = Decimal.MinValue
    '                }
    '            )

    '            ' 体重平均を先頭に挿入
    '            Me._itemList.Values(a).Add(
    '                Date.MinValue,
    '                New VitalValueItem() With {
    '                    .RecordDate = Date.MinValue,
    '                    .Value1 = average,
    '                    .Value2 = Decimal.MinValue
    '                }
    '            )
    '        Next

    '        ' 有効な評価情報だけ残す
    '        Me._evaluationItemList = Me._evaluationItemList.Where(Function(i) i.IsEvaluable).ToList()

    '        ' 体重平均行とBMI行の分を足す
    '        Me._rowCount += 2

    '        ' グラフ用データを作成
    '        Me._graphData = Me.CreateGraphData()
    '    End If

    'End Sub

    ''' <summary>
    ''' <see cref="VitalWeightGraphPartialViewModel" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="items">体重情報のコレクション。</param>
    ''' <remarks></remarks>
    Protected Overrides Sub InitializeBy(items As IEnumerable(Of VitalValueItem))

        If items IsNot Nothing AndAlso items.Count() > 0 Then
            ' 日付のディクショナリを作成
            For Each item As VitalValueItem In items
                Dim key As Date = item.RecordDate.Date

                If Not Me._itemList.ContainsKey(key) Then Me._itemList.Add(key, New SortedDictionary(Of Date, VitalValueItem)())
            Next

            ' 各日付に測定値を追加
            For Each item As VitalValueItem In items
                Dim key As Date = New Date(item.RecordDate.Year, item.RecordDate.Month, item.RecordDate.Day)

                If Me._itemList.ContainsKey(key) AndAlso Not Me._itemList(key).ContainsKey(item.RecordDate) Then
                    Me._itemList(key).Add(item.RecordDate, item)

                    ' 最大行数
                    If Me._rowCount < Me._itemList(key).Count Then Me._rowCount = Me._itemList(key).Count
                End If
            Next

            ' BMI と体重平均を算出
            For a As Integer = 0 To Me._itemList.Count - 1
                Dim values As SortedDictionary(Of Date, VitalValueItem) = Me._itemList.Values(a)
                Dim bmi As Decimal = 0 ' BMI
                Dim weightAverage As Decimal = 0 ' 体重平均
                Dim weightCount As Integer = 0 ' 体重個数
                Dim heightAverage As Decimal = 0 ' 身長平均
                Dim heightCount As Integer = 0 ' 身長個数

                If values.Count > 0 Then
                    For Each kv As KeyValuePair(Of Date, VitalValueItem) In values
                        ' 体重累計（平均への変換は後ほど）
                        If kv.Value.Value1 > 0 Then
                            weightAverage += kv.Value.Value1
                            weightCount += 1
                        End If

                        ' 身長累計（平均への変換は後ほど）
                        If kv.Value.Value2 > 0 Then
                            heightAverage += kv.Value.Value2
                            heightCount += 1
                        End If
                    Next

                    ' BMI と体重平均を算出（小数点第 2 位で切り捨て）
                    If weightCount > 0 Then
                        ' 体重平均
                        weightAverage = weightAverage / weightCount

                        ' BMI
                        If weightAverage > 0 Then
                            If heightCount > 0 Then
                                ' 体重と対になる身長を使用

                                ' 身長平均
                                heightAverage = heightAverage / heightCount

                                bmi = Math.Truncate(weightAverage / (heightAverage * heightAverage) * 100000) / 10

                                ' 評価情報を新しい日付の BMI で置き換え
                                Me._evaluationItemList(1).Value = bmi
                            ElseIf Me.height > 0 Then
                                ' コンストラクタ で渡された身長を使用
                                bmi = Math.Truncate(weightAverage / (Me.height * Me.height) * 100000) / 10

                                ' 評価情報を新しい日付の BMI で置き換え
                                Me._evaluationItemList(1).Value = bmi
                            End If
                        End If

                        ' 平均
                        weightAverage = Math.Truncate(weightAverage * 10) / 10

                        ' 評価情報を新しい日付の平均値で置き換え
                        Me._evaluationItemList(0).Value = weightAverage
                    End If
                End If

                ' BMI を先頭に挿入
                Me._itemList.Values(a).Add(
                    Date.MinValue.AddSeconds(1),
                    New VitalValueItem() With {
                        .RecordDate = Date.MinValue.AddSeconds(1),
                        .Value1 = bmi,
                        .Value2 = Decimal.MinValue
                    }
                )

                ' 体重平均を先頭に挿入
                Me._itemList.Values(a).Add(
                    Date.MinValue,
                    New VitalValueItem() With {
                        .RecordDate = Date.MinValue,
                        .Value1 = weightAverage,
                        .Value2 = Decimal.MinValue
                    }
                )
            Next

            ' 有効な評価情報だけ残す
            Me._evaluationItemList = Me._evaluationItemList.Where(Function(i) i.IsEvaluable).ToList()

            ' 体重平均行とBMI行の分を足す
            Me._rowCount += 2

            ' グラフ用データを作成
            Me._graphData = Me.CreateGraphData()

            ' グラフ用ラベルを作成
            Me._graphLabel = Me.CreateGraphLabel()
        End If

    End Sub

    ''' <summary>
    ''' ビューに展開するグラフ値を表す文字列を作成します。
    ''' </summary>
    ''' <returns>
    ''' グラフ値を表す文字列。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateGraphData() As String

        Dim values1 As New List(Of String)() ' 体重平均
        Dim values2 As New List(Of String)() ' BMI

        ' TODO:
        Dim yAxisMin As Decimal = Decimal.MaxValue
        Dim yAxisMax As Decimal = Decimal.MinValue
        Dim yAxisBmiMin As Decimal = Decimal.MaxValue
        Dim yAxisBmiMax As Decimal = Decimal.MinValue

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            ' 体重平均
            If kv.Value.Count > 0 AndAlso kv.Value.Values(0).RecordDate = Date.MinValue AndAlso kv.Value.Values(0).Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values(0).Value1

                values1.Add(value.ToString())

                ' Y 軸の最小値と最大値を更新
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                values1.Add("null")
            End If

            ' BMI
            If kv.Value.Count > 1 AndAlso kv.Value.Values(1).RecordDate = Date.MinValue.AddSeconds(1) AndAlso kv.Value.Values(1).Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values(1).Value1

                values2.Add(value.ToString())

                ' Y 軸の最小値と最大値を更新
                yAxisBmiMin = Math.Min(yAxisBmiMin, value)
                yAxisBmiMax = Math.Max(yAxisBmiMax, value)
            Else
                values2.Add("null")
            End If
        Next

        ' Y 軸の範囲を少し広げる
        If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
            Me._yAxisMin = Math.Max(yAxisMin - 1, 0)
            Me._yAxisMax = Math.Max(yAxisMax + 1, 0)
        End If

        If yAxisBmiMin <> Decimal.MaxValue AndAlso yAxisBmiMax <> Decimal.MinValue Then
            Me._yAxisBmiMin = Math.Max(yAxisBmiMin - 1, 0)
            Me._yAxisBmiMax = Math.Max(yAxisBmiMax + 1, 0)
        End If

        Return String.Format("[[{0}],[{1}]]", String.Join(","c, values1), String.Join(","c, values2)) ' "[[体重の配列],[BMIの配列]]"

    End Function

    ''' <summary>
    ''' ビューに展開するグラフ ラベルを表す文字列を作成します。
    ''' </summary>
    ''' <returns>
    ''' ラベル データ。
    ''' [値1, 値2, ...] の形式（値が無いときは値として null もしくは空白を指定）。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateGraphLabel() As String

        Dim values As New List(Of String)()

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            If kv.Value.Count > 0 AndAlso kv.Value.Values(0).RecordDate = Date.MinValue AndAlso kv.Value.Values(0).Value1 > 0 Then
                values.Add(String.Format("'{0:yyyy/M/d}'", kv.Key))
            ElseIf kv.Value.Count > 1 AndAlso kv.Value.Values(1).RecordDate = Date.MinValue.AddSeconds(1) AndAlso kv.Value.Values(1).Value1 > 0 Then
                values.Add(String.Format("'{0:yyyy/M/d}'", kv.Key))
            Else
                values.Add("null")
            End If
        Next

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

    ' 健康年齢用
    Private Sub InitializeByAge(items As IEnumerable(Of VitalValueItem))

        If items IsNot Nothing AndAlso items.Count() > 0 Then
            ' 日付のディクショナリを作成
            For Each item As VitalValueItem In items
                Dim key As Date = item.RecordDate.Date

                If Not Me._ageItemList.ContainsKey(key) Then Me._ageItemList.Add(key, New SortedDictionary(Of Date, VitalValueItem)())
            Next

            ' 各日付に測定値を追加
            For Each item As VitalValueItem In items
                Dim key As Date = New Date(item.RecordDate.Year, item.RecordDate.Month, item.RecordDate.Day)

                ' 健康年齢は 1 日あたり 1 件（0 時 0 分）
                If Me._ageItemList.ContainsKey(key) AndAlso item.RecordDate = item.RecordDate.Date AndAlso Not Me._ageItemList(key).ContainsKey(item.RecordDate) Then
                    With Me._ageItemList(key)
                        .Add(item.RecordDate, item)

                        ' 最大行数
                        If Me._ageRowCount < .Count Then Me._ageRowCount = .Count

                        '' 評価情報を新しい日付の平均値で置き換え
                        'If item.Value1 > Decimal.Zero Then Me._ageEvaluationItemList(0).Value = item.Value1
                    End With
                End If
            Next

            '' 有効な評価情報だけ残す
            'If Not Me._ageEvaluationItemList(0).IsEvaluable Then Me._ageEvaluationItemList.RemoveAt(0)

            ' グラフ用データを作成
            Me._ageGraphData = Me.CreateAgeGraphData()

            ' グラフ用ラベルを作成
            Me._ageGraphLabel = Me.CreateAgeGraphLabel()
        End If

    End Sub

    ' 健康年齢用
    Private Function CreateAgeGraphData() As String

        Dim values As New List(Of String)()

        ' TODO:
        Dim yAxisMin As Decimal = Decimal.MaxValue
        Dim yAxisMax As Decimal = Decimal.MinValue

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._ageItemList
            If kv.Value.Count = 1 AndAlso kv.Value.Values.First().RecordDate = kv.Key AndAlso kv.Value.Values.First().Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values.First().Value1

                'values.Add(String.Format("['{0:yyyy年M月d日}', {1}]", kv.Key, value))
                values.Add(String.Format("{0}", value))

                ' Y 軸の最小値と最大値を更新
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                'values.Add("[null, null]")
                values.Add("null")
            End If
        Next

        ' Y 軸の範囲を少し広げる
        If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
            Me._yAxisAgeMin = Math.Max(yAxisMin - 1, 0)
            Me._yAxisAgeMax = Math.Max(yAxisMax + 1, 0)
        End If

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

    ' 健康年齢用
    Private Function CreateAgeGraphLabel() As String

        Dim values As New List(Of String)()

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._ageItemList
            If kv.Value.Count = 1 AndAlso kv.Value.Values.First().RecordDate = kv.Key AndAlso kv.Value.Values.First().Value1 > 0 Then
                values.Add(String.Format("'{0:yyyy/M/d}'", kv.Key))
            Else
                values.Add("null")
            End If
        Next

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

#End Region

End Class
