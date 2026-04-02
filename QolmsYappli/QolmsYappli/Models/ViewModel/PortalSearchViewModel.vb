''' <summary>
''' 「医療機関検索」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalSearchViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 診療科マスタのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DepartmentN As New List(Of SearchMstItem)()

    ''' <summary>
    ''' 地域マスタのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AreaN As New List(Of SearchMstItem)()

    ''' <summary>
    ''' 市区町村マスタのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CityN As New List(Of List(Of SearchMstItem))()

    ''' <summary>
    ''' 名称検索文字列を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchName As String = String.Empty

    ''' <summary>
    ''' ページインデックスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PageIndex As Integer = Integer.MinValue

    ''' <summary>
    ''' ページ数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PageCount As Integer = Integer.MinValue

    ''' <summary>
    ''' 医療機関情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicalInstitutionN As New List(Of MedicalInstitutionItem)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalSearchViewModel" /> クラスの新しいインスタンスを初期化します。
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

        MyBase.New(mainModel, QyPageNoTypeEnum.Portalsearch)

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="PortalSearchViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="pageIndex">ページインデックス。</param>
    ''' <param name="pageCount">ページ数。</param>
    ''' <param name="MedicalInstitutionN">医療機関情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, searchText As String, pageIndex As Integer, pageCount As Integer, MedicalInstitutionN As IEnumerable(Of MedicalInstitutionItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalSearch)

        Me.SearchName = searchText
        Me.PageIndex = pageIndex
        Me.PageCount = pageCount
        Me.MedicalInstitutionN = If(MedicalInstitutionN IsNot Nothing AndAlso MedicalInstitutionN.Any(), MedicalInstitutionN.ToList(), New List(Of MedicalInstitutionItem))

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="PortalSearchViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="departmentN">診療科マスタのリスト。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, departmentN As List(Of SearchMstItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalSearch)

        Me.DepartmentN = departmentN

    End Sub

#End Region

End Class