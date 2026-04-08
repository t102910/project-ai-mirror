using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アクセス ログ の出力に関する機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class AccessLogWorker
    {
        #region "Enum"

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
            Login = QsApiAccessTypeEnum.Login,

            /// <summary>
            /// ログアウト です。
            /// </summary>
            Logout = QsApiAccessTypeEnum.Logout,

            /// <summary>
            /// 表示です。
            /// </summary>
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
            /// Web APIです。
            /// </summary>
            [Obsolete("未使用です。")]
            Api = QsApiAccessTypeEnum.Api

        }

        /// <summary>
        /// アクセス ログ の コメント の種別を表します。
        /// </summary>
        public enum CommentTypeEnum : int
        {
            /// <summary>
            /// 未指定です。
            /// </summary>
            None = 0
        }

        #endregion

        #region "Constant"

        /// <summary>
        /// アクセス ログ の コメント の種別をキー、
        /// コメント を値とする ディクショナリ を表します。
        /// </summary>
        private static readonly Dictionary<CommentTypeEnum, String> Comments = new Dictionary<CommentTypeEnum, string>()
        {
            { CommentTypeEnum.None,string.Empty}
        };

        /// <summary>
        /// x-forwarded-for ヘッダー 名を表します。
        /// </summary>
        const string X_FORWARDED_FOR_HEADER_NAME = "x-forwarded-for";

        /// <summary>
        /// x-original-host ヘッダー 名を表します。
        /// </summary>
        const string X_ORIGINAL_HOST_HEADER_NAME = "x-original-host";

        #endregion

        #region "Constructor"
        private AccessLogWorker() : base() { }

        #endregion

        #region "Private Method"

        /// <summary>
        /// 例外 メッセージ を構築します。
        /// </summary>
        /// <param name="ex">例外 オブジェクト。</param>
        /// <param name="builder">
        /// メッセージ が格納される可変型文字列（オプショナル）。
        /// 未指定の場合は メソッド 内部で インスタンス を作成します。
        /// </param>
        /// <returns>メッセージ が格納された可変型文字列。</returns>
        private static StringBuilder BuildExceptionMessage(Exception ex, StringBuilder builder = null)
        {
            if (builder == null)
            {
                builder = new StringBuilder();
            }

            if (ex != null)
            {
                builder
                    .AppendFormat("■ {0} : {1}", ex.GetType().ToString(), ex.Message)
                    .AppendLine();

                if (ex.InnerException != null)
                {
                    BuildExceptionMessage(ex.InnerException, builder);
                }
            }

            return builder;
        }


        /// <summary>
        /// AlwaysOn による アクセス ログ かを判定します。
        /// </summary>
        /// <param name="accessType">アクセス ログ の種別。</param>
        /// <param name="accessUri">アクセス URI。</param>
        /// <param name="userHostAddress">ユーザー ホスト アドレス。</param>
        /// <param name="userHostName">ユーザー ホスト 名。</param>
        /// <param name="userAgent">ユーザー エージェント。</param>
        /// <returns>
        /// AlwaysOn による アクセス ログ なら True、
        /// そうでなければ False。
        /// </returns>
        private static bool IsAlwaysOn(
           AccessTypeEnum accessType,
           string accessUri,
           string userHostAddress,
           string userHostName,
           string userAgent
        )
        {
            return accessType == AccessTypeEnum.Show
                && string.Compare(accessUri, "/start/index", true) == 0
                && string.Compare(userHostAddress, "::1", true) == 0
                && string.Compare(userHostName, "::1", true) == 0
                && string.Compare(userAgent, "alwayson", true) == 0;
        }

        /// <summary>
        /// ユーザー ホスト アドレス を取得します（アプリケーション ゲートウェイ 対応、MVC コントローラ 用）。
        /// </summary>
        /// <param name="request">HTTP 要求。</param>
        /// <returns>
        /// ユーザー ホスト アドレス。
        /// </returns>
        private static string GetUserHostAddress(HttpRequestBase request)
        {
            string result = string.Empty;
            string forwardedFor = request.Headers.Get(AccessLogWorker.X_FORWARDED_FOR_HEADER_NAME);

            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                result = forwardedFor.Split(',').ToList().First().Trim();
            }
            if (string.IsNullOrWhiteSpace(result))
            {
                result = request.UserHostAddress.Trim();
            }

            if (!string.IsNullOrWhiteSpace(result) && string.Compare(result, "::1") != 0)
            {
                var firstIndex = result.IndexOf(":");
                var lastIndex = result.LastIndexOf(":");

                if (firstIndex == lastIndex && firstIndex > 0)
                {
                    result = result.Substring(0, firstIndex);
                }
            }

            return result;
        }

        /// <summary>
        /// ユーザー ホスト 名 を取得します（アプリケーション ゲートウェイ 対応、MVC コントローラ 用）。
        /// </summary>
        /// <param name="request">HTTP 要求。</param>
        /// <returns>ユーザー ホスト 名。</returns>
        private static string GetUserHostName(HttpRequestBase request)
        {
            //' request の null チェック は済んでいる前提
            var result = string.Empty;
            var originalHost = request.Headers.Get(AccessLogWorker.X_ORIGINAL_HOST_HEADER_NAME);

            //' x-original-host ヘッダー の値を優先して取得
            if (!string.IsNullOrWhiteSpace(originalHost))
            {
                originalHost.Split(',').ToList().First().Trim();
            }

            //' x-original-host ヘッダー の値が無ければ ユーザー ホスト 名 を取得
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = request.UserHostName.Trim();
            }

            return result;
        }

        /// <summary>
        /// アクセス ログ を、Azure テーブル ストレージ へ登録します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="accessDate"></param>
        /// <param name="accessType"></param>
        /// <param name="accessUri"></param>
        /// <param name="comment"></param>
        /// <param name="userHostAddress"></param>
        /// <param name="userHostName"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private static bool WriteTableStorage(
            Guid accountKey,
            DateTime accessDate,
            AccessTypeEnum accessType,
            string accessUri,
            string comment,
            string userHostAddress,
            string userHostName,
            string userAgent)
        {
            //' テーブル ストレージ へ登録
            try
            {
                var storageWriter = new AccessTableEntityWriter<QjAccessTableEntity>();
                var storageWriterArgs = new AccessTableEntityWriterArgs<QjAccessTableEntity>()
                {
                    AccountKey = accountKey,
                    AccessDate = accessDate,
                    AccessType = (byte)accessType,
                    AccessUri = accessUri,
                    Comment = comment,
                    UserHostAddress = userHostAddress,
                    UserHostName = userHostName,
                    UserAgent = userAgent
                };
                var storageWriterResults = QsAzureStorageManager.Write(storageWriter, storageWriterArgs);
                return storageWriterResults.IsSuccess && storageWriterResults.Result == 1;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region "Public Method"

        public static void WriteAccessLog(HttpContextBase context, string accessUri, AccessTypeEnum accessType, string comment, int sleep = 0)
        {
            var encComment = $"[JOTOWebView] {(string.IsNullOrWhiteSpace(comment) ? string.Empty: comment).Trim()}";

            using (var cryptor = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                encComment = cryptor.EncryptString(encComment);
            }

            if (context != null && context.Request != null)
            {
                var mainModel = QjLoginHelper.GetQolmsJotoModel(context.Session);
                var uri = string.IsNullOrWhiteSpace(accessUri) ? context.Request.Url.AbsolutePath : accessUri;

                if (AccessLogWorker.IsAlwaysOn(
                    accessType,
                    uri,
                     context.Request.UserHostAddress,
                     context.Request.UserHostName,
                     context.Request.UserAgent
                ))
                {
                    return;
                }

                if (mainModel != null)
                {
                    AccessLogWorker.WriteTableStorage(
                        mainModel.AuthorAccount.AccountKey,
                        DateTime.Now,
                        accessType,
                        uri,
                        encComment,
                        AccessLogWorker.GetUserHostAddress(context.Request),
                        AccessLogWorker.GetUserHostName(context.Request),
                        context.Request.UserAgent
                    );
                }
                else
                {
                    AccessLogWorker.WriteTableStorage(
                        Guid.Empty,
                        DateTime.Now,
                        accessType,
                        uri,
                        encComment,
                        AccessLogWorker.GetUserHostAddress(context.Request),
                        AccessLogWorker.GetUserHostName(context.Request),
                        context.Request.UserAgent
                    );
                }
            }
            else
            {
                var uri = string.IsNullOrWhiteSpace(accessUri) ? string.Empty : accessUri;
                //' テーブル ストレージ へ登録
                AccessLogWorker.WriteTableStorage(
                    Guid.Empty,
                    DateTime.Now,
                    accessType,
                    uri,
                    encComment,
                    string.Empty,
                    string.Empty,
                    string.Empty
                );
            }

            if (sleep > 0)
            {
                //指定した ミリ 秒間待機する
                Thread.Sleep(sleep);
            }
        }

        /// <summary>
        /// コメント の種別を指定して、
        /// アクセス ログ を出力します。
        /// </summary>
        /// <param name="context">コントローラ コンテキスト。</param>
        /// <param name="accessUri">アクセス URI。未指定の場合は リクエスト から自動取得します。</param>
        /// <param name="accessType">アクセス ログ の種別。</param>
        /// <param name="commentType">コメント の種別。</param>
        /// <param name="sleep">ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。未指定の場合は待機しません。</param>
        public static void WriteAccessLog(HttpContextBase context, string accessUri, AccessTypeEnum accessType, CommentTypeEnum commentType, int sleep = 0)
        {
            AccessLogWorker.WriteAccessLog(context, accessUri, accessType, AccessLogWorker.Comments.ContainsKey(commentType) ? AccessLogWorker.Comments[commentType] : string.Empty, sleep);
        }

        /// <summary>
        /// コメント を指定して、
        /// デバッグ ログ を出力します。
        /// この メソッド は デバッグ ビルド 時のみ実行されます。
        /// </summary>
        /// <param name="context">コントローラ コンテキスト。</param>
        /// <param name="accessUri">アクセス URI。未指定の場合は リクエスト から自動取得します。</param>
        /// <param name="comment">コメント。</param>
        /// <param name="sleep">ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。未指定の場合は待機しません。</param>
        [Conditional("DEBUG")]
        public static void WriteDebugLog(HttpContextBase context, string accessUri, string comment, int sleep = 0)
        {
            AccessLogWorker.WriteAccessLog(context, accessUri, AccessTypeEnum.None, comment, sleep);
        }

        /// <summary>
        /// コメント を指定して、
        /// エラー ログ を出力します。
        /// </summary>
        /// <param name="context">コントローラ コンテキスト。</param>
        /// <param name="accessUri">アクセス URI。未指定の場合は リクエスト から自動取得します。</param>
        /// <param name="comment">コメント。</param>
        /// <param name="sleep">ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。未指定の場合は待機しません。</param>
        public static void WriteErrorLog(HttpContextBase context, string accessUri, string comment, int sleep = 0)
        {
            AccessLogWorker.WriteAccessLog(context, accessUri, AccessTypeEnum.Error, comment, sleep);
        }

        /// <summary>
        /// 例外 オブジェクト を指定して、
        /// エラー ログ を出力します。
        /// </summary>
        /// <param name="context">コントローラ コンテキスト。</param>
        /// <param name="accessUri">アクセス URI。未指定の場合は リクエスト から自動取得します。</param>
        /// <param name="comment">コメント。</param>
        /// <param name="sleep">ログ 出力後待機する時間を ミリ 秒で指定（オプショナル）。未指定の場合は待機しません。</param>
        public static void WriteErrorLog(HttpContextBase context, string accessUri, Exception ex, int sleep = 0)
        {
            AccessLogWorker.WriteAccessLog(context, accessUri, AccessTypeEnum.Error, AccessLogWorker.BuildExceptionMessage(ex).ToString(), sleep);
        }

        //テスト用の手抜きログ吐き
        //public static void DebugLog(ref string message) 
        //{
        //    try
        //    {

        //    }
        //    catch 
        //    {
        //    }
        //}

        #endregion

    }
}