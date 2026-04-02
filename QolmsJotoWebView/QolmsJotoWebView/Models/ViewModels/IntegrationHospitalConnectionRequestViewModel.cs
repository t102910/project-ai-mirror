using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationHospitalConnectionRequestViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public string HospitalName { get; set; } = "城東区医師会病院";

        public string HospitalCode { get; set; } = string.Empty;

        public string PatientNo { get; set; } = string.Empty;

        public string FamilyName { get; set; } = "城東";

        public string GivenName { get; set; } = "太郎";

        public string BirthDateLabel { get; set; } = "1990年 1月 1日";

        public bool ConsentFlag { get; set; } = true;
    }
}
