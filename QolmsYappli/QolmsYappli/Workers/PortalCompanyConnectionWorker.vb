Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsCryptV1


Friend NotInheritable Class PortalCompanyConnectionWorker

#Region "Constant"


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

    Private Shared Function ExecuteCompanyConnectionReadApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As QhYappliPortalCompanyConnectionReadApiResults

        Dim apiArgs As New QhYappliPortalCompanyConnectionReadApiArgs(
            QolmsApiCoreV1.QhApiTypeEnum.YappliPortalCompanyConnectionRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = linkageSystemNo.ToString()
        }
        Dim apiResults As QhYappliPortalCompanyConnectionReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCompanyConnectionReadApiResults)(
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


    Private Shared Function ExecuteCompanyConnectionWriteApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As QhYappliPortalCompanyConnectionWriteApiResults


        Dim apiArgs As New QhYappliPortalCompanyConnectionWriteApiArgs(
            QhApiTypeEnum.YappliPortalCompanyConnectionWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = linkageSystemNo.ToString()
        }
        Dim apiResults As QhYappliPortalCompanyConnectionWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalCompanyConnectionWriteApiResults)(
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, linkageSystemNo As Integer, fromPageNoType As QyPageNoTypeEnum) As PortalCompanyConnectionViewModel

        With PortalCompanyConnectionWorker.ExecuteCompanyConnectionReadApi(mainModel, linkageSystemNo)

            If .IsSuccess.TryToValueType(False) AndAlso .LinkageSystemNo.TryToValueType(Integer.MinValue) > 0 Then

                Dim no As Integer = .LinkageSystemNo.TryToValueType(Integer.MinValue)
                Dim id As String = .LinkageSystemId
                Dim fName As String = .LinkageSystemName
                Dim status As Byte = .StatusType.TryToValueType(Byte.MinValue)
                Dim showType As QyRelationContentTypeEnum = .ShowType.TryToValueType(QyRelationContentTypeEnum.None)

                Return New PortalCompanyConnectionViewModel() With {
                    .FromPageNoType = fromPageNoType,
                    .LinkageSystemNo = no,
                    .LinkageSystemId = id,
                    .LinkageSystemName = fName,
                    .StatusType = status,
                    .ShowType = showType
                }
            End If


        End With
        Return Nothing

    End Function

    ''' <summary>
    ''' 連携を登録します
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function Delete(mainModel As QolmsYappliModel, linkageSystemNo As Integer, ByRef message As String) As Boolean

        With PortalCompanyConnectionWorker.ExecuteCompanyConnectionWriteApi(mainModel, linkageSystemNo)
            If .IsSuccess.TryToValueType(False) Then

                Dim paymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(mainModel)

                If PremiumBizWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, paymentModel) Then

                    Return True
                End If

            End If

        End With

        Return False

    End Function

#End Region


End Class