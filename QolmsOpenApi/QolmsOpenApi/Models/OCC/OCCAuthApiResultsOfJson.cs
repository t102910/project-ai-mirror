using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class OCCAuthApiResultsOfJson
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public OCCAuthApiResultsInnerOfJson AuthenticationResult;

        /// <summary>
        /// 
        /// </summary>
        public string Result = string.Empty;

    }
    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class OCCAuthApiResultsInnerOfJson
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string AccessToken = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string IdToken = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string RefreshToken = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public int ExpiresIn = int.MinValue;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string TokenType = string.Empty;

    }
}
