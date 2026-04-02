Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 健康年齢値情報の種別を表します。
''' </summary>
''' <remarks>
''' 既存のメンバーを変更しないでください。
''' 必要に応じて新規の値を持つメンバーを追加してください。
''' </remarks>
Public Enum QyHealthAgeValueTypeEnum

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiHealthAgeValueTypeEnum.None

    ''' <summary>
    ''' BMI です。
    ''' </summary>
    ''' <remarks></remarks>
    BMI = QsApiHealthAgeValueTypeEnum.BMI

    ''' <summary>
    ''' 収縮期血圧です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch014 = QsApiHealthAgeValueTypeEnum.Ch014

    ''' <summary>
    ''' 拡張期血圧です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch016 = QsApiHealthAgeValueTypeEnum.Ch016

    ''' <summary>
    ''' 中性脂肪です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch019 = QsApiHealthAgeValueTypeEnum.Ch019

    ''' <summary>
    ''' HDL コレステロールです。
    ''' </summary>
    ''' <remarks></remarks>
    Ch021 = QsApiHealthAgeValueTypeEnum.Ch021

    ''' <summary>
    ''' LDL コレステロールです。
    ''' </summary>
    ''' <remarks></remarks>
    Ch023 = QsApiHealthAgeValueTypeEnum.Ch023

    ''' <summary>
    ''' GOT（AST）です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch025 = QsApiHealthAgeValueTypeEnum.Ch025

    ''' <summary>
    ''' GPT（ALT）です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch027 = QsApiHealthAgeValueTypeEnum.Ch027

    ''' <summary>
    ''' γ-GT（γ-GTP）です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch029 = QsApiHealthAgeValueTypeEnum.Ch029

    ''' <summary>
    ''' HbA1c（NGSP）です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch035 = QsApiHealthAgeValueTypeEnum.Ch035

    ''' <summary>
    ''' 空腹時血糖です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch035FBG = QsApiHealthAgeValueTypeEnum.Ch035FBG

    ''' <summary>
    ''' 尿糖です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch037 = QsApiHealthAgeValueTypeEnum.Ch037

    ''' <summary>
    ''' 尿蛋白（定性）です。
    ''' </summary>
    ''' <remarks></remarks>
    Ch039 = QsApiHealthAgeValueTypeEnum.Ch039

    ''' <summary>
    ''' 健康年齢算出です。
    ''' </summary>
    ''' <remarks></remarks>
    Calculation = QsApiHealthAgeValueTypeEnum.Calculation

    ''' <summary>
    ''' 同世代健康年齢分布です。
    ''' </summary>
    ''' <remarks></remarks>
    AgeDistribution = QsApiHealthAgeValueTypeEnum.AgeDistribution

    ''' <summary>
    ''' 同世代健診値比較です。
    ''' </summary>
    ''' <remarks></remarks>
    InsComparison = QsApiHealthAgeValueTypeEnum.InsComparison

    ''' <summary>
    ''' 健診結果レベル判定です。
    ''' </summary>
    ''' <remarks></remarks>
    InsDeviance = QsApiHealthAgeValueTypeEnum.InsDeviance

    ''' <summary>
    ''' 健康年齢改善アドバイスです。
    ''' </summary>
    ''' <remarks></remarks>
    Advice = QsApiHealthAgeValueTypeEnum.Advice

End Enum
