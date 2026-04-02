''' <summary>
''' 医療機関検索のマスタ情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class SearchMstItem

    ''' <summary>
    ''' Keyを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Key As String = String.Empty
    ''' <summary>
    ''' SubKeyを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubKey As String = String.Empty

    ''' <summary>
    ''' Valueを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value As String = String.Empty
    ''' <summary>
    ''' SubValueを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubValue As String = String.Empty

End Class
