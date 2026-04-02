''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' PasswordReset 系画面ビューモデルの基本クラスを表します。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class QyPasswordResetPageViewModelBase
    Inherits QyPageViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyPasswordResetPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="QyPasswordResetPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="pageNo">画面番号の種別。</param>
    ''' <remarks></remarks>
    Protected Sub New(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum)

        MyBase.New(mainModel, pageNo)

    End Sub

#End Region

End Class
