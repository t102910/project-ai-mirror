Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>QsDbDosageFormTypeEnum
''' YappliのHome画面に表示する薬剤の剤型の種別を表します。
''' </summary>
''' <remarks>
''' 既存のIDを変更しないでください。
''' メンバーを追加した場合は新規のIDを指定してください。
''' </remarks>
<Flags()>
Public Enum QyDosageFormTypeEnum As Integer

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsDbDosageFormTypeEnum.None

    ''' <summary>
    ''' 内服です。
    ''' </summary>
    ''' <remarks></remarks>
    Oral = QsDbDosageFormTypeEnum.Oral

    ''' <summary>
    ''' 内滴です。
    ''' </summary>
    ''' <remarks></remarks>
    Drip = QsDbDosageFormTypeEnum.Drip

    ''' <summary>
    ''' 頓服です。
    ''' </summary>
    ''' <remarks></remarks>
    DoseOfMedicine = QsDbDosageFormTypeEnum.DoseOfMedicine

    ''' <summary>
    ''' 注射です。
    ''' </summary>
    ''' <remarks></remarks>
    InjectionDrug = QsDbDosageFormTypeEnum.InjectionDrug

    ''' <summary>
    ''' 外用です。
    ''' </summary>
    ''' <remarks></remarks>
    External = QsDbDosageFormTypeEnum.External

    ''' <summary>
    ''' 浸煎です。
    ''' </summary>
    ''' <remarks></remarks>
    DipFry = QsDbDosageFormTypeEnum.DipFry

    ''' <summary>
    ''' 湯です。
    ''' </summary>
    ''' <remarks></remarks>
    Touzai = QsDbDosageFormTypeEnum.Touzai

    ''' <summary>
    ''' 材料です。
    ''' </summary>
    ''' <remarks></remarks>
    Materials = QsDbDosageFormTypeEnum.Materials

    ''' <summary>
    ''' その他です。
    ''' </summary>
    ''' <remarks></remarks>
    Other = QsDbDosageFormTypeEnum.Other




End Enum