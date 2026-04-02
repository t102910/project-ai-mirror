Imports System.Collections.ObjectModel

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' 「歩く」画面および「バイタル」画面の、
''' バイタル グラフ パーシャル ビュー モデルの基本クラスを表します。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class QyVitalGraphPartialViewModelBase
    Inherits QyPartialViewModelBase(Of QyPageViewModelBase)

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

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyVitalGraphPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 「歩く」画面ビュー モデルおよび目標値を指定して、
    ''' <see cref="QyVitalGraphPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">「歩く」画面ビュー モデル。</param>
    ''' <param name="targetValueLower">目標下限値。</param>
    ''' <param name="targetValueUpper">目標上限値。</param>
    ''' <remarks></remarks>
    Protected Sub New(model As NoteWalkViewModel, targetValueLower As Decimal, targetValueUpper As Decimal)

        MyBase.New(model)

        Dim targetValues() As Decimal = {targetValueLower, targetValueUpper}

        ' 評価情報リストの初期化
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower, targetValueUpper, Decimal.Zero))

        ' Y 軸の最小値と最大値の初期化
        Me._yAxisMin = targetValues.Min()
        Me._yAxisMax = targetValues.Max()

        ' 目標値
        Me._targetValue = String.Format("[{0}]", String.Join(",", targetValues))

    End Sub

    ''' <summary>
    ''' 「バイタル」画面ビュー モデルおよび目標値を指定して、
    ''' <see cref="QyVitalGraphPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">「バイタル」画面ビュー モデル。</param>
    ''' <param name="targetValueLower">目標下限値。</param>
    ''' <param name="targetValueUpper">目標上限値。</param>
    ''' <remarks></remarks>
    Protected Sub New(model As NoteVitalViewModel, targetValueLower As Decimal, targetValueUpper As Decimal)

        MyBase.New(model)

        Dim targetValues() As Decimal = {targetValueLower, targetValueUpper}

        ' 評価情報リストの初期化
        Me._evaluationItemList.Add(New VitalEvaluationItem(targetValueLower, targetValueUpper, Decimal.Zero))

        ' Y 軸の最小値と最大値の初期化
        Me._yAxisMin = targetValues.Min()
        Me._yAxisMax = targetValues.Max()

        ' 目標値
        Me._targetValue = String.Format("[{0}]", String.Join(",", targetValues))

    End Sub

#End Region

#Region "Protected Method"

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="QyVitalGraphPartialViewModelBase" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="startDate">表示開始日。</param>
    ''' <param name="endDate">表示終了日。</param>
    ''' <param name="items">バイタル情報のコレクション。</param>
    ''' <remarks></remarks>
    Protected Overridable Sub InitializeBy(startDate As Date, endDate As Date, items As IEnumerable(Of VitalValueItem))

        If startDate <= endDate AndAlso items IsNot Nothing AndAlso items.Count() > 0 Then
            Dim st As Date = New Date(startDate.Year, startDate.Month, startDate.Day)
            Dim ed As Date = New Date(endDate.Year, endDate.Month, endDate.Day)
            Dim colCount As Integer = ed.Subtract(st).Days + 1

            ' 日付のディクショナリを作成
            For a As Integer = 0 To colCount - 1
                Me._itemList.Add(st.AddDays(a), New SortedDictionary(Of Date, VitalValueItem)())
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
            For a As Integer = 0 To colCount - 1
                Dim values As SortedDictionary(Of Date, VitalValueItem) = Me._itemList.Values(a)
                Dim average As Decimal = Decimal.Zero ' 平均
                Dim count As Integer = 0 ' 個数

                If values.Count > 0 Then
                    For Each kv As KeyValuePair(Of Date, VitalValueItem) In values
                        ' 累計
                        If kv.Value.Value1 > 0 Then
                            average += kv.Value.Value1
                            count += 1
                        End If
                    Next

                    ' 平均（小数点第2位で切り捨て）
                    If count > 0 Then
                        average = Math.Truncate(average / count * 10) / 10

                        ' 評価情報を新しい日付の平均値で置き換え
                        Me._evaluationItemList(0).Value = average
                    End If
                End If

                ' 平均値を先頭に挿入（Value2 は未使用）
                Me._itemList.Values(a).Add(Date.MinValue, New VitalValueItem() With {.RecordDate = Date.MinValue, .Value1 = average, .Value2 = Decimal.Zero})
            Next

            ' 有効な評価情報だけ残す
            If Not Me._evaluationItemList(0).IsEvaluable Then Me._evaluationItemList.RemoveAt(0)

            ' 平均値行の分を足す
            Me._rowCount += 1

            ' グラフ用データを作成
            Me._graphData = Me.CreateGraphData()

            ' グラフ用ラベルを作成
            Me._graphLabel = Me.CreateGraphLabel()
        End If

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="QyVitalGraphPartialViewModelBase" /> クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="items">バイタル情報のコレクション。</param>
    ''' <remarks></remarks>
    Protected Overridable Sub InitializeBy(items As IEnumerable(Of VitalValueItem))

        ' TODO: 要実装

    End Sub

    ''' <summary>
    ''' ビューに展開するグラフ値を表す文字列を作成します。
    ''' </summary>
    ''' <returns>
    ''' バイタル値データ。
    ''' [値1, 値2, ...] の形式（値が無いときは値として null もしくは空白を指定）。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Overridable Function CreateGraphData() As String

        Dim values As New List(Of String)()

        ' TODO:
        Dim yAxisMin As Decimal = Decimal.MaxValue
        Dim yAxisMax As Decimal = Decimal.MinValue

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            If kv.Value.Count > 0 AndAlso kv.Value.Values(0).RecordDate = Date.MinValue AndAlso kv.Value.Values(0).Value1 > 0 Then
                Dim value As Decimal = kv.Value.Values(0).Value1

                values.Add(value.ToString())

                ' Y 軸の最小値と最大値を更新
                'Me._yAxisMin = Math.Min(Me._yAxisMin, value)
                'Me._yAxisMax = Math.Max(Me._yAxisMax, value)
                yAxisMin = Math.Min(yAxisMin, value)
                yAxisMax = Math.Max(yAxisMax, value)
            Else
                values.Add("null")
            End If
        Next

        ' Y 軸の範囲を少し広げる
        'Me._yAxisMin = Math.Max(Me._yAxisMin - 5, 0)
        'Me._yAxisMax = Math.Max(Me._yAxisMax + 5, 0)
        If yAxisMin <> Decimal.MaxValue AndAlso yAxisMax <> Decimal.MinValue Then
            Me._yAxisMin = Math.Max(yAxisMin - 1, 0)
            Me._yAxisMax = Math.Max(yAxisMax + 1, 0)
        End If

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

    ''' <summary>
    ''' ビューに展開するグラフ ラベルを表す文字列を作成します。
    ''' </summary>
    ''' <returns>
    ''' ラベル データ。
    ''' [値1, 値2, ...] の形式（値が無いときは値として null もしくは空白を指定）。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Overridable Function CreateGraphLabel() As String

        Dim values As New List(Of String)()

        For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me._itemList
            If kv.Value.Count > 0 AndAlso kv.Value.Values(0).RecordDate = Date.MinValue AndAlso kv.Value.Values(0).Value1 > 0 Then
                values.Add(String.Format("'{0:yyyy/M/d}'", kv.Key))
            Else
                values.Add("null")
            End If
        Next

        Return String.Format("[{0}]", String.Join(","c, values))

    End Function

#End Region

End Class
