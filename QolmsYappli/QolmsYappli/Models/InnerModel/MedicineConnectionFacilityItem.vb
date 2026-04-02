''' <summary>
''' お薬の連携先施設の情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class MedicineConnectionFacilityItem

#Region "Public Property"

    ''' <summary>
    ''' 病院コードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FacilityKey As Guid = Guid.Empty

    ''' <summary>
    ''' カナ名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property KanaName As String = String.Empty

    ''' <summary>
    ''' 施設名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' 郵便番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PostalCode As String = String.Empty

    ''' <summary>
    ''' 住所を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Address As String = String.Empty

    ''' <summary>
    ''' 外部施設コードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PharmacyId As Integer = Integer.MinValue

    ''' <summary>
    ''' 電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tel As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="MedicineConnectionFacilityItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class