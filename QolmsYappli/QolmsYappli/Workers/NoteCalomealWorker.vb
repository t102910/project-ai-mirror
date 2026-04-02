Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1
Imports System.Runtime.Serialization
Imports System.Threading.Tasks

''' <summary>
''' 「カロミル」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class NoteCalomealWorker
    
    
#Region "Public Prop"

    Public Shared Property LogFilePath As String = String.Empty

#End Region


#Region "Constant"

    ''' <summary>
    ''' カロミル API URI
    ''' </summary>
    Public Shared ReadOnly REQUEST_URI As String = ConfigurationManager.AppSettings("CalomealWebViewApiUri")

    ''' <summary>
    ''' カロミル 連携システム番号
    ''' </summary>
    Public Shared ReadOnly CALOMEAL_LINKAGESYSTEMNO As Integer = 47015

    ''' <summary>
    ''' 竹富町 JWT の設定
    ''' </summary>
    Public Shared ReadOnly TAKETOMI_JWT As String = ConfigurationManager.AppSettings("CalomealWebViewTaketomiJwt")

    
    ''' <summary>
    ''' 伊平屋村 JWT の設定
    ''' </summary>
    Public Shared ReadOnly IHEYA_JWT As String = ConfigurationManager.AppSettings("CalomealWebViewIheyaJwt")

    ''' <summary>
    ''' 沖縄セルラー JWT の設定
    ''' </summary>
    Public Shared ReadOnly OCT_JWT As String = ConfigurationManager.AppSettings("CalomealWebViewOctJwt")

    ''' <summary>
    ''' カロミル ログ のファイル名です
    ''' </summary>
    Private Const CALOMEAL_LOG_FILENAME As String = "Calomeal.Log"

    ''' <summary>
    ''' カロミル トークンAPI呼び出しログ のファイル名です
    ''' </summary>    
    Private Const CALOMEAL_TOKENLOG_FILENAME As String = "CalomealToken.Log"

    ''' <summary>
    ''' カロミル 食事API呼び出しログ のファイル名です
    ''' </summary>    
    Private Const CALOMEAL_MEALLOG_FILENAME As String = "CalomealMeal.Log"

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

#Region " 共通機能"

    'テスト用の手抜きログ吐き
    '<Conditional("DEBUG")> _
    'Private Shared Sub 'DebugLog(ByVal message As String)
    '    Try
    '        Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "Calomeal.Log")
    '        System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
    '    Catch ex As Exception
    '    End Try

    'End Sub

