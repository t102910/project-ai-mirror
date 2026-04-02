Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

<Serializable()>
Public NotInheritable Class NoteMeal2InputModel
    Inherits QyNotePageViewModelBase
    Implements IQyModelUpdater(Of NoteMeal2InputModel)

#Region "Variable"

#End Region

#Region "Public Property"
    
    ''' <summary>
    ''' 記録日を取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 食事種別を取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property MealType As Byte = Byte.MinValue

    '''' <summary>
    '''' イベント開始日時を取得または設定します。
    '''' </summary>
    '''' <returns></returns>
    'Public Property StartDate As Date = Date.MinValue

    '''' <summary>
    '''' イベント終了日時を取得または設定します。
    'Public Property ENDDATE As Date = Date.MinValue

    ''' <summary>
    ''' 品目名を取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property ItemName As String = String.Empty
   
    ''' <summary>
    ''' カロリーを取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property Calorie As Short = Short.MinValue

    '''' <summary>
    '''' 食事画像 キーを取得または設定します。
    '''' </summary>
    '''' <returns></returns>
    'Public Property Photokey As Guid = Guid.Empty

    ''' <summary>
    ''' 食事解析種別を取得または設定します。
    ''' カロミルから取得の場合は AnalysisType = 5
    ''' </summary>
    ''' <returns></returns>
    Public Property AnalysisType As Byte = Byte.MinValue

    ''' <summary>
    ''' 食事解析結果情報を取得または設定します。
    ''' カロミルから取得の場合は 取得したJson文字列をそのまま入れます。
    ''' </summary>
    ''' <returns></returns>
    Public Property AnalysisSet As String = String.Empty

    '''' 食事解析結果として採用した座標情報を取得または設定します。
    'Public Property CHOOSEPOSITIONSET As String = String.Empty

    ''' <summary>
    ''' 食事解析結果として採用した情報を取得または設定します。
    ''' カロミルから取得の場合は 取得したJson文字列をそのまま入れます。
    ''' </summary>
    ''' <returns></returns>
    Public Property ChooseSet As String = String.Empty

    ' 
    ''' <summary>
    ''' 食事割合を取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property Rate As Decimal = Decimal.MinValue
    
    ''' <summary>
    ''' 削除フラグを取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property DeleteFlag As Boolean = False

    ''' <summary>
    ''' カロミルの履歴番号を取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property HistoryId As Integer = Integer.MinValue
    
    ''' <summary>
    ''' 画像取得フラグを取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    Public Property HasImage As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMeal2InputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Private Method"

    '''' <summary>
    '''' 入力検証エラー メッセージを作成します。
    '''' </summary>
    '''' <param name="format">複合書式指定文字列。</param>
    '''' <param name="propertyName">プロパティ名。</param>
    '''' <param name="displayName">表示名。</param>
    '''' <param name="arg1">書式指定する第 1 オブジェクト。</param>
    '''' <param name="arg2">書式指定する第 2 オブジェクト（オプショナル、デフォルト = Nothing）。</param>
    '''' <returns>
    '''' 入力検証エラー メッセージ。
    '''' </returns>
    '''' <remarks></remarks>
    'Private Function CreateErrorMessage(format As String, propertyName As String, displayName As String, Optional arg1 As Object = Nothing, Optional arg2 As Object = Nothing) As String

    '    Return String.Format(format, If(String.IsNullOrWhiteSpace(displayName), propertyName, displayName), arg1, arg2)

    'End Function

    '''' <summary>
    '''' 10 進数の小数部桁数を取得します。
    '''' </summary>
    '''' <param name="value">取得対象の 10 進数。</param>
    '''' <returns>
    '''' 小数部の桁数。
    '''' </returns>
    '''' <remarks></remarks>
    'Public Function GetDecimalPartScale(value As Decimal) As Integer

    '    Return Decimal.GetBits(value)(3) >> 16 And &HFF

    'End Function

    '''' <summary>
    '''' 入力された 10 進数を検証します。
    '''' </summary>
    '''' <param name="value">入力値。</param>
    '''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 9999D）。</param>
    '''' <returns>
    '''' 検証に成功なら Decimal.MinValue 以外、
    '''' 失敗なら Decimal.MinValue。
    '''' </returns>
    '''' <remarks></remarks>
    'Private Function CheckDecimalValue(value As String, Optional upperLimit As Decimal = NoteMeal2InputModel.MAX_VALUE) As Decimal

    '    Dim result As Decimal = Decimal.MinValue

    '    If Not String.IsNullOrWhiteSpace(value) Then
    '        Dim decimalValue As Decimal = Decimal.MinValue

    '        If Decimal.TryParse(value, decimalValue) _
    '            AndAlso decimalValue >= NoteMeal2InputModel.MIN_VALUE _
    '            AndAlso decimalValue <= upperLimit Then

    '            result = decimalValue
    '        End If
    '    End If

    '    Return result

    'End Function

    '''' <summary>
    '''' 入力された 10 進数の小数部桁数を検証します。
    '''' </summary>
    '''' <param name="value">入力値。</param>
    '''' <param name="scaleLimit">許容する小数部桁数。</param>
    '''' <returns>
    '''' 検証に成功なら True、
    '''' 失敗なら False。
    '''' </returns>
    '''' <remarks></remarks>
    'Private Function CheckDecimalPartScale(value As Decimal, scaleLimit As Integer) As Boolean

    '    Return Me.GetDecimalPartScale(value) <= scaleLimit

    'End Function

    '''' <summary>
    '''' カロリーを検証します。
    '''' </summary>
    '''' <returns>
    '''' 検証に成功なら空のリスト、
    '''' 失敗ならエラー メッセージのリスト。
    '''' </returns>
    '''' <remarks></remarks>
    'Private Function CheckCalorieValue() As List(Of String)

    '    Dim result As New List(Of String)()
    '    Dim decimalValue As Decimal = Me.CheckDecimalValue(Me.Calorie)

    '    If decimalValue = Decimal.MinValue Then
    '        ' 範囲エラー
    '        result.Add(
    '            Me.CreateErrorMessage(
    '                NoteMeal2InputModel.RANGE_ERROR_MESSAGE,
    '                String.Empty,
    '                "カロリー",
    '                NoteMeal2InputModel.MIN_VALUE,
    '                NoteMeal2InputModel.MAX_VALUE
    '            )
    '        )
    '    ElseIf Not Me.CheckDecimalPartScale(decimalValue, 0) Then
    '        ' 小数部桁数エラー
    '        result.Add(
    '            Me.CreateErrorMessage(
    '                NoteMeal2InputModel.DECIMAL_PART_ERROR_MESSAGE,
    '                String.Empty,
    '                "カロリー"
    '            )
    '        )
    '    End If

    '    Return result

    'End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' インプットモデルの内容を現在のインスタンスに反映します。
    ''' </summary>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(inputModel As NoteMeal2InputModel) Implements IQyModelUpdater(Of NoteMeal2InputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.AnalysisSet = inputModel.AnalysisSet
                Me.AnalysisType = inputModel.AnalysisType
                Me.Calorie = inputModel.Calorie
                Me.ChooseSet = inputModel.ChooseSet
                Me.DeleteFlag = inputModel.DeleteFlag
                Me.HistoryId = inputModel.HistoryId
                Me.ItemName = inputModel.ItemName
                Me.MealType = inputModel.MealType
                Me.Rate = inputModel.Rate
                Me.RecordDate= inputModel.RecordDate

            End With
        End If

    End Sub

#End Region

'#Region "IValidatableObject Support"

'    ''' <summary>
'    ''' 指定されたオブジェクトが有効かどうかを判断します。
'    ''' </summary>
'    ''' <param name="validationContext">検証コンテキスト。</param>
'    ''' <returns>
'    ''' 失敗した検証の情報を保持するコレクション。
'    ''' </returns>
'    ''' <remarks></remarks>
'    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

