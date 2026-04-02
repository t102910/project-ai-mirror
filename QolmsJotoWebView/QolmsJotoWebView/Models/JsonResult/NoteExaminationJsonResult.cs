using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// データチャージPOSTの結果を保持する、
    /// JSON 形式のコンテンツを表します。
    /// このクラスは継承できません。
    /// </summary>
    [DataContract]
    [Serializable]
    public sealed class NoteExaminationJsonResult : QjJsonResultBase
    {
        #region Public Property

        /// <summary>
        /// エラーメッセージを取得または設定します。
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Messages { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PortalDatachargeJsonResult" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NoteExaminationJsonResult()
            : base()
        {
        }

        #endregion
    }
}