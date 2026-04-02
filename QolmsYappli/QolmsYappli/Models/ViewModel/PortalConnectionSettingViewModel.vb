''' <summary>
''' 「連携設定」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalConnectionSettingViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TabNoType As Byte = Byte.MinValue

    ''' <summary>
    ''' 連携設定のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConnectionSettingItems As New Dictionary(Of Integer, ConnectionSettingItem)

    ''' <summary>
    ''' 病院連携設定のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConnectionSettingHospitalItemN As New Dictionary(Of Integer, ConnectionSettingHospitalItem)

    ''' <summary>
    ''' 薬局連携設定のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConnectionSettingPharmacyItemN As New Dictionary(Of Guid, ConnectionSettingPharmacyItem)

    ''' <summary>
    ''' 企業連携設定のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConnectionSettingCompanyItemN As New Dictionary(Of Integer, ConnectionSettingCompanyItem)


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalConnectionSettingViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalConnectionSettingViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalConnectionSetting)

    End Sub

#End Region

End Class
