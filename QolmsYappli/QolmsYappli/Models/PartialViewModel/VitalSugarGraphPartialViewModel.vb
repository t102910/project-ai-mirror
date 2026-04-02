''' <summary>
''' 「バイタル」画面の、
''' 血糖値グラフ パーシャル ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class VitalSugarGraphPartialViewModel
    Inherits QyVitalGraphPartialViewModelBase

#Region "Constructor"

    ' ''' <summary>
    ' ''' 「バイタル」画面ビュー モデルおよび値を指定して、
    ' ''' <see cref="VitalSugarGraphPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ' ''' </summary>
    ' ''' <param name="model">「バイタル」画面ビュー モデル。</param>
    ' ''' <param name="startDate">表示開始日。</param>
    ' ''' <param name="endDate">表示終了日。</param>
    ' ''' <param name="items">血糖値情報のコレクション。</param>
    ' ''' <param name="targetValueLower1">空腹時目標下限値。</param>
    ' ''' <param name="targetValueUpper1">空腹時目標上限値。</param>
    ' ''' <param name="targetValueLower2">その他目標下限値。</param>
    ' ''' <param name="targetValueUpper2">その他目標上限値。</param>
    ' ''' <remarks></remarks>
    'Public Sub New(model As NoteVitalViewModel, startDate As Date, endDate As Date, items As IEnumerable(Of VitalValueItem), targetValueLower1 As Decimal, targetValueUpper1 As Decimal, targetValueLower2 As Decimal, targetValueUpper2 As Decimal)

    '    MyBase.New(model, Decimal.Zero, Decimal.Zero)

    '    Dim targetValues() As Decimal = {targetValueLower1, targetValueUpper1, targetValueLower2, targetValueUpper2}

    '    ' 評価情報リストの初期化
    '    Me._evaluationItemList.Clear()
    '    Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower1, targetValueUpper1, Decimal.Zero, "【空腹時】目標", "結果", "mg/dl")) ' 空腹時
    '    Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower2, targetValueUpper2, Decimal.Zero, "【その他】目標", "結果", "mg/dl")) ' その他

    '    ' Y 軸の最小値と最大値
    '    Me._yAxisMin = targetValues.Min()
    '    Me._yAxisMax = targetValues.Max()

    '    ' 目標値
    '    Me._targetValue = String.Format("[{0}]", String.Join(",", targetValues))

    '    Me.InitializeBy(startDate, endDate, items)

    'End Sub

    ''' <summary>
    ''' 「バイタル」画面ビュー モデルおよび値を指定して、
    ''' <see cref="VitalSugarGraphPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">「バイタル」画面ビュー モデル。</param>
    ''' <param name="items">血糖値情報のコレクション。</param>
    ''' <param name="targetValueLower1">空腹時目標下限値。</param>
    ''' <param name="targetValueUpper1">空腹時目標上限値。</param>
    ''' <param name="targetValueLower2">その他目標下限値。</param>
    ''' <param name="targetValueUpper2">その他目標上限値。</param>
    ''' <remarks></remarks>
    Public Sub New(model As NoteVitalViewModel, items As IEnumerable(Of VitalValueItem), targetValueLower1 As Decimal, targetValueUpper1 As Decimal, targetValueLower2 As Decimal, targetValueUpper2 As Decimal)

        MyBase.New(model, Decimal.Zero, Decimal.Zero)

        Dim targetValues() As Decimal = {targetValueLower1, targetValueUpper1, targetValueLower2, targetValueUpper2}

        ' 評価情報リストの初期化
        Me._evaluationItemList.Clear()
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower1, targetValueUpper1, Decimal.Zero, "【空腹時】目標", "結果", "mg/dl")) ' 空腹時
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower2, targetValueUpper2, Decimal.Zero, "【その他】目標", "結果", "mg/dl")) ' その他

        ' Y 軸の最小値と最大値
        Me._yAxisMin = targetValues.Min()
        Me._yAxisMax = targetValues.Max()

        ' 目標値
        Me._targetValue = String.Format("[{0}]", String.Join(",", targetValues))

        Me.InitializeBy(items)

    End Sub

#End Region

#Region "Protected Method"

    ' ''' <summary>
    ' ''' <see cref="VitalSugarGraphPartialViewModel" /> クラスのインスタンスを初期化します。
    ' ''' </summary>
    ' ''' <param name="startDate">表示開始日。</param>
    ' ''' <param name="endDate">表示終了日。</param>
    ' ''' <param name="items">血糖値情報のコレクション。</param>
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

    '        ' 平均値を算出
    '        For a As Integer = 0 To colCount - 1
    '            Dim values As SortedDictionary(Of Date, VitalValueItem) = Me._itemList.Values(a)
    '            Dim average1 As Decimal = 0 ' 空腹時平均
    '            Dim average2 As Decimal = 0 ' その他平均
    '            Dim count1 As Integer = 0 ' 空腹時個数
    '            Dim count2 As Integer = 0 ' その他個数

    '            If values.Count > 0 Then
    '                For Each kv As KeyValuePair(Of Date, VitalValueItem) In values
    '                    If kv.Value.ConditionType = QyVitalConditionTypeEnum.Fasting Then
    '                        ' 空腹時累計
    '                        If kv.Value.Value1 > 0 Then
    '                            average1 += kv.Value.Value1
    '                            count1 += 1
    '                        End If
    '                    Else
    '                        ' その他累計
    '                        If kv.Value.Value1 > 0 Then
    '                            average2 += kv.Value.Value1
    '                            count2 += 1
    '                        End If
    '                    End If
    '                Next

    '                ' 空腹時平均（小数点第 2 位で切り捨て）
    '                If count1 > 0 Then
    '                    average1 = Math.Truncate(average1 / count1 * 10) / 10

    '                    ' 評価情報を新しい日付の平均値で置き換え
    '                    Me._evaluationItemList(0).Value = average1
    '                End If

    '                ' その他平均（小数点第 2 位で切り捨て）
    '                If count2 > 0 Then
    '                    average2 = Math.Truncate(average2 / count2 * 10) / 10

    '                    ' 評価情報を新しい日付の平均値で置き換え
    '                    Me._evaluationItemList(1).Value = average2
    '                End If
    '            End If

    '            ' その他平均値を先頭に挿入
    '            Me._itemList.Values(a).Add(
    '                Date.MinValue.AddSeconds(1),
    '                New VitalValueItem() With {
    '                    .RecordDate = Date.MinValue.AddSeconds(1),
    '                    .Value1 = average2,
    '                    .Value2 = Decimal.MinValue
    '                }
    '            )

    '            ' 空腹時平均値を先頭に挿入
    '            Me._itemList.Values(a).Add(
    '                Date.MinValue,
    '                New VitalValueItem() With {
    '                    .RecordDate = Date.MinValue,
    '                    .Value1 = average1,
    '                    .Value2 = Decimal.MinValue
    '                }
    '            )
    '        Next

    '        ' 有効な評価情報だけ残す
    '        Me._evaluationItemList = Me._evaluationItemList.Where(Function(i) i.IsEvaluable).ToList()

    '        ' 平均値行の分を足す
    '        Me._rowCount += 2

    '        ' グラフ用データを作成
    '        Me._graphData = Me.CreateGraphData()
    '    End If

    'End Sub

    ''' <summary>
    ''' <see cref="VitalSugarGraphPartialViewModel" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="items">血糖値情報のコレクション。</param>
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

            ' 平均値を算出
            For a As Integer = 0 To Me._itemList.Count - 1
                Dim values As SortedDictionary(Of Date, VitalValueItem) = Me._itemList.Values(a)
                Dim average1 As Decimal = 0 ' 空腹時平均
                Dim average2 As Decimal = 0 ' その他平均
                Dim count1 As Integer = 0 ' 空腹時個数
                Dim count2 As Integer = 0 ' その他個数

                If values.Count > 0 Then
                    For Each kv As KeyValuePair(Of Date, VitalValueItem) In values
                        If kv.Value.ConditionType = QyVitalConditionTypeEnum.Fasting Then
                            ' 空腹時累計
                            If kv.Value.Value1 > 0 Then
                                average1 += kv.Value.Value1
                                count1 += 1
                            End If
                        Else
                            ' その他累計
                            If kv.Value.Value1 > 0 Then
                                average2 += kv.Value.Value1
                                count2 += 1
                            End If
                        End If
                    Next

                    ' 空腹時平均（小数点第 2 位で切り捨て）
                    If count1 > 0 Then
                        average1 = Math.Truncate(average1 / count1 * 10) / 10

                        ' 評価情報を新しい日付の平均値で置き換え
                        Me._evaluationItemList(0).Value = average1
                    End If

                    ' その他平均（小数点第 2 位で切り捨て）
                    If count2 > 0 Then
                        average2 = Math.Truncate(average2 / count2 * 10) / 10

                        ' 評価情報を新しい日付の平均値で置き換え
                        Me._evaluationItemList(1).Value = average2
                    End If
                End If

                ' その他平均値を先頭に挿入
                Me._itemList.Values(a).Add(
                    Date.MinValue.AddSeconds(1),
                    New VitalValueItem() With {
                        .RecordDate = Date.MinValue.AddSeconds(1),
                        .Value1 = average2,
                        .Value2 = Decimal.MinValue
                    }
                )

                ' 空腹時平均値を先頭に挿入
                Me._itemList.Values(a).Add(
                    Date.MinValue,
                    New VitalValueItem() With {
                        .RecordDate = Date.MinValue,
                        .Value1 = average1,
                        .Value2 = Decimal.MinValue
                    }
                )
            Next

            ' 有効な評価情報だけ残す
            Me._evaluationItemList = Me._evaluationItemList.Where(Function(i) i.IsEvaluable).ToList()

            ' 平均値行の分を足す
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

        Dim values1 As New List(Of String)() ' 空腹時平均
        Dim values2 As New List(Of String)() ' その他平均

        Dim yAxisMin As Decimal = Decimal.MaxValue
        Dim yAxisMax As Decimal = Decimal.MinValue

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            ' 空腹時平均
            If kv.Value.Count > 0 AndAlso kv.Value.Values(0).RecordDate = Date.MinValue AndAlso kv.Value.Values(0).Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values(0).Value1

                values1.Add(value.ToString())

                ' Y 軸の最小値と最大値を更新
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                values1.Add("null")
            End If

            ' その他平均
            If kv.Value.Count > 1 AndAlso kv.Value.Values(1).RecordDate = Date.MinValue.AddSeconds(1) AndAlso kv.Value.Values(1).Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values(1).Value1

                values2.Add(value.ToString())

                ' Y 軸の最小値と最大値を更新
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                values2.Add("null")
            End If
        Next

        ' Y 軸の範囲を少し広げる
        If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
            Me._yAxisMin = Math.Max(yAxisMin - 1, 0)
            Me._yAxisMax = Math.Max(yAxisMax + 1, 0)
        End If

        Return String.Format("[[{0}],[{1}]]", String.Join(","c, values1), String.Join(","c, values2)) ' "[[空腹時の配列],[その他の配列]]"

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

#End Region

End Class
