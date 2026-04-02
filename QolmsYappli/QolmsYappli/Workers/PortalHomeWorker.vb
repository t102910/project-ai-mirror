Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1

Friend NotInheritable Class PortalHomeWorker

#Region "Constant"

    Private Const DEFAULT_DAY_COUNT As Integer = 30

    Private Shared ReadOnly pointMessages As New Dictionary(Of String, String)() From {
        {"1", "初回プレミアム会員登録、{0}ポイント貯まりました。"},
        {"2", "ログインポイント、{0}ポイント貯まりました。"},
        {"3", "歩数5000歩到達、{0}ポイント貯まりました。"},
        {"4", "歩数6000歩到達、{0}ポイント貯まりました。"},
        {"5", "歩数7000歩到達、{0}ポイント貯まりました。"},
        {"6", "歩数8000歩到達、{0}ポイント貯まりました。"},
        {"7", "歩数9000歩到達、{0}ポイント貯まりました。"},
        {"8", "歩数10000歩到達、{0}ポイント貯まりました。"},
        {"9", "運動の登録、{0}ポイント貯まりました。"},
        {"10", "朝食の登録、{0}ポイント貯まりました。"},
        {"11", "昼食の登録、{0}ポイント貯まりました。"},
        {"12", "夕食の登録、{0}ポイント貯まりました。"},
        {"13", "間食の登録、{0}ポイント貯まりました。"},
        {"14", "バイタル登録、{0}ポイント貯まりました。"},
        {"15", "健康年齢測定、{0}ポイント貯まりました。"},
        {"18", "初回タニタヘルスプラネット連携登録、{0}ポイント貯まりました。"}
    }

    Public Shared ReadOnly Property ExaminationLinkageList As New Lazy(Of List(Of Integer))(Function() GetSetting())

    'Private Shared ReadOnly ExaminationLinkageList As New List(Of Integer)() From {
    '    {47008},
    '    {47106},
    '    {47100}
    '}

    Private Const TANITA_SITE_ID As String = "830"
    '伊平野村の場合
    '暗号化
    Private Shared ReadOnly IHEYA_CATEGORYKEY As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieCategoryKeyIheya")
    '竹富町の場合
    '暗号化
    Private Shared ReadOnly TAKETOMI_CATEGORYKEY As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieCategoryKeyTaketomi")

    '暗号化
    Private Shared ReadOnly TANITA_SITE_KEY As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieSiteKey")
    ' タニタ動画リンクUri
    Private Shared ReadOnly TANITA_MOVIE_URI As String = ConfigurationManager.AppSettings("ChallengeTanitaMovieUri")

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


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting() As List(Of Integer)

        Dim result As New List(Of Integer)()
        Dim value As String = ConfigurationManager.AppSettings("ExaminationLinkageList")

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            Dim arr As String() = value.Split(","c)
            'If arr.Any() Then

            For Each item As String In arr
                Dim outint As Integer = Integer.MinValue

                If Integer.TryParse(item, outint) Then
                    result.Add(outint)
                End If

            Next
            'End If

        End If

        Return result

    End Function

    Private Shared Function ExecutePortalHomeReadApi(mainModel As QolmsYappliModel, isInitialize As Boolean) As QjPortalHomeReadApiResults

        AccessLogWorker.WriteDebugLog(Nothing, String.Empty, String.Format("ExecutePortalHomeReadApi:ApiExecutor={0},ApiExecutorName={1},ActorKey={2},IsInitialize={3},SexType={4},Birthday={5},SessionId={6},ApiAuthorizeKey={7}" _
                                                                           , mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.AuthorAccount.AccountKey.ToApiGuidString() _
                                                                           , isInitialize.ToString, mainModel.AuthorAccount.SexType.ToString(), mainModel.AuthorAccount.Birthday.ToApiDateString() _
                                                                           , mainModel.SessionId, mainModel.ApiAuthorizeKey))

        Dim apiArgs As New QjPortalHomeReadApiArgs(
            QjApiTypeEnum.PortalHomeRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.PortalHome).ToString(),
            .IsInitialize = isInitialize.ToString,
            .SexType = mainModel.AuthorAccount.SexType.ToString(),
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString()
        }
        Dim apiResults As QjPortalHomeReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHomeReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecutePortalHomeDetailReadApi(mainModel As QolmsYappliModel, isInitialize As Boolean, mode As String, showDay As Date) As QjPortalHomeDetailReadApiResults

        Dim apiArgs As New QjPortalHomeDetailReadApiArgs(
            QjApiTypeEnum.PortalHomeDetailRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.PortalHome).ToString(),
            .IsInitialize = isInitialize.ToString,
            .SexType = mainModel.AuthorAccount.SexType.ToString(),
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString(),
            .Mode = mode,
            .ShowDay = showDay.ToApiDateString
        }
        Dim apiResults As QjPortalHomeDetailReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHomeDetailReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecutePortalHomePointReadApi(mainModel As QolmsYappliModel) As Integer

        Dim apiArgs As New QoQolmsPointReadApiArgs(QoApiTypeEnum.QolmsPointRead, mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name)

        Dim apiResults As QoQolmsPointReadApiResults = QsApiManager.ExecuteQolmsOpenApi(Of QoQolmsPointReadApiResults)(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey
                )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Dim point As Integer = Integer.MinValue
                If Integer.TryParse(.Point, point) Then
                    Return point
                Else
                    Return point
                End If
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ExecutePortalHomeMedicalLinkageReadApi(mainModel As QolmsYappliModel) As QjPortalHomeMedicalLinkageReadApiResults

        Dim apiArgs As New QjPortalHomeMedicalLinkageReadApiArgs(
            QjApiTypeEnum.PortalHomeMedicalLinkageRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNoList = PortalHomeWorker.ExaminationLinkageList.Value.ConvertAll(Function(i) i.ToString())
        }
        Dim apiResults As QjPortalHomeMedicalLinkageReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHomeMedicalLinkageReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ExecutePortalHomeTaskReadApi(mainModel As QolmsYappliModel, targetDate As Date, challengeKey As Guid, day As Integer) As QjPortalHomeTaskReadApiResults

        Dim apiArgs As New QjPortalHomeTaskReadApiArgs(
                    QjApiTypeEnum.PortalHomeTaskRead,
                    QsApiSystemTypeEnum.QolmsJoto,
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName
                ) With {
                    .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                    .TargetDate = targetDate.ToApiDateString(),
                    .ChallengeKey = challengeKey.ToString(),
                    .ChallengeDay = day.ToString()
            }
        Dim apiResults As QjPortalHomeTaskReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHomeTaskReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecutePortalHomeIsExaminationShowReadApi(mainModel As QolmsYappliModel) As Boolean

        Dim apiArgs As New QjPortalHomeIsExaminationShowReadApiArgs(
                    QjApiTypeEnum.PortalHomeIsExaminationShowRead,
                    QsApiSystemTypeEnum.QolmsJoto,
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName
                ) With {
                    .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            }
        Dim apiResults As QjPortalHomeIsExaminationShowReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHomeIsExaminationShowReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return .ExaminationFlag.TryToValueType(False)
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function GetPointNews(mainModel As QolmsYappliModel) As List(Of String)

        Dim result As New List(Of String)()
        Dim today As Date = Date.Today
        Dim news As New List(Of QoApiQolmsPointHistoryResultItem)()

        Try
            news = QolmsPointWorker.GetTargetPointFromHistoryList(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                QyPointItemTypeEnum.None,
                today.AddDays(-1),
                Date.MaxValue
            ).OrderByDescending(
                Function(x)
                    Return x.ActionDate.TryToValueType(Date.MinValue)
                End Function
            ).ThenByDescending(
                Function(x)
                    Return x.PointItemNo.TryToValueType(Integer.MinValue)
                End Function
            ).ToList()
        Catch ex As Exception
        End Try

        'If news Is Nothing Then
        '    news = New List(Of QoApiQolmsPointHistoryResultItem)()
        'End If

        'Dim pointcountMax As Integer
        'If news.Count > 3 Then
        '    pointcountMax = 2
        'Else
        '    pointcountMax = news.Count - 1
        'End If

        'For i As Integer = 0 To pointcountMax

        '    Select Case news(i).PointItemNo
        '        Case "1"
        '            result.Add(String.Format("新規プレミアム会員登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "2"
        '            result.Add(String.Format("ログインポイント、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "3"
        '            result.Add(String.Format("歩数5000歩到達、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "4"
        '            result.Add(String.Format("歩数6000歩到達、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "5"
        '            result.Add(String.Format("歩数7000歩到達、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "6"
        '            result.Add(String.Format("歩数8000歩到達、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "7"
        '            result.Add(String.Format("歩数9000歩到達、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "8"
        '            result.Add(String.Format("歩数10000歩到達、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "9"
        '            result.Add(String.Format("運動の登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "10"
        '            result.Add(String.Format("朝食の登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "11"
        '            result.Add(String.Format("昼食の登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "12"
        '            result.Add(String.Format("夕食の登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "13"
        '            result.Add(String.Format("間食の登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "14"
        '            result.Add(String.Format("バイタルデータ登録、{0}ポイント貯まりました。", news(i).PointValue))
        '        Case "15"
        '            result.Add(String.Format("健診データ登録、{0}ポイント貯まりました。", news(i).PointValue))
        '    End Select
        'Next

        If news IsNot Nothing AndAlso news.Any() Then
            If news.First().ActionDate.TryToValueType(Date.MinValue).Date >= today Then
                ' 今日の履歴あり
                news = news.Where(
                    Function(x)
                        Return x.ActionDate.TryToValueType(Date.MinValue) >= today
                    End Function
                ).ToList()
            End If

            For Each item As QoApiQolmsPointHistoryResultItem In news
                If PortalHomeWorker.pointMessages.ContainsKey(item.PointItemNo) Then
                    result.Add(String.Format(PortalHomeWorker.pointMessages(item.PointItemNo), item.PointValue))
                End If
            Next
        End If

        'If 0 <= total AndAlso total < 600 Then
        '    result.Add(String.Format("あと{0}ポイントで、銀ランクになります。", 600 - total))
        'ElseIf 600 <= total AndAlso total < 800 Then
        '    result.Add(String.Format("あと{0}ポイントで、金ランクになります。", 800 - total))
        'End If

        Return result

    End Function

    'Private Shared Function ExecutePortalHomeVitalWriteApi(mainModel As QolmsYappliModel, isInitialize As Boolean, mode As String, showDay As Date) As QhYappliPortalHomeReadApiResults

    '    Dim apiArgs As New QhYappliPortalHomeReadApiArgs(
    '        QhApiTypeEnum.YappliPortalHomeRead,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
    '        .PageNo = Convert.ToByte(QyPageNoTypeEnum.PortalHome).ToString(),
    '        .IsInitialize = isInitialize.ToString,
    '        .SexType = mainModel.AuthorAccount.SexType.ToString(),
    '        .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString(),
    '        .Mode = mode,
    '        .ShowDay = showDay.ToApiDateString
    '    }
    '    Dim apiResults As QhYappliPortalHomeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalHomeReadApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then
    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function


    'Private Shared Function ExecutePortalHomeGraphReadApi(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, dayCount As Integer) As QhYappliPortalHomeGraphReadApiResults

    '    Dim apiArgs As New QhYappliPortalHomeGraphReadApiArgs(
    '        QhApiTypeEnum.YappliPortalHomeGraphRead,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
    '        .VitalType = vitalType.ToString(),
    '        .DayCount = dayCount.ToString()
    '    }
    '    Dim apiResults As QhYappliPortalHomeGraphReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalHomeGraphReadApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then
    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function

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

