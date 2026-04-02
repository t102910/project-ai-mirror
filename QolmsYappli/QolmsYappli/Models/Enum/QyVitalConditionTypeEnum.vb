Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' バイタル情報の測定条件の種別を表します。
''' 血糖値で使用します。
''' </summary>
''' <remarks>
''' 既存のメンバーを変更しないでください。
''' 必要に応じて新規の値を持つメンバーを追加してください。
''' </remarks>
Public Enum QyVitalConditionTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' 血糖値の場合は随時を表します。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiVitalConditionTypeEnum.None

    ''' <summary>
    ''' 空腹時です（血糖値）。
    ''' </summary>
    ''' <remarks></remarks>
    Fasting = QsApiVitalConditionTypeEnum.Fasting

    ''' <summary>
    ''' 食後 2 時間未満です（血糖値）。
    ''' </summary>
    ''' <remarks></remarks>
    LessThanTwoHours = QsApiVitalConditionTypeEnum.LessThanTwoHours

    ''' <summary>
    ''' 食後 2 時間以上です（血糖値）。
    ''' </summary>
    ''' <remarks></remarks>
    NotLessThanTwoHours = QsApiVitalConditionTypeEnum.NotLessThanTwoHours

End Enum
