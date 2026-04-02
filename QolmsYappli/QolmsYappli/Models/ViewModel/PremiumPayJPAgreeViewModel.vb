''' <summary>
''' 「クレジットカード決済登録」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PremiumPayJPAgreeViewModel
    Inherits QyPageViewModelBase

#Region "Public Property"


    ''' <summary>
    ''' 同意後に遷移する画面のURL(相対パス)を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Url As String = String.Empty


    ''' <summary>
    ''' 規約文章を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Terms As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PremiumPayJPAgreeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    '''' <summary>
    '''' メイン モデルを指定して、
    '''' <see cref="PremiumPayJPAgreeViewModel" /> クラスの新しいインスタンスを初期化します。
    '''' </summary>
    '''' <param name="mainModel">メイン モデル。</param>
    '''' <remarks></remarks>
    'Public Sub New(mainModel As QolmsYappliModel)

    '    MyBase.New(mainModel, QyPageNoTypeEnum.PremiumPayJp)


    'End Sub

#End Region

#Region "Public Method"



#End Region

End Class
