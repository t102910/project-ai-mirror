Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports MGF.QOLMS.QolmsApiCoreV1
'Imports System.Reflection

<Serializable()>
Public NotInheritable Class StartSetupInputModel2
    Inherits QyPageViewModelBase
    Implements IQyModelUpdater(Of StartSetupInputModel2), 
    IValidatableObject

#Region "Constant"

    ' ''' <summary>
    ' ''' ダミーのセッションIDを表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Shared SESSION_ID As String = New String("Z"c, 100)

    ' ''' <summary>
    ' ''' ダミーのAPI認証キーを表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Shared API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    ''' <summary>
    ''' 入力値が不正であることを表す標準のエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DEFAULT_ERROR_MESSAGE As String = "{0}が不正です。"

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
    ''' 入力可能最大値（歩数以外）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAX_VALUE As Decimal = 999999.9999D

    ''' <summary>
    ''' バイタル情報の種別をキー、
    ''' 入力値の範囲のタプルを値とするディクショナリを表します。
    ''' タプルは、入力許容下限（以上）、入力許容上限（以下）、入力許容小数部桁数の 3 組で構成されます。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly valueRanges As New Dictionary(Of QyVitalTypeEnum, Tuple(Of Decimal, Decimal, Integer))() From {
        {QyVitalTypeEnum.BodyHeight, New Tuple(Of Decimal, Decimal, Integer)(100D, 250D, 1)},
        {QyVitalTypeEnum.BodyWeight, New Tuple(Of Decimal, Decimal, Integer)(20D, 250D, 1)}
    } ' TODO:


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

#Region "Public Property"

    ' ''' <summary>
    ' ''' ダミーのセッション ID を取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public ReadOnly Property SessionId As String

    '    Get
    '        Return PortalSetupInputModel.SESSION_ID
    '    End Get

    'End Property

    ' ''' <summary>
    ' ''' ダミーの API 認証キーを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public ReadOnly Property ApiAuthorizeKey As Guid

    '    Get
    '        Return PortalSetupInputModel.API_AUTHORIZE_KEY
    '    End Get

    'End Property

    ''' <summary>
    ''' 初期設定の進捗を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StepMode As Integer = 1

    ''' <summary>
    ''' クリックされたボタンの種類を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ButtonType As String = String.Empty

    ''' <summary>
    ''' アカウント キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete()>
    Public Property Accountkey As Guid = Guid.Empty

    ''' <summary>
    ''' 性別の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("性別")>
    Public Property SexType As QySexTypeEnum = QySexTypeEnum.None

    ''' <summary>
    ''' 生年月日の年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    Public Property BirthYear As Integer = Integer.MinValue

    ''' <summary>
    ''' 生年月日の月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    Public Property BirthMonth As Integer = Integer.MinValue

    ''' <summary>
    ''' 生年月日の日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    Public Property BirthDay As Integer = Integer.MinValue

    ''' <summary>
    ''' 身長を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("身長")>
    Public Property Height As String = String.Empty 'Decimal = Decimal.MinValue

    ''' <summary>
    ''' 体重を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("体重")>
    Public Property Weight As String = String.Empty 'Decimal = Decimal.MinValue

    ''' <summary>
    ''' 身体活動レベルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1：運動量が少ない人
    ''' 2：運動量が標準の人
    ''' 3：運動量が多めの人
    ''' </remarks>
    <DisplayName("運動量")>
    Public Property PhysicalActivityLevel As Byte = 2

    ''' <summary>
    ''' 目標摂取カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("摂取カロリー")>
    Public Property CaloriesIn As String = String.Empty 'Integer = Integer.MinValue

    ''' <summary>
    ''' 目標消費カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("消費カロリー")>
    Public Property CaloriesOut As String = String.Empty 'Integer = Integer.MinValue

    ''' <summary>
    ''' 目標体重を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("目標体重")>
    Public Property TargetWeight As String = String.Empty 'Decimal = Decimal.MinValue

    ''' <summary>
    ''' 期限日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("期限日")>
    Public Property TargetDate As String = String.Empty 'Date = Date.MinValue

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
            'Return Convert.ToInt32(Me.CalcBasalMetabolism(Me.CalcStdWeight()))
            Dim decimalValue As Decimal = Math.Truncate(Me.CalcBasalMetabolism(Me.CalcStdWeight()))

            If decimalValue >= 0 AndAlso decimalValue <= Integer.MaxValue Then
                Return Convert.ToInt32(decimalValue)
            Else
                Return 0
            End If
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
            'Return Convert.ToInt32(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.CalcStdWeight())))
            Dim decimalValue As Decimal = Math.Truncate(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.CalcStdWeight())))

            If decimalValue >= 0 AndAlso decimalValue <= Integer.MaxValue Then
                Return Convert.ToInt32(decimalValue)
            Else
                Return 0
            End If
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
            'Return Convert.ToInt32(Me.CalcBasalMetabolism(Me.Weight.TryToValueType(Decimal.MinValue)))
            Dim decimalValue As Decimal = Math.Truncate(Me.CalcBasalMetabolism(Me.Weight.TryToValueType(Decimal.MinValue)))

            If decimalValue >= 0 AndAlso decimalValue <= Integer.MaxValue Then
                Return Convert.ToInt32(decimalValue)
            Else
                Return 0
            End If
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
            'Return Convert.ToInt32(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.Weight.TryToValueType(Decimal.MinValue))))
            Dim decimalValue As Decimal = Math.Truncate(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.Weight.TryToValueType(Decimal.MinValue))))

            If decimalValue >= 0 AndAlso decimalValue <= Integer.MaxValue Then
                Return Convert.ToInt32(decimalValue)
            Else
                Return 0
            End If
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
            Dim decimalValue As Decimal = Math.Truncate(Me.CalcTargetCalorieIn(Me.CalcEstimatedEnergyRequirement(Me.CalcBasalMetabolism(Me.Weight.TryToValueType(Decimal.MinValue)))))

            If decimalValue >= 0 AndAlso decimalValue <= Integer.MaxValue Then
                Return Convert.ToInt32(decimalValue)
            Else
                Return 0
            End If
        End Get

    End Property

    Public ReadOnly Property BirthdayDate As Date

        Get
            Dim day As Date = Date.MinValue

            If (Me.BirthYear >= 1 And Me.BirthYear <= 9999) _
                AndAlso (Me.BirthMonth >= 1 And Me.BirthMonth <= 12) _
                AndAlso (Me.BirthDay >= 1 And Me.BirthDay <= 31) _
                AndAlso Date.TryParseExact(String.Format("{0:d4}{1:d2}{2:d2}", Me.BirthYear, Me.BirthMonth, Me.BirthDay), "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None, day) _
                AndAlso day <> Date.MinValue Then

                Return day
            Else
                Return Date.MinValue
            End If

        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="StartSetupInputModel2" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.StartSetup)

        Me.StepMode = 1
        Me.ButtonType = String.Empty
        Me.Accountkey = mainModel.AuthorAccount.AccountKey
        Me.SexType = mainModel.AuthorAccount.SexType
        Me.BirthYear = mainModel.AuthorAccount.Birthday.Year
        Me.BirthMonth = mainModel.AuthorAccount.Birthday.Month
        Me.BirthDay = mainModel.AuthorAccount.Birthday.Day
        Me.Height = String.Empty 'Decimal.MinValue
        Me.Weight = String.Empty 'Decimal.MinValue
        Me.PhysicalActivityLevel = 2
        Me.CaloriesIn = String.Empty 'Integer.MinValue
        Me.CaloriesOut = String.Empty 'Integer.MinValue
        Me.TargetWeight = String.Empty 'Decimal.MinValue
        Me.TargetDate = String.Empty 'Date.MinValue

    End Sub

    'Public Sub New(accountKey As Guid, sexType As QySexTypeEnum, birthday As Date)

    '    Me.StepMode = 1
    '    Me.Accountkey = accountKey
    '    Me.SexType = sexType
    '    Me.BirthYear = birthday.Year
    '    Me.BirthMonth = birthday.Month
    '    Me.BirthDay = birthday.Day
    '    Me.Height = Decimal.MinValue
    '    Me.Weight = Decimal.MinValue
    '    Me.PhysicalActivityLevel = 2
    '    Me.CaloriesIn = Integer.MinValue
    '    Me.CaloriesOut = Integer.MinValue

    'End Sub

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

    ' ''' <summary>
    ' ''' 標準体重を算出します。
    ' ''' </summary>
    ' ''' <returns>
    ' ''' 成功なら標準体重、
    ' ''' 失敗なら 0。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Private Function CalcStdWeight() As Decimal

    '    If Me.Height > Decimal.Zero Then
    '        Return (Me.Height * Me.Height * 22) / 10000
    '    Else
    '        Return Decimal.Zero
    '    End If

    'End Function

    ''' <summary>
    ''' 標準体重を算出します。
    ''' </summary>
    ''' <returns>
    ''' 成功なら標準体重、
    ''' 失敗なら 0。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CalcStdWeight() As Decimal

        Dim decimalHeight As Decimal = Decimal.MinValue

        decimalHeight = Me.Height.TryToValueType(Decimal.MinValue)

        If decimalHeight > Decimal.Zero Then
            Try
                Return (decimalHeight * decimalHeight * 22) / 10000
            Catch
                Return Decimal.Zero
            End Try
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
    Private Function CalcBasalMetabolism(weight As Decimal) As Decimal

        Dim result As Decimal = Decimal.Zero
        Dim day As Date = Me.BirthdayDate

        If weight > Decimal.Zero AndAlso day <> Date.MinValue Then
            Dim age As Integer = Me.GetAge(day, Date.Now.Date)

            If age > 0 Then
                Dim data As Tuple(Of QySexTypeEnum, Integer, Integer, Decimal) = StartSetupInputModel2.calcData1.Find(
                    Function(i)
                        Return i.Item1 = Me.SexType AndAlso (age >= i.Item2 And age <= i.Item3)
                    End Function
                )

                If data IsNot Nothing Then
                    Try
                        result = weight * data.Item4
                    Catch
                        result = Decimal.Zero
                    End Try
                End If
            End If
        End If

        Return result

    End Function

    ' ''' <summary>
    ' ''' 推定エネルギー必要量を算出します。
    ' ''' </summary>
    ' ''' <param name="basalMetabolism">基礎代謝量。</param>
    ' ''' <returns>
    ' ''' 成功なら推定エネルギー必要量、
    ' ''' 失敗なら 0。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Private Function CalcEstimatedEnergyRequirement(basalMetabolism As Decimal) As Decimal

    '    Dim result As Decimal = Decimal.Zero
    '    Dim day As Date = Me.BirthdayDate

    '    If Me.Weight > Decimal.Zero _
    '        AndAlso day <> Date.MinValue _
    '        AndAlso (Me.PhysicalActivityLevel >= 1 And Me.PhysicalActivityLevel <= 3) Then

    '        Dim age As Integer = Me.GetAge(day, Date.Now.Date)

    '        If age > 0 Then
    '            Dim data As Tuple(Of Byte, Integer, Integer, Decimal) = StartSetupInputModel2.calcData2.Find(
    '                Function(i)
    '                    Return i.Item1 = Me.PhysicalActivityLevel AndAlso (age >= i.Item2 And age <= i.Item3)
    '                End Function
    '            )

    '            If data IsNot Nothing Then result = basalMetabolism * data.Item4
    '        End If

    '    End If

    '    Return Convert.ToInt32(result)

    'End Function

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
        Dim decimalWeight As Decimal = Decimal.MinValue
        Dim day As Date = Me.BirthdayDate

        decimalWeight = Me.Weight.TryToValueType(Decimal.MinValue)

        If basalMetabolism > Decimal.Zero _
            AndAlso decimalWeight > Decimal.Zero _
            AndAlso day <> Date.MinValue _
            AndAlso (Me.PhysicalActivityLevel >= 1 And Me.PhysicalActivityLevel <= 3) Then

            Dim age As Integer = Me.GetAge(day, Date.Now.Date)

            If age > 0 Then
                Dim data As Tuple(Of Byte, Integer, Integer, Decimal) = StartSetupInputModel2.calcData2.Find(
                    Function(i)
                        Return i.Item1 = Me.PhysicalActivityLevel AndAlso (age >= i.Item2 And age <= i.Item3)
                    End Function
                )

                If data IsNot Nothing Then
                    Try
                        result = basalMetabolism * data.Item4
                    Catch
                        result = Decimal.Zero
                    End Try
                End If
            End If
        End If

        'Return Convert.ToInt32(result)
        Return result

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
        Dim decimalWeight As Decimal = Decimal.MinValue
        Dim decimalTargetWeight As Decimal = Decimal.MinValue
        Dim targetDate As Date = Date.MinValue

        decimalWeight = Me.Weight.TryToValueType(Decimal.MinValue)
        decimalTargetWeight = Me.TargetWeight.TryToValueType(Decimal.MinValue)

        If estimatedEnergyRequirement > Decimal.Zero _
            AndAlso decimalWeight > Decimal.Zero _
            AndAlso decimalTargetWeight > Decimal.Zero _
            AndAlso String.IsNullOrWhiteSpace(Me.TargetDate) = False _
            AndAlso Date.TryParse(Me.TargetDate, targetDate) Then

            Dim diffWeight As Decimal = decimalWeight - decimalTargetWeight
            Dim totalCalorieIn As Decimal = 7000 * diffWeight
            Dim diffDate As Decimal = DateDiff(DateInterval.Day, Date.Now.Date, targetDate) + 1

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
    ''' <param name="lowerLimit">入力値許容下限（オプショナル、デフォルト = 1D）。</param>
    ''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 999999.9999D）。</param>
    ''' <returns>
    ''' 検証に成功なら Decimal.MinValue 以外、
    ''' 失敗なら Decimal.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalValue(value As String, Optional lowerLimit As Decimal = StartSetupInputModel2.MIN_VALUE, Optional upperLimit As Decimal = StartSetupInputModel2.MAX_VALUE) As Decimal

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

    Private Function CheckSexTypeValue() As List(Of String)

        Const name As String = "性別"

        Select Case Me.SexType
            Case QySexTypeEnum.Male, QySexTypeEnum.Female
                ' OK
                Return New List(Of String)()

            Case Else
                ' エラー
                Return New List(Of String)() From {
                    Me.CreateErrorMessage(
                        StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        name
                    )
                }

        End Select

    End Function

    Private Function CheckBirthdayValue() As List(Of String)

        Const name As String = "生年月日"

        Dim day As Date = Me.BirthdayDate()

        If day <> Date.MinValue Then
            ' OK
            Return New List(Of String)()
        Else
            ' エラー
            Return New List(Of String)() From {
                Me.CreateErrorMessage(
                    StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                    String.Empty,
                    name
                )
            }
        End If

    End Function

    Private Function CheckHeightValue() As List(Of String)

        Const valueType As QyVitalTypeEnum = QyVitalTypeEnum.BodyHeight
        Const name As String = "身長"

        Dim result As New List(Of String)()
        Dim lower As Decimal = StartSetupInputModel2.valueRanges(valueType).Item1
        Dim upper As Decimal = StartSetupInputModel2.valueRanges(valueType).Item2
        Dim scale As Integer = StartSetupInputModel2.valueRanges(valueType).Item3

        'Dim decimalValue As Decimal = Me.CheckDecimalValue(
        '    Me.Height.ToString(),
        '    lower,
        '    upper
        ')
        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.Height,
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.RANGE_ERROR_MESSAGE,
                    String.Empty,
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
                Me.CreateErrorMessage(
                    StartSetupInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                    String.Empty,
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
        Dim lower As Decimal = StartSetupInputModel2.valueRanges(valueType).Item1
        Dim upper As Decimal = StartSetupInputModel2.valueRanges(valueType).Item2
        Dim scale As Integer = StartSetupInputModel2.valueRanges(valueType).Item3

        'Dim decimalValue As Decimal = Me.CheckDecimalValue(
        '    Me.Weight.ToString(),
        '    lower,
        '    upper
        ')
        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.Weight,
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.RANGE_ERROR_MESSAGE,
                    String.Empty,
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
                Me.CreateErrorMessage(
                    StartSetupInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                    String.Empty,
                    name,
                    scale
                )
            )
        End If

        Return result

    End Function

    Private Function CheckPhysicalActivityLevelValue() As List(Of String)

        Const name As String = "運動量"

        Select Case Me.PhysicalActivityLevel
            Case 1, 2, 3
                ' OK
                Return New List(Of String)()

            Case Else
                ' エラー
                Return New List(Of String)() From {
                    Me.CreateErrorMessage(
                        StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        name
                    )
                }

        End Select

    End Function

    Private Function CheckCaloriesInValue() As List(Of String)

        Const name As String = "摂取カロリー"

        Dim result As New List(Of String)()
        Dim lower As Decimal = 1D
        Dim upper As Decimal = 9999D
        Dim scale As Integer = 0

        'Dim decimalValue As Decimal = Me.CheckDecimalValue(
        '    Me.CaloriesIn.ToString(),
        '    lower,
        '    upper
        ')
        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.CaloriesIn,
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.RANGE_ERROR_MESSAGE,
                    String.Empty,
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
                Me.CreateErrorMessage(
                    StartSetupInputModel2.DECIMAL_PART_ERROR_MESSAGE,
                    String.Empty,
                    name,
                    scale
                )
            )
        End If

        Return result

    End Function

    Private Function CheckCaloriesOutValue() As List(Of String)

        Const name As String = "消費カロリー"

        Dim result As New List(Of String)()
        Dim lower As Decimal = 1D
        Dim upper As Decimal = 9999D
        Dim scale As Integer = 0

        'Dim decimalValue As Decimal = Me.CheckDecimalValue(
        '    Me.CaloriesOut.ToString(),
        '    lower,
        '    upper
        ')
        Dim decimalValue As Decimal = Me.CheckDecimalValue(
            Me.CaloriesOut,
            lower,
            upper
        )

        If decimalValue = Decimal.MinValue Then
            ' 範囲検証エラー
            result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.RANGE_ERROR_MESSAGE,
                    String.Empty,
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
                Me.CreateErrorMessage(
                    StartSetupInputModel2.DECIMAL_PART_ERROR_MESSAGE,
                    String.Empty,
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
        Dim lower As Decimal = StartSetupInputModel2.valueRanges(valueType).Item1
        Dim upper As Decimal = StartSetupInputModel2.valueRanges(valueType).Item2
        Dim scale As Integer = StartSetupInputModel2.valueRanges(valueType).Item3

        If String.IsNullOrWhiteSpace(Me.TargetWeight) Then
            result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                    String.Empty,
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
                Me.CreateErrorMessage(
                    StartSetupInputModel2.RANGE_ERROR_MESSAGE,
                    String.Empty,
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
                Me.CreateErrorMessage(
                    StartSetupInputModel2.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                    String.Empty,
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

        If String.IsNullOrWhiteSpace(Me.TargetDate) Then
            result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                    String.Empty,
                    name
                )
            )
        Else
            Dim targetDate As Date = Date.MinValue
            If Date.TryParse(Me.TargetDate, targetDate) = False Then
                result.Add(
                Me.CreateErrorMessage(
                    StartSetupInputModel2.DEFAULT_ERROR_MESSAGE,
                    String.Empty,
                    name
                )
            )
            Else
                Dim oneMonthLater As Date = DateAdd(DateInterval.Month, 1, DateTime.Today)
                If targetDate < oneMonthLater Then
                    result.Add(
                    Me.CreateErrorMessage(
                        "{0}は一か月以上先を選択してください。",
                        String.Empty,
                        name
                    )
                )
                End If
            End If
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    Public Sub UpdateByInput(inputModel As StartSetupInputModel2) Implements IQyModelUpdater(Of StartSetupInputModel2).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Select Case Me.StepMode
                    Case 1
                        Me.SexType = inputModel.SexType
                        Me.BirthYear = inputModel.BirthYear
                        Me.BirthMonth = inputModel.BirthMonth
                        Me.BirthDay = inputModel.BirthDay
                        Me.Height = inputModel.Height
                        Me.Weight = inputModel.Weight
                        Me.PhysicalActivityLevel = inputModel.PhysicalActivityLevel

                    Case 3
                        Me.ButtonType = inputModel.ButtonType
                        Me.TargetWeight = inputModel.TargetWeight
                        Me.TargetDate = inputModel.TargetDate
                        Me.CaloriesIn = inputModel.CaloriesIn
                        Me.CaloriesOut = inputModel.CaloriesOut

                End Select
            End With
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()
        Dim errorMessageN As List(Of String) = Nothing

        ' TODO:
        Select Case Me.StepMode
            Case 1
                ' 性別
                errorMessageN = Me.CheckSexTypeValue()

                If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"SexType"}))

                ' 生年月日
                errorMessageN = Me.CheckBirthdayValue()

                If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"BirthYear"}))

                ' 身長 
                'errorMessageN = Me.CheckHeightValue()

                'If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"Height"}))
                errorMessageN = New List(Of String)()

                If String.IsNullOrWhiteSpace(Me.Height) Then
                    errorMessageN.Add(
                        Me.CreateErrorMessage(
                            StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                            String.Empty,
                            "身長"
                        )
                    )
                Else
                    errorMessageN = Me.CheckHeightValue()
                End If

                If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"Height"}))

                ' 体重
                'errorMessageN = Me.CheckWeightValue()

                'If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"Weight"}))
                errorMessageN = New List(Of String)()

                If String.IsNullOrWhiteSpace(Me.Weight) Then
                    errorMessageN.Add(
                        Me.CreateErrorMessage(
                            StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                            String.Empty,
                            "体重"
                        )
                    )
                Else
                    errorMessageN = Me.CheckWeightValue()
                End If

                If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"Weight"}))

                ' 運動量
                errorMessageN = Me.CheckPhysicalActivityLevelValue()

                If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"PhysicalActivityLevel"}))
            Case 3
                If Me.ButtonType = "calc" Then
                    ' 目標体重
                    errorMessageN = New List(Of String)()
                    errorMessageN = Me.CheckTargetWeightValue()
                    If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"TargetWeight"}))

                    ' 期限日
                    errorMessageN = New List(Of String)()
                    errorMessageN = Me.CheckTargetDate()
                    If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"TargetDate"}))
                Else
                    ' 目標体重
                    errorMessageN = New List(Of String)()
                    errorMessageN = Me.CheckTargetWeightValue()
                    If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"TargetWeight"}))

                    ' 期限日
                    errorMessageN = New List(Of String)()
                    errorMessageN = Me.CheckTargetDate()
                    If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"TargetDate"}))

                    ' 摂取カロリー
                    'errorMessageN = Me.CheckCaloriesInValue()

                    'If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"CaloriesIn"}))
                    errorMessageN = New List(Of String)()

                    If String.IsNullOrWhiteSpace(Me.CaloriesIn) Then
                        errorMessageN.Add(
                            Me.CreateErrorMessage(
                                StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                                String.Empty,
                                "摂取カロリー"
                            )
                        )
                    Else
                        errorMessageN = Me.CheckCaloriesInValue()
                    End If

                    If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"CaloriesIn"}))

                    ' 消費カロリー
                    'errorMessageN = Me.CheckCaloriesOutValue()

                    'If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"CaloriesOut"}))
                    errorMessageN = New List(Of String)()

                    If String.IsNullOrWhiteSpace(Me.CaloriesOut) Then
                        errorMessageN.Add(
                            Me.CreateErrorMessage(
                                StartSetupInputModel2.REQUIRED_ERROR_MESSAGE,
                                String.Empty,
                                "消費カロリー"
                            )
                        )
                    Else
                        errorMessageN = Me.CheckCaloriesOutValue()
                    End If

                    'If errorMessageN.Any() Then result.Add(New ValidationResult(String.Join(Environment.NewLine, errorMessageN), {"CaloriesOut"}))
                    If errorMessageN.Any() Then Me.CaloriesOut = String.Empty
                End If
            Case Else
                ' TODO: 不明なエラー

        End Select

        Return result

    End Function

#End Region

End Class
