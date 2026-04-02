''' <summary>
''' バイタル値評価情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class VitalEvaluationItem

#Region "Public Property"

    ''' <summary>
    ''' 評価下限値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Lower As Decimal = Decimal.MinusOne

    ''' <summary>
    ''' 評価上限値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Upper As Decimal = Decimal.MinusOne

    ''' <summary>
    ''' 評価対象値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value As Decimal = Decimal.Zero

    ''' <summary>
    ''' 目標項目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title1 As String = String.Empty

    ''' <summary>
    ''' 結果項目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title2 As String = String.Empty

    ''' <summary>
    ''' 単位を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Unit As String = String.Empty

    ''' <summary>
    ''' 評価対象値が評価可能かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsEvaluable As Boolean

        Get
            Return Me.Lower > Decimal.Zero _
                AndAlso Me.Upper > Decimal.Zero _
                AndAlso Me.Upper >= Me.Lower _
                AndAlso Value > Decimal.Zero
        End Get

    End Property

    ''' <summary>
    ''' 評価対象値が評価下限値を下回っているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsLower As Boolean

        Get
            Return Me.IsEvaluable _
                AndAlso Me.Value < Me.Lower
        End Get

    End Property

    ''' <summary>
    ''' 評価対象値が評価上限値を上回っているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsHigher As Boolean

        Get
            Return Me.IsEvaluable _
                AndAlso Me.Value > Me.Upper
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalEvaluationItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="VitalEvaluationItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="lower">評価下限値。</param>
    ''' <param name="upper">評価上限値。</param>
    ''' <param name="value">評価対象値。</param>
    ''' <param name="title1">目標項目名（オプショナル、デフォルト = "目標"）。</param>
    ''' <param name="title2">結果項目名（オプショナル、デフォルト = "結果"）。</param>
    ''' <param name="unit">単位（オプショナル、デフォルト = ""）。</param>
    ''' <remarks></remarks>
    Public Sub New(lower As Decimal, upper As Decimal, value As Decimal, Optional title1 As String = "目標", Optional title2 As String = "結果", Optional unit As String = "")

        Me.Lower = lower
        Me.Upper = upper
        Me.Value = value
        Me.Title1 = title1
        Me.Title2 = title2
        Me.Unit = unit

    End Sub

#End Region

End Class
