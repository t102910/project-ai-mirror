Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PremiumWorker

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


    Private Shared Function ExecuteMemberShipWriteApi(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf,
                                                      processingInProgress As Boolean, memberShipType As Byte, statusNo As Integer) As QhYappliPremiumWriteApiResults

        Dim apiArgs As New QhYappliPremiumWriteApiArgs(
            QhApiTypeEnum.YappliPremiumWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .AuErrorCode = Right(auPaymentModel.AuResultCode, 5),
            .ContinueAccountId = auPaymentModel.ContinueAccountId,
            .ContinueAccountStartDate = auPaymentModel.ContinueAccountStartDate.ToApiDateString(),
            .DeleteFlag = processingInProgress.ToString(),
            .EndDate = auPaymentModel.EndDate.ToApiDateString(),
            .MemberManageNo = auPaymentModel.MemberManageNo.ToString(),
            .MemberShipType = memberShipType.ToString(),
            .PaymentType = Convert.ToByte(QyPaymentTypeEnum.au).ToString(),
            .StartDate = auPaymentModel.StartDate.ToApiDateString(),
            .StatusNo = statusNo.ToString
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



#End Region

#Region "Public Method"

    ''' <summary>
    ''' DBに登録されている有効なプレミアム会員情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetPremiumRecord(mainModel As QolmsYappliModel) As PaymentInf
        Dim result As New PaymentInf(mainModel.AuthorAccount.OpenId)
        Dim apiResult As QhYappliPremiumReadApiResults = PremiumWorker.ExecutePremiumReadApi(mainModel, False)
        If apiResult.IsSuccess.TryToValueType(False) Then
            result.ContinueAccountId = apiResult.ContinueAccountId
            result.ContinueAccountStartDate = apiResult.ContinueAccountStartDate.TryToValueType(Date.MinValue)
            result.EndDate = apiResult.EndDate.TryToValueType(Date.MaxValue)
            result.MemberManageNo = apiResult.MemberManageNo.TryToValueType(Long.MinValue)
            result.StartDate = apiResult.StartDate.TryToValueType(Date.MinValue)
            result.MemberShipType = apiResult.MemberShipType.TryToValueType(Byte.MinValue)
            result.PaymentType = apiResult.PaymentType.TryToValueType(Byte.MinValue)
            result.CustomerId = apiResult.CustomerId
            result.SubscriptionId = apiResult.SubscriptionId
            result.AdditionalSet = apiResult.AdditionalSet
            result.IsOldPaymentRecordExists = apiResult.IsOldPaymentRecordExists.TryToValueType(False)
            If result.ContinueAccountStartDate = Date.MinValue Then
                'If apiResult.IsOldPaymentRecordExists.TryToValueType(False) Then
                '    '二回目以降は、次の月の1日
                '    result.ContinueAccountStartDate = New Date(Now.Year, Now.Month, 1).AddMonths(1)
                'Else
                '    '課金開始は、3か月後の1日
                '    result.ContinueAccountStartDate = New Date(Now.Year, Now.Month, 1).AddMonths(3)
                'End If

                ' 2022/02/28 のリリース 以降は常に翌月から課金開始
                result.ContinueAccountStartDate = New Date(Now.Year, Now.Month, 1).AddMonths(1)
            End If
        End If
        Return result
    End Function

    ''' <summary>
    ''' プレミアム会員種別を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetMemberShipType(mainModel As QolmsYappliModel) As Byte
        Dim apiResult As QhYappliPremiumReadApiResults = PremiumWorker.ExecutePremiumReadApi(mainModel, False)
        If apiResult.IsSuccess.TryToValueType(False) Then
            Return apiResult.MemberShipType.TryToValueType(Byte.MinValue)
        End If

        Return Byte.MinValue
    End Function
    ''' <summary>
    ''' ステータス０　プレミアム会員登録レコードを作成。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function InsertPremiumRecord(mainModel As QolmsYappliModel, ByRef auPaymentModel As PaymentInf) As Boolean
        ' memberManageNo を採番して返す必要あり　DeleteFlag=True
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, True, QyMemberShipTypeEnum.LimitedTime, 0)
        result = apiResult.IsSuccess.TryToValueType(False)
        If result Then
            auPaymentModel.MemberManageNo = apiResult.MemberManageNo.TryToValueType(Long.MinValue)
        End If
        Return result
    End Function

    ''' <summary>
    ''' ステータス１　決済認可要求の応答取得（成否によらず）
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToPayCertRequested(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        '決済認可要求をしてOKならステータスアップデート。NGでもエラーコードアップデート。DeleteFlag=True
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, True, QyMemberShipTypeEnum.LimitedTime, 1)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    ''' <summary>
    ''' ステータス２　ユーザ認可要求の応答取得（成否によらず）
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToUserPermitBridge(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Boolean
        Dim result As Boolean = False
        '決済認可要求をしてOKならステータスアップデート。NGでもエラーコードアップデート。DeleteFlag=True
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, True, QyMemberShipTypeEnum.LimitedTime, 2)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    ''' <summary>
    ''' ステータス3　継続課金登録完了。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToSuccessPremiumRegist(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Boolean
        '継続課金登録をしてOKならステータス、処理日も取得できるので開始日にSetする。
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, False, QyMemberShipTypeEnum.LimitedTime, 3)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function


    ''' <summary>
    ''' ステータス９　継続課金登録失敗。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToFailedPremiumRegist(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Boolean
        '継続課金登録をしてNGでもエラーコードアップデート
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, True, QyMemberShipTypeEnum.LimitedTime, 9)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    '退会手続き
    ''' <summary>
    ''' ステータス１０　継続課金キャンセル登録成功。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToCancelPremiumRegist(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Boolean
        'ステータス10、終了日をSetする。DeleteFlag=True
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, True, auPaymentModel.MemberShipType, 10)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

    '退会手続き失敗。
    ''' <summary>
    ''' ステータス９　継続課金キャンセル失敗。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="auPaymentModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function UpdatePremiumRecordToCancelPremiumRegistFailed(mainModel As QolmsYappliModel, auPaymentModel As PaymentInf) As Boolean
        'ステータス9。DeleteFlag=True
        Dim result As Boolean = False
        Dim apiResult As QhYappliPremiumWriteApiResults = PremiumWorker.ExecuteMemberShipWriteApi(mainModel, auPaymentModel, False, auPaymentModel.MemberShipType, 11)
        result = apiResult.IsSuccess.TryToValueType(False)
        Return result
    End Function

#End Region

End Class
