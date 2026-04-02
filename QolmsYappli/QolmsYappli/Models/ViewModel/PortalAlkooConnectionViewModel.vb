''' <summary>
''' 「ALKOO連携」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalAlkooConnectionViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' ALKOOの連携状態を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AlkooConnectedFlag As Boolean = False

    ''' <summary>
    ''' タニタの歩数連携状態を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TanitaWalkConnectedFlag As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalAlkooConnectionViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalAlkooConnectionViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalTerms)

    End Sub

#End Region

End Class
