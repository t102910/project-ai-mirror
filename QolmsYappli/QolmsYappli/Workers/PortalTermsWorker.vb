Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PortalTermsWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region


#Region "Private Method"

    Private Shared Function ExecutePortalTermsReadApi(mainModel As QolmsYappliModel) As QhYappliPortalTermsReadApiResults

        Dim apiArgs As New QhYappliPortalTermsReadApiArgs(
         QhApiTypeEnum.YappliPortalTermsRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString
        }
        Dim apiResults As QhYappliPortalTermsReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalTermsReadApiResults)(
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

#End Region


#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalTermsViewModel
        Dim result As New PortalTermsViewModel(mainModel)

        With PortalTermsWorker.ExecutePortalTermsReadApi(mainModel)

            If .IsSuccess.TryToValueType(False) Then

                result.AcceptDate = Date.Parse(.AcceptDate)

            End If
        End With

        Return result

    End Function
#End Region

End Class
