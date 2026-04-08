using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// カロミルのユーザー情報（JWT クレーム）を格納するクラスです。
    /// </summary>
    [DataContract]
    [Serializable]
    public sealed class CalomealUser : QjJsonParameterBase
    {
        /// <summary>
        /// Audience（対象者/クライアント ID）
        /// </summary>
        [DataMember]
        public string aud { get; set; }

        /// <summary>
        /// JWT ID（トークンの一意識別子）
        /// </summary>
        [DataMember]
        public string jti { get; set; }

        /// <summary>
        /// Issued At（発行時刻、UNIX タイムスタンプ）
        /// </summary>
        [DataMember]
        public int iat { get; set; }

        /// <summary>
        /// Not Before（有効開始時刻、UNIX タイムスタンプ）
        /// </summary>
        [DataMember]
        public int nbf { get; set; }

        /// <summary>
        /// Expiration Time（有効期限、UNIX タイムスタンプ）
        /// </summary>
        [DataMember]
        public int exp { get; set; }

        /// <summary>
        /// Subject（対象者/ユーザー ID）
        /// </summary>
        [DataMember]
        public string sub { get; set; }

        /// <summary>
        /// Scopes（権限スコープ一覧）
        /// </summary>
        [DataMember]
        public List<string> scopes { get; set; }

        /// <summary>
        /// <see cref="CalomealUser" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CalomealUser()
        {
            this.scopes = new List<string>();
        }
    }

    /*
     * JSON例：
     * {
     *     "aud": "test_client1",
     *     "jti": "0c5c0d362af8fee8e77f24fce8c52c248a6c11e74cbee9c9d386c5a8221a2df3fda400da4b8b64c9",
     *     "iat": 1688369315,
     *     "nbf": 1688369315,
     *     "exp": 1688455715,
     *     "sub": "3200",
     *     "scopes": ["all"]
     * }
     */

}