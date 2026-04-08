using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// POSTの結果 にメッセージ を保持する、
    /// JSON 形式のコンテンツを表します。
    /// このクラスは継承できません。
    /// </summary>
    [DataContract]
    [Serializable]
    public class MessageJsonResult : QjJsonResultBase
    {

        #region Public Property

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        [DataMember()]
        public string Message { get; set; } = string.Empty;

        #endregion

        #region Constructor

        public MessageJsonResult() : base() { }

        #endregion

        #region Public Method

        /// <summary>
        /// JsonResult クラスへ変換します。
        /// </summary>
        /// <param name="allowGet">クライアントからの HTTP GET 要求を許可 するかどうか</param>
        /// <returns>
        /// <see cref="JsonResult" /> クラス。
        /// </returns>
        public JsonResult ToJsonResult(bool allowGet)
        {
            // JsonResult へ変換
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(this.GetType());
                serializer.WriteObject(ms, this);

                if (allowGet)
                {
                    return new JsonResult
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        Data = Encoding.UTF8.GetString(ms.ToArray()),
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = int.MaxValue
                    };
                }
                else
                {
                    return new JsonResult
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        Data = Encoding.UTF8.GetString(ms.ToArray())
                    };
                }
            }
        }

        #endregion

    }
}