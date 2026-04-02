''' <summary>
''' 「バイタル」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteVitalViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 初期表示するタブを取得または設定します。
    ''' </summary>
    ''' <value>0:指定なし、1:体重、2:血圧、3:血糖値</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tab As Byte = Byte.MinValue

    ''' <summary>
    ''' バイタル情報の有効性のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AvailableVitalN As New List(Of AvailableVitalItem)()

    ''' <summary>
    ''' 編集日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 身長を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Height As Decimal = Decimal.MinValue

    ''' <summary>
    ''' バイタル入力インプット モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditInputModel As NoteVitalEditInputModel = Nothing

    ''' <summary>
    ''' 血圧グラフ パーシャル ビュー モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PressurePartialViewModel As VitalPressureGraphPartialViewModel = Nothing

    ''' <summary>
    ''' 血糖値グラフ パーシャル ビュー モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SugarPartialViewModel As VitalSugarGraphPartialViewModel = Nothing

    ''' <summary>
    ''' 体重グラフ パーシャル ビュー モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WeightPartialViewModel As VitalWeightGraphPartialViewModel = Nothing

    ''' <summary>
    ''' ビュー 内に展開する暗号化された タニタ 会員 QR コード 情報への参照 パラメータ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Property TanitaQrReference As String

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteVitalViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteVitalViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteVital)

        Me.AvailableVitalN = {
            New AvailableVitalItem() With {
                .VitalType = QyVitalTypeEnum.BloodPressure,
                .LatestDate = Date.MinValue
            },
            New AvailableVitalItem() With {
                .VitalType = QyVitalTypeEnum.BloodSugar,
                .LatestDate = Date.MinValue
            },
            New AvailableVitalItem() With {
                .VitalType = QyVitalTypeEnum.BodyWeight,
                .LatestDate = Date.MinValue
            }
        }.ToList()

        Me.RecordDate = Date.Now.Date
        Me.Height = mainModel.AuthorAccount.Height

        ' バイタル入力インプット モデルの初期化
        Me.EditInputModel = New NoteVitalEditInputModel(Date.Now.Date, Me.AvailableVitalN.Select(Function(i) i.VitalType).ToList(), Decimal.MinusOne)

        ' 各グラフのパーシャル ビュー モデルの初期化（グラフは非同期で表示するため空で初期化）
        Me.PressurePartialViewModel = New VitalPressureGraphPartialViewModel(
            Me,
            Nothing,
            Decimal.Zero,
            Decimal.Zero,
            Decimal.Zero,
            Decimal.Zero
        )

        Me.SugarPartialViewModel = New VitalSugarGraphPartialViewModel(
            Me,
            Nothing,
            Decimal.Zero,
            Decimal.Zero,
            Decimal.Zero,
            Decimal.Zero
        )

        Me.WeightPartialViewModel = New VitalWeightGraphPartialViewModel(
            Me,
            Nothing,
            mainModel.AuthorAccount.Height,
            Decimal.Zero,
            Decimal.Zero,
            Decimal.Zero,
            Decimal.Zero,
            Nothing
        )

        ' TODO:
        Me.TanitaQrReference = String.Empty

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 指定したバイタル情報の種別が有効かを判定します。
    ''' </summary>
    ''' <param name="vitalType">判定するバイタル情報の種別。</param>
    ''' <param name="hasData"></param>
    ''' <returns>
    ''' 有効なら True、
    ''' 無効なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function IsAvailableVitalType(vitalType As QyVitalTypeEnum, ByRef hasData As Boolean) As Boolean

        hasData = False

        Dim item As AvailableVitalItem = Me.AvailableVitalN.Find(Function(i) i.VitalType = vitalType)

        If item Is Nothing Then
            Return False
        Else
            hasData = item.LatestDate <> Date.MinValue

            Return True
        End If

    End Function

#End Region

End Class
