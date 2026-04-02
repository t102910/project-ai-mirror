Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsDbEntityV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsPayJpApiCoreV1
Imports System.Runtime.Serialization
Imports System.IdentityModel.Tokens.Jwt
Imports Microsoft.IdentityModel.Tokens
Imports System.Security.Claims

Friend NotInheritable Class PremiumPayJPWorker

#Region "Constant"

    ' 課金プラン取得の設定値
    Private Const PLAN_LIMIT As Integer = 10
    Private Const PLAN_OFFSET As Integer = 0
    ' 課金プランの課金額
    Private Shared ReadOnly PLAN_AMOUNT As String = ConfigurationManager.AppSettings("PayJpPaymentPremiumMemberAmount")
    '顧客情報
    Private Const CUSTOMER_DESCRIPTION As String = "JOTOホームドクター プレミアム会員"
    Private Const CUSTOMER_EMAIL As String = "joto-hdr@qolms.com"

#End Region

#Region "Class"

    <DataContract()>
    <Serializable()>
    Private NotInheritable Class PsyJpSettingJson

        <DataMember()>
        Public Property Settings As List(Of PsyJpSettingJsonItem)

        Public Sub New()

        End Sub

    End Class


    <DataContract()>
    <Serializable()>
    Private NotInheritable Class PsyJpSettingJsonItem

        <DataMember()>
        Public Property Amount As String

        <DataMember()>
        Public Property StartDate As String

        <DataMember()>
        Public Property EndDate As String

        Public Sub New()

        End Sub

    End Class

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

    ''' <summary>
    '''  課金プランの取得
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePlanListReadApi(amount As Integer) As PayJpPlanOfJson

        Dim apiArgs As New PayJpGetPlanListApiArgs() With {
            .limit = PLAN_LIMIT,
            .offset = PLAN_OFFSET
        }
        Dim apiResults As PayJpGetPlanListApiResults = QsPayJpApiManager.Execute(Of PayJpGetPlanListApiArgs, PayJpGetPlanListApiResults)(apiArgs)

        With apiResults
            If .IsSuccess AndAlso .data.Any() Then
                Dim planOfJson As PayJpPlanOfJson = .data.Find(Function(i) i.amount = amount)

                If planOfJson IsNot Nothing Then Return planOfJson
            Else

                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("プランの取得に失敗しました。{0}", ex.Message))

                    End Try
                End If
                Throw New InvalidOperationException(String.Format("プランの取得に失敗しました。"))
            End If
        End With

    End Function

    ''' <summary>
    ''' 顧客IDから顧客情報を取得
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutecustomerReadApi(customerId As String) As PayJpGetCustomerApiResults

        Dim apiArgs As New PayJpGetCustomerApiArgs() With {
           .id = customerId
       }
        Dim apiResults As PayJpGetCustomerApiResults = QsPayJpApiManager.Execute(Of PayJpGetCustomerApiArgs, PayJpGetCustomerApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then

                If Not String.IsNullOrWhiteSpace(.id) Then Return apiResults
            Else

                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("顧客情報の取得に失敗しました。{0}", ex.Message))

                    End Try
                End If
                Throw New InvalidOperationException(String.Format("顧客情報の取得に失敗しました。"))
            End If
        End With

    End Function

    ''' <summary>
    '''  顧客情報の登録
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNewCustomerWriteApi(token As String) As PayJpNewCustomerApiResults

        ' todo: 2024/11/18 メールアドレスの指定が必要かどうか確認
        Dim apiArgs As New PayJpNewCustomerApiArgs() With {
                   .card = token,
                   .description = CUSTOMER_DESCRIPTION,
                   .email = CUSTOMER_EMAIL
               }
        Dim apiResults As PayJpNewCustomerApiResults = QsPayJpApiManager.Execute(Of PayJpNewCustomerApiArgs, PayJpNewCustomerApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then

                If Not String.IsNullOrWhiteSpace(.id) Then Return apiResults
            Else
                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("顧客情報の登録に失敗しました。{0}", ex.Message))

                    End Try
                End If

                Throw New InvalidOperationException(String.Format("顧客情報の登録に失敗しました。"))
            End If
        End With

    End Function

    ''' <summary>
    ''' カード情報の削除
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNewCustomerCardWriteApi(customerId As String, token As String) As PayJpNewCustomerCardApiResults


        Dim apiArgs As New PayJpNewCustomerCardApiArgs() With {
                   .card = token,
                   .id = customerId
               }
        Dim apiResults As PayJpNewCustomerCardApiResults = QsPayJpApiManager.Execute(Of PayJpNewCustomerCardApiArgs, PayJpNewCustomerCardApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then

                If Not String.IsNullOrWhiteSpace(.id) Then Return apiResults
            Else
                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("カード情報の登録に失敗しました。{0}", ex.Message))

                    End Try
                End If

                Throw New InvalidOperationException(String.Format("カード情報の登録に失敗しました。"))
            End If
        End With

    End Function

    ''' <summary>
    ''' カード情報の削除
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteDeleteCustomerCardWriteApi(cardId As String, customerId As String) As PayJpDeleteCustomerCardApiResults

        Dim apiArgs As New PayJpDeleteCustomerCardApiArgs() With {
                   .card_id = cardId,
                   .id = customerId
               }
        Dim apiResults As PayJpDeleteCustomerCardApiResults = QsPayJpApiManager.Execute(Of PayJpDeleteCustomerCardApiArgs, PayJpDeleteCustomerCardApiResults)(apiArgs)

        With apiResults
            If .IsSuccess AndAlso .deleted Then

                Return apiResults
            Else
                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("カード情報の削除に失敗しました。{0}", ex.Message))

                    End Try
                End If

                Throw New InvalidOperationException(String.Format("カード情報の削除に失敗しました。"))
            End If
        End With

    End Function
    ''' <summary>
    ''' 定期課金の登録
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNewSubscriptionWriteApi(customerId As String, planId As String, trialEnd As Date) As PayJpNewSubscriptionApiResults

        Dim apiArgs As New PayJpNewSubscriptionApiArgs() With {
           .customer = customerId,
           .plan = planId,
           .trial_end = QsPayJpApiHelper.ToUnixTime(trialEnd)
       }
        Dim apiResults As PayJpNewSubscriptionApiResults = QsPayJpApiManager.Execute(Of PayJpNewSubscriptionApiArgs, PayJpNewSubscriptionApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then

                If Not String.IsNullOrWhiteSpace(.id) Then Return apiResults
            Else

                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("定期課金の登録に失敗しました。{0}", ex.Message))

                    End Try
                End If
                Throw New InvalidOperationException(String.Format("定期課金の登録に失敗しました。"))
            End If
        End With

    End Function

    ''' <summary>
    ''' 定期課金の削除
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteDeleteSubscriptionWriteApi(subscriptionId As String) As PayJpDeleteSubscriptionApiResults

        Dim apiArgs As New PayJpDeleteSubscriptionApiArgs() With {
           .id = subscriptionId
       }
        Dim apiResults As PayJpDeleteSubscriptionApiResults = QsPayJpApiManager.Execute(Of PayJpDeleteSubscriptionApiArgs, PayJpDeleteSubscriptionApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then

                If Not String.IsNullOrWhiteSpace(.id) Then Return apiResults
            Else

                ' todo : error取得
                If .error IsNot Nothing Then
                    Dim sr As New QsJsonSerializer()
                    Try
                        Throw New InvalidOperationException(String.Format(sr.Serialize(.error)))
                    Catch ex As Exception
                        Throw New InvalidOperationException(String.Format("定期課金の削除に失敗しました。{0}", ex.Message))

                    End Try
                End If
                Throw New InvalidOperationException(String.Format("定期課金の削除に失敗しました。"))
            End If
        End With

    End Function

    ''' <summary>
    ''' プレミアム会員のステータスを取得します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="containDeleted"></param>s
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePremiumReadApi(mainModel As QolmsYappliModel, containDeleted As Boolean) As QhYappliPremiumReadApiResults

        Dim apiArgs As New QhYappliPremiumReadApiArgs(
            QhApiTypeEnum.YappliPremiumRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .ContainsBeingProcessed = containDeleted.ToString()
        }
        Dim apiResults As QhYappliPremiumReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPremiumReadApiResults)(
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

    ''' <summary>
    ''' プレミアム会員のメンバーシップを登録します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePremiumPayJPWriteApi(mainModel As QolmsYappliModel, memberManageNo As Long,
                                                   memberShipType As Byte, customerId As String, subscriptionId As String,
                                                   continueAccountStartDate As Date, startDate As Date, endDate As Date, statusNo As Integer,
                                                    additionalSet As String, processingInProgress As Boolean) As QhYappliPremiumWriteApiResults

        Dim apiArgs As New QhYappliPremiumWriteApiArgs(
            QhApiTypeEnum.YappliPremiumWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .MemberManageNo = memberManageNo.ToString(),
            .MemberShipType = memberShipType.ToString(),
            .PaymentType = Convert.ToByte(QyPaymentTypeEnum.pay_jp).ToString(),
            .CustomerId = customerId.ToString(),
            .SubscriptionId = subscriptionId.ToString(),
            .ContinueAccountStartDate = continueAccountStartDate.ToApiDateString(),
            .StartDate = startDate.ToApiDateString(),
            .EndDate = endDate.ToApiDateString(),
            .StatusNo = statusNo.ToString(),
            .AdditionalSet = additionalSet,
            .DeleteFlag = processingInProgress.ToString()
        }
        Dim apiResults As QhYappliPremiumWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPremiumWriteApiResults)(
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

    ''' <summary>
    ''' ステータス０　会員情報の新規作成。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="PaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateNewRecord(mainModel As QolmsYappliModel, ByRef paymentModel As PaymentInf) As Boolean
        ' memberManageNo を採番して返す必要あり　DeleteFlag=True
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, paymentModel.MemberManageNo, QyMemberShipTypeEnum.LimitedTime, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 0, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        If result Then
            paymentModel.MemberManageNo = apiResult.MemberManageNo.TryToValueType(Long.MinValue)
        End If
        Return result
    End Function

    ''' <summary>
    ''' ステータス１　顧客情報の作成（成否によらず）
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function UpdatePremiumRecordToCustomerCreated(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        '顧客情報作成のリクエストOKならステータスアップデート。NGでもエラーコードアップデート。DeleteFlag=True
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, paymentModel.MemberManageNo, QyMemberShipTypeEnum.LimitedTime, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 1, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    ''' <summary>
    ''' ステータス２　サブスクリプション情報の作成（成否によらず）
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function UpdatePremiumRecordToSubscriptionCreated(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        'サブスクリプション作成のリクエストOKならステータスアップデート。NGでもエラーコードアップデート。DeleteFlag=True
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, paymentModel.MemberManageNo, QyMemberShipTypeEnum.LimitedTime, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 2, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    ''' <summary>
    ''' ステータス３　継続課金登録完了。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function UpdatePremiumRecordToSuccessPremiumRegist(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        '決済認可要求をしてOKならステータスアップデート。NGでもエラーコードアップデート。DeleteFlag=True
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, paymentModel.MemberManageNo, QyMemberShipTypeEnum.LimitedTime, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 3, paymentModel.AdditionalSet, False)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    ''' <summary>
    ''' ステータス９　継続課金登録失敗。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function UpdatePremiumRecordToFailedPremiumRegist(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        '継続課金登録をしてNGでもエラーコードアップデート
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, paymentModel.MemberManageNo, QyMemberShipTypeEnum.LimitedTime, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 9, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    Private Shared Function EncriptString(str As String) As String

        If String.IsNullOrWhiteSpace(str) Then Return String.Empty

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            Return crypt.EncryptString(str)
        End Using

    End Function

    Private Shared Function DecriptString(str As String) As String

        If String.IsNullOrWhiteSpace(str) Then Return String.Empty

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            Return crypt.DecryptString(str)
        End Using

    End Function

    Private Shared Function SendMail(message As String) As String

        Dim br As New StringBuilder()
        br.AppendLine(String.Format("pay.jp エラーです。"))
        br.AppendLine(message)
        Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(br.ToString())


    End Function


    Private Shared Function CreateErrorMessage(ex As Exception) As String

        Dim str As New StringBuilder()
        Dim exp As Exception = ex

        If TypeOf exp Is AggregateException Then

            Dim ag As AggregateException = DirectCast(exp, AggregateException)

            For Each item As Exception In ag.InnerExceptions
                Dim exp1 As Exception = item
                For i As Integer = 1 To 10
                    str.AppendLine(exp1.Message)
                    exp1 = exp1.InnerException
                    If exp1 Is Nothing Then
                        Exit For
                    End If
                Next
            Next
        Else

            For i As Integer = 1 To 10
                str.AppendLine(exp.Message)
                exp = exp.InnerException
                If exp Is Nothing Then
                    Exit For
                End If
            Next
        End If


        Return str.ToString()

    End Function


    ''' <summary>
    ''' 顧客情報の新規登録
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <param name="token"></param>
    ''' <returns>1:成功、2：成功（有効期限が初回課金日以前）、9：失敗</returns>
    ''' <remarks></remarks>
    Private Shared Function CreateCustomer(mainModel As QolmsYappliModel, paymentModel As PaymentInf, token As String) As Integer

        Dim status As Integer = 0
        Try
            'プランの取得先を設定から指定
            Dim list As PsyJpSettingJson = PremiumPayJPWorker.SettingJson(PLAN_AMOUNT)

            Dim amount As Integer = Integer.MinValue
            For Each item As PsyJpSettingJsonItem In list.Settings
                If Date.Parse(item.StartDate) <= paymentModel.ContinueAccountStartDate AndAlso paymentModel.ContinueAccountStartDate <= Date.Parse(item.EndDate) Then
                    amount = Integer.Parse(item.Amount)
                    Exit For
                End If
            Next

            '// Pay.JPの課金登録処理
            ''//プランのリストを取得
            Dim plan As PayJpPlanOfJson = PremiumPayJPWorker.ExecutePlanListReadApi(amount)
            PremiumPayJPWorker.CreateNewRecord(mainModel, paymentModel) '0

            If plan Is Nothing Then
                PremiumPayJPWorker.SendMail("新規登録:plan取得の失敗 status = 0 ")
                Return 9
            End If

            ''//顧客情報の登録
            Dim customer As PayJpNewCustomerApiResults = PremiumPayJPWorker.ExecuteNewCustomerWriteApi(token)
            status = 1
            PremiumPayJPWorker.UpdatePremiumRecordToCustomerCreated(mainModel, paymentModel) '1

            If customer Is Nothing Then
                PremiumPayJPWorker.SendMail("新規登録:customer登録の失敗 status = 1 ")
                Return 9
            End If

            '顧客Id　暗号化
            paymentModel.CustomerId = PremiumPayJPWorker.EncriptString(customer.id)

            ''//課金情報の登録
            Dim subscription As PayJpNewSubscriptionApiResults = PremiumPayJPWorker.ExecuteNewSubscriptionWriteApi(customer.id, plan.id, paymentModel.ContinueAccountStartDate)
            status = 2
            PremiumPayJPWorker.UpdatePremiumRecordToSubscriptionCreated(mainModel, paymentModel) '2

            If subscription Is Nothing Then
                PremiumPayJPWorker.SendMail("新規登録:subscription登録の失敗 status = 2  customer;" & PremiumPayJPWorker.EncriptString(customer.id))
                Return 9
            End If

            'サブスクリプションId
            paymentModel.SubscriptionId = PremiumPayJPWorker.EncriptString(subscription.id)

            '課金情報が有効の場合
            If Not subscription Is Nothing Then
                status = 3
                PremiumPayJPWorker.UpdatePremiumRecordToSuccessPremiumRegist(mainModel, paymentModel) '3
                '有効期限の確認
                Dim cardExpDate As Date = New Date(customer.cards.data.Item(0).exp_year, customer.cards.data.Item(0).exp_month, Date.DaysInMonth(customer.cards.data.Item(0).exp_year, customer.cards.data.Item(0).exp_month))
                If cardExpDate > paymentModel.ContinueAccountStartDate Then
                    Return 1
                Else
                    Return 2
                End If
            Else
                status = 9
                PremiumPayJPWorker.UpdatePremiumRecordToFailedPremiumRegist(mainModel, paymentModel) '9
                PremiumPayJPWorker.SendMail("新規登録:subscription登録の失敗 status = 9  customer;" & PremiumPayJPWorker.EncriptString(customer.id))
                Return 9

            End If

        Catch ex As Exception

            'エラー暗号化してメール、アクセスログ
            'メール


            Dim bodyString As New StringBuilder()
            bodyString.AppendLine(String.Format("新規登録:pay.jp の登録エラーです。 status = {0}", status))
            bodyString.AppendLine(String.Format("以下エラーメッセージ"))
            bodyString.AppendLine(PremiumPayJPWorker.EncriptString(CreateErrorMessage(ex)))

            'Dim str As New StringBuilder()
            'str.AppendLine(ex.Message)

            'If Not ex.InnerException Is Nothing Then
            '    str.AppendLine(ex.InnerException.Message)
            'End If
            'bodyString.AppendLine(PremiumPayJPWorker.EncriptString(str.ToString()))
            Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())

            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(CreateErrorMessage(ex)))

            Return 9
        End Try

    End Function

    ''' <summary>
    ''' クレジットカードの再登録（登録済みのカードを削除して新しいカード情報を登録）
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <param name="token"></param>
    ''' <returns>1:成功、2：成功（有効期限が初回課金日以前）、9：失敗</returns>
    ''' <remarks></remarks>
    Private Shared Function UpdateCard(mainModel As QolmsYappliModel, paymentModel As PaymentInf, token As String) As Integer

        Try
            '顧客情報を取得
            Dim customer As PayJpGetCustomerApiResults = PremiumPayJPWorker.ExecutecustomerReadApi(PremiumPayJPWorker.DecriptString(paymentModel.CustomerId))

            If customer Is Nothing Then
                PremiumPayJPWorker.SendMail("再登録:customer取得の失敗")
                Return 9
            End If

            '既存のクレジットカード情報の削除
            For Each card As PayJpCardOfJson In customer.cards.data
                PremiumPayJPWorker.ExecuteDeleteCustomerCardWriteApi(card.id, PremiumPayJPWorker.DecriptString(paymentModel.CustomerId))
            Next

            '新しいクレジットカード情報の登録
            Dim newCard As PayJpNewCustomerCardApiResults = PremiumPayJPWorker.ExecuteNewCustomerCardWriteApi(PremiumPayJPWorker.DecriptString(paymentModel.CustomerId), token)

            If newCard Is Nothing Then
                PremiumPayJPWorker.SendMail("再登録:newCard登録の失敗 customer;" & PremiumPayJPWorker.EncriptString(customer.id))
                Return 9
            End If

            '有効期限の確認
            Dim cardExpDate As Date = New Date(newCard.exp_year, newCard.exp_month, Date.DaysInMonth(newCard.exp_year, newCard.exp_month))
            If cardExpDate > paymentModel.ContinueAccountStartDate Then
                Return 1
            Else
                Return 2
            End If

        Catch ex As Exception
            'メール
            Dim bodyString As New StringBuilder()
            bodyString.AppendLine(String.Format("再登録:pay.jp のカード更新エラーです。")) 'status保留
            bodyString.AppendLine(String.Format("以下エラーメッセージ"))
            bodyString.AppendLine(PremiumPayJPWorker.EncriptString(CreateErrorMessage(ex)))

            'Dim str As New StringBuilder()
            'str.AppendLine(ex.Message)
            'If Not ex.InnerException Is Nothing Then
            '    str.AppendLine(ex.InnerException.Message)
            'End If
            'bodyString.AppendLine(PremiumPayJPWorker.EncriptString(str.ToString()))

            Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())

            'アクセスログ
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(CreateErrorMessage(ex)))

            Return 9

        End Try

    End Function

    Private Shared Function SettingJson(json As String) As PsyJpSettingJson

        'サンプルJSON（あとで設定移動）
        'Dim jsonSet As New PsyJpSettingJson() With {
        '    .Settings = {
        '    New PsyJpSettingJsonItem() With {.Amount = 324D.ToString(), .StartDate = New Date(2019, 5, 28).ToApiDateString(), .EndDate = New Date(2019, 9, 30).ToApiDateString()},
        '    New PsyJpSettingJsonItem() With {.Amount = 330D.ToString(), .StartDate = New Date(2019, 10, 1).ToApiDateString(), .EndDate = Date.MaxValue.ToApiDateString()}
        '    }.ToList()
        '}

        'Dim jsonstr As String = String.Empty
        'Try
        '    jsonstr = sr.Serialize(jsonSet)
        'Catch ex As Exception
        'End Try
        'jsonstr = jsonstr.Replace("""", "'")

        'ここまで
        json = json.Replace("'", """")

        'デシリアライズ
        Dim sr As New QsJsonSerializer()
        Dim jsonResult As New PsyJpSettingJson()
        Try
            jsonResult = sr.Deserialize(Of PsyJpSettingJson)(json)
        Catch ex As Exception
        End Try

        Return jsonResult

    End Function


#End Region

#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PremiumPayJPViewModel 'vewmodel

        'pay.Jpキーの取得
        Dim encrypt As String = System.Web.Configuration.WebConfigurationManager.AppSettings("UseEncryptedPayJpApiSettings")
        Dim key As String = System.Web.Configuration.WebConfigurationManager.AppSettings("PayJpApiPublicKey")
        Dim publicKey As String = String.Empty

        If Not String.IsNullOrWhiteSpace(key) Then
            If encrypt.TryToValueType(False) Then
                publicKey = PremiumPayJPWorker.DecriptString(key)
            Else
                publicKey = key
            End If
        End If

        '課金開始日

        '課金額


        Dim result As New PremiumPayJPViewModel() With {
            .Key = publicKey
        }

        Return result

    End Function

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As PremiumPayJPViewModel 'vewmodel

        Dim result As New PremiumPayJPViewModel()

        '顧客情報を取得
        If Not String.IsNullOrWhiteSpace(paymentModel.CustomerId) Then
            Dim customer As PayJpGetCustomerApiResults = PremiumPayJPWorker.ExecutecustomerReadApi(PremiumPayJPWorker.DecriptString(paymentModel.CustomerId))

            If customer.cards.count > 0 Then
                Dim card As New CardItem()
                card.Brand = customer.cards.data(0).brand
                card.Last4 = customer.cards.data(0).last4
                card.ExpYear = customer.cards.data(0).exp_year
                card.ExpMonth = customer.cards.data(0).exp_month
                result.card = card
            End If
        End If

        'pay.Jpキーの取得
        Dim encrypt As String = System.Web.Configuration.WebConfigurationManager.AppSettings("UseEncryptedPayJpApiSettings")
        Dim key As String = System.Web.Configuration.WebConfigurationManager.AppSettings("PayJpApiPublicKey")
        Dim publicKey As String = String.Empty

        If Not String.IsNullOrWhiteSpace(key) Then
            If encrypt.TryToValueType(False) Then
                publicKey = PremiumPayJPWorker.DecriptString(key)
            Else
                publicKey = key
            End If
        End If

        result.Key = publicKey

        '課金開始日
        If paymentModel.ContinueAccountStartDate = Date.MinValue Then
            '' 課金の回数（2回目以降は即時課金）で課金日を計算
            'If paymentModel.IsOldPaymentRecordExists Then
            '    result.StartDate = Date.Now.AddMonths(1)
            'Else
            '    result.StartDate = Date.Now.AddMonths(3)
            'End If
            ' 2022/02/28 のリリース 以降は常に翌月から課金開始
            result.StartDate = Date.Now.AddMonths(1)
        Else
            result.StartDate = paymentModel.ContinueAccountStartDate
        End If

        '課金額
        Dim list As PsyJpSettingJson = PremiumPayJPWorker.SettingJson(PLAN_AMOUNT)

        Dim amount As Integer = Integer.MinValue
        For Each item As PsyJpSettingJsonItem In list.Settings
            If Date.Parse(item.StartDate) <= paymentModel.ContinueAccountStartDate AndAlso paymentModel.ContinueAccountStartDate <= Date.Parse(item.EndDate) Then
                amount = Integer.Parse(item.Amount)
                Exit For
            End If
        Next

        result.Amount = amount

        Return result

    End Function

    ''' <summary>
    ''' pay.jpの登録
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="token"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns>1:成功、2：成功（有効期限が初回課金日以前）、9：失敗</returns>
    ''' <remarks></remarks>
    Public Shared Function Register(mainModel As QolmsYappliModel, token As String, ByRef paymentModel As PaymentInf) As Integer

        Dim result As Integer = Integer.MinValue

        ' 現時点のメンバーシップ情報の有無を確認
        Dim menbersip As QhYappliPremiumReadApiResults = PremiumPayJPWorker.ExecutePremiumReadApi(mainModel, False)

        If Long.Parse(menbersip.MemberManageNo) > 0 AndAlso Byte.Parse(menbersip.MemberShipType) > 1 AndAlso Byte.Parse(menbersip.PaymentType) = 1 Then
            'メンバーシップがある場合ですでにプレミアム会員
            '課金情報を確認して au→false  / payJP→支払い情報ありならfalse
            Return 9
        End If

        If Integer.Parse(menbersip.StatusNo) = 3 Then
            'カード情報の更新
            result = PremiumPayJPWorker.UpdateCard(mainModel, paymentModel, token)
        Else
            '日付のチェック'検証用に日付再取得
            If paymentModel.ContinueAccountStartDate = Date.MinValue OrElse AuPaymentAccessWorker.REQUESTURI.Contains("test.") Then

                '' 課金の回数（2回目以降は即時課金）で課金日を計算
                'Dim startDate As Date = Date.MinValue
                'If menbersip.IsOldPaymentRecordExists.TryToValueType(False) Then
                '    startDate = Date.Now.AddMonths(1)
                'Else
                '    startDate = Date.Now.AddMonths(3)
                'End If
                ' 2022/02/28 のリリース 以降は常に翌月から課金開始
                Dim startDate As Date = Date.Now.AddMonths(1)

                '課金日の午前9：00に設定（pay.jp側の課金サイクルの都合）
                paymentModel.ContinueAccountStartDate = New Date(startDate.Year, startDate.Month, 1).AddHours(9)
            Else
                '課金日の午前9：00に設定
                If paymentModel.ContinueAccountStartDate.Hour = 0 Then
                    paymentModel.ContinueAccountStartDate = paymentModel.ContinueAccountStartDate.AddHours(9)
                End If
            End If
            If paymentModel.StartDate = Date.MinValue Then
                paymentModel.StartDate = Date.Now
            End If
            If paymentModel.EndDate = Date.MinValue Then
                paymentModel.EndDate = Date.MaxValue
            End If

            '新規登録
            result = PremiumPayJPWorker.CreateCustomer(mainModel, paymentModel, token)
        End If

        ''ポイント付与
        'If result = 1 OrElse result = 2 Then
        '    '前にプレミアム登録がなくて、ステータスが予約（-2）か、未登録（iteger.minvalue）の時
        '    Dim status As Integer = Integer.Parse(menbersip.StatusNo)
        '    If menbersip.IsOldPaymentRecordExists.TryToValueType(False) = False AndAlso (status = -2 OrElse status = Integer.MinValue) Then
        '        ' プレミアム会員初登録時にポイント付与
        '        'Dim actionDate As Date = Now
        '        '' Dim limit As Date = Now.Date.AddMonths(7).AddDays(-1)
        '        'Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
        '        'Dim pointItemList As New List(Of QolmsPointGrantItem)
        '        'pointItemList.Add(New QolmsPointGrantItem(QyMemberShipTypeEnum.LimitedTime, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.InitialRegistration, limit)) '初回プレミアム登録
        '        'Task.Run(
        '        '    Sub()
        '        '        QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
        '        '        mainModel.AuthorAccount.AccountKey, pointItemList)
        '        '    End Sub
        '        ')
        '    End If
        'End If

        Return result

    End Function

    <Obsolete("現在はこの処理は未使用のはず。")>
    Public Shared Function Reserve(mainModel As QolmsYappliModel) As Boolean

        ' pay.JPの支払い予約ステータスを作成して、後日入力ができるようにする
        ' 6月中旬まで

        ' 現時点のメンバーシップ情報の有無を確認
        Dim menbersip As QhYappliPremiumReadApiResults = PremiumPayJPWorker.ExecutePremiumReadApi(mainModel, False)
        ' 課金の回数（2回目以降は即時課金）
        Dim menbersipType As Byte = Byte.Parse(menbersip.MemberShipType)
        If menbersipType = 2 OrElse menbersipType = 3 Then
            'すでにプレミアムだったら抜ける
            Return False
        End If

        ' 処理ステータス　予約
        Dim status As Integer = -2
        Dim now As Date = Date.Now
        'Dim chargeMonth As Date = Date.MinValue
        'If menbersip.IsOldPaymentRecordExists.TryToValueType(False) Then
        '    chargeMonth = now.AddMonths(1)
        'Else
        '    chargeMonth = now.AddMonths(3)
        'End If
        ' 2022/02/28 のリリース 以降は常に翌月から課金開始
        Dim chargeMonth As Date = now.AddMonths(1)

        '予約登録
        Dim result As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, Long.MinValue, QyMemberShipTypeEnum.LimitedTime, String.Empty, String.Empty, New Date(chargeMonth.Year, chargeMonth.Month, 1), now, Date.MaxValue, status, String.Empty, False)

        Return Long.Parse(result.MemberManageNo) > 0

    End Function

    '退会手続き
    ''' <summary>
    ''' ステータス１０　継続課金キャンセル登録成功。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="PaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToCancelPremiumRegist(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        'ステータス10、終了日をSetする。DeleteFlag=True
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumPayJPWorker.ExecutePremiumPayJPWriteApi(mainModel, paymentModel.MemberManageNo, paymentModel.MemberShipType, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.Now, 10, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function
    ''' <summary>
    ''' pay.jp解約処理
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RequestContBillCancel(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False

        ' 現時点のメンバーシップ情報の有無を確認
        Dim menbersip As QhYappliPremiumReadApiResults = PremiumPayJPWorker.ExecutePremiumReadApi(mainModel, False)

        If Byte.Parse(menbersip.MemberShipType) = 2 OrElse Byte.Parse(menbersip.MemberShipType) = 3 Then
            'プレミアム会員の場合
            '予約の場合はスルー
            If Integer.Parse(menbersip.StatusNo) = -2 Then
                Return True
            End If

            Try
                '顧客情報を取得
                Dim customer As PayJpGetCustomerApiResults = PremiumPayJPWorker.ExecutecustomerReadApi(PremiumPayJPWorker.DecriptString(menbersip.CustomerId))

                If customer Is Nothing Then
                    PremiumPayJPWorker.SendMail("解約:custome取得の失敗")
                    Return False
                End If
                'サブスクリプションの削除
                Dim subscription As PayJpDeleteSubscriptionApiResults = PremiumPayJPWorker.ExecuteDeleteSubscriptionWriteApi(PremiumPayJPWorker.DecriptString(menbersip.SubscriptionId))

                If subscription Is Nothing Then
                    PremiumPayJPWorker.SendMail("解約:subscription削除の失敗 customer;" & PremiumPayJPWorker.EncriptString(customer.id))
                    Return False
                End If

                'カード情報の削除（リストを削除たぶん１枚しかない）
                For Each card As PayJpCardOfJson In customer.cards.data
                    PremiumPayJPWorker.ExecuteDeleteCustomerCardWriteApi(card.id, PremiumPayJPWorker.DecriptString(menbersip.CustomerId))
                Next

            Catch ex As Exception
                'メール
                Dim bodyString As New StringBuilder()
                bodyString.AppendLine(String.Format("解約:pay.jp の解約エラーです。")) 'status保留
                bodyString.AppendLine(String.Format("以下エラーメッセージ"))
                bodyString.AppendLine(PremiumPayJPWorker.EncriptString(CreateErrorMessage(ex)))
                'Dim str As New StringBuilder()
                'str.AppendLine(ex.Message)
                'If Not ex.InnerException Is Nothing Then
                '    str.AppendLine(ex.InnerException.Message)
                'End If
                'bodyString.AppendLine(PremiumPayJPWorker.EncriptString(str.ToString()))

                Dim task As Task(Of Boolean) = NoticeMailWorker.SendAsync(bodyString.ToString())
                'アクセスログ
                AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(CreateErrorMessage(ex)))

                Return False
            End Try

        End If

        Return True

    End Function

    Friend Shared Function RedirectTds(token As String, url As String) As String

        'pay.Jpキーの取得(共通化）
        Dim encrypt As String = System.Web.Configuration.WebConfigurationManager.AppSettings("UseEncryptedPayJpApiSettings")
        Dim key As String = System.Web.Configuration.WebConfigurationManager.AppSettings("PayJpApiPublicKey")
        Dim publicKey As String = String.Empty

        If Not String.IsNullOrWhiteSpace(key) Then
            If encrypt.TryToValueType(False) Then
                publicKey = PremiumPayJPWorker.DecriptString(key)
            Else
                publicKey = key
            End If
        End If

        Dim siteurl As String = System.Web.Configuration.WebConfigurationManager.AppSettings("QolmsYappliSiteUri")

        Dim baseURL As Uri = New Uri(siteurl)
        Dim apiArgs As New PayJpToken3dSecuerApiArgs() With {
           .resource_id = token,
           .publickey = publicKey,
           .back_url = GenerateJwtUrl(New Uri(baseURL, url).AbsoluteUri)
       }

        Return QsPayJpApiManager.GetUrlAsync(apiArgs).Result

    End Function

    Friend Shared Function TdsFinish(token As String) As String

        Dim apiArgs As New PayJpToken3dSecuerFinishApiArgs() With {
            .id = token
        }

        Dim apiResults As PayJpToken3dSecuerFinishApiResults = QsPayJpApiManager.Execute(Of PayJpToken3dSecuerFinishApiArgs, PayJpToken3dSecuerFinishApiResults)(apiArgs)

        With apiResults

            If .IsSuccess Then
                Return .id

            End If

        End With
        Return String.Empty

    End Function

    ' JWT URL生成メソッド
    Public Shared Function GenerateJwtUrl(baseUrl As String, Optional parameters As Dictionary(Of String, String) = Nothing) As String

        'pay.Jpキーの取得(共通化）
        Dim encrypt As String = System.Web.Configuration.WebConfigurationManager.AppSettings("UseEncryptedPayJpApiSettings")
        Dim pub As String = System.Web.Configuration.WebConfigurationManager.AppSettings("PayJpApiPublicKey")
        Dim seq As String = System.Web.Configuration.WebConfigurationManager.AppSettings("PayJpApiSecretKey")
        Dim publicKey As String = String.Empty
        Dim secretKey As String = String.Empty

        If Not String.IsNullOrWhiteSpace(pub) Then
            If encrypt.TryToValueType(False) Then
                publicKey = PremiumPayJPWorker.DecriptString(pub)
                secretKey = PremiumPayJPWorker.DecriptString(seq)
            Else
                publicKey = pub
                secretKey = seq
            End If
        End If
        Dim queryParams As String = String.Empty
        Dim encodedUrl As String = String.Empty
        ' パラメータのURLエンコード
        If parameters IsNot Nothing Then
            queryParams = String.Join("&", parameters.Select(Function(p) $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"))
            encodedUrl = $"{baseUrl}?{queryParams}"

        Else
            encodedUrl = $"{baseUrl}"
        End If

        ' JWTペイロード
        Dim tokenHandler As New JwtSecurityTokenHandler()
        Dim key As Byte() = Encoding.ASCII.GetBytes(secretKey)

        Dim tokenDescriptor As New SecurityTokenDescriptor() With {
            .Subject = New ClaimsIdentity(New Claim() {New Claim("url", encodedUrl)}),
            .Expires = DateTime.UtcNow.AddMinutes(5),
            .SigningCredentials = New SigningCredentials(New SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        }

        Dim token As SecurityToken = tokenHandler.CreateToken(tokenDescriptor)
        Return tokenHandler.WriteToken(token)

    End Function

    Public Shared Function CreateAgreeViewModel(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum) As PremiumPayJPAgreeViewModel

        Dim nextPageUrl As String = String.Empty
        If pageNo = QyPageNoTypeEnum.PremiumPayJpRegist Then
            nextPageUrl = "../premium/payjpcardregister"
        ElseIf pageNo = QyPageNoTypeEnum.PremiumPayJpEdit Then
            nextPageUrl = "../premium/payjpcardupdate"
        Else
            Throw New InvalidOperationException("最初からやり直してください。")
        End If

        Return New PremiumPayJPAgreeViewModel() With {
            .Terms = TarmsWorker.GetTermsContent(mainModel, 105),
            .Url = nextPageUrl
        }

    End Function




#End Region

End Class
