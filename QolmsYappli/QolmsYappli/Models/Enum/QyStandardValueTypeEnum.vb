Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' バイタル標準値の種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyStandardValueTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiStandardValueTypeEnum.None

    ''' <summary>
    ''' 血圧上です。
    ''' </summary>
    ''' <remarks></remarks>
    BloodPressureUpper = QsApiStandardValueTypeEnum.BloodPressureUpper

    ''' <summary>
    ''' 血圧下です。
    ''' </summary>
    ''' <remarks></remarks>
    BloodPressureLower = QsApiStandardValueTypeEnum.BloodPressureLower

    ''' <summary>
    ''' その他血糖値です。
    ''' </summary>
    ''' <remarks></remarks>
    BloodSugarOther = QsApiStandardValueTypeEnum.BloodSugarOther

    ''' <summary>
    ''' 空腹時血糖値です。
    ''' </summary>
    ''' <remarks></remarks>
    BloodSugarFasting = QsApiStandardValueTypeEnum.BloodSugarFasting

End Enum
