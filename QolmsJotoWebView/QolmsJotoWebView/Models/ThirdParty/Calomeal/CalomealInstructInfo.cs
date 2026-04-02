using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// カロミルのアドバイス同意情報を格納するクラスです。
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class CalomealInstructInfo
    {
        #region Public Property

        /// <summary>
        /// ユーザー名
        /// </summary>
        [DataMember]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 店名
        /// </summary>
        [DataMember]
        public string StoreName { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="CalomealInstructInfo" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CalomealInstructInfo()
        {
        }

        #endregion
    }
}