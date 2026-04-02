''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' Health 系画面ビューモデルの基本クラスを表します。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class QyHealthPageViewModelBase
    Inherits QyPageViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyHealthPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="QyHealthPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="pageNo">画面番号の種別。</param>
    ''' <remarks></remarks>
    Protected Sub New(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum)

        MyBase.New(mainModel, pageNo)

    End Sub

#End Region

End Class
