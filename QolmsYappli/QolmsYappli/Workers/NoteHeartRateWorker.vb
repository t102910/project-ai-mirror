Imports System.Globalization
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsKaradaKaruteApiCoreV1

Friend NotInheritable Class NoteHeartRateWorker

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

    Private Shared Function ExecuteNoteVitalReadApi(mainModel As QolmsYappliModel) As QhYappliNoteVitalReadApiResults

        Dim apiArgs As New QhYappliNoteVitalReadApiArgs(
            QhApiTypeEnum.YappliNoteVitalRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .PageNo = Convert.ToByte(QyPageNoTypeEnum.NoteVital).ToString(),
            .IsInitialize = (mainModel.AuthorAccount.Height <= Decimal.Zero OrElse Not mainModel.AuthorAccount.StandardValues.Any()).ToString(),
            .SexType = mainModel.AuthorAccount.SexType.ToString(),
            .Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString()
        }
        Dim apiResults As QhYappliNoteVitalReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteVitalReadApiResults)(
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


    Private Shared Function ExecuteNoteVitalDetailReadApi(mainModel As QolmsYappliModel, startDate As Date, endDate As Date, vitalType As QyVitalTypeEnum) As QhYappliNoteVitalDetailReadApiResults

        Dim apiArgs As New QhYappliNoteVitalDetailReadApiArgs(
            QhApiTypeEnum.YappliNoteVitalDetailRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .StartDate = startDate.ToApiDateString(),
            .EndDate = endDate.ToApiDateString(),
            .VitalType = vitalType.ToString()
        }
        Dim apiResults As QhYappliNoteVitalDetailReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteVitalDetailReadApiResults)(
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

        Return New QhYappliNoteVitalDetailReadApiResults()

    End Function

    Private Shared Sub GetTargetValues(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, ByRef refValue1 As Decimal, ByRef refValue2 As Decimal, ByRef refValue3 As Decimal, ByRef refValue4 As Decimal)

        refValue1 = Decimal.MinusOne
        refValue2 = Decimal.MinusOne
        refValue3 = Decimal.MinusOne
        refValue4 = Decimal.MinusOne

        Select Case vitalType
            Case QyVitalTypeEnum.BodyWeight
                ' 体重
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

                ' BMI
                refValue3 = 18.5D
                refValue4 = 24.9D

            Case QyVitalTypeEnum.BloodPressure
                ' 血圧下
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureLower, refValue1, refValue2)

                ' 血圧上
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureUpper, refValue3, refValue4)

            Case QyVitalTypeEnum.BloodSugar
                ' 空腹時血糖値
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarFasting, refValue1, refValue2)

                ' その他血糖値
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarOther, refValue3, refValue4)

            Case QyVitalTypeEnum.Glycohemoglobin,
                QyVitalTypeEnum.BodyWaist,
                QyVitalTypeEnum.BodyTemperature,
                QyVitalTypeEnum.Steps,
                QyVitalTypeEnum.BodyFatPercentage,
                QyVitalTypeEnum.MuscleMass,
                QyVitalTypeEnum.BoneMass,
                QyVitalTypeEnum.VisceralFat,
                QyVitalTypeEnum.BasalMetabolism,
                QyVitalTypeEnum.BodyAge,
                QyVitalTypeEnum.TotalBodyWater

                ' 脈拍、HbA1c、腹囲、体温、歩数、体脂肪率、筋肉量、推定骨量、内脂肪レベル、基礎代謝、体内年齢、水分率
                TargetValueWorker.GetTargetValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

            Case QyVitalTypeEnum.Pulse

                ' 固定値で良い
                refValue1 = 50
                refValue2 = 120

            Case QyVitalTypeEnum.Mets

                ' 基準値は存在しない
                refValue1 = 1
                refValue2 = 1

        End Select

    End Sub

#End Region

#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, periodType As QyPeriodTypeEnum, startDate As DateTime, endDate As DateTime) As NoteHeartRateViewModel

        Dim viewModel As NoteHeartRateViewModel = Nothing

        ' 基準値は固定値になったのでコメントアウトしておきます
        'With NoteHeartRateWorker.ExecuteNoteVitalReadApi(mainModel)
        '    ' バイタル標準値
        '    If mainModel.AuthorAccount.StandardValues Is Nothing OrElse Not mainModel.AuthorAccount.StandardValues.Any() Then StandardValueWorker.SetStandardValue(mainModel, .StandardValueN)
        '    ' バイタル目標値
        '    If mainModel.AuthorAccount.TargetValues Is Nothing OrElse Not mainModel.AuthorAccount.TargetValues.Any() Then TargetValueWorker.SetTargetValue(mainModel, .TargetValueN)
        'End With

        ' 目標値
        Dim targetValues() As Decimal = {Decimal.Zero, Decimal.Zero, Decimal.Zero, Decimal.Zero}
        NoteHeartRateWorker.GetTargetValues(mainModel, QyVitalTypeEnum.Pulse, targetValues(0), targetValues(1), targetValues(2), targetValues(3))

        With NoteHeartRateWorker.ExecuteNoteVitalDetailReadApi(mainModel, startDate, endDate, QyVitalTypeEnum.Pulse)

            ' アラートメッセージ
            Dim noticeList As List(Of String) = New List(Of String)

            ' メッセージに日付が入っているので日付はとりあえず出さない。
            For Each item As QhApiHealthRecordAlertItem In .HealthRecordAlertN
                'Dim msgDate As Date = item.RecordDate.TryToValueType(Date.MinValue)
                'noticeList.Add(msgDate.ToString("yyyy/MM/dd HH:mm") + " " + item.Message)
                noticeList.Add(item.Message)
            Next

            Dim isChallenge As Boolean = PortalChallengeWorker.IsChallengeEntry(mainModel, Guid.Parse("CDF50EC6-DA20-4D47-84DE-6F14BF9CEC1F"))

            viewModel = New NoteHeartRateViewModel(mainModel,
                                                   .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
                                                   targetValues(0),
                                                   targetValues(1),
                                                   periodType,
                                                   startDate,
                                                   endDate,
                                                   noticeList,
                                                   isChallenge)

        End With

        Return viewModel

    End Function

#End Region

End Class
