using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 病院の連携設定情報を表します。
    /// </summary>
    [Serializable]
    public sealed class ConnectionSettingHospitalItem
    {
        public int LinkageSystemNo { get; set; } = int.MinValue;

        public string LinkageSystemName { get; set; } = string.Empty;

        public byte Status { get; set; } = byte.MinValue;
    }
}