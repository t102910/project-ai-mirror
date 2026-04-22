using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{   
    /// <summary>
    /// Smapa API の共通戻り値
    /// </summary>
    public class SmapaApiResults
    {
        /// <summary>
        /// エラー 0:正常 1:エラー
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// エラー種別 
        /// </summary>
        [JsonProperty("error_flag")]
        public string ErrorFlag { get; set; } = string.Empty;

        /// <summary>
        /// HomeのURL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// エラー詳細 Jsonには含まれない。
        /// </summary>
        public string ErrorDetail { get; set; } = string.Empty;
    }
}