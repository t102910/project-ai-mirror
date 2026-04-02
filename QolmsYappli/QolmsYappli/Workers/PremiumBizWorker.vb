Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Runtime.Serialization

Friend NotInheritable Class PremiumBizWorker

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
    Private Shared Function ExecutePremiumWriteApi(mainModel As QolmsYappliModel, memberManageNo As Long,
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
            .PaymentType = Convert.ToByte(QyPaymentTypeEnum.Other).ToString(),
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


#End Region

#Region "Public Method"

    ''' <summary>
    ''' ステータス３　継続課金登録完了。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="paymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToSuccessPremiumRegist(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        '決済認可要求をしてOKならステータスアップデート。
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumBizWorker.ExecutePremiumWriteApi(mainModel, paymentModel.MemberManageNo, paymentModel.MemberShipType, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 3, paymentModel.AdditionalSet, False)
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
    Public Shared Function UpdatePremiumRecordToFailedPremiumRegist(mainModel As QolmsYappliModel, paymentModel As PaymentInf) As Boolean
        '継続課金登録をしてNGでもエラーコードアップデート
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumBizWorker.ExecutePremiumWriteApi(mainModel, paymentModel.MemberManageNo, paymentModel.MemberShipType, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.MaxValue, 9, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
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
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumBizWorker.ExecutePremiumWriteApi(mainModel, paymentModel.MemberManageNo, paymentModel.MemberShipType, paymentModel.CustomerId, paymentModel.SubscriptionId, paymentModel.ContinueAccountStartDate, paymentModel.StartDate, Date.Now, 10, paymentModel.AdditionalSet, True)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function
#End Region

End Class
