Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Threading.Tasks

Friend NotInheritable Class PortalTargetSettingWorker2

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    Private Shared Function ToTargetValueList(inputModel As PortalTargetSettingInputModel2) As List(Of QhApiTargetValueItem)

        Dim result As New List(Of QhApiTargetValueItem)()

        With result
            ' 歩数
            .Add(
                New QhApiTargetValueItem() With {
                    .VitalType = QyVitalTypeEnum.Steps.ToString(),
                    .ValueType = QyStandardValueTypeEnum.None.ToString(),
                    .Lower = inputModel.TargetValue4,
                    .Upper = inputModel.TargetValue3
                }
            )

            ' 体重
            .Add(
                New QhApiTargetValueItem() With {
                    .VitalType = QyVitalTypeEnum.BodyWeight.ToString(),
                    .ValueType = QyStandardValueTypeEnum.None.ToString(),
                    .Lower = inputModel.TargetValue6,
                    .Upper = inputModel.TargetValue5
                }
            )

            ' 血圧（上）
            .Add(
                New QhApiTargetValueItem() With {
                    .VitalType = QyVitalTypeEnum.BloodPressure.ToString(),
                    .ValueType = QyStandardValueTypeEnum.BloodPressureUpper.ToString(),
                    .Lower = inputModel.TargetValue8,
                    .Upper = inputModel.TargetValue7
                }
            )

            ' 血圧（下）
            .Add(
                New QhApiTargetValueItem() With {
                    .VitalType = QyVitalTypeEnum.BloodPressure.ToString(),
                    .ValueType = QyStandardValueTypeEnum.BloodPressureLower.ToString(),
                    .Lower = inputModel.TargetValue10,
                    .Upper = inputModel.TargetValue9
                }
            )

            ' 血糖値（空腹時）
            .Add(
                New QhApiTargetValueItem() With {
                    .VitalType = QyVitalTypeEnum.BloodSugar.ToString(),
                    .ValueType = QyStandardValueTypeEnum.BloodSugarFasting.ToString(),
                    .Lower = inputModel.TargetValue12,
                    .Upper = inputModel.TargetValue11
                }
            )

            ' 血糖値（その他）
            .Add(
                New QhApiTargetValueItem() With {
                    .VitalType = QyVitalTypeEnum.BloodSugar.ToString(),
                    .ValueType = QyStandardValueTypeEnum.BloodSugarOther.ToString(),
                    .Lower = inputModel.TargetValue14,
                    .Upper = inputModel.TargetValue13
                }
            )
        End With

        Return result

    End Function

    Private Shared Function ExecutePortalTargetSettingReadApi(mainModel As QolmsYappliModel) As QhYappliPortalTargetSettingReadApiResults

        Dim apiArgs As New QhYappliPortalTargetSettingReadApiArgs(
            QhApiTypeEnum.YappliPortalTargetSettingRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.PortalTargetSetting).ToString(),
            .SexType = mainModel.AuthorAccount.SexType.ToString(),
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString()
        }
        Dim apiResults As QhYappliPortalTargetSettingReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTargetSettingReadApiResults)(
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

    Private Shared Function ExecutePortalTargetSettingWriteApi(mainModel As QolmsYappliModel, inputModel As PortalTargetSettingInputModel2) As QhYappliPortalTargetSettingWriteApiResults

        Dim apiArgs As New QhYappliPortalTargetSettingWriteApiArgs(
            QhApiTypeEnum.YappliPortalTargetSettingWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.PortalTargetSetting).ToString(),
            .SexType = mainModel.AuthorAccount.SexType.ToString(),
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString(),
            .TargetValueN = PortalTargetSettingWorker2.ToTargetValueList(inputModel),
            .CaloriesIn = inputModel.TargetValue1,
            .CaloriesOut = inputModel.TargetValue2,
            .Height = inputModel.Height.ToString(),
            .Weight = inputModel.Weight.ToString(),
            .PhysicalActivityLevel = inputModel.PhysicalActivityLevel.ToString(),
            .BasalMetabolism = inputModel.NowBasalMetabolism.ToString(),
            .TargetWeight = inputModel.TargetWeight,
            .TargetDate = inputModel.TargetDate.ToApiDateString()
        }
        Dim apiResults As QhYappliPortalTargetSettingWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTargetSettingWriteApiResults)(
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

    Public Shared Function CreateInputModel(mainModel As QolmsYappliModel) As PortalTargetSettingInputModel2

        Dim result As New PortalTargetSettingInputModel2(mainModel, String.Empty, Decimal.Zero.ToString(), 2, 0I, 0I, String.Empty, Date.MinValue, Nothing, Nothing)

        With PortalTargetSettingWorker2.ExecutePortalTargetSettingReadApi(mainModel)
            result = New PortalTargetSettingInputModel2(
                mainModel,
                .Height,
                .Weight,
                .PhysicalActivityLevel.TryToValueType(Convert.ToByte(2)),
                .CaloriesIn.TryToValueType(0I),
                .CaloriesOut.TryToValueType(0I),
                .TargetWeight,
                .TargetDate.TryToValueType(Date.MinValue),
                .StandardValueN,
                .TargetValueN
            )

            ' バイタル標準値
            If mainModel.AuthorAccount.StandardValues Is Nothing OrElse Not mainModel.AuthorAccount.StandardValues.Any() Then StandardValueWorker.SetStandardValue(mainModel, .StandardValueN)

            ' バイタル目標値
            If mainModel.AuthorAccount.TargetValues Is Nothing OrElse Not mainModel.AuthorAccount.TargetValues.Any() Then TargetValueWorker.SetTargetValue(mainModel, .TargetValueN)

            ' モデルをキャッシュへ格納
            mainModel.SetInputModelCache(result)
        End With

        Return result

    End Function

    Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As PortalTargetSettingInputModel2) As Boolean

        Dim actionDate As Date = Date.Now

        'カロミル目標値を登録
        NoteCalomealWorker.SetGoal(mainModel,inputModel.TargetDate,inputModel.TargetWeight.TryToValueType(Decimal.MinValue))
                
        'カロミルへ体重登録
        Dim decWeight As Decimal = Decimal.MinValue
        If Decimal.TryParse(inputModel.Weight,decWeight) AndAlso decWeight > Decimal.Zero
            NoteCalomealWorker.SetAnthropometric(mainModel,actionDate,decWeight)
        End If

        'カロミル目標カロリーを取得
        Dim goalList As List(Of CalomealGoalSet) = NoteCalomealWorker.GetMealWithBasis(mainModel,Date.Now,Date.Now)
        If goalList.Any() Then

            '取得できた場合
            Dim goal As CalomealGoalSet = goalList.OrderByDescending(Function(i) i.TargetDate).FirstOrDefault()
            inputModel.TargetValue1 = iif(goal.BasisAllCalorie > 0, goal.BasisAllCalorie, 1).ToString()
            
        Else
            '取得できなかった場合
            inputModel.TargetValue1 = inputModel.NowTargetCalorieIn.ToString()

        End If

        With PortalTargetSettingWorker2.ExecutePortalTargetSettingWriteApi(mainModel, inputModel)
            If .IsSuccess.TryToValueType(False) Then
                ' バイタル標準値を更新
                StandardValueWorker.SetStandardValue(mainModel, .StandardValueN)

                ' バイタル目標値を更新
                TargetValueWorker.SetTargetValue(mainModel, .TargetValueN)

                ' 「バイタル」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of NoteVitalViewModel)()

                ' 「歩数」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of NoteWalkViewModel)()

                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                If .CanGivePoint.TryToValueType(False) Then

                    'point
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

                End If

                'todo:ベイジアン再計算

                Return True
            Else
                Return False
            End If
        End With

    End Function

    Public Shared Function GetInputModelCache(mainModel As QolmsYappliModel) As PortalTargetSettingInputModel2

        ' キャッシュから取得
        Dim result As PortalTargetSettingInputModel2 = mainModel.GetInputModelCache(Of PortalTargetSettingInputModel2)()

        If result Is Nothing Then
            ' インプット モデルを生成しキャッシュへ追加
            result = PortalTargetSettingWorker2.CreateInputModel(mainModel)
            'mainModel.SetInputModelCache(PortalTargetSettingWorker2.CreateInputModel(mainModel))

            '' キャッシュから取得
            'result = mainModel.GetInputModelCache(Of PortalTargetSettingInputModel2)()
        End If

        Return result

    End Function
    
    Public Shared Function UpdateCalTarget(mainModel As QolmsYappliModel,cal As Integer) As Boolean
        'API変更なし状態での暫定処理

        Dim model As new PortalTargetSettingInputModel2()
        With PortalTargetSettingWorker2.ExecutePortalTargetSettingReadApi(mainModel)
                        
            model = New PortalTargetSettingInputModel2(
                mainModel,
                .Height,
                .Weight,
                .PhysicalActivityLevel.TryToValueType(Convert.ToByte(2)),
                .CaloriesIn.TryToValueType(0I),
                .CaloriesOut.TryToValueType(0I),
                .TargetWeight,
                .TargetDate.TryToValueType(Date.MinValue),
                .StandardValueN,
                .TargetValueN
            )

        End With

        'カロリーだけ更新
        model.TargetValue1 = cal.ToString()

        With PortalTargetSettingWorker2.ExecutePortalTargetSettingWriteApi(mainModel, model)

            '目標更新時に体重も同時に登録しているのでバイタルのポイント付与
            If .CanGivePoint.TryToValueType(False) Then

                'point
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

            End If

            Return .IsSuccess.TryToValueType(False)

        End With

    End Function
#End Region

End Class
