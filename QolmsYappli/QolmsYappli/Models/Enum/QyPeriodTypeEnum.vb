Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 表示期間の種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyPeriodTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiYappliPeriodTypeEnum.None

    ''' <summary>
    ''' 一日です。
    ''' </summary>
    ''' <remarks></remarks>
    OneDay = QsApiYappliPeriodTypeEnum.OneDay

    ''' <summary>
    ''' 1週間です。
    ''' </summary>
    ''' <remarks></remarks>
    OneWeek = QsApiYappliPeriodTypeEnum.OneWeek

    ''' <summary>
    ''' 1ヶ月です。
    ''' </summary>
    ''' <remarks></remarks>
    OneMonth = QsApiYappliPeriodTypeEnum.OneMonth

    ''' <summary>
    ''' 3ヶ月です。
    ''' </summary>
    ''' <remarks></remarks>
    ThreeMonths = QsApiYappliPeriodTypeEnum.ThreeMonths

End Enum