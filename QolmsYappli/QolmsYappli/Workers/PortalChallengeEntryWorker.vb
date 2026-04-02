Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1


''' <summary>
''' 「チャレンジエントリー」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalChallengeEntryWorker

#Region "Constant"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 「チャレンジエントリー」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeEntryReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeEntryReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeEntryReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeEntryRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .Challengekey = challengekey.ToApiGuidString()
         }

        Dim apiResults As QhYappliPortalChallengeEntryReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEntryReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' 「チャレンジエントリー」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeEntryAgreeReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeEntryAgreeReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeEntryAgreeReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeEntryAgreeRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .Challengekey = challengekey.ToApiGuidString()
         }

        Dim apiResults As QhYappliPortalChallengeEntryAgreeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEntryAgreeReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function



    ''' <summary>
    ''' 「チャレンジエントリー」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeEntryPassReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeEntryPassReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeEntryPassReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeEntryPassRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .Challengekey = challengekey.ToApiGuidString()
         }

        Dim apiResults As QhYappliPortalChallengeEntryPassReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEntryPassReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecutePortalChallengeEntryWriteApi(mainModel As QolmsYappliModel, preaentModel As PortalChallengeEntryInputModel) As QhYappliPortalChallengeEntryWriteApiResults

        Dim apiArgs As New QhYappliPortalChallengeEntryWriteApiArgs(
         QhApiTypeEnum.YappliPortalChallengeEntryWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .Challengekey = preaentModel.ChallengeItem.Challengekey.ToApiGuidString(),
             .PassCode = If(preaentModel.Pass Is Nothing, String.Empty, preaentModel.Pass),
             .RequiredN = preaentModel.RequiredN.ToList().ConvertAll(Function(i) New QhApiChallengeEntryRequiredItem() With {.Key = i.Key, .Value = i.Value}),
             .OptionalN = preaentModel.OptionalN.ToList().ConvertAll(Function(i) New QhApiChallengeEntryOptionalItem() With {.Key = i.Key, .Value = i.Value}),
             .RelationContentFlags = Convert.ToByte(preaentModel.RelationContentFlags).ToString()
         }

        Dim apiResults As QhYappliPortalChallengeEntryWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEntryWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function



    ''' <summary>
    ''' 「チャレンジエントリー」画面PostCode取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengePostalCodeEntryReadApi(mainModel As QolmsYappliModel, postCode As String) As QhPostalCodeToAddressReadApiResults

        Dim apiArgs As New QhPostalCodeToAddressReadApiArgs(
         QhApiTypeEnum.PostalCodeToAddressRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .PostalCode = postCode
         }

        Dim apiResults As QhPostalCodeToAddressReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhPostalCodeToAddressReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function


#End Region

