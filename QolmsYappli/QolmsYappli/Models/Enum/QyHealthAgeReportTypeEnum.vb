Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 健康年齢レポート情報の種別を表します。
''' </summary>
''' <remarks>
''' 既存のメンバーを変更しないでください。
''' 必要に応じて新規の値を持つメンバーを追加してください。
''' </remarks>
Public Enum QyHealthAgeReportTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiHealthAgeReportTypeEnum.None

    ''' <summary>
    ''' 年齢分布です。
    ''' </summary>
    ''' <remarks></remarks>
    Distribution = QsApiHealthAgeReportTypeEnum.Distribution

    ''' <summary>
    ''' 肥満レポートです。
    ''' </summary>
    ''' <remarks></remarks>
    Fat = QsApiHealthAgeReportTypeEnum.Fat

    ''' <summary>
    ''' 血糖レポートです。
    ''' </summary>
    ''' <remarks></remarks>
    Glucose = QsApiHealthAgeReportTypeEnum.Glucose

    ''' <summary>
    ''' 血圧レポートです。
    ''' </summary>
    ''' <remarks></remarks>
    Pressure = QsApiHealthAgeReportTypeEnum.Pressure

    ''' <summary>
    ''' 脂質レポートです。
    ''' </summary>
    ''' <remarks></remarks>
    Lipid = QsApiHealthAgeReportTypeEnum.Lipid

    ''' <summary>
    ''' 肝臓レポートです。
    ''' </summary>
    ''' <remarks></remarks>
    Liver = QsApiHealthAgeReportTypeEnum.Liver

    ''' <summary>
    ''' 尿糖・尿蛋白レポートです。
    ''' </summary>
    ''' <remarks></remarks>
    Urine = QsApiHealthAgeReportTypeEnum.Urine

End Enum
