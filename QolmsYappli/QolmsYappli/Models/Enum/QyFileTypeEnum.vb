Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' ファイルの種別を表します。
''' </summary>
''' <remarks></remarks>
Public Enum QyFileTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = QsApiFileTypeEnum.None

    ''' <summary>
    ''' オリジナル ファイルです。
    ''' </summary>
    ''' <remarks></remarks>
    Original = QsApiFileTypeEnum.Original

    ''' <summary>
    ''' 必要に応じて回転変換された画像です。
    ''' 画像以外のファイルの場合は、
    ''' オリジナル ファイルになります。
    ''' </summary>
    ''' <remarks></remarks>
    Edited = QsApiFileTypeEnum.Edited

    ''' <summary>
    ''' サムネイル画像です。
    ''' 画像以外のファイルの場合は、
    ''' ファイルの種別に応じた固定の画像になります。
    ''' </summary>
    ''' <remarks></remarks>
    Thumbnail = QsApiFileTypeEnum.Thumbnail

End Enum
