Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

<Serializable()>
Public NotInheritable Class NoteExerciseInputModel
    Inherits QyNotePageViewModelBase
    Implements IQyModelUpdater(Of NoteExerciseInputModel), 
        IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 入力値が必須であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERROR_MESSAGE As String = "{0}を入力してください。"

    ''' <summary>
    ''' 入力値の範囲が不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const RANGE_ERROR_MESSAGE As String = "{0}は{1}～{2}の範囲で入力してください。"

    ''' <summary>
    ''' 入力値に小数部が含まれていることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_ERROR_MESSAGE As String = "{0}は整数で入力して下さい。"

    ''' <summary>
    ''' 入力可能最小値を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MIN_VALUE As Decimal = 1D

    ''' <summary>
    ''' 入力可能最大値を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAX_VALUE As Decimal = 9999D

    ''' <summary>
    ''' 品目名の入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_100 As Integer = 100

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 運動日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("運動日")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' AM / PM を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Meridiem As String = String.Empty

    ''' <summary>
    ''' 時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Hour As String = String.Empty

    ''' <summary>
    ''' 分を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Minute As String = String.Empty

    ''' <summary>
    ''' 運動種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("運動種別")>
    Public Property ExerciseType As Short = Short.MinValue

    ''' <summary>
    ''' 開始日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartDate As Date = Date.MinValue

    ''' <summary>
    ''' 終了日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EndDate As Date = Date.MinValue

    ''' <summary>
    ''' 運動名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("運動名")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(NoteExerciseInputModel.LENGTH_100, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property ItemName As String = String.Empty

    ' ''' <summary>
    ' ''' カロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("カロリー")>
    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    'Public Property Calorie As Short = Short.MinValue

    ''' <summary>
    ''' カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("カロリー")>
    Public Property Calorie As String = String.Empty

    ''' <summary>
    ''' 外部ファイルキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ForeignKey As String = String.Empty

    ''' <summary>
    ''' 更新日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UpdatedDate As Date = Date.MinValue

    ''' <summary>
    ''' 日付け内連番を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sequence As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteExerciseInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    'Public Sub New(
    '    mainModel As QolmsYappliModel,
    '    recorddate As Date,
    '    exerciseType As Byte,
    '    startDate As Date,
    '    endDate As Date,
    '    itemName As String,
    '    calorie As Short,
    '    foreignKey As String,
    '    sequence As Integer
    ')
    '    MyBase.New(mainModel, QyPageNoTypeEnum.NoteExercise)

    '    Me.RecordDate = recorddate
    '    Me.exerciseType = exerciseType
    '    Me.StartDate = startDate
    '    Me.EndDate = endDate
    '    Me.ItemName = itemName
    '    Me.Calorie = calorie
    '    Me.ForeignKey = foreignKey
    '    Me.Sequence = sequence

    '    Me.UpdatedDate = UpdatedDate

    'End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 入力検証エラー メッセージを作成します。
    ''' </summary>
    ''' <param name="format">複合書式指定文字列。</param>
    ''' <param name="propertyName">プロパティ名。</param>
    ''' <param name="displayName">表示名。</param>
    ''' <param name="arg1">書式指定する第 1 オブジェクト。</param>
    ''' <param name="arg2">書式指定する第 2 オブジェクト（オプショナル、デフォルト = Nothing）。</param>
    ''' <returns>
    ''' 入力検証エラー メッセージ。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateErrorMessage(format As String, propertyName As String, displayName As String, Optional arg1 As Object = Nothing, Optional arg2 As Object = Nothing) As String

        Return String.Format(format, If(String.IsNullOrWhiteSpace(displayName), propertyName, displayName), arg1, arg2)

    End Function

    ''' <summary>
    ''' 10 進数の小数部桁数を取得します。
    ''' </summary>
    ''' <param name="value">取得対象の 10 進数。</param>
    ''' <returns>
    ''' 小数部の桁数。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetDecimalPartScale(value As Decimal) As Integer

        Return Decimal.GetBits(value)(3) >> 16 And &HFF

    End Function

    ''' <summary>
    ''' 入力された 10 進数を検証します。
    ''' </summary>
    ''' <param name="value">入力値。</param>
    ''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 9999D）。</param>
    ''' <returns>
    ''' 検証に成功なら Decimal.MinValue 以外、
    ''' 失敗なら Decimal.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalValue(value As String, Optional upperLimit As Decimal = NoteExerciseInputModel.MAX_VALUE) As Decimal

        Dim result As Decimal = Decimal.MinValue

        If Not String.IsNullOrWhiteSpace(value) Then
            Dim decimalValue As Decimal = Decimal.MinValue

            If Decimal.TryParse(value, decimalValue) _
                AndAlso decimalValue >= NoteExerciseInputModel.MIN_VALUE _
                AndAlso decimalValue <= upperLimit Then

                result = decimalValue
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 入力された 10 進数の小数部桁数を検証します。
    ''' </summary>
    ''' <param name="value">入力値。</param>
    ''' <param name="scaleLimit">許容する小数部桁数。</param>
    ''' <returns>
    ''' 検証に成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalPartScale(value As Decimal, scaleLimit As Integer) As Boolean

        Return Me.GetDecimalPartScale(value) <= scaleLimit

    End Function

    ''' <summary>
    ''' カロリーを検証します。
    ''' </summary>
    ''' <returns>
    ''' 検証に成功なら空のリスト、
    ''' 失敗ならエラー メッセージのリスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckCalorieValue() As List(Of String)

        Dim result As New List(Of String)()
        Dim decimalValue As Decimal = Me.CheckDecimalValue(Me.Calorie)

        If decimalValue = Decimal.MinValue Then
            ' 範囲エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteExerciseInputModel.RANGE_ERROR_MESSAGE,
                    String.Empty,
                    "カロリー",
                    NoteExerciseInputModel.MIN_VALUE,
                    NoteExerciseInputModel.MAX_VALUE
                )
            )
        ElseIf Not Me.CheckDecimalPartScale(decimalValue, 0) Then
            ' 小数部桁数エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteExerciseInputModel.DECIMAL_PART_ERROR_MESSAGE,
                    String.Empty,
                    "カロリー"
                )
            )
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' インプットモデルの内容を現在のインスタンスに反映します。
    ''' </summary>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(inputModel As NoteExerciseInputModel) Implements IQyModelUpdater(Of NoteExerciseInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.RecordDate = .RecordDate
                Me.ExerciseType = .ExerciseType
                Me.Sequence = .Sequence

                Me.StartDate = .StartDate
                Me.EndDate = .EndDate

                Me.Hour = .Hour
                Me.Minute = .Minute
                Me.Meridiem = .Meridiem
                Me.ItemName = .ItemName
                Me.Calorie = .Calorie
                Me.ForeignKey = .ForeignKey
            End With
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    ''' <summary>
    ''' 指定されたオブジェクトが有効かどうかを判断します。
    ''' </summary>
    ''' <param name="validationContext">検証コンテキスト。</param>
    ''' <returns>
    ''' 失敗した検証の情報を保持するコレクション。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()

        '運動日を判定
        If Me.RecordDate = Date.MinValue Then
            result.Add(New ValidationResult("運動日が不正です。", {"RecordDate"}))
        ElseIf Me.RecordDate.Date > Date.Now.Date Then
            result.Add(New ValidationResult("運動日が未来です。", {"RecordDate"}))
        End If

        '運動種別を判定
        If Me.ExerciseType = Byte.MinValue Then
            result.Add(New ValidationResult("運動種別が不正です。", {"ExerciseType"}))
        End If

        ''カロリーを判定
        'If Me.Calorie < 0 OrElse Me.Calorie > 9999 Then
        '    result.Add(New ValidationResult("カロリーは0～9999の値で入力して下さい。", {"Calorie"}))
        'End If

        ' カロリーを判定
        If String.IsNullOrWhiteSpace(Me.Calorie) Then
            ' 必須入力エラー
            result.Add(
                New ValidationResult(
                    Me.CreateErrorMessage(
                        NoteExerciseInputModel.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        "カロリー"
                    ),
                    {"Calorie"}
                )
            )
        Else
            Dim messageN As List(Of String) = Me.CheckCalorieValue()

            If messageN.Any() Then
                ' 変換エラー
                result.Add(
                    New ValidationResult(
                        String.Join(Environment.NewLine, messageN),
                        {"Calorie"}
                    )
                )
            End If
        End If

        Return result

    End Function

#End Region

End Class