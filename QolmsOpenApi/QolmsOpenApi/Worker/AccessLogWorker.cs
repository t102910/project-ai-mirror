
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Web.Http.Controllers;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsCryptV1;

/// <summary>
/// アクセス ログ の出力に関する機能を提供します。
/// この クラス は継承できません。
/// </summary>
internal sealed class AccessLogWorker
{
    /// <summary>
    /// アクセス ログ の種別を表します。
    /// </summary>
    public enum AccessTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsApiAccessTypeEnum.None,

        /// <summary>
        /// ログイン です。
        /// </summary>
        [Obsolete("未使用です。")]
        Login = QsApiAccessTypeEnum.Login,

        /// <summary>
        /// ログアウト です。
        /// </summary>
        [Obsolete("未使用です。")]
        Logout = QsApiAccessTypeEnum.Logout,

        /// <summary>
        /// 表示です。
        /// </summary>
        [Obsolete("未使用です。")]
        Show = QsApiAccessTypeEnum.Show,

        /// <summary>
        /// 登録です。
        /// </summary>
        [Obsolete("未使用です。")]
        Register = QsApiAccessTypeEnum.Register,

        /// <summary>
        /// 修正です。
        /// </summary>
        [Obsolete("未使用です。")]
        Modify = QsApiAccessTypeEnum.Modify,

        /// <summary>
        /// 削除です。
        /// </summary>
        [Obsolete("未使用です。")]
        Delete = QsApiAccessTypeEnum.Delete,

        /// <summary>
        /// エラー です。
        /// </summary>
        Error = QsApiAccessTypeEnum.Error,

