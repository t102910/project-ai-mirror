Imports System.Runtime.Serialization
Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 「コルムス サイト」が使用する、
''' 日付および日付内連番ごとの検査結果情報を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ExaminationSetItem

#Region "Public Property"

    ''' <summary>
    ''' キャッシュ キー を取得または設定します。
    ''' この値は、健診 CDA ファイル 名になり、暗号化して ビュー に展開します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CacheKey As String = String.Empty

    ''' <summary>
    ''' 検査分類IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CategoryId As Integer = Integer.MinValue

    ''' <summary>
    ''' 検査日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 日付内連番を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sequence As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携先番号を取得または設定します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 施設 キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OrganizationKey As String = String.Empty

    ''' <summary>
    ''' 施設名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property OrganizationName As String = String.Empty

    ''' <summary>
    ''' 施設電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property OrganizationTel As String = String.Empty

    ''' <summary>
    ''' 検査結果情報の リスト を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property ExaminationN As New List(Of ExaminationItem)()

    ''' <summary>
    ''' 総合所見 PDF ファイル を格納する ブロブ キー の リスト を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Property OverallAssessmentPdfKeyN As New List(Of Guid)()

    ''' <summary>
    ''' DICOM健診画像アクセスキー を格納する ブロブ キー の リスト を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Property DicomUrlAccessKeyN As New List(Of String)()


    ''' <summary>
    ''' 健康年齢を取得または設定します（JOTO 用）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property HealthAge As Integer = Integer.MinValue

    ''' <summary>
    ''' 総合所見 CSV ファイル から取得した、
    ''' 検査所見・判定情報の リスト を取得または設定します（JOTO 用）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property ExaminationJudgementN As New List(Of ExaminationJudgementItem)()

    ''' <summary>
    ''' 総合所見 PDF ファイル を格納する ブロブ キー の リスト を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OverallAssessmentPdfN As New List(Of ExaminationAssociatedFileItem)()

#End Region

#Region "Public Method"

    '<Obsolete("検討中")>
    'Public Function GetKey() As Tuple(Of Date, Integer)

    '    Return New Tuple(Of Date, Integer)(Me.RecordDate.TryToValueType(Date.MinValue), Me.Sequence.TryToValueType(Integer.MinValue))

    'End Function

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExaminationSetItem" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class