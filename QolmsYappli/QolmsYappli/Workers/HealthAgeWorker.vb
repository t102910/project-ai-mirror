Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「健康年齢」画面に関する機能を提供します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class HealthAgeWorker

#Region "Constant"

    Private Const DEFAULT_DATA_COUNT As Integer = 3

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    Private Shared Function ExecuteHealthAgeReadApi(mainModel As QolmsYappliModel) As QhYappliHealthAgeReadApiResults

        Dim apiArgs As New QhYappliHealthAgeReadApiArgs(
            QhApiTypeEnum.YappliHealthAgeRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.HealthAge).ToString(),
            .IsInitialize = Boolean.TrueString,
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString()
        }
        Dim apiResults As QhYappliHealthAgeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliHealthAgeReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecuteHealthAgeReportRead(mainModel As QolmsYappliModel, healthAgeReportType As QyHealthAgeReportTypeEnum, dataCount As Integer) As QhYappliHealthAgeReportReadApiResults

        Dim apiArgs As New QhYappliHealthAgeReportReadApiArgs(
            QhApiTypeEnum.YappliHealthAgeReportRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .HealthAgeReportType = healthAgeReportType.ToString(),
            .DataCount = dataCount.ToString()
        }
        Dim apiResults As QhYappliHealthAgeReportReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliHealthAgeReportReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 「健康年齢」画面 ビュー モデル を作成します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fromPageNoType">遷移元の画面番号の種別。</param>
    ''' <returns>
    ''' 「健康年齢」画面 ビュー モデル。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, fromPageNoType As QyPageNoTypeEnum) As HealthAgeViewModel

        Dim result As HealthAgeViewModel = mainModel.GetInputModelCache(Of HealthAgeViewModel)() ' キャッシュから取得

        If result Is Nothing OrElse result.MembershipType <> mainModel.AuthorAccount.MembershipType OrElse result.FromPageNoType <> fromPageNoType Then
            ' キャッシュが無ければ API を実行
            With HealthAgeWorker.ExecuteHealthAgeReadApi(mainModel)
                result = New HealthAgeViewModel(mainModel, fromPageNoType)

                ' TODO:
                result.AvailableHealthAgeReportN = .AvailableHealthAgeReportN.ConvertAll(
                    Function(i)
                        Return New AvailableHealthAgeReportItem() With {
                            .HealthAgeReportType = i.HealthAgeReportType.TryToValueType(QyHealthAgeReportTypeEnum.None),
                            .LatestDate = i.LatestDate.TryToValueType(Date.MinValue)
                        }
                    End Function
                )

                ' 会員の種別
                result.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

                ' アカウント情報の会員の種別を更新
                mainModel.AuthorAccount.MembershipType = result.MembershipType
                'result.MembershipType = QyMemberShipTypeEnum.Premium ' TODO: debug

                ' 病院連携中かの フラグ
                result.HasHospitalConnection = .LinkegeSystemNoN.Any()

                ' 健康年齢
                result.HealthAge = .HealthAge.TryToValueType(Decimal.MinValue)

                ' 実年齢との差
                result.HealthAgeDistance = .HealthAgeDistance.TryToValueType(Decimal.MinValue)

                ' 測定日
                result.LatestDate = .LatestDate.TryToValueType(Date.MinValue)

                ' 予測医療費
                result.MedicalExpenses = .MedicalExpenses.TryToValueType(Integer.MinValue)

                ' 健康年齢の推移のリスト
                result.HealthAgeN = .HealthAgeN.ConvertAll(Function(i) i.ToHealthAgeValueItem())

                ' 健康年齢改善アドバイス情報のリスト
                result.HealthAgeAdviceN = .HealthAgeAdviceN.ConvertAll(Function(i) i.ToHealthAgeAdviceItem())

                ' キャッシュへ追加
                mainModel.SetInputModelCache(result)
            End With
        End If

        Return result

    End Function

    Public Shared Function CreateReportPartialViewModel(mainModel As QolmsYappliModel, pageViewModel As HealthAgeViewModel, healthAgeReportType As QyHealthAgeReportTypeEnum, ByRef refPartialViewName As String) As QyHealthAgeReportPartialViewModelBase

        refPartialViewName = String.Empty

        Dim result As QyHealthAgeReportPartialViewModelBase = Nothing

        ' 年齢分布
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Distribution Then
            refPartialViewName = "_HealthAgeDistributionReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.DistributionPartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Distribution, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgeDistributionReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.DistributionPartialViewModel = DirectCast(result, HealthAgeDistributionReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.DistributionPartialViewModel
                End If
            Else
                Return New HealthAgeDistributionReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        ' 肥満
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Fat Then
            refPartialViewName = "_HealthAgeFatReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.FatPartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Fat, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgeFatReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.FatPartialViewModel = DirectCast(result, HealthAgeFatReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.FatPartialViewModel
                End If
            Else
                Return New HealthAgeFatReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        ' 血糖
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Glucose Then
            refPartialViewName = "_HealthAgeGlucoseReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.GlucosePartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Glucose, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgeGlucoseReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.GlucosePartialViewModel = DirectCast(result, HealthAgeGlucoseReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.GlucosePartialViewModel
                End If
            Else
                Return New HealthAgeGlucoseReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        ' 血圧
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Pressure Then
            refPartialViewName = "_HealthAgePressureReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.PressurePartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Pressure, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgePressureReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.PressurePartialViewModel = DirectCast(result, HealthAgePressureReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.PressurePartialViewModel
                End If
            Else
                Return New HealthAgePressureReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        ' 脂質
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Lipid Then
            refPartialViewName = "_HealthAgeLipidReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.LipidPartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Lipid, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgeLipidReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.LipidPartialViewModel = DirectCast(result, HealthAgeLipidReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.LipidPartialViewModel
                End If
            Else
                Return New HealthAgeLipidReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        ' 肝臓
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Liver Then
            refPartialViewName = "_HealthAgeLiverReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.LiverPartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Liver, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgeLiverReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.LiverPartialViewModel = DirectCast(result, HealthAgeLiverReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.LiverPartialViewModel
                End If
            Else
                Return New HealthAgeLiverReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        ' 尿糖・尿蛋白
        If healthAgeReportType = QyHealthAgeReportTypeEnum.Urine Then
            refPartialViewName = "_HealthAgeUrineReportPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, hasData) AndAlso hasData Then
                If Not pageViewModel.UrinePartialViewModel.ReportItem.HealthAgeValueN.Any() Then
                    With HealthAgeWorker.ExecuteHealthAgeReportRead(mainModel, QyHealthAgeReportTypeEnum.Urine, HealthAgeWorker.DEFAULT_DATA_COUNT)
                        result = New HealthAgeUrineReportPartialViewModel(
                            pageViewModel,
                            .HealthAgeReport.ToHealthAgeReportItem()
                        ) ' TODO:
                    End With

                    pageViewModel.UrinePartialViewModel = DirectCast(result, HealthAgeUrineReportPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.UrinePartialViewModel
                End If
            Else
                Return New HealthAgeUrineReportPartialViewModel(pageViewModel, Nothing)
            End If
        End If

        Return result

    End Function

#End Region

End Class