        /// <summary>
        /// Web API です。
        /// </summary>
        Api = QsApiAccessTypeEnum.Api
    }
    public enum CommentTypeEnum : int
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = 0
    }

    /// <summary>
    /// コメント の種別を キー、コメント の内容を値とする ディクショナリ を表します。
    /// </summary>
    private static readonly Dictionary<CommentTypeEnum, string> Comments = new Dictionary<CommentTypeEnum, string>()
    {
        {
            CommentTypeEnum.None,
            string.Empty
        }
    };


    /// <summary>
    /// 例外 メッセージ を構築します。
    /// </summary>
    /// <param name="ex">例外 オブジェクト。</param>
    /// <param name="builder">メッセージ が格納される可変型文字列（オプショナル）。未指定の場合は メソッド 内部で インスタンス を作成。</param>
    /// <returns>メッセージ が格納された可変型文字列。</returns>
    internal static StringBuilder BuildExceptionMessage(Exception ex, StringBuilder builder = null)
    {
        if (builder == null)
            builder = new StringBuilder();

        if (ex != null)
        {
            builder.AppendFormat("■{0}：{1}", ex.GetType().ToString(), ex.Message).AppendLine();

            if (ex.InnerException != null)
                builder = AccessLogWorker.BuildExceptionMessage(ex.InnerException, builder);
        }

        return builder;
    }

    /// <summary>
    /// アクセス ログ キュー の末尾に、
    /// アクセス ログ を登録します。
    /// </summary>
    /// <param name="executor">実行者 ID。</param>
    /// <param name="accessDate">アクセス 日時。</param>
    /// <param name="accessType">アクセス タイプ。</param>
    /// <param name="accessUri">アクセス URI。</param>
    /// <param name="comment">コメント。</param>
    /// <param name="userHostAddress">ホスト アドレス。</param>
    /// <param name="userHostName">ホスト 名。</param>
    /// <param name="userAgent">ユーザー エージェント。</param>
    /// <remarks></remarks>
    private static void Enqueue(Guid executor, DateTime accessDate, byte accessType, string accessUri, string comment, string userHostAddress, string userHostName, string userAgent)
    {

        // Azure キュー ストレージ へ登録
        AccessQueueEntityWriter queueWriter = new AccessQueueEntityWriter();
        AccessQueueEntityWriterArgs queueWriterArgs = new AccessQueueEntityWriterArgs()
        {
            Entity = new QoAccessQueueEntity()
            {
                Message = new QoAccessQueueMessage()
                {
                    ACCOUNTKEY = executor.ToApiGuidString(),
                    ACCESSDATE = accessDate.ToApiDateString(),
                    ACCESSTYPE = accessType.ToString(),
                    ACCESSURI = accessUri,
                    COMMENT = comment,
                    USERHOSTADDRESS = userHostAddress,
                    USERHOSTNAME = userHostName,
                    USERAGENT = userAgent
                }
            }
        };
        AccessQueueEntityWriterResults queueWriterResults = QsAzureStorageManager.Write(queueWriter, queueWriterArgs);
    }

    /// <summary>
    /// 値を指定して、
    /// アクセス ログ を出力します。
    /// </summary>
    /// <param name="executeSystemType">実行 システム の種別。</param>
    /// <param name="executor">実行者 ID。</param>
    /// <param name="accessDate">アクセス 日時。</param>
    /// <param name="accessType">アクセス タイプ。</param>
    /// <param name="accessUri">アクセス URI。</param>
    /// <param name="comment">コメント。</param>
    /// <param name="userHostAddress">ホスト アドレス。</param>
    /// <param name="userHostName">ホスト 名。</param>
    /// <param name="userAgent">ユーザー エージェント。</param>
    /// <param name="sleep">
    /// ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    /// 未指定の場合は待機なし。
    /// </param>
    /// <remarks></remarks>
    [Obsolete]
    public static void WriteAccessLog(QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate, AccessTypeEnum accessType, string accessUri, string comment, string userHostAddress, string userHostName, string userAgent, int sleep = 0)
    {
        switch (executeSystemType)
        {
            case QsApiSystemTypeEnum.Qolms:
            case QsApiSystemTypeEnum.QolmsApi:
            case QsApiSystemTypeEnum.QolmsViewer:
            case QsApiSystemTypeEnum.QolmsViewerApi:
            case QsApiSystemTypeEnum.QolmsPortal:
            case QsApiSystemTypeEnum.QolmsPortalApi:
            case QsApiSystemTypeEnum.QolmsManagement:
            case QsApiSystemTypeEnum.QolmsManagementApi:
            case QsApiSystemTypeEnum.QolmsIdentityApi:
            case QsApiSystemTypeEnum.QolmsReportApi:
                // 既定のアクセスログテーブルが存在するシステムはApiManager.WriteAccessLogで振り分け
                string encComment = string.Empty;
                if (!string.IsNullOrWhiteSpace(comment))
                {
                    // コメント は暗号化
                    using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                    {
                        encComment = crypt.EncryptString(comment);
                    }
                }                
                QsApiManager.WriteAccessLog(executeSystemType, executor, accessDate, Convert.ToByte(accessType), accessUri, encComment, userHostAddress, userHostName, userAgent);
                break;
            default:
                {
                    // OpenApiのアクセスログに落とすシステム（アプリ含む）はキューを使用

                    // 「コルムス 公開 API サイト」の場合は Azure キュー ストレージへ 登録（コメント の暗号化は キュー 回収時に行う）
                    AccessLogWorker.Enqueue(executor, accessDate, Convert.ToByte(accessType), accessUri, comment, userHostAddress, userHostName, userAgent);
                    break;
                }
        }
        if (sleep > 0)
            Thread.Sleep(sleep); // 指定した ミリ 秒間待機する
    }

    /// <summary>
    /// コメント を指定して、
    /// アクセス ログ を出力します。
    /// </summary>
    /// <param name="context">コントローラ コンテキスト。</param>
    /// <param name="executeSystemType">実行 システム の種別。</param>
    /// <param name="executor">実行者 ID。</param>
    /// <param name="accessDate">
    /// アクセス 日時。
    /// クエリ 実行時に自動的に設定する場合は、
    /// Date.MinValueを指定。
    /// </param>
    /// <param name="accessType">アクセス タイプ。</param>
    /// <param name="accessUri">
    /// アクセス URI。
    /// 未指定の場合は リクエスト から自動取得。
    /// </param>
    /// <param name="comment">コメント。</param>
    /// <param name="sleep">
    /// ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    /// 未指定の場合は待機なし。
    /// </param>
    /// <remarks></remarks>
    public static void WriteAccessLog(HttpControllerContext context, QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate, AccessTypeEnum accessType, string accessUri, string comment, int sleep = 0)
    {
        AccessLogWorker.WriteAccessLog(executeSystemType, executor, accessDate, accessType, string.IsNullOrWhiteSpace(accessUri) && context != null && context.Request != null ? context.Request.RequestUri.AbsolutePath : accessUri, comment.Trim(), string.Empty, string.Empty, string.Empty, sleep);
    }

    /// <summary>
    /// コメント を指定して、
    /// アクセス ログ を出力します。
    /// </summary>
    /// <param name="context">コントローラ コンテキスト。</param>
    /// <param name="executeSystemType">実行 システム の種別。</param>
    /// <param name="executor">実行者 ID。</param>
    /// <param name="accessDate">
    /// アクセス 日時。
    /// クエリ 実行時に自動的に設定する場合は、
    /// cref="Date.MinValue"を指定。
    /// </param>
    /// <param name="accessType">アクセス タイプ。</param>
    /// <param name="accessUri">
    /// アクセス URI。
    /// 未指定の場合は リクエスト から自動取得。
    /// </param>
    /// <param name="comment">コメント。</param>
    /// <param name="userHostAddress"></param>
    /// <param name="userHostName"></param>
    /// <param name="userAgent"></param>
    /// <param name="sleep"></param>
    /// <remarks></remarks>
    public static void WriteAccessLog(HttpControllerContext context, QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate, AccessTypeEnum accessType, string accessUri, string comment, string userHostAddress, string userHostName, string userAgent, int sleep = 0)
    {
        AccessLogWorker.WriteAccessLog(executeSystemType, executor, accessDate, accessType, string.IsNullOrWhiteSpace(accessUri) && context != null && context.Request != null ? context.Request.RequestUri.AbsolutePath : accessUri, comment.Trim(), userHostAddress != null ? userHostAddress : string.Empty, userHostName != null ? userHostName : string.Empty, userAgent != null ? userAgent : string.Empty, sleep);
    }

    /// <summary>
    /// 例外 オブジェクト を指定して、
    /// エラー ログ を出力します。
    /// </summary>
    /// <param name="context">コントローラ コンテキスト。</param>
    /// <param name="executeSystemType">実行 システム の種別。</param>
    /// <param name="executor">実行者 ID。</param>
    /// <param name="accessDate">
    /// アクセス 日時。
    /// クエリ 実行時に自動的に設定する場合は、
    /// cref="Date.MinValue" を指定。
    /// </param>
    /// <param name="accessUri">
    /// アクセス URI。
    /// 未指定の場合は リクエスト から自動取得。
    /// </param>
    /// <param name="ex ">例外 オブジェクト。</param>
    /// <param name="sleep">
    /// ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。
    /// 未指定の場合は待機なし。
    /// </param>
    /// <remarks></remarks>
    public static void WriteErrorLog(HttpControllerContext context, QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate, string accessUri, Exception ex, int sleep = 0)
    {
        AccessLogWorker.WriteAccessLog(context, executeSystemType, executor, accessDate, AccessTypeEnum.Error, accessUri, AccessLogWorker.BuildExceptionMessage(ex).ToString(), sleep);
    }

    /// <summary>
    /// 例外 オブジェクト を指定して、
    /// エラー ログ を出力します。
    /// </summary>
    /// <param name="context">コントローラ コンテキスト。</param>
    /// <param name="executeSystemType">実行 システム の種別。</param>
    /// <param name="executor">実行者 ID。</param>
    /// <param name="accessDate">
    /// アクセス 日時。
    /// クエリ 実行時に自動的に設定する場合は、
    /// cref="Date.MinValue"を指定。
    /// </param>
    /// <param name="accessUri">
    /// アクセス URI。
    /// 未指定の場合は リクエスト から自動取得。
    /// </param>
    /// <param name="ex ">例外 オブジェクト。</param>
    /// <remarks></remarks>
    public static void WriteErrorLogAsync(HttpControllerContext context, QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate, string accessUri, Exception ex)
    {
        System.Threading.Tasks.Task.Run(() => AccessLogWorker.WriteAccessLog(context, executeSystemType, executor, accessDate, AccessTypeEnum.Error, accessUri, AccessLogWorker.BuildExceptionMessage(ex).ToString()));
    }

    /// <summary>
    /// [OpenApi専用]
    /// コメント を指定して、
    /// アクセス ログ を出力します。
    /// </summary>
    /// <param name="comment">コメント。</param>
    /// <param name="accessUri"></param>
    /// <remarks></remarks>
    internal static void WriteErrorLog(string comment, string accessUri = "")
    {
        AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessTypeEnum.Error, accessUri, comment);
    }
}
