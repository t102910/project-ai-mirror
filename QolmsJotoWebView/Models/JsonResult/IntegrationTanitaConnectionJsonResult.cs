using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{

    /// <summary>
    /// タニタ連携POSTの結果を保持する、
    /// JSON 形式のコンテンツを表します。
    /// このクラスは継承できません。
    /// </summary>
    [DataContract]
    [Serializable]
    public sealed class IntegrationTanitaConnectionJsonResult : QjJsonResultBase
    {
        #region Public Property

        /// <summary>
        /// エラーメッセージを取得または設定します。
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// デバイスリストを取得または設定します。
        /// </summary>
        [DataMember]
        public List<byte> Devises { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="IntegrationTanitaConnectionJsonResult" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public IntegrationTanitaConnectionJsonResult() : base()
        {
        }

        #endregion
    }

}