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
    public sealed class OCCAuthApiArgsOfJson
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string AuthFlow = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string ClientId = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public OCCAuthApiArgsInnerOfJson AuthParameters;
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class OCCAuthApiArgsInnerOfJson
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string USERNAME = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string PASSWORD = string.Empty;


    }
}
