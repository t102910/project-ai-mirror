Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsFitbitApiCoreV1
Imports System.Globalization
Imports System.Runtime.Serialization.Json
Imports System.Security.Policy

Friend NotInheritable Class FitbitWorker

#Region "Constant"

    '連携システム番号
    Private Const FITBIT_LINKAGESYSTEMNO As Integer = 47011
    Private Shared ReadOnly CLIENT_ID As String = ConfigurationManager.AppSettings("FitbitApiCliantId")
    Private Shared ReadOnly CLIENT_SECRET As String = ConfigurationManager.AppSettings("FitbitApiSecretKey")


    Private Shared ReadOnly REDIRECT_URI As String = FitbitWorker.CreateReturnUrl()

    ''' <summary>
    ''' ReturnUrlに入れるパス（ドメイン以外）
    ''' </summary>
    Private Const RETURN_URL_PASS As String = "portal/fitbitconnectionresult"

    Private Const CODE_VERIFIER As String = "aaa28746e5ae45448bbff1dacdeb7b015c44ab70c8fc41019db24104cb337301"

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

    Private Shared Function CreateReturnUrl() As String

        Dim root As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri")

        If Not root.EndsWith("/") Then
            root += "/"
        End If

        Dim url As String = root + RETURN_URL_PASS

        Return url

    End Function

    ''' <summary>
    ''' Fitbitトークンを取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteFitbitTokenApi(code As String, redirectUrl As String) As FitbitOAth2TokenApiResults

        DebugLog("ExecuteFitbitNewTokenApi")

        Dim apiArgs As New FitbitOAth2TokenApiArgs() With {
            .code = code,
            .grant_type = "authorization_code",
            .client_id = CLIENT_ID,
            .redirect_uri = redirectUrl,
            .expected_in = 28800,
            .code_verifier = QsFitbitApiManager.GetCodeChallenge(CODE_VERIFIER),
        .authorization = QsFitbitApiManager.GetAuthorizationString()
        }
        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As FitbitOAth2TokenApiResults = QsFitbitApiManager.Execute(Of FitbitOAth2TokenApiArgs, FitbitOAth2TokenApiResults)(apiArgs)

        With apiResults
            DebugLog(String.Format("StatuCode={0},ResponseString={1},isSuccess={2}",.StatusCode,.ResponseString,.IsSuccess))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "FitbitNewToken"))
            End If

        End With

    End Function

    ''' <summary>
    ''' ID トークン更新
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteFitbitRefreshTokenApi(refreshToken As String) As FitbitRefreshTokenApiResults
        DebugLog("ExecuteFitbitRefreshTokenApi")


        Dim apiArgs As New FitbitRefreshTokenApiArgs() With {
            .authorization = QsFitbitApiManager.GetAuthorizationString(),
            .expected_in = 28800,
            .grant_type = "refresh_token",
            .refresh_token = refreshToken
            }

        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As FitbitRefreshTokenApiResults = QsFitbitApiManager.Execute(Of FitbitRefreshTokenApiArgs, FitbitRefreshTokenApiResults)(apiArgs)

        With apiResults

            DebugLog(String.Format("StatuCode={0},ResponseString={1},isSuccess={2}",.StatusCode,.ResponseString,.IsSuccess))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "FitbitRefreshToken"))
            End If

        End With

    End Function


    ''' <summary>
    ''' ID トークンの取り消し
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteFitbitRevokeTokenApi(token As FitbitTokenSet) As FitbitRevokeTokenApiResults
        DebugLog("ExecuteFitbitReissueTokenApi")

        Dim tokenStr As String = IIf(token.TokenExpires > Date.Now, token.Token, token.RefreshToken).ToString()

        Dim apiArgs As New FitbitRevokeTokenApiArgs() With {
            .authorization = QsFitbitApiManager.GetAuthorizationString,
            .client_id = CLIENT_ID,
            .token = tokenStr
        }
        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As FitbitRevokeTokenApiResults = QsFitbitApiManager.Execute(Of FitbitRevokeTokenApiArgs, FitbitRevokeTokenApiResults)(apiArgs)

        With apiResults
            DebugLog(String.Format("StatuCode={0},ResponseString={1},isSuccess={2}",.StatusCode,.ResponseString,.IsSuccess))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)

            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "FitbitRevokeToken"))
            End If

        End With

    End Function

    Private Shared Function MakeCyptDataString(args As QsFitbitApiArgsBase) As String
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
            '
        End Try
        Return ""
    End Function

    'テスト用の手抜きログ吐き
    Private Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "Fitbit.Log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub

#End Region

#Region "Public Method"

    'スコープを外から渡すかどうか
    Public Shared Function GetFitbitAuthorizationUrl() As String

        Dim scorp As QsFitbitApiScorpTypeEnum = QsFitbitApiScorpTypeEnum.Activity Or QsFitbitApiScorpTypeEnum.Heartrate

        Dim url As String = QsFitbitApiManager.GetFitbitAuthorizationUrl(CODE_VERIFIER, scorp, REDIRECT_URI)

        Return url
    End Function


    Public Shared Function GetFitbitToken(code As String, redirectUrl As String) As FitbitTokenSet

        Dim result As New FitbitTokenSet()

        With FitbitWorker.ExecuteFitbitTokenApi(code, REDIRECT_URI)

            If .IsSuccess Then
                Dim res As String = .ResponseString
                DebugLog(res)

                result.RefreshToken = .refresh_token
                result.RefreshTokenExpires = Date.MaxValue
                result.Token = .access_token
                result.TokenExpires = Date.Now.AddHours(8)
                result.user_id = .user_id
                result.token_type = .token_type

            End If
        End With

        Return result

    End Function

    Public Shared Function RevokeFitbitToken(ByRef tokenSet As FitbitTokenSet) As Boolean

        'If tokenSet.TokenExpires < Date.Now Then
        '    With FitbitWorker.ExecuteFitbitRefreshTokenApi(tokenSet.RefreshToken)

        '        tokenSet.Token = .access_token
        '        tokenSet.TokenExpires = Date.Now.AddHours(8)
        '        tokenSet.RefreshToken = .refresh_token
        '        tokenSet.RefreshTokenExpires = Date.MaxValue
        '        tokenSet.user_id = .user_id
        '        tokenSet.token_type = .token_type

        '    End With
        'End If
        Try 
                    
            With FitbitWorker.ExecuteFitbitRevokeTokenApi(tokenSet)

                Return .StatusCode = 200 ' 成功かどうか

            End With

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, ex.Message)
        End Try    

        Return False

    End Function

#End Region

End Class

