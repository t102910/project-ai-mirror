''' <summary>
''' 「レシピ動画」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PasswordResetRecoverSMSPassCodeViewModel
    Inherits QyPasswordResetPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 暗号化した電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CryptPhoneNumber As String = String.Empty

    ''' <summary>
    ''' マスクした表示用の電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispPhoneNumber As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PasswordResetRecoverSMSPassCodeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PasswordResetRecoverSMSPassCodeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PasswordResetResetRecoverSms)

    End Sub


#End Region

End Class
