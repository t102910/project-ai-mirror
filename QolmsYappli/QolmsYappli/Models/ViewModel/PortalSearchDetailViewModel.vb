''' <summary>
''' 「医療機関検索詳細」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalSearchDetailViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 病院コードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CodeNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 医療機関カナ名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InstitutionKana As String = String.Empty

    ''' <summary>
    ''' 医療機関名称を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InstitutionName As String = String.Empty

    ''' <summary>
    ''' 郵便番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PostalCode As String = String.Empty

    ''' <summary>
    ''' 住所を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Address As String = String.Empty

    ''' <summary>
    ''' 電話番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tel As String = String.Empty

    ''' <summary>
    ''' 受け付け時間メモを取得または設定します。ACCEPTEDTIMEMEMO
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns> 
    ''' <remarks></remarks>
    Public Property AcceptedTimeMemo As String = String.Empty

    ''' <summary>
    ''' 休診メモを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns> 
    ''' <remarks></remarks>
    Public Property ClosedMemo As String = String.Empty

    ''' <summary>
    ''' URLを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Url As String = String.Empty

    ''' <summary>
    ''' 路線名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RouteName As String = String.Empty

    ''' <summary>
    ''' 最寄駅を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NeareStstation As String = String.Empty

    ''' <summary>
    ''' 交通手段を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Transportation As String = String.Empty

    ''' <summary>
    ''' 所要時間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequiredTime As Integer = Integer.MinValue

    ''' <summary>
    ''' 備考を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RouteRemarks As String = String.Empty

    ''' <summary>
    ''' 診療科のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DepartmentN As New List(Of String)()

    ''' <summary>
    ''' 緯度を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Latitude As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 経度を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Longitude As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 診療時間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicalOfficeHouersN As New List(Of List(Of MedicalOfficeHouers))()

    ''' <summary>
    ''' 各フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OptionFlags As Integer = Integer.MinValue

    ''' <summary>
    ''' 後払いリクエストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequestFlag As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalSearchdetailViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalSearchViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalSearch)

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="PortalSearchDetailViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="pageIndex">ページインデックス。</param>
    ''' <param name="pageCount">ページ数。</param>
    ''' <param name="MedicalInstitutionN">医療機関情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, searchText As String, pageIndex As Integer, pageCount As Integer, MedicalInstitutionN As IEnumerable(Of MedicalInstitutionItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalSearchDetail)

        'Me.CodeNo = searchText
        'Me.KanaName = pageIndex
        'Me.PageCount = pageCount
        'Me.MedicalInstitutionN = If(MedicalInstitutionN IsNot Nothing AndAlso MedicalInstitutionN.Any(), MedicalInstitutionN.ToList(), New List(Of MedicalInstitutionItem))

    End Sub

#End Region

End Class