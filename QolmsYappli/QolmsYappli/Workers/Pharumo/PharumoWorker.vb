Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsPharumoApiCoreV1
Imports System.Globalization
Imports System.Runtime.Serialization.Json

Friend NotInheritable Class PharumoWorker

#Region "Constant"

    '連携システム番号
    Private Const PHARMO_SYSTEM_NO As Integer = 47009
    Private Shared ReadOnly CLIENT_KEY As String = ConfigurationManager.AppSettings("PharumoApiPublicKey")
    Private Shared ReadOnly CLIENT_SECRET As String = ConfigurationManager.AppSettings("PharumoApiSecretKey")

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
    ''' ファルモ新規ユーザー登録
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePharumoNewTokenApi(mainModel As QolmsYappliModel, name As String, birthday As Date, sex As QySexTypeEnum) As PharumoNewTokenApiResults

        DebugLog("ExecutePharumoNewTokenApi")

        Dim apiArgs As New PharumoNewTokenApiArgs() With {
            .new_token = New PharumoNewTokenOfJson() With {
                .client_key = CLIENT_KEY,
                .client_secret = CLIENT_SECRET,
                .resource = New PharumoResourceOfJson() With {
                .type = "User",
                .attrs = New PharumoResourceAttrsOfJson() With {
                            .birthday = birthday.ToString("yyyy-MM-dd"),
                            .gender_type = sex.ToString().ToLower(),
                            .name = name
                        }
                    }
                }
        }
        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As PharumoNewTokenApiResults = QsPharumoApiManager.Execute(Of PharumoNewTokenApiArgs, PharumoNewTokenApiResults)(apiArgs)

        With apiResults
            DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "PharumoNewToken"))
            End If

        End With

    End Function

    ''' <summary>
    ''' ID トークン更新
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePharumoUpdateTokenApi(token As String, refreshToken As String) As PharumoUpdateTokenApiResults
        DebugLog("ExecutePharumoUpdateTokenApi")


        Dim apiArgs As New PharumoUpdateTokenApiArgs() With {
            .update_token = New PharumoUpdateTokenOfJson() With {
                .id_token = token,
                .nonce = "",
                .options = "",
                .refresh_token = refreshToken
            }
        }
        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As PharumoUpdateTokenApiResults = QsPharumoApiManager.Execute(Of PharumoUpdateTokenApiArgs, PharumoUpdateTokenApiResults)(apiArgs)

        With apiResults

            DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "PharumoUpdateToken"))
            End If

        End With

    End Function

    ''' <summary>
    ''' ID トークン再発行
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePharumoReissueTokenApi(token As String, refreshToken As String) As PharumoReissueTokenApiResults
        DebugLog("ExecutePharumoReissueTokenApi")

        Dim apiArgs As New PharumoReissueTokenApiArgs() With {
            .reissue_token = New PharumoReissueTokenOfJson() With {
                .client_key = CLIENT_KEY,
                .client_secret = CLIENT_SECRET,
                .id_token = token
            }
        }
        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As PharumoReissueTokenApiResults = QsPharumoApiManager.Execute(Of PharumoReissueTokenApiArgs, PharumoReissueTokenApiResults)(apiArgs)

        With apiResults
            DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)

            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "PharumoReissueToken"))
            End If

        End With

    End Function

    ''' <summary>
    ''' お薬自動登録 詳細取得
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePharumoGetAutomaticPrescriptionApi(token As String, pharmacyId As Integer) As PharumoGetAutomaticPrescriptionApiResults
        DebugLog("ExecutePharumoGetAutomaticPrescriptionApi")

        Dim apiArgs As New PharumoGetAutomaticPrescriptionApiArgs() With {
            .id_token = token,
            .pharmacy_id = pharmacyId
        }

        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As PharumoGetAutomaticPrescriptionApiResults = QsPharumoApiManager.Execute(Of PharumoGetAutomaticPrescriptionApiArgs, PharumoGetAutomaticPrescriptionApiResults)(apiArgs)

        With apiResults
            DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)

            If .IsSuccess Then
                Return apiResults
            Else
                DebugLog(apiResults.StatusCode.ToString)

                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "PharumoGetAutomaticPrescription"))
            End If

        End With

    End Function

    ''' <summary>
    ''' お薬自動登録 登録
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePharumoRegisterAutomaticPrescriptionApi(token As String, pharmacyId As Integer, receiptCode As String) As PharumoRegisterAutomaticPrescriptionApiResults
        DebugLog("ExecutePharumoRegisterAutomaticPrescriptionApi")

        Dim apiArgs As New PharumoRegisterAutomaticPrescriptionApiArgs() With {
            .id_token = token,
            .pharmacy = New PharumoAutomaticPrescriptionOfJson() With {
                .pharmacy_id = pharmacyId,
                .receipt_code = receiptCode
            }
        }

        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As PharumoRegisterAutomaticPrescriptionApiResults = QsPharumoApiManager.Execute(Of PharumoRegisterAutomaticPrescriptionApiArgs, PharumoRegisterAutomaticPrescriptionApiResults)(apiArgs)

        With apiResults

            DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)

            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "PharumoRegisterAutomaticPrescription"))
            End If

        End With

    End Function


    ''' <summary>
    ''' お薬自動登録 登録
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePharumoDeleteAutomaticPrescriptionApi(token As String, pharmacyId As Integer) As PharumoDeleteAutomaticPrescriptionApiResults
        DebugLog("ExecutePharumoDeleteAutomaticPrescriptionApi")

        Dim apiArgs As New PharumoDeleteAutomaticPrescriptionApiArgs() With {
            .id_token = token,
            .pharmacy_id = pharmacyId
        }

        DebugLog(MakeCyptDataString(apiArgs))
        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, MakeCyptDataString(apiArgs))

        Dim apiResults As PharumoDeleteAutomaticPrescriptionApiResults = QsPharumoApiManager.Execute(Of PharumoDeleteAutomaticPrescriptionApiArgs, PharumoDeleteAutomaticPrescriptionApiResults)(apiArgs)

        With apiResults

            DebugLog(.ResponseString)
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, .ResponseString)

            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", "PharumoDeleteAutomaticPrescription"))
            End If

        End With

    End Function

    Private Shared Function MakeCyptDataString(args As QsPharumoApiArgsBase) As String
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
    <Conditional("DEBUG")> _
    Private Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "Pharumo.Log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub

#End Region

#Region "Public Method"

    Public Shared Function PharumoConnection(mainModel As QolmsYappliModel, name As String, birthday As Date, sexType As QySexTypeEnum, pharmacyId As Integer, patientCardNo As String, ByRef tokenSet As PharumoTokenSet, ByRef message As String) As Boolean

        DebugLog(String.Format("token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires))
        DebugLog(String.Format("pharmacy_id:{0}", pharmacyId))

        Dim logMessage As String = String.Empty
        Dim now As Date = Date.Now
        DebugLog(String.Format("NOW:{0}", now))
        ' リフレッシュトークン有効期限変換用
        Dim expires As Date = Date.MinValue
        Dim ci As CultureInfo = CultureInfo.CurrentCulture
        Dim dts As DateTimeStyles = DateTimeStyles.None

        If String.IsNullOrWhiteSpace(tokenSet.Token) AndAlso String.IsNullOrWhiteSpace(tokenSet.RefreshToken) Then

            DebugLog(String.Format("トークンがない場合"))
            ' トークンがない場合、ユーザー新規登録へ
            Try
                ' ユーザー登録API
                With PharumoWorker.ExecutePharumoNewTokenApi(mainModel, name, birthday, sexType)
                    If .IsSuccess Then

                        tokenSet.Token = .id_token
                        tokenSet.RefreshToken = .refresh_token.token
                        '成功したらトークンの有効期限を更新
                        tokenSet.TokenExpires = Date.Now.AddHours(1)

                        If Date.TryParse(.refresh_token.expires_in, expires) AndAlso expires > Date.MinValue Then
                            tokenSet.RefreshTokenExpires = expires
                            DebugLog(String.Format("refreshExpires:{0}", tokenSet.RefreshTokenExpires))
                        End If
                    Else
                        logMessage = String.Format("ResponseString:{0}", .ResponseString)
                        message = "ユーザー登録に失敗しました。"
                    End If

                End With
            Catch ex As Exception

                logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                message = "ユーザー登録に失敗しました。"

            End Try
        Else

            DebugLog(String.Format("トークンがある場合"))
            If tokenSet.TokenExpires < now Then
                DebugLog(String.Format("トークン有効期限切れ"))

                ' (トークンが有効期限内?)
                If now < tokenSet.RefreshTokenExpires Then

                    DebugLog(String.Format("リフレッシュトークンが有効期限内"))
                    ' リフレッシュトークンが有効期限内
                    ' ID トークン 更新
                    Try
                        With PharumoWorker.ExecutePharumoUpdateTokenApi(tokenSet.Token, tokenSet.RefreshToken)

                            If .IsSuccess Then

                                tokenSet.Token = .id_token
                                tokenSet.RefreshToken = .refresh_token.token
                                '成功したらトークンの有効期限を更新
                                tokenSet.TokenExpires = Date.Now.AddHours(1)

                                If Date.TryParse(.refresh_token.expires_in, expires) AndAlso expires > Date.MinValue Then
                                    tokenSet.RefreshTokenExpires = expires
                                    DebugLog(String.Format("refreshExpires:{0}", tokenSet.RefreshTokenExpires))
                                End If

                            Else
                                logMessage = String.Format("ResponseString:{0}", .ResponseString)
                                message = "トークンの更新に失敗しました。"
                            End If

                        End With

                    Catch ex As Exception

                        logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                        message = "トークンの更新に失敗しました。"
                    End Try

                Else
                    DebugLog(String.Format("リフレッシュトークンの有効期限切れ"))
                    ' トークンの有効期限切れ
                    Try

                        With PharumoWorker.ExecutePharumoReissueTokenApi(tokenSet.Token, tokenSet.RefreshToken)

                            If .IsSuccess Then

                                tokenSet.Token = .id_token
                                tokenSet.RefreshToken = .refresh_token.token
                                '成功したらトークンの有効期限を更新
                                tokenSet.TokenExpires = Date.Now.AddHours(1)

                                If Date.TryParse(.refresh_token.expires_in, expires) AndAlso expires > Date.MinValue Then
                                    tokenSet.RefreshTokenExpires = expires
                                    DebugLog(String.Format("refreshExpires:{0}", tokenSet.RefreshTokenExpires))
                                End If

                            Else
                                logMessage = String.Format("ResponseString:{0}", .ResponseString)
                                message = "トークンの再発行に失敗しました。"

                            End If

                        End With
                    Catch ex As Exception

                        logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                        message = "トークンの再発行に失敗しました。"
                    End Try

                End If

            End If

        End If

        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, String.Format("token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires))

        If Not String.IsNullOrWhiteSpace(tokenSet.Token) AndAlso String.IsNullOrWhiteSpace(logMessage) Then
            ' トークンが取得できたら、薬局登録へ

            DebugLog(String.Format("トークンが取得できたら、薬局登録"))
            ' 薬局登録

            Try
                ' お薬自動登録API
                With PharumoWorker.ExecutePharumoRegisterAutomaticPrescriptionApi(tokenSet.Token, pharmacyId, patientCardNo)
                    If .IsSuccess Then
                        '.patient.status_type
                        ' 成功
                        Return True
                    Else
                        logMessage = String.Format("ResponseString:{0}", .ResponseString)
                        message = "お薬自動登録に失敗しました。"

                    End If

                End With

            Catch ex As Exception

                logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                message = "お薬自動登録に失敗しました。"
            End Try

        End If

        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, logMessage)
        Return False

    End Function

    Public Shared Function Cancel(mainModel As QolmsYappliModel, pharmacyId As Integer, ByRef tokenSet As PharumoTokenSet, ByRef message As String) As Boolean

        '外でTokenとかの引数が正しいか確認してから入るようにしてください。
        DebugLog(String.Format("token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires))
        DebugLog(String.Format("pharmacy_id:{0}", pharmacyId))

        Dim logMessage As String = String.Empty
        Dim now As Date = Date.Now
        DebugLog(String.Format("NOW:{0}", now))

        ' リフレッシュトークン有効期限変換用
        Dim expires As Date = Date.MinValue
        Dim ci As CultureInfo = CultureInfo.CurrentCulture
        Dim dts As DateTimeStyles = DateTimeStyles.None

        If tokenSet.TokenExpires < now Then
            DebugLog(String.Format("トークン有効期限切れ"))
            If now < tokenSet.RefreshTokenExpires Then

                DebugLog(String.Format("リフレッシュトークンが有効期限内"))
                ' リフレッシュトークンが有効期限内
                ' ID トークン 更新
                Try
                    With PharumoWorker.ExecutePharumoUpdateTokenApi(tokenSet.Token, tokenSet.RefreshToken)

                        If .IsSuccess Then

                            tokenSet.Token = .id_token
                            tokenSet.RefreshToken = .refresh_token.token
                            '成功したらトークンの有効期限を更新
                            tokenSet.TokenExpires = Date.Now.AddHours(1)

                            If Date.TryParse(.refresh_token.expires_in, expires) AndAlso expires > Date.MinValue Then
                                tokenSet.RefreshTokenExpires = expires
                                DebugLog(String.Format("refreshExpires:{0}", tokenSet.RefreshTokenExpires))
                               
                            End If

                        Else
                            logMessage = String.Format("ResponseString:{0}", .ResponseString)
                            message = "トークンの更新に失敗しました。"

                        End If

                    End With
                Catch ex As Exception
                    logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                    message = "トークンの更新に失敗しました。"

                End Try

            Else
                DebugLog(String.Format("リフレッシュトークンの有効期限切れ"))
                Try
                    ' トークンの有効期限切れ
                    With PharumoWorker.ExecutePharumoReissueTokenApi(tokenSet.Token, tokenSet.RefreshToken)

                        If .IsSuccess Then

                            tokenSet.Token = .id_token
                            tokenSet.RefreshToken = .refresh_token.token
                            '成功したらトークンの有効期限を更新
                            tokenSet.TokenExpires = Date.Now.AddHours(1)

                            If Date.TryParse(.refresh_token.expires_in, expires) AndAlso expires > Date.MinValue Then
                                tokenSet.RefreshTokenExpires = expires
                                DebugLog(String.Format("refreshExpires:{0}", tokenSet.RefreshTokenExpires))
                     
                            End If

                        Else
                            logMessage = String.Format("ResponseString:{0}", .ResponseString)
                            message = "トークンの再発行に失敗しました。"

                        End If

                    End With

                Catch ex As Exception
                    logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                    message = "トークンの再発行に失敗しました。"

                End Try
            End If

        End If

        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Api, String.Format("token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires))
        '薬局連携削除
        DebugLog(String.Format("薬局連携削除"))
        If Not String.IsNullOrWhiteSpace(tokenSet.Token) AndAlso String.IsNullOrWhiteSpace(logMessage) Then
            Try

                With PharumoWorker.ExecutePharumoDeleteAutomaticPrescriptionApi(tokenSet.Token, pharmacyId)

                    If .IsSuccess Then
                        Return .IsSuccess
                    Else
                        logMessage = String.Format("ResponseString:{0}", .ResponseString)
                        message = "お薬自動登録の削除に失敗しました。"
                    End If

                End With
            Catch ex As Exception
                logMessage = String.Format("ErrorMessage:{0}", ex.Message)
                message = "お薬自動登録の削除に失敗しました。"

            End Try

        End If

        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, logMessage)
        Return False

    End Function
#End Region

End Class

