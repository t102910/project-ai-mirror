Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class NoteWalkWorker

#Region "Constant"

    Private Const DEFAULT_DAY_COUNT As Integer = 30

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

    Private Shared Function ExecuteNoteWalkReadApi(mainModel As QolmsYappliModel) As QhYappliNoteWalkReadApiResults

        Dim apiArgs As New QhYappliNoteWalkReadApiArgs(
            QhApiTypeEnum.YappliNoteWalkRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.NoteWalk).ToString(),
            .IsInitialize = (mainModel.AuthorAccount.Height <= Decimal.Zero OrElse Not mainModel.AuthorAccount.StandardValues.Any()).ToString(),
            .SexType = mainModel.AuthorAccount.SexType.ToString(),
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString()
        }
        Dim apiResults As QhYappliNoteWalkReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteWalkReadApiResults)(
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

    Private Shared Function ExecuteNoteWalkGraphReadApi(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, dayCount As Integer) As QhYappliNoteWalkGraphReadApiResults

        Dim apiArgs As New QhYappliNoteWalkGraphReadApiArgs(
            QhApiTypeEnum.YappliNoteWalkGraphRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .VitalType = vitalType.ToString(),
            .DayCount = dayCount.ToString()
        }
        Dim apiResults As QhYappliNoteWalkGraphReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteWalkGraphReadApiResults)(
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

    Private Shared Function ExecuteNoteWalkWriteApi(mainModel As QolmsYappliModel, inputModelN As Dictionary(Of Date, VitalValueInputModel)) As QhYappliNoteWalkWriteApiResults

        Dim apiArgs As New QhYappliNoteWalkWriteApiArgs(
            QhApiTypeEnum.YappliNoteWalkWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .VitalValueN = inputModelN.ToList().ConvertAll(Function(i) i.Value.ToApiVitalValueItem(i.Key))
        }
        Dim apiResults As QhYappliNoteWalkWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteWalkWriteApiResults)(
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

    '<Obsolete("廃止予定")>
    'Private Shared Sub GetStandardValues(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, ByRef refValue1 As Decimal, ByRef refValue2 As Decimal, ByRef refValue3 As Decimal, ByRef refValue4 As Decimal)

    '    refValue1 = Decimal.MinusOne
    '    refValue2 = Decimal.MinusOne
    '    refValue3 = Decimal.MinusOne
    '    refValue4 = Decimal.MinusOne

    '    Select Case vitalType
    '        Case QyVitalTypeEnum.BloodPressure
    '            ' 血圧下
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureLower, refValue1, refValue2)

    '            ' 血圧上
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureUpper, refValue3, refValue4)

    '        Case QyVitalTypeEnum.BloodSugar
    '            ' 空腹時血糖値
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarFasting, refValue1, refValue2)

    '            ' その他血糖値
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarOther, refValue3, refValue4)

    '        Case QyVitalTypeEnum.BodyWeight
    '            ' 体重
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

    '            ' BMI
    '            refValue3 = 18.5D
    '            refValue4 = 24.9D

    '        Case QyVitalTypeEnum.Pulse,
    '            QyVitalTypeEnum.Glycohemoglobin,
    '            QyVitalTypeEnum.BodyWaist,
    '            QyVitalTypeEnum.BodyTemperature,
    '            QyVitalTypeEnum.Steps,
    '            QyVitalTypeEnum.BodyFatPercentage,
    '            QyVitalTypeEnum.MuscleMass,
    '            QyVitalTypeEnum.BoneMass,
    '            QyVitalTypeEnum.VisceralFat,
    '            QyVitalTypeEnum.BasalMetabolism,
    '            QyVitalTypeEnum.BodyAge,
    '            QyVitalTypeEnum.TotalBodyWater

    '            ' 脈拍、HbA1c、腹囲、体温、歩数、体脂肪率、筋肉量、推定骨量、内脂肪レベル、基礎代謝、体内年齢、水分率
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

    '    End Select

    'End Sub

    Private Shared Sub GetTargetValues(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, ByRef refValue1 As Decimal, ByRef refValue2 As Decimal, ByRef refValue3 As Decimal, ByRef refValue4 As Decimal)

        refValue1 = Decimal.MinusOne
        refValue2 = Decimal.MinusOne
        refValue3 = Decimal.MinusOne
        refValue4 = Decimal.MinusOne

        Select Case vitalType
            Case QyVitalTypeEnum.BloodPressure
                ' 血圧下
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureLower, refValue1, refValue2)

                ' 血圧上
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureUpper, refValue3, refValue4)

            Case QyVitalTypeEnum.BloodSugar
                ' 空腹時血糖値
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarFasting, refValue1, refValue2)

                ' その他血糖値
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarOther, refValue3, refValue4)

            Case QyVitalTypeEnum.BodyWeight
                ' 体重
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

                ' BMI
                refValue3 = 18.5D
                refValue4 = 24.9D

            Case QyVitalTypeEnum.Pulse,
                QyVitalTypeEnum.Glycohemoglobin,
                QyVitalTypeEnum.BodyWaist,
                QyVitalTypeEnum.BodyTemperature,
                QyVitalTypeEnum.Steps,
                QyVitalTypeEnum.BodyFatPercentage,
                QyVitalTypeEnum.MuscleMass,
                QyVitalTypeEnum.BoneMass,
                QyVitalTypeEnum.VisceralFat,
                QyVitalTypeEnum.BasalMetabolism,
                QyVitalTypeEnum.BodyAge,
                QyVitalTypeEnum.TotalBodyWater

                ' 脈拍、HbA1c、腹囲、体温、歩数、体脂肪率、筋肉量、推定骨量、内脂肪レベル、基礎代謝、体内年齢、水分率
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

        End Select

    End Sub

