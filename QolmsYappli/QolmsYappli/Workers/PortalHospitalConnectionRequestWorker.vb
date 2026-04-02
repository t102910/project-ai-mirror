Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsCryptV1


Friend NotInheritable Class PortalHospitalConnectionRequestWorker


#Region "Constant"

    Shared ReadOnly HOSPITAL_ENABLE_LINKAGELIST As List(Of Integer) = New List(Of Integer) From {47106, 47000016, 47000020, 47500110}

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

    Private Shared Function ExecuteHospitalConnectionRequestReadApi(mainModel As QolmsYappliModel, LinkageSystemNo As Integer) As QjPortalHospitalConnectionRequestReadApiResults

        Dim apiArgs As New QjPortalHospitalConnectionRequestReadApiArgs(
            QolmsApiCoreV1.QjApiTypeEnum.PortalHospitalConnectionRequestRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
        .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
        .LinkageSystemNo = LinkageSystemNo.ToString()}


        Dim apiResults As QjPortalHospitalConnectionRequestReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHospitalConnectionRequestReadApiResults)(
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


    Private Shared Function ExecuteHospitalConnectionRequestWriteApi(mainModel As QolmsYappliModel, model As PortalHospitalConnectionRequestInputModel) As QjPortalHospitalConnectionRequestWriteApiResults

        Dim birthday As Date = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))

        Dim apiArgs As New QjPortalHospitalConnectionRequestWriteApiArgs(
            QjApiTypeEnum.PortalHospitalConnectionRequestWrite,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .LinkageSystemNo = model.LinkageSystemNo.ToString(),
            .LinkageSystemId = model.LinkageSystemId,
            .FamilyName = model.FamilyName,
            .GivenName = model.GivenName,
            .FamilyKanaName = model.FamilyKanaName,
            .GivenKanaName = model.GivenKanaName,
            .SexType = model.SexType.ToString(),
            .BirthDay = birthday.ToApiDateString(),
            .MailAddress = model.MailAddress,
            .IdentityUpdateFlag = model.IdentityUpdateFlag.ToString(),
            .RelationContentType = model.RelationContentFlags.ToString()
        }
        Dim apiResults As QjPortalHospitalConnectionRequestWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalHospitalConnectionRequestWriteApiResults)(
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

    ''' <summary>
    ''' 設定を取得します。
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting() As List(Of Integer)

        Dim result As New List(Of Integer)()
        Dim value As String = ConfigurationManager.AppSettings("HospitalEnableLinkageList")

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            Dim arr As String() = value.Split(","c)

            For Each item As String In arr
                Dim outint As Integer = Integer.MinValue

                If Integer.TryParse(item, outint) Then
                    result.Add(outint)
                End If

            Next

        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    Shared Function CreateViewModel(mainModel As QolmsYappliModel, LinkageSystemNo As Integer, fromPageNoType As QyPageNoTypeEnum) As PortalHospitalConnectionRequestInputModel

        Dim list As New List(Of KeyValuePair(Of Integer, String))
        Dim linkageSystemId As String = String.Empty
        Dim statusType As Byte = Byte.MinValue
        Dim mailaddress As String = String.Empty
        Dim relationFlag As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None

        With PortalHospitalConnectionRequestWorker.ExecuteHospitalConnectionRequestReadApi(mainModel, LinkageSystemNo)

            '表示対象を設定値から取得したLinkageSystemNoで制限
            Dim hospitalEnableLinkageList As List(Of Integer) = PortalHospitalConnectionRequestWorker.GetSetting()

            list = .HospitalList.Where(
                Function(i) hospitalEnableLinkageList.Contains(i.LinkageSystemNo.TryToValueType(Integer.MinValue))).ToList() _
                .ConvertAll(Function(i) New KeyValuePair(Of Integer, String)(Integer.Parse(i.LinkageSystemNo), i.LinkageName)
                )

            '表示対象をリストで制限（暫定対応）
            'list = .HospitalList.Where(
            '    Function(i) HOSPITAL_ENABLE_LINKAGELIST.Contains(i.LinkageSystemNo.TryToValueType(Integer.MinValue))).ToList() _
            '    .ConvertAll(Function(i) New KeyValuePair(Of Integer, String)(Integer.Parse(i.LinkageSystemNo), i.LinkageName)
            '    )

            '//list = .HospitalList.ConvertAll(Function(i) New KeyValuePair(Of Integer, String)(Integer.Parse(i.LinkageSystemNo), i.LinkageName))
            linkageSystemId = .LinkageSystemId
            statusType = .StatusType.TryToValueType(Byte.MinValue)
            mailaddress = .MailAddress
            relationFlag = DirectCast([Enum].ToObject(GetType(QyRelationContentTypeEnum), .RelationContentType.TryToValueType(Long.MinValue)), QyRelationContentTypeEnum)
        End With

        Return New PortalHospitalConnectionRequestInputModel() With {
                .FromPageNoType = fromPageNoType,
                .HospitalList = list,
                .LinkageSystemNo = LinkageSystemNo,
                .LinkageSystemId = linkageSystemId,
                .StatusType = statusType,
                .FamilyName = mainModel.AuthorAccount.FamilyName,
                .GivenName = mainModel.AuthorAccount.GivenName,
                .FamilyKanaName = mainModel.AuthorAccount.FamilyKanaName,
                .GivenKanaName = mainModel.AuthorAccount.GivenKanaName,
                .SexType = mainModel.AuthorAccount.SexType,
                .BirthYear = mainModel.AuthorAccount.Birthday.Year.ToString(),
                .BirthMonth = mainModel.AuthorAccount.Birthday.Month.ToString(),
                .BirthDay = mainModel.AuthorAccount.Birthday.Day.ToString(),
                .MailAddress = mailaddress,
                .RelationContentFlags = relationFlag
            }



    End Function

    Shared Function Request(mainModel As QolmsYappliModel, model As PortalHospitalConnectionRequestInputModel, ByRef message As String) As Boolean

        '登録処理
        With PortalHospitalConnectionRequestWorker.ExecuteHospitalConnectionRequestWriteApi(mainModel, model)
            If .Count = 1 AndAlso .IsSuccess.TryToValueType(False) Then

                '成功したらアカウント情報(名前)の更新
                mainModel.AuthorAccount.FamilyName = model.FamilyName
                mainModel.AuthorAccount.GivenName = model.GivenName
                mainModel.AuthorAccount.FamilyKanaName = model.FamilyKanaName
                mainModel.AuthorAccount.GivenKanaName = model.GivenKanaName

            ElseIf .Count = 0 AndAlso .IsSuccess.TryToValueType(False) Then

                message = "診察券番号/個人番号に誤りがあります。"

            End If

            Return .IsSuccess.TryToValueType(False)

        End With

    End Function

#End Region

End Class
