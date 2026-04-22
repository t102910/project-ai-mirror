using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アクセス ログ の出力に関する機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class QoAccessLog
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
            ///Login = QsApiAccessTypeEnum.Login,

            /// <summary>
            /// ログアウト です。
            /// </summary>
            ///Logout = QsApiAccessTypeEnum.Logout,

            /// <summary>
            /// 表示です。
            /// </summary>
            ///Show = QsApiAccessTypeEnum.Show,

            /// <summary>
            /// 登録です。
            /// </summary>
            ///[Obsolete("未使用です。")]
            ///Register = QsApiAccessTypeEnum.Register,

            /// <summary>
            /// 修正です。
            /// </summary>
            ///[Obsolete("未使用です。")]
            ///Modify = QsApiAccessTypeEnum.Modify,

            /// <summary>
            /// 削除です。
            /// </summary>
            ///[Obsolete("未使用です。")]
            ///Delete = QsApiAccessTypeEnum.Delete,

            /// <summary>
            /// エラー です。
            /// </summary>
            Error = QsApiAccessTypeEnum.Error,

            /// <summary>
            /// Web APIです。
            /// </summary>
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
        /// アクセス ログ の コメント の種別を キー、
        /// コメント を値とする ディクショナリ を表します。
        /// </summary>
        private static readonly Dictionary<CommentTypeEnum, string> Comments = new Dictionary<CommentTypeEnum, string>()
        {
            {
                CommentTypeEnum.None,
                string.Empty
            }
        };
        
       

        /// <summary>
        /// デバッグモードかを取得または設定します。
        /// </summary>
        public static bool IsDebug { get; set; }
        
        


        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="QoAccessLog" /> クラス の新しい インスタンス を 1 度だけ初期化します。
        /// </summary>
        static QoAccessLog() { }

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
        /// <returns>
        /// メッセージ が格納された可変型文字列。
        /// </returns>
        private static StringBuilder BuildExceptionMessage(Exception ex, StringBuilder builder = null)
        {
            if (builder == null) builder = new StringBuilder();

            if (ex != null)
            {
                builder.AppendFormat("■{0}：{1}", ex.GetType().ToString(), ex.Message).AppendLine();

                if (ex.InnerException != null) builder = QoAccessLog.BuildExceptionMessage(ex.InnerException, builder);
            }

            return builder;
        }
        private static string GetClientIp(System.Net.Http.HttpRequestMessage request = null)
        {
            
            if (request!=null && request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return string.Empty ;
            }
        }
        /// <summary>
        /// アクセス ログ を、
        /// Azure テーブル ストレージ へ登録します。
        /// </summary>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="accessDate">アクセス 日時。</param>
        /// <param name="accessType">アクセス  ログ の種別。</param>
        /// <param name="accessUri">アクセス URI。</param>
        /// <param name="comment">補足 コメント（暗号化済みの物を指定）。</param>
        /// <param name="userHostAddress">ユーザー ホスト アドレス。</param>
        /// <param name="userHostName">ユーザー ホスト 名。</param>
        /// <param name="userAgent">ユーザー エージェント。</param>
        /// <returns>
        /// 成功ならtrue失敗ならfalse
        /// </returns>
        private static bool WriteTableStorage(
            Guid accountKey,
            DateTime accessDate,
            AccessTypeEnum accessType,
            string accessUri,
            string comment,
            string userHostAddress,
            string userHostName,
            string userAgent
        )
        {
            // テーブル　ストレージ へ登録
            try
            {
                var storageWriter = new AccessTableEntityWriter<QoAccessTableEntity>();
                var storageWriterArgs = new AccessTableEntityWriterArgs<QoAccessTableEntity>()
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
                AccessTableEntityWriterResults storageWriterResults = QsAzureStorageManager.Write(storageWriter, storageWriterArgs);

                return storageWriterResults.IsSuccess && storageWriterResults.Result == 1;
            }
            catch
            {
                return false;
            }
        }

        private static void WriteDebugLog(string message)
        {
            if (!IsDebug) return;
            try
            {
                string log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "Debuglog.txt");
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", DateTime.Now, message, Environment.NewLine ));
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region "Public Method"

 
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
        /// <param name="userHostAddress">ユーザー ホスト アドレス。</param>
        /// <param name="userHostName">ユーザー ホスト 名。</param>
        /// <param name="userAgent">ユーザー エージェント。</param>
        /// <param name="sleep">
        /// ログ 出力後待機する時間（ミリ 秒）（オプショナル）。
        /// 未指定の場合は待機しません。
        /// </param>
        public static void WriteAccessLog( QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate,
                                            AccessTypeEnum accessType, string accessUri, string comment,
                                            string userHostAddress,string userHostName,string userAgent,int sleep = 0 )
        {
          
            WriteDebugLog(string.Format("{0}:{1}", userHostAddress,comment));
            // 「コルムス Open API」の場合のみ Azure テーブル ストレージ へ登録
            //if (executeSystemType == QsApiSystemTypeEnum.QolmsOpenApi )
            //{
            // コメント を暗号化
            string encComment = string.Empty;

                if (!string.IsNullOrWhiteSpace(comment))
                {
                    using (var cryptor = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                    {
                        encComment = cryptor.EncryptString(comment.Trim());
                    }
                }

                // テーブル ストレージ へ登録
                QoAccessLog.WriteTableStorage(
                    executor,
                    accessDate,
                    accessType,
                    accessUri,
                    encComment,
                    userHostAddress,
                    userHostName,
                    userAgent
                );

                // 指定した ミリ 秒間待機する
                if (sleep > 0) Thread.Sleep(sleep);
            //}
        }

        /// <summary> コメント を指定して、アクセス ログ を出力します。</summary>
        /// <param name="context">コントローラ コンテキスト。</param>
        /// <param name="executeSystemType">実行 システム の種別。</param>
        /// <param name="executor">実行者 ID。</param>
        /// <param name="accessDate">
        /// アクセス 日時。クエリ 実行時に自動的に設定する場合は、
        /// <see cref="DateTime.MinValue" />を指定します。
        /// </param>
        /// <param name="accessType">アクセス ログ の種別。</param>
        /// <param name="accessUri">アクセス URI。未指定の場合は リクエスト から自動取得します。</param>
        /// <param name="comment">コメント。</param>
        /// <param name="sleep">ログ 出力後待機する時間（ミリ 秒）（オプショナル）。未指定の場合は待機しません。</param>
        public static void WriteAccessLog( HttpControllerContext context, QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate,
                                        AccessTypeEnum accessType,string accessUri, string comment, int sleep = 0 )
        {
            if (context != null && context.Request != null)
                QoAccessLog.WriteAccessLog(
                    executeSystemType,
                    executor,
                    accessDate,
                    accessType,
                    string.IsNullOrWhiteSpace(accessUri) ? context.Request.RequestUri.AbsolutePath : accessUri,
                    comment.Trim(),
                    GetClientIp(context.Request),
                    string.Empty,
                    string.Empty
                );
            else
                QoAccessLog.WriteAccessLog(
                    executeSystemType,
                    executor,
                    accessDate,
                    accessType,
                    string.Empty,
                    comment.Trim(),
                    GetClientIp(),
                    string.Empty,
                    string.Empty
                );

            // 指定した ミリ 秒間待機する
            if (sleep > 0) Thread.Sleep(sleep);
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
        /// <see cref="DateTime.MinValue" />を指定します。
        /// </param>
        /// <param name="accessUri">
        /// アクセス URI。
        /// 未指定の場合は リクエスト から自動取得します。
        /// </param>
        /// <param name="ex">例外 オブジェクト。</param>
        /// <param name="sleep">
        /// ログ 出力後待機する時間（ミリ 秒）（オプショナル）。
        /// 未指定の場合は待機しません。
        /// </param>
        public static void WriteErrorLog(HttpControllerContext context, QsApiSystemTypeEnum executeSystemType, Guid executor, DateTime accessDate, string accessUri, Exception ex, int sleep = 0)
        { 
             QoAccessLog.WriteAccessLog(context, executeSystemType, executor, accessDate, AccessTypeEnum.Error, accessUri, QoAccessLog.BuildExceptionMessage(ex).ToString(), sleep);
            WriteDebugLog(ex.StackTrace);
        }


        public static void WriteErrorLog(Exception ex, Guid executor)
        {
            QoAccessLog.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, executor, DateTime.Now, AccessTypeEnum.Error, string.Empty, QoAccessLog.BuildExceptionMessage(ex).ToString());
            WriteDebugLog(ex.StackTrace);
        }
        public static void WriteErrorLog(Exception ex,string comment, Guid executor)
        {
            QoAccessLog.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, executor, DateTime.Now, AccessTypeEnum.Error, string.Empty, QoAccessLog.BuildExceptionMessage(ex).ToString() + comment );
            WriteDebugLog(ex.StackTrace);
        }
        public static void WriteErrorLog(string comment, Guid executor)
        {
            QoAccessLog.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, executor, DateTime.Now, AccessTypeEnum.Error, string.Empty, comment );
        }

        /// <summary>
        /// デバッグモード時のみ、テキストログを出力します。
        /// </summary>
        /// <param name="message"></param>
        public static void WriteInfoLog(string message)
        {
            WriteDebugLog(message);
        }
        #endregion
    }

}