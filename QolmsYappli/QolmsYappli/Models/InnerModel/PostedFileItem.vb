''' <summary>
''' 仮アップロードされたファイル情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PostedFileItem

#Region "Public Property"

    ''' <summary>
    ''' 仮アップロード時のファイル キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TempFileKey As String = String.Empty

    ''' <summary>
    ''' ファイル名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FileName As String = String.Empty

    ''' <summary>
    ''' MIME タイプを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ContentType As String = String.Empty

    ''' <summary>
    ''' ファイルのバイト配列を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Data As Byte() = Nothing

    ''' <summary>
    ''' サムネイル画像のバイト配列を取得または設定します。。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Thumbnail As Byte() = Nothing

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PostedFileItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
