''' <summary>
''' 市町村 情報を表します
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class CityItem

#Region "Public Property"
    
    ''' <summary>
    ''' 地域 NO を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property AreaNo As Integer = Integer.MinValue
    
    ''' <summary>
    ''' 市町村 NO を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property CityNo As String = String.Empty
    ''' <summary>
    ''' 市町村 名 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property CityName As String = String.Empty

    ''' <summary>
    ''' 表示順 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property DispOrder As Integer = Integer.MinValue
    
    ''' <summary>
    ''' 施設数 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property InstitutionCount As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="CouponItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
