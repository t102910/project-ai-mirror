Imports System.ComponentModel.DataAnnotations
Imports System.Globalization

''' <summary>
''' 「歩数」画面および「バイタル」画面の インプット モデル を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteVitalEditInputModel
    Implements IQyModelUpdater(Of NoteVitalEditInputModel), 
    IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 入力値が不正であることを表す標準の エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DEFAULT_ERROR_MESSAGE As String = "{0}が不正です。"

    ''' <summary>
    ''' 日付が未来であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FUTURE_ERROR_MESSAGE As String = "{0}の日付が未来です。"

    ''' <summary>
    ''' 入力値が必須であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERROR_MESSAGE As String = "{0}を入力してください。"

    ''' <summary>
    ''' 入力値の範囲が不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const RANGE_ERROR_MESSAGE As String = "{0}は{1}～{2}の範囲で入力してください。"

    ''' <summary>
    ''' 入力値が重複していることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const OVERLAP_ERROR_MESSAGE As String = "{0}が重複しています。"

    ''' <summary>
    ''' 入力値が逆転していることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REVERSE_ERROR_MESSAGE As String = "{0}が逆転しています。"

    ''' <summary>
    ''' 入力値に小数部が含まれていることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_ERROR_MESSAGE As String = "{0}は整数で入力して下さい。"

    ''' <summary>
    ''' 入力値の小数部桁数が不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_DIGIT_ERROR_MESSAGE As String = "{0}は小数点以下{1}桁以内で入力してください。"

    ''' <summary>
    ''' デフォルト の入力可能最小値を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MIN_VALUE As Decimal = 1D

    ''' <summary>
    ''' デフォルト の 入力可能最大値を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAX_VALUE As Decimal = 999999.9999D

    ' ''' <summary>
    ' ''' 入力可能最大値（歩数）を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const SIX_DIGIT_MAX_VALUE As Decimal = 999999D

    ''' <summary>
    ''' AM/PM を表す正規表現です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly regexMeridiem As New Regex("^[a|p]m$", RegexOptions.IgnoreCase)

    ''' <summary>
    ''' 時を表す正規表現です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly regexHour As New Regex("(^\d$)|(^1[0|1]$)")

    ''' <summary>
    ''' 分を表す正規表現です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly regexMinute As New Regex("^[0-9]$|^[1-5][0-9]$")

    ''' <summary>
    ''' 「バイタル 情報の種別」・「バイタル 標準値の種別」 の 2 組を キー、
    ''' 「入力許容下限（以上）」・「入力許容上限（以下）」・「入力許容小数部桁数」の 3 組を値とする
    ''' ディクショナリ を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly valueRanges As New Dictionary(Of Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum), Tuple(Of Decimal, Decimal, Integer))() From {
        {
            New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.Steps, QyStandardValueTypeEnum.None),
            New Tuple(Of Decimal, Decimal, Integer)(1D, 999999D, 0)
        },
        {
            New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyWeight, QyStandardValueTypeEnum.None),
            New Tuple(Of Decimal, Decimal, Integer)(20D, 250D, 1)
        },
        {
            New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyHeight, QyStandardValueTypeEnum.None),
            New Tuple(Of Decimal, Decimal, Integer)(100D, 250D, 1)
        },
        {
            New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodPressure, QyStandardValueTypeEnum.BloodPressureUpper),
            New Tuple(Of Decimal, Decimal, Integer)(60D, 300D, 0)
        },
        {
            New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodPressure, QyStandardValueTypeEnum.BloodPressureLower),
            New Tuple(Of Decimal, Decimal, Integer)(30D, 150D, 0)
        },
        {
            New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodSugar, QyStandardValueTypeEnum.None),
            New Tuple(Of Decimal, Decimal, Integer)(20D, 600D, 0)
        }
    }

#End Region

#Region "Variable"

    ''' <summary>
    ''' 空の バイタル 情報 インプット モデル に設定する時刻を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _defaultDate As Date = Date.Now

#End Region

