Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text.RegularExpressions

''' <summary>
''' 検査結果表の列項目もしくは行項目を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ExaminationAxis

#Region "Constant"

    ''' <summary>
    ''' 検査項目コードを表す正規表現パターンです。
    ''' 医誠会ローカルコード版
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly regexLocalCode As New Regex("^[0-9]{6}$", RegexOptions.IgnoreCase)

    Private Shared ReadOnly regexCode As New Regex("^[0-9A-Z]{17}$", RegexOptions.IgnoreCase)
#End Region

#Region "Variable"

    ''' <summary>
    ''' 列または行を示すキー値を保持します。
    ''' 列の場合は「検査実施年月日（YYYYMMDD）_日付内連番」、行の場合は「検査項目コード」になります。
    ''' </summary>
    ''' <remarks></remarks>
    Private _key As String = String.Empty

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 列または行を示すキー値を取得します。
    ''' 列の場合は「検査実施年月日（YYYYMMDD）_日付内連番」、行の場合は「検査項目コード」になります。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Key As String

        Get
            Return Me._key
        End Get

    End Property

    ''' <summary>
    ''' ヘッダに表示する1つ目の値を取得または設定します。
    ''' 列の場合は「検査実施機関名」、
    ''' 行の場合は 種別（グループ）列：「種別名」、項目列：「検査項目名」になります。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Header1 As String = String.Empty

    ''' <summary>
    ''' ヘッダに表示する2つ目の値を取得または設定します。
    ''' 列の場合は「検査実施日」になります。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Header2 As String = String.Empty

    ''' <summary>
    ''' ヘッダに表示する単位を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HeaderUnit As String = String.Empty

    ''' <summary>
    ''' 標準の単位以外を保持しているかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HasDifferentUnit As Boolean = False

    ''' <summary>
    ''' ヘッダに表示する基準値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HeaderStandardValue As String = String.Empty

    ''' <summary>
    ''' 基準値範囲外の結果を保持しているかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HasAbnormalValue As Boolean = False

    ''' <summary>
    ''' 検査手帳付随 ファイル 情報の リスト を取得または設定します。
    ''' 列の場合のみ有効です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Property AssociatedFileN As New List(Of AssociatedFileItem)()

    ''' <summary>
    ''' 検査所見・判定 の リスト を取得または設定します。
    ''' 列の場合のみ有効です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExaminationJudgementN As New Dictionary(Of String, ExaminationJudgementItem)()

    ''' <summary>
    ''' 健康年齢を取得または設定します。
    ''' 列の場合のみ有効です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAge As Decimal = Decimal.MinValue

    ''' <summary>
    ''' コメントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Comment As String = String.Empty

    ' ''' <summary>
    ' ''' 健康年齢の測定対象かどうかを取得または設定します。
    ' ''' 列の場合のみ有効です。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property HealthAgeCalcFlag As Boolean = False

    ' ''' <summary>
    ' ''' 健康年齢の測定対象の場合に測定に必要な数値を取得または設定します。
    ' ''' 列の場合のみ有効です。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property HealthAgeCalcN As New Dictionary(Of String, String)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExaminationAxis" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="ExaminationAxis" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="key">
    ''' 列または行を示すキー値。
    ''' 列の場合は「検査実施年月日（YYYYMMDD）_日付内連番」、行の場合は「検査項目コード」になります。
    ''' </param>
    ''' <param name="header1">
    ''' ヘッダに表示する1つ目の値。
    ''' 列の場合は「検査実施機関名」、行の場合は「検査項目名」になります。
    ''' </param>
    ''' <param name="header2">
    ''' ヘッダに表示する2つ目の値。
    ''' 列の場合は「検査実施年月日（yyyy/M/d）」になります。
    ''' </param>
    ''' <param name="headerUnit">ツールチップで表示する単位。</param>
    ''' <param name="headerSV">ツールチップで表示する基準値。</param>
    ''' <param name="associatedFileN">
    ''' 検査手帳付随 ファイル 情報の リスト を取得または設定します（オプショナル、デフォルト = Nothing）。
    ''' 列の場合のみ指定してください。
    ''' </param>
    ''' <param name="examinationJudgementN">
    ''' 検査所見・判定 の リスト を取得または設定します（オプショナル、デフォルト = Nothing）。
    ''' 列の場合のみ指定してください。    ''' 
    ''' </param>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Sub New(key As String, header1 As String, header2 As String, headerUnit As String, headerSV As String, healthAge As Decimal, comment As String, Optional associatedFileN As List(Of AssociatedFileItem) = Nothing, Optional examinationJudgementN As Dictionary(Of String, ExaminationJudgementItem) = Nothing)

        Me._key = key
        Me.Header1 = header1
        Me.Header2 = header2
        Me.HeaderUnit = headerUnit
        Me.HeaderStandardValue = headerSV
        Me.HealthAge = healthAge
        Me.Comment = comment

        If associatedFileN IsNot Nothing AndAlso associatedFileN.Any() Then
            Me.AssociatedFileN = associatedFileN
        Else
            Me.AssociatedFileN = New List(Of AssociatedFileItem)()
        End If

        If examinationJudgementN IsNot Nothing AndAlso examinationJudgementN.Any() Then
            Me.ExaminationJudgementN = examinationJudgementN
        Else
            Me.ExaminationJudgementN = New Dictionary(Of String, ExaminationJudgementItem)()
        End If

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 列キーとして使用可能な文字列かチェックします。
    ''' </summary>
    ''' <param name="fileName">健診CDAファイル名。</param>
    ''' <param name="key">列キーとして使用可能な文字列が格納される変数。</param>
    ''' <returns>
    ''' 使用可能ならTrue、
    ''' 使用不可能ならFalse。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function IsColKey(fileName As String, ByRef key As String) As Boolean

        'Dim result As Boolean = False

        key = String.Empty

        key = fileName

        'If Not String.IsNullOrWhiteSpace(fileName) Then
        '    Dim parts() As String = IO.Path.GetFileNameWithoutExtension(fileName).Split({"_"}, StringSplitOptions.None)
        '    Dim day As Integer = 0
        '    Dim seq As Integer = 0
        '    Dim id As Byte = 0

        '    If parts.Count() = 10 AndAlso Integer.TryParse(parts(1), day) AndAlso day >= 10101 AndAlso day <= 99991231 AndAlso Integer.TryParse(parts(8), seq) AndAlso Byte.TryParse(parts(9), id) Then
        '        key = String.Format("{0:d8}_{1}_{2}", day, seq, id)
        '        result = True
        '    End If
        'End If

        Return True

    End Function

    ''' <summary>
    ''' 行キーとして使用可能な文字列かチェックします。
    ''' </summary>
    ''' <param name="code">検査項目コード。</param>
    ''' <param name="key">行キーとして使用可能な文字列が格納される変数。</param>
    ''' <returns>
    ''' 使用可能ならTrue、
    ''' 使用不可能ならFalse。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function IsRowKey(code As String, ByRef key As String) As Boolean

        Dim result As Boolean = False

        key = String.Empty

        If Not String.IsNullOrWhiteSpace(code) AndAlso (ExaminationAxis.regexCode.IsMatch(code) OrElse ExaminationAxis.regexLocalCode.IsMatch(code)) Then
            key = code
            result = True
        End If

        Return result

    End Function

    ''' <summary>
    ''' インスタンスのディープコピーを作成します。
    ''' </summary>
    ''' <returns>
    ''' このインスタンスをディープコピーしたオブジェクト。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Copy() As ExaminationAxis

        Dim result As ExaminationAxis = Nothing

        Using stream As New MemoryStream()
            With New BinaryFormatter()
                .Serialize(stream, Me)

                stream.Position = 0

                result = DirectCast(.Deserialize(stream), ExaminationAxis)
            End With
        End Using

        Return result

    End Function

#End Region

End Class