''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' 画面パーシャル ビュー モデルの基本クラスを表します。
''' </summary>
''' <typeparam name="TModel">親画面ビュー モデルの型。</typeparam>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class QyPartialViewModelBase(Of TModel As QyPageViewModelBase)

#Region "Public Property"

    ''' <summary>
    ''' 親画面ビュー モデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PageViewModel As TModel = Nothing

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
    End Sub

    ''' <summary>
    ''' 親画面ビュー モデルを指定して、
    ''' <see cref="QyPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">親画面ビュー モデル。</param>
    ''' <remarks></remarks>
    Protected Sub New(model As TModel)

        If model Is Nothing Then Throw New ArgumentNullException("model", "親画面ビュー モデルが Null 参照です。")

        Me.PageViewModel = model

    End Sub

#End Region

End Class
