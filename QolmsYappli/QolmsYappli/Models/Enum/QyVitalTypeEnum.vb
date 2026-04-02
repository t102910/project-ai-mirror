Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' バイタル情報の種別を表します。
''' </summary>
''' <remarks>
''' 既存のメンバーを変更しないでください。
''' 必要に応じて新規の値を持つメンバーを追加してください。
''' </remarks>
Public Enum QyVitalTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiVitalTypeEnum.None

    ''' <summary>
    ''' 血圧（mmHg）です。
    ''' </summary>
    ''' <remarks></remarks>
    BloodPressure = QsApiVitalTypeEnum.BloodPressure

    ''' <summary>
    ''' 脈拍（回/分）です。
    ''' </summary>
    ''' <remarks></remarks>
    Pulse = QsApiVitalTypeEnum.Pulse

    ''' <summary>
    ''' 血糖値（mg/dl）です。
    ''' </summary>
    ''' <remarks></remarks>
    BloodSugar = QsApiVitalTypeEnum.BloodSugar

    ''' <summary>
    ''' HbA1c（%）です。
    ''' </summary>
    ''' <remarks></remarks>
    Glycohemoglobin = QsApiVitalTypeEnum.Glycohemoglobin

    ''' <summary>
    ''' 体重（kg）です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyWeight = QsApiVitalTypeEnum.BodyWeight

    ''' <summary>
    ''' 腹囲（cm）です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyWaist = QsApiVitalTypeEnum.BodyWaist

    ''' <summary>
    ''' 体温（℃）です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyTemperature = QsApiVitalTypeEnum.BodyTemperature

    ''' <summary>
    ''' 歩数（歩）です。
    ''' </summary>
    ''' <remarks></remarks>
    Steps = QsApiVitalTypeEnum.Steps

    ''' <summary>
    ''' 身長（cm）です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyHeight = QsApiVitalTypeEnum.BodyHeight

    ''' <summary>
    ''' BMI です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyMassIndex = QsApiVitalTypeEnum.BodyMassIndex

    ''' <summary>
    ''' 体脂肪率（%）です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyFatPercentage = QsApiVitalTypeEnum.BodyFatPercentage

    ''' <summary>
    ''' 筋肉量（kg）です。
    ''' </summary>
    ''' <remarks></remarks>
    MuscleMass = QsApiVitalTypeEnum.MuscleMass

    ''' <summary>
    ''' 推定骨量（kg）です。
    ''' </summary>
    ''' <remarks></remarks>
    BoneMass = QsApiVitalTypeEnum.BoneMass

    ''' <summary>
    ''' 内脂肪レベルです。
    ''' </summary>
    ''' <remarks></remarks>
    VisceralFat = QsApiVitalTypeEnum.VisceralFat

    ''' <summary>
    ''' 基礎代謝（kcal）です。
    ''' </summary>
    ''' <remarks></remarks>
    BasalMetabolism = QsApiVitalTypeEnum.BasalMetabolism

    ''' <summary>
    ''' 体内年齢（歳）です。
    ''' </summary>
    ''' <remarks></remarks>
    BodyAge = QsApiVitalTypeEnum.BodyAge

    ''' <summary>
    ''' 水分率（%）です。
    ''' </summary>
    ''' <remarks></remarks>
    TotalBodyWater = QsApiVitalTypeEnum.TotalBodyWater

    ''' <summary>
    ''' METs です。
    ''' </summary>
    ''' <remarks></remarks>

    'TODO 定義待ち 
    'Mets = QsDbVitalTypeEnum.Mets(20)

    ''' Mets = QsApiVitalTypeEnum.Mets
    Mets = 20

End Enum
