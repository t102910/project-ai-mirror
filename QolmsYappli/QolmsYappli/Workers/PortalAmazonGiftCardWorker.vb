Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Net
Imports System.IO
Imports System.Threading.Tasks

Friend NotInheritable Class PortalAmazonGiftCardWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Constant"


#End Region

#Region "Private Method"

    Private Shared Function ExecuteAmazonGiftCardReadApi(mainModel As QolmsYappliModel) As QhYappliPortalAmazonGiftCardReadApiResults

        Dim apiArgs As New QhYappliPortalAmazonGiftCardReadApiArgs(
         QhApiTypeEnum.YappliPortalAmazonGiftCardRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }
        Dim apiResults As QhYappliPortalAmazonGiftCardReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAmazonGiftCardReadApiResults)(
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

    Private Shared Function ExecuteAmazonGiftCardMasterReadApi(mainModel As QolmsYappliModel, giftCardType As Byte) As QhYappliPortalAmazonGiftCardMasterReadApiResults

        Dim apiArgs As New QhYappliPortalAmazonGiftCardMasterReadApiArgs(
        QhApiTypeEnum.YappliPortalAmazonGiftCardMasterRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .GiftCardType = giftCardType.ToString()
        }
        Dim apiResults As QhYappliPortalAmazonGiftCardMasterReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAmazonGiftCardMasterReadApiResults)(
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

    Private Shared Function ExecuteAmazonGiftCardWriteApi(mainModel As QolmsYappliModel, giftCardType As Byte, serialCode As String) As QhYappliPortalAmazonGiftCardWriteApiResults

        Dim apiArgs As New QhYappliPortalAmazonGiftCardWriteApiArgs(
         QhApiTypeEnum.YappliPortalAmazonGiftCardWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .GiftCardType = giftCardType.ToString(),
             .SerialCode = serialCode
        }
        Dim apiResults As QhYappliPortalAmazonGiftCardWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalAmazonGiftCardWriteApiResults)(
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum) As PortalAmazonGiftCardViewModel

        Dim result As New PortalAmazonGiftCardViewModel()
        Dim apiResult As QhYappliPortalAmazonGiftCardReadApiResults = PortalAmazonGiftCardWorker.ExecuteAmazonGiftCardReadApi(mainModel)
        result.GiftCardN = apiResult.AmazonGiftCardN.ConvertAll(Function(i) i.ToAmazonGiftCardItem())
        result.GiftCardHistN = apiResult.AmazonGiftCardHistN.ConvertAll(Function(i) i.ToAmazonGiftCardHistItem())
        result.FromPageNoType = pageNo

        'ポイント数の表示
        Try
            result.Point = QolmsPointWorker.GetQolmsPoint(
                            mainModel.ApiExecutor,
                            mainModel.ApiExecutorName,
                            mainModel.SessionId,
                            mainModel.ApiAuthorizeKey,
                            mainModel.AuthorAccount.AccountKey
                            )
        Catch ex As Exception
            result.Point = 0
        End Try

        Return result

    End Function

    Public Shared Function Exchange(mainModel As QolmsYappliModel, itemId As Byte) As Boolean

        Dim actionDate As Date = Date.Now
        Dim masterResult As QhYappliPortalAmazonGiftCardMasterReadApiResults = PortalAmazonGiftCardWorker.ExecuteAmazonGiftCardMasterReadApi(mainModel, itemId)

        'point減算
        Dim limit As Date = Date.MinValue

        Dim pointResult As Dictionary(Of String, Integer) = QolmsPointWorker.AddQolmsPoints(
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey,
            mainModel.AuthorAccount.AccountKey,
            {
                New QolmsPointGrantItem() With {
                        .ActionDate = actionDate,
                        .SerialCode = Guid.NewGuid().ToApiGuidString(),
                        .PointItemNo = QyPointItemTypeEnum.AmazonPoint,
                        .Point = Integer.Parse(masterResult.GiftCard.DemandPoint) * -1,
                        .PointTargetDate = actionDate,
                        .PointExpirationDate = Date.MaxValue,
                        .Reason = "Amazonギフト券と交換"
                        }
            }.ToList()
        )
        If pointResult.First().Value = 0 Then
            '成功したら交換
            Try
                Dim result As QhYappliPortalAmazonGiftCardWriteApiResults = PortalAmazonGiftCardWorker.ExecuteAmazonGiftCardWriteApi(mainModel, itemId, pointResult.First().Key)

                If Integer.Parse(result.Result) > 0 Then

                    'メールtodo:枚数確認
                    If Integer.Parse(result.Count) <= 500 Then

                        Dim bodyString As New StringBuilder()
                        bodyString.AppendLine("ポイント交換Amazonギフト券残数500枚以下。")
                        bodyString.AppendLine(String.Format("{0} 残り：{1}", masterResult.GiftCard.GiftcardName, result.Count))

                        Dim task As Task = NoticeMailWorker.SendAsync(bodyString.ToString())
                    End If

                    Return True

                Else

                    Dim bodyString As New StringBuilder()
                    bodyString.AppendLine("ポイント交換Amazonギフト券の発行できるコードがありません。")
                    bodyString.AppendLine(masterResult.GiftCard.GiftcardName)

                    Dim task As Task = NoticeMailWorker.SendAsync(bodyString.ToString())

                    Throw New InvalidOperationException(String.Format("ポイント交換Amazonギフト券の発行できるコードがありません。"))
                End If
            Catch ex As Exception
                '交換失敗
                Dim pointLimitDate As Date = New Date(
                       actionDate.Year,
                       actionDate.Month,
                       1
                   ).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

                Dim removePointResult As Dictionary(Of String, Integer) = QolmsPointWorker.AddQolmsPoints(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey,
                    {
                        New QolmsPointGrantItem() With {
                                .ActionDate = actionDate,
                                .SerialCode = Guid.NewGuid().ToApiGuidString(),
                                .PointItemNo = QyPointItemTypeEnum.RecoveryPoint,
                                .Point = Integer.Parse(masterResult.GiftCard.DemandPoint),
                                .PointTargetDate = actionDate,
                                .PointExpirationDate = pointLimitDate,
                                .Reason = "Amazonギフト券と交換失敗のためポイント復元"
                           }
                    }.ToList()
                )

                If removePointResult.First().Value <> 0 Then
                    Dim bodyString1 As New StringBuilder()
                    bodyString1.AppendLine("Amazonギフト券ポイント修正のエラーです。")
                    bodyString1.AppendLine(String.Format("error:{0}", removePointResult.First().Key))

                    Dim task1 As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString1.ToString())
                End If

            End Try
        Else
            Throw New InvalidOperationException(String.Format("ポイントの減算に失敗しました。({0}):{1}", pointResult.First().Value, pointResult.First().Key))
        End If
        Return False

    End Function

#End Region

End Class
