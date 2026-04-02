Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsKaradaKaruteApiCoreV1
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Threading

Friend NotInheritable Class PortalMedicineConnectionWorker

#Region "Constant"

    '連携システム番号
    Private Const PHARMO_SYSTEM_NO As Integer = 47009
    'Private Shared ReadOnly JOB_URL As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJob")
    'Private Shared ReadOnly JOB_ACCOUNT As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJobAccount")
    'Private Shared ReadOnly JOB_PASSWORD As String = ConfigurationManager.AppSettings("TanitaConnectionFirstJobPassword")
    'Dim url As String = "https://devqolmsjotohdr.scm.azurewebsites.net/api/triggeredwebjobs/QolmsKaradaKaruteFirstJob/run"
    'Dim jobAccount As String = "$devqolmsjotohdr"
    'Dim jobPassword As String = "TbMEDvyGgxl2wHG83ghuaCGgB8dT6xKy7Yeoa6yujr8YSeRe67uFs9vdlD6Q"


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


    Private Shared Function ExecutePortalMedicineConnectionReadApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer, facilitykey As Guid) As QhYappliPortalMedicineConnectionReadApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionReadApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .linkageSystemNo = linkageSystemNo.ToString(),
                .facilitykey = facilitykey.ToString()
            }

        Dim apiResults As QhYappliPortalMedicineConnectionReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionReadApiResults)(
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


    Private Shared Function ExecutePortalMedicineConnectionCancelReadApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer, facilitykey As Guid) As QhYappliPortalMedicineConnectionCancelReadApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionCancelReadApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionCancelRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .LinkageSystemNo = linkageSystemNo.ToString(),
                .Facilitykey = facilitykey.ToString()
            }

        Dim apiResults As QhYappliPortalMedicineConnectionCancelReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionCancelReadApiResults)(
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


    Private Shared Function ExecutePortalMedicineConnectionWriteApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer, facilitykey As Guid, tokenSet As PharumoTokenSet) As QhYappliPortalMedicineConnectionWriteApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionWriteApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .LinkageSystemNo = linkageSystemNo.ToString(),
                .Facilitykey = facilitykey.ToString(),
                .TokenSet = New QhApiPharumoTokenSetItem() With {
                    .Token = tokenSet.Token,
                    .TokenExpires = tokenSet.TokenExpires.ToApiDateString(),
                    .RefreshToken = tokenSet.RefreshToken,
                    .RefreshTokenExpires = tokenSet.RefreshTokenExpires.ToApiDateString()
                }
            }

        Dim apiResults As QhYappliPortalMedicineConnectionWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionWriteApiResults)(
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


    Private Shared Function ExecutePortalMedicineConnectionTokenWriteApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer, tokenSet As PharumoTokenSet) As QhYappliPortalMedicineConnectionTokenWriteApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionTokenWriteApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionTokenWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .LinkageSystemNo = linkageSystemNo.ToString(),
                .TokenSet = New QhApiPharumoTokenSetItem() With {
                    .Token = tokenSet.Token,
                    .TokenExpires = tokenSet.TokenExpires.ToApiDateString(),
                    .RefreshToken = tokenSet.RefreshToken,
                    .RefreshTokenExpires = tokenSet.RefreshTokenExpires.ToApiDateString()
                }
            }

        Dim apiResults As QhYappliPortalMedicineConnectionTokenWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionTokenWriteApiResults)(
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

    Private Shared Function EncriptString(str As String) As String

        If String.IsNullOrWhiteSpace(str) Then Return String.Empty

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            Return crypt.EncryptString(str)
        End Using

    End Function

    'Private Shared Function DecriptString(str As String) As String

    '    If String.IsNullOrWhiteSpace(str) Then Return String.Empty

    '    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
    '        Return crypt.DecryptString(str)
    '    End Using

    'End Function

    Private Shared Sub SendMail(message As String)

        Dim br As New StringBuilder()
        br.AppendLine(String.Format("ファルモ登録エラーです。"))
        br.AppendLine(message)
        br.AppendLine("※Tokenの更新は毎回確認する")
        br.AppendLine("Tokenが更新されている/DB登録されていない場合：QH_PHARUMOTOKENMANAGEMENT_DAT")
        br.AppendLine(" Tokenの有効期限は発行から1時間")
        br.AppendLine(" RefreshTokenの有効期限は発行から半年間")
        br.AppendLine("Token新規作成の場合：QH_LINKAGE_DAT、QH_PATIENTCARD_DATの新規作成")
        br.AppendLine("薬局の登録に失敗の場合：該当のfacilitykeyのQH_PHARUMOTOKENMANAGEMENT_DATの新規作成")
        br.AppendLine("薬局の削除に失敗の場合：該当のfacilitykeyのQH_PHARUMOTOKENMANAGEMENT_DATの行を削除")

        Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(br.ToString())

    End Sub

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

    ''' <summary>
    ''' 薬局連携確認画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, linkageSystemNo As Integer, facilitykey As Guid, fromPageNoType As QyPageNoTypeEnum) As PortalMedicineConnectionViewModel

        Dim result As New PortalMedicineConnectionViewModel()

        With PortalMedicineConnectionWorker.ExecutePortalMedicineConnectionReadApi(mainModel, linkageSystemNo, facilitykey)

            result.FromPageNoType = fromPageNoType
            result.LinkageSystemNo = .LinkageSystemNo.TryToValueType(Integer.MinValue)
            result.FacilityKey = .Facilitykey.TryToValueType(Guid.Empty)
            result.FacilityName = .FacilityName
            result.StatusType = .StatusType.TryToValueType(Byte.MinValue)
            Return result

        End With

    End Function

    ''' <summary>
    ''' 薬局連携確認画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Cancel(mainModel As QolmsYappliModel, linkageSystemNo As Integer, facilitykey As Guid, ByRef message As String) As Boolean


        ' 連携を解除するのに必要な情報を取得
        ' Token、RefreshToken、pharmacyid

        With PortalMedicineConnectionWorker.ExecutePortalMedicineConnectionCancelReadApi(mainModel, linkageSystemNo, facilitykey)
            Dim token As String = .Token
            Dim tokenExpires As Date = Date.Parse(.TokenExpires)
            Dim refreshToken As String = .RefreshToken
            Dim refreshTokenExpires As Date = Date.Parse(.RefreshTokenExpires)

            DebugLog(String.Format("token:{0}", token))
            DebugLog(String.Format("tokenExpires:{0}", tokenExpires))
            DebugLog(String.Format("refreshToken:{0}", refreshToken))
            DebugLog(String.Format("refreshTokenExpires:{0}", refreshTokenExpires))

            Dim tokenSet As New PharumoTokenSet() With {
                .Token = token,
                .TokenExpires = tokenExpires,
                .RefreshToken = refreshToken,
                .RefreshTokenExpires = refreshTokenExpires
            }

            If Not String.IsNullOrWhiteSpace(.Token) AndAlso Not String.IsNullOrWhiteSpace(.RefreshToken) Then
                Dim tokenRegisterFlag As Boolean = False '登録が必要（エラーの場合）にTrue

                If PharumoWorker.Cancel(mainModel, .PharmacyId.TryToValueType(Integer.MinValue), tokenSet, message) Then
                    DebugLog(String.Format("refreshTokenExpires:{0}", tokenSet.RefreshTokenExpires))

                    Try
                        With PortalMedicineConnectionWorker.ExecutePortalMedicineConnectionWriteApi(mainModel, linkageSystemNo, facilitykey, tokenSet)
                            DebugLog(.IsSuccess)

                            Return .IsSuccess.TryToValueType(False)

                        End With

                    Catch ex As Exception
                        message = "データ登録処理に失敗しました。"

                        Dim errorMessage As String = String.Format("薬局データの削除に失敗しました。token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}/accountkey:{4}/LinkageSystemNo:{5}/FacilityKey:{6}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires, mainModel.AuthorAccount.AccountKey, linkageSystemNo, facilitykey)

                        PortalMedicineConnectionWorker.SendMail(EncriptString(errorMessage))

                        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, errorMessage)
                        tokenRegisterFlag = True
                    End Try

                Else
                    tokenRegisterFlag = True

                End If

                If tokenRegisterFlag AndAlso tokenSet.Token <> token AndAlso tokenSet.TokenExpires > tokenExpires Then
                    ' 登録に失敗したけどトークンの更新がある場合
                    Try
                        PortalMedicineConnectionWorker.ExecutePortalMedicineConnectionTokenWriteApi(mainModel, linkageSystemNo, tokenSet)

                    Catch ex As Exception
                        message = "トークンの登録に失敗しました。"

                        Dim errorMessage As String = String.Format("トークンの登録/更新に失敗しました。token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}/accountkey:{4}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires, mainModel.AuthorAccount.AccountKey)

                        PortalMedicineConnectionWorker.SendMail(EncriptString(errorMessage))
                        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, errorMessage)
                    End Try

                End If

            End If

        End With

        Return False

    End Function

    Public Shared Function UpdatePharumoUserToken(mainModel As QolmsYappliModel, linkageSystemNo As Integer, tokenSet As PharumoTokenSet) As Boolean

        With PortalMedicineConnectionWorker.ExecutePortalMedicineConnectionTokenWriteApi(mainModel, linkageSystemNo, tokenSet)
            Return .IsSuccess.TryToValueType(False)
        End With

    End Function

#End Region

End Class