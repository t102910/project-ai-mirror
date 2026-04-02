''' <summary>
''' 検査結果項目の種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyExaminationItemTypeEnum

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = 0

    ''' <summary>
    ''' 一連検査グループです。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("未使用です。")>
    Group = 1

    ''' <summary>
    ''' 検査結果です。
    ''' </summary>
    ''' <remarks></remarks>
    Value = 2

    ''' <summary>
    ''' 検査結果に対する判定コメントです。
    ''' </summary>
    ''' <remarks></remarks>
    Judgment = 3

    ''' <summary>
    ''' 総合判定コメントです。
    ''' </summary>
    ''' <remarks></remarks>
    TotalJudgment = 4

End Enum