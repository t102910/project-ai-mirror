Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「チャレンジ編集」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalChallengeEditWorker

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
    ''' 「チャレンジ編集」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeEditReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeEditReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeEditReadApiArgs(
             QhApiTypeEnum.YappliPortalChallengeEditRead,
             QsApiSystemTypeEnum.Qolms,
             mainModel.ApiExecutor,
             mainModel.ApiExecutorName
             ) With {
                 .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                 .Challengekey = challengekey.ToApiGuidString()
        }

        Dim apiResults As QhYappliPortalChallengeEditReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEditReadApiResults)(
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


    Private Shared Function ExecutePortalChallengeEditWriteApi(mainModel As QolmsYappliModel, model As PortalChallengeEditInputModel) As QhYappliPortalChallengeEditWriteApiResults

        Dim apiArgs As New QhYappliPortalChallengeEditWriteApiArgs(
         QhApiTypeEnum.YappliPortalChallengeEditWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .Challengekey = model.ChallengeItem.Challengekey.ToApiGuidString(),
             .PassCode = model.ChallengeInputItem.PassCode,
             .RequiredN = model.ChallengeInputItem.RequiredN.ToList().ConvertAll(Function(i) New QhApiChallengeEntryRequiredItem() With {.Key = i.Key, .Value = i.Value}),
             .OptionalN = model.ChallengeInputItem.OptionalN.ToList().ConvertAll(Function(i) New QhApiChallengeEntryOptionalItem() With {.Key = i.Key, .Value = i.Value}),
             .RelationContentFlags = Convert.ToByte(model.RelationContentFlags).ToString()
         }

        Dim apiResults As QhYappliPortalChallengeEditWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEditWriteApiResults)(
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

    Private Shared Function ExecutePortalChallengeCancelWriteApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeEditCancelWriteApiResults

        Dim apiArgs As New QhYappliPortalChallengeEditCancelWriteApiArgs(
         QhApiTypeEnum.YappliPortalChallengeEditCancelWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .Challengekey = challengekey.ToApiGuidString()
         }

        Dim apiResults As QhYappliPortalChallengeEditCancelWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeEditCancelWriteApiResults)(
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
    ''' チャレンジ画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, challengekey As Guid) As PortalChallengeEditInputModel

        Dim apiResult As QhYappliPortalChallengeEditReadApiResults = PortalChallengeEditWorker.ExecutePortalChallengeEditReadApi(mainModel, challengekey)


        If apiResult.IsSuccess.TryToValueType(False) Then

            Dim requiredN As New List(Of ChallengeInputValueItem)
            For Each item As QhApiChallengeInputValueItem In apiResult.ChallengeInputItem.RequiredN

                requiredN.Add(New ChallengeInputValueItem() With {.Key = item.Key, .Title = item.Title, .Value = item.Value})

            Next

            Dim optionalN As New List(Of ChallengeInputValueItem)
            For Each item As QhApiChallengeInputValueItem In apiResult.ChallengeInputItem.OptionalN

                optionalN.Add(New ChallengeInputValueItem() With {.Key = item.Key, .Title = item.Title, .Value = item.Value})

            Next

            Dim model As New PortalChallengeEditInputModel() With {
                .ChallengeItem = New ChallengeItem() With {
                    .Challengekey = apiResult.ChallengeItem.Challengekey.TryToValueType(Guid.Empty),
                    .Description = apiResult.ChallengeItem.Description,
                    .EntryEndDate = apiResult.ChallengeItem.EntryEndDate.TryToValueType(Date.MinValue),
                    .EntryStartDate = apiResult.ChallengeItem.EntryStartDate.TryToValueType(Date.MinValue),
                    .ExternalId = apiResult.ChallengeItem.ExternalId,
                    .ImageSrc = "",
                    .Name = apiResult.ChallengeItem.Name,
                    .Period = apiResult.ChallengeItem.Period.TryToValueType(Integer.MinValue),
                    .RelationFlag = apiResult.ChallengeItem.RelationFlag.TryToValueType(False),
                    .StartDate = apiResult.ChallengeItem.StartDate.TryToValueType(Date.MinValue),
                    .StatusType = apiResult.ChallengeItem.StatusType.TryToValueType(Byte.MinValue),
                    .UserEndDate = apiResult.ChallengeItem.UserEndDate.TryToValueType(Date.MinValue),
                    .UserStartDate = apiResult.ChallengeItem.UserStartDate.TryToValueType(Date.MinValue)
                },
                .ChallengeInputItem = New ChallengeInputItem() With {
                    .PassCode = apiResult.ChallengeInputItem.PassCode,
                    .RequiredN = requiredN,
                    .OptionalN = optionalN
                },
                .RelationContentFlags = DirectCast([Enum].ToObject(GetType(QyRelationContentTypeEnum), apiResult.RelationContentFlags.TryToValueType(Byte.MinValue)), QyRelationContentTypeEnum),
                .linkageSystemId = apiResult.linkageItem.LinkageSystemId,
                .linkageStatus = Byte.Parse(apiResult.linkageItem.StatusType)
            }

            'Dim linkageno As String = apiResult.linkageItem.LinkageSystemNo
            'Dim linkageid As String = apiResult.linkageItem.LinkageSystemId
            'Dim status As String = apiResult.linkageItem.StatusType

            Return model

        End If

        Return New PortalChallengeEditInputModel()

    End Function

    Public Shared Function Edit(mainModel As QolmsYappliModel, preaentModel As PortalChallengeEditInputModel) As Boolean

        Try

            With PortalChallengeEditWorker.ExecutePortalChallengeEditWriteApi(mainModel, preaentModel)

                Return .IsSuccess.TryToValueType(False)
            End With

        Catch ex As Exception

            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ChallengeEntry Error:{0}", ex.Message))
        End Try

        Return False

    End Function


    Shared Function Cancel(mainModel As QolmsYappliModel, challengekey As Guid) As Boolean

        Try

            With PortalChallengeEditWorker.ExecutePortalChallengeCancelWriteApi(mainModel, challengekey)

                Return .IsSuccess.TryToValueType(False)
            End With

        Catch ex As Exception

            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ChallengeEntry Error:{0}", ex.Message))
        End Try

        Return False
    End Function

    'Shared Function Cancel(mainModel As QolmsYappliModel) As Boolean

    '    Try

    '        With PortalChallengeEditWorker.ExecutePortalChallengeCancelWriteApi(mainModel)

    '            Return .IsSuccess.TryToValueType(False)
    '        End With

    '    Catch ex As Exception

    '        AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("ChallengeEntry Error:{0}", ex.Message))
    '    End Try

    '    Return False


    'End Function

#End Region

End Class
