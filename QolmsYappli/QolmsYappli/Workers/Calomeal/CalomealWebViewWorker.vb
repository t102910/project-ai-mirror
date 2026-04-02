Imports System.Runtime.Serialization.Json
Imports MGF.QOLMS.QolmsCalomealWebViewApiCoreV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports System.Globalization
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsJwtAuthCore

Friend NotInheritable Class CalomealWebViewWorker


#Region "Public Prop"

    Public Shared Property LogFilePath As String = String.Empty

#End Region


#Region "Constant"

    ''' <summary>
    ''' カロミル クライアント ID
    ''' </summary>
    Public Shared ReadOnly CLIENT_ID As String = New Lazy(Of String)(Function() GetConfigSettings("CalomealApiClientID")).Value

    ''' <summary>
    ''' カロミル クライアント シークレット
    ''' </summary>
    Public Shared ReadOnly CLIENT_SECRET As String = New Lazy(Of String)(Function() GetConfigSettings("CalomealApiClientSecret")).Value

    Private Shared ReadOnly REDIRECT_URI As String = CalomealWebViewWorker.CreateReturnUrl()

    ''' <summary>
    ''' ReturnUrlに入れるパス（ドメイン以外）
    ''' </summary>
    Private Const RETURN_URL_PASS As String = "note/calomealresult"

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
    ''' Config設定を取得します。
    ''' </summary>
    ''' <param name="settingsName">ConfigのKey名</param>
    ''' <returns>
    ''' Configのvalue値
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetConfigSettings(settingsName As String) As String

        Dim result As String = String.Empty

        If Not String.IsNullOrWhiteSpace(settingsName) Then

            Try
                result = ConfigurationManager.AppSettings(settingsName)
            Catch
            End Try

        End If

        Return result

    End Function

    Private Shared Function CreateReturnUrl() As String

        Dim root As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri")

        If Not root.EndsWith("/") Then
            root += "/"
        End If

        Dim url As String = root + RETURN_URL_PASS

        Return url

    End Function


    ' UNIXエポックを表すDateTimeオブジェクトを取得
    Private Shared UNIX_EPOCH As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)

    Private Shared Function GetUnixTime(ByVal targetTime As DateTime) As Integer

        If targetTime > Date.MinValue Then
            ' UTC時間に変換
            targetTime = targetTime.ToUniversalTime()

            ' UNIXエポックからの経過時間を取得
            Dim elapsedTime As TimeSpan = targetTime - UNIX_EPOCH
            ' 経過秒数に変換
            Return Convert.ToInt32(elapsedTime.TotalSeconds)
        Else
            Return 0
        End If


    End Function


    ''' <summary>
    ''' Fitbitトークンを取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteAccessTokenApi(code As String) As AccessTokenApiResults

        'DebugLog("ExecuteAccessTokenApi")

        Dim apiArgs As New AccessTokenApiArgs() With {
            .grant_type = "authorization_code",
            .client_id = CLIENT_ID,
            .client_secret = CLIENT_SECRET,
            .redirect_uri = REDIRECT_URI,
            .code = code
        }
        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of AccessTokenApiResults)

        Dim callback As Func(Of Task(Of AccessTokenApiResults), AccessTokenApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteAccessTokenApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New AccessTokenApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        Try
            apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of AccessTokenApiArgs, AccessTokenApiResults)(apiArgs, callback)

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, ex.Message)
        End Try

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else

                Try
                    AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, String.Format("statuscod={0},.error={1}", .StatusCode, .error))

                Catch ex As Exception
                    AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, ex.Message)
                    AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, ex.InnerException.Message)
                End Try

                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.AccessTokenApi"))
            End If

        End With

    End Function

    ''' <summary>
    ''' ID トークン更新
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteRefreshTokenApi(refreshToken As String) As AccessTokenApiResults
        'DebugLog("ExecuteRefreshTokenApi")

        Dim apiArgs As New AccessTokenApiArgs() With {
            .grant_type = "refresh_token",
            .client_id = CLIENT_ID,
            .client_secret = CLIENT_SECRET,
            .redirect_uri = REDIRECT_URI,
            .refresh_token = refreshToken
        }

        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of AccessTokenApiResults)

        Dim callback As Func(Of Task(Of AccessTokenApiResults), AccessTokenApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteRefreshTokenApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New AccessTokenApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function
        Try
            apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of AccessTokenApiArgs, AccessTokenApiResults)(apiArgs, callback)

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, ex.Message)
        End Try

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else

                Try
                    AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, String.Format("statuscod={0},.error={1}", .StatusCode, .error))

                Catch ex As Exception
                    AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, ex.Message)
                    AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_accesstoken_error", AccessLogWorker.AccessTypeEnum.Error, ex.InnerException.Message)
                End Try

                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.AccessTokenApi"))
            End If

        End With

    End Function


    ''' <summary>
    ''' 食事履歴の取得
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteMealApi(unixTime As Integer, token As String) As MealApiResults
        'DebugLog("ExecuteMealApi")

        Dim apiArgs As New MealApiArgs() With {
           .updated_at = unixTime,
           .Token = token
        }
        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_meal", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of MealApiResults)
        Dim callback As Func(Of Task(Of MealApiResults), MealApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteMealApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New MealApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of MealApiArgs, MealApiResults)(apiArgs, callback)

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_meal", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.MealApi"))
            End If

        End With

    End Function

    ''' <summary>
    ''' プロフィールを登録します。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Private Shared Function ExecuteSetProfileApi(token As String, birthDay As Date, height As Decimal, weight As Decimal, sex As QsCalomealWebViewApiSexTypeEnum) As SetProfileApiResults


        Dim apiArgs As New SetProfileApiArgs() With {
           .Token = token,
           .BirthDay = birthDay.ToString("yyyy/MM/dd"),
           .Sex = QsCalomealWebViewApiSexTypeEnum.Men.ToString("d"),
           .Height = height,
           .Weight = weight
        }
        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_setprofile", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of SetProfileApiResults)
        Dim callback As Func(Of Task(Of SetProfileApiResults), SetProfileApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteSetProfileApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New SetProfileApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of SetProfileApiArgs, SetProfileApiResults)(apiArgs, callback)

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_setprofile", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.SetProfile"))
            End If

        End With

    End Function

    ''' <summary>
    ''' ユーザー目標設定を登録します。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Private Shared Function ExecuteSetGoalApi(token As String, term As Date, weight As Decimal) As SetGoalApiResults


        Dim apiArgs As New SetGoalApiArgs() With {
           .Token = token,
           .Term = term.ToString("yyyyMMdd"),
           .Weight = weight,
           .Calculationtype = "recommend"
        }
        '目標値以下入れなくていいはずだけどminVal受け付けてくれるかはテスト。
        'decimalで入れていいかテスト。
        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_setgoal", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of SetGoalApiResults)
        Dim callback As Func(Of Task(Of SetGoalApiResults), SetGoalApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteSetGoalApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New SetGoalApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of SetGoalApiArgs, SetGoalApiResults)(apiArgs, callback)

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_setgoal", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.SetGoal"))
            End If

        End With

    End Function

    ''' <summary>
    ''' 食事履歴取得
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Private Shared Function ExecuteMealWithBasisApi(token As String, startDate As Date, endDate As Date) As MealWithBasisResults

        Dim apiArgs As New MealWithBasisApiArgs() With {
           .Token = token,
           .StartDate = startDate.ToString("yyyy/MM/dd"),
           .EndDate = endDate.ToString("yyyy/MM/dd")
        }
        '目標値以下入れなくていいはずだけどminVal受け付けてくれるかはテスト。
        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_mealwithbasis", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of MealWithBasisResults)
        Dim callback As Func(Of Task(Of MealWithBasisResults), MealWithBasisResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteMealWithBasisApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New MealWithBasisResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of MealWithBasisApiArgs, MealWithBasisResults)(apiArgs, callback)

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_mealwithbasis", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.MealWithBasis"))
            End If

        End With

    End Function


    ''' <summary>
    ''' 体重、体脂肪率設定を登録します。
    ''' </summary>
    ''' <param name="token"></param>
    Private Shared Function ExecuteSetAnthropometricApi(token As String, targetDate As Date, weight As Decimal, section As Integer) As SetAnthropometricApiResults
        'fat As Decimal,
        Dim apiArgs As New SetAnthropometricApiArgs() With {
            .Token = token,
            .list = New List(Of SetAnthropometricOfJson) From {
                New SetAnthropometricOfJson() With {
                    .date = targetDate.ToString("yyyy/MM/dd"),
                    .weight = weight,
                    .section = section
                }
            }
        }
        '.Fat = 25D, Nothing指定すると0になる　=指定なし、ができるかどうか確認、なければ0でいいか確認。
        'decimalで入れていいかテスト。JSONにすると小数点消えるらしい、　"[{\"date\":\"2023\\/09\\/12\",\"fat\":25,\"section\":1,\"weight\":60}]"
        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_setanthropometric", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of SetAnthropometricApiResults)
        Dim callback As Func(Of Task(Of SetAnthropometricApiResults), SetAnthropometricApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteSetAnthropometricApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New SetAnthropometricApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of SetAnthropometricApiArgs, SetAnthropometricApiResults)(apiArgs, callback)

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_setanthropometric", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.SetAnthropometric"))
            End If

        End With

    End Function

    ''' <summary>
    ''' カロミルアドバイスの同意を取得します。。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Private Shared Function ExecuteInstructInfoApi(token As String) As InstructInfoApiResults

        Dim apiArgs As New InstructInfoApiArgs() With {
           .Token = token
        }

        Dim request As String = MakeCyptDataString(apiArgs)
        'DebugLog(request)
        AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_instructinfo", AccessLogWorker.AccessTypeEnum.Api, request)

        Dim apiResults As Threading.Tasks.Task(Of InstructInfoApiResults)
        Dim callback As Func(Of Task(Of InstructInfoApiResults), InstructInfoApiResults) =
            Function(antecedent)

                Dim gid As Guid = Guid.NewGuid()
                FileLog($" {gid}: ExecuteInstructInfoApi")
                If antecedent.Status = Threading.Tasks.TaskStatus.RanToCompletion Then

                    If antecedent.Result.IsSuccess Then
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")

                    Else
                        FileLog($" {gid}: {antecedent.Result.IsSuccess}")
                        FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}")
                        FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}")

                    End If

                ElseIf antecedent.Status = TaskStatus.Faulted Then
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.Message}")
                    FileLog($" {gid}: {antecedent.Exception.GetBaseException.StackTrace}")
                    Return New InstructInfoApiResults() With {.[error] = antecedent.Exception.GetBaseException.Message}
                End If

                Return antecedent.Result

            End Function

        apiResults = QsCalomealWebViewApiManager.ExecuteAsync(Of InstructInfoApiArgs, InstructInfoApiResults)(apiArgs, callback)

        With apiResults.Result
            'DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, "/note/calomeal/api_instructinfo", AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults.Result
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "QolmsCalomealWebViewApiCoreV1.InstructInfo"))
            End If

        End With

    End Function

    ''' <summary>
    ''' リクエスト引数を暗号化した文字列を作成します
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Private Shared Function TestMakeCyptDataString(args As Object) As String
        Try
            Using ms As New IO.MemoryStream()
                With New DataContractJsonSerializer(args.GetType)
                    .WriteObject(ms, args)

                    Dim json As String = Encoding.UTF8.GetString(ms.ToArray())
                    Return Encoding.UTF8.GetString(ms.ToArray())
                End With
            End Using
        Catch ex As Exception
        End Try
        Return ""
    End Function

    ''' <summary>
    ''' リクエスト引数を暗号化した文字列を作成します
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Private Shared Function MakeCyptDataString(args As QsCalomealWebViewApiArgsBase) As String
        Try
            Using crypt As New QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem)
                Using ms As New IO.MemoryStream()
                    With New DataContractJsonSerializer(args.GetType)
                        .WriteObject(ms, args)

                        Return crypt.EncryptString(Encoding.UTF8.GetString(ms.ToArray()))
                    End With
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return ""
    End Function
    'テスト用の手抜きログ吐き
    '<Conditional("DEBUG")>
    'Public Shared Sub DebugLog(ByVal message As String)
    '    Try
    '        Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "Calomeal.Log")
    '        System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
    '    Catch ex As Exception
    '    End Try

    'End Sub

    ''' <summary>
    ''' 本番出力用ログ
    ''' </summary>
    ''' <param name="message"></param>
    Public Shared Sub FileLog(ByVal message As String)
        Try
            Dim log As String = CalomealWebViewWorker.LogFilePath
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub
#End Region

