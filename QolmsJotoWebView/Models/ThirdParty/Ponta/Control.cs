using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public class Control : QjJsonParameterBase
    {
        /// <summary>
        /// 処理結果コード 
        /// </summary>
        [DataMember]
        public string resultCd { get; set; } = string.Empty;
    }
}
