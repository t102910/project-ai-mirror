using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 画面ビュー内に展開するするための、
    /// JSON 形式のパラメータを保持するための基本クラスを表します。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public abstract class QjJsonParameterBase
    {
        #region "Constructor"
        public QjJsonParameterBase() : base() { }

        #endregion

        #region "Public Method"

        /// <summary>
        /// パラメータ クラスを、
        /// JSON 形式の文字列へシリアル化します。
        /// </summary>
        /// <typeparam name="T">シリアル化するパラメータ クラスの型。</typeparam>
        /// <param name="value">シリアル化するパラメータ クラス。</param>
        /// <returns>
        /// JSON形式の文字列。
        /// </returns>
        public static string ToJsonString<T>(T value) where T : QjJsonParameterBase
        {
            using (var ms =new MemoryStream())
            {
                new DataContractJsonSerializer(value.GetType()).WriteObject(ms, value);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// JSON 形式の文字列を、
        /// パラメータ クラスへ逆シリアル化します。
        /// </summary>
        /// <typeparam name="T">逆シリアル化するパラメータ クラスの型。</typeparam>
        /// <param name="value">逆シリアル化する JSON 形式の文字列。</param>
        /// <returns>
        /// パラメータ クラスのインスタンス。
        /// </returns>
        public static T FromJsonString<T>(string value) where T : QjJsonParameterBase
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value));
                ms.Position = 0;

                return (T) new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        /// <summary>
        /// JSON 形式の文字列へシリアル化します。
        /// </summary>
        /// <returns>
        /// JSON 形式の文字列。
        /// </returns>
        public string ToJsonString()
        {
            return QjJsonParameterBase.ToJsonString(this);
        }

        #endregion
    }
}