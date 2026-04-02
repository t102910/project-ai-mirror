Imports System.Runtime.Serialization
Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 検査グループ情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class ExaminationGroupItem

#Region "Public Property"

    ''' <summary>
    ''' グループ番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GroupNo As Integer = Integer.MinValue

    ''' <summary>
    ''' グループ名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' コメントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Comment As String = String.Empty

    ''' <summary>
    ''' 検査グループ情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChildN As New List(Of ExaminationGroupItem)()

    ''' <summary>
    ''' 検査結果情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExaminationN As New List(Of ExaminationItem)()

    <Obsolete("検討中")>
    Public Property ParentNo As String = String.Empty

    <Obsolete("検討中")>
    Public Property DispOrder As String = String.Empty

    ''' <summary>
    ''' 判定を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Judgement As New ExaminationJudgementItem()

#End Region

#Region "Public Method"

    '<Obsolete("検討中")>
    'Public Function GetKey() As Integer

    '    Return Me.GroupNo.TryToValueType(Integer.MinValue)

    'End Function

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExaminationGroupItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class