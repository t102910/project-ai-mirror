using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// アプリバージョン、アップデート指示を管理するjson
    /// </summary>
    public class VersionJsonResult
    {
        /// <summary>
        /// アップデート推奨
        /// </summary>
        public string app_recommended_version { get; set; }
        /// <summary>
        /// アップデート必須
        /// </summary>
        public string app_required_version { get; set; }
        /// <summary>
        /// 利用規約バージョン
        /// </summary>
        public string terms_version { get; set; }
        /// <summary>
        /// 許諾必須利用規約バージョン
        /// </summary>
        public string terms_agree_need_version { get; set; }
        /// <summary>
        /// アップデート推奨メッセージ
        /// </summary>
        public string recommended_message { get; set; }
        /// <summary>
        /// アップデート必須メッセージ
        /// </summary>
        public string required_message { get; set; }
        /// <summary>
        /// アップデート用のストアURL
        /// </summary>
        public string update_url { get; set; }
    }
}

