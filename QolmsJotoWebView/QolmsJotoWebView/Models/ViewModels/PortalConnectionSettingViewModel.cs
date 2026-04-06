using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「連携設定」画面ビュー モデルを表します。
    /// </summary>
    [Serializable]
    public sealed class PortalConnectionSettingViewModel : QjPageViewModelBase
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.None;

        public byte TabNoType { get; set; } = byte.MinValue;

        public Dictionary<int, ConnectionSettingItem> ConnectionSettingItems { get; } = new Dictionary<int, ConnectionSettingItem>();

        public Dictionary<int, ConnectionSettingHospitalItem> ConnectionSettingHospitalItemN { get; } = new Dictionary<int, ConnectionSettingHospitalItem>();

        public Dictionary<Guid, ConnectionSettingPharmacyItem> ConnectionSettingPharmacyItemN { get; } = new Dictionary<Guid, ConnectionSettingPharmacyItem>();

        public Dictionary<int, ConnectionSettingCompanyItem> ConnectionSettingCompanyItemN { get; } = new Dictionary<int, ConnectionSettingCompanyItem>();
    }
}