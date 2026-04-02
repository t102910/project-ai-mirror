Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PortalUnsubscribeWorker

#Region "Constant"

    ''' <summary>
    ''' 連携システムNOを決め打ち。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const OKINAWAAULINKAGESYSTEMNO As String = "47003"

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

    Private Shared Function ExecutePortalUnsubscribeReadApi(mainModel As QolmsYappliModel) As QhYappliPortalUnsubscribeReadApiResults

        Dim apiArgs As New QhYappliPortalUnsubscribeReadApiArgs(
         QhApiTypeEnum.YappliPortalUnsubscribeRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
         .LinkageSystemNo = OKINAWAAULINKAGESYSTEMNO
        }
 
        Dim apiResults As QhYappliPortalUnsubscribeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalUnsubscribeReadApiResults)(
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

    Private Shared Function ExecutePortalUnsubscribeWriteApi(mainModel As QolmsYappliModel, inputModel As PortalUnsubscribeInputModel) As QiQolmsYappliPortalUnsubscribeWriteApiResults

        Dim apiArgs As New QiQolmsYappliPortalUnsubscribeWriteApiArgs(
         QiApiTypeEnum.QolmsYappliPortalUnsubscribeWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
        .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
        .LinkageSystemNo = OKINAWAAULINKAGESYSTEMNO,
        .UnsubScribeItemNo = inputModel.ReasonCode.ToString(),
        .Comment = IIf(inputModel.ReasonComment = Nothing, String.Empty, inputModel.ReasonComment).ToString()
        }

        Dim apiResults As QiQolmsYappliPortalUnsubscribeWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliPortalUnsubscribeWriteApiResults)(
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

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalUnsubscribeInputModel
        Dim result As New PortalUnsubscribeInputModel(mainModel)

        With PortalUnsubscribeWorker.ExecutePortalUnsubscribeReadApi(mainModel)

            If .IsSuccess.TryToValueType(False) Then
                result.PremiumMemberShipType = Byte.Parse(.PremiumMemberShipType)
                result.ReasonList = .UnsubscribeListN.ConvertAll(Of KeyValuePair(Of Integer, String))( _
                    Function(x) New KeyValuePair(Of Integer, String)(Integer.Parse(x.Key), x.Value))
            End If
        End With
        Return result

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="inputModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns>1：成功、2：会員情報更新失敗、 3：プレミアム会員解約失敗</returns>
    ''' <remarks></remarks>
    Public Shared Function Register(mainModel As QolmsYappliModel, inputModel As PortalUnsubscribeInputModel, auPaymentModel As PaymentInf) As Integer
        inputModel.PremiumMemberShipType = PremiumWorker.GetMemberShipType(mainModel)

        'プレミアム会員解約
        Dim premium As Integer = Integer.MinValue
        If (inputModel.PremiumMemberShipType = 2 OrElse inputModel.PremiumMemberShipType = 3) Then
            premium = PremiumCancel(mainModel, auPaymentModel)
        End If

        If (premium = 1 Or premium = 2) OrElse inputModel.PremiumMemberShipType = 1 Then '「プレミアム解約済み」か「一般会員」
            With PortalUnsubscribeWorker.ExecutePortalUnsubscribeWriteApi(mainModel, inputModel)
                If .IsSuccess.TryToValueType(False) Then
                    '成功
                    Return 1
                Else
                    '会員情報の更新失敗（エラーのはず）
                    Return 2
                End If
            End With
        Else
            'プレミアム会員の解約失敗
            Return 3
        End If

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns>1:成功、2：解約済み、9：失敗</returns>
    ''' <remarks></remarks>
    Public Shared Function PremiumCancel(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Integer

        If auPaymentModel Is Nothing Then Throw New ArgumentNullException("auPaymentModel", "auPaymentModelがNull参照です。")
        If mainModel Is Nothing Then Throw New ArgumentNullException("mainModel", "mainModelがNull参照です。")

        If auPaymentModel.MemberManageNo <= 0 Then
            '削除済み、未登録
            Return 2
        End If

        Dim debag As Boolean = False
        Try
            debag = Boolean.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings("DemoMode"))
        Catch ex As Exception
        End Try
        If debag Then
            If auPaymentModel.PaymentType = QyPaymentTypeEnum.au Then
                PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
            Else
                If PremiumPayJPWorker.RequestContBillCancel(mainModel, auPaymentModel) Then
                    PremiumPayJPWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
                    Return 1
                Else
                    '失敗の内容を確認
                    Return 9
                End If
            End If
            Return 1
        Else
            If auPaymentModel.PaymentType = QyPaymentTypeEnum.au Then
                'au簡単決済の解約
                If AuPaymentAccessWorker.RequestContBillCancel(auPaymentModel) Then
                    PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
                    Return 1
                Else
                    If auPaymentModel.AuResultCode = "MPL40011" Then    'すでに解約済みエラー
                        'もう一度ステータス確認し、ステータスをそろえる
                        Dim oldAuPaymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(mainModel)
                        If oldAuPaymentModel.MemberShipType <> QyMemberShipTypeEnum.Free Then
                            If auPaymentModel.EndDate = Date.MaxValue Then auPaymentModel.EndDate = Now.Date
                            PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
                        End If
                        Return 2
                    End If
                    PremiumWorker.UpdatePremiumRecordToCancelPremiumRegistFailed(mainModel, auPaymentModel)
                    Throw New InvalidOperationException(String.Format("auid={0}：{1}", auPaymentModel.AuId, "継続課金情報解除登録要求時のエラーです。"))
                End If
            Else
                'pay.jpの解約
                If PremiumPayJPWorker.RequestContBillCancel(mainModel, auPaymentModel) Then
                    PremiumPayJPWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
                    Return 1
                Else
                    '失敗の内容を確認
                    Return 9
                End If
            End If
        End If
        Return 9

    End Function

#End Region


End Class
