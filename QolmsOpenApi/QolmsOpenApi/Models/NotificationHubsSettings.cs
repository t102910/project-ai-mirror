using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// Azure Notification Hubs の設定
    /// </summary>
    public class NotificationHubsSettings
    {
        /// <summary>
        /// 接続文字列
        /// </summary>
        public string HubConnectionString { get; set; }
        /// <summary>
        /// 名前
        /// </summary>
        public string HubName { get; set; }
    }
}