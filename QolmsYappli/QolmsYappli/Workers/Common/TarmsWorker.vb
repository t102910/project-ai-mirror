Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 規約に関する共通機能を提供します。
''' このクラスは継承できません。
''' </summary>
Friend NotInheritable Class TarmsWorker
#Region "Constant"

    ''連携システム番号
    'Public Shared ReadOnly LinkageList As Integer() = {47005, 47006, 47010, 47011}
    'Public Shared ReadOnly MedicineLinkageList As Integer() = {47009}
    'Public Shared ReadOnly CompanyLinkageList As Integer() = {47100, 11111}

    'Public Shared ReadOnly Property AppLinkageNoList As New Lazy(Of List(Of Integer))(Function() GetSetting())
    'Public Shared ReadOnly Property HospitalLinkageNoList As New Lazy(Of List(Of Integer))(Function() GetSetting())
    'Public Shared ReadOnly Property CompnyLinkageNoList As New Lazy(Of List(Of Integer))(Function() GetSetting())
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
    ''' 設定を取得します。
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting() As List(Of Integer)

        Dim result As New List(Of Integer)()
        Dim value As String = ConfigurationManager.AppSettings("ExaminationLinkageList")

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            Dim arr As String() = value.Split(","c)
            'If arr.Any() Then

            For Each item As String In arr
                Dim outint As Integer = Integer.MinValue

                If Integer.TryParse(item, outint) Then
                    result.Add(outint)
                End If

            Next
            'End If

        End If

        Return result

    End Function

    ''' <summary>
    ''' 連携システム番号 を指定して、JOTO API から連携情報 を取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル</param>
    ''' <param name="termsNo">連携システム番号</param>
    ''' <returns></returns>
    Private Shared Function ExecuteTermsReadApi(mainModel As QolmsYappliModel, termsNo As Long) As QjCommonTermsReadApiResults

        Dim apiArgs As New QjCommonTermsReadApiArgs(
            QjApiTypeEnum.CommonTermsRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .TermsNo = termsNo.ToString(),
            .SystemType = "14"
        }
        Dim apiResults As QjCommonTermsReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjCommonTermsReadApiResults)(
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


#Region "Private Method"

    ''' <summary>
    '''  を指定して、 を取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル</param>
    ''' <param name="termsNo">連携システム番号</param>
    ''' <returns></returns>
    Public Shared Function GetTermsContent(mainModel As QolmsYappliModel, termsNo As Integer) As String

        '[TERMSNO] 主キー
        'システムタイプ
        With TarmsWorker.ExecuteTermsReadApi(mainModel, termsNo)
            Return .Contents.Trim()
        End With
        'Dim apiResult As QjLinkageReadApiResults = TarmsWorker.ExecuteLinkageReadApi(mainModel, linkageSystemNo)

        'Return New LinkageItem() With {
        '        .LinkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(Integer.MinValue),
        '        .LinkageSystemId = apiResult.LinkageSystemId,
        '        .Dataset = apiResult.Dataset,
        '        .StatusType = apiResult.StatusType.TryToValueType(Byte.MinValue),
        '        .Facilitykey = apiResult.Facilitykey.TryToValueType(Guid.Empty),
        '        .LinkageSystemName = apiResult.LinkageSystemName
        '}

    End Function

#End Region

End Class
