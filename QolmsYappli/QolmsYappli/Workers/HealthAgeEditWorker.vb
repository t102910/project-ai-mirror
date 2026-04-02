Imports System.ComponentModel.DataAnnotations
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsHealthAgeApiCoreV1

''' <summary>
''' 「健康年齢測定」画面に関する機能を提供します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class HealthAgeEditWorker

#Region "Constant"

    ''' <summary>
    ''' JMDC 健康年齢 Web API 用の ID を保持します（任意値）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared jmdcId As String = "jotohdr"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタ は使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 例外 メッセージ を構築します。
    ''' </summary>
    ''' <param name="ex">例外 オブジェクト。</param>
    ''' <param name="builder">
    ''' メッセージ が格納される可変型文字列（オプショナル）。
    ''' 未指定の場合は メソッド 内部で インスタンス を作成。
    ''' </param>
    ''' <returns>メッセージが 格納された可変型文字列。</returns>
    ''' <remarks></remarks>
    Private Shared Function BuildExceptionMessage(ex As Exception, Optional builder As StringBuilder = Nothing) As StringBuilder

        If builder Is Nothing Then builder = New StringBuilder()

        If ex IsNot Nothing Then
            builder.AppendFormat("■{0}：{1}", ex.GetType().ToString(), ex.Message).AppendLine()

            If ex.InnerException IsNot Nothing Then
                builder = BuildExceptionMessage(ex.InnerException, builder)
            End If
        End If

        Return builder

    End Function

    '' debug
    'Private Shared Sub DebugLog(message As String)

    '    Dim log As String = IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "apilog.txt")

    '    IO.File.AppendAllText(log, String.Format("{0} : {1}{2}", Now, message, vbCrLf))

    'End Sub

    ''' <summary>
    ''' 生年月日より、
    ''' 指定日における年齢を算出します。
    ''' </summary>
    ''' <param name="birthday">生年月日。</param>
    ''' <param name="oneDay">指定日。</param>
    ''' <returns>
    ''' 成功なら指定日における年齢、
    ''' 失敗なら Integer.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetAge(birthday As Date, oneDay As Date) As Integer

        Dim result As Integer = Integer.MinValue

        If birthday <> Date.MinValue _
            AndAlso oneDay <> Date.MinValue _
            AndAlso oneDay >= birthday Then

            Dim age As Integer = Integer.MinValue

            age = ((oneDay.Year * 10000 + oneDay.Month * 100 + oneDay.Day) - (birthday.Year * 10000 + birthday.Month * 100 + birthday.Day)) \ 10000

            If age >= Byte.MinValue AndAlso age <= Byte.MaxValue Then result = age
        End If

        Return result

    End Function

    ''' <summary>
    ''' JMDC 健康年齢 Web API が メンテナンス 中か判定します。
    ''' </summary>
    ''' <param name="refMessage">メンテナンス 中の場合に メッセージ が格納される変数。</param>
    ''' <returns>
    ''' メンテナンス なら True、
    ''' そうでなければ False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CheckMaintenance(ByRef refMessage As String) As Boolean

        refMessage = String.Empty

        Dim result As Boolean = False
        Dim now As Date = Date.Now
        Dim startDate As Date = New Date(now.Year, now.Month, 22 + DayOfWeek.Saturday - New Date(now.Year, now.Month, 1).DayOfWeek, 20, 0, 0) ' 第 4 土曜日 20 時
        Dim endDate As Date = startDate.AddHours(12) ' 翌 8 時

        If now >= startDate And now <= endDate Then
            ' 定期 メンテナンス 中
            refMessage = String.Format("{0}{1}{2}", "健康年齢WEBAPIは定期メンテナンス中です。", Environment.NewLine, "（毎月第四土曜日20時～翌8時）")

            result = True
        End If

        ' TODO: 臨時 メンテナンス の対応を検討する

        Return result

    End Function

    ''' <summary>
    ''' 「健康年齢測定」画面 インプット モデル の内容を、
    ''' API 用の健康年齢値情報の リスト へ変換します。
    ''' </summary>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' API 用の健康年齢値情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ToApiHealthAgeValueList(inputModel As HealthAgeEditInputModel) As List(Of QhApiHealthAgeValueItem)

        Dim result As New List(Of QhApiHealthAgeValueItem)()
        Dim recordDate As String = inputModel.RecordDate.ToApiDateString() ' 日まで

        With result
            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.BMI.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.BMI.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch014.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch014.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch016.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch016.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch019.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch019.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch021.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch021.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch023.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch023.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch025.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch025.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch027.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch027.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch029.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch029.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch035.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch035.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch035FBG.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch035FBG.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch037.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch037.ToString()
                }
            )

            .Add(
                New QhApiHealthAgeValueItem() With {
                    .HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch039.ToString(),
                    .RecordDate = recordDate,
                    .Value = inputModel.Ch039.ToString()
                }
            )
        End With

        Return result

    End Function

    ''' <summary>
    ''' 「健康年齢測定」画面の取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <returns>
    ''' 成功なら Web API 戻り値 クラス、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteHealthAgeEditReadApi(mainModel As QolmsYappliModel) As QhYappliHealthAgeEditReadApiResults

        Dim apiArgs As New QhYappliHealthAgeEditReadApiArgs(
            QhApiTypeEnum.YappliHealthAgeEditRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.HealthAgeEdit).ToString()
        }
        Dim apiResults As QhYappliHealthAgeEditReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliHealthAgeEditReadApiResults)(
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

    ''' <summary>
    ''' 「健康年齢測定」画面の登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <param name="responseN">健康年齢 Web API の レスポンス 情報の リスト。</param>
    ''' <returns>
    ''' 成功なら Web API 戻り値 クラス、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteHealthAgeEditWriteApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel, responseN As List(Of QhApiHealthAgeResponseItem)) As QhYappliHealthAgeEditWriteApiResults

        Dim apiArgs As New QhYappliHealthAgeEditWriteApiArgs(
            QhApiTypeEnum.YappliHealthAgeEditWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.HealthAgeEdit).ToString(),
            .HealthAgeValueN = HealthAgeEditWorker.ToApiHealthAgeValueList(inputModel),
            .HealthAgeResponseN = responseN
        }
        Dim apiResults As QhYappliHealthAgeEditWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliHealthAgeEditWriteApiResults)(
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

    ''' <summary>
    ''' 健康年齢（ベイジアン）の登録 API を実行します。
    ''' 体重の編集後に使用します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <param name="response">健康年齢 Web API の レスポンス 情報。</param>
    ''' <param name="apiExecutor">Web API の実行者の アカウント キー。</param>
    ''' <param name="apiExecutorName">Web API の実行者名。</param>
    ''' <param name="sessionId">セッション ID。</param>
    ''' <param name="apiAuthorizeKey">API 認証 キー。</param>
    ''' <returns>
    ''' 成功なら Web API 戻り値 クラス、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteHealthAgeEditBayesianWriteApi(
        accountKey As Guid,
        response As QhApiHealthAgeResponseItem,
        apiExecutor As Guid,
        apiExecutorName As String,
        sessionId As String,
        apiAuthorizeKey As Guid
    ) As QhYappliHealthAgeEditBayesianWriteApiResults

        Dim apiArgs As New QhYappliHealthAgeEditBayesianWriteApiArgs(
            QhApiTypeEnum.YappliHealthAgeEditBayesianWrite,
            QsApiSystemTypeEnum.Qolms,
            apiExecutor,
            apiExecutorName
        ) With {
            .ActorKey = accountKey.ToApiGuidString(),
            .HealthAgeResponse = response
        }
        Dim apiResults As QhYappliHealthAgeEditBayesianWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliHealthAgeEditBayesianWriteApiResults)(
            apiArgs,
            sessionId,
            apiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

#Region "JMDC 健康年齢 Web API"

    ''' <summary>
    ''' 健康年齢算出 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' 健康年齢 Web API の レスポンス 情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteCalculationApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel) As QhApiHealthAgeResponseItem

        Dim result As New QhApiHealthAgeResponseItem()
        Dim apiArgs As New calculationApiArgs()
        Dim apiResults As calculationApiResults = Nothing

        With result
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString() ' 日まで
            .ApiName = apiArgs.ApiName
            .ValueSet = String.Empty
            .Status = Byte.MinValue.ToString()
            .StatusCode = Integer.MinValue.ToString()
            .Message = String.Empty
        End With

        With apiArgs
            .id = HealthAgeEditWorker.jmdcId
            .visitDate = inputModel.RecordDate.ToString("yyyy/MM/dd")
            .gender = If(mainModel.AuthorAccount.SexType = QySexTypeEnum.Male, 0, 1)
            .age = HealthAgeEditWorker.GetAge(mainModel.AuthorAccount.Birthday.Date, inputModel.RecordDate.Date)
            .height = 0D
            .weight = 0D
            .bmi = inputModel.BMI.TryToValueType(Decimal.Zero)
            .ch014 = inputModel.Ch014.TryToValueType(Decimal.Zero)
            .ch016 = inputModel.Ch016.TryToValueType(Decimal.Zero)
            .ch019 = inputModel.Ch019.TryToValueType(Decimal.Zero)
            .ch021 = inputModel.Ch021.TryToValueType(Decimal.Zero)
            .ch023 = inputModel.Ch023.TryToValueType(Decimal.Zero)
            .ch025 = inputModel.Ch025.TryToValueType(Decimal.Zero)
            .ch027 = inputModel.Ch027.TryToValueType(Decimal.Zero)
            .ch029 = inputModel.Ch029.TryToValueType(Decimal.Zero)
            .ch035 = inputModel.Ch035.TryToValueType(Decimal.Zero)
            .ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(Decimal.Zero)
            .ch037 = inputModel.Ch037.TryToValueType(Decimal.Zero)
            .ch039 = inputModel.Ch039.TryToValueType(Decimal.Zero)
        End With

        Try
            apiResults = QsHealthAgeApiManager.Execute(Of calculationApiArgs, calculationApiResults)(apiArgs)

            'HealthAgeEditWorker.DebugLog(String.Format("{0} {1} {2} {3} {4}", mainModel.AuthorAccount.AccountKey, apiArgs.ApiName, apiResults.StatusCode, apiResults.requestString, apiResults.responseString)) ' debug
        Catch ex As Exception
            Dim sb As New StringBuilder()

            HealthAgeEditWorker.BuildExceptionMessage(ex, sb) ' debug
            'HealthAgeEditWorker.DebugLog(sb.ToString()) ' debug

            result.Message = sb.ToString()
        End Try

        If apiResults IsNot Nothing AndAlso apiResults.IsSuccess Then
            result.ValueSet = apiResults.responseString
            result.Status = 2.ToString()
            result.StatusCode = apiResults.StatusCode.ToString()
        End If

        Return result

    End Function

    ''' <summary>
    ''' 同世代健康年齢分布取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' 健康年齢 Web API の レスポンス 情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteAgeDistributionApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel) As QhApiHealthAgeResponseItem

        Dim result As New QhApiHealthAgeResponseItem()
        Dim apiArgs As New ageDistributionApiArgs()
        Dim apiResults As ageDistributionApiResults = Nothing

        With result
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString() ' 日まで
            .ApiName = apiArgs.ApiName
            .ValueSet = String.Empty
            .Status = Byte.MinValue.ToString()
            .StatusCode = Integer.MinValue.ToString()
            .Message = String.Empty
        End With

        With apiArgs
            .id = HealthAgeEditWorker.jmdcId
            .gender = If(mainModel.AuthorAccount.SexType = QySexTypeEnum.Male, 0, 1)
            .age = HealthAgeEditWorker.GetAge(mainModel.AuthorAccount.Birthday.Date, inputModel.RecordDate.Date)
        End With

        Try
            apiResults = QsHealthAgeApiManager.Execute(Of ageDistributionApiArgs, ageDistributionApiResults)(apiArgs)

            'HealthAgeEditWorker.DebugLog(String.Format("{0} {1} {2} {3} {4}", mainModel.AuthorAccount.AccountKey, apiArgs.ApiName, apiResults.StatusCode, apiResults.requestString, apiResults.responseString)) ' debug
        Catch ex As Exception
            Dim sb As New StringBuilder()

            HealthAgeEditWorker.BuildExceptionMessage(ex, sb) ' debug
            'HealthAgeEditWorker.DebugLog(sb.ToString()) ' debug

            result.Message = sb.ToString()
        End Try

        If apiResults IsNot Nothing AndAlso apiResults.IsSuccess Then
            result.ValueSet = apiResults.responseString
            result.Status = 2.ToString()
            result.StatusCode = apiResults.StatusCode.ToString()
        End If

        Return result

    End Function

    ''' <summary>
    ''' 健診結果 レベル 判定 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' 健康年齢 Web API の レスポンス 情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteInsDevianceApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel) As QhApiHealthAgeResponseItem

        Dim result As New QhApiHealthAgeResponseItem()
        Dim apiArgs As New insDevianceApiArgs()
        Dim apiResults As insDevianceApiResults = Nothing

        With result
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString() ' 日まで
            .ApiName = apiArgs.ApiName
            .ValueSet = String.Empty
            .Status = Byte.MinValue.ToString()
            .StatusCode = Integer.MinValue.ToString()
            .Message = String.Empty
        End With

        With apiArgs
            .id = HealthAgeEditWorker.jmdcId
            .height = 0D
            .weight = 0D
            .bmi = inputModel.BMI.TryToValueType(Decimal.Zero)
            .ch014 = inputModel.Ch014.TryToValueType(Decimal.Zero)
            .ch016 = inputModel.Ch016.TryToValueType(Decimal.Zero)
            .ch019 = inputModel.Ch019.TryToValueType(Decimal.Zero)
            .ch021 = inputModel.Ch021.TryToValueType(Decimal.Zero)
            .ch023 = inputModel.Ch023.TryToValueType(Decimal.Zero)
            .ch025 = inputModel.Ch025.TryToValueType(Decimal.Zero)
            .ch027 = inputModel.Ch027.TryToValueType(Decimal.Zero)
            .ch029 = inputModel.Ch029.TryToValueType(Decimal.Zero)
            .ch035 = inputModel.Ch035.TryToValueType(Decimal.Zero)
            .ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(Decimal.Zero)
        End With

        Try
            apiResults = QsHealthAgeApiManager.Execute(Of insDevianceApiArgs, insDevianceApiResults)(apiArgs)

            'HealthAgeEditWorker.DebugLog(String.Format("{0} {1} {2} {3} {4}", mainModel.AuthorAccount.AccountKey, apiArgs.ApiName, apiResults.StatusCode, apiResults.requestString, apiResults.responseString)) ' debug
        Catch ex As Exception
            Dim sb As New StringBuilder()

            HealthAgeEditWorker.BuildExceptionMessage(ex, sb) ' debug
            'HealthAgeEditWorker.DebugLog(sb.ToString()) ' debug

            result.Message = sb.ToString()
        End Try

        If apiResults IsNot Nothing AndAlso apiResults.IsSuccess Then
            result.ValueSet = apiResults.responseString
            result.Status = 2.ToString()
            result.StatusCode = apiResults.StatusCode.ToString()
        End If

        Return result

    End Function

    ''' <summary>
    ''' 同世代健診値比較 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' 健康年齢 Web API の レスポンス 情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteInsComparisonApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel) As QhApiHealthAgeResponseItem

        Dim result As New QhApiHealthAgeResponseItem()
        Dim apiArgs As New insComparisonApiArgs()
        Dim apiResults As insComparisonApiResults = Nothing

        With result
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString() ' 日まで
            .ApiName = apiArgs.ApiName
            .ValueSet = String.Empty
            .Status = Byte.MinValue.ToString()
            .StatusCode = Integer.MinValue.ToString()
            .Message = String.Empty
        End With

        With apiArgs
            .id = HealthAgeEditWorker.jmdcId
            .gender = If(mainModel.AuthorAccount.SexType = QySexTypeEnum.Male, 0, 1)
            .age = HealthAgeEditWorker.GetAge(mainModel.AuthorAccount.Birthday.Date, inputModel.RecordDate.Date)
            .height = 0D
            .weight = 0D
            .bmi = inputModel.BMI.TryToValueType(Decimal.Zero)
            .ch014 = inputModel.Ch014.TryToValueType(Decimal.Zero)
            .ch016 = inputModel.Ch016.TryToValueType(Decimal.Zero)
            .ch019 = inputModel.Ch019.TryToValueType(Decimal.Zero)
            .ch021 = inputModel.Ch021.TryToValueType(Decimal.Zero)
            .ch023 = inputModel.Ch023.TryToValueType(Decimal.Zero)
            .ch025 = inputModel.Ch025.TryToValueType(Decimal.Zero)
            .ch027 = inputModel.Ch027.TryToValueType(Decimal.Zero)
            .ch029 = inputModel.Ch029.TryToValueType(Decimal.Zero)
            .ch035 = inputModel.Ch035.TryToValueType(Decimal.Zero)
            .ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(Decimal.Zero)
        End With

        Try
            apiResults = QsHealthAgeApiManager.Execute(Of insComparisonApiArgs, insComparisonApiResults)(apiArgs)

            'HealthAgeEditWorker.DebugLog(String.Format("{0} {1} {2} {3} {4}", mainModel.AuthorAccount.AccountKey, apiArgs.ApiName, apiResults.StatusCode, apiResults.requestString, apiResults.responseString)) ' debug
        Catch ex As Exception
            Dim sb As New StringBuilder()

            HealthAgeEditWorker.BuildExceptionMessage(ex, sb) ' debug
            'HealthAgeEditWorker.DebugLog(sb.ToString()) ' debug

            result.Message = sb.ToString()
        End Try

        If apiResults IsNot Nothing AndAlso apiResults.IsSuccess Then
            result.ValueSet = apiResults.responseString
            result.Status = 2.ToString()
            result.StatusCode = apiResults.StatusCode.ToString()
        End If

        Return result

    End Function

    ''' <summary>
    ''' 健康年齢改善 アドバイス API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' 健康年齢 Web API の レスポンス 情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteAdviceApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel) As QhApiHealthAgeResponseItem

        Dim result As New QhApiHealthAgeResponseItem()
        Dim apiArgs As New adviceApiArgs()
        Dim apiResults As adviceApiResults = Nothing

        With result
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString() ' 日まで
            .ApiName = apiArgs.ApiName
            .ValueSet = String.Empty
            .Status = Byte.MinValue.ToString()
            .StatusCode = Integer.MinValue.ToString()
            .Message = String.Empty
        End With

        With apiArgs
            .id = HealthAgeEditWorker.jmdcId
            .gender = If(mainModel.AuthorAccount.SexType = QySexTypeEnum.Male, 0, 1)
            .age = HealthAgeEditWorker.GetAge(mainModel.AuthorAccount.Birthday.Date, inputModel.RecordDate.Date)
            .height = 0D
            .weight = 0D
            .bmi = inputModel.BMI.TryToValueType(Decimal.Zero)
            .ch014 = inputModel.Ch014.TryToValueType(Decimal.Zero)
            .ch016 = inputModel.Ch016.TryToValueType(Decimal.Zero)
            .ch019 = inputModel.Ch019.TryToValueType(Decimal.Zero)
            .ch021 = inputModel.Ch021.TryToValueType(Decimal.Zero)
            .ch023 = inputModel.Ch023.TryToValueType(Decimal.Zero)
            .ch025 = inputModel.Ch025.TryToValueType(Decimal.Zero)
            .ch027 = inputModel.Ch027.TryToValueType(Decimal.Zero)
            .ch029 = inputModel.Ch029.TryToValueType(Decimal.Zero)
            .ch035 = inputModel.Ch035.TryToValueType(Decimal.Zero)
            .ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(Decimal.Zero)
        End With

        Try
            apiResults = QsHealthAgeApiManager.Execute(Of adviceApiArgs, adviceApiResults)(apiArgs)

            'HealthAgeEditWorker.DebugLog(String.Format("{0} {1} {2} {3} {4}", mainModel.AuthorAccount.AccountKey, apiArgs.ApiName, apiResults.StatusCode, apiResults.requestString, apiResults.responseString)) ' debug
        Catch ex As Exception
            Dim sb As New StringBuilder()

            HealthAgeEditWorker.BuildExceptionMessage(ex, sb) ' debug
            'HealthAgeEditWorker.DebugLog(sb.ToString()) ' debug

            result.Message = sb.ToString()
        End Try

        If apiResults IsNot Nothing AndAlso apiResults.IsSuccess Then
            result.ValueSet = apiResults.responseString
            result.Status = 2.ToString()
            result.StatusCode = apiResults.StatusCode.ToString()
        End If

        Return result

    End Function

    ''' <summary>
    ''' 健康年齢 ベイジアン ネットワーク 算出 API を実行します。
    ''' 体重の編集後に使用します。
    ''' </summary>
    ''' <param name="sexType">性別の種別。</param>
    ''' <param name="birthday">生年月日。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <param name="plus">今回測定した BMI。</param>
    ''' <returns>
    ''' 健康年齢 Web API の レスポンス 情報の リスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteBayesianApi(sexType As QySexTypeEnum, birthday As Date, inputModel As HealthAgeEditInputModel, plus As Decimal) As QhApiHealthAgeResponseItem

        Dim result As New QhApiHealthAgeResponseItem()
        Dim apiArgs As New bayesianApiArgs()
        Dim apiResults As bayesianApiResults = Nothing

        With result
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString() ' 日まで
            .ApiName = apiArgs.ApiName
            .ValueSet = String.Empty
            .Status = Byte.MinValue.ToString()
            .StatusCode = Integer.MinValue.ToString()
            .Message = String.Empty
        End With

        With apiArgs
            .id = HealthAgeEditWorker.jmdcId
            .gender = If(sexType = QySexTypeEnum.Male, 0, 1)
            .age = HealthAgeEditWorker.GetAge(birthday.Date, inputModel.RecordDate.Date)
            .bmi = inputModel.BMI.TryToValueType(Decimal.Zero)
            .ch014 = inputModel.Ch014.TryToValueType(Decimal.Zero)
            .ch016 = inputModel.Ch016.TryToValueType(Decimal.Zero)
            .ch019 = inputModel.Ch019.TryToValueType(Decimal.Zero)
            .ch021 = inputModel.Ch021.TryToValueType(Decimal.Zero)
            .ch023 = inputModel.Ch023.TryToValueType(Decimal.Zero)
            .ch025 = inputModel.Ch025.TryToValueType(Decimal.Zero)
            .ch027 = inputModel.Ch027.TryToValueType(Decimal.Zero)
            .ch029 = inputModel.Ch029.TryToValueType(Decimal.Zero)
            .ch035 = inputModel.Ch035.TryToValueType(Decimal.Zero)
            .ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(Decimal.Zero)
            .ch037 = inputModel.Ch037.TryToValueType(Decimal.Zero)
            .ch039 = inputModel.Ch039.TryToValueType(Decimal.Zero)
            .plus = plus
        End With

        Try
            apiResults = QsHealthAgeApiManager.Execute(Of bayesianApiArgs, bayesianApiResults)(apiArgs)

            '' debug
            'HealthAgeEditWorker.DebugLog(String.Format("{0} {1} {2} {3} {4}", mainModel.AuthorAccount.AccountKey, apiArgs.ApiName, apiResults.StatusCode, apiResults.requestString, apiResults.responseString)) ' debug
        Catch ex As Exception
            Dim sb As New StringBuilder()

            HealthAgeEditWorker.BuildExceptionMessage(ex, sb) ' debug
            'HealthAgeEditWorker.DebugLog(sb.ToString()) ' debug

            result.Message = sb.ToString()
        End Try

        If apiResults IsNot Nothing AndAlso apiResults.IsSuccess Then
            result.ValueSet = apiResults.responseString
            result.Status = 2.ToString()
            result.StatusCode = apiResults.StatusCode.ToString()
        End If

        Return result

    End Function

#End Region

#End Region

#Region "Public Method"

    ''' <summary>
    ''' メイン モデル を指定して、
    ''' 「健康年齢測定」画面 インプット モデル を作成します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <returns>
    ''' 成功なら「健康年齢測定」画面 インプット モデル、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateInputModel(mainModel As QolmsYappliModel, fromPageNoType As QyPageNoTypeEnum) As HealthAgeEditInputModel

        Dim result As New HealthAgeEditInputModel(mainModel, Nothing)

        With HealthAgeEditWorker.ExecuteHealthAgeEditReadApi(mainModel)
            Dim dic As New Dictionary(Of QyHealthAgeValueTypeEnum, Tuple(Of Date, Decimal))()

            If .IsSuccess.TryToValueType(False) AndAlso .HealthAgeValueN IsNot Nothing AndAlso .HealthAgeValueN.Any() Then
                .HealthAgeValueN.ForEach(
                    Sub(i)
                        Dim key As QyHealthAgeValueTypeEnum = i.HealthAgeValueType.TryToValueType(QyHealthAgeValueTypeEnum.None)

                        If key <> QyHealthAgeValueTypeEnum.None AndAlso Not dic.ContainsKey(key) Then
                            dic.Add(
                                key,
                                New Tuple(Of Date, Decimal)(i.RecordDate.TryToValueType(Date.MinValue), i.Value.TryToValueType(Decimal.MinValue))
                            )
                        End If
                    End Sub
                )
            End If

            result = New HealthAgeEditInputModel(mainModel, dic)
            result.IsMaintenance = HealthAgeEditWorker.CheckMaintenance(result.MaintenanceMessage)
            result.FromPageNoType = fromPageNoType
        End With

        Return result

    End Function

    ''' <summary>
    ''' メイン モデル および健診結果を指定して、
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="recordDate">健診受診日。</param>
    ''' <param name="valueN">
    ''' 健康年齢測定の種別を キー、
    ''' 健診結果値を値とする ディクショナリ。
    ''' </param>
    ''' <returns>
    ''' 成功なら「健康年齢測定」画面 インプット モデル、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks>
    ''' <paramref name="valueN" /> の キーの意味。
    ''' <see cref="QyHealthAgeValueTypeEnum.BMI" />：BMI
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch014" />：収縮期血圧
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch016" />：拡張期血圧
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch019" />：中性脂肪を
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch021" />：HDL コレステロール
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch023" />：LDL コレステロール
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch025" />：GOT（AST）
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch027" />：GPT（ALT）
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch029" />：γ-GT（γ-GTP）
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch035" />：HbA1c（NGSP）
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch035FBG" />：空腹時血糖
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch037" />：尿糖（値は数値に変換、1：－、2：±、3：＋、4：＋＋、5：＋＋＋）
    ''' <see cref="QyHealthAgeValueTypeEnum.Ch039" />：尿蛋白（定性）（値は数値に変換、1：－、2：±、3：＋、4：＋＋、5：＋＋＋）
    ''' </remarks>
    Public Shared Function CreateInputModelByExamination(mainModel As QolmsYappliModel, recordDate As Date, valueN As Dictionary(Of QyHealthAgeValueTypeEnum, Decimal), pageNo As QyPageNoTypeEnum) As HealthAgeEditInputModel

        Dim result As HealthAgeEditInputModel = HealthAgeEditWorker.CreateInputModel(mainModel, pageNo)

        If recordDate <> Date.MinValue AndAlso valueN IsNot Nothing AndAlso valueN.Any() Then
            Dim dic As New Dictionary(Of QyHealthAgeValueTypeEnum, Tuple(Of Date, Decimal))()

            valueN.ToList().ForEach(
                Sub(i)
                    If i.Key <> QyHealthAgeValueTypeEnum.None AndAlso Not dic.ContainsKey(i.Key) Then
                        dic.Add(
                            i.Key,
                            New Tuple(Of Date, Decimal)(recordDate, i.Value)
                        )
                    End If
                End Sub
            )

            result.UpdateByInput(New HealthAgeEditInputModel(mainModel, dic) With {.RecordDate = recordDate})
            result.FromPageNoType = pageNo
        End If

        Return result

    End Function

    ''' <summary>
    ''' 健診結果を使用して作成した「健康年齢測定」画面 インプット モデル を検証します。
    ''' </summary>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' 失敗した検証の情報を保持する コレクション。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function ValidateByInputModelByExamination(inputModel As HealthAgeEditInputModel) As IEnumerable(Of ValidationResult)

        Return inputModel.Validate(Nothing)

    End Function

    ''' <summary>
    ''' 健康年齢を登録します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <param name="responseN">健康年齢 Web API の レスポンス 情報の リスト。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel, responseN As List(Of QhApiHealthAgeResponseItem)) As Boolean

        Dim pointActionDate As Date = Date.Now
        Dim pointTargetDate As Date = New Date(
            inputModel.RecordDate.Year,
            inputModel.RecordDate.Month,
            inputModel.RecordDate.Day
        )
        Dim pointLimitDate As Date = New Date(
            pointActionDate.Year,
            pointActionDate.Month,
            1
        ).AddMonths(7).AddDays(-1) ' ポイント 有効期限は 6 ヶ月後の月末（起点は操作日時）

        With HealthAgeEditWorker.ExecuteHealthAgeEditWriteApi(mainModel, inputModel, responseN)
            ' アカウント 情報の会員の種別を更新
            mainModel.AuthorAccount.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

            ' 「健康年齢」画面の キャッシュ を クリア
            mainModel.RemoveInputModelCache(Of HealthAgeViewModel)()

            ' 「ホーム」画面の キャッシュ を クリア
            mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

            '' 非同期で ポイント 付与
            'Task.Run(
            '    Sub()
            '        ' ポイント 対象日に対して未付与なら、付与を実行
            '        If QolmsPointWorker.GetTargetPointFromHistory(
            '            mainModel.ApiExecutor,
            '            mainModel.ApiExecutorName,
            '            mainModel.SessionId,
            '            mainModel.ApiAuthorizeKey,
            '            mainModel.AuthorAccount.AccountKey,
            '            QyPointItemTypeEnum.Examination,
            '            pointTargetDate
            '        ) = 0 Then

            '            QolmsPointWorker.AddQolmsPoints(
            '                mainModel.ApiExecutor,
            '                mainModel.ApiExecutorName,
            '                mainModel.SessionId,
            '                mainModel.ApiAuthorizeKey,
            '                mainModel.AuthorAccount.AccountKey,
            '                {
            '                    New QolmsPointGrantItem(
            '                        mainModel.AuthorAccount.MembershipType,
            '                        pointActionDate,
            '                        Guid.NewGuid().ToApiGuidString(),
            '                        QyPointItemTypeEnum.Examination,
            '                        pointLimitDate,
            '                        pointTargetDate
            '                    )
            '                }.ToList()
            '            )
            '        End If
            '    End Sub
            ')

            Return True
        End With

    End Function

    ''' <summary>
    ''' 健診受診日の時点での年齢が、
    ''' 18 歳以上かつ 74 歳以下か判定します。
    ''' </summary>
    ''' <param name="birthday">生年月日。</param>
    ''' <param name="recordDate">健診受診日。</param>
    ''' <returns>
    ''' 条件を満たすなら String.Empty、
    ''' そうでなければ エラー メッセージ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CheckRecordDate(birthday As Date, recordDate As Date) As String

        Dim result As String = String.Empty

        If recordDate <> Date.MinValue Then
            If recordDate >= birthday Then
                Dim age As Integer = HealthAgeEditWorker.GetAge(birthday, recordDate) ' 健診受診日における年齢

                If age < 18 OrElse age > 74 Then
                    result = "健診受診日の時点で18歳以上74歳以下である必要があります。"
                End If
            Else
                result = "健診受診日が不正です。"
            End If
        Else
            result = "健診受診日を入力してください。"
        End If

        Return result

    End Function

    ''' <summary>
    ''' JMDC 健康年齢 Web API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <returns>
    ''' Web API のレスポンス情報のリスト。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function ExecuteJmdcHealthAgeApi(mainModel As QolmsYappliModel, inputModel As HealthAgeEditInputModel) As List(Of QhApiHealthAgeResponseItem)

        Dim result As New List(Of QhApiHealthAgeResponseItem)()

        ' 会員レベルに関係なく API はすべて実行しておく
        With result
            .Add(HealthAgeEditWorker.ExecuteCalculationApi(mainModel, inputModel))
            .Add(HealthAgeEditWorker.ExecuteAgeDistributionApi(mainModel, inputModel))
            .Add(HealthAgeEditWorker.ExecuteInsDevianceApi(mainModel, inputModel))
            .Add(HealthAgeEditWorker.ExecuteInsComparisonApi(mainModel, inputModel))
            .Add(HealthAgeEditWorker.ExecuteAdviceApi(mainModel, inputModel))
        End With

        Return result

    End Function

    ''' <summary>
    ''' 健康年齢（ベイジアン）を登録します。
    ''' 体重の編集後に使用します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <param name="sexType">性別の種別。</param>
    ''' <param name="birthday">生年月日。</param>
    ''' <param name="membershipType">会員の種別。</param>
    ''' <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
    ''' <param name="weight">体重。</param>
    ''' <param name="height">身長。</param>
    ''' <param name="apiExecutor">Web API の実行者の アカウント キー。</param>
    ''' <param name="apiExecutorName">Web API の実行者名。</param>
    ''' <param name="sessionId">セッション ID。</param>
    ''' <param name="apiAuthorizeKey">API 認証 キー。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False もしくは例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function EditBayesian(
        accountKey As Guid,
        sexType As QySexTypeEnum,
        birthday As Date,
        membershipType As QyMemberShipTypeEnum,
        inputModel As HealthAgeEditInputModel,
        weight As Decimal,
        height As Decimal,
        apiExecutor As Guid,
        apiExecutorName As String,
        sessionId As String,
        apiAuthorizeKey As Guid
    ) As Boolean

        'Return True ' debug

        Dim result As Boolean = False
        Dim response As New QhApiHealthAgeResponseItem() With {
            .RecordDate = inputModel.RecordDate.Date.ToApiDateString(),
            .ApiName = New bayesianApiArgs().ApiName,
            .ValueSet = String.Empty,
            .Status = Byte.MinValue.ToString(),
            .StatusCode = Integer.MinValue.ToString(),
            .Message = String.Empty
        }
        Dim message As String = String.Empty

        ' プレミアム会員なら実行
        If (membershipType = QyMemberShipTypeEnum.LimitedTime OrElse membershipType = QyMemberShipTypeEnum.Premium _
            OrElse membershipType = QyMemberShipTypeEnum.Business OrElse membershipType = QyMemberShipTypeEnum.BusinessFree) _
            AndAlso String.IsNullOrWhiteSpace(HealthAgeEditWorker.CheckRecordDate(birthday, inputModel.RecordDate)) _
            AndAlso Not HealthAgeEditWorker.CheckMaintenance(message) Then

            '今回の BMI 値
            Dim plus As Decimal = Decimal.MinValue

            If weight > Decimal.Zero AndAlso height > Decimal.Zero Then plus = Math.Truncate(weight / (height * height) * 100000) / 10

            If plus > Decimal.Zero Then
                ' 登録（更新）
                response = HealthAgeEditWorker.ExecuteBayesianApi(sexType, birthday, inputModel, plus)

                If Not String.IsNullOrWhiteSpace(response.ValueSet) _
                    AndAlso response.Status.TryToValueType(Byte.MinValue) = 2 _
                    AndAlso response.StatusCode.TryToValueType(Integer.MinValue) = 200 Then

                    Dim jsonObject As bayesianApiResults = Nothing

                    Try
                        jsonObject = New QolmsDbEntityV1.QsJsonSerializer().Deserialize(Of bayesianApiResults)(response.ValueSet.Trim())
                    Catch
                    End Try

                    If jsonObject IsNot Nothing Then
                        With HealthAgeEditWorker.ExecuteHealthAgeEditBayesianWriteApi(
                            accountKey,
                            response,
                            apiExecutor,
                            apiExecutorName,
                            sessionId,
                            apiAuthorizeKey
                        )

                            result = .IsSuccess.TryToValueType(False)

                            'Debug.Print(String.Format("■健康年齢（ベイジアン）：{0} 歳", jsonObject.healthage)) ' debug
                        End With
                    End If
                End If
            Else
                ' 削除
                response.ValueSet = String.Empty
                response.Status = "2"
                response.StatusCode = "200"

                With HealthAgeEditWorker.ExecuteHealthAgeEditBayesianWriteApi(
                    accountKey,
                    response,
                    apiExecutor,
                    apiExecutorName,
                    sessionId,
                    apiAuthorizeKey
                )

                    result = .IsSuccess.TryToValueType(False)

                    'Debug.Print("■健康年齢（ベイジアン）：削除") ' debug
                End With
            End If
        End If

        Return result

    End Function

#End Region

End Class
