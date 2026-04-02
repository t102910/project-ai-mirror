Imports System.Runtime.Serialization
Imports MGF.QOLMS.QolmsApiCoreV1

    ''' <summary>
    ''' 日付および日付内連番ごとの検査結果情報を表します。
    ''' この クラス は継承できません。
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public NotInheritable Class ExaminationJudgementItem


#Region "Public Property"

    ''' <summary>
    ''' 院内 コード を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property LocalCode As String = String.Empty

    ''' <summary>
    ''' 主 コース 名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property CourseName As String = String.Empty

    ''' <summary>
    ''' 項目名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' 結果値・所見を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Value As String = String.Empty

    ''' <summary>
    ''' 判定 1 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Judgment1 As String = String.Empty

    ''' <summary>
    ''' 判定 2 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Judgment2 As String = String.Empty

    ''' <summary>
    ''' 判定内容 1 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property JudgmentContent1 As String = String.Empty

    ''' <summary>
    ''' 判定内容 2 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property JudgmentContent2 As String = String.Empty

    ''' <summary>
    ''' 総合判定かを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property IsTotalJudgment As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExaminationJudgementItem" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

    End Class
