Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' ログインに関する補助機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class QyLoginHelper

#Region "Constant"

    ''' <summary>
    ''' セッション状態オブジェクト内でのログインモデルのキー名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly LOGIN_MODEL_SESSION_KEY As String = GetType(LoginModel).Name

    ''' <summary>LoginModel
    ''' セッション状態オブジェクト内でのメインモデルのキー名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly MAIN_MODEL_SESSION_KEY As String = GetType(QolmsYappliModel).Name

    ''' <summary>
    ''' 自動ログインの期限日数を表します。（本日から）
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTOLOGIN_EXPIRES As Integer = 20

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
    ''' <see cref="QolmsYappliModel" /> の有効性をチェックします。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 有効なら True、
    ''' 無効なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CheckQolmsYappliModel(mainModel As QolmsYappliModel) As Boolean

        Return mainModel IsNot Nothing _
            AndAlso mainModel.AuthorAccount IsNot Nothing _
            AndAlso mainModel.AuthorAccount.AccountKey <> Guid.Empty _
            AndAlso Not String.IsNullOrWhiteSpace(mainModel.AuthorAccount.Name)

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' ログインモデルを取得します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <returns>
    ''' 成功ならログインモデルのインスタンス、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetLoginModel(session As HttpSessionStateBase) As LoginModel

        Dim result As LoginModel = Nothing

        Return If(QySessionHelper.GetItem(session, QyLoginHelper.LOGIN_MODEL_SESSION_KEY, result), result, Nothing)

    End Function

    ''' <summary>
    ''' ログインモデルを削除します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <remarks></remarks>
    Public Shared Sub RemoveLoginModel(session As HttpSessionStateBase)

        QySessionHelper.RemoveItem(session, QyLoginHelper.LOGIN_MODEL_SESSION_KEY)

    End Sub

    ''' <summary>
    ''' ログインモデルを追加します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <param name="model">ログイン処理モデル。</param>
    ''' <remarks></remarks>
    Public Shared Sub SetLoginModel(session As HttpSessionStateBase, model As LoginModel)

        QySessionHelper.RemoveItem(session, QyLoginHelper.LOGIN_MODEL_SESSION_KEY)
        QySessionHelper.SetItem(session, QyLoginHelper.LOGIN_MODEL_SESSION_KEY, model)

    End Sub

    ''' <summary>
    ''' メイン モデルを取得します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <returns>
    ''' 成功ならメイン モデルのインスタンス、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetQolmsYappliModel(session As HttpSessionStateBase) As QolmsYappliModel

        Dim result As QolmsYappliModel = Nothing

        Return If(QySessionHelper.GetItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY, result) AndAlso QyLoginHelper.CheckQolmsYappliModel(result), result, Nothing)

    End Function

    ''' <summary>
    ''' ログイン済みかチェックします。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <param name="request">HTTP 要求。</param>
    ''' <returns>
    ''' ログイン済みなら True、
    ''' 未ログインなら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CheckLogin(session As HttpSessionStateBase, request As HttpRequestBase) As Boolean

        Dim model As QolmsYappliModel = Nothing

        Return QySessionHelper.GetItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY, model) _
            AndAlso QyLoginHelper.CheckQolmsYappliModel(model) _
            AndAlso QyCookieHelper.CheckFormsAuthCookie(request, model.AuthorAccount.UserId)

    End Function

    ' TODO: ダミー
    Public Shared Function ToLogin(
        session As HttpSessionStateBase,
        response As HttpResponseBase,
        authorAccount As AuthorAccountItem,
        apiAuthorizeKey As Guid,
        apiAuthorizeExpires As Date,
        apiAuthorizeKey2 As Guid,
        apiAuthorizeExpires2 As Date,
        rememberId As Boolean,
        rememberLogin As Boolean
    ) As Boolean

        Dim result As Boolean = False

        Try
            Dim mainModel As QolmsYappliModel = New QolmsYappliModel(
                authorAccount,
                session.SessionID,
                apiAuthorizeKey,
                apiAuthorizeExpires,
                apiAuthorizeKey2,
                apiAuthorizeExpires2
            )

            If mainModel IsNot Nothing Then
                ' 認証クッキーを設定
                QyCookieHelper.SetFormsAuthCookie(response, authorAccount.UserId, session.Timeout)

                ' TODO:
                QyCookieHelper.SetRememberIdCookie(response, rememberId, authorAccount.UserId, authorAccount.LoginAt)

                mainModel.SetIsAutoLogin(QyCookieHelper.SetRememberLoginCookie(response, rememberLogin, authorAccount.UserId, authorAccount.PasswordHash, authorAccount.LoginAt, Date.Now.AddDays(AUTOLOGIN_EXPIRES)))

                'StandardValueWorker.SetStandardValue(mainModel, standardValueN)

                'TargetValueWorker.SetTargetValue(mainModel, targetValueN, height)

                QySessionHelper.RemoveItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY)
                QySessionHelper.SetItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY, mainModel)

                ' 成功
                result = True
            End If
        Catch
            ' エラー
            Throw
        End Try

        Return result

    End Function

    'OpenIDでのログインはこちらを使います。
    Public Shared Function ToLogin(
           session As HttpSessionStateBase,
           response As HttpResponseBase,
           authorAccount As AuthorAccountItem,
           apiAuthorizeKey As Guid,
           apiAuthorizeExpires As Date,
           apiAuthorizeKey2 As Guid,
           apiAuthorizeExpires2 As Date,
           byWowId As Boolean
       ) As Boolean

        Dim result As Boolean = False

        Try
            Dim mainModel As QolmsYappliModel = New QolmsYappliModel(
                authorAccount,
                session.SessionID,
                apiAuthorizeKey,
                apiAuthorizeExpires,
                apiAuthorizeKey2,
                apiAuthorizeExpires2
            )

            If mainModel IsNot Nothing Then
                ' 認証クッキーを設定
                QyCookieHelper.SetFormsAuthCookie(response, authorAccount.UserId, session.Timeout)
                QySessionHelper.RemoveItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY)
                QySessionHelper.SetItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY, mainModel)

                ' 成功
                result = True
            End If
        Catch
            ' エラー
            Throw
        End Try

        Return result

    End Function
    ''' <summary>
    ''' ログアウト状態へ移行します。
    ''' </summary>
    ''' <param name="session"></param>
    ''' <param name="response"></param>
    ''' <param name="clearRemember"></param>
    ''' <remarks></remarks>
    Public Shared Sub ToLogout(session As HttpSessionStateBase, response As HttpResponseBase, clearRemember As Boolean)

        QySessionHelper.RemoveItem(session, QyLoginHelper.MAIN_MODEL_SESSION_KEY)

        ' TODO:
        If clearRemember Then
            QyCookieHelper.DisableRememberLoginCookie(response)
            QyCookieHelper.DisableRememberIdCookie(response)
            'auのautoログイン設定のcookieを削除
            QyCookieHelper.DisableAutoAuIdLoginCookie(response)
        End If

    End Sub

#End Region

End Class