#Region "Public Property"

    ''' <summary>
    ''' バイタル 情報の種別の リスト を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VitalTypeN As New List(Of QyVitalTypeEnum)()

    ''' <summary>
    ''' 歩数の測定日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StepsDate As Date = Date.MinValue

    ''' <summary>
    ''' 体重の測定日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WeightDate As Date = Date.MinValue

    ''' <summary>
    ''' 血圧の測定日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PressureDate As Date = Date.MinValue

    ''' <summary>
    ''' 血糖値の測定日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SugarDate As Date = Date.MinValue

    ''' <summary>
    ''' 歩数情報 インプット モデル を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Steps As VitalValueInputModel = Me.CreateEmptyVitalValueInputModel(QyVitalTypeEnum.Steps)

    ''' <summary>
    ''' 体重情報 インプット モデル を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Weight As VitalValueInputModel = Me.CreateEmptyVitalValueInputModel(QyVitalTypeEnum.BodyWeight)

    ''' <summary>
    ''' 血圧情報 インプット モデル を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Pressure As VitalValueInputModel = Me.CreateEmptyVitalValueInputModel(QyVitalTypeEnum.BloodPressure)

    ''' <summary>
    ''' 血糖値情報 インプット モデル を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sugar As VitalValueInputModel = Me.CreateEmptyVitalValueInputModel(QyVitalTypeEnum.BloodSugar)

    ''' <summary>
    ''' この インプット モデル が値を保持しているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>
    ''' 値を保持しているなら True、
    ''' 保持していないなら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public ReadOnly Property HasValues As Boolean

        Get
            Return Me.HasValue(Me.Steps) _
                OrElse Me.HasValue(Me.Weight) _
                OrElse Me.HasValue(Me.Pressure) _
                OrElse Me.HasValue(Me.Sugar)
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteVitalEditInputModel" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteVitalEditInputModel" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <param name="recordDate">測定日。</param>
    ''' <param name="vitalTypeN">バイタル 情報の種別の リスト。</param>
    ''' <param name="defaultHeight">体重の前回値取得と合わせて使用する身長。</param>
    ''' <remarks></remarks>
    Public Sub New(
        recordDate As Date,
        vitalTypeN As List(Of QyVitalTypeEnum),
        defaultHeight As Decimal
    )

        Me.InitializeBy(recordDate, vitalTypeN, defaultHeight)

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteVitalEditInputModel" /> クラス の インスタンス を初期化します。
    ''' </summary>
    ''' <param name="recordDate">編集日。</param>
    ''' <param name="vitalTypeN">バイタル 情報の種別の リスト。</param>
    ''' <param name="defaultHeight">体重の前回値取得と合わせて使用する身長。</param>
    Private Sub InitializeBy(
        recordDate As Date,
        vitalTypeN As List(Of QyVitalTypeEnum),
        defaultHeight As Decimal
    )

        Me.StepsDate = recordDate.Date
        Me.WeightDate = recordDate.Date
        Me.PressureDate = recordDate.Date
        Me.SugarDate = recordDate.Date

        If vitalTypeN IsNot Nothing AndAlso vitalTypeN.Any() Then Me.VitalTypeN = vitalTypeN.Where(Function(i) i <> QyVitalTypeEnum.None And i <> QyVitalTypeEnum.BodyHeight).ToList()

        ' 体重が入力対象なら身長を設定しておく
        If Me.IsAvailableVitalType(QyVitalTypeEnum.BodyWeight) AndAlso defaultHeight > Decimal.Zero Then Me.Weight.Value2 = defaultHeight.ToString("0.####")

    End Sub

    ''' <summary>
    ''' 空の バイタル 情報 インプット モデル を作成します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <returns>
    ''' 空の バイタル 情報 インプット モデル。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateEmptyVitalValueInputModel(vitalType As QyVitalTypeEnum) As VitalValueInputModel

        Dim result As New VitalValueInputModel()

        With result
            ' バイタル 情報の種別
            .VitalType = vitalType

            ' AM / PM
            .Meridiem = Me._defaultDate.ToString("tt", CultureInfo.InvariantCulture).ToLower()

            ' 時
            .Hour = Me._defaultDate.ToString("%h")

            If result.Hour.CompareTo("12") = 0 Then result.Hour = "0"

            ' 分
            .Minute = Me._defaultDate.ToString("%m")

            ' 測定値 1
            .Value1 = String.Empty

            ' 測定値 2
            .Value2 = String.Empty

            ' 測定 タイミング
            .ConditionType = QyVitalConditionTypeEnum.None.ToString()
        End With

        Return result

    End Function

    ''' <summary>
    ''' バイタル 情報 インプット モデル が値を保持しているかを取得します。
    ''' </summary>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 値を保持しているなら True、
    ''' 保持していないなら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function HasValue(inputModel As VitalValueInputModel) As Boolean

        Return inputModel IsNot Nothing AndAlso IsAvailableVitalType(inputModel.VitalType) AndAlso Not String.IsNullOrWhiteSpace(inputModel.Value1 + inputModel.Value2)

    End Function

    ''' <summary>
    ''' 入力検証 エラー メッセージ を作成します。
    ''' </summary>
    ''' <param name="format">複合書式指定文字列。</param>
    ''' <param name="propertyName">プロパティ 名。</param>
    ''' <param name="displayName">表示名。</param>
    ''' <param name="arg1">書式指定する第 1 オブジェクト。</param>
    ''' <param name="arg2">書式指定する第 2 オブジェクト（オプショナル、デフォルト = Nothing）。</param>
    ''' <returns>
    ''' 入力検証 エラー メッセージ。
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
    ''' 入力された時分を検証します。
    ''' </summary>
    ''' <param name="meridiem">AM/PM を表す文字列。</param>
    ''' <param name="hour">時を表す文字列。</param>
    ''' <param name="minute">分を表す文字列。</param>
    ''' <returns>
    ''' 検証に成功なら Date.MinValue 以外、
    ''' 失敗なら Date.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckTimeValue(meridiem As String, hour As String, minute As String) As Date

        Dim result As Date = Date.MinValue

        If NoteVitalEditInputModel.regexMeridiem.IsMatch(meridiem) _
            AndAlso NoteVitalEditInputModel.regexHour.IsMatch(hour) _
            AndAlso NoteVitalEditInputModel.regexMinute.IsMatch(minute) Then

            Dim dateValue As Date = Date.MinValue

            If Date.TryParse(String.Format("#12/31/9999 {0}:{1}:00 {2}#", hour, minute, meridiem), dateValue) Then
                result = dateValue
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 入力された 10 進数を検証します。
    ''' </summary>
    ''' <param name="value">入力値。</param>
    ''' <param name="lowerLinit">入力値許容下限（オプショナル、デフォルト = 1D）。</param>
    ''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 999999.9999D）。</param>
    ''' <returns>
    ''' 検証に成功なら Decimal.MinValue 以外、
    ''' 失敗なら Decimal.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalValue(value As String, Optional lowerLinit As Decimal = NoteVitalEditInputModel.MIN_VALUE, Optional upperLimit As Decimal = NoteVitalEditInputModel.MAX_VALUE) As Decimal

        Dim result As Decimal = Decimal.MinValue

        If Not String.IsNullOrWhiteSpace(value) Then
            Dim decimalValue As Decimal = Decimal.MinValue

            If Decimal.TryParse(value, decimalValue) _
                AndAlso decimalValue >= lowerLinit _
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
    ''' 歩数の測定日時を検証します。
    ''' </summary>
    ''' <param name="recordDate">測定日。</param>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckStepsDate(recordDate As Date, inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.Steps Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 日付が有効か チェック
        If recordDate = Date.MinValue Then
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                    String.Empty,
                    "歩数の日付"
                )
            )
        ElseIf recordDate.Date > Date.Now.Date Then
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.FUTURE_ERROR_MESSAGE,
                    String.Empty,
                    "歩数"
                )
            )
        Else
            If Me.CheckTimeValue(inputModel.Meridiem, inputModel.Hour, inputModel.Minute) = Date.MinValue Then
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                        String.Empty,
                        "歩数の日付"
                    )
                )
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 体重の測定日時を検証します。
    ''' </summary>
    ''' <param name="recordDate">測定日。</param>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckWeightDate(recordDate As Date, inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.BodyWeight Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 体重が入力されていれば日時が有効か チェック
        If Not String.IsNullOrWhiteSpace(inputModel.Value1) Then
            If recordDate = Date.MinValue Then
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                        String.Empty,
                        "体重の日時"
                    )
                )
            ElseIf recordDate.Date > Date.Now.Date Then
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.FUTURE_ERROR_MESSAGE,
                        String.Empty,
                        "体重"
                    )
                )
            Else
                If Me.CheckTimeValue(inputModel.Meridiem, inputModel.Hour, inputModel.Minute) = Date.MinValue Then
                    result.Add(
                        Me.CreateErrorMessage(
                            NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                            String.Empty,
                            "体重の日時"
                        )
                    )
                End If
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 血圧の測定日時を検証します。
    ''' </summary>
    ''' <param name="recordDate">測定日。</param>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckPressureDate(recordDate As Date, inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.BloodPressure Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 日時が有効か チェック
        If recordDate = Date.MinValue Then
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                    String.Empty,
                    "血圧の日時"
                )
            )
        ElseIf recordDate.Date > Date.Now.Date Then
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.FUTURE_ERROR_MESSAGE,
                    String.Empty,
                    "血圧"
                )
            )
        Else
            If Me.CheckTimeValue(inputModel.Meridiem, inputModel.Hour, inputModel.Minute) = Date.MinValue Then
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                        String.Empty,
                        "血圧の日時"
                    )
                )
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 血糖値の測定日時を検証します。
    ''' </summary>
    ''' <param name="recordDate">測定日。</param>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckSugarDate(recordDate As Date, inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.BloodSugar Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 日時が有効か チェック
        If recordDate = Date.MinValue Then
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                    String.Empty,
                    "血糖値の日時"
                )
            )
        ElseIf recordDate.Date > Date.Now.Date Then
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.FUTURE_ERROR_MESSAGE,
                    String.Empty,
                    "血糖値"
                )
            )
        Else
            If Me.CheckTimeValue(inputModel.Meridiem, inputModel.Hour, inputModel.Minute) = Date.MinValue Then
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                        String.Empty,
                        "血糖値の日時"
                    )
                )
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 歩数を検証します。
    ''' </summary>
    ''' <param name="inputModel">バイタル情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckStepsValue(inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.Steps Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 値の許容範囲
        Dim stepsRange As Tuple(Of Decimal, Decimal, Integer) = NoteVitalEditInputModel.valueRanges(New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(inputModel.VitalType, QyStandardValueTypeEnum.None))

        ' 値の チェック（時刻および値の保持は事前に保証しておく）
        Dim decimalValue As Decimal = Me.CheckDecimalValue(inputModel.Value1, stepsRange.Item1, stepsRange.Item2)

        If decimalValue = Decimal.MinValue Then
            ' 範囲 エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.RANGE_ERROR_MESSAGE,
                    String.Empty,
                    "歩数",
                    stepsRange.Item1,
                    stepsRange.Item2
                )
            )
        ElseIf Not Me.CheckDecimalPartScale(decimalValue, stepsRange.Item3) Then
            ' 小数部桁数 エラー（整数のみ許容）
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                    String.Empty,
                    "歩数"
                )
            )
        End If

        Return result

    End Function

    ''' <summary>
    ''' 体重を検証します。
    ''' </summary>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckWeightValue(inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.BodyWeight Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 値の許容範囲
        Dim weightRange As Tuple(Of Decimal, Decimal, Integer) = NoteVitalEditInputModel.valueRanges(New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(inputModel.VitalType, QyStandardValueTypeEnum.None))
        Dim heightRange As Tuple(Of Decimal, Decimal, Integer) = NoteVitalEditInputModel.valueRanges(New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyHeight, QyStandardValueTypeEnum.None))

        ' 値の チェック（時刻および値の保持は事前に保証しておく）
        Dim decimalValue1 As Decimal = Decimal.MinValue
        Dim decimalValue2 As Decimal = Decimal.MinValue

        ' 体重が入力されていれば身長も チェック
        If Not String.IsNullOrWhiteSpace(inputModel.Value1) Then
            ' 体重の チェック
            decimalValue1 = Me.CheckDecimalValue(inputModel.Value1, weightRange.Item1, weightRange.Item2)

            If decimalValue1 = Decimal.MinValue Then
                ' 範囲 エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.RANGE_ERROR_MESSAGE,
                        String.Empty,
                        "体重",
                        weightRange.Item1,
                        weightRange.Item2
                    )
                )
            ElseIf Not Me.CheckDecimalPartScale(decimalValue1, weightRange.Item3) Then
                ' 小数部桁数 エラー（小数点以下一桁まで許容）
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                        String.Empty,
                        "体重",
                        weightRange.Item3
                    )
                )
            End If

            ' 身長の チェック
            If Not String.IsNullOrWhiteSpace(inputModel.Value2) Then
                decimalValue2 = Me.CheckDecimalValue(inputModel.Value2, heightRange.Item1, heightRange.Item2)

                If decimalValue2 = Decimal.MinValue Then
                    ' 範囲 エラー
                    result.Add(
                        Me.CreateErrorMessage(
                            NoteVitalEditInputModel.RANGE_ERROR_MESSAGE,
                            String.Empty,
                            "身長",
                            heightRange.Item1,
                            heightRange.Item2
                        )
                    )
                ElseIf Not Me.CheckDecimalPartScale(decimalValue2, heightRange.Item3) Then
                    ' 小数部桁数 エラー（小数点以下一桁まで許容）
                    result.Add(
                        Me.CreateErrorMessage(
                            NoteVitalEditInputModel.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                            String.Empty,
                            "身長",
                            heightRange.Item3
                        )
                    )
                End If
            Else
                ' 必須 エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        "身長"
                    )
                )
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 血圧を検証します。
    ''' </summary>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckPressureValue(inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.BloodPressure Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 値の許容範囲
        Dim upperRange As Tuple(Of Decimal, Decimal, Integer) = NoteVitalEditInputModel.valueRanges(New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(inputModel.VitalType, QyStandardValueTypeEnum.BloodPressureUpper))
        Dim lowerRange As Tuple(Of Decimal, Decimal, Integer) = NoteVitalEditInputModel.valueRanges(New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(inputModel.VitalType, QyStandardValueTypeEnum.BloodPressureLower))

        ' 値の チェック（時刻および値の保持は事前に保証しておく）
        Dim decimalValue1 As Decimal = Decimal.MinValue
        Dim decimalValue2 As Decimal = Decimal.MinValue

        ' 血圧（上）の チェック
        If Not String.IsNullOrWhiteSpace(inputModel.Value1) Then
            decimalValue1 = Me.CheckDecimalValue(inputModel.Value1, upperRange.Item1, upperRange.Item2)

            If decimalValue1 = Decimal.MinValue Then
                ' 範囲 エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.RANGE_ERROR_MESSAGE,
                        String.Empty,
                        "血圧（上）",
                        upperRange.Item1,
                        upperRange.Item2
                    )
                )
            ElseIf Not Me.CheckDecimalPartScale(decimalValue1, upperRange.Item3) Then
                ' 小数部桁数 エラー（整数のみ許容）
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                        String.Empty,
                        "血圧（上）"
                    )
                )
            End If
        Else
            ' 必須 エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.REQUIRED_ERROR_MESSAGE,
                    String.Empty,
                    "血圧（上）"
                )
            )
        End If

        ' 血圧（下）の チェック
        If Not String.IsNullOrWhiteSpace(inputModel.Value2) Then
            decimalValue2 = Me.CheckDecimalValue(inputModel.Value2, lowerRange.Item1, lowerRange.Item2)

            If decimalValue2 = Decimal.MinValue Then
                ' 範囲 エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.RANGE_ERROR_MESSAGE,
                        String.Empty,
                        "血圧（下）",
                        lowerRange.Item1,
                        lowerRange.Item2
                    )
                )
            ElseIf Not Me.CheckDecimalPartScale(decimalValue2, lowerRange.Item3) Then
                ' 小数部桁数 エラー（整数のみ許容）
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                        String.Empty,
                        "血圧（下）"
                    )
                )
            End If
        Else
            ' 必須 エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.REQUIRED_ERROR_MESSAGE,
                    String.Empty,
                    "血圧（下）"
                )
            )
        End If

        If Not String.IsNullOrWhiteSpace(inputModel.Value1) _
            AndAlso Not String.IsNullOrWhiteSpace(inputModel.Value2) _
            AndAlso decimalValue1 <> Decimal.MinValue _
            AndAlso decimalValue2 <> Decimal.MinValue Then

            If decimalValue1 = decimalValue2 Then
                ' 重複 エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.OVERLAP_ERROR_MESSAGE,
                        String.Empty,
                        "血圧の上下"
                    )
                )
            ElseIf decimalValue1 < decimalValue2 Then
                ' 逆転 エラー
                result.Add(
                    Me.CreateErrorMessage(
                        NoteVitalEditInputModel.REVERSE_ERROR_MESSAGE,
                        String.Empty,
                        "血圧の上下"
                    )
                )
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 血糖値を検証します。
    ''' </summary>
    ''' <param name="inputModel">バイタル 情報 インプット モデル。</param>
    ''' <returns>
    ''' 検証に成功なら空の リスト、
    ''' 失敗なら エラー メッセージ の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckSugarValue(inputModel As VitalValueInputModel) As List(Of String)

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプット モデル が Null 参照です。")
        If inputModel.VitalType <> QyVitalTypeEnum.BloodSugar Then Throw New ArgumentOutOfRangeException("inputModel.VitalType", "バイタル 情報の種別が不正です。")

        Dim result As New List(Of String)()

        ' 値の許容範囲
        Dim sugarRange As Tuple(Of Decimal, Decimal, Integer) = NoteVitalEditInputModel.valueRanges(New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(inputModel.VitalType, QyStandardValueTypeEnum.None))

        ' 値の チェック（時刻および値の保持は事前に保証しておく）
        Dim decimalValue As Decimal = Me.CheckDecimalValue(inputModel.Value1, sugarRange.Item1, sugarRange.Item2)
        Dim enumValue As QyVitalConditionTypeEnum = QyVitalConditionTypeEnum.None

        ' 血糖値の チェック
        If decimalValue = Decimal.MinValue Then
            ' 範囲 エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.RANGE_ERROR_MESSAGE,
                    String.Empty,
                    "血糖値",
                    sugarRange.Item1,
                    sugarRange.Item2
                )
            )
        ElseIf Not Me.CheckDecimalPartScale(decimalValue, sugarRange.Item3) Then
            ' 小数部桁数 エラー（整数のみ許容）
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                    String.Empty,
                    "血糖値"
                )
            )
        End If

        ' 測定 タイミング の チェック
        If Not [Enum].TryParse(inputModel.ConditionType, enumValue) Then
            ' 列挙値 エラー
            result.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.DEFAULT_ERROR_MESSAGE,
                    String.Empty,
                    "測定タイミング"
                )
            )
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 指定した バイタル 情報の種別が有効かを判定します。
    ''' </summary>
    ''' <param name="vitalType">判定する バイタル 情報の種別。</param>
    ''' <returns>
    ''' 有効なら True、
    ''' 無効なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function IsAvailableVitalType(vitalType As QyVitalTypeEnum) As Boolean

        Return Me.VitalTypeN.Contains(vitalType)

    End Function

#End Region

#Region "IQyModelUpdater Support"

    ''' <summary>
    ''' インプット モデル の内容を現在の インスタンス に反映します。
    ''' </summary>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(inputModel As NoteVitalEditInputModel) Implements IQyModelUpdater(Of NoteVitalEditInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.VitalTypeN = .VitalTypeN

                Me.StepsDate = .StepsDate
                Me.WeightDate = .WeightDate
                Me.PressureDate = .PressureDate
                Me.SugarDate = .SugarDate

                Me.Steps = .Steps
                Me.Weight = .Weight
                Me.Pressure = .Pressure
                Me.Sugar = .Sugar
            End With
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    ''' <summary>
    ''' 指定された オブジェクト が有効かどうかを判断します。
    ''' </summary>
    ''' <param name="validationContext">検証 コンテキスト。</param>
    ''' <returns>
    ''' 失敗した検証の情報を保持する コレクション。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()
        Dim messages As New List(Of String)()
        Dim dic As New Dictionary(Of QyVitalTypeEnum, Tuple(Of Date, VitalValueInputModel))()

        If Me.HasValue(Me.Steps) Then dic.Add(Me.Steps.VitalType, New Tuple(Of Date, VitalValueInputModel)(Me.StepsDate, Me.Steps))
        If Me.HasValue(Me.Weight) Then dic.Add(Me.Weight.VitalType, New Tuple(Of Date, VitalValueInputModel)(Me.WeightDate, Me.Weight))
        If Me.HasValue(Me.Pressure) Then dic.Add(Me.Pressure.VitalType, New Tuple(Of Date, VitalValueInputModel)(Me.PressureDate, Me.Pressure))
        If Me.HasValue(Me.Sugar) Then dic.Add(Me.Sugar.VitalType, New Tuple(Of Date, VitalValueInputModel)(Me.SugarDate, Me.Sugar))

        ' 値が 1 つ以上入力されているか チェック
        If dic.Count = 0 Then
            messages.Add(
                Me.CreateErrorMessage(
                    NoteVitalEditInputModel.REQUIRED_ERROR_MESSAGE,
                    String.Empty,
                    "値"
                )
            )
        End If

        ' それぞれの値を チェック
        If Not messages.Any() Then
            ' 歩数
            If dic.ContainsKey(QyVitalTypeEnum.Steps) Then
                With dic(QyVitalTypeEnum.Steps)
                    messages.AddRange(Me.CheckStepsDate(.Item1, .Item2)) ' 日時
                    messages.AddRange(Me.CheckStepsValue(.Item2)) ' 値
                End With
            End If

            ' 体重
            If dic.ContainsKey(QyVitalTypeEnum.BodyWeight) Then
                With dic(QyVitalTypeEnum.BodyWeight)
                    messages.AddRange(Me.CheckWeightDate(.Item1, .Item2)) ' 日時
                    messages.AddRange(Me.CheckWeightValue(.Item2)) ' 値
                End With
            End If

            ' 血圧
            If dic.ContainsKey(QyVitalTypeEnum.BloodPressure) Then
                With dic(QyVitalTypeEnum.BloodPressure)
                    messages.AddRange(Me.CheckPressureDate(.Item1, .Item2)) ' 日時
                    messages.AddRange(Me.CheckPressureValue(.Item2)) ' 値
                End With
            End If

            ' 血糖値
            If dic.ContainsKey(QyVitalTypeEnum.BloodSugar) Then
                With dic(QyVitalTypeEnum.BloodSugar)
                    messages.AddRange(Me.CheckSugarDate(.Item1, .Item2)) ' 日時
                    messages.AddRange(Me.CheckSugarValue(.Item2)) ' 値
                End With
            End If
        End If

        If messages.Any() Then
            ' 検証失敗
            result.AddRange(messages.ConvertAll(Function(i) New ValidationResult(i, {"Alert"})))
        Else
            ' 検証成功

            ' 歩数の時刻は午前 0 時 0 分へ置き換え
            If dic.ContainsKey(QyVitalTypeEnum.Steps) Then
                With dic(QyVitalTypeEnum.Steps).Item2
                    .Meridiem = "am"
                    .Hour = "0"
                    .Minute = "0"
                End With
            End If

            ' 体重が空なら身長も空にする
            If dic.ContainsKey(QyVitalTypeEnum.BodyWeight) Then
                With dic(QyVitalTypeEnum.BodyWeight).Item2
                    If String.IsNullOrWhiteSpace(.Value1) Then .Value2 = String.Empty
                End With
            End If
        End If

        Return result

    End Function

#End Region

End Class
