''' <summary>
''' 「歩く」画面の、
''' 歩数グラフ パーシャル ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class VitalStepsGraphPartialViewModel
    Inherits QyVitalGraphPartialViewModelBase

#Region "Constructor"

    ' ''' <summary>
    ' ''' 「歩く」画面ビュー モデルおよび値を指定して、
    ' ''' <see cref="VitalStepsGraphPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ' ''' </summary>
    ' ''' <param name="model">「歩く」画面ビュー モデル。</param>
    ' ''' <param name="startDate">表示開始日。</param>
    ' ''' <param name="endDate">表示終了日。</param>
    ' ''' <param name="items">歩数情報のコレクション。</param>
    ' ''' <param name="targetValueLower">目標下限値。</param>
    ' ''' <param name="targetValueUpper">目標上限値。</param>
    ' ''' <remarks></remarks>
    'Public Sub New(model As NoteWalkViewModel, startDate As Date, endDate As Date, items As IEnumerable(Of VitalValueItem), targetValueLower As Decimal, targetValueUpper As Decimal)

    '    MyBase.New(model, targetValueLower, targetValueUpper)

    '    Me.InitializeBy(startDate, endDate, items)

    'End Sub

    ''' <summary>
    ''' 「歩く」画面ビュー モデルおよび値を指定して、
    ''' <see cref="VitalStepsGraphPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">「歩く」画面ビュー モデル。</param>
    ''' <param name="items">歩数情報のコレクション。</param>
    ''' <param name="targetValueLower">目標下限値。</param>
    ''' <param name="targetValueUpper">目標上限値。</param>
    ''' <remarks></remarks>
    Public Sub New(model As NoteWalkViewModel, items As IEnumerable(Of VitalValueItem), targetValueLower As Decimal, targetValueUpper As Decimal)

        MyBase.New(model, targetValueLower, targetValueUpper)

        Me.InitializeBy(items)

    End Sub

#End Region

#Region "Protected Method"

    ' ''' <summary>
    ' ''' <see cref="VitalStepsGraphPartialViewModel" /> クラスのインスタンスを初期化します。
    ' ''' </summary>
    ' ''' <param name="startDate">表示開始日。</param>
    ' ''' <param name="endDate">表示終了日。</param>
    ' ''' <param name="items">歩数情報のコレクション。</param>
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

    '            ' 歩数は 1 日あたり 1 件（0 時 0 分）
    '            If Me._itemList.ContainsKey(key) AndAlso item.RecordDate = item.RecordDate.Date AndAlso Not Me._itemList(key).ContainsKey(item.RecordDate) Then
    '                'Me._itemList(key).Add(item.ActiveDate, item)

    '                '' 最大行数
    '                'If Me._rowCount < Me._itemList(key).Count Then Me._rowCount = Me._itemList(key).Count

    '                With Me._itemList(key)
    '                    .Add(item.RecordDate, item)

    '                    ' 最大行数
    '                    If Me._rowCount < .Count Then Me._rowCount = .Count

    '                    ' 評価情報を新しい日付の平均値で置き換え
    '                    If item.Value1 > Decimal.Zero Then Me._evaluationItemList(0).Value = item.Value1
    '                End With
    '            End If
    '        Next

    '        ' 有効な評価情報だけ残す
    '        If Not Me._evaluationItemList(0).IsEvaluable Then Me._evaluationItemList.RemoveAt(0)

    '        ' グラフ用データを作成
    '        Me._graphData = Me.CreateGraphData()
    '    End If

    'End Sub

    ''' <summary>
    ''' <see cref="VitalStepsGraphPartialViewModel" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="items">歩数情報のコレクション。</param>
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

                ' 歩数は 1 日あたり 1 件（0 時 0 分）
                If Me._itemList.ContainsKey(key) AndAlso item.RecordDate = item.RecordDate.Date AndAlso Not Me._itemList(key).ContainsKey(item.RecordDate) Then
                    'Me._itemList(key).Add(item.ActiveDate, item)

                    '' 最大行数
                    'If Me._rowCount < Me._itemList(key).Count Then Me._rowCount = Me._itemList(key).Count

                    With Me._itemList(key)
                        .Add(item.RecordDate, item)

                        ' 最大行数
                        If Me._rowCount < .Count Then Me._rowCount = .Count

                        ' 評価情報を新しい日付の平均値で置き換え
                        If item.Value1 > Decimal.Zero Then Me._evaluationItemList(0).Value = item.Value1
                    End With
                End If
            Next

            ' 有効な評価情報だけ残す
            If Not Me._evaluationItemList(0).IsEvaluable Then Me._evaluationItemList.RemoveAt(0)

            ' グラフ用データを作成
            Me._graphData = Me.CreateGraphData()
        End If

    End Sub

    ' ''' <summary>
    ' ''' ビューに展開するグラフ値を表す文字列を作成します。
    ' ''' </summary>
    ' ''' <returns>
    ' ''' バイタル値データ。
    ' ''' [値1, 値2, ...] の形式（値が無いときは値として null もしくは空白を指定）。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Protected Overrides Function CreateGraphData() As String

    '    Dim values As New List(Of String)()

    '    ' TODO:
    '    Dim yAxisMin As Decimal = Decimal.MaxValue
    '    Dim yAxisMax As Decimal = Decimal.MinValue

    '    For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
    '        If kv.Value.Count = 1 AndAlso kv.Value.Values.First().RecordDate = kv.Key AndAlso kv.Value.Values.First().Value1 > 0 Then
    '            Dim value As Decimal = kv.Value.Values.First().Value1

    '            values.Add(value.ToString())

    '            ' Y 軸の最小値と最大値を更新
    '            'Me._yAxisMin = Math.Min(Me._yAxisMin, value)
    '            'Me._yAxisMax = Math.Max(Me._yAxisMax, value)
    '            yAxisMin = Math.Min(yAxisMin, value)
    '            yAxisMax = Math.Max(yAxisMax, value)
    '        Else
    '            values.Add("null")
    '        End If
    '    Next

    '    ' Y 軸の範囲を少し広げる
    '    'Me._yAxisMin = Math.Max(Me._yAxisMin - 5, 0)
    '    'Me._yAxisMax = Math.Max(Me._yAxisMax + 5, 0)
    '    If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
    '        Me._yAxisMin = 0 'Math.Max(yAxisMin - 1, 0)
    '        Me._yAxisMax = Math.Max(yAxisMax + 1, 0)
    '    End If

    '    Return String.Format("[{0}]", String.Join(","c, values))

    'End Function

    ''' <summary>
    ''' ビューに展開するグラフ値を表す文字列を作成します。
    ''' </summary>
    ''' <returns>
    ''' バイタル値データ。
    ''' [値1, 値2, ...] の形式（値が無いときは値として null もしくは空白を指定）。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateGraphData() As String

        Dim values As New List(Of String)()

        ' TODO:
        Dim yAxisMin As Decimal = Decimal.MaxValue
        Dim yAxisMax As Decimal = Decimal.MinValue

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            If kv.Value.Count = 1 AndAlso kv.Value.Values.First().RecordDate = kv.Key AndAlso kv.Value.Values.First().Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values.First().Value1

                values.Add(String.Format("['{0:yyyy年M月d日}', {1}]", kv.Key, value))

                ' Y 軸の最小値と最大値を更新
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                values.Add("[null, null]")
            End If
        Next

        ' Y 軸の範囲を少し広げる
        If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
            Me._yAxisMin = 0
            Me._yAxisMax = Math.Max(yAxisMax + 1, 0)
        End If

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

#End Region

End Class
