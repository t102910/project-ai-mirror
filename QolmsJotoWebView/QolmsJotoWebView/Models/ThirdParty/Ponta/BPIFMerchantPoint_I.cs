using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public sealed class BPIFMerchantPoint_I : QjJsonParameterBase
    {
        /// <summary>
        /// 加盟店ID
        /// </summary>
        [DataMember]
        public string memberId { get; set; } = string.Empty;

        /// <summary>
        /// サービスID
        /// </summary>
        [DataMember]
        public string serviceId { get; set; } = string.Empty;

        /// <summary>
        /// セキュアキー 
        /// </summary>
        [DataMember]
        public string secureKey { get; set; } = string.Empty;

        /// <summary>
        /// 認証区分
        /// </summary>
        [DataMember]
        public string authKbn { get; set; } = string.Empty;

        /// <summary>
        /// openId
        /// </summary>
        [DataMember]
        public string openId { get; set; } = string.Empty;

        /// <summary>
        /// auId 
        /// </summary>
        [DataMember]
        public string auId { get; set; } = string.Empty;

        /// <summary>
        /// 加盟店依頼番号
        /// </summary>
        [DataMember]
        public string memberAskNo { get; set; } = string.Empty;

        /// <summary>
        /// dispKbn 
        /// </summary>
        [DataMember]
        public string dispKbn { get; set; } = string.Empty;

        /// <summary>
        /// 摘要
        /// </summary>
        [DataMember]
        public string commodity { get; set; } = string.Empty;

        /// <summary>
        /// useAuIdPoint 
        /// </summary>
        [DataMember]
        public string useAuIdPoint { get; set; } = string.Empty;

        /// <summary>
        /// useCmnPoint  
        /// </summary>
        [DataMember]
        public string useCmnPoint { get; set; } = string.Empty;

        /// <summary>
        /// obtnPoint  
        /// </summary>
        [DataMember]
        public string obtnPoint { get; set; } = string.Empty;

        /// <summary>
        /// tmpObtnKbn 
        /// </summary>
        [DataMember]
        public string tmpObtnKbn { get; set; } = string.Empty;

        /// <summary>
        /// pointObtnExpctDate 
        /// </summary>
        [DataMember]
        public string pointObtnExpctDate { get; set; } = string.Empty;

        /// <summary>
        /// pointEffTimlmtKbn 
        /// </summary>
        [DataMember]
        public string pointEffTimlmtKbn { get; set; } = string.Empty;

        /// <summary>
        /// obtnPointEffTimlmt  
        /// </summary>
        [DataMember]
        public string obtnPointEffTimlmt { get; set; } = string.Empty;

        /// <summary>
        /// obtnPointEffTimlmtKbn  
        /// </summary>
        [DataMember]
        public string obtnPointEffTimlmtKbn { get; set; } = string.Empty;

        #region Constructor

        /// <summary>
        /// <see cref="BPIFMerchantPoint_I" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public BPIFMerchantPoint_I()
        {
        }

        #endregion
    }

}