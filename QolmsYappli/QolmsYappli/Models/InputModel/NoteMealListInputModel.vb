Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

<Serializable()>
Public NotInheritable Class NoteMealListInputModel
    Inherits QyNotePageViewModelBase
    Implements IQyModelUpdater(Of NoteMealListInputModel), 
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
    Private Const MIN_VALUE As Decimal = 0D

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

    ''' <summary>
    ''' 添付ファイルの最大登録可能数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAX_ATTACHED_COUNT As Integer = 1

#End Region

#Region "Variable"

    ''' <summary>
    ''' 添付ファイルが変更されているかを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _isChangeFile As Boolean = False

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 添付ファイルが変更されているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsChangeFile As Boolean

        Get
            Return Me._isChangeFile
        End Get

    End Property

    ''' <summary>
    ''' 添付ファイルの最大登録可能数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MaxAttachedCount As Integer

        Get
            Return NoteMealListInputModel.MAX_ATTACHED_COUNT
        End Get

    End Property

    ''' <summary>
    ''' 編集前の食事情報を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BeforeRecordDate As New List(Of Date)()

    ''' <summary>
    ''' 編集前の食事区分を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BeforeMealType As New List(Of QyMealTypeEnum)()

    ''' <summary>
    ''' 食事日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("食事日")>
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
    ''' 食事種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("食事種別")>
    Public Property MealType As QyMealTypeEnum = QyMealTypeEnum.None

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
    ''' 品目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemName As New List(Of String)()

    ''' <summary>
    ''' カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Calorie As New List(Of String)()

    ''' <summary>
    ''' 画像解析結果の候補の品目のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FoodN As New List(Of List(Of FoodItem))()

    ''' <summary>
    ''' 外部ファイルキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ForeignKey As New List(Of Guid)()

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
    Public Property Sequence As New List(Of Integer)

    ''' <summary>
    ''' 添付ファイル情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AttachedFileN As New List(Of AttachedFileItem)()

    ''' <summary>
    ''' 品目のパラメータ文字列を取得または設定します。
    ''' （文字検索からの登録用）
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PalString As New List(Of String)()

    ''' <summary>
    ''' 続けて登録かどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InputNext As String = String.Empty

    ''' <summary>
    ''' 検索されたかどうかを取得または設定します。(写真から登録画面、編集画面で利用)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchFlag As Boolean = False

    ''' <summary>
    ''' 写真解析結果の最大ページ数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchedMaxPage As Integer = Integer.MinValue

    ''' <summary>
    ''' 写真解析結果の現在ページ数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispedPage As Integer = Integer.MinValue

    ''' <summary>
    ''' 写真のデータを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PhotoData As String = String.Empty

    ''' <summary>
    ''' 写真の名前を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PhotoName As String = String.Empty

    ''' <summary>
    ''' 食事日が未来日かどうか判定を行うフラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsCheckFutureDate As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMealListInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

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
    Private Function CheckDecimalValue(value As String, Optional upperLimit As Decimal = NoteMealListInputModel.MAX_VALUE) As Decimal

        Dim result As Decimal = Decimal.MinValue

        If Not String.IsNullOrWhiteSpace(value) Then
            Dim decimalValue As Decimal = Decimal.MinValue

            If Decimal.TryParse(value, decimalValue) _
                AndAlso decimalValue >= NoteMealListInputModel.MIN_VALUE _
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
        Dim setCalorie As New List(Of String)()

        For Each cal As String In Me.Calorie
            Dim decimalValue As Decimal = Me.CheckDecimalValue(cal)

            If decimalValue = Decimal.MinValue Then
                ' 範囲エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteMealListInputModel.RANGE_ERROR_MESSAGE,
                        String.Empty,
                        "カロリー",
                        NoteMealListInputModel.MIN_VALUE,
                        NoteMealListInputModel.MAX_VALUE
                    )
                )
                Exit For
            ElseIf Not Me.CheckDecimalPartScale(decimalValue, 0) Then
                ' 小数部桁数エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteMealListInputModel.DECIMAL_PART_ERROR_MESSAGE,
                        String.Empty,
                        "カロリー"
                    )
                )
                Exit For
            Else
                setCalorie.Add(decimalValue.ToString())
            End If
        Next

        If result.Any() = False Then
            Me.Calorie.Clear()
            Me.Calorie = setCalorie
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
    Public Sub UpdateByInput(inputModel As NoteMealListInputModel) Implements IQyModelUpdater(Of NoteMealListInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.InputNext = .InputNext
                Me.BeforeRecordDate = .BeforeRecordDate
                Me.BeforeMealType = .BeforeMealType
                Me.RecordDate = .RecordDate
                Me.MealType = .MealType
                Me.Sequence = .Sequence

                Me.StartDate = .StartDate
                Me.EndDate = .EndDate

                Me.Hour = .Hour
                Me.Minute = .Minute
                Me.Meridiem = .Meridiem
                Me.ItemName = .ItemName
                Me.Calorie = .Calorie
                Me.ForeignKey = .ForeignKey
                Me.FoodN = .FoodN

                Me.SearchedMaxPage = .SearchedMaxPage
                Me.DispedPage = .DispedPage
                'Me.AttachedFileN = .AttachedFileN

            End With
        End If

    End Sub

    ''' <summary>
    ''' 添付ファイルを追加します。
    ''' </summary>
    ''' <param name="item">追加する添付ファイル情報。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function AddAttachedFile(item As AttachedFileItem) As Boolean

        Dim result As Boolean = False

        If item IsNot Nothing AndAlso Me.AttachedFileN.Count < NoteMealListInputModel.MAX_ATTACHED_COUNT Then
            If item.FileKey <> Guid.Empty And Me.AttachedFileN.Find(Function(i) i.FileKey = item.FileKey) Is Nothing Then
                Me.AttachedFileN.Add(item)
                Me._isChangeFile = True

                result = True
            ElseIf Not String.IsNullOrWhiteSpace(item.TempFileKey) And Me.AttachedFileN.Find(Function(i) i.TempFileKey.CompareTo(item.TempFileKey) = 0) Is Nothing Then
                Me.AttachedFileN.Add(item)
                Me._isChangeFile = True

                result = True
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 添付ファイルを削除します。
    ''' </summary>
    ''' <param name="item">削除する添付ファイル情報。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function RemoveAttachedFile(item As AttachedFileItem) As Boolean

        Dim result As Boolean = False

        If item IsNot Nothing Then
            Dim target As AttachedFileItem = Nothing

            If item.FileKey <> Guid.Empty Then
                target = Me.AttachedFileN.Find(Function(i) i.FileKey = item.FileKey)
            ElseIf Not String.IsNullOrWhiteSpace(item.TempFileKey) Then
                target = Me.AttachedFileN.Find(Function(i) i.TempFileKey.CompareTo(item.TempFileKey) = 0)
            End If

            If target IsNot Nothing Then
                Me.AttachedFileN.Remove(target)
                Me._isChangeFile = True

                result = True
            End If
        End If

        Return result

    End Function

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

        '食事日を判定
        If Me.RecordDate = Date.MinValue Then
            result.Add(New ValidationResult("食事日が不正です。", {"RecordDate"}))
        ElseIf Me.IsCheckFutureDate AndAlso Me.RecordDate.Date > Date.Now.Date Then
            result.Add(New ValidationResult("食事日が未来です。", {"RecordDate"}))
        End If

        '食事種別を判定
        If Me.MealType = QyMealTypeEnum.None Then
            result.Add(New ValidationResult("食事種別が不正です。", {"MealType"}))
        End If

        ' 品目名を判定
        If Me.ItemName.Count = 0 Then
            result.Add(New ValidationResult("品目名は必須項目です。", {"ItemName"}))
        Else
            For Each item As String In Me.ItemName
                If String.IsNullOrWhiteSpace(item) Then
                    result.Add(New ValidationResult("品目名は必須項目です。", {"ItemName"}))
                    Exit For
                ElseIf (item.Length() > NoteMealListInputModel.LENGTH_100) Then
                    result.Add(New ValidationResult( _
                               String.Format("{0}は{1}文字以下で入力してください。", "品目名", NoteMealListInputModel.LENGTH_100), {"ItemName"}))
                    Exit For
                End If
            Next
        End If

        ' カロリーを判定
        If Me.Calorie.Count = 0 Then
            ' 必須入力エラー
            result.Add(
                New ValidationResult(
                    Me.CreateErrorMessage(
                        NoteMealListInputModel.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        "カロリー"
                    ),
                    {"Calorie"}
                )
            )
        Else
            Dim isError As Boolean = False

            For Each cal As String In Me.Calorie
                If String.IsNullOrWhiteSpace(cal) Then
                    ' 必須入力エラー
                    result.Add(
                        New ValidationResult(
                            Me.CreateErrorMessage(
                                NoteMealListInputModel.REQUIRED_ERROR_MESSAGE,
                                String.Empty,
                                "カロリー"
                            ),
                            {"Calorie"}
                        )
                    )
                    isError = True
                    Exit For
                End If
            Next

            If isError = False Then
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
        End If

        Return result

    End Function

#End Region

End Class