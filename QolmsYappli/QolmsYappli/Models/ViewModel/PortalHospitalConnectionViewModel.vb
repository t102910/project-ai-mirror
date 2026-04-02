''' <summary>
''' 「病院連携」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalHospitalConnectionViewModel
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
    ''' 連携システム番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携システムIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemId As String = String.Empty

    ''' <summary>
    ''' 連携名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemName As String = String.Empty

    ''' <summary>
    ''' 連携状態を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StatusType As Byte = Byte.MinValue

    ' ''' <summary>
    ' ''' 連携ユーザーの一覧を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property RelationList As New RelationUserItem()

    ''' <summary>
    ''' 連携内容を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowType As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None

    ''' <summary>
    ''' ステータス拒否の理由を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DisapprovedReason As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHospitalConnectionViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalHospitalConnectionViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalCompanyConnection)

    End Sub

#End Region

End Class
