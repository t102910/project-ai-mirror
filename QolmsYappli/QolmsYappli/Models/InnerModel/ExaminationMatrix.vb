Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' 健診結果表を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ExaminationMatrix

#Region "Variable"

    ''' <summary>
    ''' 列項目のリストを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _xAxis As New List(Of ExaminationAxis)()

    ''' <summary>
    ''' 行項目のリストを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _yAxis As New List(Of ExaminationAxis)()

    ''' <summary>
    ''' 結果項目の行列を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _Items As New List(Of List(Of ExaminationItem))()

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 列項目の読み取り専用コレクションを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property XAxis As ReadOnlyCollection(Of ExaminationAxis)

        Get
            Return Me._xAxis.AsReadOnly()
        End Get

    End Property

    ''' <summary>
    ''' 行項目の読み取り専用コレクションを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxis As ReadOnlyCollection(Of ExaminationAxis)

        Get
            Return Me._yAxis.AsReadOnly()
        End Get

    End Property

    ''' <summary>
    ''' 結果項目の行列の読み取り専用コレクションを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Items As ReadOnlyCollection(Of ReadOnlyCollection(Of ExaminationItem))

        Get
            Return Me._Items.ConvertAll(Function(i) i.AsReadOnly()).AsReadOnly()
        End Get

    End Property

    ''' <summary>
    ''' 列項目数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property XAxisCount As Integer

        Get
            Return Me._xAxis.Count
        End Get

    End Property

    ''' <summary>
    ''' 行項目数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property YAxisCount As Integer

        Get
            Return Me._yAxis.Count
        End Get

    End Property

    ''' <summary>
    ''' 列項目数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ColCount As Integer

        Get
            Return Me.XAxisCount
        End Get

    End Property

    ''' <summary>
    ''' 行項目数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RowCount As Integer

        Get
            Return Me.YAxisCount
        End Get

    End Property

    ''' <summary>
    ''' 基準値範囲外の結果を保持しているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property HasAbnormalValue As Boolean

        Get
            Return Me._xAxis.FindIndex(Function(i) i.HasAbnormalValue) >= 0 OrElse Me._yAxis.FindIndex(Function(i) i.HasAbnormalValue) >= 0
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExaminationMatrix" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 指定した列インデックスが有効かチェックします。
    ''' </summary>
    ''' <param name="index">列インデックス。</param>
    ''' <returns>
    ''' 有効ならTrue、
    ''' 無効ならFalse。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckColIndex(index As Integer) As Boolean

        Return index >= 0 AndAlso index < Me.ColCount

    End Function

    ''' <summary>
    ''' 指定した行インデックスが有効かチェックします。
    ''' </summary>
    ''' <param name="index">行インデックス。</param>
    ''' <returns>
    ''' 有効ならTrue、
    ''' 無効ならFalse。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckRowIndex(index As Integer) As Boolean

        Return index >= 0 AndAlso index < Me.RowCount

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 検索条件を指定して、
    ''' 列インデックスを検索します。
    ''' </summary>
    ''' <param name="match">検索条件。</param>
    ''' <returns>
    ''' 列が見つかれば0以上の列インデックス、
    ''' 列が見つからなければInteger.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function FindCol(match As Predicate(Of ExaminationAxis)) As Integer

        If match Is Nothing Then Throw New ArgumentNullException("match", "検索条件がNull参照です。")

        Dim result As Integer = Me._xAxis.FindIndex(match)

        Return If(result < 0, Integer.MinValue, result)

    End Function

    ''' <summary>
    ''' 列キーを指定して、
    ''' 列インデックスを検索します。
    ''' </summary>
    ''' <param name="key">列キー。</param>
    ''' <returns>
    ''' 列が見つかれば0以上の列インデックス、
    ''' 列が見つからなければInteger.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function FindCol(key As String) As Integer

        If String.IsNullOrWhiteSpace(key) Then Throw New ArgumentNullException("key", "列キーがNull参照もしくは空白です。")

        Dim result As Integer = Me.FindCol(Function(i) i.Key.CompareTo(key) = 0)

        Return If(result < 0, Integer.MinValue, result)

    End Function

    ''' <summary>
    ''' 検索条件を指定して、
    ''' 行インデックスを検索します。
    ''' </summary>
    ''' <param name="match">検索条件。</param>
    ''' <returns>
    ''' 行が見つかれば0以上の行インデックス、
    ''' 行が見つからなければInteger.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function FindRow(match As Predicate(Of ExaminationAxis)) As Integer

        If match Is Nothing Then Throw New ArgumentNullException("match", "検索条件がNull参照です。")

        Dim result As Integer = Me._yAxis.FindIndex(match)

        Return If(result < 0, Integer.MinValue, result)

    End Function

    ''' <summary>
    ''' 行キーを指定して、
    ''' 行インデックスを検索します。
    ''' </summary>
    ''' <param name="key">行キー。</param>
    ''' <returns>
    ''' 行が見つかれば0以上の行インデックス、
    ''' 行が見つからなければInteger.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function FindRow(key As String) As Integer

        If String.IsNullOrWhiteSpace(key) Then Throw New ArgumentNullException("key", "行キーがNull参照もしくは空白です。")
        If Me.ColCount = 0 Then Throw New ArgumentOutOfRangeException("ColCount", "列が存在しません。")

        Dim result As Integer = Me.FindRow(Function(i) i.Key.CompareTo(key) = 0)

        Return If(result < 0, Integer.MinValue, result)

    End Function

    ''' <summary>
    ''' 列項目を指定して、
    ''' 列を追加します。
    ''' </summary>
    ''' <param name="axis">列項目。</param>
    ''' <param name="index">
    ''' 列項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
    ''' 未指定の場合は末尾に追加。
    ''' </param>
    ''' <returns>
    ''' 挿入された位置を表す列インデックス。
    ''' <paramref name="key" />に該当する列がすでに存在する場合は挿入は行わず、
    ''' 該当する列インデックスを返却。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function AddCol(axis As ExaminationAxis, Optional index As Integer = Integer.MaxValue) As Integer

        If axis Is Nothing Then Throw New ArgumentNullException("axis", "列項目がNull参照です。")
        If String.IsNullOrWhiteSpace(axis.Key) Then Throw New ArgumentNullException("axis.Key", "列キーがNull参照もしくは空白です。")
        If index < 0 Then Throw New ArgumentOutOfRangeException("index", "挿入位置が不正です。")

        Dim result As Integer = Me.FindCol(axis.Key)

        If result = Integer.MinValue Then
            Dim pos As Integer = Math.Min(Me.ColCount, index)

            Me._xAxis.Insert(pos, New ExaminationAxis(axis.Key, axis.Header1, axis.Header2, axis.HeaderUnit, axis.HeaderStandardValue, Decimal.MinValue, axis.Comment, axis.AssociatedFileN, axis.ExaminationJudgementN)) ' TOSO: 実装中
            Me._Items.Insert(pos, New List(Of ExaminationItem)())

            If Me.RowCount > 0 Then
                For y As Integer = 0 To Me.RowCount - 1
                    Me._Items(pos).Add(New ExaminationItem())
                Next
            End If

            result = pos
        End If

        Return result

    End Function

    ''' <summary>
    ''' 値を指定して、
    ''' 列を追加します。
    ''' </summary>
    ''' <param name="key">列キー。</param>
    ''' <param name="header1">検査受診施設名。</param>
    ''' <param name="header2">検査受診日（yyyy/M/d）。</param>
    ''' <param name="associatedFileN">検査手帳付随 ファイル 情報の リスト。</param>
    ''' <param name="index">
    ''' 列項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
    ''' 未指定の場合は末尾に追加。
    ''' </param>
    ''' <returns>
    ''' 挿入された位置を表す列インデックス。
    ''' <paramref name="key" />に該当する列がすでに存在する場合は挿入は行わず、
    ''' 該当する列インデックスを返却。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Function AddCol(key As String, header1 As String, header2 As String, associatedFileN As List(Of AssociatedFileItem), JudgementN As Dictionary(Of String, ExaminationJudgementItem), healthAge As Decimal, comment As String, Optional index As Integer = Integer.MaxValue) As Integer

        Return Me.AddCol(New ExaminationAxis(key, header1, header2, String.Empty, String.Empty, healthAge, comment, associatedFileN, JudgementN), index)

    End Function

    ''' <summary>
    ''' 行項目を指定して、
    ''' 行を追加します。
    ''' </summary>
    ''' <param name="row">検査項目アイテム。</param>
    ''' <param name="index">
    ''' 行項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
    ''' 未指定の場合は末尾に追加。
    ''' </param>
    ''' <returns>
    ''' 挿入された位置を表す行インデックス。
    ''' <paramref name="key" />に該当する行がすでに存在する場合は挿入は行わず、
    ''' 該当する行インデックスを返却。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function AddRow(row As ExaminationItem, Optional index As Integer = Integer.MaxValue) As Integer

        Dim axis As ExaminationAxis = New ExaminationAxis(row.Code, row.Name, String.Empty, row.Unit, row.ReferenceDisplayName, Decimal.MinValue, row.Comment)
        'axis As ExaminationAxis, Optional index As Integer = Integer.MaxValue) As Integer

        If axis Is Nothing Then Throw New ArgumentNullException("axis", "行項目がNull参照です。")
        If String.IsNullOrWhiteSpace(axis.Key) Then Throw New ArgumentNullException("axis.Key", "行キーがNull参照もしくは空白です。")
        If index < 0 Then Throw New ArgumentOutOfRangeException("index", "挿入位置が不正です。")
        If Me.ColCount = 0 Then Throw New ArgumentOutOfRangeException("ColCount", "列が存在しません。")

        Dim result As Integer = Me.FindRow(axis.Key)

        If result = Integer.MinValue Then
            Dim pos As Integer = Math.Min(Me.RowCount, index)

            Me._yAxis.Insert(pos, New ExaminationAxis(axis.Key, axis.Header1, axis.Header2, axis.HeaderUnit, axis.HeaderStandardValue, Decimal.MinValue, axis.Comment))
            Me._Items.ForEach(Sub(i) i.Insert(pos, New ExaminationItem()))

            result = pos
        End If

        Return result

    End Function

    ''' <summary>
    ''' 行項目を指定して、
    ''' 行を追加します。
    ''' ＊検査種別（グループ）表示用行＊
    ''' </summary>
    ''' <param name="index">
    ''' 行項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
    ''' 未指定の場合は末尾に追加。
    ''' </param>
    ''' <returns>
    ''' 挿入された位置を表す行インデックス。
    ''' <paramref name="key" />に該当する行がすでに存在する場合は挿入は行わず、
    ''' 該当する行インデックスを返却。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function AddGroupRow(grp As ExaminationGroupItem, Optional index As Integer = Integer.MaxValue) As Integer

        If String.IsNullOrWhiteSpace(grp.GroupNo.ToString) Then Throw New ArgumentNullException("groupNo", "グループ番号がNull参照もしくは空白です。")
        'If index < 0 Then Throw New ArgumentOutOfRangeException("index", "挿入位置が不正です。")
        If Me.ColCount = 0 Then Throw New ArgumentOutOfRangeException("ColCount", "列が存在しません。")

        Dim result As Integer = Integer.MinValue

        If result = Integer.MinValue Then
            Dim pos As Integer = Math.Min(Me.RowCount, index)

            Me._yAxis.Insert(pos, New ExaminationAxis(grp.GroupNo.ToString, grp.Name, "groupRow", String.Empty, String.Empty, Decimal.MinValue, grp.Comment)) '二つ判定があれば二つ出す
            Me._Items.ForEach(Sub(i) i.Insert(pos, New ExaminationItem() With {.Value = "gr"}))

            result = pos

        End If

        Return result

    End Function

    ' ''' <summary>
    ' ''' 値を指定して、
    ' ''' 行を追加します。
    ' ''' </summary>
    ' ''' <param name="key">行キー。</param>
    ' ''' <param name="index">
    ' ''' 行項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
    ' ''' 未指定の場合は末尾に追加。
    ' ''' </param>
    ' ''' <returns>
    ' ''' 挿入された位置を表す行インデックス。
    ' ''' <paramref name="key" />に該当する行がすでに存在する場合は挿入は行わず、
    ' ''' 該当する行インデックスを返却。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    ' ''' 
    'Public Function AddRow(col As ExaminationItem, Optional index As Integer = Integer.MaxValue) As Integer

    '    'Public Function AddRow(key As String, Optional index As Integer = Integer.MaxValue) As Integer

    '    Return Me.AddRow(col, index)

    'End Function

    ''' <summary>
    ''' 列インデックスを指定して、
    ''' 列を削除します。
    ''' </summary>
    ''' <param name="index">列インデックス。</param>
    ''' <remarks></remarks>
    Public Sub RemoveCol(index As Integer)

        If Not Me.CheckColIndex(index) Then Throw New ArgumentOutOfRangeException("index", "列インデックスが不正です。")

        Me._Items.RemoveAt(index)
        Me._xAxis.RemoveAt(index)

    End Sub

    ''' <summary>
    ''' 列キーを指定して、
    ''' 列を削除します。
    ''' </summary>
    ''' <param name="key">列キー。</param>
    ''' <remarks></remarks>
    Public Sub RemoveCol(key As String)

        If String.IsNullOrWhiteSpace(key) Then Throw New ArgumentNullException("key", "列キーがNull参照もしくは空白です。")

        Dim index As Integer = Me.FindCol(key)

        If index >= 0 Then Me.RemoveCol(index)

    End Sub

    ''' <summary>
    ''' 行インデックスを指定して、
    ''' 行を削除します。
    ''' </summary>
    ''' <param name="index">行インデックス。</param>
    ''' <remarks></remarks>
    Public Sub RemoveRow(index As Integer)

        If Not Me.CheckRowIndex(index) Then Throw New ArgumentOutOfRangeException("index", "行インデックスが不正です。")

        Me._Items.ForEach(Sub(i) i.RemoveAt(index))
        Me._yAxis.RemoveAt(index)

    End Sub

    ''' <summary>
    ''' 行キーを指定して、
    ''' 行を削除します。
    ''' </summary>
    ''' <param name="key">行キー。</param>
    ''' <remarks></remarks>
    Public Sub RemoveRow(key As String)

        If String.IsNullOrWhiteSpace(key) Then Throw New ArgumentNullException("key", "行キーがNull参照もしくは空白です。")

        Dim index As Integer = Me.FindRow(key)

        If index >= 0 Then Me.RemoveRow(index)

    End Sub

    ''' <summary>
    ''' 列インデックスで指定された列を、
    ''' 新しい位置へ移動します。
    ''' </summary>
    ''' <param name="sourceIndex">移動する列を表すインデックス。</param>
    ''' <param name="destIndex">新しい位置を表すインデックス。</param>
    ''' <remarks></remarks>
    Public Sub MoveCol(sourceIndex As Integer, destIndex As Integer)

        If Not Me.CheckColIndex(sourceIndex) Then Throw New ArgumentOutOfRangeException("sourceIndex", "移動元列インデックスが不正です。")

        If Me.ColCount > 1 Then
            Dim pos As Integer = Integer.MinValue
            Dim Items As List(Of ExaminationItem) = Me._Items(sourceIndex)
            Dim axis As ExaminationAxis = Me._xAxis(sourceIndex)

            If sourceIndex > destIndex Then
                pos = Math.Max(0, destIndex)
            ElseIf sourceIndex < destIndex Then
                pos = Math.Min(destIndex, Me.ColCount - 1)
            End If

            If pos >= 0 Then
                Me._Items.Remove(Items)
                Me._Items.Insert(pos, Items)
                Me._xAxis.Remove(axis)
                Me._xAxis.Insert(pos, axis)
            End If
        End If

    End Sub

    ''' <summary>
    ''' 列キーで指定された列を、
    ''' 新しい位置へ移動します。
    ''' </summary>
    ''' <param name="sourceKey">移動する列を表すキー。</param>
    ''' <param name="destIndex">新しい位置を表すインデックス。</param>
    ''' <remarks></remarks>
    Public Sub MoveCol(sourceKey As String, destIndex As Integer)

        If String.IsNullOrWhiteSpace(sourceKey) Then Throw New ArgumentNullException("sourceKey", "移動元列キーがNull参照もしくは空白です。")

        Me.MoveCol(Me.FindCol(sourceKey), destIndex)

    End Sub

    ''' <summary>
    ''' 行インデックスで指定された行を、
    ''' 新しい位置へ移動します。
    ''' </summary>
    ''' <param name="sourceIndex">移動する行を表すインデックス。</param>
    ''' <param name="destIndex">新しい位置を表すインデックス。</param>
    ''' <remarks></remarks>
    Public Sub MoveRow(sourceIndex As Integer, destIndex As Integer)

        If Not Me.CheckRowIndex(sourceIndex) Then Throw New ArgumentOutOfRangeException("sourceIndex", "移動元行インデックスが不正です。")

        If Me.ColCount > 0 AndAlso Me.RowCount > 1 Then
            Dim pos As Integer = Integer.MinValue
            Dim axis As ExaminationAxis = Me._yAxis(sourceIndex)

            If sourceIndex > destIndex Then
                pos = Math.Max(0, destIndex)
            ElseIf sourceIndex < destIndex Then
                pos = Math.Min(destIndex, Me.RowCount - 1)
            End If

            If pos > 0 Then
                For x As Integer = 0 To Me.ColCount - 1
                    Dim Item As ExaminationItem = Me._Items(x)(sourceIndex)

                    Me._Items(x).Remove(Item)
                    Me._Items(x).Insert(pos, Item)
                Next

                Me._yAxis.Remove(axis)
                Me._yAxis.Insert(pos, axis)
            End If
        End If

    End Sub

    ''' <summary>
    ''' 行キーで指定された行を、
    ''' 新しい位置へ移動します。
    ''' </summary>
    ''' <param name="sourceKey">移動する行を表すキー。</param>
    ''' <param name="destIndex">新しい位置を表すインデックス。</param>
    ''' <remarks></remarks>
    Public Sub MoveRow(sourceKey As String, destIndex As Integer)

        If String.IsNullOrWhiteSpace(sourceKey) Then Throw New ArgumentNullException("sourceKey", "移動元行キーがNull参照もしくは空白です。")

        Me.MoveCol(Me.FindRow(sourceKey), destIndex)

    End Sub

    ''' <summary>
    ''' 列キーを使用して列をソートします。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SortColByKey()

        If Me.ColCount > 1 Then
            Dim sortedXAxis As List(Of ExaminationAxis) = Me.XAxis.ToList()

            ' 降順ソート
            sortedXAxis.Sort(
                New Comparison(Of ExaminationAxis)(
                    Function(x As ExaminationAxis, y As ExaminationAxis)
                        Dim parts() As String = x.Key.Split({"_"}, StringSplitOptions.None)
                        Dim day As Integer = 0
                        Dim seq As Integer = 0
                        Dim id As Integer = 0
                        Dim keyX As Decimal = 0
                        Dim keyY As Decimal = 0

                        If parts.Count = 3 AndAlso Integer.TryParse(parts(0), day) AndAlso Integer.TryParse(parts(1), seq) AndAlso Integer.TryParse(parts(2), id) Then
                            keyX = Decimal.Parse(String.Format("{0:d8}{1:d10}{2:d3}", day, Integer.MaxValue - seq, Integer.MaxValue - id))

                            parts = y.Key.Split({"_"}, StringSplitOptions.None)
                            day = 0
                            seq = 0
                            id = 0

                            If parts.Count = 3 AndAlso Integer.TryParse(parts(0), day) AndAlso Integer.TryParse(parts(1), seq) AndAlso Integer.TryParse(parts(2), id) Then
                                keyY = Decimal.Parse(String.Format("{0:d8}{1:d10}{2:d3}", day, Integer.MaxValue - seq, Integer.MaxValue - id))

                                Return keyX.CompareTo(keyY)
                            Else
                                Return 0
                            End If
                        Else
                            Return 0
                        End If
                    End Function
                )
            )

            For x As Integer = 0 To sortedXAxis.Count - 1
                Dim sourceIndex As Integer = Me.FindCol(sortedXAxis(x).Key)

                If sourceIndex >= 0 Then Me.MoveCol(sourceIndex, Integer.MaxValue)
            Next
        End If

    End Sub

    ''' <summary>
    ''' 行キーを使用して行をソートします。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SortRowByKey()

        If Me.ColCount > 0 AndAlso Me.RowCount > 1 Then
            Dim sortedYAxis As List(Of ExaminationAxis) = Me.YAxis.ToList()

            ' 昇順ソート
            sortedYAxis.Sort(New Comparison(Of ExaminationAxis)(Function(x As ExaminationAxis, y As ExaminationAxis) x.Key.CompareTo(y.Key)))

            For y As Integer = 0 To sortedYAxis.Count - 1
                Dim sourceIndex As Integer = Me.FindRow(sortedYAxis(y).Key)

                If sourceIndex >= 0 Then Me.MoveRow(sourceIndex, Integer.MaxValue)
            Next
        End If

    End Sub

    ''' <summary>
    ''' 表示順定義を使用して行をソートします。
    ''' </summary>
    ''' <param name="definition">表示順定義（検査項目コードをキー、表示順を値とするディクショナリ）。</param>
    ''' <remarks></remarks>
    <Obsolete("検証中、未使用")>
    Public Sub SortRowByDisplayOrder(definition As Dictionary(Of String, Integer))

        If Me.ColCount > 0 AndAlso Me.RowCount > 1 Then
            Dim sortedYAxis As List(Of ExaminationAxis) = Me.YAxis.ToList()

            ' 昇順ソート
            sortedYAxis.Sort(
                New Comparison(Of ExaminationAxis)(
                    Function(x As ExaminationAxis, y As ExaminationAxis)
                        Dim xOrder As Integer = If(definition.ContainsKey(x.Key), definition(x.Key), Integer.MaxValue)
                        Dim yOrder As Integer = If(definition.ContainsKey(y.Key), definition(y.Key), Integer.MaxValue)

                        Return xOrder.CompareTo(yOrder)
                    End Function
                )
            )

            For y As Integer = 0 To sortedYAxis.Count - 1
                Dim sourceIndex As Integer = Me.FindRow(sortedYAxis(y).Key)

                If sourceIndex >= 0 Then Me.MoveRow(sourceIndex, Integer.MaxValue)
            Next
        End If

    End Sub

    ''' <summary>
    ''' インデックスを指定して、
    ''' 検査結果項目を取得します。
    ''' </summary>
    ''' <param name="colIndex">列インデックス。</param>
    ''' <param name="rowIndex">行インデックス。</param>
    ''' <returns>
    ''' 成功なら該当する検査結果項目のコピーインスタンス、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetItem(colIndex As Integer, rowIndex As Integer) As ExaminationItem

        If Not Me.CheckColIndex(colIndex) Then Throw New ArgumentOutOfRangeException("colIndex", "列インデックスが不正です。")
        If Not Me.CheckRowIndex(rowIndex) Then Throw New ArgumentOutOfRangeException("rowIndex", "行インデックスが不正です。")

        Return Me._Items(colIndex)(rowIndex).Copy()

    End Function

    ''' <summary>
    ''' 列キーおよび行キーを指定して、
    ''' 検査結果項目を取得します。
    ''' </summary>
    ''' <param name="colKey">列キー。</param>
    ''' <param name="rowKey">行キー。</param>
    ''' <returns>
    ''' 成功なら該当する検査結果項目のコピーインスタンス、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetItem(colKey As String, rowKey As String) As ExaminationItem

        Return Me.GetItem(Me.FindCol(colKey), Me.FindRow(rowKey))

    End Function

    ''' <summary>
    ''' 一意のキーを指定して、
    ''' 検査結果項目を取得します。
    ''' </summary>
    ''' <param name="keyGuid"></param>
    ''' <returns>
    ''' 成功なら該当する検査結果項目のコピーインスタンス、
    ''' 失敗ならNothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetItem(keyGuid As Guid) As ExaminationItem

        Dim result As ExaminationItem = Nothing

        Me._Items.ForEach(
            Sub(x)
                x.ForEach(
                    Sub(y)
                        If y.KeyGuid = keyGuid Then
                            result = y.Copy()

                            Exit Sub
                        End If
                    End Sub
                )

                If result IsNot Nothing Then Exit Sub
            End Sub
        )

        Return result

    End Function

    ''' <summary>
    ''' インデックスを指定して、
    ''' 検査結果項目を設定します。
    ''' </summary>
    ''' <param name="Item">検査結果項目。</param>
    ''' <param name="colIndex">列インデックス。</param>
    ''' <param name="rowIndex">行インデックス。</param>
    ''' <remarks></remarks>
    Public Sub SetItem(Item As ExaminationItem, colIndex As Integer, rowIndex As Integer)

        If Item Is Nothing Then Throw New ArgumentNullException("Item", "検査結果項目がNull参照です。")
        If Not Me.CheckColIndex(colIndex) Then Throw New ArgumentOutOfRangeException("colIndex", "列インデックス")
        'If Not Me.CheckRowIndex(rowIndex) Then Throw New ArgumentOutOfRangeException("rowIndex", "行インデックス")

        Dim code As String = String.Empty

        If rowIndex = Integer.MinValue Then

        Else
            If ExaminationAxis.IsRowKey(Item.Code, code) AndAlso String.Compare(Me._yAxis(rowIndex).Key, code) = 0 Then Me._Items(colIndex)(rowIndex) = Item.Copy()

        End If


    End Sub

    ''' <summary>
    ''' 列キーおよび行キーを指定して、
    ''' 検査結果項目を設定します。
    ''' </summary>
    ''' <param name="Item">検査結果項目。</param>
    ''' <param name="colKey">列キー。</param>
    ''' <param name="rowKey">行キー。</param>
    ''' <remarks></remarks>
    Public Sub SetItem(Item As ExaminationItem, colKey As String, rowKey As String)

        Me.SetItem(Item, Me.FindCol(colKey), Me.FindRow(rowKey))

    End Sub

    ''' <summary>
    ''' 検査結果表を更新します。
    ''' 検査結果表に対する必要な操作を終えた後に実行してください。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateMatrix()

        ' 空の列を削除
        For x As Integer = Me.ColCount - 1 To 0 Step -1
            If Me._Items(x).FindIndex(Function(i) Not i.IsEmpty) < 0 Then Me.RemoveCol(x)
        Next

        ' 空の行を削除
        If Me.ColCount > 0 Then
            Dim hasGroupValue As Boolean = False
            For y As Integer = Me.RowCount - 1 To 0 Step -1
                Dim isGroupRow As Boolean = Me.YAxis(y).Header2 = "groupRow"

                If isGroupRow Then
                    If Not hasGroupValue Then Me.RemoveRow(y)
                    hasGroupValue = False
                Else
                    Dim hasValue As Boolean = False

                    For x As Integer = 0 To Me.ColCount - 1
                        If Not Me._Items(x)(y).IsEmpty Then
                            hasValue = True
                            hasGroupValue = True
                            Exit For
                        End If
                    Next

                    If Not hasValue Then Me.RemoveRow(y)
                End If
            Next
        End If

        ' 列項目と行項目の更新
        If Me.ColCount > 0 AndAlso Me.RowCount > 0 Then
            For y As Integer = 0 To Me.RowCount - 1
                Dim defName As String = String.Empty
                Dim defUnit As String = String.Empty
                Dim defLow As String = String.Empty
                Dim defHigh As String = String.Empty
                Dim defRange As String = String.Empty

                Dim hasDifferentUnit As Boolean = False
                'Dim dicHealthAgeValues As New Dictionary(Of String, String)()
                'Dim linkageno As Integer = 47008

                For x As Integer = 0 To Me.ColCount - 1
                    With Me._Items(x)(y)

                        'If .LocalCode Then
                        '    dicHealthAgeValues
                        'End If

                        If Not .IsEmpty Then
                            If String.IsNullOrWhiteSpace(defName) AndAlso Not String.IsNullOrWhiteSpace(.Name) Then defName = .Name

                            If .IsPhysicalQuantity Then
                                If .IsAbnormalValue Then
                                    Me._xAxis(x).HasAbnormalValue = True
                                    Me._yAxis(y).HasAbnormalValue = True
                                End If

                                If String.IsNullOrWhiteSpace(defUnit) AndAlso Not String.IsNullOrWhiteSpace(.Unit) Then defUnit = .Unit
                                If String.IsNullOrWhiteSpace(defLow) AndAlso Not String.IsNullOrWhiteSpace(.Low) Then defLow = .Low
                                If String.IsNullOrWhiteSpace(defHigh) AndAlso Not String.IsNullOrWhiteSpace(.High) Then defHigh = .High
                                If String.IsNullOrWhiteSpace(defRange) AndAlso Not String.IsNullOrWhiteSpace(.ReferenceDisplayName) Then defRange = .ReferenceDisplayName

                                .IsDifferentUnit = String.Compare(.Unit, defUnit, True) <> 0 _
                                    OrElse String.Compare(.Low, defLow, True) <> 0 _
                                    OrElse String.Compare(.High, defHigh, True) <> 0 _
                                    OrElse String.Compare(.ReferenceDisplayName, defRange, True) <> 0

                                If .IsDifferentUnit Then
                                    hasDifferentUnit = True
                                End If

                            End If
                        End If
                    End With
                Next

                If Not String.IsNullOrWhiteSpace(defName) Then Me._yAxis(y).Header1 = defName
                If Not String.IsNullOrWhiteSpace(defUnit) Then Me._yAxis(y).HeaderUnit = defUnit
                If Not String.IsNullOrWhiteSpace(defRange) Then Me._yAxis(y).HeaderStandardValue = defRange

                If hasDifferentUnit Then Me._yAxis(y).HasDifferentUnit = hasDifferentUnit

                Select Case True
                    Case Not String.IsNullOrWhiteSpace(defHigh) And Not String.IsNullOrWhiteSpace(defLow)
                        Me._yAxis(y).HeaderStandardValue = defRange

                    Case Not String.IsNullOrWhiteSpace(defHigh)
                        Me._yAxis(y).HeaderStandardValue = defHigh

                    Case Not String.IsNullOrWhiteSpace(defLow)
                        Me._yAxis(y).HeaderStandardValue = defLow

                    Case Not String.IsNullOrWhiteSpace(defRange)
                        Me._yAxis(y).HeaderStandardValue = defRange
                    Case Else

                        Me._yAxis(y).HeaderStandardValue = String.Empty

                End Select
            Next
        End If



    End Sub

    ''' <summary>
    ''' 現在のインスタンスをコピーして、
    ''' 特定の検査分類のみで構成される検査結果表を作成します。
    ''' ※ひろしまでは、健診種別（特定健診等）、医誠会では、病院名＋受診科（内科等）を
    ''' １件選択出来た。コルムスではどのように表示するか要検討。
    ''' </summary>
    ''' <param name="categoryId">検査分類ID。</param>
    ''' <returns>
    ''' 特定の検査分類のみで構成された検査結果表。
    ''' 検査結果項目が存在しない場合はNothingを返却。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function NarrowInCategory(categoryId As Integer) As ExaminationMatrix

        Dim result As ExaminationMatrix = Me.Copy()

        ' 指定された検査分類IDの列のみ残す
        For x As Integer = result.ColCount - 1 To 0 Step -1
            Dim parts() As String = result._xAxis(x).Key.Split({"_"}, StringSplitOptions.None)
            Dim id As Integer = 0

            If parts.Count() = 3 AndAlso Integer.TryParse(parts.Last(), id) AndAlso id <> categoryId Then result.RemoveCol(x)
        Next

        ' 検査結果表を更新
        If result.ColCount > 0 Then
            result.UpdateMatrix()

            If result.ColCount > 0 AndAlso result.RowCount > 0 Then Return result ' 検査結果項目が存在する
        End If

        ' 検査結果項目が存在しない
        Return New ExaminationMatrix()

    End Function

    ''' <summary>
    ''' 現在のインスタンスをコピーして、
    ''' 基準範囲外の結果のみで構成される検査結果表を作成します。
    ''' </summary>
    ''' <returns>
    ''' 基準範囲外の結果のみで構成された検査結果表。
    ''' 検査結果項目が存在しない場合はNothingを返却。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function NarrowInAbnormal() As ExaminationMatrix

        Dim result As ExaminationMatrix = Me.Copy()

        ' 基準値範囲外の値を保持している列のみ残す
        For x As Integer = result.ColCount - 1 To 0 Step -1
            If Not result._xAxis(x).HasAbnormalValue Then result.RemoveCol(x)
        Next

        ' 基準値範囲外の値を保持している行のみ残す
        If result.ColCount > 0 Then
            Dim hasAbnormal As Boolean = False
            For y As Integer = result.RowCount - 1 To 0 Step -1
                If result._yAxis(y).HasAbnormalValue Then
                    hasAbnormal = True
                    For x As Integer = 0 To result.ColCount - 1
                        ' 基準値内の値をクリア
                        If Not result._Items(x)(y).IsAbnormalValue Then
                            result._Items(x)(y).Value = String.Empty
                            result._Items(x)(y).IsDifferentUnit = False
                        End If
                    Next
                Else
                    ' 基準値範囲外の値を1つも保持していないグループは削除
                    Dim isGroupRow As Boolean = result.YAxis(y).Header2 = "groupRow"
                    If (Not isGroupRow) OrElse (isGroupRow AndAlso hasAbnormal = False) Then
                        result.RemoveRow(y)
                    End If
                    If isGroupRow Then
                        hasAbnormal = False
                    End If
                End If
            Next
        End If

        ' 検査結果表を更新
        If result.ColCount > 0 AndAlso result.RowCount > 0 Then
            result.UpdateMatrix()

            If result.ColCount > 0 AndAlso result.RowCount > 0 Then Return result ' 検査結果項目が存在する
        End If

        ' 検査結果項目が存在しない
        Return New ExaminationMatrix()

    End Function

    ''' <summary>
    ''' インスタンスのディープコピーを作成します。
    ''' </summary>
    ''' <returns>
    ''' このインスタンスをディープコピーしたオブジェクト。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Copy() As ExaminationMatrix

        Dim result As ExaminationMatrix = Nothing

        Using stream As New MemoryStream()
            With New BinaryFormatter()
                .Serialize(stream, Me)

                stream.Position = 0

                result = DirectCast(.Deserialize(stream), ExaminationMatrix)
            End With
        End Using

        Return result

    End Function

    ' ''' <summary>
    ' ''' 値を指定して、
    ' ''' 列を追加します。
    ' ''' </summary>
    ' ''' <param name="key">列キー。</param>
    ' ''' <param name="dicHelthAgeColcN">健康年齢測定に必要な検査のリスト。</param>
    ' ''' <param name="healthAgeColcFlag">健康年齢を測定できるかどうか。</param>
    ' ''' </param>
    ' ''' <returns>
    ' ''' 挿入された位置を表す列インデックス。
    ' ''' <paramref name="key" />に該当する列がすでに存在する場合は挿入は行わず、
    ' ''' 該当する列インデックスを返却。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    '<Obsolete("実装中")>
    'Public Sub AddColHealthAgeColcParamater(key As String, dicHelthAgeColcN As Dictionary(Of String, String), healthAgeColcFlag As Boolean)

    '    If _xAxis.Find(Function(i) i.Key = key) IsNot Nothing Then

    '        _xAxis.Find(Function(i) i.Key = key).HealthAgeCalcFlag = healthAgeColcFlag

    '        If healthAgeColcFlag Then
    '            _xAxis.Find(Function(i) i.Key = key).HealthAgeCalcN = dicHelthAgeColcN

    '        End If

    '    End If

    'End Sub

#End Region

End Class
