using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// LineMessageWebhookAPIを実行した結果を格納する引数クラスを表します。 このクラスは継承できません。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class LineWebhookEventResults
    {
        #region "Public Property"

        ///// <summary>
        ///// チャネルシークレットhash値 を取得または設定します。
        ///// </summary>
        //[DataMember()]
        //public string XLineSignature { get; set; } = string.Empty;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LineWebhookEventResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LineWebhookEventResults() : base() { }

        #endregion
    }
}