using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    [DataContract]
    public class pointIf : QjJsonParameterBase
    {
        /// <summary>
        /// 電文制御情報
        /// </summary>
        [DataMember]
        public Control control { get; set; } = new Control();

        /// <summary>
        /// 電文データ情報  
        /// </summary>
        [DataMember]
        public ProcessResult processResult { get; set; } = new ProcessResult();
    }

}

