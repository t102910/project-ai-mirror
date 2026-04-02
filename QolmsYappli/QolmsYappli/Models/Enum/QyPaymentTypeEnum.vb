''' <summary>
''' 沖縄セルラー課金種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyPaymentTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = 0

    ''' <summary>
    ''' au簡単決済です。
    ''' </summary>
    ''' <remarks></remarks>
    au = 1

    ''' <summary>
    ''' Pay.JP決済です。
    ''' </summary>
    ''' <remarks></remarks>
    pay_jp = 2

    ''' <summary>
    ''' その他です。
    ''' </summary>
    ''' <remarks></remarks>
    Other = 255

End Enum
