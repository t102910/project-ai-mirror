using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationHospitalConnectionViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public string HospitalName { get; set; } = "城東区医師会病院";

        public string PatientNo { get; set; } = string.Empty;

        public bool MockConnectedFlag { get; set; } = false;

        public bool ExaminationConnectedFlag { get; set; } = true;
    }
}
