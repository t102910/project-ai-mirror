Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 沖縄セルラー会員種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyMemberShipTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsDbMemberShipTypeEnum.None

    ''' <summary>
    ''' 無料会員です。
    ''' </summary>
    ''' <remarks></remarks>
    Free = QsDbMemberShipTypeEnum.Free

    ''' <summary>
    ''' 期間限定プレミアム会員です。
    ''' </summary>
    ''' <remarks></remarks>
    LimitedTime = QsDbMemberShipTypeEnum.LimitedTime

    ''' <summary>
    ''' 有料プレミアム会員です。
    ''' </summary>
    ''' <remarks></remarks>
    Premium = QsDbMemberShipTypeEnum.Premium

    ''' <summary>
    ''' ビジネスプレミアム会員です。
    ''' </summary>
    ''' <remarks></remarks>
    Business = QsDbMemberShipTypeEnum.Business

    ''' <summary>
    ''' ビジネスプレミアム会員です。
    ''' </summary>
    ''' <remarks></remarks>
    BusinessFree = QsDbMemberShipTypeEnum.BusinessFree

    ''' <summary>
    ''' その他です。
    ''' </summary>
    ''' <remarks></remarks>
    Other = QsDbMemberShipTypeEnum.Other

End Enum
