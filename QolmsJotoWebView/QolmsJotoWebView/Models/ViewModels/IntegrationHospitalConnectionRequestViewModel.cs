using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationHospitalConnectionRequestViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public int LinkageSystemNo { get; set; } = 0;

        public List<KeyValuePair<int, string>> HospitalList { get; set; } = new List<KeyValuePair<int, string>>();

        public string LinkageSystemId { get; set; } = string.Empty;

        public string FamilyName { get; set; } = string.Empty;

        public string GivenName { get; set; } = string.Empty;

        public string FamilyKanaName { get; set; } = string.Empty;

        public string GivenKanaName { get; set; } = string.Empty;

        public string SexLabel { get; set; } = string.Empty;

        public string BirthDateLabel { get; set; } = string.Empty;

        public string MailAddress { get; set; } = string.Empty;

        public bool ShareBasicInfo { get; set; } = true;

        public bool ShareVitalInfo { get; set; } = true;

        public bool ShareMedicineInfo { get; set; } = true;

        public bool ShareExaminationInfo { get; set; } = true;

        public bool ShareMealInfo { get; set; } = true;
    }
}