'        Dim result As New List(Of ValidationResult)()

'        '食事日を判定
'        If Me.RecordDate = Date.MinValue Then
'            result.Add(New ValidationResult("食事日が不正です。", {"RecordDate"}))
'        ElseIf Me.RecordDate.Date > Date.Now.Date Then
'            result.Add(New ValidationResult("食事日が未来です。", {"RecordDate"}))
'        End If

'        '食事種別を判定
'        If Me.MealType = QyMealTypeEnum.None Then
'            result.Add(New ValidationResult("食事種別が不正です。", {"MealType"}))
'        End If

'        ''カロリーを判定
'        'If Me.Calorie - System.Math.Floor(Me.Calorie) <> 0 OrElse Me.Calorie < 0 OrElse Me.Calorie > 9999 Then
'        '    result.Add(New ValidationResult("カロリーは0～9999の整数で入力して下さい。", {"Calorie"}))
'        'End If

'        ' カロリーを判定
'        If String.IsNullOrWhiteSpace(Me.Calorie) Then
'            ' 必須入力エラー
'            result.Add(
'                New ValidationResult(
'                    Me.CreateErrorMessage(
'                        NoteMeal2InputModel.REQUIRED_ERROR_MESSAGE,
'                        String.Empty,
'                        "カロリー"
'                    ),
'                    {"Calorie"}
'                )
'            )
'        Else
'            Dim messageN As List(Of String) = Me.CheckCalorieValue()

'            If messageN.Any() Then
'                ' 変換エラー
'                result.Add(
'                    New ValidationResult(
'                        String.Join(Environment.NewLine, messageN),
'                        {"Calorie"}
'                    )
'                )
'            End If
'        End If

'        Return result

'    End Function

'#End Region

End Class