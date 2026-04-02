Imports System.Runtime.Serialization

''' <summary>
''' カロミルのトークンのセットを格納するクラスです。
''' </summary>
''' <remarks></remarks>
<Serializable()>
<DataContract()>
Public NotInheritable Class CalomealGoalSet
#Region "Public Property"

        
    ''' <summary>
    ''' 1日の摂取目標カロリー
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property TargetDate As Date = Date.MinValue

    ''' <summary>
    ''' 1日の摂取目標カロリー
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property BasisAllCalorie As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="FitbitTokenSet" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
