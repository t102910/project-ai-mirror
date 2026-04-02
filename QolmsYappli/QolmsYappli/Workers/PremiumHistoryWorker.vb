Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PremiumHistoryWorker

#Region "Constant"

    Private Shared ReadOnly payJpStatusCodeHash As New HashSet(Of String)(
        {
            "200",
            "402"
        }
    )

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

    Private Shared Function ExecutePremiumHistoryReadApi(mainModel As QolmsYappliModel, startDate As Date, endDate As Date) As QhYappliPremiumHistoryReadApiResults

        Dim apiArgs As New QhYappliPremiumHistoryReadApiArgs(
            QhApiTypeEnum.YappliPremiumHistoryRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.PremiumHistory).ToString(),
            .StartDate = startDate.ToApiDateString(),
            .EndDate = endDate.ToApiDateString()
        }
        Dim apiResults As QhYappliPremiumHistoryReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPremiumHistoryReadApiResults)(
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

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PremiumHistoryViewModel

        Dim result As New PremiumHistoryViewModel(mainModel, Nothing)
        Dim nextMonth As Date = Date.Now.AddMonths(1)
        Dim endDate As Date = New Date(nextMonth.Year, nextMonth.Month, 1).AddDays(-1)
        Dim startDate As Date = endDate.AddYears(-1).AddDays(1)

        With PremiumHistoryWorker.ExecutePremiumHistoryReadApi(mainModel, startDate, endDate)
            If .IsSuccess.TryToValueType(False) Then
                Dim logN As New List(Of PaymentLogItem)()

                If .PaymentLogN IsNot Nothing AndAlso .PaymentLogN.Any() Then
                    logN = .PaymentLogN.ConvertAll(Function(i) i.ToPaymentLogItem()).Where(
                        Function(i)

                            Return i.PaymentYear >= 1 _
                                AndAlso i.PaymentYear <= 9999 _
                                AndAlso i.PaymentMonth >= 1 _
                                AndAlso i.PaymentMonth <= 12 _
                                AndAlso (i.PaymentType = 1 Or (i.PaymentType = 2 And PremiumHistoryWorker.payJpStatusCodeHash.Contains(i.StatusCode))) _
                                AndAlso i.Amount >= Decimal.Zero

                        End Function
                    ).ToList()
                End If

                result = New PremiumHistoryViewModel(mainModel, logN)
            End If
        End With

        Return result

    End Function

#End Region

End Class
