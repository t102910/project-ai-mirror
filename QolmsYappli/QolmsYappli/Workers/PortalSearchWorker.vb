Imports System.Threading
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsAzureStorageCoreV1
Imports MGF.QOLMS.QolmsCalomealApiCoreV1

''' <summary>
''' 「医療機関検索」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalSearchWorker

#Region "Constant"

    ''' <summary>
    ''' ダミーのセッションIDを現します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミーのAPI認証キーを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 「医療機関検索」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalSearchReadApi(mainModel As QolmsYappliModel) As QhYappliPortalSearchReadApiResults

        Dim apiArgs As New QhYappliPortalSearchReadApiArgs(
         QhApiTypeEnum.YappliPortalSearchRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString
         }

        Dim apiResults As QhYappliPortalSearchReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalSearchReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' 「医療機関検索」画面検索 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalSearchFilterApi( _
                                                        mainModel As QolmsYappliModel, _
                                                         searchName As String, SearchDepartment As Integer, _
                                                         SearchCity As String, pageIndex As Integer, latitude As Decimal, _
                                                         longitude As Decimal, optionFlags As Integer, openFlag As Boolean) _
                                                     As QhYappliPortalSearchFilterApiResults

        Dim apiArgs As New QhYappliPortalSearchFilterApiArgs(
         QhApiTypeEnum.YappliPortalSearchFilter,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .SearchName = searchName,
             .PageSize = 5,
             .PageIndex = If(pageIndex > 0, pageIndex.ToString, "0"),
             .Latitude = latitude,
             .Longitude = longitude,
             .SearchDepartment = SearchDepartment,
             .SearchCity = SearchCity,
             .OptionFlags = optionFlags,
             .OpenFlag = openFlag
         }

        Dim apiResults As QhYappliPortalSearchFilterApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalSearchFilterApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"
    ''' <summary>
    ''' 医療機関検索画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalSearchViewModel

        Dim result As New PortalSearchViewModel()
        ' API を実行
        With PortalSearchWorker.ExecutePortalSearchReadApi(mainModel)
            result.DepartmentN = .DepartmentN.ConvertAll(Function(i) i.ToSearchMstItem())
            result.AreaN = .AreaN.ConvertAll(Function(i) i.ToSearchMstItem())
            result.CityN = .CityN.ConvertAll(Function(i) i.convertall(Function(x) x.ToSearchMstItem()))
            Return result

        End With

    End Function

    ''' <summary>
    ''' 医療機関を検索します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Search(mainModel As QolmsYappliModel, _
                                  searchName As String, SearchDepartment As Integer, _
                                  SearchCity As String, pageIndex As Integer, _
                                  latitude As Decimal, longitude As Decimal, _
                                  optionFlags As Integer, openFlag As Boolean, _
                                  ByRef resultPageIndex As Integer, ByRef resultPageCount As Integer, _
                                  ByRef medicalN As List(Of MedicalInstitutionItem)) _
                              As Boolean

        If String.IsNullOrWhiteSpace(searchName) Then
            searchName = String.Empty
        End If
        'Throw New ArgumentNullException("searchName", "検索文字列が空です。")
        If pageIndex = Integer.MinValue Then Throw New ArgumentNullException("pageIndex", "ページ指定が不正です。")

        ' API を実行
        With PortalSearchWorker.ExecutePortalSearchFilterApi(mainModel, searchName, SearchDepartment, SearchCity, pageIndex, latitude, longitude, optionFlags, openFlag)
            If .IsSuccess.TryToValueType(False) Then

                resultPageIndex = Integer.Parse(.PageIndex)
                resultPageCount = Integer.Parse(.MaxPageIndex)
                medicalN = .FacilityN.ConvertAll(Function(i) i.ToMedicalInstitutionItem())

                Return True

            End If
        End With

        Return True

    End Function

#End Region

End Class
