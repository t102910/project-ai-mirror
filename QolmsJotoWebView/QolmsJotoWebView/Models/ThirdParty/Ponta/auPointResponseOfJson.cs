using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public sealed class auPointResponseOfJson : QjJsonParameterBase
    {
        /// <summary>
        /// 加盟店ID
        /// </summary>
        [DataMember]
        public pointIf pointIf { get; set; } = new pointIf();

        #region Constructor

        /// <summary>
        /// <see cref="auPointResponseOfJson" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public auPointResponseOfJson()
        {
            // base.New() は C# では明示的に呼ぶ必要がない場合が多いですが、
            // 必要に応じて base(); と書くことができます
        }

        #endregion
    }

}
