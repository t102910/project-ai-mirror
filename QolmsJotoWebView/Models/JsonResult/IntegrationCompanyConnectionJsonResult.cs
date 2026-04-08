using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携系POSTの共通レスポンスを表します。
    /// </summary>
    [DataContract]
    [Serializable]
    public sealed class IntegrationCompanyConnectionJsonResult : QjJsonResultBase
    {
        /// <summary>
        /// 入力エラーや業務エラーのメッセージを保持します。
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Messages { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 連携対象のシステム番号を保持します。
        /// </summary>
        [DataMember]
        public string LinkageSystemNo { get; set; } = string.Empty;
    }
}