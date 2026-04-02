Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class PortalMedicineConnectionSearchWorker

#Region "Constant"

    '連携システム番号
    Private Const PHARMO_SYSTEM_NO As Integer = 47009

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

    Private Shared Function ExecutePortalMedicineConnectionSearchReadApi(mainModel As QolmsYappliModel, pageIndex As Integer) As QhYappliPortalMedicineConnectionSearchReadApiResults

        Dim apiArgs As New QhYappliPortalMedicineConnectionSearchReadApiArgs(
            QhApiTypeEnum.YappliPortalMedicineConnectionSearchRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
            ) With {
                .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                .PageIndex = pageIndex.ToString()
            }

        Dim apiResults As QhYappliPortalMedicineConnectionSearchReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalMedicineConnectionSearchReadApiResults)(
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
    ''' 薬局連携確認画面の情報を取得する
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, pageIndex As Integer, fromPageNoType As QyPageNoTypeEnum) As PortalMedicineConnectionSearchViewModel

        Dim result As New PortalMedicineConnectionSearchViewModel()

        ' 連携先薬局のリストを取得五十音順、ページ番号
        With PortalMedicineConnectionSearchWorker.ExecutePortalMedicineConnectionSearchReadApi(mainModel, pageIndex)

            result.FromPageNoType = fromPageNoType
            result.PageCount = Integer.Parse(.PageCount)
            result.PageIndex = Integer.Parse(.PageIndex)

            For Each item As QhApiMedicineConnectionFacilityItem In .MedicineConnectionFacilityItemN

                Dim facility As New MedicineConnectionFacilityItem() With {
                    .FacilityKey = item.FacilityKey.TryToValueType(Guid.Empty),
                    .Name = item.Name,
                    .KanaName = item.KanaName,
                    .PostalCode = item.PostalCode,
                    .Address = item.Address,
                    .Tel = item.Tel,
                    .PharmacyId = item.PharmacyId.TryToValueType(Integer.MinValue)
                }

                result.MedicineConnectionFacilityItemN.Add(facility.FacilityKey, facility)
            Next

        End With

        mainModel.SetInputModelCache(result)
        Return result

    End Function

#End Region

End Class
