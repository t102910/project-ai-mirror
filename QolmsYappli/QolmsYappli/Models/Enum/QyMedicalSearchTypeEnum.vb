Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' Yappliの医療機関検索に表示する検索条件チェックボックス項目を表します。
''' </summary>
''' <remarks>
''' 既存のIDを変更しないでください。
''' メンバーを追加した場合は新規のIDを指定してください。
''' </remarks>
<Flags()>
Public Enum QyMedicalSearchTypeEnum
    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiYappliMedicalSearchTypeEnum.None

    ''' <summary>
    ''' au後払いです。
    ''' </summary>
    ''' <remarks></remarks>
    AuLaterPayment = QsApiYappliMedicalSearchTypeEnum.AuLaterPayment

    ''' <summary>
    ''' 夜間診療可です。
    ''' </summary>
    ''' <remarks></remarks>
    NightTimeService = QsApiYappliMedicalSearchTypeEnum.NightTimeService

    ''' <summary>
    ''' 日祝診療可です。
    ''' </summary>
    ''' <remarks></remarks>
    HolidayService = QsApiYappliMedicalSearchTypeEnum.HolidayService

    ''' <summary>
    ''' ファルモお薬自動登録対応です。
    ''' </summary>
    ''' <remarks></remarks>
    PharumoAutoRegistration = QsApiYappliMedicalSearchTypeEnum.PharumoAutoRegistration

End Enum
