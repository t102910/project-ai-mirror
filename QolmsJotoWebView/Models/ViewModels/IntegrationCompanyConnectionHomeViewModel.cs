using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationCompanyConnectionHomeViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public string LinkageSystemName { get; set; } = "企業";
    }
}
