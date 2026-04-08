using MGF.QOLMS.QolmsApiCoreV1;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class QjApiResult
    {
        /// <summary>
        /// メッセージ
        /// </summary>
        private static readonly Dictionary<QjApiResultCodeTypeEnum, string> _messages = new Dictionary<QjApiResultCodeTypeEnum, string>()
        {
            {
                QjApiResultCodeTypeEnum.Success,
                "正常に終了しました。"
            },
            {
                QjApiResultCodeTypeEnum.ArgumentError,
                "APIの実行に失敗しました。"
            },
            {
                QjApiResultCodeTypeEnum.OperationError,
                "APIの実行に失敗しました。"
            },
            {
                QjApiResultCodeTypeEnum.DatabaseError,
                "APIの実行に失敗しました。"
            },
            {
                QjApiResultCodeTypeEnum.StorageError,
                "APIの実行に失敗しました。"
            },
            {
                QjApiResultCodeTypeEnum.SignUpMailAddressCountOver,
                "同じメールアドレスで規定回数以上の申請がありました。"
            },
            {
                QjApiResultCodeTypeEnum.SignUpGuidError,
                "仮アカウントキーの登録に失敗しました。"
            },
            {
                QjApiResultCodeTypeEnum.UserIdDuplicate,
                "このユーザーIDは既に使用されています。"
            },
            {
                QjApiResultCodeTypeEnum.AccountRegisterExpired,
                "本登録の有効期限切れです。"
            },
            {
                QjApiResultCodeTypeEnum.Maintenance,
                "メンテナンス中です。"
            },
            {
                QjApiResultCodeTypeEnum.UnknownError,
                "不明なエラーが発生しました。"
            },
            {
                QjApiResultCodeTypeEnum.UsedMailAddress,
                "このメールアドレスは既に使用されています。他のメールアドレスをご指定ください。"
            }
        };

        /// <summary>
        /// Exceptionを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QjApiResultItem Build(Exception ex, string message = "")
        {
            QjApiResultCodeTypeEnum code = QjApiResultCodeTypeEnum.UnknownError;

            if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                code = QjApiResultCodeTypeEnum.ArgumentError;
            }
            else if (ex is NullReferenceException || ex is InvalidOperationException || ex is InvalidCastException || ex is FormatException)
            {
                code = QjApiResultCodeTypeEnum.OperationError;
            }
            else if (ex is SqlException)
            {
                code = QjApiResultCodeTypeEnum.DatabaseError;
            }
            else if (ex is StorageException)
            {
                code = QjApiResultCodeTypeEnum.StorageError;
            }
            else
            {
                code = QjApiResultCodeTypeEnum.InternalServerError;
            }

            StringBuilder detail = new StringBuilder();
            detail.Append("APIの実行に失敗しました。");
            detail.Append(message);
            // エラーログ
            Task.Run(() =>
            {
                AccessLogWorker.WriteErrorLog(null, string.Empty, ex);
            });

            //AccessLogWorker.WriteInfoLog(ex.Message);
            return new QjApiResultItem() { Code = Convert.ToInt32(code).ToString("d4"), Detail = detail.ToString() };
        }

        /// <summary>
        /// 結果コードを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QjApiResultItem Build(QjApiResultCodeTypeEnum code, string detail = "")
        {
            string message = string.Empty;

            if (_messages.ContainsKey(code))
                message = _messages[code];
            else
            {
                message = "APIの実行に失敗しました。";

                // 未定義のエラーコードを検知するためのログ
                Task.Run(
                    () => AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.Api, string.Format("メッセージ未定義のコード[QjApiResultCodeTypeEnum.{0} = {1}]が使用されています。", Enum.GetName(typeof(QjApiResultCodeTypeEnum), code), Convert.ToInt32(code)))
                    );
            }

            return new QjApiResultItem() { Code = Convert.ToInt32(code).ToString("d4"), Detail = message + detail };
        }

        /// <summary>
        /// 結果コードとエラーログメッセージを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static QjApiResultItem Build(QjApiResultCodeTypeEnum code, string error, string detail = "")
        {
            // エラーログ
            Task.Run(() =>
            {
                AccessLogWorker.WriteErrorLog(null,string.Empty, error);
            });
            return Build(code, detail);
        }

        /*[Obsolete("TODO；Enum定義共通化対象")]
        public static QoApiResultItem Build(string detail)
        {
            return new QoApiResultItem() { Code = Convert.ToInt32(QjApiResultCodeTypeEnum.GeneralError).ToString("d4"), Detail = detail };
        }

        [Obsolete("TODO；メッセージ定義共通化対象")]
        public static QoApiResultItem Build(QjApiResultCodeTypeEnum code, string detail)
        {
            return new QoApiResultItem() { Code = Convert.ToInt32(code).ToString("d4"), Detail = detail };
        }

        [Obsolete("TODO；Enum、メッセージ定義共通化対象")]
        public static QoApiResultItem Build(int code, string detail)
        {
            return new QoApiResultItem() { Code = code.ToString("d4"), Detail = detail };
        }*/
    }
}