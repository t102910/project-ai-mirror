Imports System.Threading
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsAzureStorageCoreV1
Imports MGF.QOLMS.QolmsCryptV1

''' <summary>
''' アクセス ログ の出力に関する機能を提供します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class AccessLogWorker

#Region "Enum"

    ''' <summary>
    ''' アクセス ログ の種別を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum AccessTypeEnum As Byte

        ''' <summary>
        ''' 未指定です。
        ''' </summary>
        ''' <remarks></remarks>
        None = QsApiAccessTypeEnum.None

        ''' <summary>
        ''' ログイン です。
        ''' </summary>
        ''' <remarks></remarks>
        Login = QsApiAccessTypeEnum.Login

        ''' <summary>
        ''' ログアウト です。
        ''' </summary>
        ''' <remarks></remarks>
        Logout = QsApiAccessTypeEnum.Logout

        ''' <summary>
        ''' 表示です。
        ''' </summary>
        ''' <remarks></remarks>
        Show = QsApiAccessTypeEnum.Show

        ''' <summary>
        ''' 登録です。
        ''' </summary>
        ''' <remarks></remarks>
        <Obsolete("未使用です。")>
        Register = QsApiAccessTypeEnum.Register

        ''' <summary>
        ''' 修正です。
        ''' </summary>
        ''' <remarks></remarks>
        <Obsolete("未使用です。")>
        Modify = QsApiAccessTypeEnum.Modify

        ''' <summary>
        ''' 削除です。
        ''' </summary>
        ''' <remarks></remarks>
        <Obsolete("未使用です。")>
        Delete = QsApiAccessTypeEnum.Delete

        ''' <summary>
        ''' エラー です。
        ''' </summary>
        ''' <remarks></remarks>
        [Error] = QsApiAccessTypeEnum.Error

        ''' <summary>
        ''' Web APIです。
        ''' </summary>
        ''' <remarks></remarks>
        <Obsolete("未使用です。")>
        Api = QsApiAccessTypeEnum.Api

    End Enum

    ''' <summary>
    ''' アクセス ログ の コメント の種別を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum CommentTypeEnum As Integer

        ''' <summary>
        ''' 未指定です。
        ''' </summary>
        ''' <remarks></remarks>
        None = 0

    End Enum

#End Region

