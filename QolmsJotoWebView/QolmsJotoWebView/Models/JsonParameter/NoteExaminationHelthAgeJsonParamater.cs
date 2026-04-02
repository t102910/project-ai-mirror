using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [DataContract]
    [Serializable]
    public sealed class NoteExaminationHelthAgeJsonParamater : QjJsonParameterBase
    {
        #region "Public Property"

        [DataMember]
        public Guid Accountkey { get; set; } = Guid.Empty;

        [DataMember]
        public DateTime LoginAt { get; set; } = DateTime.MinValue;

        [DataMember]
        public Dictionary<DateTime, Dictionary<string, string>> healthAgeCalcN { get; set; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="NoteExaminationHelthAgeJsonParamater" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NoteExaminationHelthAgeJsonParamater()
            : base()
        {
        }

        #endregion
    }

}