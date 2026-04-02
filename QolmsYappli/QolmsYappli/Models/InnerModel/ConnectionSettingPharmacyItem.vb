''' <summary>
''' 薬局の連携設定情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ConnectionSettingPharmacyItem

#Region "Public Property"

    ''' <summary>
    ''' 連携番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携施設キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FacilityKey As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携施設名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSysyemName As String = String.Empty

    ''' <summary>
    ''' 連携 ステータスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Status As Byte = Byte.MinValue


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
