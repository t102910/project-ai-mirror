using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// LineMessageWebhookAPIを実行するための情報を格納する引数クラスを表します。 このクラスは継承できません。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class LineWebhookEventArgs
    {
        #region "Public Property"

        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        public string LinkageSystemNo { get; set; } = string.Empty;

        /// <summary>
        /// チャネルシークレットhash値 を取得または設定します。
        /// </summary>
        public string XLineSignature { get; set; } = string.Empty;

        /// <summary>
        /// 検証用Body を取得または設定します。
        /// </summary>
        public string body { get; set; } = string.Empty;

        /// <summary>
        /// destination を取得または設定します。
        /// </summary>
        [DataMember()]
        public string destination { get; set; } = string.Empty;

        /// <summary>
        /// events を取得または設定します。
        /// </summary>
        [DataMember()]
        public List<QoApiLineMessageEventItem> events { get; set; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LineWebhookEventArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LineWebhookEventArgs() : base() { }

        #endregion
    }
}