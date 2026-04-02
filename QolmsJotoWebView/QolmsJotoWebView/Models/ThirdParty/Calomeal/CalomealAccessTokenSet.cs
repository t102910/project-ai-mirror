using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// カロミルのトークンのセットを格納するクラスです。
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class CalomealAccessTokenSet
    {
        #region Public Property

        /// <summary>
        /// token_typeを取得または設定します。
        /// </summary>
        [DataMember]
        public string token_type { get; set; } = string.Empty;

        /// <summary>
        /// expires_in を取得または設定します。
        /// </summary>
        [DataMember]
        public string expires_in { get; set; } = string.Empty;

        /// <summary>
        /// トークン
        /// </summary>
        [DataMember]
        public string access_token { get; set; } = string.Empty;

        /// <summary>
        /// トークンの有効期限
        /// </summary>
        [DataMember]
        public DateTime TokenExpires { get; set; } = DateTime.MinValue;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        [DataMember]
        public string refresh_token { get; set; } = string.Empty;

        /// <summary>
        /// リフレッシュトークンの有効期限
        /// </summary>
        [DataMember]
        public DateTime RefreshTokenExpires { get; set; } = DateTime.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="CalomealAccessTokenSet" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CalomealAccessTokenSet()
        {
        }

        #endregion
    }
}