''' <summary>
''' 検査結果データ型の種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyExaminationItemValueTypeEnum


    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = 0

    ''' <summary>
    ''' 数値です。
    ''' </summary>
    ''' <remarks></remarks>
    PQ = 1

    ''' <summary>
    ''' 順序付コード値です。
    ''' </summary>
    ''' <remarks></remarks>
    CO = 2

    ''' <summary>
    ''' 順序なしコード値です。
    ''' </summary>
    ''' <remarks></remarks>
    CD = 3

    ''' <summary>
    ''' 文字列です。
    ''' </summary>
    ''' <remarks></remarks>
    ST = 4
End Enum