#End Region

#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, Optional targetDate As Nullable(Of Date) = Nothing) As NoteWalkViewModel

        Dim result As NoteWalkViewModel = Nothing ' キャッシュは使わない mainModel.GetInputModelCache(Of NoteWalkViewModel)() ' キャッシュから取得

        Dim recordDate As Date = Date.Now
        If targetDate.HasValue AndAlso targetDate.Value > Date.MinValue Then
            recordDate = New Date(targetDate.Value.Year, targetDate.Value.Month, targetDate.Value.Day, recordDate.Hour, recordDate.Minute, 0)
        End If


        If result Is Nothing Then
            ' キャッシュが無ければ API を実行
            With NoteWalkWorker.ExecuteNoteWalkReadApi(mainModel)
                ' TODO:
                result = New NoteWalkViewModel(mainModel)

                result.RecordDate = recordDate

                result.AvailableVitalN = .AvailableVitalN.ConvertAll(
                    Function(i)
                        Return New AvailableVitalItem() With {
                            .VitalType = i.VitalType.TryToValueType(QyVitalTypeEnum.None),
                            .LatestDate = i.LatestDate.TryToValueType(Date.MinValue)
                        }
                    End Function
                )

                ' バイタル標準値
                If mainModel.AuthorAccount.StandardValues Is Nothing OrElse Not mainModel.AuthorAccount.StandardValues.Any() Then StandardValueWorker.SetStandardValue(mainModel, .StandardValueN)

                ' バイタル目標値
                If mainModel.AuthorAccount.TargetValues Is Nothing OrElse Not mainModel.AuthorAccount.TargetValues.Any() Then TargetValueWorker.SetTargetValue(mainModel, .TargetValueN)

                ' 身長
                Dim height As Decimal = .Height.TryToValueType(Decimal.MinValue)

                If height > Decimal.Zero Then mainModel.AuthorAccount.Height = height

                'tanita歩数の連携があるか
                Dim tanita As List(Of DeviceItem) = PortalTanitaConnectionWorker.GetConnectedDevice(mainModel)
                result.TanitaWalkConnected = tanita.Where(Function(i) i.DevicePropertyName = "Pedometer").ToList().First().Checked

                ' キャッシュへ追加
                mainModel.SetInputModelCache(result)
            End With
        End If

        Return result

    End Function

    Public Shared Function CreateGraphPartialViewModel(mainModel As QolmsYappliModel, pageViewModel As NoteWalkViewModel, vitalType As QyVitalTypeEnum, ByRef refPartialViewName As String, Optional forceRead As Boolean = False) As QyVitalGraphPartialViewModelBase

        refPartialViewName = String.Empty

        Dim result As VitalStepsGraphPartialViewModel = Nothing

        ' 目標値
        Dim targetValues() As Decimal = {Decimal.Zero, Decimal.Zero, Decimal.Zero, Decimal.Zero}

        ' TODO: 検証中
        NoteWalkWorker.GetTargetValues(mainModel, vitalType, targetValues(0), targetValues(1), targetValues(2), targetValues(3))

        ' 歩数
        'If vitalType = QyVitalTypeEnum.Steps Then
        '    refPartialViewName = "_NoteVitalStepsGraphPartialView"

        '    Dim hasData As Boolean = False

        '    If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso hasData Then
        '        If Not pageViewModel.StepsPartialViewModel.ItemList.Any() Then
        '            With NoteWalkWorker.ExecuteNoteWalkGraphReadApi(mainModel, vitalType, NoteWalkWorker.DEFAULT_DAY_COUNT)
        '                result = New VitalStepsGraphPartialViewModel(
        '                    pageViewModel,
        '                    .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
        '                    targetValues(0),
        '                    targetValues(1)
        '                )
        '            End With

        '            pageViewModel.StepsPartialViewModel = result

        '            mainModel.SetInputModelCache(pageViewModel)

        '            Return result
        '        Else
        '            Return pageViewModel.StepsPartialViewModel
        '        End If
        '    Else
        '        Return New VitalStepsGraphPartialViewModel(
        '            pageViewModel,
        '            Nothing,
        '            Decimal.Zero,
        '            Decimal.Zero
        '        )
        '    End If
        'End If
        If vitalType = QyVitalTypeEnum.Steps Then
            refPartialViewName = "_NoteVitalStepsGraphPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then ' TODO: 暫定対応
                If Not pageViewModel.StepsPartialViewModel.ItemList.Any() Then
                    With NoteWalkWorker.ExecuteNoteWalkGraphReadApi(mainModel, vitalType, NoteWalkWorker.DEFAULT_DAY_COUNT)
                        result = New VitalStepsGraphPartialViewModel(
                            pageViewModel,
                            .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
                            targetValues(0),
                            targetValues(1)
                        )
                    End With

                    pageViewModel.StepsPartialViewModel = result

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.StepsPartialViewModel
                End If
            Else
                Return New VitalStepsGraphPartialViewModel(
                    pageViewModel,
                    Nothing,
                    Decimal.Zero,
                    Decimal.Zero
                )
            End If
        End If

        Throw New ArgumentOutOfRangeException("vitalType", "バイタル情報の種別が不正です。")

    End Function

    Public Shared Function CreateDetailPartialViewModel(mainModel As QolmsYappliModel, pageViewModel As NoteWalkViewModel, vitalType As QyVitalTypeEnum, recordDate As Date, ByRef refPartialViewName As String, Optional forceRead As Boolean = False) As QyVitalDetailPartialViewModelBase

        refPartialViewName = String.Empty

        Dim result As VitalStepsDetailPartialViewModel = Nothing

        If vitalType = QyVitalTypeEnum.Steps Then
            refPartialViewName = "_NoteVitalStepsDetailPartialView"

            Dim hasData As Boolean = False

            'If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso hasData Then result = New VitalStepsDetailPartialViewModel(pageViewModel, recordDate)
            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then result = New VitalStepsDetailPartialViewModel(pageViewModel, recordDate) ' TODO: 暫定対応
        End If

        Return result

    End Function

    Public Shared Function Delete(mainModel As QolmsYappliModel, pageViewModel As NoteWalkViewModel, vitalType As QyVitalTypeEnum, targetN As List(Of Date)) As NoteVitalDeleteJsonResult

        Dim result As New NoteVitalDeleteJsonResult() With {
            .VitalType = QyVitalTypeEnum.None.ToString(),
            .IsSuccess = Boolean.FalseString
        }

        If vitalType = QyVitalTypeEnum.Steps AndAlso targetN IsNot Nothing AndAlso targetN.Any() Then
            Dim inputModelN As New Dictionary(Of Date, VitalValueInputModel)()

            targetN.ForEach(
                Sub(i)
                    If i <> Date.MinValue AndAlso Not inputModelN.ContainsKey(i.Date) Then
                        inputModelN.Add(
                            i.Date,
                            New VitalValueInputModel() With {
                                .VitalType = QyVitalTypeEnum.Steps,
                                .Meridiem = "am",
                                .Hour = "0",
                                .Minute = "0",
                                .Value1 = "",
                                .Value2 = "",
                                .ConditionType = QyVitalConditionTypeEnum.None.ToString()
                            }
                        )
                    End If
                End Sub
            )

            If inputModelN.Any() Then
                With NoteWalkWorker.ExecuteNoteWalkWriteApi(mainModel, inputModelN)
                    For Each item As QhApiAvailableVitalItem In .AvailableVitalN
                        If item.VitalType.TryToValueType(QyVitalTypeEnum.None) = vitalType Then
                            Dim available As AvailableVitalItem = pageViewModel.AvailableVitalN.Find(Function(i) i.VitalType = vitalType)

                            If available IsNot Nothing Then available.LatestDate = item.LatestDate.TryToValueType(Date.MinValue)

                            Exit For
                        End If
                    Next

                    ' 歩数グラフ パーシャル ビュー モデルの初期化
                    pageViewModel.StepsPartialViewModel = New VitalStepsGraphPartialViewModel(
                        pageViewModel,
                        Nothing,
                        Decimal.Zero,
                        Decimal.Zero
                    )

                    ' キャッシュへ追加
                    mainModel.SetInputModelCache(pageViewModel)

                    result.VitalType = QyVitalTypeEnum.Steps.ToString()
                    result.IsSuccess = Boolean.TrueString

                    ' 「ホーム」画面のキャッシュをクリア
                    mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()
                End With
            End If
        End If

        Return result

    End Function

    Public Shared Function Edit(mainModel As QolmsYappliModel, steps As Integer, stepsDate As Date) As Boolean

        Dim recordDate As Date = stepsDate.Date

        Dim apiresult As QhYappliNoteWalkWriteApiResults = NoteWalkWorker.ExecuteNoteWalkWriteApi(
                mainModel, New Dictionary(Of Date, VitalValueInputModel)() From {
                    {
                        recordDate,
                        New VitalValueInputModel() With {
                            .VitalType = QyVitalTypeEnum.Steps,
                            .Value1 = steps.ToString(),
                            .Value2 = String.Empty,
                            .Meridiem = "am",
                            .Hour = "0",
                            .Minute = "0"
                        }
                    }
                }
            )

        ' ポイント付与
        Dim pointDic As New Dictionary(Of Tuple(Of Date, QyPointItemTypeEnum), Date)()
        Dim pointActionDate As Date = Date.Now
        Dim pointMaxDay As Date = pointActionDate.Date ' ポイント付与範囲終了日（今日）
        Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）
        Dim key As Date = recordDate
        Dim stepsPoint As Decimal = steps

        If (key >= pointMinDay And key <= pointMaxDay) AndAlso stepsPoint >= 5000D Then
            Dim limit As Date = New Date(key.Year, key.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は測定日）

            If steps >= 5000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk5k), limit)
            If steps >= 6000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk6k), limit)
            If steps >= 7000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk7k), limit)
            If steps >= 8000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk8k), limit)
            If steps >= 9000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk9k), limit)
            If steps >= 10000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk10k), limit)
        End If

        If pointDic.Any() Then
            ' 非同期で付与
            Task.Run(
                Sub()
                    QolmsPointWorker.AddQolmsPoints(
                        mainModel.ApiExecutor,
                        mainModel.ApiExecutorName,
                        mainModel.SessionId,
                        mainModel.ApiAuthorizeKey,
                        mainModel.AuthorAccount.AccountKey,
                        pointDic.ToList().ConvertAll(Of QolmsPointGrantItem)(
                            Function(i)
                                        Return New QolmsPointGrantItem(
                                            mainModel.AuthorAccount.MembershipType,
                                            pointActionDate,
                                            Guid.NewGuid().ToApiGuidString(),
                                            i.Key.Item2,
                                            i.Value,
                                            i.Key.Item1
                                        )
                                    End Function
                        )
                    )
                End Sub
            )
        End If
        Return True

    End Function

    Public Shared Function Edit(mainModel As QolmsYappliModel, pageViewModel As NoteWalkViewModel, inputModel As NoteVitalEditInputModel, ByRef refVitalTypeN As List(Of QyVitalTypeEnum), ByRef refMessageN As List(Of String)) As Boolean

        refVitalTypeN = New List(Of QyVitalTypeEnum)()
        refMessageN = New List(Of String)() From {"登録に失敗しました。"}

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプットモデルが Null 参照です。")

        ' モデルへ入力値をを反映
        pageViewModel.EditInputModel.UpdateByInput(inputModel)

        Dim result As Boolean = False

        If inputModel.HasValues AndAlso inputModel.IsAvailableVitalType(QyVitalTypeEnum.Steps) Then
            Dim pointDic As New Dictionary(Of Tuple(Of Date, QyPointItemTypeEnum), Date)()
            Dim pointActionDate As Date = Date.Now
            Dim pointMaxDay As Date = pointActionDate.Date ' ポイント付与範囲終了日（今日）
            Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）

            ' API を実行
            With NoteWalkWorker.ExecuteNoteWalkWriteApi(
                mainModel, New Dictionary(Of Date, VitalValueInputModel)() From {
                    {
                        inputModel.StepsDate,
                        inputModel.Steps
                    }
                }
            )

                pageViewModel.RecordDate = Date.Now.Date
                pageViewModel.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

                ' アカウント情報の会員の種別を更新
                mainModel.AuthorAccount.MembershipType = pageViewModel.MembershipType

                'For Each item As QhApiAvailableVitalItem In .AvailableVitalN
                '    If item.VitalType.TryToValueType(QyVitalTypeEnum.None) = QyVitalTypeEnum.Steps Then
                '        Dim available As AvailableVitalItem = pageViewModel.AvailableVitalN.Find(Function(i) i.VitalType = QyVitalTypeEnum.Steps)

                '        If available IsNot Nothing Then available.LatestDate = item.LatestDate.TryToValueType(Date.MinValue)

                '        Exit For
                '    End If
                'Next

                ' バイタル入力インプット モデルの初期化
                pageViewModel.EditInputModel = New NoteVitalEditInputModel(
                    pageViewModel.RecordDate,
                    pageViewModel.AvailableVitalN.Select(Function(i) i.VitalType).ToList(),
                    Decimal.MinusOne
                )

                ' 歩数グラフ パーシャル ビュー モデルの初期化
                pageViewModel.StepsPartialViewModel = New VitalStepsGraphPartialViewModel(
                    pageViewModel,
                    Nothing,
                    Decimal.Zero,
                    Decimal.Zero
                )

                ' キャッシュへ追加
                mainModel.SetInputModelCache(pageViewModel)

                refVitalTypeN.Add(QyVitalTypeEnum.Steps)
                refMessageN = New List(Of String)()
                result = True

                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                '' ポイント付与
                'Dim key As Date = inputModel.StepsDate.Date
                'Dim steps As Decimal = inputModel.Steps.Value1.TryToValueType(Decimal.Zero)

                'If (key >= pointMinDay And key <= pointMaxDay) AndAlso steps >= 5000D Then
                '    Dim limit As Date = New Date(key.Year, key.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は測定日）

                '    If steps >= 5000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk5k), limit)
                '    If steps >= 6000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk6k), limit)
                '    If steps >= 7000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk7k), limit)
                '    If steps >= 8000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk8k), limit)
                '    If steps >= 9000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk9k), limit)
                '    If steps >= 10000D Then pointDic.Add(New Tuple(Of Date, QyPointItemTypeEnum)(key, QyPointItemTypeEnum.Walk10k), limit)
                'End If

                'If pointDic.Any() Then
                '    ' 非同期で付与
                '    Task.Run(
                '        Sub()
                '            QolmsPointWorker.AddQolmsPoints(
                '                mainModel.ApiExecutor,
                '                mainModel.ApiExecutorName,
                '                mainModel.SessionId,
                '                mainModel.ApiAuthorizeKey,
                '                mainModel.AuthorAccount.AccountKey,
                '                pointDic.ToList().ConvertAll(Of QolmsPointGrantItem)(
                '                    Function(i)
                '                        Return New QolmsPointGrantItem(
                '                            mainModel.AuthorAccount.MembershipType,
                '                            pointActionDate,
                '                            Guid.NewGuid().ToApiGuidString(),
                '                            i.Key.Item2,
                '                            i.Value,
                '                            i.Key.Item1
                '                        )
                '                    End Function
                '                )
                '            )
                '        End Sub
                '    )
                'End If
            End With
        Else
            refMessageN = New List(Of String)() From {"値を入力してください。"}
        End If

        Return result

    End Function

    ' TODO: 検証中
    Public Shared Function GetViewModelCache(mainModel As QolmsYappliModel, Optional checkCache As Boolean = True) As NoteWalkViewModel

        ' キャッシュから取得
        Dim result As NoteWalkViewModel = mainModel.GetInputModelCache(Of NoteWalkViewModel)()
        Dim partialViewName As String = String.Empty

        If checkCache And result Is Nothing Then
            ' ページ ビュー モデルを生成しキャッシュへ追加
            result = NoteWalkWorker.CreateViewModel(mainModel)

            ' 歩数グラフ パーシャル ビュー モデルを生成しキャッシュへ追加
            NoteWalkWorker.CreateGraphPartialViewModel(mainModel, result, QyVitalTypeEnum.Steps, partialViewName)

            ' キャッシュから取得
            result = mainModel.GetInputModelCache(Of NoteWalkViewModel)()
        End If

        Return result

    End Function


    ' TODO: 検証中
    Public Shared Sub SynkAlkooSteps(mainModel As QolmsYappliModel, linkageId As String)

        '<add key="TargetStartDay" value="-31" />
        '<add key="TargetEndDay" value="-3" />
        '<add key="OnlyUpdateSteps" value="True" />
        Dim endDate As Date = Date.Now
        Dim startDate As Date = endDate.AddDays(-7) 'とりあえず一週間
        Dim onlyUpdateFlag As Boolean = True

        Dim result As Dictionary(Of Date, Decimal) = PortalAlkooConnectionWorker.GetBulkSteps(mainModel, linkageId, startDate, endDate, onlyUpdateFlag)

        Dim inputModelN As New Dictionary(Of Date, VitalValueInputModel)

        For Each item As KeyValuePair(Of Date, Decimal) In result

            Dim inp As New VitalValueInputModel() With {
                    .ConditionType = String.Empty,
                    .Hour = "0",
                    .Meridiem = "AM",
                    .Minute = "0",
                    .Value1 = item.Value.ToString(),
                    .Value2 = String.Empty,
                    .VitalType = QyVitalTypeEnum.Steps
                }
            inputModelN.Add(item.Key, inp)

        Next
        If inputModelN.Any() Then

            With NoteWalkWorker.ExecuteNoteWalkWriteApi(mainModel, inputModelN)
                If .IsSuccess.TryToValueType(False) Then
                    Dim type As String = .MembershipType
                    'ポイントつける？
                End If
            End With

        End If

    End Sub

#End Region

End Class
