Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Net
Imports System.IO
Imports System.Threading.Tasks

Friend NotInheritable Class PortalPointExchangeWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Constant"

    'メールを送る
    Public Shared ReadOnly IsSendMail As String = ConfigurationManager.AppSettings("IsSendMailPointExchange")

#End Region

#Region "Private Method"

    Private Shared Function ExecutePointExchangeReadApi(mainModel As QolmsYappliModel) As QhYappliPortalPointExchangeReadApiResults

        Dim apiArgs As New QhYappliPortalPointExchangeReadApiArgs(
         QhApiTypeEnum.YappliPortalPointExchangeRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }
        Dim apiResults As QhYappliPortalPointExchangeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalPointExchangeReadApiResults)(
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

    Private Shared Function ExecutePointExchangeMasterReadApi(mainModel As QolmsYappliModel, couponType As Byte) As QhYappliPortalPointExchangeMasterReadApiResults

        Dim apiArgs As New QhYappliPortalPointExchangeMasterReadApiArgs(
         QhApiTypeEnum.YappliPortalPointExchangeMasterRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .CouponType = couponType.ToString()
        }
        Dim apiResults As QhYappliPortalPointExchangeMasterReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalPointExchangeMasterReadApiResults)(
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

    Private Shared Function ExecutePointExchangeWriteApi(mainModel As QolmsYappliModel, couponType As Integer, serialCode As String) As QhYappliPortalPointExchangeWriteApiResults

        Dim apiArgs As New QhYappliPortalPointExchangeWriteApiArgs(
         QhApiTypeEnum.YappliPortalPointExchangeWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .CouponType = couponType.ToString(),
             .SerialCode = serialCode
        }
        Dim apiResults As QhYappliPortalPointExchangeWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalPointExchangeWriteApiResults)(
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
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, pageno As QyPageNoTypeEnum) As PortalPointExchangeViewModel

        Dim result As New PortalPointExchangeViewModel()
        Dim apiResult As QhYappliPortalPointExchangeReadApiResults = PortalPointExchangeWorker.ExecutePointExchangeReadApi(mainModel)
        result.CouponN = apiResult.CouponN.ConvertAll(Function(i) i.ToCouponItem())
        result.PointExchangeHistN = apiResult.PointExchangeHistN.Where(Function(j) j.IssueDate.TryToValueType(Date.MinValue) > Date.Now.AddYears(-2)).ToList().ConvertAll(Function(i) i.ToPointExchangeHistItem())
        result.FromPageNoType = pageno

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

    Public Shared Function Exchange(mainModel As QolmsYappliModel, couponType As Byte) As Boolean

        Dim actionDate As Date = Date.Now
        Dim masterResult As QhYappliPortalPointExchangeMasterReadApiResults = PortalPointExchangeWorker.ExecutePointExchangeMasterReadApi(mainModel, couponType)

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
                        .PointItemNo = QyPointItemTypeEnum.PointExchange,
                        .Point = Integer.Parse(masterResult.Coupon.Point) * -1,
                        .PointTargetDate = actionDate,
                        .PointExpirationDate = Date.MaxValue,
                        .Reason = "「沖縄CLIPマルシェ」クーポンと交換"
                        }
            }.ToList()
        )
        If pointResult.First().Value = 0 Then
            '成功したら交換
            Try
                Dim result As QhYappliPortalPointExchangeWriteApiResults = PortalPointExchangeWorker.ExecutePointExchangeWriteApi(mainModel, couponType, pointResult.First().Key)

                If Integer.Parse(result.Result) > 0 Then

                    'メール
                    If Integer.Parse(result.Count) <= 100 AndAlso ((Not String.IsNullOrWhiteSpace(IsSendMail)) AndAlso Boolean.Parse(IsSendMail)) Then

                        Dim bodyString As New StringBuilder()
                        bodyString.AppendLine("ポイント交換クーポン残数100枚以下。")
                        bodyString.AppendLine(String.Format("残り：{0}", result.Count))

                        Dim task As Task = NoticeMailWorker.SendAsync(bodyString.ToString())
                    End If

                    Return True

                Else
                    Throw New InvalidOperationException(String.Format("発行できるクーポンがありません。"))

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
                                .Point = Integer.Parse(masterResult.Coupon.Point),
                                .PointTargetDate = actionDate,
                                .PointExpirationDate = pointLimitDate,
                                .Reason = "「沖縄CLIPマルシェ」クーポンと交換失敗のためポイント復元"
                           }
                    }.ToList()
                )

                If removePointResult.First().Value <> 0 Then
                    Dim bodyString1 As New StringBuilder()
                    bodyString1.AppendLine("クーポンポイント修正のエラーです。")
                    bodyString1.AppendLine(String.Format("error:{0}", removePointResult.First().Key))

                    Dim task1 As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString1.ToString())
                End If
            End Try
        Else
            Throw New InvalidOperationException(String.Format("ポイントの減算に失敗しました。"))
        End If
        Return False

    End Function
#End Region
End Class
