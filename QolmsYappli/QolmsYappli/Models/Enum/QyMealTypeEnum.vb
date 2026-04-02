Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 食事の種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyMealTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiMealTypeEnum.None

    ''' <summary>
    ''' 朝食です。
    ''' </summary>
    ''' <remarks></remarks>
    Breakfast = QsApiMealTypeEnum.Breakfast

    ''' <summary>
    ''' 昼食です。
    ''' </summary>
    ''' <remarks></remarks>
    Lunch = QsApiMealTypeEnum.Lunch

    ''' <summary>
    ''' 夕食です。
    ''' </summary>
    ''' <remarks></remarks>
    Dinner = QsApiMealTypeEnum.Dinner

    ''' <summary>
    ''' 間食です。
    ''' </summary>
    ''' <remarks></remarks>
    Snacking = QsApiMealTypeEnum.Snacking

    ''' <summary>
    ''' その他です。
    ''' </summary>
    ''' <remarks></remarks>
    Other = QsApiMealTypeEnum.Other

End Enum