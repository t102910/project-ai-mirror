Imports System.Globalization
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsKaradaKaruteApiCoreV1

Friend NotInheritable Class NoteVitalWorker

#Region "Constant"

    Private Const DEFAULT_DAY_COUNT As Integer = 30

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

#Region "健康年齢（ベイジアン）"

    Private Shared Function EditBayesian(
        accountKey As Guid,
        sexType As QySexTypeEnum,
        birthday As Date,
        membershipType As QyMemberShipTypeEnum,
        healthAgeValueN As List(Of QhApiHealthAgeValueItem),
        weight As Decimal,
        height As Decimal,
        apiExecutor As Guid,
        apiExecutorName As String,
        sessionId As String,
        apiAuthorizeKey As Guid
    ) As Boolean

        Dim result As Boolean = False
        Dim dic As New Dictionary(Of QyHealthAgeValueTypeEnum, Tuple(Of Date, Decimal))()

        healthAgeValueN.ForEach(
            Sub(i)
                Dim key As QyHealthAgeValueTypeEnum = i.HealthAgeValueType.TryToValueType(QyHealthAgeValueTypeEnum.None)
                Dim value1 As Date = i.RecordDate.TryToValueType(Date.MinValue)
                Dim value2 As Decimal = i.Value.TryToValueType(Decimal.MinValue)

                If Not dic.ContainsKey(key) _
                    AndAlso value1 <> Date.MinValue _
                    AndAlso value2 > Decimal.Zero Then

                    dic.Add(key, New Tuple(Of Date, Decimal)(value1, value2))
                End If
            End Sub
        )

        If membershipType = QyMemberShipTypeEnum.LimitedTime OrElse membershipType = QyMemberShipTypeEnum.Premium _
            OrElse membershipType = QyMemberShipTypeEnum.Business OrElse membershipType = QyMemberShipTypeEnum.BusinessFree Then
            ' 期間限定プレミアム会員 or プレミアム会員
            If dic.Any() Then
                Dim inputModel As New HealthAgeEditInputModel()

                inputModel.SetValues(dic.First().Value.Item1, dic)

                result = HealthAgeEditWorker.EditBayesian(
                    accountKey,
                    sexType,
                    birthday,
                    membershipType,
                    inputModel,
                    weight,
                    height,
                    apiExecutor,
                    apiExecutorName,
                    sessionId,
                    apiAuthorizeKey
                )
            End If
        Else
            ' 無料会員
            result = True
        End If

        Return result

    End Function

#End Region

