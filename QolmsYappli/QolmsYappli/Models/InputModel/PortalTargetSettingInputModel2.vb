Imports System.ComponentModel.DataAnnotations
Imports System.Reflection
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.ComponentModel

<Serializable()>
Public NotInheritable Class PortalTargetSettingInputModel2
    Inherits QyPortalPageViewModelBase
    Implements IQyModelUpdater(Of PortalTargetSettingInputModel2), 
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

    ' ''' <summary>
    ' ''' 入力値が重複していることを表すエラー メッセージです。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const OVERLAP_ERROR_MESSAGE As String = "{0}が重複しています。"

    ''' <summary>
    ''' 入力値が逆転していることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REVERSE_ERROR_MESSAGE As String = "{0}が逆転しています。"

    ''' <summary>
    ''' 入力値に小数部が含まれていることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_ERROR_MESSAGE As String = "{0}は整数で入力して下さい。"

    ''' <summary>
    ''' 入力値の小数部桁数が不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_DIGIT_ERROR_MESSAGE As String = "{0}は小数点以下{1}桁以内で入力してください。"

    ''' <summary>
    ''' 入力可能最小値を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MIN_VALUE As Decimal = 1D

    ''' <summary>
    ''' 入力可能最大値（体重、血圧、血糖値）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAX_VALUE As Decimal = 999999.9999D

    ''' <summary>
    ''' 入力可能最大値（カロリー）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FOUR_DIGIT_MAX_VALUE As Decimal = 9999D

    ''' <summary>
    ''' 入力可能最大値（歩数）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const SIX_DIGIT_MAX_VALUE As Decimal = 999999D

    ''' <summary>
    ''' 入力可能最大値（身長）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const HEIGHT_MAX_VALUE As Decimal = 250D

    ''' <summary>
    ''' 入力可能最大値（体重）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const WEIGHT_MAX_VALUE As Decimal = 250D

    ''' <summary>
    ''' バイタル情報の種別をキー、
    ''' 入力値の範囲のタプルを値とするディクショナリを表します。
    ''' タプルは、入力許容下限（以上）、入力許容上限（以下）、入力許容小数部桁数の 3 組で構成されます。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly vitalRanges As New Dictionary(Of QyVitalTypeEnum, Tuple(Of Decimal, Decimal, Integer))() From {
        {QyVitalTypeEnum.BodyHeight, New Tuple(Of Decimal, Decimal, Integer)(100D, 250D, 1)},
        {QyVitalTypeEnum.BodyWeight, New Tuple(Of Decimal, Decimal, Integer)(20D, 250D, 1)}
    } ' TODO:


    Private Shared ReadOnly valueRanges As New Dictionary(Of Tuple(Of String, String), Tuple(Of String, Decimal, Integer))() From {
        {New Tuple(Of String, String)("TargetValue1", String.Empty), New Tuple(Of String, Decimal, Integer)("カロリー", PortalTargetSettingInputModel2.FOUR_DIGIT_MAX_VALUE, 0)},
        {New Tuple(Of String, String)("TargetValue2", String.Empty), New Tuple(Of String, Decimal, Integer)("カロリー", PortalTargetSettingInputModel2.FOUR_DIGIT_MAX_VALUE, 0)},
        {New Tuple(Of String, String)("TargetValue3", "TargetValue4"), New Tuple(Of String, Decimal, Integer)("歩数", PortalTargetSettingInputModel2.SIX_DIGIT_MAX_VALUE, 0)},
        {New Tuple(Of String, String)("TargetValue5", "TargetValue6"), New Tuple(Of String, Decimal, Integer)("体重", PortalTargetSettingInputModel2.MAX_VALUE, 4)},
        {New Tuple(Of String, String)("TargetValue7", "TargetValue8"), New Tuple(Of String, Decimal, Integer)("血圧", PortalTargetSettingInputModel2.MAX_VALUE, 4)},
        {New Tuple(Of String, String)("TargetValue9", "TargetValue10"), New Tuple(Of String, Decimal, Integer)("血圧", PortalTargetSettingInputModel2.MAX_VALUE, 4)},
        {New Tuple(Of String, String)("TargetValue11", "TargetValue12"), New Tuple(Of String, Decimal, Integer)("血糖値", PortalTargetSettingInputModel2.MAX_VALUE, 4)},
        {New Tuple(Of String, String)("TargetValue13", "TargetValue14"), New Tuple(Of String, Decimal, Integer)("血糖値", PortalTargetSettingInputModel2.MAX_VALUE, 4)},
        {New Tuple(Of String, String)("Height", String.Empty), New Tuple(Of String, Decimal, Integer)("身長", PortalTargetSettingInputModel2.HEIGHT_MAX_VALUE, 4)},
        {New Tuple(Of String, String)("Weight", String.Empty), New Tuple(Of String, Decimal, Integer)("体重", PortalTargetSettingInputModel2.WEIGHT_MAX_VALUE, 4)}
    }

    Private Shared ReadOnly calcData1 As New List(Of Tuple(Of QySexTypeEnum, Integer, Integer, Decimal))() From {
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 1, 2, 61D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 3, 5, 54.8D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 6, 7, 44.3D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 8, 9, 40.8D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 10, 11, 37.4D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 12, 14, 31D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 15, 17, 27D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 18, 29, 24D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 30, 49, 22.3D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 50, 69, 21.5D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Male, 70, Integer.MaxValue, 21.5D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 1, 2, 59.7D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 3, 5, 52.2D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 6, 7, 41.9D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 8, 9, 38.3D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 10, 11, 34.8D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 12, 14, 29.6D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 15, 17, 25.3D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 18, 29, 22.1D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 30, 49, 21.7D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 50, 69, 20.7D),
        New Tuple(Of QySexTypeEnum, Integer, Integer, Decimal)(QySexTypeEnum.Female, 70, Integer.MaxValue, 20.7D)
    }

    Private Shared ReadOnly calcData2 As New List(Of Tuple(Of Byte, Integer, Integer, Decimal))() From {
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 1, 2, 1.35D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 3, 5, 1.35D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 6, 7, 1.35D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 8, 9, 1.4D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 10, 11, 1.45D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 12, 14, 1.5D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 15, 17, 1.55D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 18, 29, 1.5D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 30, 49, 1.5D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 50, 69, 1.5D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(1, 70, Integer.MaxValue, 1.45D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 1, 2, 1.35D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 3, 5, 1.45D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 6, 7, 1.55D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 8, 9, 1.6D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 10, 11, 1.65D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 12, 14, 1.7D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 15, 17, 1.75D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 18, 29, 1.75D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 30, 49, 1.75D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 50, 69, 1.75D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(2, 70, Integer.MaxValue, 1.7D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 1, 2, 1.35D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 3, 5, 1.45D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 6, 7, 1.75D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 8, 9, 1.8D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 10, 11, 1.85D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 12, 14, 1.9D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 15, 17, 1.95D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 18, 29, 2D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 30, 49, 2D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 50, 69, 2D),
        New Tuple(Of Byte, Integer, Integer, Decimal)(3, 70, Integer.MaxValue, 1.95D)
    }



#End Region

#Region "Variable"

    Public _sexType As QySexTypeEnum = QySexTypeEnum.None

    Public _birthday As Date = Date.MinValue

    'Private _height As Decimal = Decimal.MinValue

    'Private _weight As Decimal = Decimal.MinValue

    'Private _physicalActivityLevel As Byte = 2

#End Region

#Region "Public Property"

    ''' <summary>
    ''' クリックされたボタンの種類を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ButtonType As String = String.Empty

    ''' <summary>
    ''' 選択中のタブ ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TabId As String = String.Empty

    ''' <summary>
    ''' 体重を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Weight As String = String.Empty
    ''' <summary>
    ''' 身長を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Height As String = String.Empty

    ''' <summary>
    ''' 運動レベルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("運動レベル")>
    Public Property PhysicalActivityLevel As Integer = Integer.MinValue

    ''' <summary>
    ''' 摂取カロリー目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue1 As String = String.Empty

    ''' <summary>
    ''' 消費カロリー目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue2 As String = String.Empty


    ''' <summary>
    ''' 歩数上限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue3 As String = String.Empty

    ''' <summary>
    ''' 歩数下限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue4 As String = String.Empty

    ''' <summary>
    ''' 体重上限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue5 As String = String.Empty

    ''' <summary>
    ''' 体重下限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue6 As String = String.Empty

    ''' <summary>
    ''' 血圧（上）上限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue7 As String = String.Empty

    ''' <summary>
    ''' 血圧（上）下限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue8 As String = String.Empty

    ''' <summary>
    ''' 血圧（下）上限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue9 As String = String.Empty

    ''' <summary>
    ''' 血圧（下）下限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue10 As String = String.Empty

    ''' <summary>
    ''' 血糖値（空腹時）上限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue11 As String = String.Empty

    ''' <summary>
    ''' 血糖値（空腹時）下限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue12 As String = String.Empty

    ''' <summary>
    ''' 血糖値（その他）上限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue13 As String = String.Empty

    ''' <summary>
    ''' 血糖値（その他）下限目標を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetValue14 As String = String.Empty

    ''' <summary>
    ''' 目標体重を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetWeight As String = String.Empty

    ''' <summary>
    ''' 期限日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetDate As Date = Date.MinValue

    ''' <summary>
    ''' 標準体重を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property StdWeight As Decimal

        Get
            Return Me.CalcStdWeight()
        End Get

    End Property

    ''' <summary>
    ''' 標準体重における基礎代謝量を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property StdBasalMetabolism As Integer

        Get
            Return Convert.ToInt32(Me.CalcBasalMetabolism(Me.CalcStdWeight().ToString()))
        End Get

    End Property

    ''' <summary>
    ''' 標準体重における推定エネルギー必要量を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property StdEstimatedEnergyRequirement As Integer

        Get
            Return Convert.ToInt32(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.CalcStdWeight().ToString())))
        End Get

    End Property

    ''' <summary>
    ''' 現体重における基礎代謝量を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property NowBasalMetabolism As Integer

        Get
            Return Convert.ToInt32(Me.CalcBasalMetabolism(Me.Weight))
        End Get

    End Property

    ''' <summary>
    ''' 現体重における推定エネルギー必要量を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property NowEstimatedEnergyRequirement As Integer

        Get
            Return Convert.ToInt32(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.Weight)))
        End Get

    End Property

    ''' <summary>
    ''' 目標摂取カロリーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property NowTargetCalorieIn As Integer

        Get
            Dim decimalValue As Decimal = Math.Truncate(Me.CalcTargetCalorieIn(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.Weight))))

            If decimalValue >= 0 AndAlso decimalValue <= Integer.MaxValue Then
                Return Convert.ToInt32(decimalValue)
            Else
                Return 0
            End If
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalTargetSettingInputModel2" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(mainModel As QolmsYappliModel, height As String, weight As String, physicalActivityLevel As Byte, caloriesIn As Integer, caloriesOut As Integer, targetWeight As String, targetDate As Date, standardValueN As List(Of QhApiTargetValueItem), targetValueN As List(Of QhApiTargetValueItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalTargetSetting)

        If standardValueN IsNot Nothing AndAlso standardValueN.Any() Then StandardValueWorker.SetStandardValue(mainModel, standardValueN)
        If targetValueN IsNot Nothing AndAlso targetValueN.Any() Then TargetValueWorker.SetTargetValue(mainModel, targetValueN)

        Me._sexType = mainModel.AuthorAccount.SexType
        Me._birthday = mainModel.AuthorAccount.Birthday
        Me._Height = height
        Me._Weight = weight
        Me._PhysicalActivityLevel = Convert.ToByte(If(physicalActivityLevel >= 1 And physicalActivityLevel <= 3, physicalActivityLevel, 2))

        ' 目標体重
        Me.TargetWeight = targetWeight

        ' 期限日
        Me.TargetDate = If(targetDate = Date.MinValue, DateAdd(DateInterval.Month, 1, Date.Now.Date), targetDate)

        ' 摂取カロリー
        Me.TargetValue1 = If(caloriesIn >= 0, caloriesIn.ToString(), String.Empty)

        ' 消費カロリー
        Me.TargetValue2 = If(caloriesOut >= 0, caloriesOut.ToString(), String.Empty)

        ' 歩数
        Dim lower As Decimal = Decimal.MinusOne
        Dim upper As Decimal = Decimal.MinusOne

        TargetValueWorker.GetTargetValue(mainModel, QyVitalTypeEnum.Steps, QyStandardValueTypeEnum.None, lower, upper)
        Me.TargetValue3 = If(upper >= 0, String.Format("{0:0.####}", upper), String.Empty)
        Me.TargetValue4 = If(lower >= 0, String.Format("{0:0.####}", lower), String.Empty)

        ' 体重
        lower = Decimal.MinusOne
        upper = Decimal.MinusOne

        TargetValueWorker.GetTargetValue(mainModel, QyVitalTypeEnum.BodyWeight, QyStandardValueTypeEnum.None, lower, upper)
        Me.TargetValue5 = If(upper >= 0, String.Format("{0:0.####}", upper), String.Empty)
        Me.TargetValue6 = If(lower >= 0, String.Format("{0:0.####}", lower), String.Empty)

        ' 血圧（上）
        lower = Decimal.MinusOne
        upper = Decimal.MinusOne

        TargetValueWorker.GetTargetValue(mainModel, QyVitalTypeEnum.BloodPressure, QyStandardValueTypeEnum.BloodPressureUpper, lower, upper)
        Me.TargetValue7 = If(upper >= 0, String.Format("{0:0.####}", upper), String.Empty)
        Me.TargetValue8 = If(lower >= 0, String.Format("{0:0.####}", lower), String.Empty)

        ' 血圧（下）
        lower = Decimal.MinusOne
        upper = Decimal.MinusOne

        TargetValueWorker.GetTargetValue(mainModel, QyVitalTypeEnum.BloodPressure, QyStandardValueTypeEnum.BloodPressureLower, lower, upper)
        Me.TargetValue9 = If(upper >= 0, String.Format("{0:0.####}", upper), String.Empty)
        Me.TargetValue10 = If(lower >= 0, String.Format("{0:0.####}", lower), String.Empty)

        ' 血糖値（空腹時）
        lower = Decimal.MinusOne
        upper = Decimal.MinusOne

        TargetValueWorker.GetTargetValue(mainModel, QyVitalTypeEnum.BloodSugar, QyStandardValueTypeEnum.BloodSugarFasting, lower, upper)
        Me.TargetValue11 = If(upper >= 0, String.Format("{0:0.####}", upper), String.Empty)
        Me.TargetValue12 = If(lower >= 0, String.Format("{0:0.####}", lower), String.Empty)

        ' 血糖値（その他）
        lower = Decimal.MinusOne
        upper = Decimal.MinusOne

        TargetValueWorker.GetTargetValue(mainModel, QyVitalTypeEnum.BloodSugar, QyStandardValueTypeEnum.BloodSugarOther, lower, upper)
        Me.TargetValue13 = If(upper >= 0, String.Format("{0:0.####}", upper), String.Empty)
        Me.TargetValue14 = If(lower >= 0, String.Format("{0:0.####}", lower), String.Empty)

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 生年月日より、
    ''' 指定日における年齢を算出します。
    ''' </summary>
    ''' <param name="birthday">生年月日。</param>
    ''' <param name="oneDay">指定日。</param>
    ''' <returns>
    ''' 成功なら指定日における年齢、
    ''' 失敗なら Integer.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function GetAge(birthday As Date, oneDay As Date) As Integer

        Dim result As Integer = Integer.MinValue

        If birthday <> Date.MinValue _
            AndAlso oneDay <> Date.MinValue _
            AndAlso oneDay >= birthday Then

            Dim age As Integer = Integer.MinValue

            age = ((oneDay.Year * 10000 + oneDay.Month * 100 + oneDay.Day) - (birthday.Year * 10000 + birthday.Month * 100 + birthday.Day)) \ 10000

            If age >= Byte.MinValue AndAlso age <= Byte.MaxValue Then result = age
        End If

        Return result

    End Function

    ''' <summary>
    ''' 標準体重を算出します。
    ''' </summary>
    ''' <returns>
    ''' 成功なら標準体重、
    ''' 失敗なら 0。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CalcStdWeight() As Decimal

        Dim height As Decimal = Decimal.MinValue
        Decimal.TryParse(Me.Height, height)

        If height > Decimal.Zero Then
            Return (height * height * 22) / 10000
        Else
            Return Decimal.Zero
        End If

    End Function

    ''' <summary>
    ''' 基礎代謝量を算出します。
    ''' </summary>
    ''' <param name="weight">体重。</param>
    ''' <returns>
    ''' 成功なら基礎代謝量、
    ''' 失敗なら 0。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CalcBasalMetabolism(weight As String) As Decimal

        Dim result As Decimal = Decimal.Zero
        Dim day As Date = Me._birthday
        Dim decWeight As Decimal = Decimal.MinValue

        If Decimal.TryParse(weight,decWeight) AndAlso decWeight > Decimal.Zero AndAlso day <> Date.MinValue Then
            Dim age As Integer = Me.GetAge(day, Date.Now.Date)

            If age > 0 Then
                Dim data As Tuple(Of QySexTypeEnum, Integer, Integer, Decimal) = PortalTargetSettingInputModel2.calcData1.Find(
                    Function(i)
                        Return i.Item1 = Me._sexType AndAlso (age >= i.Item2 And age <= i.Item3)
                    End Function
                )

                If data IsNot Nothing Then result = decWeight * data.Item4
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 推定エネルギー必要量を算出します。
    ''' </summary>
    ''' <param name="basalMetabolism">基礎代謝量。</param>
    ''' <returns>
    ''' 成功なら推定エネルギー必要量、
    ''' 失敗なら 0。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CalcEstimatedEnergyRequirement(basalMetabolism As Decimal) As Decimal

        Dim result As Decimal = Decimal.Zero
        Dim day As Date = Me._birthday
        Dim decWeight As Decimal = Decimal.MinValue

        If Decimal.TryParse(weight,decWeight) AndAlso  decWeight > Decimal.Zero _
            AndAlso day <> Date.MinValue _
            AndAlso (Me.PhysicalActivityLevel >= 1 And Me.PhysicalActivityLevel <= 3) Then

            Dim age As Integer = Me.GetAge(day, Date.Now.Date)

            If age > 0 Then
                Dim data As Tuple(Of Byte, Integer, Integer, Decimal) = PortalTargetSettingInputModel2.calcData2.Find(
                    Function(i)
                        Return i.Item1 = Me._PhysicalActivityLevel AndAlso (age >= i.Item2 And age <= i.Item3)
                    End Function
                )

                If data IsNot Nothing Then result = basalMetabolism * data.Item4
            End If

        End If

        Return Convert.ToInt32(result)

    End Function

    ''' <summary>
    ''' 目標摂取カロリーを算出します。
    ''' </summary>
    ''' <param name="estimatedEnergyRequirement">推定エネルギー必要量</param>
    ''' <returns>
    ''' 成功なら目標摂取カロリー、
    ''' 失敗なら 0。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CalcTargetCalorieIn(estimatedEnergyRequirement As Decimal) As Decimal

        Dim result As Decimal = Decimal.Zero
        Dim decimalTargetWeight As Decimal = Decimal.MinValue
        Dim decWeight As Decimal = Decimal.MinValue
        Decimal.TryParse(Me.Weight,decWeight)
        Decimal.TryParse(Me.TargetWeight, decimalTargetWeight)

        If estimatedEnergyRequirement > Decimal.Zero _
            AndAlso decWeight > Decimal.Zero _
            AndAlso decimalTargetWeight > Decimal.Zero _
            AndAlso Me.TargetDate <> Date.MinValue Then

            Dim diffWeight As Decimal = decWeight - decimalTargetWeight
            Dim totalCalorieIn As Decimal = 7000 * diffWeight
            Dim diffDate As Decimal = DateDiff(DateInterval.Day, Date.Now.Date, Me.TargetDate) + 1

            result = estimatedEnergyRequirement - totalCalorieIn / diffDate

            If result < 1D Then
                result = 1D
            ElseIf result > 9999D Then
                result = 9999D
            End If
        End If

        Return result

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
    ''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 999999.9999D）。</param>
    ''' <returns>
    ''' 検証に成功なら Decimal.MinValue 以外、
    ''' 失敗なら Decimal.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalValue(value As String, Optional upperLimit As Decimal = PortalTargetSettingInputModel2.MAX_VALUE) As Decimal

        Dim result As Decimal = Decimal.MinValue

        If Not String.IsNullOrWhiteSpace(value) Then
            Dim decimalValue As Decimal = Decimal.MinValue

            If Decimal.TryParse(value, decimalValue) _
                AndAlso decimalValue >= PortalTargetSettingInputModel2.MIN_VALUE _
                AndAlso decimalValue <= upperLimit Then

                result = decimalValue
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 入力された 10 進数を検証します。
    ''' </summary>
    ''' <param name="value">入力値。</param>
    ''' <param name="lowerLimit">入力値許容下限（オプショナル、デフォルト = 1D）。</param>
    ''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 999999.9999D）。</param>
    ''' <returns>
    ''' 検証に成功なら Decimal.MinValue 以外、
    ''' 失敗なら Decimal.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalValue(value As String, Optional lowerLimit As Decimal = PortalTargetSettingInputModel2.MIN_VALUE, Optional upperLimit As Decimal = PortalTargetSettingInputModel2.MAX_VALUE) As Decimal

        Dim result As Decimal = Decimal.MinValue

        If Not String.IsNullOrWhiteSpace(value) Then
            Dim decimalValue As Decimal = Decimal.MinValue

            If Decimal.TryParse(value, decimalValue) _
                AndAlso decimalValue >= lowerLimit _
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

    Private Function CheckValue(propertyName As String) As List(Of String)

        Dim result As New List(Of String)()
        Dim key As New Tuple(Of String, String)(propertyName, String.Empty)
        Dim range As Tuple(Of String, Decimal, Integer) = If(PortalTargetSettingInputModel2.valueRanges.ContainsKey(key), PortalTargetSettingInputModel2.valueRanges(key), Nothing)
        Dim pi As PropertyInfo = Me.GetType().GetProperty(propertyName)

        If range IsNot Nothing And pi IsNot Nothing Then
            ' 値のチェック
            Dim obj As Object = pi.GetValue(Me)
            Dim value As String = If(obj Is Nothing, String.Empty, obj.ToString())
            Dim decimalValue As Decimal = Me.CheckDecimalValue(value, range.Item2)

            If decimalValue <> Decimal.MinValue Then
                If Me.CheckDecimalPartScale(decimalValue, range.Item3) Then
                    ' OK
                Else
                    ' 小数部桁数エラー
                    If range.Item3 = 0 Then
                        ' 小数部エラー
                        result.Add(
                            String.Format(
                                PortalTargetSettingInputModel2.DECIMAL_PART_ERROR_MESSAGE,
                                range.Item1
                            )
                        )
                    Else
                        ' 小数部桁数エラー
                        result.Add(
                            String.Format(
                                PortalTargetSettingInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                range.Item1,
                                range.Item3
                            )
                        )
                    End If
                End If
            Else
                ' 範囲エラー
                result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.RANGE_ERROR_MESSAGE,
                        range.Item1,
                        PortalTargetSettingInputModel2.MIN_VALUE,
                        range.Item2
                    )
                )
            End If
        Else
            ' 不正な処理
            result.Add("入力値が不正です。")
        End If

        Return result

    End Function

    Private Function CheckRangeValue(propertyName1 As String, propertyName2 As String) As List(Of String)

        Dim result As New List(Of String)()
        Dim key As New Tuple(Of String, String)(propertyName1, propertyName2)
        Dim range As Tuple(Of String, Decimal, Integer) = If(PortalTargetSettingInputModel2.valueRanges.ContainsKey(key), PortalTargetSettingInputModel2.valueRanges(key), Nothing)
        Dim pi1 As PropertyInfo = Me.GetType().GetProperty(propertyName1)
        Dim pi2 As PropertyInfo = Me.GetType().GetProperty(propertyName2)

        If range IsNot Nothing And pi1 IsNot Nothing And pi2 IsNot Nothing Then
            Dim hash As New HashSet(Of String)()

            ' 上限値のチェック
            Dim obj1 As Object = pi1.GetValue(Me)
            Dim value1 As String = If(obj1 Is Nothing, String.Empty, obj1.ToString())
            Dim decimalValue1 As Decimal = Me.CheckDecimalValue(value1, range.Item2)

            If decimalValue1 <> Decimal.MinValue Then
                If Me.CheckDecimalPartScale(decimalValue1, range.Item3) Then
                    ' OK
                Else
                    ' 小数部桁数エラー
                    If range.Item3 = 0 Then
                        ' 小数部エラー
                        hash.Add(
                            String.Format(
                                PortalTargetSettingInputModel2.DECIMAL_PART_ERROR_MESSAGE,
                                range.Item1
                            )
                        )
                    Else
                        ' 小数部桁数エラー
                        hash.Add(
                            String.Format(
                                PortalTargetSettingInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                range.Item1,
                                range.Item3
                            )
                        )
                    End If
                End If
            Else
                ' 範囲エラー
                hash.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.RANGE_ERROR_MESSAGE,
                        range.Item1,
                        PortalTargetSettingInputModel2.MIN_VALUE,
                        range.Item2
                    )
                )
            End If

            ' 下限値のチェック
            Dim obj2 As Object = pi2.GetValue(Me)
            Dim value2 As String = If(obj2 Is Nothing, String.Empty, obj2.ToString())
            Dim decimalValue2 As Decimal = Me.CheckDecimalValue(value2, range.Item2)

            If decimalValue2 <> Decimal.MinValue Then
                If Me.CheckDecimalPartScale(decimalValue2, range.Item3) Then
                    ' OK
                Else
                    ' 小数部桁数エラー
                    If range.Item3 = 0 Then
                        ' 小数部エラー
                        hash.Add(
                            String.Format(
                                PortalTargetSettingInputModel2.DECIMAL_PART_ERROR_MESSAGE,
                                range.Item1
                            )
                        )
                    Else
                        ' 小数部桁数エラー
                        hash.Add(
                            String.Format(
                                PortalTargetSettingInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                range.Item1,
                                range.Item3
                            )
                        )
                    End If
                End If
            Else
                ' 範囲エラー
                hash.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.RANGE_ERROR_MESSAGE,
                        range.Item1,
                        PortalTargetSettingInputModel2.MIN_VALUE,
                        range.Item2
                    )
                )
            End If

            If hash.Count = 0 Then
                If decimalValue1 < decimalValue2 Then
                    ' 逆転エラー
                    hash.Add(
                        String.Format(
                            PortalTargetSettingInputModel2.REVERSE_ERROR_MESSAGE,
                            range.Item1
                        )
                    )
                End If
            End If

            result.AddRange(hash)
        Else
            ' 不正な処理
            result.Add("入力値が不正です。")
        End If

        Return result

    End Function

    Private Function CheckHeightValue() As List(Of String)

        Const valueType As QyVitalTypeEnum = QyVitalTypeEnum.BodyHeight
        Const name As String = "身長"

        Dim result As New List(Of String)()
        Dim lower As Decimal = PortalTargetSettingInputModel2.vitalRanges(valueType).Item1
        Dim upper As Decimal = PortalTargetSettingInputModel2.vitalRanges(valueType).Item2
        Dim scale As Integer = PortalTargetSettingInputModel2.vitalRanges(valueType).Item3

        If String.IsNullOrWhiteSpace(Me.Height) Then
            ' 未入力エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.REQUIRED_ERROR_MESSAGE,
                        name
                    )
            )

            Return result
        End If

        'Dim decimalValue As Decimal = Me.CheckDecimalValue(
        '    Me.Height.ToString(),
        '    lower,
        '    upper
        ')
        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.Height.ToString(),
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                      String.Format(
                        PortalTargetSettingInputModel2.RANGE_ERROR_MESSAGE,
                        name,
                        lower,
                        upper
                    )
            )
        ElseIf Not Me.CheckDecimalPartScale(
            decimalValue,
            scale
        ) Then
            ' 小数部桁数エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                        name,
                        scale
                    )
            )
        End If

        Return result

    End Function

    Private Function CheckWeightValue() As List(Of String)

        Const valueType As QyVitalTypeEnum = QyVitalTypeEnum.BodyWeight
        Const name As String = "体重"

        Dim result As New List(Of String)()
        Dim lower As Decimal = PortalTargetSettingInputModel2.vitalRanges(valueType).Item1
        Dim upper As Decimal = PortalTargetSettingInputModel2.vitalRanges(valueType).Item2
        Dim scale As Integer = PortalTargetSettingInputModel2.vitalRanges(valueType).Item3

        If String.IsNullOrWhiteSpace(Me.Weight) Then
            ' 未入力エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.REQUIRED_ERROR_MESSAGE,
                        name
                    )
            )

            Return result
        End If

        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.Weight.ToString(),
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.RANGE_ERROR_MESSAGE,
                        name,
                        lower,
                       upper
                    )
            )
        ElseIf Not Me.CheckDecimalPartScale(
            decimalValue,
            scale
        ) Then
            ' 小数部桁数エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                        name,
                        scale
                    )
            )
        End If

        Return result

    End Function

    Private Function CheckTargetWeightValue() As List(Of String)

        Const valueType As QyVitalTypeEnum = QyVitalTypeEnum.BodyWeight
        Const name As String = "目標体重"

        Dim result As New List(Of String)()
        Dim lower As Decimal = PortalTargetSettingInputModel2.vitalRanges(valueType).Item1
        Dim upper As Decimal = PortalTargetSettingInputModel2.vitalRanges(valueType).Item2
        Dim scale As Integer = PortalTargetSettingInputModel2.vitalRanges(valueType).Item3

        If String.IsNullOrWhiteSpace(Me.TargetWeight) Then
            ' 未入力エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.REQUIRED_ERROR_MESSAGE,
                        name
                    )
            )

            Return result
        End If

        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.TargetWeight,
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.RANGE_ERROR_MESSAGE,
                        name,
                        lower,
                       upper
                    )
            )
        ElseIf Not Me.CheckDecimalPartScale(
            decimalValue,
            scale
        ) Then
            ' 小数部桁数エラー
            result.Add(
                    String.Format(
                        PortalTargetSettingInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                        name,
                        scale
                    )
            )
        End If

        Return result

    End Function

    Private Function CheckTargetDate() As List(Of String)

        Const name As String = "期限日"

        Dim result As New List(Of String)()

        If Me.TargetDate = Date.MinValue Then
            result.Add(
                String.Format(
                    PortalTargetSettingInputModel2.REQUIRED_ERROR_MESSAGE,
                    name
                )
            )
        Else
                            
            Dim oneMonthLater As Date = DateAdd(DateInterval.Month, 1, DateTime.Today)
            If Me.TargetDate < oneMonthLater Then
                result.Add(
                String.Format(
                    "{0}は一か月以上先を選択してください。",
                    name
                )
            )
            End If

        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    Public Sub UpdateByInput(inputModel As PortalTargetSettingInputModel2) Implements IQyModelUpdater(Of PortalTargetSettingInputModel2).UpdateByInput

        Dim decWeight As Decimal = Decimal.MinValue

        Decimal.TryParse(inputModel.Weight,decWeight)

        If inputModel IsNot Nothing Then
            With inputModel
                Me.TabId = If(String.IsNullOrWhiteSpace(inputModel.TabId), "input-1", inputModel.TabId.Trim())
                Me.ButtonType = inputModel.ButtonType
                Me.Weight = If(decWeight <= 0, 0, decWeight).ToString()
                Me.Height = If(String.IsNullOrWhiteSpace(inputModel.Height), String.Empty, inputModel.Height.Trim())
                Me.PhysicalActivityLevel = If(inputModel.PhysicalActivityLevel < 1 And inputModel.PhysicalActivityLevel > 3, 2, inputModel.PhysicalActivityLevel)
                Me.TargetWeight = If(String.IsNullOrWhiteSpace(inputModel.TargetWeight), String.Empty, inputModel.TargetWeight.Trim())
                Me.TargetDate = inputModel.TargetDate
                Me.TargetValue1 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue1), String.Empty, inputModel.TargetValue1.Trim())
                Me.TargetValue2 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue2), String.Empty, inputModel.TargetValue2.Trim())
                Me.TargetValue3 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue3), String.Empty, inputModel.TargetValue3.Trim())
                Me.TargetValue4 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue4), String.Empty, inputModel.TargetValue4.Trim())
                Me.TargetValue5 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue5), String.Empty, inputModel.TargetValue5.Trim())
                Me.TargetValue6 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue6), String.Empty, inputModel.TargetValue6.Trim())
                Me.TargetValue7 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue7), String.Empty, inputModel.TargetValue7.Trim())
                Me.TargetValue8 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue8), String.Empty, inputModel.TargetValue8.Trim())
                Me.TargetValue9 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue9), String.Empty, inputModel.TargetValue9.Trim())
                Me.TargetValue10 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue10), String.Empty, inputModel.TargetValue10.Trim())
                Me.TargetValue11 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue11), String.Empty, inputModel.TargetValue11.Trim())
                Me.TargetValue12 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue12), String.Empty, inputModel.TargetValue12.Trim())
                Me.TargetValue13 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue13), String.Empty, inputModel.TargetValue13.Trim())
                Me.TargetValue14 = If(String.IsNullOrWhiteSpace(inputModel.TargetValue14), String.Empty, inputModel.TargetValue14.Trim())
            End With
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()

        If Me.ButtonType = "calc" Then

            '体重
            Dim errorMessage As List(Of String) = Me.CheckWeightValue()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"Weight"}))
            End If

            '目標体重
            errorMessage = New List(Of String)
            errorMessage = Me.CheckTargetWeightValue()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"TargetWeight"}))
            End If

            '期限日
            errorMessage = New List(Of String)
            errorMessage = Me.CheckTargetDate()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"TargetDate"}))
            End If

        Else

            Dim dicN As New Dictionary(Of String, List(Of String))()

            'dicN.Add("TargetValue1", Me.CheckValue("TargetValue1"))
            'dicN.Add("TargetValue2", Me.CheckValue("TargetValue2"))
            dicN.Add("TargetValue4", Me.CheckRangeValue("TargetValue3", "TargetValue4"))
            dicN.Add("TargetValue6", Me.CheckRangeValue("TargetValue5", "TargetValue6"))
            dicN.Add("TargetValue8", Me.CheckRangeValue("TargetValue7", "TargetValue8"))
            dicN.Add("TargetValue10", Me.CheckRangeValue("TargetValue9", "TargetValue10"))
            dicN.Add("TargetValue12", Me.CheckRangeValue("TargetValue11", "TargetValue12"))
            dicN.Add("TargetValue14", Me.CheckRangeValue("TargetValue13", "TargetValue14"))

            If dicN.Any() Then
                'Select Case True
                '    Case dicN("TargetValue1").Count > 0
                '        Me.TabId = "input-1"

                '    Case dicN("TargetValue4").Count + dicN("TargetValue6").Count + dicN("TargetValue8").Count + dicN("TargetValue10").Count + dicN("TargetValue12").Count + dicN("TargetValue14").Count > 0
                '        Me.TabId = "input-2"

                'End Select

                dicN.ToList().ForEach(
                    Sub(i)
                        If i.Value.Any() Then
                            result.Add(New ValidationResult(String.Join(Environment.NewLine, i.Value), {i.Key}))
                        End If
                    End Sub
                )
            Else
                dicN.Add("TargetValue2", Me.CheckValue("TargetValue2"))
                If dicN.Any() Then
                    Me.TargetValue2 = String.Empty
                End If
            End If

                        
            '身長
            Dim errorMessage As List(Of String) = Me.CheckHeightValue()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"Height"}))
            End If

            '体重
            errorMessage = Me.CheckWeightValue()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"Weight"}))
            End If

            ' 運動レベル
            If physicalActivityLevel < 1 And physicalActivityLevel > 3 Then
                errorMessage =New List(Of String)()
                errorMessage.Add(String.Format(PortalTargetSettingInputModel2.REQUIRED_ERROR_MESSAGE,"運動レベル"))

                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"PhysicalActivityLevel"}))

            End If

            '目標体重
            errorMessage = New List(Of String)
            errorMessage = Me.CheckTargetWeightValue()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"TargetWeight"}))
            End If

            '期限日
            errorMessage = New List(Of String)
            errorMessage = Me.CheckTargetDate()
            If errorMessage.Any() Then
                result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessage), {"TargetDate"}))
            End If

            'アラートのある方のタブにサマリーのアラートを追加
            If result.Any() Then
                If result.Where(Function(i) i.MemberNames.Contains("Height")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("Weight")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetWeight")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetDate")).Any() _
                    Then
                    Me.TabId = "input-1"
                    result.Add(New ValidationResult(String.Join(Environment.NewLine, "カロリー目標の入力にエラーがあります。"), {"input-1"}))

                    Else If result.Where(Function(i) i.MemberNames.Contains("TargetValue4")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetValue6")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetValue8")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetValue10")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetValue12")).Any() _
                    OrElse result.Where(Function(i) i.MemberNames.Contains("TargetValue14")).Any() _
                    Then

                    Me.TabId = "input-2"
                    result.Add(New ValidationResult(String.Join(Environment.NewLine, "バイタル目標の入力にエラーがあります。"), {"input-2"}))
                End If
                

            End If


            If Me.TabId.CompareTo("input-1") <> 0 And Me.TabId.CompareTo("input-2") <> 0 Then Me.TabId = "input-1"

        End If

        Return result

    End Function

#End Region

End Class
