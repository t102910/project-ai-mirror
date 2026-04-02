Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PortalUserInfomationEditWorker

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

    Private Shared Function ExecutePortalUserInfomationEditReadApi(mainModel As QolmsYappliModel) As QjPortalUserInfomationReadApiResults

        Dim apiArgs As New QjPortalUserInfomationReadApiArgs(
            QjApiTypeEnum.PortalUserInfomationRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {.Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()}

        Dim apiResults As QjPortalUserInfomationReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalUserInfomationReadApiResults)(
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

    Private Shared Function ExecuteUserInfomationEditWriteApi(mainModel As QolmsYappliModel, model As PortalUserInfomationEditInputModel) As QjPortalUserInfomationWriteApiResults

        Dim birthday As Date = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))

        Dim apiArgs As New QjPortalUserInfomationWriteApiArgs(
            QjApiTypeEnum.PortalUserInfomationWrite,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .FamilyName = model.FamilyName,
            .GivenName = model.GivenName,
            .FamilyKanaName = model.FamilyKanaName,
            .GivenKanaName = model.GivenKanaName,
            .SexType = model.SexType.ToString(),
            .BirthDay = birthday.ToApiDateString(),
            .MailAddress = model.MailAddress,
            .Prefectures = model.Prefectures.ToString(),
            .CityNo = If(model.CityNo <> Nothing, model.CityNo, String.Empty),
            .PhoneNo = If(model.PhoneNo <> Nothing, model.PhoneNo, String.Empty)
        }
        Dim apiResults As QjPortalUserInfomationWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalUserInfomationWriteApiResults)(
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

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, fromPageNoType As QyPageNoTypeEnum) As PortalUserInfomationEditInputModel
       
        Dim apiResult As QjPortalUserInfomationReadApiResults = PortalUserInfomationEditWorker.ExecutePortalUserInfomationEditReadApi(mainModel)
        Dim birthday As Date= apiResult.Birthday.TryToValueType(Date.MinValue)

        Return New PortalUserInfomationEditInputModel() With {
            .FromPageNoType = fromPageNoType,
            .JotoId = apiResult.JotoId,
            .OpenId = apiResult.OpenId,
            .FamilyName = apiResult.FamilyName,
            .GivenName = apiResult.GivenName,
            .FamilyKanaName = apiResult.FamilyKanaName,
            .GivenKanaName = apiResult.GivenKanaName,
            .SexType = apiResult.SexType.TryToValueType(QySexTypeEnum.None),
            .BirthYear = birthday.Year.ToString(),
            .BirthMonth = birthday.Month.ToString(),
            .BirthDay = birthday.Day.ToString(),
            .MailAddress = apiResult.MailAddress,
            .Prefectures = apiResult.PrefecturesNo.TryToValueType(Byte.MinValue),
            .CityNo = apiResult.CityNo,
            .PhoneNo = apiResult.PhoneNo,
            .CityItemN = apiResult.CityItemN.ConvertAll(Function(i) i.ToCityItem())
        }

    End Function

    Shared Function Resister(mainModel As QolmsYappliModel, model As PortalUserInfomationEditInputModel) As Boolean

        '登録処理
        With PortalUserInfomationEditWorker.ExecuteUserInfomationEditWriteApi(mainModel, model)

            If .IsSuccess.TryToValueType(False) Then

                '成功したらアカウント情報(名前)の更新
                mainModel.AuthorAccount.FamilyName = model.FamilyName
                mainModel.AuthorAccount.GivenName = model.GivenName
                mainModel.AuthorAccount.FamilyKanaName = model.FamilyKanaName
                mainModel.AuthorAccount.GivenKanaName = model.GivenKanaName

            End If

            Return .IsSuccess.TryToValueType(False)

        End With

    End Function

#End Region


End Class
