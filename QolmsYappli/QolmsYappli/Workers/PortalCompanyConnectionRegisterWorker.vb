Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsCryptV1


Friend NotInheritable Class PortalCompanyConnectionRequestWorker

#Region "Constant"

    Private Const ERROR_NOT_EMPLOYEE_EXISTS As String = "従業員が存在しません。"
    Private Const ERROR_NO_IDENTIFICATION As String = "従業員情報が一致しません。"
    Private Const ERROR_LINKAGE_REGISTER_FAILD As String = "企業連携の登録に失敗しました。"
    Private Const ERROR_RELATION_REGISTER_FAILD As String = "従業員連携の登録に失敗しました。"
    Private Const ERROR_EMPLOYEE_NAME_IDENTIFICATION_REGISTER_FAILD As String = "従業員情報の変換に失敗しました。"
    Private Const ERROR_UNKNOWN As String = "不明なエラーです。"

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

    Private Shared Function ExecuteCompanyConnectionRequestWriteApi(mainModel As QolmsYappliModel, model As PortalCompanyConnectionRequestInputModel) As QhYappliPortalCompanyConnectionRequestWriteApiResults

        Dim birthday As Date = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))

        Dim apiArgs As New QhYappliPortalCompanyConnectionRequestWriteApiArgs(
            QhApiTypeEnum.YappliPortalCompanyConnectionRequestWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .FacilityId = model.FacilityId,
            .LinkageSystemId = model.EmployeeNo,
            .FamilyName = model.FamilyName,
            .GivenName = model.GivenName,
            .FamilyKanaName = model.FamilyKanaName,
            .GivenKanaName = model.GivenKanaName,
            .SexType = model.SexType.ToString(),
            .Birtyday = birthday.ToApiDateString(),
        .RelationContentType = model.RelationContentFlags.ToString()
        }
        Dim apiResults As QhYappliPortalCompanyConnectionRequestWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCompanyConnectionRequestWriteApiResults)(
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

    ''テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/Log"), "Alkoo.log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 画面情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, fromPageNoType As QyPageNoTypeEnum) As PortalCompanyConnectionRequestInputModel

        Dim premium As PaymentInf = PremiumWorker.GetPremiumRecord(mainModel)

        Return New PortalCompanyConnectionRequestInputModel() With {
            .FromPageNoType = fromPageNoType,
            .FacilityId = String.Empty,
            .EmployeeNo = String.Empty,
            .FamilyName = mainModel.AuthorAccount.FamilyName,
            .GivenName = mainModel.AuthorAccount.GivenName,
            .FamilyKanaName = mainModel.AuthorAccount.FamilyKanaName,
            .GivenKanaName = mainModel.AuthorAccount.GivenKanaName,
            .SexType = mainModel.AuthorAccount.SexType,
            .BirthYear = mainModel.AuthorAccount.Birthday.Year.ToString(),
            .BirthMonth = mainModel.AuthorAccount.Birthday.Month.ToString(),
            .BirthDay = mainModel.AuthorAccount.Birthday.Day.ToString(),
        .MembershipType = CType(premium.MemberShipType, QyMemberShipTypeEnum)
        }

    End Function

    ''' <summary>
    ''' 連携を登録します
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="model"></param>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function Request(mainModel As QolmsYappliModel, model As PortalCompanyConnectionRequestInputModel, ByRef linkageSystemNo As Integer, ByRef message As String) As Boolean

        With PortalCompanyConnectionRequestWorker.ExecuteCompanyConnectionRequestWriteApi(mainModel, model)
            If .IsSuccess.TryToValueType(False) AndAlso .ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None) = QsApiCompanyConnectionRequestErrorTypeEnum.None AndAlso .LinkageSystemNo.TryToValueType(Integer.MinValue) > 0 Then

                Dim payment As PaymentInf = PremiumWorker.GetPremiumRecord(mainModel)

                If model.FacilityId = "47500107" Then
                    payment.MemberShipType = QyMemberShipTypeEnum.BusinessFree
                Else
                    payment.MemberShipType = QyMemberShipTypeEnum.Business
                End If

                payment.ContinueAccountStartDate = Date.Now
                payment.StartDate = Date.Now
                payment.EndDate = Date.MaxValue
                payment.PaymentType = 255

                'プレミアム会員登録
                If PremiumBizWorker.UpdatePremiumRecordToSuccessPremiumRegist(mainModel, payment) Then
                    linkageSystemNo = .LinkageSystemNo.TryToValueType(Integer.MinValue)

                    '成功したらアカウント情報(名前)の更新
                    mainModel.AuthorAccount.FamilyName = model.FamilyName
                    mainModel.AuthorAccount.GivenName = model.GivenName
                    mainModel.AuthorAccount.FamilyKanaName = model.FamilyKanaName
                    mainModel.AuthorAccount.GivenKanaName = model.GivenKanaName

                    Return True

                End If

            Else
                'エラー
                Select Case .ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None)

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.NotEmployeeExists
                        message = ERROR_NOT_EMPLOYEE_EXISTS

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.NoIdentification
                        message = ERROR_NO_IDENTIFICATION

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.LinkageRegisterFaild
                        message = ERROR_LINKAGE_REGISTER_FAILD

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.RelationRegisterFaild
                        message = ERROR_RELATION_REGISTER_FAILD

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.EmployeeNameIdentificationRegisterFaild
                        message = ERROR_EMPLOYEE_NAME_IDENTIFICATION_REGISTER_FAILD

                    Case Else
                        message = ERROR_UNKNOWN

                End Select

                Return False

            End If

        End With


    End Function

#End Region


End Class