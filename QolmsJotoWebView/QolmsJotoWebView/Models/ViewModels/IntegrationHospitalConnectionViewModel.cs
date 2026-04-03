using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public class IntegrationHospitalConnectionViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public int LinkageSystemNo { get; set; } = 0;

        public string HospitalName { get; set; } = "城東区医師会病院";

        /// <summary>1=連携承認待ち, 2=連携済み, 3=承認不可</summary>
        public int StatusType { get; set; } = 1;

        public string DisapprovedReason { get; set; } = string.Empty;

        public bool ShareBasicInfo { get; set; } = true;

        public bool ShareVitalInfo { get; set; } = true;

        public bool ShareMedicineInfo { get; set; } = true;

        public bool ShareExaminationInfo { get; set; } = true;

        public bool ShareMealInfo { get; set; } = true;
    }
}
