Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsPharumoApiCoreV1
Imports System.Globalization
Imports System.Runtime.Serialization.Json
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Threading.Tasks

Friend NotInheritable Class PortalMedicineConnectionRequestWorker

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

    Private Shared Function ExecutePortalMedicineConnectionRequestPharumoTokenReadApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As QhYappliPortalMedicineConnectionRequestPharumoTokenReadApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionRequestPharumoTokenReadApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionRequestPharumoTokenRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .LinkageSystemNo = linkageSystemNo.ToString()
            }

        Dim apiResults As QhYappliPortalMedicineConnectionRequestPharumoTokenReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionRequestPharumoTokenReadApiResults)(
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

    Private Shared Function ExecutePortalMedicineConnectionRequestWriteApi(mainModel As QolmsYappliModel, model As PortalMedicineConnectionRequestInputModel, tokenSet As PharumoTokenSet) As QhYappliPortalMedicineConnectionRequestWriteApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionRequestWriteApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionRequestWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .BirthDay = (New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))).ToApiDateString(),
                .Facilitykey = model.FacilityKey.ToString(),
                .FamilyName = model.FamilyName,
                .GivenName = model.GivenName,
                .IdentityUpdateFlag = model.IdentityUpdateFlag.ToString(),
                .LinkageSystemNo = model.LinkageSystemNo.ToString(),
                .PatientCardNo = model.PatientCardNo,
                .SexType = model.SexType.ToString(),
                .TokenSet = New QhApiPharumoTokenSetItem() With {
                    .Token = tokenSet.Token,
                    .TokenExpires = tokenSet.TokenExpires.ToApiDateString(),
                    .RefreshToken = tokenSet.RefreshToken,
                    .RefreshTokenExpires = tokenSet.RefreshTokenExpires.ToApiDateString()
                }
            }

        Dim apiResults As QhYappliPortalMedicineConnectionRequestWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionRequestWriteApiResults)(
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, linkageSystemNo As Integer, facilityKey As Guid, fromPageNo As QyPageNoTypeEnum) As PortalMedicineConnectionRequestInputModel

        Dim result As New PortalMedicineConnectionRequestInputModel()
        '施設キー施設名をキャッシュから取得できるようにする
        Dim model As PortalMedicineConnectionSearchViewModel = mainModel.GetInputModelCache(Of PortalMedicineConnectionSearchViewModel)()

        If model IsNot Nothing AndAlso model.MedicineConnectionFacilityItemN.Count > 0 Then

            If model.MedicineConnectionFacilityItemN.ContainsKey(facilityKey) Then

                Dim facility As MedicineConnectionFacilityItem = model.MedicineConnectionFacilityItemN(facilityKey)

                result.FromPageNoType = fromPageNo
                '氏名、生年月日、性別をメインモデルから取得
                result.BirthDay = mainModel.AuthorAccount.Birthday.Day.ToString()
                result.BirthMonth = mainModel.AuthorAccount.Birthday.Month.ToString()
                result.BirthYear = mainModel.AuthorAccount.Birthday.Year.ToString()
                result.FacilityKey = facility.FacilityKey
                result.FacilityName = facility.Name
                result.FamilyName = mainModel.AuthorAccount.FamilyName
                result.GivenName = mainModel.AuthorAccount.GivenName
                result.IdentityUpdateFlag = False
                result.LinkageSystemNo = PHARMO_SYSTEM_NO
                result.PatientCardNo = String.Empty
                result.PharmacyId = facility.PharmacyId
                result.SexType = mainModel.AuthorAccount.SexType

            End If

        End If

        'キャッシュが取得できなかった場合、ConnectionSettingにリダイレクトさせる

        Return result

    End Function


    ''' <summary>
    ''' 薬局連携確認画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Request(mainModel As QolmsYappliModel, model As PortalMedicineConnectionRequestInputModel, ByRef message As String) As Boolean

        ' DBのトークンを取得
        Dim rokenResult As QhYappliPortalMedicineConnectionRequestPharumoTokenReadApiResults
        Try
            rokenResult = PortalMedicineConnectionRequestWorker.ExecutePortalMedicineConnectionRequestPharumoTokenReadApi(mainModel, PHARMO_SYSTEM_NO)
        Catch ex As Exception

            message = "トークンの取得に失敗しました。"
        End Try

        Dim token As String = rokenResult.Token
        Dim tokenExpires As Date = Date.Parse(rokenResult.TokenExpires)
        Dim refreshToken As String = rokenResult.RefreshToken
        Dim refreshTokenExpires As Date = Date.Parse(rokenResult.RefreshTokenExpires)

        'DebugLog(String.Format("token:{0}", token))
        'DebugLog(String.Format("tokenExpires:{0}", tokenExpires))
        'DebugLog(String.Format("refreshToken:{0}", refreshToken))
        'DebugLog(String.Format("refreshTokenExpires:{0}", refreshTokenExpires))

        Dim tokenSet As New PharumoTokenSet() With {
            .Token = token,
            .TokenExpires = tokenExpires,
            .RefreshToken = refreshToken,
            .RefreshTokenExpires = refreshTokenExpires
        }

        ' トークンがない場合、ユーザー新規登録へ
        Dim birthday As Date = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))
        Dim name As String = String.Format("{0} {1}", model.FamilyName, model.GivenName)

        Dim tokenRegisterFlag As Boolean = False '登録が必要（エラーの場合）にTrue
        If PharumoWorker.PharumoConnection(mainModel, name, birthday, model.SexType, model.PharmacyId, model.PatientCardNo, tokenSet, message) Then
            DebugLog("PharumoConnection:true")
            DebugLog(String.Format("refreshTokenExpires:{0}", tokenSet.RefreshTokenExpires))
            Try
                Dim writeResult As QhYappliPortalMedicineConnectionRequestWriteApiResults = PortalMedicineConnectionRequestWorker.ExecutePortalMedicineConnectionRequestWriteApi(mainModel, model, tokenSet)
                DebugLog(String.Format("ExecutePortalMedicineConnectionRequestWriteApi:{0}", writeResult.IsSuccess))

                With writeResult
                    '成功したらアカウント情報(名前)の更新
                    mainModel.AuthorAccount.FamilyName = model.FamilyName
                    mainModel.AuthorAccount.GivenName = model.GivenName

                End With
 
                Return writeResult.IsSuccess.TryToValueType(False)

            Catch ex As Exception

                message = "データ登録処理に失敗しました。"
                tokenRegisterFlag = True
                Dim errorMessage As String = String.Format("薬局データの登録に失敗しました。token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}/accountkey:{4}/LinkageSystemNo:{5}/PatientCardNo:{6}/FacilityKey:{7}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires, mainModel.AuthorAccount.AccountKey, model.LinkageSystemNo, model.PatientCardNo, model.FacilityKey)

                PortalMedicineConnectionRequestWorker.SendMail(EncriptString(errorMessage))

                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, errorMessage)

            End Try

        Else
            tokenRegisterFlag = True
        End If

        If tokenRegisterFlag AndAlso tokenSet.Token <> token AndAlso tokenSet.TokenExpires > tokenExpires Then
            ' 登録に失敗したけどトークンの更新がある場合
            DebugLog("UpdatePharumoUserToken")
            Try
                PortalMedicineConnectionWorker.UpdatePharumoUserToken(mainModel, PHARMO_SYSTEM_NO, tokenSet)

            Catch ex As Exception
                Dim errorMessage As String = String.Format("トークンの登録/更新に失敗しました。token:{0}/token_expires:{1}/refreshToken:{2}/refreshToken_expires:{3}/accountkey:{4}/LinkageSystemNo:{5}", tokenSet.Token, tokenSet.TokenExpires, tokenSet.RefreshToken, tokenSet.RefreshTokenExpires, mainModel.AuthorAccount.AccountKey, model.LinkageSystemNo)

                PortalMedicineConnectionRequestWorker.SendMail(EncriptString(errorMessage))
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, errorMessage)
            End Try
        End If

        DebugLog("PharumoConnection:false")
        Return False

    End Function

#End Region

End Class