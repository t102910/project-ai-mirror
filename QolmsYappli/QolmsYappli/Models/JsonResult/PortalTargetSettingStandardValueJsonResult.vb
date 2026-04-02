Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class PortalTargetSettingStandardValueJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' バイタル情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property VitalType As String = String.Empty

    ''' <summary>
    ''' 目標下限値 1 を取得または設定します。
    ''' 血圧（上）、血糖値（空腹時）、その他バイタル値で使用します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Lower1 As String = String.Empty

    ''' <summary>
    ''' 目標上限値 1 を取得または設定します。
    ''' 血圧（上）、血糖値（空腹時）、その他バイタル値で使用します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Upper1 As String = String.Empty

    ''' <summary>
    ''' 目標下限値 2 を取得または設定します。
    ''' 血圧（下）、血糖値（その他）で使用します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Lower2 As String = String.Empty

    ''' <summary>
    ''' 目標上限値 2 を取得または設定します。
    ''' 血圧（下）、血糖値（その他）で使用します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Upper2 As String = String.Empty

    ''' <summary>
    ''' エラー メッセージを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Message As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalTargetSettingStandardValueJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class