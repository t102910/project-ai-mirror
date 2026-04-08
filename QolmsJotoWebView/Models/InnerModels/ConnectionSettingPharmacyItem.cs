using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 薬局の連携設定情報を表します。
    /// </summary>
    [Serializable]
    public sealed class ConnectionSettingPharmacyItem
    {
        public int LinkageSystemNo { get; set; } = int.MinValue;

        public Guid FacilityKey { get; set; } = Guid.Empty;

        public string LinkageSystemName { get; set; } = string.Empty;

        public byte Status { get; set; } = byte.MinValue;
    }
}