using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// HAIP データ収集完了通知受信 APIを実行した結果を格納する引数クラスを表します。 このクラスは継承できません。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class HaipPhrReceiveNotificationResults
    {
        #region "Public Property"

        /// <summary>
        /// チャネルシークレットhash値 を取得または設定します。
        /// </summary>
        [DataMember(Name ="date_time")]
        public string datetime { get; set; } = string.Empty;

        /// <summary>
        /// チャネルシークレットhash値 を取得または設定します。
        /// </summary>
        [DataMember(Name = "request_id")]
        public string requestid { get; set; } = string.Empty;

        /// <summary>
        /// チャネルシークレットhash値 を取得または設定します。
        /// </summary>
        [DataMember()]
        public string error { get; set; } = string.Empty;

        /// <summary>
        /// チャネルシークレットhash値 を取得または設定します。
        /// </summary>
        [DataMember()]
        public string error_description { get; set; } = string.Empty;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="HaipPhrReceiveNotificationResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HaipPhrReceiveNotificationResults() : base() { }

        #endregion
    }
}