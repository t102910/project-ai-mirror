Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' ファイルの連携先の種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyFileRelationTypeEnum

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiFileRelationTypeEnum.None

    ''' <summary>
    ''' 連絡手帳です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteContact = QsApiFileRelationTypeEnum.NoteContact

    ''' <summary>
    ''' 基本情報です。
    ''' </summary>
    ''' <remarks></remarks>
    AccountInformation = QsApiFileRelationTypeEnum.AccountInformation

    ''' <summary>
    ''' 利用者カードです。
    ''' </summary>
    ''' <remarks></remarks>
    LinkagePatientCard = QsApiFileRelationTypeEnum.LinkagePatientCard

    ''' <summary>
    ''' 市販薬です。
    ''' </summary>
    ''' <remarks></remarks>
    MedicinePhoto = QsApiFileRelationTypeEnum.MedicinePhoto

    ''' <summary>
    ''' 食事です。
    ''' </summary>
    ''' <remarks></remarks>
    MealPhoto = QsApiFileRelationTypeEnum.MealPhoto

    ''' <summary>
    ''' 人物の顔写真画像です。
    ''' </summary>
    ''' <remarks></remarks>
    PersonPhoto = QsApiFileRelationTypeEnum.PersonPhoto

End Enum
