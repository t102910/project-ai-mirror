''' <summary>
''' 連携ユーザーを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class RelationUserItem

#Region "Public Property"

    ''' <summary>
    ''' アカウントキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Accountkey As Guid = Guid.Empty

    ''' <summary>
    ''' 氏名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' クーポン種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RelationStatusType As Byte = Byte.MinValue

    ''' <summary>
    ''' クーポン種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RelationShowType As Byte = Byte.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="RelationUserItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
