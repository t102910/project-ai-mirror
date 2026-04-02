Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 処方情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PrescriptionItem

#Region "Public Property"

    ''' <summary>
    ''' キャッシュキーを取得または設定します。
    ''' この値は、処方CDAファイル名になり、暗号化してビューに展開します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CacheKey As String = String.Empty

    ''' <summary>
    ''' 調剤日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 日付け内連番を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sequence As Integer = Integer.MinValue

    ''' <summary>
    ''' 記録タイプを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataType As Byte = Byte.MinValue

    ''' <summary>
    ''' 情報提供元タイプタイプを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OwnerType As Byte = Byte.MinValue

    ' ''' <summary>
    ' ''' 連携先番号を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 薬局情報を表示するかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HidePharmacyInf As Boolean = False

    ''' <summary>
    ''' 薬局医療機関IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PharmacyId As String = String.Empty

    ''' <summary>
    ''' 薬局医療機関名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PharmacyName As String = String.Empty

    ''' <summary>
    ''' 調剤薬剤師名IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PharmacistId As String = String.Empty

    ''' <summary>
    ''' 調剤薬剤師名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PharmacistName As String = String.Empty

    ''' <summary>
    ''' 処方日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PrescriptionDate As Date = Date.MinValue

    ''' <summary>
    ''' 処方依頼元施設IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FacilityId As String = String.Empty

    ''' <summary>
    ''' 処方依頼元施設名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FacilityName As String = String.Empty

    ''' <summary>
    ''' 特記事項を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SpecialNotes As String = String.Empty

    ''' <summary>
    ''' メモを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Memo As String = String.Empty

    ''' <summary>
    ''' 薬品用法情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicineUsageN As New List(Of QhMedicineSetUsageItemOfJson)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PrescriptionItem" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Public Method"


#End Region

End Class