#End Region

#Region "Public Method"


    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, Optional enableYappliSdk As Boolean = False) As PortalHomeViewModel

        'カロミルの食事履歴同期を実行
        Task.Run(
                Sub()
                    NoteCalomealWorker.History(mainModel)
                End Sub
            )

        Dim result As New PortalHomeViewModel(mainModel) '= mainModel.GetInputModelCache(Of PortalHomeViewModel)() ' キャッシュから取得

        With PortalHomeWorker.ExecutePortalHomeReadApi(mainModel, True)

            result.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

            ' アカウント情報の会員の種別を更新
            mainModel.AuthorAccount.MembershipType = result.MembershipType

            result.SetUpFlag = .SetupFlag.TryToValueType(False)

            Dim now As Date = Date.Now
            If .Challengekey.TryToValueType(Guid.Empty) <> Guid.Empty AndAlso .Challengekey.TryToValueType(Guid.Empty) = Guid.Parse("DED05070-8718-4313-924A-25233E35E218") Then

                result.ChallengeAreaPartialViewModel.Challengekey = .Challengekey.TryToValueType(Guid.Empty)
                result.ChallengeAreaPartialViewModel.ChallengeAllDays = .ChallengeAllDays.TryToValueType(Integer.MinValue)
                result.ChallengeAreaPartialViewModel.ChallengeStartDate = .ChallengeStartDate.TryToValueType(Date.MinValue)
                result.ChallengeAreaPartialViewModel.ChallengeTargetDay = (now - result.ChallengeAreaPartialViewModel.ChallengeStartDate).Days + 1
                result.ChallengeAreaPartialViewModel.ChallengeTargetWeightLoss = .ChallegeTargetWeightLoss.TryToValueType(Decimal.MinValue)

            End If

            result.ChallengeList = .ChallengeList.ConvertAll(Function(i) i.TryToValueType(Guid.Empty))
            
            'カロミルAIフィードバックの通知表示
            result.IsCalomealAiFeedback = .IsCalomealAiFeedback.TryToValueType(False)

        End With

        'タニタ接続
        result.TanitaConnectedFlag = PortalTanitaConnectionWorker.GetConnected(mainModel)


        'ALKOO接続
        Dim id As String = String.Empty
        result.AlkooConnectedFlag = PortalAlkooConnectionWorker.GetConnected(mainModel, id)
        result.AlkooJotoId = id

        result.LinkageList = PortalHomeWorker.GetMedicalLinkageList(mainModel).Select(Function(i) i.LinkageSystemNo.TryToValueType(Integer.MinValue)).ToList()

        result.UrlList.Add("NavitimeEventLandingUrl", ConfigurationManager.AppSettings("NavitimeEventLandingUrl"))
        result.UrlList.Add("NavitimeEntryUrl", ConfigurationManager.AppSettings("NavitimeEntryUrl"))
        result.UrlList.Add("NavitimeEventUrl", ConfigurationManager.AppSettings("NavitimeEventUrl"))

        Dim ginowanlinkage As LinkageItem = LinkageWorker.GetLinkageItem(mainModel, 47900022)

        result.GinowanStatusType = If(ginowanlinkage IsNot Nothing AndAlso ginowanlinkage.LinkageSystemNo > 0, ginowanlinkage.StatusType, Byte.MinValue)

        ' Yappli SDK を使用した、一意 ID の送信の有効 フラグ
        result.EnableYappliSdk = enableYappliSdk
        result.PartialViewModel = New PortalHomeDataAreaPartialViewModel(result) With {
            .BasalMetabolism = 0,
            .CalBreakfast = 0,
            .CalLunch = 0,
            .CalDinner = 0,
            .CalSnacking = 0,
            .CalConsumption = 0,
            .ExerciseCal = 0,
            .Mode = "Daily",
            .PointNow = 0,
            .ShowDay = Date.Today,
            .Steps = 0,
            .StepsCal = 0,
            .TargetCalorieIn = 0
        }

        Return result

    End Function


    Public Shared Function CreatePartialViewModel(mainModel As QolmsYappliModel, pageViewModel As PortalHomeViewModel, mode As String, showDay As Date) As PortalHomeDataAreaPartialViewModel

        Dim result As New PortalHomeDataAreaPartialViewModel(pageViewModel) '= mainModel.GetInputModelCache(Of PortalHomeDataAreaPartialViewModel)() ' キャッシュから取得

        'If result Is Nothing OrElse result.MembershipType <> mainModel.AuthorAccount.MembershipType Or (result IsNot Nothing AndAlso ((result.ShowDay <> showDay) OrElse (result.Mode <> mode))) Then
        'OrElse(result.Mode <> mode)
        ' キャッシュが無ければ API を実行

        showDay = showDay.AddDays(1).Date.AddMinutes(-1)
        With PortalHomeWorker.ExecutePortalHomeDetailReadApi(mainModel, True, mode, showDay)

            result.Mode = mode
            result.ShowDay = showDay

            Dim tp As Decimal = Decimal.MinValue
            If Decimal.TryParse(.TargetCalorieIn, tp) AndAlso tp > 0 Then
                result.TargetCalorieIn = Decimal.ToInt32(tp)
            Else
                result.TargetCalorieIn = 0
            End If
                        
            'カロミル目標カロリーを取得
            Dim goalList As List(Of CalomealGoalSet) = NoteCalomealWorker.GetMealWithBasis(mainModel,showDay,showDay)
            If goalList.Any() Then
                '取得出来たら
                Dim goal As CalomealGoalSet = goalList.OrderByDescending(Function(i) i.TargetDate).FirstOrDefault()
                Dim cal As Integer = iif(goal.BasisAllCalorie > 0, goal.BasisAllCalorie, 1).ToString().TryToValueType(1)'変換できない場合最小値の1へ置き換え（念のため）
                result.TargetCalorieIn = cal
                If goal.TargetDate >= Date.Now.Date andalso result.TargetCalorieIn <>cal Then '今日だったら更新
                    Task.Run(
                    Sub()
                        '登録の可否はとりあえず見なくていいし、登録は非同期
                        PortalTargetSettingWorker2.UpdateCalTarget(mainModel,cal)
                    End Sub
                )
                End If

            End If

            ' 食事個別のカロリーからトータルの　IN Cal計算
            tp = Decimal.MinValue
            If Decimal.TryParse(.CalBreakfast, tp) AndAlso tp > 0 Then
                result.CalBreakfast = tp
            Else
                result.CalBreakfast = 0
            End If
            tp = Decimal.MinValue
            If Decimal.TryParse(.CalLunch, tp) AndAlso tp > 0 Then
                result.CalLunch = tp
            Else
                result.CalLunch = 0
            End If
            tp = Decimal.MinValue
            If Decimal.TryParse(.CalDinner, tp) AndAlso tp > 0 Then
                result.CalDinner = tp
            Else
                result.CalDinner = 0
            End If
            tp = Decimal.MinValue
            If Decimal.TryParse(.CalSnacking, tp) AndAlso tp > 0 Then
                result.CalSnacking = tp
            Else
                result.CalSnacking = 0
            End If
            tp = Decimal.MinValue
            If Decimal.TryParse(.Steps, tp) AndAlso tp > 0 Then
                result.Steps = Decimal.ToInt32(tp)
            Else
                result.Steps = 0
            End If

            'TODO 歩数の消費カロリー計算
            ' 運動と歩数の消費カロリーからトータルの Out Cal計算
            tp = Decimal.MinValue
            If Decimal.TryParse(.StepsCal, tp) AndAlso tp > 0 Then
                result.StepsCal = Decimal.ToInt32(tp)
            Else
                result.StepsCal = 0
            End If

            tp = Decimal.MinValue
            If Decimal.TryParse(.ExerciseCal, tp) AndAlso tp > 0 Then
                result.ExerciseCal = Decimal.ToInt32(tp)
            Else
                result.ExerciseCal = 0
            End If
            tp = Decimal.MinValue
            If Decimal.TryParse(.BasalMetabolism, tp) AndAlso tp > 0 Then
                result.BasalMetabolism = Decimal.ToInt32(tp)
            Else
                result.BasalMetabolism = 0
            End If

            'If Integer.Parse(.TargetCalorieIn) > 0 Then
            '    result.TargetCalorieIn = Integer.Parse(.TargetCalorieIn)
            'Else
            '    result.TargetCalorieIn = 0
            'End If

            'If Integer.Parse(.TargetCalorieOut) > 0 Then
            '    result.TargetCalorieOut = Integer.Parse(.TargetCalorieOut)
            'Else
            '    result.TargetCalorieOut = 0
            'End If

            If Integer.Parse(.BasalMetabolism) > 0 Then
                result.BasalMetabolism = Integer.Parse(.BasalMetabolism)
            Else
                result.BasalMetabolism = 0
            End If

            'ポイント数の表示
            Try
                result.PointNow = QolmsPointWorker.GetQolmsPoint(
                                mainModel.ApiExecutor,
                                mainModel.ApiExecutorName,
                                mainModel.SessionId,
                                mainModel.ApiAuthorizeKey,
                                mainModel.AuthorAccount.AccountKey
                                )
            Catch ex As Exception
            End Try


            'ポイント履歴一覧
            'result.NewsN = GetPointNews(mainModel, result.PointNow)
            'result.NewsN = GetPointNews(mainModel)

            'result.PointMax = 6500
            'memo:プレミアム会員が半年で取得できるポイント数の上限（シミュレーション）

            ''健康年齢
            'tp = Decimal.MinValue
            'If Decimal.TryParse(.HealthAge, tp) AndAlso tp > 0 Then
            '    result.HealthAge = tp
            'Else
            '    result.HealthAge = Decimal.MinValue
            'End If

            'ポイントがつくかフラグ
            'Dim pInt As Integer = 0
            'If Integer.TryParse(.PointFlags, pInt) AndAlso pInt > 0 Then
            'result.PointFlags = CType([Enum].Parse(GetType(QyHomePointTypeEnum), .PointFlags), QyHomePointTypeEnum)
            'Else
            'result.PointFlags = 0
            'End If


            ' キャッシュへ追加
            'mainModel.SetInputModelCache(result)
        End With

        Return result

    End Function

    'Public Shared Function CreateGraphPartialViewModel(mainModel As QolmsYappliModel, pageViewModel As PortalHomeViewModel, vitalType As QyVitalTypeEnum, ByRef refPartialViewName As String) As QyVitalGraphPartialViewModelBase

    '    refPartialViewName = String.Empty

    '    Dim result As VitalStepsGraphPartialViewModel = Nothing

    '    ' 目標値
    '    Dim targetValues() As Decimal = {Decimal.Zero, Decimal.Zero, Decimal.Zero, Decimal.Zero}

    '    PortalHomeWorker.GetStandardValues(mainModel, vitalType, targetValues(0), targetValues(1), targetValues(2), targetValues(3))

    '    If vitalType = QyVitalTypeEnum.Steps Then
    '        refPartialViewName = "_NoteVitalStepsGraphPartialView"

    '        Dim hasData As Boolean = False

    '        If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso hasData Then
    '            If Not pageViewModel.StepsPartialViewModel.ItemList.Any() Then
    '                With PortalHomeWorker.ExecutePortalHomeGraphReadApi(mainModel, vitalType, PortalHomeWorker.DEFAULT_DAY_COUNT)
    '                    result = New VitalStepsGraphPartialViewModel(
    '                        pageViewModel,
    '                        .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
    '                        targetValues(0),
    '                        targetValues(1)
    '                    )
    '                End With

    '                pageViewModel.StepsPartialViewModel = result

    '                mainModel.SetInputModelCache(pageViewModel)

    '                Return result
    '            Else
    '                Return pageViewModel.StepsPartialViewModel
    '            End If
    '        Else
    '            Return New VitalStepsGraphPartialViewModel(
    '                pageViewModel,
    '                Nothing,
    '                Decimal.Zero,
    '                Decimal.Zero
    '            )
    '        End If
    '    End If

    'End Function

    ' TODO: 実装中
    Public Shared Function GetViewModelCache(mainModel As QolmsYappliModel) As PortalHomeViewModel

        ' キャッシュから取得
        Dim result As PortalHomeViewModel = mainModel.GetInputModelCache(Of PortalHomeViewModel)()

        If result Is Nothing Then
            ' ページ ビュー モデルを生成しキャッシュへ追加
            mainModel.SetInputModelCache(PortalHomeWorker.CreateViewModel(mainModel))

            ' キャッシュから取得
            result = mainModel.GetInputModelCache(Of PortalHomeViewModel)()
        End If

        Return result

    End Function

    Public Shared Function UpdateSteps(mainModel As QolmsYappliModel) As Integer

        'Alkoo Api で歩数のデータを取得
        Dim steps As Integer = PortalAlkooConnectionWorker.GetSteps(mainModel, Date.Now)

        '１歩以上だったら更新、それ以外は何もしない
        If steps > 0 AndAlso steps <= 999999D Then
            If NoteWalkWorker.Edit(mainModel, steps, Date.Now.Date) Then
                Return steps
            End If
        End If

        Return 0
    End Function


    Shared Function GetNews(mainModel As QolmsYappliModel) As List(Of String)

        'ポイント履歴一覧
        Return PortalHomeWorker.GetPointNews(mainModel)

    End Function

    Shared Function IsExaminationShow(mainModel As QolmsYappliModel) As Boolean

        'Linkageの確認、連携済みのリストを取得
        With PortalHomeWorker.ExecutePortalHomeMedicalLinkageReadApi(mainModel)

            '連携済みが一つ以上必要
            'もしくはビジネスプレミアムの登録が必要
            'もしくは健診結果が必要
            Return .ExeminationLinkageList.Count > 0 _
                OrElse mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Business _
                OrElse mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.BusinessFree _
                OrElse PortalHomeWorker.ExecutePortalHomeIsExaminationShowReadApi(mainModel)

        End With

    End Function

    Shared Function GetTasks(mainModel As QolmsYappliModel, showDay As Date) As PortalHomeTaskJsonResult

        Dim result As New PortalHomeTaskJsonResult()

        Dim model As PortalHomeViewModel = mainModel.GetInputModelCache(Of PortalHomeViewModel)()
        Dim challengeDay As Integer = (showDay - model.ChallengeAreaPartialViewModel.ChallengeStartDate).Days + 1 '何日目かを返すので開始日は１日目

        With PortalHomeWorker.ExecutePortalHomeTaskReadApi(mainModel, showDay, model.ChallengeAreaPartialViewModel.Challengekey, challengeDay)

            result.Steps = IIf(.StepsFlag.TryToValueType(False), "on", String.Empty).ToString()
            result.Weight = IIf(.WeightFlag.TryToValueType(False), "on", String.Empty).ToString()
            result.Breakfast = IIf(.BreakfastFlag.TryToValueType(False), "on", String.Empty).ToString()
            result.Lunch = IIf(.LunchFlag.TryToValueType(False), "on", String.Empty).ToString()
            result.Dinner = IIf(.DinnerFlag.TryToValueType(False), "on", String.Empty).ToString()

            If model.ChallengeAreaPartialViewModel.Challengekey <> Guid.Empty Then

                If .ColumnDispFlag.TryToValueType(False) Then

                    result.Column = IIf(.ColumnFlag.TryToValueType(False), "on", String.Empty).ToString()
                Else
                    result.Column = "disabled"
                End If
            Else
                result.Column = "hide"

            End If

            result.IsSuccess = Boolean.TrueString

            Return result

        End With

    End Function


    Shared Function IsFitbitConnected(mainModel As QolmsYappliModel) As Boolean

        Return PortalFitbitConnectionWorker.IsFitbitConnected(mainModel)

    End Function

    ''' <summary>
    ''' 動的なURLの文字列を作成します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="linkType">
    ''' 作成するリンクの種別
    ''' 1:健康ショートドラマ（Tanita）
    ''' 2:タニタオンラインセミナー動画（Tanita）
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function CreateUrl(mainModel As QolmsYappliModel, linkType As Byte) As PortalHomeUrlResult

        Dim result As New PortalHomeUrlResult

        Dim pageViewModel As PortalHomeViewModel = mainModel.GetInputModelCache(Of PortalHomeViewModel)() 'mainModel.GetInputModelCache(Of PortalHomeViewModel)()

        result.Url = TANITA_MOVIE_URI
        result.Param.Add("sid", TANITA_SITE_ID)

        Dim tanitaId As String = String.Empty
        Dim siteId As String = TANITA_SITE_ID

        If PortalTanitaConnectionWorker.GetConnected(mainModel, tanitaId) Then

            '暗号化処理
            Dim text As String = String.Format("{0},{1},{2}", siteId, tanitaId, Date.Now.ToString("yyyyMMddHHmm"))
            Using crypt As New QsCrypt(QsCryptTypeEnum.JotoKaradakarute)
                result.Param.Add("uid", crypt.EncryptString(text))
            End Using

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.Default)
                    result.Param.Add("siteKey", crypt.DecryptString(TANITA_SITE_KEY))

                    If pageViewModel.ChallengeList.Contains(Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1")) Then
                        '竹富                
                        result.Param.Add("categoryKey", crypt.DecryptString(TAKETOMI_CATEGORYKEY))

                    ElseIf pageViewModel.ChallengeList.Contains(Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c")) Then
                        ' 伊平屋
                        result.Param.Add("categoryKey", crypt.DecryptString(IHEYA_CATEGORYKEY))

                    End If

                End Using
            Catch ex As Exception
            End Try

            If linkType = 1 Then
                '1:健康ショートドラマの場合()
                '　/video/health_support_drama/webview
                result.Param.Add("url", "/video/health_support_drama/webview")

            Else
                '２．オンラインセミナー動画（動画版）の場合
                '　/video/seminar_drama/today/webview
                result.Param.Add("url", "/video/seminar_drama/today/webview")

            End If

            result.IsSuccess = Boolean.TrueString
            Return result
        Else

            result.IsSuccess = Boolean.FalseString
            Return result

        End If

    End Function
        
    Shared Function GetMedicalLinkageList(mainModel As QolmsYappliModel) As List(Of QhApiLinkageItem)

       'Linkageの確認、連携済みのリストを取得
        With PortalHomeWorker.ExecutePortalHomeMedicalLinkageReadApi(mainModel)

            Return .ExeminationLinkageList
        End With

    End Function


#End Region



End Class
