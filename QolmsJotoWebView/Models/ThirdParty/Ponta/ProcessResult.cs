using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public class ProcessResult : QjJsonParameterBase
    {
        /// <summary>
        /// ポイント返却情報
        /// </summary>
        [DataMember]
        public PointInfo pointInfo { get; set; } = new PointInfo();
    }

}