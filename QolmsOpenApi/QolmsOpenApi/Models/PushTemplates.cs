using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PushTemplates
    {
        /// <summary>
        /// 
        /// </summary>
        public class Generic
        {            
            /// <summary>
            /// Android FcmV1用
            /// </summary>
            public const string AndroidFcmV1 = "{\"message\":{\"notification\":{ \"title\" : \"$(title)\", \"body\" : \"$(alertMessage)\"}, \"data\" : { \"extra\" : \"$(extra)\", \"url\" : \"$(url)\", \"id\" : \"$(id)\" } } }";

            /// <summary>
            /// iOS用
            /// </summary>
            public const string iOS = "{ \"aps\" : { \"alert\" : { \"title\" : \"$(title)\", \"body\" : \"$(alertMessage)\" }, \"sound\": \"default\" $(badge) }, \"extra\" : $(extra), \"url\" : \"$(url)\" , \"id\" : \"$(id)\"}";

        }

        /// <summary>
        /// 
        /// </summary>
        public class Silent
        {
            /// <summary>
            /// Android FcmV1用
            /// </summary>
            public const string AndroidFcmV1 = "{\"message\":{ \"data\" : { \"extra\" : \"$(extra)\", \"url\" : \"$(url)\", \"id\" : \"$(id)\" } } }";


            /// <summary>
            /// iOS用
            /// </summary>
            public const string iOS = "{ \"aps\" : {\"content-available\" : 1 }, \"extra\" : $(extra), \"url\" : \"$(url)\", \"id\" : \"$(id)\" }";
        }
    }
}