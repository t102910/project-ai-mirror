Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsCryptV1

Friend NotInheritable Class PortalCompanyConnectionEditWorker

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

    Private Shared Function ExecuteCompanyConnectionEditReadApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As QhYappliPortalCompanyConnectionEditReadApiResults


        Dim apiArgs As New QhYappliPortalCompanyConnectionEditReadApiArgs(
            QhApiTypeEnum.YappliPortalCompanyConnectionEditRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = linkageSystemNo.ToString()
        }
        Dim apiResults As QhYappliPortalCompanyConnectionEditReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCompanyConnectionEditReadApiResults)(
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

    Private Shared Function ExecuteCompanyConnectionEditWriteApi(mainModel As QolmsYappliModel, model As PortalCompanyConnectionEditInputModel) As QhYappliPortalCompanyConnectionEditWriteApiResults

        Dim apiArgs As New QhYappliPortalCompanyConnectionEditWriteApiArgs(
            QhApiTypeEnum.YappliPortalCompanyConnectionEditWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = model.LinkageSystemNo.ToString(),
            .RelationContentType = Convert.ToInt64(model.RelationContentFlags).ToString(),
            .MailAddress = model.MailAddress
        }
        Dim apiResults As QhYappliPortalCompanyConnectionEditWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCompanyConnectionEditWriteApiResults)(
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, linkageSystemNo As Integer, fromPageNoType As QyPageNoTypeEnum) As PortalCompanyConnectionEditInputModel

        Dim result As New PortalCompanyConnectionEditInputModel()
        Dim hiddenMailaddresSetting As String = ConfigurationManager.AppSettings("CompanyConnectionEditHiddenMailaddressCsv")

        Dim hiddenMailaddres As List(Of String) = hiddenMailaddresSetting.Split(","c).ToList()

        With PortalCompanyConnectionEditWorker.ExecuteCompanyConnectionEditReadApi(mainModel, linkageSystemNo)

            If .IsSuccess.TryToValueType(False) AndAlso .LinkageSystemNo.TryToValueType(Integer.MinValue) > 0 Then

                result.FromPageNoType = fromPageNoType
                result.LinkageSystemNo = .LinkageSystemNo.TryToValueType(Integer.MinValue)
                result.LinkageSystemId = .LinkageSystemId
                result.LinkageSystemName = .LinkageSystemName
                result.CompanyConnectedFlag = .StatusType.TryToValueType(Byte.MinValue) = 2
                result.RelationContentFlags = .ShowType.TryToValueType(QyRelationContentTypeEnum.None)
                result.MailAddress = IIf(hiddenMailaddres.IndexOf(.MailAddress) < 0, .MailAddress, String.Empty).ToString()

            End If

        End With

        Return result

    End Function

    ''' <summary>
    ''' 連携を登録します
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="model"></param>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function Edit(mainModel As QolmsYappliModel, model As PortalCompanyConnectionEditInputModel, ByRef linkageSystemNo As Integer, ByRef message As String) As Boolean

        With PortalCompanyConnectionEditWorker.ExecuteCompanyConnectionEditWriteApi(mainModel, model)
            If .IsSuccess.TryToValueType(False) AndAlso .ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None) = QsApiCompanyConnectionRequestErrorTypeEnum.None AndAlso .LinkageSystemNo.TryToValueType(Integer.MinValue) > 0 Then

                linkageSystemNo = .LinkageSystemNo.TryToValueType(Integer.MinValue)
                Return True

            Else

                Select Case .ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None)

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.NotEmployeeExists
                        message = ERROR_NOT_EMPLOYEE_EXISTS

                    Case QsApiCompanyConnectionRequestErrorTypeEnum.LinkageRegisterFaild
                        message = ERROR_LINKAGE_REGISTER_FAILD

                    Case Else
                        message = ERROR_UNKNOWN

                End Select

                Return False

            End If

        End With


    End Function

#End Region


End Class