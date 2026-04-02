Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsApiCoreV1

Public Class PortalHospitalConnectionWorker


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

    Private Shared Function ExecuteHospitalConnectionReadApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As QjPortalHospitalConnectionReadApiResults

        Dim apiArgs As New QjPortalHospitalConnectionReadApiArgs(
            QolmsApiCoreV1.QjApiTypeEnum.PortalHospitalConnectionRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = linkageSystemNo.ToString()
        }

        Dim apiResults As QjPortalHospitalConnectionReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHospitalConnectionReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ExecuteHospitalConnectionWriteApi(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As QjPortalHospitalConnectionWriteApiResults


        Dim apiArgs As New QjPortalHospitalConnectionWriteApiArgs(
            QjApiTypeEnum.PortalHospitalConnectionWrite,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = linkageSystemNo.ToString()
        }
        Dim apiResults As QjPortalHospitalConnectionWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHospitalConnectionWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    'Private Shared Function EncriptString(str As String) As String

    '    If String.IsNullOrWhiteSpace(str) Then Return String.Empty

    '    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
    '        Return crypt.EncryptString(str)
    '    End Using

    'End Function

    'Private Shared Function DecriptString(str As String) As String

    '    If String.IsNullOrWhiteSpace(str) Then Return String.Empty

    '    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
    '        Return crypt.DecryptString(str)
    '    End Using

    'End Function

    'Private Shared Sub SendMail(message As String)

    '    Dim br As New StringBuilder()
    '    br.AppendLine(String.Format("ALKOO接続のエラーです。"))
    '    br.AppendLine(message)
    '    Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(br.ToString())

    'End Sub

    'Private Shared Function ExceptionString(mainModel As QolmsYappliModel, ex As Exception) As String

    '    Dim message As New StringBuilder()
    '    message.AppendLine("ACCOUNTKEY : " & mainModel.AuthorAccount.AccountKey.ToString())
    '    message.AppendLine(ex.Source)

    '    Dim exp As Exception = ex
    '    For i As Integer = 0 To 3
    '        message.AppendLine(ex.Message)
    '        message.AppendLine(ex.StackTrace)

    '        If exp.InnerException Is Nothing Then
    '            Exit For
    '        Else
    '            exp = ex.InnerException
    '        End If
    '    Next

    '    Return message.ToString()

    'End Function

#End Region

#Region "Public Method"
    Shared Function CreateViewModel(mainModel As QolmsYappliModel, linkageSystemNo As Integer, fromPageNoType As QyPageNoTypeEnum) As PortalHospitalConnectionViewModel

        Dim result As New PortalHospitalConnectionViewModel()

        With PortalHospitalConnectionWorker.ExecuteHospitalConnectionReadApi(mainModel, linkageSystemNo)

            result.FromPageNoType = fromPageNoType
            result.LinkageSystemNo = Integer.Parse(.LinkageSystemNo)
            result.LinkageSystemId = .LinkageSystemId
            result.LinkageSystemName = .LinkageSystemName
            result.StatusType = Byte.Parse(.StatusType)
            result.ShowType = .ShowType.TryToValueType(QyRelationContentTypeEnum.None)
            result.DisapprovedReason = .DisapprovedReason

        End With

        Return result

    End Function

    Shared Function Cancel(mainModel As QolmsYappliModel, linkageSystemNo As Integer) As Boolean

        With PortalHospitalConnectionWorker.ExecuteHospitalConnectionWriteApi(mainModel, linkageSystemNo)

            Return .IsSuccess.TryToValueType(False)

        End With

    End Function


#End Region


End Class
