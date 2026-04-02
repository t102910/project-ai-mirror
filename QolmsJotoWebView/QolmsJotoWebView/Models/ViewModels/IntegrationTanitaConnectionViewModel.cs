using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationTanitaConnectionViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public string ConnectionId { get; set; } = string.Empty;

        public bool AlkooConnectedFlag { get; set; } = false;

        public bool BodyCompositionMeter { get; set; } = true;

        public bool Sphygmomanometer { get; set; } = true;

        public bool Pedometer { get; set; } = true;
    }
}
