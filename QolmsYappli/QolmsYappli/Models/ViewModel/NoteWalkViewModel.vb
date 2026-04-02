''' <summary>
''' 「歩く」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteWalkViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

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
    ''' バイタル入力インプット モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditInputModel As NoteVitalEditInputModel = Nothing

    ''' <summary>
    ''' 歩数グラフ パーシャル ビュー モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StepsPartialViewModel As VitalStepsGraphPartialViewModel = Nothing

    ''' <summary>
    ''' タニタの歩数連携があるかを 取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TanitaWalkConnected As Boolean = False
#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteWalkViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteWalkViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteWalk)

        Me.AvailableVitalN = {
            New AvailableVitalItem() With {
                .VitalType = QyVitalTypeEnum.Steps,
                .LatestDate = Date.MinValue
            }
        }.ToList()

        Me.RecordDate = Date.Now.Date

        ' バイタル入力インプット モデルの初期化
        Me.EditInputModel = New NoteVitalEditInputModel(Me.RecordDate, Me.AvailableVitalN.Select(Function(i) i.VitalType).ToList(), Decimal.MinusOne)

        ' 各グラフのパーシャル ビュー モデルの初期化（グラフは非同期で表示するため空で初期化）
        Me.StepsPartialViewModel = New VitalStepsGraphPartialViewModel(Me, Nothing, Decimal.Zero, Decimal.Zero)

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
