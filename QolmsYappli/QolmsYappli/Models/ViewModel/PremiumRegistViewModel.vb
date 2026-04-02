''' <summary>
''' 「プレミアム登録」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PremiumRegistViewModel
    Inherits QyPageViewModelBase

#Region "Public Property"


    ''' <summary>
    ''' 課金開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PaymentStartDate As Date = Date.MinValue


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PremiumRegistViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PremiumRegistViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PremiumRegist)


    End Sub

#End Region

#Region "Public Method"

    

#End Region

End Class
