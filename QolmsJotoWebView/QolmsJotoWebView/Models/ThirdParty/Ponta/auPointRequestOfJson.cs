using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public sealed class auPointRequestOfJson : QjJsonParameterBase
    {
        /// <summary>
        /// 加盟店ID
        /// </summary>
        [DataMember]
        public BPIFMerchantPoint_I BPIFMerchantPoint_I { get; set; } = new BPIFMerchantPoint_I();

        #region Constructor

        /// <summary>
        /// <see cref="auPointRequestOfJson" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public auPointRequestOfJson()
        {
        }

        #endregion
    }

}