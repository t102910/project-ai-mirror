using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [DataContract()]
    [Serializable()]
    public class AuthTicketJsonParameter: QjJsonParameterBase
    {
        #region "Public Property"

        /// <summary>
        /// ユーザーID
        /// </summary>
        [DataMember()]
        public string UserId { get; set; } = string.Empty;

        #endregion

        #region "Constructor"
        public AuthTicketJsonParameter() : base() { }

        #endregion
    }
}