using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 市民画面の結果を保持する、
    /// JSON 形式のコンテンツを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    [DataContract]
    [Serializable]
    public sealed class MessageListJsonResult : QjJsonResultBase
    {
        #region Public Property

        /// <summary>
        /// 結果のリストを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public Dictionary<string, string> Messages { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PortalLocalIdVerificationJsonResult" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public MessageListJsonResult()
        {
        }

        #endregion
    }
}
