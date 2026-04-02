using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public class PointInfo : QjJsonParameterBase
    {
        /// <summary>
        /// ポイント受付番号 
        /// </summary>
        [DataMember]
        public string pointReceiptNo { get; set; } = string.Empty;

        /// <summary>
        /// 処理日
        /// </summary>
        [DataMember]
        public string processDay { get; set; } = string.Empty;

        /// <summary>
        /// 処理時間
        /// </summary>
        [DataMember]
        public string processTime { get; set; } = string.Empty;

        /// <summary>
        /// 獲得発生年月日
        /// </summary>
        [DataMember]
        public string obtnHpnDay { get; set; } = string.Empty;

        /// <summary>
        /// 利用WALLETポイント発生年月日
        /// </summary>
        [DataMember]
        public string useAuIDHpnDay { get; set; } = string.Empty;

        /// <summary>
        /// 利用auポイント発生年月日
        /// </summary>
        [DataMember]
        public string useCmnHpnDay { get; set; } = string.Empty;
    }
}