#Region "Constant"

    ''' <summary>
    ''' アクセス ログ の コメント の種別をキー、
    ''' コメント を値とする ディクショナリ を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly Comments As New Dictionary(Of CommentTypeEnum, String) From {
        {
            CommentTypeEnum.None,
            String.Empty
        }
    }

    ''' <summary>
    ''' x-forwarded-for ヘッダー 名を表します。
    ''' </summary>
    Const X_FORWARDED_FOR_HEADER_NAME As String = "x-forwarded-for"

    ''' <summary>
    ''' x-original-host ヘッダー 名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Const X_ORIGINAL_HOST_HEADER_NAME As String = "x-original-host"

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

    ''' <summary>
    ''' 例外 メッセージ を構築します。
    ''' </summary>
    ''' <param name="ex">例外 オブジェクト。</param>
    ''' <param name="builder">
    ''' メッセージ が格納される可変型文字列（オプショナル）。
    ''' 未指定の場合は メソッド 内部で インスタンス を作成します。
    ''' </param>
    ''' <returns>メッセージ が格納された可変型文字列。</returns>
    ''' <remarks></remarks>
    Private Shared Function BuildExceptionMessage(ex As Exception, Optional builder As StringBuilder = Nothing) As StringBuilder

        If builder Is Nothing Then builder = New StringBuilder()

        If ex IsNot Nothing Then
            builder.AppendFormat("■{0}：{1}", ex.GetType().ToString(), ex.Message).AppendLine()

            If ex.InnerException IsNot Nothing Then
                builder = AccessLogWorker.BuildExceptionMessage(ex.InnerException, builder)
            End If
        End If

        Return builder

    End Function

    ''' <summary>
    ''' AlwaysOn による アクセス ログ かを判定します。
    ''' </summary>
    ''' <param name="accessType">アクセス ログ の種別。</param>
    ''' <param name="accessUri">アクセス URI。</param>
    ''' <param name="userHostAddress">ユーザー ホスト アドレス。</param>
    ''' <param name="userHostName">ユーザー ホスト 名。</param>
    ''' <param name="userAgent">ユーザー エージェント。</param>
    ''' <returns>
    ''' AlwaysOn による アクセス ログ なら True、
    ''' そうでなければ False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function IsAlwaysOn(
        accessType As AccessTypeEnum,
        accessUri As String,
        userHostAddress As String,
        userHostName As String,
        userAgent As String
    ) As Boolean

        Return accessType = AccessTypeEnum.Show _
            AndAlso String.Compare(accessUri, "/start/index", True) = 0 _
            AndAlso String.Compare(userHostAddress, "::1", True) = 0 _
            AndAlso String.Compare(userHostName, "::1", True) = 0 _
            AndAlso String.Compare(userAgent, "alwayson", True) = 0

    End Function

    ''' <summary>
    ''' ユーザー ホスト アドレス を取得します（アプリケーション ゲートウェイ 対応、MVC コントローラ 用）。
    ''' </summary>
    ''' <param name="request">HTTP 要求。</param>
    ''' <returns>
    ''' ユーザー ホスト アドレス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetUserHostAddress(request As HttpRequestBase) As String

        ' request の null チェック は済んでいる前提
        Dim result As String = String.Empty
        Dim forwardedFor As String = request.Headers.Get(AccessLogWorker.X_FORWARDED_FOR_HEADER_NAME)

        ' x-forwarded-for ヘッダー の値を優先して取得
        If Not String.IsNullOrWhiteSpace(forwardedFor) Then result = forwardedFor.Split(","c).ToList().First().Trim()

        ' x-forwarded-for ヘッダー の値が無ければ ユーザー ホスト アドレス を取得
        If String.IsNullOrWhiteSpace(result) Then result = request.UserHostAddress.Trim()

        ' ポート番号付き IPv4 の場合だけ ポート 部分を取り除く
        ' その他の場合は加工せずそのままにしておく（IPv6 対応）
        If Not String.IsNullOrWhiteSpace(result) AndAlso String.Compare(result, "::1") <> 0 Then
            Dim firstIndex As Integer = result.IndexOf(":")
            Dim lastIndex As Integer = result.LastIndexOf(":")

            ' ":" が 1 つなら、おそらくポート 番号付き IPv4
            If firstIndex = lastIndex AndAlso firstIndex > 0 Then result = result.Substring(0, firstIndex)
        End If

        Return result

    End Function

    ''' <summary>
    ''' ユーザー ホスト 名 を取得します（アプリケーション ゲートウェイ 対応、MVC コントローラ 用）。
    ''' </summary>
    ''' <param name="request">HTTP 要求。</param>
    ''' <returns>
    ''' ユーザー ホスト 名。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetUserHostName(request As HttpRequestBase) As String

        ' request の null チェック は済んでいる前提
        Dim result As String = String.Empty
        Dim originalHost As String = request.Headers.Get(AccessLogWorker.X_ORIGINAL_HOST_HEADER_NAME)

        ' x-original-host ヘッダー の値を優先して取得
        If Not String.IsNullOrWhiteSpace(originalHost) Then result = originalHost.Split(","c).ToList().First().Trim()

        ' x-original-host ヘッダー の値が無ければ ユーザー ホスト 名 を取得
        If String.IsNullOrWhiteSpace(result) Then result = request.UserHostName.Trim()

        Return result

    End Function

    ''' <summary>
    ''' アクセス ログ を、
    ''' Azure テーブル ストレージ へ登録します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <param name="accessDate">アクセス 日時。</param>
    ''' <param name="accessType">アクセス ログ の種別。</param>
    ''' <param name="accessUri">アクセス URI。</param>
    ''' <param name="comment">補足 コメント（暗号化済みの物を指定）。</param>
    ''' <param name="userHostAddress">ユーザー ホスト アドレス。</param>
    ''' <param name="userHostName">ユーザー ホスト 名。</param>
    ''' <param name="userAgent">ユーザー エージェント。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function WriteTableStorage(
        accountKey As Guid,
        accessDate As Date,
        accessType As AccessTypeEnum,
        accessUri As String,
        comment As String,
        userHostAddress As String,
        userHostName As String,
        userAgent As String
    ) As Boolean

        ' テーブル ストレージ へ登録
        Try
            Dim storageWriter As New AccessTableEntityWriter(Of QhAccessTableEntity)()
            Dim storageWriterArgs As New AccessTableEntityWriterArgs(Of QhAccessTableEntity)() With {
                .AccountKey = accountKey,
                .AccessDate = accessDate,
                .AccessType = accessType,
                .AccessUri = accessUri,
                .Comment = comment,
                .UserHostAddress = userHostAddress,
                .UserHostName = userHostName,
                .UserAgent = userAgent
            }
            Dim storageWriterResults As AccessTableEntityWriterResults = QsAzureStorageManager.Write(storageWriter, storageWriterArgs)

            Return storageWriterResults.IsSuccess AndAlso storageWriterResults.Result = 1
        Catch
            Return False
        End Try

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' コメント を指定して、
    ''' アクセス ログ を出力します。
    ''' </summary>
    ''' <param name="context">コントローラ コンテキスト。</param>
    ''' <param name="accessUri">
    ''' アクセス URI。
    ''' 未指定の場合は リクエスト から自動取得します。
    ''' </param>
    ''' <param name="accessType">アクセス ログ の種別。</param>
    ''' <param name="comment">コメント。</param>
    ''' <param name="sleep">
    ''' ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    ''' 未指定の場合は待機しません。
    ''' </param>
    ''' <remarks></remarks>
    Public Shared Sub WriteAccessLog(context As HttpContextBase, accessUri As String, accessType As AccessTypeEnum, comment As String, Optional sleep As Integer = 0)

        ' コメント を暗号化
        Dim encComment As String = String.Format("[Yappli] {0}", If(comment Is Nothing, String.Empty, comment)).Trim()

        Using cryptor As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            encComment = cryptor.EncryptString(encComment)
        End Using

        If context IsNot Nothing AndAlso context.Request IsNot Nothing Then
            ' HTTP 要求有り
            Dim mainModel As QolmsYappliModel = QyLoginHelper.GetQolmsYappliModel(context.Session)

            With context.Request
                Dim uri As String = If(String.IsNullOrWhiteSpace(accessUri), .Url.AbsolutePath, accessUri)

                ' Azure による接続維持のための ログ は出力しない
                If AccessLogWorker.IsAlwaysOn(
                    accessType,
                    uri,
                    .UserHostAddress,
                    .UserHostName,
                    .UserAgent
                ) Then
                    Exit Sub
                End If

                If mainModel IsNot Nothing Then
                    ' ログイン 中

                    ' テーブル ストレージ へ登録
                    AccessLogWorker.WriteTableStorage(
                        mainModel.AuthorAccount.AccountKey,
                        Date.Now,
                        accessType,
                        uri,
                        encComment,
                        AccessLogWorker.GetUserHostAddress(context.Request),
                        AccessLogWorker.GetUserHostName(context.Request),
                        .UserAgent
                    )
                Else
                    ' 未 ログイン

                    ' テーブル ストレージ へ登録
                    AccessLogWorker.WriteTableStorage(
                        Guid.Empty,
                        Date.Now,
                        accessType,
                        uri,
                        encComment,
                        AccessLogWorker.GetUserHostAddress(context.Request),
                        AccessLogWorker.GetUserHostName(context.Request),
                        .UserAgent
                    )
                End If
            End With
        Else
            ' HTTP 要求無し
            Dim uri As String = If(String.IsNullOrWhiteSpace(accessUri), String.Empty, accessUri)

            ' テーブル ストレージ へ登録
            AccessLogWorker.WriteTableStorage(
                Guid.Empty,
                Date.Now,
                accessType,
                uri,
                encComment,
                String.Empty,
                String.Empty,
                String.Empty
            )
        End If

        ' 指定した ミリ 秒間待機する
        If sleep > 0 Then Thread.Sleep(sleep)

    End Sub

    ''' <summary>
    ''' コメント の種別を指定して、
    ''' アクセス ログ を出力します。
    ''' </summary>
    ''' <param name="context">コントローラ コンテキスト。</param>
    ''' <param name="accessUri">
    ''' アクセス URI。
    ''' 未指定の場合は リクエスト から自動取得します。
    ''' </param>
    ''' <param name="accessType">アクセス ログ の種別。</param>
    ''' <param name="commentType">コメント の種別。</param>
    ''' <param name="sleep">
    ''' ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    ''' 未指定の場合は待機しません。
    ''' </param>
    ''' <remarks></remarks>
    Public Shared Sub WriteAccessLog(context As HttpContextBase, accessUri As String, accessType As AccessTypeEnum, commentType As CommentTypeEnum, Optional sleep As Integer = 0)

        AccessLogWorker.WriteAccessLog(context, accessUri, accessType, If(AccessLogWorker.Comments.ContainsKey(commentType), AccessLogWorker.Comments(commentType), String.Empty), sleep)

    End Sub

    ''' <summary>
    ''' コメント を指定して、
    ''' デバッグ ログ を出力します。
    ''' この メソッド は デバッグ ビルド 時のみ実行されます。
    ''' </summary>
    ''' <param name="context">コントローラ コンテキスト。</param>
    ''' <param name="accessUri">
    ''' アクセス URI。
    ''' 未指定の場合は リクエスト から自動取得します。
    ''' </param>
    ''' <param name="comment">コメント。</param>
    ''' <param name="sleep">
    ''' ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    ''' 未指定の場合は待機しません。
    ''' </param>
    ''' <remarks></remarks>
    <Conditional("DEBUG")>
    Public Shared Sub WriteDebugLog(context As HttpContextBase, accessUri As String, comment As String, Optional sleep As Integer = 0)

        AccessLogWorker.WriteAccessLog(context, accessUri, AccessTypeEnum.None, comment, sleep)

    End Sub

    ''' <summary>
    ''' コメント を指定して、
    ''' エラー ログ を出力します。
    ''' </summary>
    ''' <param name="context">コントローラ コンテキスト。</param>
    ''' <param name="accessUri">
    ''' アクセス URI。
    ''' 未指定の場合は リクエスト から自動取得します。
    ''' </param>
    ''' <param name="comment">コメント。</param>
    ''' <param name="sleep">
    ''' ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    ''' 未指定の場合は待機しません。
    ''' </param>
    ''' <remarks></remarks>
    Public Shared Sub WriteErrorLog(context As HttpContextBase, accessUri As String, comment As String, Optional sleep As Integer = 0)

        AccessLogWorker.WriteAccessLog(context, accessUri, AccessTypeEnum.Error, comment, sleep)

    End Sub

    ''' <summary>
    ''' 例外 オブジェクト を指定して、
    ''' エラー ログ を出力します。
    ''' </summary>
    ''' <param name="context">コントローラ コンテキスト。</param>
    ''' <param name="accessUri">
    ''' アクセス URI。
    ''' 未指定の場合は リクエスト から自動取得します。
    ''' </param>
    ''' <param name="ex ">例外 オブジェクト。</param>
    ''' <param name="sleep">
    ''' ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    ''' 未指定の場合は待機しません。
    ''' </param>
    ''' <remarks></remarks>
    Public Shared Sub WriteErrorLog(context As HttpContextBase, accessUri As String, ex As Exception, Optional sleep As Integer = 0)

        AccessLogWorker.WriteAccessLog(context, accessUri, AccessTypeEnum.Error, AccessLogWorker.BuildExceptionMessage(ex).ToString(), sleep)

    End Sub

    'テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "log.txt")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub
    
    'テスト用の手抜きログ吐き
    ''' <summary>
    ''' MapPath と ファイル名を指定してログを書き込みします。
    ''' デバック専用です。
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="fileName"></param>
    ''' <param name="message"></param>
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(path As String,fileName As String,ByVal message As String,context As HttpContextBase)
        Try
            Dim log As String = IO.Path.Combine(context.Server.MapPath(path), fileName)
            IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub

#End Region

End Class
