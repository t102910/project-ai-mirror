using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// Smapa API の共通引数
    /// </summary>
    public class SmapaApiArgs
    {
        /// <summary>
        /// 日別の連番
        /// </summary>
        [JsonProperty("tran_no")]
        public int TranNo { get; set; }

        /// <summary>
        /// 現在日時をYYYYMMDDhhmmssで
        /// </summary>
        [JsonProperty("date_time")]
        public string DateTime { get; set; }

        /// <summary>
        /// 病院コード
        /// </summary>
        [JsonProperty("hosp_code")]
        public string HospCode { get; set; }

        /// <summary>
        /// 患者ID
        /// </summary>
        [JsonProperty("localid")]
        public string LocalId { get; set; }
    }
}