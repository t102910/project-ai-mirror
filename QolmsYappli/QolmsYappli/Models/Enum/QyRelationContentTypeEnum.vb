Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 連携内容種別を表します。
''' </summary>
''' <remarks></remarks>
<Flags()>
Public Enum QyRelationContentTypeEnum As Long
    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsDbRelationTypeEnum.None

    ''' <summary>
    ''' 基本情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Information = QsDbRelationTypeEnum.Information

    ''' <summary>
    ''' バイタル情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Vital = QsDbRelationTypeEnum.Vital

    ''' <summary>
    ''' お薬情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Medicine = QsDbRelationTypeEnum.Medicine

    ''' <summary>
    '''検査・ 健診情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Examination = QsDbRelationTypeEnum.Examination

    ''' <summary>
    ''' 連絡手帳です。
    ''' </summary>
    ''' <remarks></remarks>
    Contact = QsDbRelationTypeEnum.Contact

    ''' <summary>
    ''' 歯科情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Dental = QsDbRelationTypeEnum.Dental

    ''' <summary>
    ''' 活動情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Assessment = QsDbRelationTypeEnum.Assessment
    
    ''' <summary>
    ''' 食事情報です。
    ''' </summary>
    ''' <remarks></remarks>
    Meal = QsDbRelationTypeEnum.Meal

End Enum
