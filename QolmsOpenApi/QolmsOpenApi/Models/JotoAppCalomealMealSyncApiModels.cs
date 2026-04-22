using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsApiEntityV1
{
    [DataContract]
    [Serializable]
    public class QoJotoAppCalomealMealSyncApiArgs : QoApiArgsBase
    {
        [DataMember(Name = "TargetDateTime")]
        public string TargetDateTime { get; set; }

        [DataMember(Name = "TimeSpanInHours")]
        public string TimeSpanInHours { get; set; }
    }

    [DataContract]
    [Serializable]
    public class QoJotoAppCalomealMealSyncApiResults : QoApiResultsBase
    {
        [DataMember(Name = "TargetDateTime")]
        public string TargetDateTime { get; set; }

        [DataMember(Name = "TimeSpanInHours")]
        public string TimeSpanInHours { get; set; }

        [DataMember(Name = "ProcessedCount")]
        public string ProcessedCount { get; set; }

        [DataMember(Name = "SuccessCount")]
        public string SuccessCount { get; set; }

        [DataMember(Name = "ErrorCount")]
        public string ErrorCount { get; set; }

        [DataMember(Name = "AddedCount")]
        public string AddedCount { get; set; }

        [DataMember(Name = "ModifiedCount")]
        public string ModifiedCount { get; set; }

        [DataMember(Name = "DeletedCount")]
        public string DeletedCount { get; set; }

        [DataMember(Name = "TokenRefreshed")]
        public string TokenRefreshed { get; set; }

        [DataMember(Name = "Message")]
        public string Message { get; set; }

        [DataMember(Name = "ErrorMessageN")]
        public List<string> ErrorMessageN { get; set; } = new List<string>();
    }
}