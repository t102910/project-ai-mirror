''' <summary>
''' 「健診結果」画面ビューモデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteExaminationViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 表示開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartDate As Date = Date.MinValue

    ''' <summary>
    ''' 表示終了日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EndDate As Date = Date.MinValue

    ''' <summary>
    ''' 検査結果表のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MatrixN As New List(Of ExaminationMatrix)

    ''' <summary>
    ''' 特定の健診分類のみへ絞り込むための健診分類IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NarrowInCategory As Byte = Byte.MinValue

    ''' <summary>
    ''' 基準範囲外の結果のみへ絞り込むかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NarrowInAbnormal As Boolean = False

    ''' <summary>
    ''' 検査グループ情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExaminationGroupN As New List(Of ExaminationGroupItem)()

    ''' <summary>
    ''' 日付および日付内連番ごとの検査結果情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExaminationSetN As New List(Of ExaminationSetItem)()

    ''' <summary>
    ''' 健康年齢計算用パラメータのJson形式の文字列を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeCalcJson As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteExaminationViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メインモデルを指定して、
    ''' <see cref="NoteExaminationViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteExamination)

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteExaminationViewModel" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">ログイン済みモデル。</param>
    ''' <param name="startDate">表示開始日。</param>
    ''' <param name="endDate">表示終了日。</param>
    ''' <param name="matrix">検査結果表のリスト。</param>
    ''' <param name="narrowInAbnormal">基準範囲外の結果のみへ絞り込むかのフラグ。</param>
    ''' <param name="examinationN">検査結果のリスト。</param>
    ''' <param name="groupN">検査種別のリスト。</param>
    ''' <remarks></remarks>
    Public Sub New(
        mainModel As QolmsYappliModel,
        startDate As Date,
        endDate As Date,
        matrixN As List(Of ExaminationMatrix),
        narrowInAbnormal As Boolean,
        examinationN As List(Of ExaminationSetItem),
        groupN As List(Of ExaminationGroupItem),
        healthAgeCalcJson As String
    )

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteExamination)

        Me.StartDate = startDate
        Me.EndDate = endDate
        Me.ExaminationSetN = examinationN
        Me.ExaminationGroupN = groupN

        Me.HealthAgeCalcJson = healthAgeCalcJson
        '要検討
        'categoryTreeN As IEnumerable(Of CheckupTreeNode),
        '        selectedN As IEnumerable(Of CheckupReferenceJsonParameter),
        'narrowInCategory As Byte,

        Me.InitializeBy(
        matrixN,
        NarrowInCategory,
        narrowInAbnormal
        )
        'categoryTreeN,
        '          selectedN,
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteExaminationViewModel" />クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="matrixN">検査結果表のリスト。</param>
    ''' <param name="narrowInAbnormal">基準範囲外の結果のみへ絞り込むかのフラグ。</param>
    ''' <remarks></remarks>
    Private Sub InitializeBy(
        matrixN As List(Of ExaminationMatrix),
        narrowInCategory As Byte,
        narrowInAbnormal As Boolean
    )

        'categoryTreeN As IEnumerable(Of CheckupTreeNode),
        'selectedN As IEnumerable(Of CheckupReferenceJsonParameter),

        'If categoryTreeN IsNot Nothing AndAlso categoryTreeN.Any() Then Me.CategoryTreeN = categoryTreeN.ToList()

        'If selectedN IsNot Nothing AndAlso selectedN.Any() Then
        '    Me.SelectedN = selectedN.ToList()
        Dim newMatrixN As New List(Of ExaminationMatrix)
        If matrixN.Any Then
            For Each mat As ExaminationMatrix In matrixN
                If mat.ColCount > 0 AndAlso mat.RowCount > 0 Then
                    '' 特定の健診分類IDのみへ絞り込む
                    'If Me.NarrowInCategory <> Byte.MinValue Then mat = mat.NarrowInCategory(Me.NarrowInCategory)

                    ' 基準範囲外の結果のみへ絞り込む
                    If Me.NarrowInAbnormal Then mat = mat.NarrowInAbnormal()
                Else
                    Me.NarrowInAbnormal = False
                    Me.NarrowInCategory = Byte.MinValue
                End If

                newMatrixN.Add(mat)

            Next

        End If

        Me.MatrixN = newMatrixN
        Me.NarrowInCategory = narrowInCategory
        Me.NarrowInAbnormal = narrowInAbnormal

    End Sub
#End Region

#Region "Public Method"

#End Region

End Class