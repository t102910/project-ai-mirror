Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' Web API を実行するための認証 キー に関する機能を提供します。
''' この クラス は継承できません。
''' </summary>
Friend NotInheritable Class ApiAuthorizeWorker

#Region "Constant"

    ''' <summary>
    ''' ダミー の セッション ID を表します。
    ''' </summary>
    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミー の API 認証 キー を表します。
    ''' </summary>
    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタ は使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    <Obsolete("未使用です。")>
    Private Shared Function ExecuteQolmsDeleteAuthorizeKeysApi(executor As Guid) As Boolean

        Dim apiArgs As New QiQolmsDeleteAuthorizeKeysApiArgs(
            QiApiTypeEnum.QolmsDeleteAuthorizeKeys,
            QsApiSystemTypeEnum.Qolms,
            executor,
            String.Empty
        )
        Dim apiResults As QiQolmsDeleteAuthorizeKeysApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsDeleteAuthorizeKeysApiResults)(
            apiArgs,
            ApiAuthorizeWorker.DUMMY_SESSION_ID,
            ApiAuthorizeWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return True
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' QolmsApi 用 API 認証 キー 取得 API を実行します。
    ''' </summary>
    ''' <param name="executor">実行者 アカウント キー。</param>
    ''' <param name="sessionID">セッション ID。</param>
    ''' <returns>
    ''' API 戻り値 クラス。
    ''' </returns>
    Private Shared Function ExecuteQolmsNewAuthorizeKeyApi(executor As Guid, sessionID As String) As QiQolmsNewAuthorizeKeyApiResults

        Dim apiArgs As New QiQolmsNewAuthorizeKeyApiArgs(
            QiApiTypeEnum.QolmsNewAuthorizeKey,
            QsApiSystemTypeEnum.Qolms,
            executor,
            String.Empty
        ) With {
            .SessionId = sessionID
        }
        Dim apiResults As QiQolmsNewAuthorizeKeyApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsNewAuthorizeKeyApiResults)(
            apiArgs,
            ApiAuthorizeWorker.DUMMY_SESSION_ID,
            ApiAuthorizeWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証 キー 取得 API を実行します。
    ''' </summary>
    ''' <param name="executor">実行者 アカウント キー。</param>
    ''' <param name="sessionID">セッション ID。</param>
    ''' <returns>
    ''' API 戻り値 クラス。
    ''' </returns>
    Private Shared Function ExecuteQolmsJotoNewAuthorizeKeyApi(executor As Guid, sessionID As String) As QiQolmsJotoNewAuthorizeKeyApiResults

        Dim apiArgs As New QiQolmsJotoNewAuthorizeKeyApiArgs(
            QiApiTypeEnum.QolmsJotoNewAuthorizeKey,
            QsApiSystemTypeEnum.QolmsJoto,
            executor,
            String.Empty
        ) With {
            .SessionId = sessionID
        }
        Dim apiResults As QiQolmsJotoNewAuthorizeKeyApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoNewAuthorizeKeyApiResults)(
            apiArgs,
            ApiAuthorizeWorker.DUMMY_SESSION_ID,
            ApiAuthorizeWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"

    <Obsolete("未使用です。")>
    Public Shared Function ClearKeys(executor As Guid) As Boolean

        Return ApiAuthorizeWorker.ExecuteQolmsDeleteAuthorizeKeysApi(executor)

    End Function

    ''' <summary>
    ''' 新しい QolmsApi 用の API 認証 キー と API 有効期限を取得します。
    ''' </summary>
    ''' <param name="executor">実行者 アカウント キー。</param>
    ''' <param name="sessionId">セッション ID。</param>
    ''' <param name="refAuthorizeKey">取得した API 認証 キー が格納される変数。</param>
    ''' <param name="refAuthorizeExpires">取得した API 有効期限が格納される変数。</param>
    Public Shared Sub NewKey(executor As Guid, sessionId As String, ByRef refAuthorizeKey As Guid, ByRef refAuthorizeExpires As Date)

        With ApiAuthorizeWorker.ExecuteQolmsNewAuthorizeKeyApi(executor, sessionId)
            refAuthorizeKey = .AuthorizeKey.TryToValueType(Guid.Empty)
            refAuthorizeExpires = .AuthorizeExpires.TryToValueType(Date.MinValue)
        End With

    End Sub

    ''' <summary>
    ''' 新しい QolmsJotoApi 用の API 認証 キー と API 有効期限を取得します。
    ''' </summary>
    ''' <param name="executor">実行者 アカウント キー。</param>
    ''' <param name="sessionId">セッション ID。</param>
    ''' <param name="refAuthorizeKey">取得した API 認証 キー が格納される変数。</param>
    ''' <param name="refAuthorizeExpires">取得した API 有効期限が格納される変数。</param>
    Public Shared Sub NewKey2(executor As Guid, sessionId As String, ByRef refAuthorizeKey As Guid, ByRef refAuthorizeExpires As Date)

        With ApiAuthorizeWorker.ExecuteQolmsJotoNewAuthorizeKeyApi(executor, sessionId)
            refAuthorizeKey = .AuthorizeKey.TryToValueType(Guid.Empty)
            refAuthorizeExpires = .AuthorizeExpires.TryToValueType(Date.MinValue)
        End With

    End Sub

#End Region

End Class
