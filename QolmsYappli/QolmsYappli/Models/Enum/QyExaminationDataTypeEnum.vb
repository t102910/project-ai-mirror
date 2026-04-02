''' <summary>
''' 記録タイプを表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyExaminationDataTypeEnum

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = 0

    ''' <summary>
    ''' 健診総合所見です。
    ''' </summary>
    ''' <remarks></remarks>
    OverallAssessmentPdf = 1
    OverallAssessmentCsv = 2

    ''' <summary>
    ''' 健診画像です。
    ''' </summary>
    ''' <remarks></remarks>
    DicomData = 129

End Enum