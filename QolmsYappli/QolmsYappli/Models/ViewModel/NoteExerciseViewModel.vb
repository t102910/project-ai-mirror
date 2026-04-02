''' <summary>
''' 「運動」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteExerciseViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 編集日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 運動アイテムのリスト（ユーザー登録）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseItemN As New List(Of ExerciseItem)

    ''' <summary>
    ''' 運動アイテムのリスト（スタンプ用）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseStampN As New List(Of ExerciseItem)

    ''' <summary>
    ''' 運動アイテムのリスト（文字リスト用）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseStringN As New List(Of ExerciseItem)

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteExerciseViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteExerciseViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteExercise)

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteExerciseViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, recordDate As Date, exerciseItemN As List(Of ExerciseItem), exerciseStampN As List(Of ExerciseItem), exerciseStringN As List(Of ExerciseItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteExercise)

        Me.RecordDate = recordDate

        Me.ExerciseItemN = exerciseItemN

        Me.ExerciseStampN = exerciseStampN

        Me.ExerciseStringN = exerciseStringN

    End Sub

#End Region

End Class