#End Region

    Private Shared Function ExecuteCalomealConnectionWriteApi(mainModel As QolmsYappliModel, linkgaSystemId As String, token As CalomealAccessTokenSet, deleteFlag As Boolean) As QhYappliPortalCalomealConnectionWriteApiResults

        Dim apiArgs As New QhYappliPortalCalomealConnectionWriteApiArgs(
            QhApiTypeEnum.YappliPortalCalomealConnectionWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = CALOMEAL_LINKAGESYSTEMNO.ToString(),
            .LinkageSystemId = linkgaSystemId,
            .DeleteFlag = deleteFlag.ToString(),
            .TokenSet = New QhApiCalomealTokenSetItem() With {
            .Token = token.access_token,
            .TokenExpires = token.TokenExpires.ToApiDateString(),
            .RefreshToken = token.refresh_token,
            .RefreshTokenExpires = token.RefreshTokenExpires.ToApiDateString()
        }
        }
        Dim apiResults As QhYappliPortalCalomealConnectionWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCalomealConnectionWriteApiResults)(
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

    Private Shared Function ExecuteCalomealConnectionTokenReadApi(mainModel As QolmsYappliModel) As QhYappliPortalCalomealConnectionTokenReadApiResults

        Dim apiArgs As New QhYappliPortalCalomealConnectionTokenReadApiArgs(
            QhApiTypeEnum.YappliPortalCalomealConnectionTokenRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = CALOMEAL_LINKAGESYSTEMNO.ToString()
        }
        Dim apiResults As QhYappliPortalCalomealConnectionTokenReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCalomealConnectionTokenReadApiResults)(
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

    Private Shared Function ExecuteCalomealMealSyncRead(mainModel As QolmsYappliModel) As QhYappliNoteCalomealMealSyncReadApiResults

        Dim apiArgs As New QhYappliNoteCalomealMealSyncReadApiArgs(
            QhApiTypeEnum.YappliNoteCalomealMealSyncRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = CALOMEAL_LINKAGESYSTEMNO.ToString()
        }
        Dim apiResults As QhYappliNoteCalomealMealSyncReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteCalomealMealSyncReadApiResults)(
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

    Private Shared Function ExecuteCalomealMealSyncWrite(mainModel As QolmsYappliModel, model As List(Of NoteMeal2InputModel)) As QhYappliNoteCalomealMealSyncWriteApiResults

        Dim apiArgs As New QhYappliNoteCalomealMealSyncWriteApiArgs(
            QhApiTypeEnum.YappliNoteCalomealMealSyncWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .MealItemN = model.ConvertAll(Of QhApiCalomealMealSyncItem)(
                Function(i) New QhApiCalomealMealSyncItem() With {
                    .AnalysisSet = i.AnalysisSet,
                    .AnalysisType = i.AnalysisType.ToString(),
                    .Calorie = i.Calorie.ToString(),
                    .ChooseSet = i.ChooseSet,
                    .DeleteFlag = i.DeleteFlag.ToString(),
                    .HistoryId = i.HistoryId.ToString(),
                    .ItemName = i.ItemName.ToString(),
                    .MealType = i.MealType.ToString(),
                    .Rate = i.Rate.ToString(),
                    .RecordDate = i.RecordDate.ToApiDateString(),
                    .HasImage = i.HasImage.ToString()
                }
            )
        }
        Dim apiResults As QhYappliNoteCalomealMealSyncWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteCalomealMealSyncWriteApiResults)(
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
    ''' 日付入りのファイルパスをセットします
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="Name"></param>
    Private Shared Sub GetLogFilePath(path As String, Name As String)

        'ログパスをセット
        NoteCalomealWorker.LogFilePath = System.IO.Path.Combine(path, $"{Date.Today.ToString("yyyyMMdd")}_{Name}")
        CalomealWebViewWorker.LogFilePath = NoteCalomealWorker.LogFilePath

    End Sub

#Region "Public Method"

    Public Shared Function GetToken(mainModel As QolmsYappliModel, code As String) As String

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_TOKENLOG_FILENAME)

        'Codeから取得
        Dim tokenSet As CalomealAccessTokenSet = CalomealWebViewWorker.GetNewToken(code)

        If Not String.IsNullOrWhiteSpace(tokenSet.access_token) Then


            Dim str() As String = tokenSet.access_token.Split(CChar("."))
            Dim jwt As String = str.GetValue(1).ToString()

            '文字列の長さが4の倍数じゃないとBase64エンコードで引っかかる
            If jwt.Length Mod 4 <> 0 Then
                Dim m As Integer = jwt.Length Mod 4
                '足りない分を空白"="で埋める
                For index As Integer = 1 To 4 - m
                    jwt += "="
                Next

            End If

            Dim enc As New UTF8Encoding
            Dim json As String = enc.GetString(Convert.FromBase64String(jwt))

            Dim sr As New QsJsonSerializer()
            Dim c As CalomealUser = sr.Deserialize(Of CalomealUser)(json)


            'DBに登録
            Dim apiresult As QhYappliPortalCalomealConnectionWriteApiResults = ExecuteCalomealConnectionWriteApi(mainModel, c.sub, tokenSet, False)


            If apiresult.Height.TryToValueType(Decimal.MinValue) > 0 Then
                '一応更新しておく
                mainModel.AuthorAccount.Height = apiresult.Height.TryToValueType(Decimal.MinValue)

            End If

            Dim weight As Decimal = Decimal.Zero
            '食事ボタンが押せる時点で体重の登録はあるはず
            If apiresult.Weight.TryToValueType(Decimal.MinValue) > 0 Then
                weight = apiresult.Weight.TryToValueType(Decimal.MinValue)
            End If
            '初回なのでプロフィール登録する。
            If Not CalomealWebViewWorker.SetProfile(tokenSet.access_token, mainModel.AuthorAccount.Birthday, mainModel.AuthorAccount.Height,
                                                  weight, mainModel.AuthorAccount.SexType) Then
                'error

                AccessLogWorker.WriteErrorLog(Nothing, "api_setprofile_error", New InvalidOperationException(String.Format("カロミルプロフィールの登録に失敗しました。accountkey={0}", mainModel.AuthorAccount.AccountKey)))
            End If
        Else
            '取れなかった場合
            'error
            AccessLogWorker.WriteErrorLog(Nothing, "api_accesstoken_error", New InvalidOperationException(String.Format("カロミルトークンの取得に失敗しました。accountkey={0}", mainModel.AuthorAccount.AccountKey)))
        End If

        Return tokenSet.access_token

    End Function

    Public Shared Function TokenRead(mainModel As QolmsYappliModel) As String

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_TOKENLOG_FILENAME)

        ''DebugLog("TokenRead")

        Dim result As QhYappliPortalCalomealConnectionTokenReadApiResults = NoteCalomealWorker.ExecuteCalomealConnectionTokenReadApi(mainModel)

        ''DebugLog(result.TokenSet.Token)

        If String.IsNullOrWhiteSpace(result.TokenSet.Token) Then

            'DebugLog("トークンなし")
            Return String.Empty

        ElseIf result.TokenSet.TokenExpires.TryToValueType(Date.MinValue) > Date.Now Then
            Return result.TokenSet.Token
        Else
            'DebugLog("GetRefreshToken前")

            Dim tokenSet As CalomealAccessTokenSet = CalomealWebViewWorker.GetRefreshToken(result.TokenSet.RefreshToken)
            'DebugLog("GetRefreshToken後")

            If Not String.IsNullOrWhiteSpace(tokenSet.access_token) Then

                '-------------------

                Dim str() As String = tokenSet.access_token.Split(CChar("."))
                Dim jwt As String = str.GetValue(1).ToString()

                '文字列の長さが4の倍数じゃないとBase64エンコードで引っかかる
                If jwt.Length Mod 4 <> 0 Then
                    Dim m As Integer = jwt.Length Mod 4
                    '足りない分を空白"="で埋める
                    For index As Integer = 1 To 4 - m
                        jwt += "="
                    Next

                End If

                Dim enc As New UTF8Encoding
                Dim json As String = enc.GetString(Convert.FromBase64String(jwt))

                Dim sr As New QsJsonSerializer()
                Dim c As CalomealUser = sr.Deserialize(Of CalomealUser)(json)

                'DebugLog(c.sub)

                '-------------------
                Dim apiresult As QhYappliPortalCalomealConnectionWriteApiResults = NoteCalomealWorker.ExecuteCalomealConnectionWriteApi(mainModel, c.sub, tokenSet, False)

            Else
                '取れなかった場合の処理？
                AccessLogWorker.WriteErrorLog(Nothing, "api_accesstoken_error", New InvalidOperationException(String.Format("カロミルトークンの取得に失敗しました。accountkey={0}", mainModel.AuthorAccount.AccountKey)))

            End If

            Return tokenSet.access_token

        End If

    End Function

    Public Shared Function RefreshToken(mainModel As QolmsYappliModel) As String

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_TOKENLOG_FILENAME)

        'DebugLog("TokenRead")

        Dim result As QhYappliPortalCalomealConnectionTokenReadApiResults = NoteCalomealWorker.ExecuteCalomealConnectionTokenReadApi(mainModel)

        'DebugLog(result.TokenSet.Token)

        If Not String.IsNullOrWhiteSpace(result.TokenSet.RefreshToken) Then

            'DebugLog("GetRefreshToken前")

            Dim tokenSet As CalomealAccessTokenSet = CalomealWebViewWorker.GetRefreshToken(result.TokenSet.RefreshToken)
            'DebugLog("GetRefreshToken後")

            If Not String.IsNullOrWhiteSpace(tokenSet.access_token) Then

                '-------------------

                Dim str() As String = tokenSet.access_token.Split(CChar("."))
                Dim jwt As String = str.GetValue(1).ToString()

                '文字列の長さが4の倍数じゃないとBase64エンコードで引っかかる
                If jwt.Length Mod 4 <> 0 Then
                    Dim m As Integer = jwt.Length Mod 4
                    '足りない分を空白"="で埋める
                    For index As Integer = 1 To 4 - m
                        jwt += "="
                    Next

                End If

                Dim enc As New UTF8Encoding
                Dim json As String = enc.GetString(Convert.FromBase64String(jwt))

                Dim sr As New QsJsonSerializer()
                Dim c As CalomealUser = sr.Deserialize(Of CalomealUser)(json)

                'DebugLog(c.sub)

                '-------------------
                Dim apiresult As QhYappliPortalCalomealConnectionWriteApiResults = NoteCalomealWorker.ExecuteCalomealConnectionWriteApi(mainModel, c.sub, tokenSet, False)

            Else
                '取れなかった場合の処理？
                AccessLogWorker.WriteErrorLog(Nothing, "api_accesstoken_error", New InvalidOperationException(String.Format("カロミルトークンの取得に失敗しました。accountkey={0}", mainModel.AuthorAccount.AccountKey)))

            End If

            Return tokenSet.access_token

        End If

    End Function


    Public Shared Function GetWebViewAuthUrl(token As String, selectDate As Date, meal As Byte) As String

        Dim state As String = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N")
        Dim rootUri As String = REQUEST_URI

        If Not rootUri.EndsWith("/") Then
            rootUri += "/"
        End If

        Dim mealStr As String = String.Empty
        Select Case meal
         'breakfast:朝食, lunch:昼食, dinner:夕食, snack:間食

            Case 1
                mealStr = "breakfast"
            Case 2
                mealStr = "lunch"
            Case 3
                mealStr = "dinner"
            Case 4
                mealStr = "snack"
        End Select


        If String.IsNullOrWhiteSpace(token) Then
            'Tokenなし
            Return String.Format(rootUri + "auth/request?response_type=code&client_id={0}&state={1}&only_create_user=1", CalomealWebViewWorker.CLIENT_ID, state)
        Else
            'Tokenあり
            If String.IsNullOrWhiteSpace(mealStr) Then
                '食事ボタンなのでカロミル食事画面TOPへ
                Return String.Format(rootUri + "web/?client_id={0}&access_token={1}&date={2}&meal_type={3}", CalomealWebViewWorker.CLIENT_ID, token, selectDate.ToString("yyyyMMdd"), mealStr)

            Else
                '目標ボタンなので食事種別を指定して入力画面へ
                Return String.Format(rootUri + "web/meal?client_id={0}&access_token={1}&date={2}&meal_type={3}", CalomealWebViewWorker.CLIENT_ID, token, selectDate.ToString("yyyyMMdd"), mealStr)
            End If

        End If

    End Function

    Public Shared Function History(mainModel As QolmsYappliModel) As Boolean

        Dim updatedDate As Date = Date.MinValue

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)

        With NoteCalomealWorker.ExecuteCalomealMealSyncRead(mainModel) '後でトークン取得を消す（トークンはトークンで取る用のメソッドにまとめた）

            updatedDate = .LastUpdatedDate.TryToValueType(Date.MinValue)

        End With

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME)

        If Not String.IsNullOrWhiteSpace(token) Then
            ' トークンがなければ食事の同期はいらない
            Dim mealEvents As List(Of NoteMeal2InputModel) = CalomealWebViewWorker.GetMeal(updatedDate, token).ConvertAll(Function(i) CalomealWebViewWorker.ToMealEvent2(i, mainModel.AuthorAccount.AccountKey))

            '登録できないデータがないか確認する
            mealEvents = mealEvents.Where(Function(i) i.HistoryId > 0).ToList()
            If mealEvents.Any() Then
                Dim ret As QhYappliNoteCalomealMealSyncWriteApiResults = NoteCalomealWorker.ExecuteCalomealMealSyncWrite(mainModel, mealEvents)

                '登録する
                With ret
                    If ret.IsSuccess.TryToValueType(False) Then

                        '対象のポイントを付与
                        'ループせずに一括登録へ変更
                        Dim pointItemN As New List(Of QolmsPointGrantItem)()
                        Dim hsPointDate As New HashSet(Of Date) '付与対象日が一意になるようにする
                        For Each item As String In .CanGivePointDateN

                            Dim recordDate As Date = item.TryToValueType(Date.MinValue)
                            If recordDate > Date.MinValue AndAlso hsPointDate.Add(recordDate.Date) Then

                                recordDate = recordDate.Date
                                Dim limit As Date = New Date(recordDate.Year, recordDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                                Dim pointMaxDay As Date = Date.Now.Date ' ポイント付与範囲終了日（今日）
                                Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）
                                Dim pointType As QyPointItemTypeEnum = QyPointItemTypeEnum.None

                                If (recordDate >= pointMinDay And recordDate <= pointMaxDay) Then
                                    ' 2022/2/28から食事は種別に関わらず1日1ポイントに変更
                                    pointType = QyPointItemTypeEnum.Meal
                                    pointItemN.Add(New QolmsPointGrantItem(
                                            mainModel.AuthorAccount.MembershipType,
                                            Date.Now,
                                            Guid.NewGuid().ToApiGuidString(),
                                            pointType,
                                            limit,
                                            recordDate
                                        )
                                        )

                                End If

                            End If

                        Next

                        ' 非同期で付与
                        Task.Run(
                            Sub()
                                QolmsPointWorker.AddQolmsPoints(
                                    mainModel.ApiExecutor,
                                    mainModel.ApiExecutorName,
                                    mainModel.SessionId,
                                    mainModel.ApiAuthorizeKey,
                                    mainModel.AuthorAccount.AccountKey,
                                    pointItemN
                                )
                            End Sub
                        )

                    End If

                End With

            End If

            Return True

        End If

        Return False

    End Function

    ''' <summary>
    ''' カロミルWebViewに体重を登録します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    Public Shared Function SetAnthropometric(mainModel As QolmsYappliModel, targetDate As Date, weight As Decimal) As Boolean


        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME)

        If Not String.IsNullOrWhiteSpace(token) AndAlso CalomealWebViewWorker.SetAnthropometric(token, targetDate, weight, 1) Then
            Return True

        End If

        Return False

    End Function


    ''' <summary>
    ''' カロミルWebViewに目標を登録します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    Public Shared Function SetGoal(mainModel As QolmsYappliModel, targetDate As Date, weight As Decimal) As Boolean

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME)

        If Not String.IsNullOrWhiteSpace(token) AndAlso CalomealWebViewWorker.SetGoal(token, targetDate, weight) Then
            Return True

        End If

        Return False

    End Function

    ''' <summary>
    ''' カロミルWebViewに目標を登録します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    Public Shared Function GetMealWithBasis(mainModel As QolmsYappliModel, startDate As Date, endDate As Date) As List(Of CalomealGoalSet)

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME)

        If Not String.IsNullOrWhiteSpace(token) Then

            Dim GoalSet As List(Of CalomealGoalSet) = CalomealWebViewWorker.GetMealWithBasis(token, startDate, endDate).ConvertAll(Function(i) CalomealWebViewWorker.ToCalomealGoalSet(i))
            Return GoalSet
        End If

        Return New List(Of CalomealGoalSet)()

    End Function

    ''' <summary>
    ''' カロミルアドバイス同意画面の呼び出しをするかどうか判定します。
    ''' </summary>
    ''' <param name="challengekey">該当のチャレンジキーを返却します</param>
    ''' <returns></returns>
    Public Shared Function IsCallDynamicLink(mainModel As QolmsYappliModel, ByRef challengekey As Guid?, ByRef linkage As Integer) As Boolean

        '初期値
        challengekey = Nothing
        linkage = Integer.MinValue

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)
        Dim challenge As Dictionary(Of Guid, String) = PortalChallengeWorker.GetChallengeEntryList(mainModel)
        Dim list As Dictionary(Of Integer, ConnectionSettingItem) = PortalConnectionSettiongWorker.GetLinkageLists(mainModel)

        'ログパスをセット
        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_LOG_FILENAME)

        '全部有効な場合は優先順位が高い方から採用されます。
        Select Case True

            Case list.ContainsKey(47100)
                '1) 沖縄セルラー
                linkage = 47100
            Case challenge.ContainsKey(Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1"))
                '2) 竹富町
                challengekey = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1")
            Case challenge.ContainsKey(Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c"))
                '3) 伊平屋村
                challengekey = Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c")

        End Select

        If challengekey <> Guid.Empty OrElse linkage > 0 Then
            'チャレンジ参加者だったら
            Dim info As CalomealInstructInfo = CalomealWebViewWorker.ToCalomealInstructInfo(CalomealWebViewWorker.InstructInfoApi(token))
            If String.IsNullOrWhiteSpace(info.UserName) Then
                'まだ同意してなかったら表示
                Return True
            End If

        End If

        Return False
    End Function


    Public Shared Function DynamicLink(mainModel As QolmsYappliModel, Optional challengeKey As Nullable(Of Guid) = Nothing, Optional linkage As Integer = Integer.MinValue) As String

        Dim jwt As String = String.Empty
        'チャレンジ
        If challengeKey.HasValue Then

            Select Case challengeKey.Value
                Case Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1")
                    '竹富町と連携するためのJWT
                    jwt = TAKETOMI_JWT

                Case Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c")
                    '伊平屋
                    jwt = IHEYA_JWT

            End Select
        End If

        '連携
        Select Case linkage

            Case 47100
                '沖縄セルラーと連携するためのJWT
                jwt = OCT_JWT

        End Select

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)
        If Not String.IsNullOrWhiteSpace(token) AndAlso Not String.IsNullOrWhiteSpace(jwt) Then
            '一応確認するけど、ない人は入らないように実装する
            'Dim state As String = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N")
            Dim rootUri As String = REQUEST_URI

            If Not rootUri.EndsWith("/") Then
                rootUri += "/"
            End If

            Dim str As String = String.Format(rootUri + "web/dynamic_link?client_id={0}&access_token={1}&jwt={2}&name={3}", CalomealWebViewWorker.CLIENT_ID, token, jwt, mainModel.AuthorAccount.Name)
            Return str

        End If

        Return NoteCalomealWorker.GetWebViewAuthUrl(token, Today, 0)

    End Function


    Public Shared Function DynamicLink(mainModel As QolmsYappliModel, jwt As String) As String

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)

        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_LOG_FILENAME)
        CalomealWebViewWorker.FileLog(jwt)

        If Not String.IsNullOrWhiteSpace(token) AndAlso Not String.IsNullOrWhiteSpace(jwt) Then
            '一応確認するけど、ない人は入らないように実装する
            'Dim state As String = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N")
            Dim rootUri As String = REQUEST_URI

            If Not rootUri.EndsWith("/") Then
                rootUri += "/"
            End If

            Dim str As String = String.Format(rootUri + "web/dynamic_link?client_id={0}&access_token={1}&jwt={2}", CalomealWebViewWorker.CLIENT_ID, token, jwt)
            CalomealWebViewWorker.FileLog(str)

            Return str

        End If

        Return NoteCalomealWorker.GetWebViewAuthUrl(token, Today, 0)

    End Function

    Friend Shared Function Hokenshido(mainModel As QolmsYappliModel) As String

        Dim token As String = NoteCalomealWorker.TokenRead(mainModel)

        NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_LOG_FILENAME)

        If Not String.IsNullOrWhiteSpace(token) Then
            '一応確認するけど、ない人は入らないように実装する
            'Dim state As String = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N")
            Dim rootUri As String = REQUEST_URI

            If Not rootUri.EndsWith("/") Then
                rootUri += "/"
            End If

            Dim str As String = String.Format(rootUri + "web/hokenshido?client_id={0}&access_token={1}", CalomealWebViewWorker.CLIENT_ID, token)
            CalomealWebViewWorker.FileLog(str)

            Return str

        End If

        Return NoteCalomealWorker.GetWebViewAuthUrl(token, Today, 0)
    End Function


#End Region

#End Region

End Class

