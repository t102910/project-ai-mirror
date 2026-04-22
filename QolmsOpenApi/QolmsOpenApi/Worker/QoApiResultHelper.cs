using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Enums;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    public sealed class QoApiResultHelper
    {
        private static readonly Dictionary<QoApiResultCodeTypeEnum, string> _messages = new Dictionary<QoApiResultCodeTypeEnum, string>(){
            {QoApiResultCodeTypeEnum.Success, "正常に終了しました。"},
            {QoApiResultCodeTypeEnum.ArgumentError, "APIの実行に失敗しました。"},
            {QoApiResultCodeTypeEnum.OperationError, "APIの実行に失敗しました。"},
            {QoApiResultCodeTypeEnum.DatabaseError, "APIの実行に失敗しました。"},
            {QoApiResultCodeTypeEnum.StorageError, "APIの実行に失敗しました。"},
            {QoApiResultCodeTypeEnum.SignUpMailAddressCountOver, "同じメールアドレスで規定回数以上の申請がありました。"},
            {QoApiResultCodeTypeEnum.SignUpGuidError, "仮アカウントキーの登録に失敗しました。"},
            {QoApiResultCodeTypeEnum.UserIdDuplicate, "このユーザーIDは既に使用されています。"},
            {QoApiResultCodeTypeEnum.AccountRegisterExpired, "本登録の有効期限切れです。"},
            {QoApiResultCodeTypeEnum.Maintenance, "メンテナンス中です。"},
            {QoApiResultCodeTypeEnum.UnknownError, "不明なエラーが発生しました。"},
            {QoApiResultCodeTypeEnum.UsedMailAddress, "このメールアドレスは既に使用されています。他のメールアドレスをご指定ください。"}
        };

        /// <summary>
        /// Exceptionを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoApiResultItem Build(Exception ex)
        {
            QoApiResultCodeTypeEnum code = QoApiResultCodeTypeEnum.UnknownError;

            switch (true) 
            {                
                case bool _ when ex.GetType() == typeof(ArgumentException):
                case bool _ when ex.GetType() == typeof(ArgumentNullException):
                case bool _ when ex.GetType() == typeof(ArgumentOutOfRangeException):
                    code = QoApiResultCodeTypeEnum.ArgumentError;
                    break;

                case bool _ when ex.GetType() == typeof(NullReferenceException):
                case bool _ when ex.GetType() == typeof(InvalidOperationException):
                case bool _ when ex.GetType() == typeof(InvalidCastException):
                case bool _ when ex.GetType() == typeof(FormatException):
                    code = QoApiResultCodeTypeEnum.OperationError;
                    break;

                case bool _ when ex.GetType() == typeof(System.Data.SqlClient.SqlException):
                    code = QoApiResultCodeTypeEnum.DatabaseError;
                    break;

                case bool _ when ex.GetType() == typeof(Microsoft.WindowsAzure.Storage.StorageException):
                    code = QoApiResultCodeTypeEnum.StorageError;
                    break;
                default:
                    code = QoApiResultCodeTypeEnum.InternalServerError;
                    break;
            }

            StringBuilder detail = new StringBuilder();
            detail.Append("APIの実行に失敗しました。");
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia */        /* TODO ERROR: Skipped EndIfDirectiveTrivia */                // エラーログ
            Task.Run(() => AccessLogWorker.WriteErrorLog(null/* TODO Change to default(_) if this is not a reference type */, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, string.Empty, ex));

            return new QoApiResultItem()
            {
                Code = Convert.ToInt32(code).ToString("d4"),
                Detail = detail.ToString()
            };
        }

        /// <summary>
        /// 結果コードを指定して、
        /// API処理結果クラスを生成します。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoApiResultItem Build(QoApiResultCodeTypeEnum code)
        {
            string message = string.Empty;

            if (QoApiResultHelper._messages.ContainsKey(code))
                message = QoApiResultHelper._messages[code];
            else
            {
                message = "APIの実行に失敗しました。";

                // 未定義のエラーコードを検知するためのログ
                Task.Run(() => AccessLogWorker.WriteAccessLog(null/* TODO Change to default(_) if this is not a reference type */, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, string.Format("メッセージ未定義のコード[QoApiResultCodeTypeEnum.{0} = {1}]が使用されています。", Enum.GetName(typeof(QoApiResultCodeTypeEnum), code), Convert.ToInt32(code))
        ));
            }

            return new QoApiResultItem()
            {
                Code = Convert.ToInt32(code).ToString("d4"),
                Detail = message
            };
        }

        [Obsolete("TODO；Enum定義共通化対象")]
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
        }

    }
}