#Region "タニタ QR コード"

    ''' <summary>
    ''' タニタ 連携（体重、血圧）が有効かを判定します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="refMemberNo">会員識別番号が格納される変数。</param>
    ''' <returns>
    ''' 連携が有効なら True、
    ''' 無効なら False。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Private Shared Function IsTanitaConnectionEnabled(mainModel As QolmsYappliModel, ByRef refMemberNo As String) As Boolean

        refMemberNo = String.Empty

        Dim result As Boolean = False

        ' 連携の チェック
        Dim devices As List(Of DeviceItem) = PortalTanitaConnectionWorker.GetConnectedDevice(mainModel, refMemberNo)

        If Not String.IsNullOrWhiteSpace(refMemberNo) AndAlso devices.Any() Then
            For Each device As DeviceItem In devices
                If device.DevicePropertyName.TryToValueType(QsKaradaKaruteApiDeviceTypeEnum.None) = QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter AndAlso device.Checked Then
                    ' 体重連携
                    result = True

                    Exit For
                ElseIf device.DevicePropertyName.TryToValueType(QsKaradaKaruteApiDeviceTypeEnum.None) = QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer AndAlso device.Checked Then
                    ' 血圧連携
                    result = True

                    Exit For
                End If
            Next

            If Not result Then refMemberNo = String.Empty
        End If

        Return result

    End Function

    ''' <summary>
    ''' ビュー 内に展開する暗号化された タニタ 会員 QR コード 情報への参照 パラメータ を作成します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <returns>
    ''' 連携が有効なら暗号化された タニタ 会員 QR コード 情報への参照 パラメータ、
    ''' 無効なら String.Empty。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Private Shared Function CreateEncryptedTanitaQrReference(mainModel As QolmsYappliModel) As String

        Dim memberNo As String = String.Empty

        ' 連携の チェック
        If NoteVitalWorker.IsTanitaConnectionEnabled(mainModel, memberNo) AndAlso Not String.IsNullOrWhiteSpace(memberNo) Then
            ' 連携有効
            Using cryptor As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                Return cryptor.EncryptString(
                    New NoteVitalTanitaQrReferenceJsonParameter() With {
                        .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                        .LoginAt = mainModel.AuthorAccount.LoginAt.ToApiDateString(),
                        .MemberNo = memberNo
                    }.ToJsonString()
                )
            End Using
        Else
            ' 連携無効
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' タニタ 会員 QR コード 文字列取得 Web API を実行します。
    ''' </summary>
    ''' <param name="memberNo">会員識別番号。</param>
    ''' <returns>
    ''' タニタ 会員 QR コード 文字列取得 Web API を実行した結果を格納する戻り値 クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Private Shared Function ExecuteGetQrCodeApi(memberNo As String) As GetQrCodeApiResults

        Dim apiArgs As New GetQrCodeApiArgs() With {
            .siteId = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSiteId,
            .sitePass = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSitePassword,
            .memberNo = memberNo
        }
        Dim apiResults As GetQrCodeApiResults = QsKaradaKaruteApiManager.Execute(Of GetQrCodeApiArgs, GetQrCodeApiResults)(apiArgs)

        With apiResults
            If .IsSuccess Then
                Return apiResults
            Else
                Throw New InvalidOperationException(
                    String.Format(
                        "タニタ 会員 QR コード 文字列取得 Web API の実行に失敗しました（memberNo={0}、StatusCode={1}、status={2}、error_code={3}）。",
                        memberNo,
                        apiResults.StatusCode,
                        apiResults.status,
                        apiResults.error_code
                    )
                )
            End If
        End With

    End Function

#End Region

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

    Private Shared Function ExecuteNoteVitalGraphRead(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, dayCount As Integer) As QhYappliNoteVitalGraphReadApiResults

        Dim apiArgs As New QhYappliNoteVitalGraphReadApiArgs(
            QhApiTypeEnum.YappliNoteVitalGraphRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .VitalType = vitalType.ToString(),
            .DayCount = dayCount.ToString()
        }
        Dim apiResults As QhYappliNoteVitalGraphReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteVitalGraphReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            Debug.Print("■" + .HealthAgeValueN.Count.ToString())

            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecuteNoteVitalWriteApi(mainModel As QolmsYappliModel, inputModelN As Dictionary(Of Tuple(Of QyVitalTypeEnum, Date), VitalValueInputModel)) As QhYappliNoteVitalWriteApiResults

        Dim apiArgs As New QhYappliNoteVitalWriteApiArgs(
            QhApiTypeEnum.YappliNoteVitalWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .VitalValueN = inputModelN.ToList().ConvertAll(Function(i) i.Value.ToApiVitalValueItem(i.Key.Item2))
        }
        Dim apiResults As QhYappliNoteVitalWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteVitalWriteApiResults)(
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

    '<Obsolete("廃止予定")>
    'Private Shared Sub GetStandardValues(mainModel As QolmsYappliModel, vitalType As QyVitalTypeEnum, ByRef refValue1 As Decimal, ByRef refValue2 As Decimal, ByRef refValue3 As Decimal, ByRef refValue4 As Decimal)

    '    refValue1 = Decimal.MinusOne
    '    refValue2 = Decimal.MinusOne
    '    refValue3 = Decimal.MinusOne
    '    refValue4 = Decimal.MinusOne

    '    Select Case vitalType
    '        Case QyVitalTypeEnum.BloodPressure
    '            ' 血圧下
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureLower, refValue1, refValue2)

    '            ' 血圧上
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureUpper, refValue3, refValue4)

    '        Case QyVitalTypeEnum.BloodSugar
    '            ' 空腹時血糖値
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarFasting, refValue1, refValue2)

    '            ' その他血糖値
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarOther, refValue3, refValue4)

    '        Case QyVitalTypeEnum.BodyWeight
    '            ' 体重
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

    '            ' BMI
    '            refValue3 = 18.5D
    '            refValue4 = 24.9D

    '        Case QyVitalTypeEnum.Pulse,
    '            QyVitalTypeEnum.Glycohemoglobin,
    '            QyVitalTypeEnum.BodyWaist,
    '            QyVitalTypeEnum.BodyTemperature,
    '            QyVitalTypeEnum.Steps,
    '            QyVitalTypeEnum.BodyFatPercentage,
    '            QyVitalTypeEnum.MuscleMass,
    '            QyVitalTypeEnum.BoneMass,
    '            QyVitalTypeEnum.VisceralFat,
    '            QyVitalTypeEnum.BasalMetabolism,
    '            QyVitalTypeEnum.BodyAge,
    '            QyVitalTypeEnum.TotalBodyWater

    '            ' 脈拍、HbA1c、腹囲、体温、歩数、体脂肪率、筋肉量、推定骨量、内脂肪レベル、基礎代謝、体内年齢、水分率
    '            StandardValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.None, refValue1, refValue2)

    '    End Select

    'End Sub

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

            Case QyVitalTypeEnum.Pulse,
                QyVitalTypeEnum.Glycohemoglobin,
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

        End Select

    End Sub

#End Region

#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As NoteVitalViewModel

        Dim result As NoteVitalViewModel = Nothing ' キャッシュは使わない mainModel.GetInputModelCache(Of NoteVitalViewModel)() ' キャッシュから取得

        If result Is Nothing OrElse result.MembershipType <> mainModel.AuthorAccount.MembershipType Then
            ' キャッシュが無ければ API を実行
            With NoteVitalWorker.ExecuteNoteVitalReadApi(mainModel)
                result = New NoteVitalViewModel(mainModel)

                result.AvailableVitalN = .AvailableVitalN.ConvertAll(
                    Function(i)
                        Return New AvailableVitalItem() With {
                            .VitalType = i.VitalType.TryToValueType(QyVitalTypeEnum.None),
                            .LatestDate = i.LatestDate.TryToValueType(Date.MinValue)
                        }
                    End Function
                )

                ' バイタル標準値
                If mainModel.AuthorAccount.StandardValues Is Nothing OrElse Not mainModel.AuthorAccount.StandardValues.Any() Then StandardValueWorker.SetStandardValue(mainModel, .StandardValueN)

                ' バイタル目標値
                If mainModel.AuthorAccount.TargetValues Is Nothing OrElse Not mainModel.AuthorAccount.TargetValues.Any() Then TargetValueWorker.SetTargetValue(mainModel, .TargetValueN)

                ' 身長
                Dim height As Decimal = .Height.TryToValueType(Decimal.MinValue)

                If height > Decimal.Zero Then
                    mainModel.AuthorAccount.Height = height
                    result.Height = height
                End If

                ' TODO:
                result.TanitaQrReference = NoteVitalWorker.CreateEncryptedTanitaQrReference(mainModel)

                ' キャッシュへ追加
                mainModel.SetInputModelCache(result)
            End With
        End If

        Return result

    End Function

    Public Shared Function CreateGraphPartialViewModel(mainModel As QolmsYappliModel, pageViewModel As NoteVitalViewModel, vitalType As QyVitalTypeEnum, ByRef refPartialViewName As String, Optional forceRead As Boolean = False) As QyVitalGraphPartialViewModelBase

        refPartialViewName = String.Empty

        Dim result As QyVitalGraphPartialViewModelBase = Nothing

        ' 目標値
        Dim targetValues() As Decimal = {Decimal.Zero, Decimal.Zero, Decimal.Zero, Decimal.Zero}

        ' TODO: 検証中
        NoteVitalWorker.GetTargetValues(mainModel, vitalType, targetValues(0), targetValues(1), targetValues(2), targetValues(3))

        ' 体重
        If vitalType = QyVitalTypeEnum.BodyWeight Then
            refPartialViewName = "_NoteVitalWeightGraphPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then ' TODO: 暫定対応 'hasData Then
                If Not pageViewModel.WeightPartialViewModel.ItemList.Any() Then
                    With NoteVitalWorker.ExecuteNoteVitalGraphRead(mainModel, vitalType, NoteVitalWorker.DEFAULT_DAY_COUNT)
                        result = New VitalWeightGraphPartialViewModel(
                            pageViewModel,
                            .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
                            pageViewModel.Height,
                            targetValues(0),
                            targetValues(1),
                            targetValues(2),
                            targetValues(3),
                            .HealthAgeValueN.ConvertAll(Function(i) i.ToVitalValueItem())
                        )
                    End With

                    pageViewModel.WeightPartialViewModel = DirectCast(result, VitalWeightGraphPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.WeightPartialViewModel
                End If
            Else
                Return New VitalWeightGraphPartialViewModel(
                    pageViewModel,
                    Nothing,
                    pageViewModel.Height,
                    targetValues(0),
                    targetValues(1),
                    targetValues(2),
                    targetValues(3),
                    Nothing
                )
            End If
        End If

        ' 血圧
        If vitalType = QyVitalTypeEnum.BloodPressure Then
            refPartialViewName = "_NoteVitalPressureGraphPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then ' TODO: 暫定対応 'hasData Then
                If Not pageViewModel.PressurePartialViewModel.ItemList.Any() Then
                    With NoteVitalWorker.ExecuteNoteVitalGraphRead(mainModel, vitalType, NoteVitalWorker.DEFAULT_DAY_COUNT)
                        result = New VitalPressureGraphPartialViewModel(
                            pageViewModel,
                            .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
                            targetValues(0),
                            targetValues(1),
                            targetValues(2),
                            targetValues(3)
                        )
                    End With

                    pageViewModel.PressurePartialViewModel = DirectCast(result, VitalPressureGraphPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.PressurePartialViewModel
                End If
            Else
                Return New VitalPressureGraphPartialViewModel(
                    pageViewModel,
                    Nothing,
                    targetValues(0),
                    targetValues(1),
                    targetValues(2),
                    targetValues(3)
                )
            End If
        End If

        ' 血糖値
        If vitalType = QyVitalTypeEnum.BloodSugar Then
            refPartialViewName = "_NoteVitalSugarGraphPartialView"

            Dim hasData As Boolean = False

            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then ' TODO: 暫定対応 'hasData Then
                If Not pageViewModel.SugarPartialViewModel.ItemList.Any() Then
                    With NoteVitalWorker.ExecuteNoteVitalGraphRead(mainModel, vitalType, NoteVitalWorker.DEFAULT_DAY_COUNT)
                        result = New VitalSugarGraphPartialViewModel(
                            pageViewModel,
                            .VitalValueN.ConvertAll(Function(i) i.ToVitalValueItem()),
                            targetValues(0),
                            targetValues(1),
                            targetValues(2),
                            targetValues(3)
                        )
                    End With

                    pageViewModel.SugarPartialViewModel = DirectCast(result, VitalSugarGraphPartialViewModel)

                    mainModel.SetInputModelCache(pageViewModel)

                    Return result
                Else
                    Return pageViewModel.SugarPartialViewModel
                End If
            Else
                Return New VitalSugarGraphPartialViewModel(
                    pageViewModel,
                    Nothing,
                    targetValues(0),
                    targetValues(1),
                    targetValues(2),
                    targetValues(3)
                )
            End If
        End If

        Throw New ArgumentOutOfRangeException("vitalType", "バイタル情報の種別が不正です。")

    End Function

    Public Shared Function CreateDetailPartialViewModel(mainModel As QolmsYappliModel, pageViewModel As NoteVitalViewModel, vitalType As QyVitalTypeEnum, recordDate As Date, ByRef refPartialViewName As String, Optional forceRead As Boolean = False) As QyVitalDetailPartialViewModelBase

        refPartialViewName = String.Empty

        Dim result As QyVitalDetailPartialViewModelBase = Nothing

        ' 体重
        If vitalType = QyVitalTypeEnum.BodyWeight Then
            refPartialViewName = "_NoteVitalWeightDetailPartialView"

            Dim hasData As Boolean = False

            'If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso hasData Then result = New VitalWeightDetailPartialViewModel(pageViewModel, recordDate)
            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then result = New VitalWeightDetailPartialViewModel(pageViewModel, recordDate) ' TODO: 暫定対応
        End If

        ' 血圧
        If vitalType = QyVitalTypeEnum.BloodPressure Then
            refPartialViewName = "_NoteVitalPressureDetailPartialView"

            Dim hasData As Boolean = False

            'If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso hasData Then result = New VitalPressureDetailPartialViewModel(pageViewModel, recordDate)
            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then result = New VitalPressureDetailPartialViewModel(pageViewModel, recordDate) ' TODO: 暫定対応
        End If

        ' 血糖値
        If vitalType = QyVitalTypeEnum.BloodSugar Then
            refPartialViewName = "_NoteVitalSugarDetailPartialView"

            Dim hasData As Boolean = False

            'If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso hasData Then result = New VitalSugarDetailPartialViewModel(pageViewModel, recordDate)
            If pageViewModel.IsAvailableVitalType(vitalType, hasData) AndAlso (hasData OrElse forceRead) Then result = New VitalSugarDetailPartialViewModel(pageViewModel, recordDate) ' TODO: 暫定対応
        End If

        Return result

    End Function

    Public Shared Function Delete(mainModel As QolmsYappliModel, pageViewModel As NoteVitalViewModel, vitalType As QyVitalTypeEnum, targetN As List(Of Date)) As NoteVitalDeleteJsonResult

        Dim result As New NoteVitalDeleteJsonResult() With {
            .VitalType = QyVitalTypeEnum.None.ToString(),
            .Height = String.Empty,
            .IsSuccess = Boolean.FalseString
        }

        ' 体重
        If vitalType = QyVitalTypeEnum.BodyWeight AndAlso targetN IsNot Nothing AndAlso targetN.Any() Then
            Dim inputModelN As New Dictionary(Of Tuple(Of QyVitalTypeEnum, Date), VitalValueInputModel)()

            targetN.ForEach(
                Sub(i)
                    Dim key As New Tuple(Of QyVitalTypeEnum, Date)(vitalType, i)

                    If i <> Date.MinValue AndAlso Not inputModelN.ContainsKey(key) Then
                        Dim meridiem As String = i.ToString("tt", CultureInfo.InvariantCulture).ToLower()
                        Dim hour As String = i.ToString("%h")
                        Dim minute As String = i.ToString("%m")

                        If hour.CompareTo("12") = 0 Then hour = "0"

                        inputModelN.Add(
                            key,
                            New VitalValueInputModel() With {
                                .VitalType = QyVitalTypeEnum.BodyWeight,
                                .Meridiem = meridiem,
                                .Hour = hour,
                                .Minute = minute,
                                .Value1 = "",
                                .Value2 = "",
                                .ConditionType = QyVitalConditionTypeEnum.None.ToString()
                            }
                        )
                    End If
                End Sub
            )

            If inputModelN.Any() Then
                With NoteVitalWorker.ExecuteNoteVitalWriteApi(mainModel, inputModelN)
                    pageViewModel.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

                    ' アカウント情報の会員の種別を更新
                    mainModel.AuthorAccount.MembershipType = pageViewModel.MembershipType

                    For Each item As QhApiAvailableVitalItem In .AvailableVitalN
                        If item.VitalType.TryToValueType(QyVitalTypeEnum.None) = vitalType Then
                            Dim available As AvailableVitalItem = pageViewModel.AvailableVitalN.Find(Function(i) i.VitalType = vitalType)

                            If available IsNot Nothing Then available.LatestDate = item.LatestDate.TryToValueType(Date.MinValue)

                            Exit For
                        End If
                    Next

                    ' 身長
                    Dim height As Decimal = .Height.TryToValueType(Decimal.MinValue)

                    If height > Decimal.Zero Then
                        mainModel.AuthorAccount.Height = height
                        pageViewModel.Height = height

                        result.Height = height.ToString("0.####")
                    End If

                    ' 同期で健康年齢（ベイジアン ネットワーク）を算出し登録（非同期だとグラフの再読み込みが先に実行される可能性がある）
                    NoteVitalWorker.EditBayesian(
                        mainModel.AuthorAccount.AccountKey,
                        mainModel.AuthorAccount.SexType,
                        mainModel.AuthorAccount.Birthday,
                        mainModel.AuthorAccount.MembershipType,
                        .HealthAgeValueN,
                        .WeightOfOneDay.TryToValueType(Decimal.MinValue),
                        .HeightOfOneDay.TryToValueType(Decimal.MinValue),
                        mainModel.ApiExecutor,
                        mainModel.ApiExecutorName,
                        mainModel.SessionId,
                        mainModel.ApiAuthorizeKey
                    )

                    ' 体重グラフ パーシャル ビュー モデルの初期化
                    pageViewModel.WeightPartialViewModel = New VitalWeightGraphPartialViewModel(
                        pageViewModel,
                        Nothing,
                        pageViewModel.Height,
                        Decimal.Zero,
                        Decimal.Zero,
                        Decimal.Zero,
                        Decimal.Zero,
                        Nothing
                    )

                    ' キャッシュへ追加
                    mainModel.SetInputModelCache(pageViewModel)

                    result.VitalType = QyVitalTypeEnum.BodyWeight.ToString()
                    result.IsSuccess = Boolean.TrueString
                End With
            End If
        End If

        ' 血圧
        If vitalType = QyVitalTypeEnum.BloodPressure AndAlso targetN IsNot Nothing AndAlso targetN.Any() Then
            Dim inputModelN As New Dictionary(Of Tuple(Of QyVitalTypeEnum, Date), VitalValueInputModel)()

            targetN.ForEach(
                Sub(i)
                    Dim key As New Tuple(Of QyVitalTypeEnum, Date)(vitalType, i)

                    If i <> Date.MinValue AndAlso Not inputModelN.ContainsKey(key) Then
                        Dim meridiem As String = i.ToString("tt", CultureInfo.InvariantCulture).ToLower()
                        Dim hour As String = i.ToString("%h")
                        Dim minute As String = i.ToString("%m")

                        If hour.CompareTo("12") = 0 Then hour = "0"

                        inputModelN.Add(
                            key,
                            New VitalValueInputModel() With {
                                .VitalType = QyVitalTypeEnum.BloodPressure,
                                .Meridiem = meridiem,
                                .Hour = hour,
                                .Minute = minute,
                                .Value1 = "",
                                .Value2 = "",
                                .ConditionType = QyVitalConditionTypeEnum.None.ToString()
                            }
                        )
                    End If
                End Sub
            )

            If inputModelN.Any() Then
                With NoteVitalWorker.ExecuteNoteVitalWriteApi(mainModel, inputModelN)
                    pageViewModel.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

                    ' アカウント情報の会員の種別を更新
                    mainModel.AuthorAccount.MembershipType = pageViewModel.MembershipType

                    For Each item As QhApiAvailableVitalItem In .AvailableVitalN
                        If item.VitalType.TryToValueType(QyVitalTypeEnum.None) = vitalType Then
                            Dim available As AvailableVitalItem = pageViewModel.AvailableVitalN.Find(Function(i) i.VitalType = vitalType)

                            If available IsNot Nothing Then available.LatestDate = item.LatestDate.TryToValueType(Date.MinValue)

                            Exit For
                        End If
                    Next

                    ' 血圧グラフ パーシャル ビュー モデルの初期化
                    pageViewModel.PressurePartialViewModel = New VitalPressureGraphPartialViewModel(
                        pageViewModel,
                        Nothing,
                        Decimal.Zero,
                        Decimal.Zero,
                        Decimal.Zero,
                        Decimal.Zero
                    )

                    ' キャッシュへ追加
                    mainModel.SetInputModelCache(pageViewModel)

                    result.VitalType = QyVitalTypeEnum.BloodPressure.ToString()
                    result.IsSuccess = Boolean.TrueString
                End With
            End If
        End If

        ' 血糖値
        If vitalType = QyVitalTypeEnum.BloodSugar AndAlso targetN IsNot Nothing AndAlso targetN.Any() Then
            Dim inputModelN As New Dictionary(Of Tuple(Of QyVitalTypeEnum, Date), VitalValueInputModel)()

            targetN.ForEach(
                Sub(i)
                    Dim key As New Tuple(Of QyVitalTypeEnum, Date)(vitalType, i)

                    If i <> Date.MinValue AndAlso Not inputModelN.ContainsKey(key) Then
                        Dim meridiem As String = i.ToString("tt", CultureInfo.InvariantCulture).ToLower()
                        Dim hour As String = i.ToString("%h")
                        Dim minute As String = i.ToString("%m")

                        If hour.CompareTo("12") = 0 Then hour = "0"

                        inputModelN.Add(
                            key,
                            New VitalValueInputModel() With {
                                .VitalType = QyVitalTypeEnum.BloodSugar,
                                .Meridiem = meridiem,
                                .Hour = hour,
                                .Minute = minute,
                                .Value1 = "",
                                .Value2 = "",
                                .ConditionType = QyVitalConditionTypeEnum.None.ToString()
                            }
                        )
                    End If
                End Sub
            )

            If inputModelN.Any() Then
                With NoteVitalWorker.ExecuteNoteVitalWriteApi(mainModel, inputModelN)
                    pageViewModel.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

                    ' アカウント情報の会員の種別を更新
                    mainModel.AuthorAccount.MembershipType = pageViewModel.MembershipType

                    For Each item As QhApiAvailableVitalItem In .AvailableVitalN
                        If item.VitalType.TryToValueType(QyVitalTypeEnum.None) = vitalType Then
                            Dim available As AvailableVitalItem = pageViewModel.AvailableVitalN.Find(Function(i) i.VitalType = vitalType)

                            If available IsNot Nothing Then available.LatestDate = item.LatestDate.TryToValueType(Date.MinValue)

                            Exit For
                        End If
                    Next

                    ' 血糖値グラフ パーシャル ビュー モデルの初期化
                    pageViewModel.SugarPartialViewModel = New VitalSugarGraphPartialViewModel(
                        pageViewModel,
                        Nothing,
                        Decimal.Zero,
                        Decimal.Zero,
                        Decimal.Zero,
                        Decimal.Zero
                    )

                    ' キャッシュへ追加
                    mainModel.SetInputModelCache(pageViewModel)

                    result.VitalType = QyVitalTypeEnum.BloodSugar.ToString()
                    result.IsSuccess = Boolean.TrueString
                End With
            End If
        End If

        Return result

    End Function

    Public Shared Function Edit(mainModel As QolmsYappliModel, pageViewModel As NoteVitalViewModel, inputModel As NoteVitalEditInputModel, ByRef refVitalTypeN As List(Of QyVitalTypeEnum), ByRef refMessageN As List(Of String), ByRef refHeight As Decimal) As Boolean

        ' TODO: 身長

        refVitalTypeN = New List(Of QyVitalTypeEnum)()
        refMessageN = New List(Of String)() From {"登録に失敗しました。"}
        refHeight = Decimal.MinValue

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプットモデルが Null 参照です。")

        ' モデルへ入力値をを反映
        pageViewModel.EditInputModel.UpdateByInput(inputModel)

        Dim result As Boolean = False

        If inputModel.HasValues Then
            Dim vitalDic As New Dictionary(Of Tuple(Of QyVitalTypeEnum, Date), VitalValueInputModel)()

            ' 体重
            If inputModel.IsAvailableVitalType(QyVitalTypeEnum.BodyWeight) Then
                If inputModel.Weight IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(inputModel.Weight.Value1) AndAlso Not String.IsNullOrWhiteSpace(inputModel.Weight.Value2) Then
                    vitalDic.Add(New Tuple(Of QyVitalTypeEnum, Date)(QyVitalTypeEnum.BodyWeight, inputModel.WeightDate), inputModel.Weight)
                End If
            End If

            ' 血圧
            If inputModel.IsAvailableVitalType(QyVitalTypeEnum.BloodPressure) Then
                If inputModel.Pressure IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(inputModel.Pressure.Value1) AndAlso Not String.IsNullOrWhiteSpace(inputModel.Pressure.Value2) Then
                    vitalDic.Add(New Tuple(Of QyVitalTypeEnum, Date)(QyVitalTypeEnum.BloodPressure, inputModel.PressureDate), inputModel.Pressure)
                End If
            End If

            ' 血糖値
            If inputModel.IsAvailableVitalType(QyVitalTypeEnum.BloodSugar) Then
                If inputModel.Sugar IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(inputModel.Sugar.Value1) AndAlso Not String.IsNullOrWhiteSpace(inputModel.Sugar.ConditionType) Then
                    vitalDic.Add(New Tuple(Of QyVitalTypeEnum, Date)(QyVitalTypeEnum.BloodSugar, inputModel.SugarDate), inputModel.Sugar)
                End If
            End If

            If vitalDic.Any() Then
                Dim pointDic As New Dictionary(Of Tuple(Of QyVitalTypeEnum, Date), Tuple(Of QyPointItemTypeEnum, Date))()
                Dim pointActionDate As Date = Date.Now
                Dim pointMaxDay As Date = pointActionDate.Date ' ポイント付与範囲終了日（今日）
                Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）

                ' API を実行
                With NoteVitalWorker.ExecuteNoteVitalWriteApi(mainModel, vitalDic)
                    pageViewModel.RecordDate = Date.Now.Date
                    pageViewModel.MembershipType = .MembershipType.TryToValueType(QyMemberShipTypeEnum.Free)

                    ' アカウント情報の会員の種別を更新
                    mainModel.AuthorAccount.MembershipType = pageViewModel.MembershipType

                    Dim hash As New HashSet(Of QyVitalTypeEnum)()

                    For Each item As QhApiAvailableVitalItem In .AvailableVitalN
                        Dim vitalType As QyVitalTypeEnum = item.VitalType.TryToValueType(QyVitalTypeEnum.None)

                        If vitalType = QyVitalTypeEnum.BloodPressure OrElse vitalType = QyVitalTypeEnum.BloodSugar OrElse vitalType = QyVitalTypeEnum.BodyWeight Then
                            Dim available As AvailableVitalItem = pageViewModel.AvailableVitalN.Find(Function(i) i.VitalType = vitalType)

                            If available IsNot Nothing Then
                                available.LatestDate = item.LatestDate.TryToValueType(Date.MinValue)

                                hash.Add(vitalType)
                            End If
                        End If
                    Next

                    ' バイタル入力インプット モデルの初期化
                    pageViewModel.EditInputModel = New NoteVitalEditInputModel(
                        pageViewModel.RecordDate,
                        pageViewModel.AvailableVitalN.Select(Function(i) i.VitalType).ToList(),
                        Decimal.MinusOne
                    )

                    ' 体重
                    If hash.Contains(QyVitalTypeEnum.BodyWeight) Then
                        ' 身長
                        Dim height As Decimal = .Height.TryToValueType(Decimal.MinValue)

                        If height > Decimal.Zero Then
                            mainModel.AuthorAccount.Height = height
                            pageViewModel.Height = height

                            refHeight = height
                        End If

                        ' 同期で健康年齢（ベイジアン ネットワーク）を算出し登録（非同期だとグラフの再読み込みが先に実行される可能性がある）
                        NoteVitalWorker.EditBayesian(
                            mainModel.AuthorAccount.AccountKey,
                            mainModel.AuthorAccount.SexType,
                            mainModel.AuthorAccount.Birthday,
                            mainModel.AuthorAccount.MembershipType,
                            .HealthAgeValueN,
                            .WeightOfOneDay.TryToValueType(Decimal.MinValue),
                            .HeightOfOneDay.TryToValueType(Decimal.MinValue),
                            mainModel.ApiExecutor,
                            mainModel.ApiExecutorName,
                            mainModel.SessionId,
                            mainModel.ApiAuthorizeKey
                        )


                        '同期でカロミルWebViewに体重登録
                        '登録された体重をそのまま送信
                        Dim weight As Decimal = inputModel.Weight.Value1.TryToValueType(Decimal.MinValue)

                        If NoteCalomealWorker.SetAnthropometric(mainModel,inputModel.WeightDate,weight)
                             
                            'カロミル目標カロリーを取得
                            Dim goalList As List(Of CalomealGoalSet) = NoteCalomealWorker.GetMealWithBasis(mainModel,Date.Now,Date.Now)
                            If goalList.Any() Then
                                '取得出来たら更新
                                Dim goal As CalomealGoalSet = goalList.OrderByDescending(Function(i) i.TargetDate).FirstOrDefault()
                                Dim cal As Integer = iif(goal.BasisAllCalorie > 0, goal.BasisAllCalorie, 1).ToString().TryToValueType(1)'変換できない場合最小値の1へ置き換え（念のため）

                                Task.Run(
                                        Sub()
                                            '登録の可否はとりあえず見なくていいし、登録は非同期
                                            PortalTargetSettingWorker2.UpdateCalTarget(mainModel,cal)
                                        End Sub
                                    )
                            End If
                        End If
                       
                        ' 体重グラフ パーシャル ビュー モデルの初期化
                        pageViewModel.WeightPartialViewModel = New VitalWeightGraphPartialViewModel(
                            pageViewModel,
                            Nothing,
                            pageViewModel.Height,
                            Decimal.Zero,
                            Decimal.Zero,
                            Decimal.Zero,
                            Decimal.Zero,
                            Nothing
                        )
                    End If

                    ' 血圧
                    If hash.Contains(QyVitalTypeEnum.BloodPressure) Then
                        ' 血圧グラフ パーシャル ビュー モデルの初期化
                        pageViewModel.PressurePartialViewModel = New VitalPressureGraphPartialViewModel(
                            pageViewModel,
                            Nothing,
                            Decimal.Zero,
                            Decimal.Zero,
                            Decimal.Zero,
                            Decimal.Zero
                        )
                    End If

                    ' 血糖値
                    If hash.Contains(QyVitalTypeEnum.BloodSugar) Then
                        ' 血糖値グラフ パーシャル ビュー モデルの初期化
                        pageViewModel.SugarPartialViewModel = New VitalSugarGraphPartialViewModel(
                            pageViewModel,
                            Nothing,
                            Decimal.Zero,
                            Decimal.Zero,
                            Decimal.Zero,
                            Decimal.Zero
                        )
                    End If

                    ' キャッシュへ追加
                    mainModel.SetInputModelCache(pageViewModel)

                    refVitalTypeN.AddRange(hash.ToList())
                    refMessageN = New List(Of String)()
                    result = True

                    ' 「ホーム」画面のキャッシュをクリア
                    mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                    ' ポイント 付与
                    If .CanGivePoint.TryToValueType(False) Then
                        vitalDic.ToList().ForEach(
                            Sub(i)
                                Dim key As New Tuple(Of QyVitalTypeEnum, Date)(
                                    i.Key.Item1,
                                    i.Value.ToApiVitalValueItem(i.Key.Item2).RecordDate.TryToValueType(i.Key.Item2)
                                )
                                Dim limit As Date = Date.MinValue

                                If (key.Item2.Date >= pointMinDay And key.Item2.Date <= pointMaxDay) AndAlso Not pointDic.ContainsKey(key) Then
                                    limit = New Date(key.Item2.Year, key.Item2.Month, 1).AddMonths(7).AddDays(-1) ' ポイント 有効期限は 6 ヶ月後の月末（起点は測定日時）

                                    pointDic.Add(key, New Tuple(Of QyPointItemTypeEnum, Date)(QyPointItemTypeEnum.Vital, limit))
                                End If
                            End Sub
                        )

                        If pointDic.Any() Then
                            ' 非同期で ポイント 付与
                            Task.Run(
                                Sub()
                                    QolmsPointWorker.AddQolmsPoints(
                                        mainModel.ApiExecutor,
                                        mainModel.ApiExecutorName,
                                        mainModel.SessionId,
                                        mainModel.ApiAuthorizeKey,
                                        mainModel.AuthorAccount.AccountKey,
                                        pointDic.ToList().ConvertAll(Of QolmsPointGrantItem)(
                                            Function(i)
                                                Return New QolmsPointGrantItem(
                                                    mainModel.AuthorAccount.MembershipType,
                                                    pointActionDate,
                                                    Guid.NewGuid().ToApiGuidString(),
                                                    QyPointItemTypeEnum.Vital,
                                                    i.Value.Item2,
                                                    i.Key.Item2
                                                )
                                            End Function
                                        )
                                    )
                                End Sub
                            )
                        End If
                    End If
                End With
            Else
                refMessageN = New List(Of String)() From {"値を入力してください。"}
            End If
        Else
            refMessageN = New List(Of String)() From {"値を入力してください。"}
        End If

        Return result

    End Function

    ' TODO: 検証中
    Public Shared Function GetViewModelCache(mainModel As QolmsYappliModel, Optional checkCache As Boolean = True) As NoteVitalViewModel

        ' キャッシュから取得
        Dim result As NoteVitalViewModel = mainModel.GetInputModelCache(Of NoteVitalViewModel)()
        Dim partialViewName As String = String.Empty

        If checkCache And result Is Nothing Then
            ' ページ ビュー モデルを生成しキャッシュへ追加
            result = NoteVitalWorker.CreateViewModel(mainModel)

            ' 体重グラフ パーシャル ビュー モデルを生成しキャッシュへ追加
            NoteVitalWorker.CreateGraphPartialViewModel(mainModel, result, QyVitalTypeEnum.BodyWeight, partialViewName)

            ' 血圧グラフ パーシャル ビュー モデルを生成しキャッシュへ追加
            NoteVitalWorker.CreateGraphPartialViewModel(mainModel, result, QyVitalTypeEnum.BloodPressure, partialViewName)

            ' 血糖値グラフ パーシャル ビュー モデルを生成しキャッシュへ追加
            NoteVitalWorker.CreateGraphPartialViewModel(mainModel, result, QyVitalTypeEnum.BloodSugar, partialViewName)

            ' キャッシュから取得
            result = mainModel.GetInputModelCache(Of NoteVitalViewModel)()
        End If

        Return result

    End Function

    ''' <summary>
    ''' タニタ 会員 QR コード 情報を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="memberNo">会員識別番号。</param>
    ''' <returns>
    ''' タニタ 会員 QR コード 情報を取得した結果を保持する、
    ''' JSON 形式の コンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Shared Function GetTanitaQrCode(mainModel As QolmsYappliModel, memberNo As String) As NoteVitalTanitaQrJsonResult

        Dim result As New NoteVitalTanitaQrJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .QrCode = String.Empty,
            .Experies = String.Empty
        }
        Dim refMemberNo As String

        ' タニタ の体重、血圧連携の チェック
        If NoteVitalWorker.IsTanitaConnectionEnabled(mainModel, refMemberNo) AndAlso String.Compare(refMemberNo, memberNo) = 0 Then
            ' 連携有効

            ' タニタ の API から QR 情報を取得
            Dim apiResults As GetQrCodeApiResults = NoteVitalWorker.ExecuteGetQrCodeApi(refMemberNo)
            Dim experies As Date = Date.MinValue

            With apiResults
                If .IsSuccess _
                    AndAlso Not String.IsNullOrWhiteSpace(.qr_code) _
                    AndAlso Not String.IsNullOrWhiteSpace(.limit_time) _
                    AndAlso Date.TryParseExact(.limit_time, "yyyyMMddHHmmss", Nothing, DateTimeStyles.None, experies) _
                    AndAlso experies <> Date.MinValue Then

                    result.IsSuccess = .IsSuccess.ToString()
                    result.QrCode = .qr_code
                    result.Experies = experies.ToString("HH 時 mm 分 ss 秒まで有効")
                End If
            End With

            '' TODO: 以下 ダミー 情報
            'Dim now As Date = Date.Now

            'result.IsSuccess = Boolean.TrueString
            'result.QrCode = now.ToString("yyyy/MM/dd HH:mm:ss.fffffff")
            'result.Experies = now.AddMinutes(1).ToString("HH 時 mm 分 ss 秒まで有効")
        End If

        Return result

    End Function

#End Region

End Class
