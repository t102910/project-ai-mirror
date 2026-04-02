using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationCompanyConnectionViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public int LinkageSystemNo { get; set; } = 47001;

        public string LinkageSystemName { get; set; } = "企業";

        public bool MockConnectedFlag { get; set; } = true;

        public bool ShareBasicInfo { get; set; } = true;

        public bool ShareContactNotebook { get; set; } = true;

        public bool ShareVitalNotebook { get; set; } = true;

        public bool ShareMedicineNotebook { get; set; } = true;

        public bool ShareExaminationNotebook { get; set; } = true;
    }
}
