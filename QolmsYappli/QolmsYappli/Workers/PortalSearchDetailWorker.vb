Imports System.Threading
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsAzureStorageCoreV1
Imports MGF.QOLMS.QolmsCalomealApiCoreV1

''' <summary>
''' 「医療機関検索詳細」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalSearchDetailWorker

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
    ''' 「医療機関検索詳細」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <param name="CodeNo">医療機関コード。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalSearchDetailReadApi(mainModel As QolmsYappliModel, CodeNo As String) As QhYappliPortalSearchDetailReadApiResults

        Dim apiArgs As New QhYappliPortalSearchDetailReadApiArgs(
         QhApiTypeEnum.YappliPortalSearchDetailRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .CodeNo = CodeNo
         }

        Dim apiResults As QhYappliPortalSearchDetailReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalSearchDetailReadApiResults)(
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


    Private Shared Function ExecutePortalRequesWriteApi(mainModel As QolmsYappliModel, codeNo As String, requestFlag As Boolean) As QhYappliPortalSearchDetailWriteApiResults

        Dim result As Boolean = False

        Dim apiArgs As New QhYappliPortalSearchDetailWriteApiArgs(
            QhApiTypeEnum.YappliPortalSearchDetailWrite,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .Codeno = codeNo,
            .RequestFlag = requestFlag.ToString()
        }
        Dim apiResults As QhYappliPortalSearchDetailWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalSearchDetailWriteApiResults)(
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
    ''' 医療機関詳細を検索します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="CodeNo">医療機関コード。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, CodeNo As String) As PortalSearchDetailViewModel

        Dim result As New PortalSearchDetailViewModel()
        'API を実行
        With PortalSearchDetailWorker.ExecutePortalSearchDetailReadApi(mainModel, CodeNo)

            If .IsSuccess.TryToValueType(False) Then
                result.CodeNo = Integer.Parse(.CodeNo)
                result.InstitutionName = .InstitutionName
                result.InstitutionKana = .InstitutionKana
                result.PostalCode = .PostalCode
                result.Address = .Address
                result.Tel = .Tel
                result.AcceptedTimeMemo = .AcceptedTimeMemo
                result.ClosedMemo = .ClosedMemo
                result.Url = .Url
                result.RouteName = .RouteName
                result.NeareStstation = .NeareStstation
                result.Transportation = .Transportation.ToString()
                result.RequiredTime = Integer.Parse(.RequiredTime)
                result.RouteRemarks = .RouteRemarks
                result.Latitude = Decimal.Parse(.Latitude)
                result.Longitude = Decimal.Parse(.Longitude)
                result.DepartmentN = .DepartmentN
                result.MedicalOfficeHouersN = .MedicalOfficeHouersN.ConvertAll(Of List(Of MedicalOfficeHouers))(Function(i) i.ConvertAll(Of MedicalOfficeHouers)(Function(x) x.ToMedicalOfficeHouers()))
                result.OptionFlags = Integer.Parse(.OptionFlags)
                result.RequestFlag = Boolean.Parse(.RequestFlag)

            End If
        End With
        Return result
    End Function

    Public Shared Function PostPayRequest(mainModel As QolmsYappliModel, codeNo As String, requestFlag As Boolean) As PostpayRequestJsonResult

        If String.IsNullOrWhiteSpace(codeNo) Then Throw New ArgumentNullException("CodeNo", "CodeNoが空です。")

        ' APIを実行
        '後払いリクエストを編集
        Dim apiResult As QhYappliPortalSearchDetailWriteApiResults = PortalSearchDetailWorker.ExecutePortalRequesWriteApi(mainModel, codeNo, requestFlag)
        Return New PostpayRequestJsonResult() With {.IsSuccess = apiResult.IsSuccess.ToString(), .RequestFlag = apiResult.RequestFlag.ToString()}

    End Function
#End Region

End Class