#Region "Public Method"
    ''' <summary>
    ''' チャレンジエントリー画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, challengekey As Guid) As PortalChallengeEntryInputModel

        Dim result As New PortalChallengeEntryInputModel(mainModel)
        ' API を実行
        With PortalChallengeEntryWorker.ExecutePortalChallengeEntryReadApi(mainModel, challengekey)

            Dim item As QhApiChallengeItem = .ChallengeItem

            Dim imageStr As String = String.Empty
            'If PortalChallengeWorker.imageDic.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then
            If PortalChallengeWorker.ChallengeFolder.Value.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then
                imageStr = String.Format("../dist/img/challenge/{0}/", PortalChallengeWorker.ChallengeFolder.Value(item.Challengekey.TryToValueType(Guid.Empty)))
            End If


            Dim challenge As New ChallengeItem() With {
                .Challengekey = item.Challengekey.TryToValueType(Guid.Empty),
                .Description = item.Description,
                .EntryEndDate = item.EntryEndDate.TryToValueType(Date.MinValue),
                .EntryStartDate = item.EntryStartDate.TryToValueType(Date.MinValue),
                .Name = item.Name,
                .Period = item.Period.TryToValueType(Integer.MinValue),
                .StartDate = item.StartDate.TryToValueType(Date.MinValue),
                .StatusType = item.StatusType.TryToValueType(Byte.MinValue),
                .StatusTypeMaster = item.StatusTypeMaster.ConvertAll(Function(i) New ChallengeStatusItem() With {
                                                                         .Challengekey = i.Challengekey.TryToValueType(Guid.Empty),
                                                                         .StatusType = i.StatusType.TryToValueType(Byte.MinValue),
                                                                         .StatusName = i.StatusName
                                                                     }),
                .UserEndDate = item.UserEndDate.TryToValueType(Date.MinValue),
                .UserStartDate = item.UserStartDate.TryToValueType(Date.MinValue),
                .ImageSrc = imageStr
                }

            result.ChallengeItem = challenge

            Return result

        End With

    End Function

    ''' <summary>
    ''' 規約同意画面のパーシャルビューモデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="preaentModel">親モデル</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function CreateAgreePartialViewModel(mainModel As QolmsYappliModel, preaentModel As PortalChallengeEntryInputModel) As PortalChallengeEntryAgreeResultPartialViewModel

        Dim result As New PortalChallengeEntryAgreeResultPartialViewModel()

        If preaentModel.ChallengeItem.Challengekey <> Guid.Empty Then

            With PortalChallengeEntryWorker.ExecutePortalChallengeEntryAgreeReadApi(mainModel, preaentModel.ChallengeItem.Challengekey)
                result.PageViewModel = preaentModel

                result.Terms = .Terms.Replace(vbCrLf, "<br/>")
            End With

        End If

        Return result

    End Function


    ''' <summary>
    ''' 参加資格確認画面のパーシャルビューモデルを取得します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="preaentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function CreatePartialPassViewModel(mainModel As QolmsYappliModel, preaentModel As PortalChallengeEntryInputModel) As PortalChallengeEntryPassResultPartialViewModel

        Dim result As New PortalChallengeEntryPassResultPartialViewModel()
        result.PageViewModel = preaentModel

        If preaentModel.ChallengeItem.Challengekey <> Guid.Empty Then

            With PortalChallengeEntryWorker.ExecutePortalChallengeEntryPassReadApi(mainModel, preaentModel.ChallengeItem.Challengekey)

                'If .IsPremiumOnry.TryToValueType(False) Then
                '    'プレミアム会員かどうかをチェックして画面を返す？
                '    'とりあえず使わないので実装は後
                'End If

                If .PassCodes.Count > 0 Then
                    result.PassCodeVisible = True
                    result.PassCodes = .PassCodes
                End If

                For Each item As QhApiChallengeEntryPassRequiredItem In .RequiredN
                    result.RequiredN.Add(item.Key, item.Name)
                Next

                For Each item As QhApiChallengeEntryPassOptionalItem In .OptionalN
                    result.OptionalN.Add(item.Key, item.Name)
                Next

                result.LinkageSystemNo = .LinkageSystemNo.TryToValueType(Integer.MinValue)
                result.MailAddress = .MailAddress
                result.RelationFlag = .RelationFlag.TryToValueType(False)

            End With

        End If

        Return result

    End Function

    Public Shared Sub GetInitialValue(mainModel As QolmsYappliModel, ByRef preaentModel As PortalChallengeEntryInputModel)

        With preaentModel

            For Each item As KeyValuePair(Of String, String) In .PassPartialViewModel.RequiredN

                Select Case item.Key

                    Case "Name"
                        If Not .RequiredN.ContainsKey("Name") Then

                            .RequiredN.Add("Name", mainModel.AuthorAccount.Name)
                        End If

                    Case "KanaName"
                        If Not .RequiredN.ContainsKey("KanaName") Then
                            .RequiredN.Add("KanaName", mainModel.AuthorAccount.KanaName)
                        End If
                    Case "Mail"
                        If Not .RequiredN.ContainsKey("Mail") Then
                            .RequiredN.Add("Mail", preaentModel.PassPartialViewModel.MailAddress)
                        End If
                    Case "Birthday"
                        If Not .RequiredN.ContainsKey("Birthday") Then
                            .RequiredN.Add("Birthday", mainModel.AuthorAccount.Birthday.ToString("yyyy/MM/dd"))
                        End If

                End Select

            Next

        End With

    End Sub

    Shared Function Entry(mainModel As QolmsYappliModel, preaentModel As PortalChallengeEntryInputModel) As Boolean

        Try

            With PortalChallengeEntryWorker.ExecutePortalChallengeEntryWriteApi(mainModel, preaentModel)

                Return .IsSuccess.TryToValueType(False)
            End With

        Catch ex As Exception

            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ChallengeEntry Error:{0}", ex.Message))
        End Try

        Return False
    End Function

    Shared Function RequiredItemValidate(key As String, value As String, ByRef errorMessage As String) As Boolean

        Dim sb As New StringBuilder()

        Select Case key

            Case "PhoneNumber"
                '数値＋桁数（10か11）
                If Not Regex.IsMatch(value, "^[0-9]+$") Then sb.AppendLine("数値のみで入力してください。")
                If value.Length < 10 OrElse value.Length > 11 Then sb.AppendLine("市外局番を含む10桁または11桁で入力してください。")

        End Select

        errorMessage = sb.ToString()

        Return sb.Length = 0

    End Function

    Friend Shared Function PostalCodeToAddress(mainModel As QolmsYappliModel, postCode As String) As String

        With PortalChallengeEntryWorker.ExecutePortalChallengePostalCodeEntryReadApi(mainModel, postCode)

            Return .AddressN.First().Address1 + .AddressN.First().Address2 + .AddressN.First().Address3

        End With

    End Function

#End Region

End Class
