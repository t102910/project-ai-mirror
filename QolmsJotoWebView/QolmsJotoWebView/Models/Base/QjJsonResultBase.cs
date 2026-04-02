using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// JSON 形式の コンテンツ を応答に送信するための基本 クラス を表します。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public abstract class QjJsonResultBase
    {
        #region "Public Property"

        /// <summary>
        /// 処理結果を取得または設定します。
        /// </summary>
        [DataMember()]
        public string IsSuccess { get; set; } = bool.FalseString;

        #endregion

        #region "Constructor"

        protected QjJsonResultBase() : base() { }

        #endregion

        #region "Public Method"

        /// <summary>
        /// JsonResult クラス へ変換します。
        /// </summary>
        /// <returns></returns>
        public virtual System.Web.Mvc.JsonResult ToJsonResult()
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(this.GetType()).WriteObject(ms, this);
                return new System.Web.Mvc.JsonResult()
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    Data = Encoding.UTF8.GetString(ms.ToArray())

                };
            }
        }

        /// <summary>
        /// 必要に応じて プロパティ を サニタイズ して、 
        /// JsonResult クラス へ変換します。
        /// </summary>
        /// JsonResult クラス。</returns>
        public System.Web.Mvc.JsonResult ToJsonResultWithSanitize()
        {
            // コピー インスタンス で操作
            var result = this.Copy();

            //サニタイズ
            result.GetType().GetProperties().ToList().ForEach(
                p =>
                {
                    if (p.IsDefined(typeof(QjForceSanitizing), false))
                    {
                        if (p.PropertyType == typeof(String))
                        {
                            // String 型
                            string value = (string)p.GetValue(result);
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                p.SetValue(result, HttpUtility.HtmlEncode(value));
                            }
                        }
                        else
                        {
                            //' List(Of String) 型
                            List<string> value = ((List<string>)p.GetValue(result)).ConvertAll(i => string.IsNullOrWhiteSpace(i) ? string.Empty : HttpUtility.HtmlEncode(i));
                            if (value != null && value.Any())
                            {
                                p.SetValue(result, HttpUtility.HtmlEncode(value));
                            }
                        }
                    }
                }
            );

            using (var ms = new MemoryStream()) 
            {
                new DataContractJsonSerializer(result.GetType()).WriteObject(ms, result);

                return new System.Web.Mvc.JsonResult()
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    Data = Encoding.UTF8.GetString(ms.ToArray())
                };
            }
        }

        #endregion
    }
}