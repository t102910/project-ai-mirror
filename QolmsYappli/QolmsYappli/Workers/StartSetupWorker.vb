Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class StartSetupWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    Private Shared Function ExecuteStartSetUpWriteApi(mainModel As QolmsYappliModel, inputModel As StartSetupInputModel, recordDate As Date) As QhYappliStartSetUpWriteApiResults

        Dim now As Date = Date.Now

        Dim apiArgs As New QhYappliStartSetUpWriteApiArgs(
            QhApiTypeEnum.YappliStartSetUpWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .SexType = inputModel.SexType.ToString(),
            .Birthday = inputModel.BirthdayDate.ToApiDateString(),
            .VitalValueN = New List(Of QhApiVitalValueItem)() From {
                New QhApiVitalValueItem() With {
                    .RecordDate = recordDate.ToApiDateString(),
                    .VitalType = QyVitalTypeEnum.BodyWeight.ToString(),
                    .Value1 = inputModel.Weight.ToString(),
                    .Value2 = inputModel.Height.ToString(),
                    .Value3 = Decimal.MinusOne.ToString(),
                    .Value4 = Decimal.MinusOne.ToString(),
                    .ConditionType = QyVitalConditionTypeEnum.None.ToString()
                }
            },
            .BasalMetabolism = inputModel.NowBasalMetabolism.ToString(),
            .PhysicalActivityLevel = inputModel.PhysicalActivityLevel.ToString(),
            .TargetCalorieIn = inputModel.CaloriesIn.ToString(),
            .TargetCalorieOut = inputModel.CaloriesOut.ToString()
        }
        Dim apiResults As QhYappliStartSetUpWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliStartSetUpWriteApiResults)(
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

    <Obsolete()>
    Public Shared Function CreateInputModel(mainModel As QolmsYappliModel) As StartSetupInputModel

        ' TODO:
        Return New StartSetupInputModel(mainModel)

    End Function

    <Obsolete()>
    Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As StartSetupInputModel) As Boolean

        Dim pointActionDate As Date = Date.Now
        Dim pointTargetDate As Date = New Date(
            pointActionDate.Year,
            pointActionDate.Month,
            pointActionDate.Day,
            pointActionDate.Hour,
            pointActionDate.Minute,
            0
        )
        Dim pointLimitDate As Date = New Date(
            pointTargetDate.Year,
            pointTargetDate.Month,
            1
        ).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は測定日時）

        ' API を実行
        With StartSetupWorker.ExecuteStartSetUpWriteApi(mainModel, inputModel, pointTargetDate)
            ' アカウント情報を更新（性別、生年月日、会員の種別）
            Dim sexType As QySexTypeEnum = inputModel.SexType
            Dim birthday As Date = inputModel.BirthdayDate

            If (sexType = QySexTypeEnum.Male OrElse sexType = QySexTypeEnum.Female) And mainModel.AuthorAccount.SexType <> sexType Then mainModel.AuthorAccount.SexType = sexType
            If birthday <> Date.MinValue And mainModel.AuthorAccount.Birthday <> birthday Then mainModel.AuthorAccount.Birthday = birthday

            mainModel.AuthorAccount.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

            ' 「健康年齢」画面のキャッシュをクリア
            mainModel.RemoveInputModelCache(Of HealthAgeViewModel)()

            ' 「バイタル」画面のキャッシュをクリア
            mainModel.RemoveInputModelCache(Of NoteVitalViewModel)()

            ' 「歩数」画面のキャッシュをクリア
            mainModel.RemoveInputModelCache(Of NoteWalkViewModel)()

            ' 「ホーム」画面のキャッシュをクリア
            mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

            ' 非同期でポイント付与
            Task.Run(
                Sub()
                    QolmsPointWorker.AddQolmsPoints(
                        mainModel.ApiExecutor,
                        mainModel.ApiExecutorName,
                        mainModel.SessionId,
                        mainModel.ApiAuthorizeKey,
                        mainModel.AuthorAccount.AccountKey,
                        {
                            New QolmsPointGrantItem(
                                mainModel.AuthorAccount.MembershipType,
                                pointActionDate,
                                Guid.NewGuid().ToApiGuidString(),
                                QyPointItemTypeEnum.Vital,
                                pointLimitDate,
                                pointTargetDate
                            )
                        }.ToList()
                    )
                End Sub
            )

            Return True
        End With

    End Function

#End Region

End Class
