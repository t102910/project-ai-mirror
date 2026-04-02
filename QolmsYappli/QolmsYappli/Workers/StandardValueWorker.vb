Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' バイタル標準値に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class StandardValueWorker

#Region "Constant"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly valueKeys As New HashSet(Of Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum))() From {
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodPressure, QyStandardValueTypeEnum.BloodPressureUpper)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodPressure, QyStandardValueTypeEnum.BloodPressureLower)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.Pulse, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodSugar, QyStandardValueTypeEnum.BloodSugarOther)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BloodSugar, QyStandardValueTypeEnum.BloodSugarFasting)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.Glycohemoglobin, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyWeight, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyWaist, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyTemperature, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.Steps, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyFatPercentage, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.MuscleMass, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BoneMass, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.VisceralFat, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BasalMetabolism, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.BodyAge, QyStandardValueTypeEnum.None)},
        {New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(QyVitalTypeEnum.TotalBodyWater, QyStandardValueTypeEnum.None)}
    }

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 値を指定して、
    ''' API 用バイタル目標値情報を作成します。
    ''' </summary>
    ''' <param name="vitalType">バイタル情報の種別。</param>
    ''' <param name="valueType">目標値の種別。</param>
    ''' <param name="lower">目標下限値。</param>
    ''' <param name="upper">目標上限値。</param>
    ''' <returns>
    ''' API 用バイタル目標値情報の新しいインスタンス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CreateTargetValue(vitalType As QyVitalTypeEnum, valueType As QyStandardValueTypeEnum, lower As Decimal, upper As Decimal) As QhApiTargetValueItem

        Return New QhApiTargetValueItem() With {
            .VitalType = vitalType.ToString(),
            .ValueType = valueType.ToString(),
            .Lower = lower.ToString(),
            .Upper = upper.ToString()
        }

    End Function

    ''' <summary>
    ''' 目標下限値と目標上限値をコピーします。
    ''' </summary>
    ''' <param name="source">コピー元 API 用バイタル目標値情報。</param>
    ''' <param name="destination">コピー先 API 用バイタル目標値情報。</param>
    ''' <remarks></remarks>
    Private Shared Sub CopyTargetValue(source As QhApiTargetValueItem, destination As QhApiTargetValueItem)

        If destination IsNot Nothing Then
            With destination
                If source Is Nothing Then
                    .Lower = Decimal.MinusOne.ToString()
                    .Upper = Decimal.MinusOne.ToString()
                Else
                    .Lower = source.Lower.TryToValueType(Decimal.MinusOne).ToString()
                    .Upper = source.Upper.TryToValueType(Decimal.MinusOne).ToString()
                End If
            End With
        End If

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="standardValueN"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetStandardValue(mainModel As QolmsYappliModel, standardValueN As List(Of QhApiTargetValueItem))

        ' 初期化
        Dim values As New Dictionary(Of Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum), QhApiTargetValueItem)()

        StandardValueWorker.valueKeys.ToList().ForEach(
            Sub(i)
                values.Add(
                    New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(i.Item1, i.Item2),
                    StandardValueWorker.CreateTargetValue(
                        i.Item1,
                        i.Item2,
                        Decimal.MinusOne,
                        Decimal.MinusOne
                    )
                )
            End Sub
        )

        If standardValueN IsNot Nothing AndAlso standardValueN.Any() Then
            standardValueN.ForEach(
                Sub(i)
                    Dim key As New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(
                        i.VitalType.TryToValueType(QyVitalTypeEnum.None),
                        i.ValueType.TryToValueType(QyStandardValueTypeEnum.None)
                    )

                    If values.ContainsKey(key) Then StandardValueWorker.CopyTargetValue(i, values(key))
                End Sub
            )
        End If

        ' 標準値を設定
        mainModel.AuthorAccount.StandardValues = values

    End Sub

    ''' <summary>
    ''' バイタル情報の種別および標準値の種別を指定して、
    ''' バイタル標準値を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="vitalType">バイタル情報の種別。</param>
    ''' <param name="valueType">標準値の種別。</param>
    ''' <param name="refLower">標準下限値が格納される変数。</param>
    ''' <param name="refUpper">標準上限値が格納される変数。</param>
    ''' <remarks></remarks>
    Public Shared Sub GetStandardValue(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, valueType As QyStandardValueTypeEnum, ByRef refLower As Decimal, ByRef refUpper As Decimal)

        Dim value As QhApiTargetValueItem = StandardValueWorker.CreateTargetValue(QyVitalTypeEnum.None, QyStandardValueTypeEnum.None, Decimal.MinusOne, Decimal.MinusOne)
        Dim key As New Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum)(vitalType, valueType)

        If mainModel.AuthorAccount.StandardValues.ContainsKey(key) Then StandardValueWorker.CopyTargetValue(mainModel.AuthorAccount.StandardValues(key), value)

        refLower = value.Lower.TryToValueType(Decimal.MinusOne)
        refUpper = value.Upper.TryToValueType(Decimal.MinusOne)

    End Sub

#End Region

End Class
