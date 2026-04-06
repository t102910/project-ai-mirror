using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 連携設定情報を表します。
    /// </summary>
    [Serializable]
    public sealed class ConnectionSettingItem
    {
        public int LinkageNo { get; set; } = int.MinValue;

        public List<string> Devices { get; set; } = new List<string>();

        public List<string> Tags { get; set; } = new List<string>();

        public byte Status { get; set; } = byte.MinValue;
    }
}