Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class StartLoginEditWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region


#Region "Private Method"

    Private Shared Function ExecuteStartStartLoginEditReaderApi(mainModel As QolmsYappliModel) As QiQolmsYappliStartLoginEditReadApiResults

        Dim apiArgs As New QiQolmsYappliStartLoginEditReadApiArgs(
            QiApiTypeEnum.QolmsYappliStartLoginEditRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }
        Dim apiResults As QiQolmsYappliStartLoginEditReadApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliStartLoginEditReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ExecuteStartStartLoginEditWriteApi(mainModel As QolmsYappliModel, inputModel As StartLoginEditInputModel) As QiQolmsYappliStartLoginEditWriteApiResults

        Dim apiArgs As New QiQolmsYappliStartLoginEditWriteApiArgs(
            QiApiTypeEnum.QolmsYappliStartLoginEditWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
          mainModel.ApiExecutorName
        ) With {
        .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
        .OpenId = inputModel.OpenId,
        .OpenIdType = inputModel.OpenIdType.ToString(),
        .UserId = IIf(String.IsNullOrWhiteSpace(inputModel.UserId), String.Empty, String.Format("JOTO{0}", inputModel.UserId.Trim())).ToString(),
        .Password = inputModel.Password
        }
        Dim apiResults As QiQolmsYappliStartLoginEditWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliStartLoginEditWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"


    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As StartLoginEditInputModel

        Dim result As New StartLoginEditInputModel()

        With ExecuteStartStartLoginEditReaderApi(mainModel)

            If .IsSuccess.TryToValueType(False) Then

                result.Accountkey = mainModel.AuthorAccount.AccountKey
                If Not String.IsNullOrWhiteSpace(.UserId) Then
                    result.UserId = .UserId.Remove(0, 4)

                End If
                result.Password = String.Empty
                result.Password2 = String.Empty
                result.OpenIdType = .OpenIdType.TryToValueType(Byte.MinValue)
                result.OpenId = .OpenId

            End If

        End With

        Return result

    End Function

    Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As StartLoginEditInputModel, ByRef errorMessage As String) As Boolean

        With ExecuteStartStartLoginEditWriteApi(mainModel, inputModel)

            If .ErrorList.Any() Then
                Dim str As String = String.Empty

                For Each item As String In .ErrorList

                    Select Case item
                        Case "OpenId"

                            str = "このauIDは既に登録されています。"

                        Case "UserId"

                            str = "ユーザーIDが登録できません。"
                    End Select

                    errorMessage += str
                Next

                AuIdLoginWorker.DebugLog(str)

                Return False
            End If

            Return .IsSuccess.TryToValueType(False)

        End With

    End Function

#End Region

End Class