#Region "Public Method"

    ''' <summary>
    ''' ユーザー認証コードからアクセストークンを発行する
    ''' </summary>
    ''' <param name="code"></param>
    ''' <returns></returns>
    Public Shared Function GetNewToken(code As String) As CalomealAccessTokenSet

        With CalomealWebViewWorker.ExecuteAccessTokenApi(code)

            If .IsSuccess Then

                Dim tokenSet As New CalomealAccessTokenSet()
                tokenSet.access_token = .access_token
                tokenSet.expires_in = .expires_in
                tokenSet.RefreshTokenExpires = Date.MaxValue
                tokenSet.refresh_token = .refresh_token
                tokenSet.TokenExpires = Date.Now.AddSeconds(Integer.Parse(.expires_in))
                tokenSet.token_type = .token_type

                Return tokenSet

            End If

        End With

        Return New CalomealAccessTokenSet()

    End Function

    ''' <summary>
    ''' フレッシュトークンからアクセストークンを再発行する
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="refreshToken"></param>
    ''' <returns></returns>
    Public Shared Function GetRefreshToken(refreshToken As String) As CalomealAccessTokenSet

        With CalomealWebViewWorker.ExecuteRefreshTokenApi(refreshToken)

            If .IsSuccess Then

                Dim tokenSet As New CalomealAccessTokenSet()
                tokenSet.access_token = .access_token
                tokenSet.expires_in = .expires_in
                tokenSet.RefreshTokenExpires = Date.MaxValue
                tokenSet.refresh_token = refreshToken
                tokenSet.TokenExpires = Date.Now.AddSeconds(Integer.Parse(.expires_in))
                tokenSet.token_type = .token_type

                Return tokenSet

            End If

        End With

        Return New CalomealAccessTokenSet()

    End Function
    ''' <summary>
    ''' 指定されたアクセストークンに該当するユーザーの食事履歴を取得する
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="updatedDate"></param>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Public Shared Function GetMeal(updatedDate As Date, token As String) As List(Of MealHistoriesOfJson)

        Dim unixTime As Integer = GetUnixTime(updatedDate)

        With CalomealWebViewWorker.ExecuteMealApi(unixTime, token)
            If .IsSuccess Then

                Return .meal_histories
            End If

        End With

        Return New List(Of MealHistoriesOfJson)

    End Function

    ''' <summary>
    ''' 指定されたアクセストークンに該当するユーザーのプロフィール情報を設定する
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="birthDay"></param>
    ''' <param name="height"></param>
    ''' <param name="weight"></param>
    ''' <param name="sex"></param>
    ''' <returns></returns>
    Public Shared Function SetProfile(token As String, birthDay As Date, height As Decimal, weight As Decimal, sex As QySexTypeEnum) As Boolean

        Try

            With CalomealWebViewWorker.ExecuteSetProfileApi(token, birthDay, height, weight, sex.ToString("d").TryToValueType(Of QsCalomealWebViewApiSexTypeEnum)(0))

                Return .IsSuccess AndAlso .StatusCode = 200

            End With

        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(Nothing, "/note/calomeal/api_setprofile_error", ex.Message)
        End Try
        Return False

    End Function

    ''' <summary>
    ''' 指定されたアクセストークンに該当するユーザーの体重、体脂肪率の記録、目標から算出した摂取基準、および食事履歴を日単位に取得する
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="startDate"></param>
    ''' <param name="endDate"></param>
    ''' <returns></returns>
    Public Shared Function GetMealWithBasis(token As String, startDate As Date, endDate As Date) As List(Of MealWithBasisOfJson)

        Try
            With CalomealWebViewWorker.ExecuteMealWithBasisApi(token, startDate, endDate)
                If .IsSuccess AndAlso .StatusCode = 200 Then

                    Return .meal_with_basis
                End If

            End With

        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(Nothing, "/note/calomeal/api_mealwithbasis_error", ex.Message)
        End Try
        Return New List(Of MealWithBasisOfJson)

    End Function

    ''' <summary>
    ''' 指定されたアクセストークンに該当するユーザーの体重、体脂肪率を追加する
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="targetDate"></param>
    ''' <param name="weight"></param>
    ''' <param name="section"></param>
    ''' <returns></returns>
    Public Shared Function SetAnthropometric(token As String, targetDate As Date, weight As Decimal, section As Integer) As Boolean
        ' fat As Decimal,
        Try
            With CalomealWebViewWorker.ExecuteSetAnthropometricApi(token, targetDate, weight, section)

                Return .IsSuccess AndAlso .StatusCode = 200

            End With

        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(Nothing, "/note/calomeal/api_setanthropometric_error", ex.Message)
        End Try

        Return False

    End Function

    ''' <summary>
    ''' 指定されたアクセストークンに該当するユーザーの体重、体脂肪率を追加する
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="weight"></param>
    Public Shared Function SetGoal(token As String, tarmDate As Date, weight As Decimal) As Boolean
        ' fat As Decimal,
        Try
            With CalomealWebViewWorker.ExecuteSetGoalApi(token, tarmDate, weight)

                Return .IsSuccess AndAlso .StatusCode = 200

            End With

        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(Nothing, "/note/calomeal/api_setgoal_error", ex.Message)
        End Try

        Return False

    End Function


    ''' <summary>
    ''' アドバイス
    ''' カロミルAPIを呼び出します。
    ''' </summary>
    ''' <param name="token"></param>
    Public Shared Function InstructInfoApi(token As String) As InstructInfoOfJson

        Try
            With CalomealWebViewWorker.ExecuteInstructInfoApi(token)

                Return .info

            End With

        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(Nothing, "/note/calomeal/api_instructinfo_error", ex.Message)
        End Try

        Return New InstructInfoOfJson()

    End Function

    ''' <summary>
    ''' 食事区分の文字列からEnumへ変換します。
    ''' </summary>
    ''' <param name="target"></param>
    ''' <returns></returns>
    Private Shared Function toMealTypeEnum(target As String) As QyMealTypeEnum

        Select Case target

            Case "朝食"
                Return QyMealTypeEnum.Breakfast

            Case "昼食"
                Return QyMealTypeEnum.Lunch

            Case "夕食"
                Return QyMealTypeEnum.Dinner

            Case "間食"
                Return QyMealTypeEnum.Snacking

            Case Else
                Return QyMealTypeEnum.None

        End Select

    End Function

    ''' <summary>
    ''' カロミルの食事JosnをJOTOの食事登録モデルに変換します。
    ''' </summary>
    ''' <param name="target">MealHistoriesOfJson</param>
    ''' <param name="accountkey">コンバート失敗エラー登録用のアカウントキー</param>
    ''' <returns></returns>
    Public Shared Function ToMealEvent2(target As MealHistoriesOfJson, accountkey As Guid) As NoteMeal2InputModel

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Dim ci As CultureInfo = CultureInfo.CurrentCulture
        Dim dts As DateTimeStyles = DateTimeStyles.None
        Dim sr As New QsJsonSerializer


        Dim RecordDate As Date = Date.MinValue
        Date.TryParseExact(target.meal_date, "yyyy/MM/dd", ci, dts, RecordDate)
        If RecordDate > Date.MinValue Then
            RecordDate = RecordDate.AddHours(target.hour.TryToValueType(Integer.MinValue))
        End If

        'Nullでもせいこう
        Dim cal As Short = Short.MinValue
        If target.calorie Is Nothing OrElse Short.TryParse(target.calorie, cal) Then

            'Rateは後で入るかも
            Return New NoteMeal2InputModel() With {
                .AnalysisSet = sr.Serialize(target).ToString(),
                .AnalysisType = 5,
                .Calorie = target.calorie.TryToValueType(Short.MinValue),
                .ChooseSet = sr.Serialize(target).ToString(),
                .DeleteFlag = target.is_deleted.TryToValueType(False),
                .HistoryId = target.meal_history_id.TryToValueType(Integer.MinValue),
                .ItemName = IIf(target.menu_name Is Nothing, String.Empty, target.menu_name).ToString(),
                .MealType = IIf(target.meal_type Is Nothing, 0, Convert.ToByte(CalomealWebViewWorker.toMealTypeEnum(target.meal_type))).ToString().TryToValueType(Byte.MinValue),
                .Rate = 1,
                .RecordDate = RecordDate,
                .HasImage = target.has_image.TryToValueType(False)
            }

        Else

            AccessLogWorker.WriteErrorLog(Nothing, "/note/calomeal/api_meal_error", New InvalidOperationException(String.Format("カロミル食事の変換に失敗しました。accountkey={0},historyid={1},dataset={2}", accountkey, target.meal_history_id, sr.Serialize(target).ToString())))
            Return New NoteMeal2InputModel()

        End If

    End Function


    ''' <summary>
    ''' カロミルの食事JosnをJOTOの食事登録モデルに変換します。
    ''' </summary>
    ''' <param name="target">MealHistoriesOfJson</param>
    ''' <returns></returns>
    Public Shared Function ToCalomealGoalSet(target As MealWithBasisOfJson) As CalomealGoalSet

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New CalomealGoalSet() With {
            .TargetDate = target.date.TryToValueType(Date.MinValue),
            .BasisAllCalorie = target.basis.all.calorie.TryToValueType(Integer.MinValue)
        }

    End Function

    ''' <summary>
    ''' に変換します。
    ''' </summary>
    ''' <param name="target">InstructInfoOfJson</param>
    ''' <returns></returns>
    Public Shared Function ToCalomealInstructInfo(target As InstructInfoOfJson) As CalomealInstructInfo

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New CalomealInstructInfo() With {
            .StoreName = target.store_name,
            .UserName = target.user_name
        }

    End Function

#End Region


#Region "呼び出しテスト"


    ''' <summary>
    ''' </summary>
    Public Shared Function TestOpenPushApi(mainModel As QolmsYappliModel) As String

        Dim jwt As String = New QsJwtTokenProvider().CreateOpenApiJwtApiKey("OyW2F2CHReoBvSNY7BKUWz8MzvkvxKW39z1FZLZggScuwVbOryEvJnh8lqIjlXr9", 1024, New Date(2050, 12, 31, 23, 59, 59))

        Dim apiArgs As New QoJotoHdrPushSendApiArgs(
           QoApiTypeEnum.JotoHdrPushSend,
           QsApiSystemTypeEnum.Qolms,
           Guid.Parse("FF728404-0069-1901-0000-000000047015"),
           "TzgTkJTHH0mWYGKOERp8vQ=="
           ) With {
                .Message = "テスト用Push通知です",
                .LinkageSystemNo = "47015",
                .LinkageId = "2303417",
                .DeeplinkUrl = "jotohdr:/tab/custom/5d67c170?url=https%3A%2F%2Fjoto.qolms.com%2Fstart%2Fcalomealdynamiclink%3Fjwt%3DeyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL3Rlc3QtaG9rZW5zaGlkby1hcHAuY2Fsb21lYWwuY29tIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LWhva2Vuc2hpZG8tYXBwLmNhbG9tZWFsLmNvbSIsImlhdCI6MTcyOTUwMTIyMywibmJmIjoxNzI5NTAxMjIzLCJzdWIiOiIzbStVazZlTjNtYm9zNmQ4RE1KKzR3bnZNRXdnZWc0dzhVbm4xeFNXdnRNNWt0dTdrQWR2YWFyOFJwYjh2MnVBK0dCRS9hM0RnUFUyT3RUMzN0VG1hRjdIMjAyZ211QkhMVXR3YnFBaE1UODk2UkZlOXRyeGppWDBhOUdreDhGWmQ0THcyajI0ZEFOSU1rVG9UU1ZGcnc9PSIsImp0aSI6IjZiMGZiYWQwOWViZTM5ZjI3N2IxZDk1ZTk5NWUwMDc1YTI0MThkODM4OTkyNjg4Yjk1YzliMDdhZTM0M2M3NWQifQ.erPbkQXczfY_HgHlv0FvZ6MBz-pvRH4ck5jcV8Beptg"
        }

        Dim apiResult As QoJotoHdrPushSendApiResults = QsApiManager.ExecuteQolmsOpenApi(Of QoJotoHdrPushSendApiResults)(
            apiArgs,
            String.Empty,
            jotoApiKey:=jwt
            )

        With apiResult
            Return $"{apiResult.Result.Code} {apiResult.Result.Detail}"

        End With
    End Function


#End Region

End Class

