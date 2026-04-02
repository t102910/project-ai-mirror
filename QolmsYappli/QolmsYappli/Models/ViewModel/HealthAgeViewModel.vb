''' <summary>
''' 「健康年齢」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class HealthAgeViewModel
    Inherits QyHealthPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ''' <summary>
    ''' 病院連携中かを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HasHospitalConnection As Boolean = False

    ''' <summary>
    ''' 健康年齢を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAge As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 実年齢との差を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeDistance As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 測定日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LatestDate As Date = Date.MinValue

    ''' <summary>
    ''' 予測医療費（100% 負担額）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicalExpenses As Integer = Integer.MinValue

    ''' <summary>
    ''' 健康年齢の推移のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeN As New List(Of HealthAgeValueItem)()

    ''' <summary>
    ''' 健康年齢改善アドバイス情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeAdviceN As New List(Of HealthAgeAdviceItem)()

    ''' <summary>
    ''' 健康年齢レポート情報の有効性のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AvailableHealthAgeReportN As New List(Of AvailableHealthAgeReportItem)()

    ' 年齢分布
    Public Property DistributionPartialViewModel As HealthAgeDistributionReportPartialViewModel = Nothing

    ' 肥満レポート
    Public Property FatPartialViewModel As HealthAgeFatReportPartialViewModel = Nothing

    ' 血糖レポート
    Public Property GlucosePartialViewModel As HealthAgeGlucoseReportPartialViewModel = Nothing

    ' 血圧レポート
    Public Property PressurePartialViewModel As HealthAgePressureReportPartialViewModel = Nothing

    ' 脂質レポート
    Public Property LipidPartialViewModel As HealthAgeLipidReportPartialViewModel = Nothing

    ' 肝臓レポート
    Public Property LiverPartialViewModel As HealthAgeLiverReportPartialViewModel = Nothing

    ' 尿糖・尿蛋白レポート
    Public Property UrinePartialViewModel As HealthAgeUrineReportPartialViewModel = Nothing

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="HealthAgeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fromPageNoType">遷移元の画面番号の種別。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, fromPageNoType As QyPageNoTypeEnum)

        MyBase.New(mainModel, QyPageNoTypeEnum.HealthAge)

        Me.FromPageNoType = fromPageNoType
        Me.HasHospitalConnection = False
        Me.HealthAge = Decimal.MinValue
        Me.HealthAgeDistance = Decimal.MinValue
        Me.MedicalExpenses = Integer.MinValue
        Me.HealthAgeAdviceN = New List(Of HealthAgeAdviceItem)()
        Me.LatestDate = Date.MinValue

        Me.AvailableHealthAgeReportN = {
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Distribution,
                .LatestDate = Date.MinValue
            },
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Fat,
                .LatestDate = Date.MinValue
            },
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Glucose,
                .LatestDate = Date.MinValue
            },
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Pressure,
                .LatestDate = Date.MinValue
            },
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Lipid,
                .LatestDate = Date.MinValue
            },
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Liver,
                .LatestDate = Date.MinValue
            },
            New AvailableHealthAgeReportItem() With {
                .HealthAgeReportType = QyHealthAgeReportTypeEnum.Urine,
                .LatestDate = Date.MinValue
            }
        }.ToList()

        ' 各レポートのパーシャル ビュー モデルの初期化（レポートは非同期で表示するため空で初期化）
        Me.DistributionPartialViewModel = New HealthAgeDistributionReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Distribution}
        )

        Me.FatPartialViewModel = New HealthAgeFatReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Fat}
        )

        Me.GlucosePartialViewModel = New HealthAgeGlucoseReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Glucose}
        )

        Me.PressurePartialViewModel = New HealthAgePressureReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Pressure}
        )

        Me.LipidPartialViewModel = New HealthAgeLipidReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Lipid}
        )

        Me.LiverPartialViewModel = New HealthAgeLiverReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Liver}
        )

        Me.UrinePartialViewModel = New HealthAgeUrineReportPartialViewModel(
            Me,
            New HealthAgeReportItem() With {.HealthAgeReportType = QyHealthAgeReportTypeEnum.Urine}
        )

    End Sub

#End Region

#Region "Public Method"

    Public Function IsAvailableHealthAgeReportType(healthAgeReportType As QyHealthAgeReportTypeEnum, ByRef hasData As Boolean) As Boolean

        hasData = False

        Dim item As AvailableHealthAgeReportItem = Me.AvailableHealthAgeReportN.Find(Function(i) i.HealthAgeReportType = healthAgeReportType)

        If item Is Nothing Then
            Return False
        Else
            hasData = item.LatestDate <> Date.MinValue

            Return True
        End If

    End Function

#End Region

End Class
