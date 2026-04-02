Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 「市民確認 同意」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalLocalIdVerificationRegisterWorker

#Region "Constant"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 「チャレンジエントリー」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalLocalIdVerificationRegisterReadApi(mainModel As QolmsYappliModel) As QjPortalLocalIdVerificationRegisterReadApiResults

        Dim apiArgs As New QjPortalLocalIdVerificationRegisterReadApiArgs(
         QjApiTypeEnum.PortalLocalIdVerificationRegisterRead,
         QsApiSystemTypeEnum.QolmsJoto,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .LinkageSystemNo = "47900021"
         }

        Dim apiResults As QjPortalLocalIdVerificationRegisterReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalLocalIdVerificationRegisterReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' 「」画面登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>x
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalLocalIdVerificationRegisterWriteApi(mainModel As QolmsYappliModel, inputModel As PortalLocalIdVerificationRegisterInputModel, deleteFlag As Boolean) As QjPortalLocalIdVerificationRegisterWriteApiResults

        Dim apiArgs As New QjPortalLocalIdVerificationRegisterWriteApiArgs(
         QjApiTypeEnum.PortalLocalIdVerificationRegisterWrite,
         QsApiSystemTypeEnum.QolmsJoto,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
             .LinkageSystemNo = "47900021",
             .MailAddress = inputModel.MailAddress,
             .PhoneNumber = inputModel.PhoneNumber,
             .IdentityUpdateFlag = Boolean.TrueString,
             .RelationContentType = Convert.ToByte(inputModel.RelationContentFlags).ToString(),
             .DeleteFlag = deleteFlag.ToString(),
             .GinowanProjectJoin = "47900022"
         }

        Dim apiResults As QjPortalLocalIdVerificationRegisterWriteApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjPortalLocalIdVerificationRegisterWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"
    ''' <summary>
    ''' 市民確認登録画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalLocalIdVerificationRegisterInputModel

        Dim result As New PortalLocalIdVerificationRegisterInputModel()

        'メールアドレスと電話番号はここから修正させる？（できればデフォルトのユーザー情報編集画面からしてほしい）

        With ExecutePortalLocalIdVerificationRegisterReadApi(mainModel)

            '編集モードで開く(LinkageSystemNo>0)
            '編集だったら中止できるように表示する
            result.LinkageSystemNo = .LinkageSystemNo.TryToValueType(Integer.MinValue)
            result.LinkageSystemId = .LinkageSystemId
            result.Status = .Status.TryToValueType(Byte.MinValue)
            'メールアドレスとでんわ番号とってくる
            result.MailAddress = .MailAddress
            result.PhoneNumber = .PhoneNumber

            result.RelationContentFlags = .RelationContentFlags.TryToValueType(QyRelationContentTypeEnum.None)

        End With

        '画面の元の値をキャッシュ
        mainModel.SetInputModelCache(result)

        Return result

    End Function

    ''' <summary>
    ''' 市民確認エントリーを登録した結果を返却します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    Public Shared Function Register(mainModel As QolmsYappliModel, input As PortalLocalIdVerificationRegisterInputModel, ByRef errorMessage As String) As Boolean

        Dim DeleteFlag As Boolean = False
        With PortalLocalIdVerificationRegisterWorker.ExecutePortalLocalIdVerificationRegisterWriteApi(mainModel, input, DeleteFlag)

            errorMessage = .ErrorMessage
            '登録結果
            Return .IsSuccess.TryToValueType(False) AndAlso String.IsNullOrWhiteSpace(errorMessage)

        End With

    End Function

    Friend Shared Function Cancel(mainModel As QolmsYappliModel, ByRef errorMessage As String) As Boolean

        '画面の元の値キャッシュをしゅとく
        Dim inp As PortalLocalIdVerificationRegisterInputModel = mainModel.GetInputModelCache(Of PortalLocalIdVerificationRegisterInputModel)()

        Dim DeleteFlag As Boolean = True
        With PortalLocalIdVerificationRegisterWorker.ExecutePortalLocalIdVerificationRegisterWriteApi(mainModel, inp, DeleteFlag)

            errorMessage = .ErrorMessage
            '登録結果
            Return .IsSuccess.TryToValueType(False) AndAlso String.IsNullOrWhiteSpace(errorMessage)

        End With

    End Function

#End Region

End Class
