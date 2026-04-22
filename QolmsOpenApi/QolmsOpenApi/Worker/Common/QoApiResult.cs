using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 結果コードの返却値作成ヘルパー
    /// </summary>
    public class QoApiResult
    {
        /// <summary>
        /// メッセージ
        /// </summary>
        private static readonly Dictionary<QoApiResultCodeTypeEnum, string> _messages = new Dictionary<QoApiResultCodeTypeEnum, string>()
        {
            {
                QoApiResultCodeTypeEnum.Success,
                "正常に終了しました。"
            },
            {
                QoApiResultCodeTypeEnum.ArgumentError,
                "APIの実行に失敗しました。"
            },
            {
                QoApiResultCodeTypeEnum.OperationError,
                "APIの実行に失敗しました。"
            },
            {
                QoApiResultCodeTypeEnum.DatabaseError,
                "APIの実行に失敗しました。"
            },
            {
                QoApiResultCodeTypeEnum.StorageError,
                "APIの実行に失敗しました。"
            },
            {
                QoApiResultCodeTypeEnum.SignUpMailAddressCountOver,
                "同じメールアドレスで規定回数以上の申請がありました。"
            },
            {
                QoApiResultCodeTypeEnum.SignUpGuidError,
                "仮アカウントキーの登録に失敗しました。"
            },
            {
                QoApiResultCodeTypeEnum.UserIdDuplicate,
                "このユーザーIDは既に使用されています。"
            },
            {
                QoApiResultCodeTypeEnum.AccountRegisterExpired,
                "本登録の有効期限切れです。"
            },
            {
                QoApiResultCodeTypeEnum.Maintenance,
                "メンテナンス中です。"
            },
            {
                QoApiResultCodeTypeEnum.UnknownError,
                "不明なエラーが発生しました。"
            },
            {
                QoApiResultCodeTypeEnum.UsedMailAddress,
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
        public static QoApiResultItem Build(Exception ex, string message = "")
        {
            QoApiResultCodeTypeEnum code = QoApiResultCodeTypeEnum.UnknownError;

            if(ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                code = QoApiResultCodeTypeEnum.ArgumentError;
            }
            else if(ex is NullReferenceException || ex is InvalidOperationException || ex is InvalidCastException || ex is FormatException)
            {
                code = QoApiResultCodeTypeEnum.OperationError;
            }
            else if( ex is System.Data.SqlClient.SqlException)
            {
                code = QoApiResultCodeTypeEnum.DatabaseError;
            }
            else if( ex is Microsoft.WindowsAzure.Storage.StorageException)
            {
                code = QoApiResultCodeTypeEnum.StorageError;
            }
            else
            {
                code = QoApiResultCodeTypeEnum.InternalServerError;
            }
            
            StringBuilder detail = new StringBuilder();
            detail.Append("APIの実行に失敗しました。");
            detail.Append(message);
            // エラーログ
            Task.Run(() =>
            {
                QoAccessLog.WriteErrorLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, string.Empty, ex);                
            });

            QoAccessLog.WriteInfoLog(ex.Message);

            return new QoApiResultItem() { Code = Convert.ToInt32(code).ToString("d4"), Detail = detail.ToString() };
        }

        

        /// <summary>
        /// 結果コードを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoApiResultItem Build(QoApiResultCodeTypeEnum code, string detail="")
        {
            string message = string.Empty;

            if (_messages.ContainsKey(code))
                message = _messages[code];
            else
            {
                message = "APIの実行に失敗しました。";

                // 未定義のエラーコードを検知するためのログ
                Task.Run(
                    () => QoAccessLog.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, string.Empty, string.Format("メッセージ未定義のコード[QoApiResultCodeTypeEnum.{0} = {1}]が使用されています。", Enum.GetName(typeof(QoApiResultCodeTypeEnum), code), Convert.ToInt32(code))    )
                    );
            }

            return new QoApiResultItem() { Code = Convert.ToInt32(code).ToString("d4"), Detail = message + detail};
        }

        /// <summary>
        /// 結果コードとエラーログメッセージを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static QoApiResultItem Build(QoApiResultCodeTypeEnum code, string error, string detail = "")
        {
            // エラーログ
            Task.Run(() =>
            {
                QoAccessLog.WriteErrorLog(error, Guid.Empty);
            });
            return Build(code, detail);
        }

        /*[Obsolete("TODO；Enum定義共通化対象")]
        public static QoApiResultItem Build(string detail)
        {
            return new QoApiResultItem() { Code = Convert.ToInt32(QoApiResultCodeTypeEnum.GeneralError).ToString("d4"), Detail = detail };
        }

        [Obsolete("TODO；メッセージ定義共通化対象")]
        public static QoApiResultItem Build(QoApiResultCodeTypeEnum code, string detail)
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