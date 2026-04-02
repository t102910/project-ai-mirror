Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Threading.Tasks

Friend NotInheritable Class PortalCouponForFitbitWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Constant"


    ''' <summary>
    ''' ポイント交換後にメールを送信するかどうかを設定します。
    ''' </summary>
    Public Shared ReadOnly IsSendMail As String = ConfigurationManager.AppSettings("IsSendMailPointExchange")
        
    ''' <summary>
    ''' ポイント交換後にメールを送信する残りクーポン枚数を設定します。IsSendMail= Trueのときのみ有効です。
    ''' </summary>
    Public Shared ReadOnly IsSendMailRestCount As String = ConfigurationManager.AppSettings("IsSendMailPointExchangeRestCount")

#End Region

#Region "Private Method"

    Private Shared Function ExecuteCouponForFitbitReadApi(mainModel As QolmsYappliModel) As QjPortalCouponForFitbitReadApiResults

        Dim apiArgs As New QjPortalCouponForFitbitReadApiArgs(
            QjApiTypeEnum.PortalCouponForFitbitRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            }
        Dim apiResults As QjPortalCouponForFitbitReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalCouponForFitbitReadApiResults)(
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

    Private Shared Function ExecuteCouponForFitbitMasterReadApi(mainModel As QolmsYappliModel, couponType As Byte) As QjPortalCouponForFitbitMasterReadApiResults

        Dim apiArgs As New QjPortalCouponForFitbitMasterReadApiArgs(
            QjApiTypeEnum.PortalCouponForFitbitMasterRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .CouponType = couponType.ToString()
            }

        Dim apiResults As QjPortalCouponForFitbitMasterReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalCouponForFitbitMasterReadApiResults)(
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

    Private Shared Function ExecuteCouponForFitbitWriteApi(mainModel As QolmsYappliModel, couponType As Integer, serialCode As String) As QjPortalCouponForFitbitWriteApiResults

        Dim apiArgs As New QjPortalCouponForFitbitWriteApiArgs(
         QjApiTypeEnum.PortalCouponForFitbitWrite,
         QsApiSystemTypeEnum.QolmsJoto,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
        ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .CouponType = couponType.ToString(),
             .SerialCode = serialCode
        }
        Dim apiResults As QjPortalCouponForFitbitWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalCouponForFitbitWriteApiResults)(
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

#End Region

#Region "Public Method"


    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, pageno As QyPageNoTypeEnum) As PortalCouponForFitbitViewModel
    
        Dim result As New PortalCouponForFitbitViewModel(mainModel)
        
        Dim path As String =HttpContext.Current.Server.MapPath("~/App_Data/CouponForFitbitDescription.txt")
        Dim str As String = String.Empty
        If Not String.IsNullOrWhiteSpace(path) AndAlso IO.File.Exists(path) Then
            str = IO.File.ReadAllText(path)
        End If
        result.Description = str

        Dim apiResult As QjPortalCouponForFitbitReadApiResults = PortalCouponForFitbitWorker.ExecuteCouponForFitbitReadApi(mainModel)
        result.CouponN = apiResult.CouponN.ConvertAll(Function(i) i.ToCouponItem())
        result.PointExchangeHistN = apiResult.PointExchangeHistN.ConvertAll(Function(i) i.ToPointExchangeHistItem())

        'result.CouponN = New List(Of CouponItem)()From{
        '    New CouponItem()with{.CouponType =1 ,.DispName="100円",.Point = 100,.RestCount=100}
        '}

        'result.PointExchangeHistN = New List(Of PointExchangeHistItem) From{
        '    New PointExchangeHistItem()With{.CouponId = "xxxx",.CouponType = 1,.DispName="100円",.Point = 100,.ExpirationDate = New Date (2024,3,1),.IssueDate = New Date(2023,10,3)}
        '}

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
        Dim masterResult As QjPortalCouponForFitbitMasterReadApiResults = PortalCouponForFitbitWorker.ExecuteCouponForFitbitMasterReadApi(mainModel, couponType)

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
                        .Reason = "「Fitbit値引き」クーポンと交換"
                        }
            }.ToList()
        )
        If pointResult.First().Value = 0 Then
            '成功したら交換
            Try
                Dim result As QjPortalCouponForFitbitWriteApiResults = PortalCouponForFitbitWorker.ExecuteCouponForFitbitWriteApi(mainModel, couponType, pointResult.First().Key)

                If Integer.Parse(result.Result) > 0 Then

                    'メール
                    If Integer.Parse(result.Count) <= 100 AndAlso ((Not String.IsNullOrWhiteSpace(IsSendMail)) AndAlso Boolean.Parse(IsSendMail)) Then

                        Dim bodyString As New StringBuilder()
                        bodyString.AppendLine("「Fitbit値引き」クーポン残数100枚以下。")
                        bodyString.AppendLine(String.Format("残り：{0}", result.Count))

                        Dim task As Task = NoticeMailWorker.SendAsync(bodyString.ToString())
                    End If

                    Return True

                Else
                    Throw New InvalidOperationException(String.Format("発行できる「Fitbit値引き」クーポンがありません。"))

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
                                .Reason = "「Fitbit値引き」クーポンと交換失敗のためポイント復元"
                           }
                    }.ToList()
                )

                If removePointResult.First().Value <> 0 Then
                    Dim bodyString1 As New StringBuilder()
                    bodyString1.AppendLine("「Fitbit値引き」クーポンポイント修正のエラーです。")
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
