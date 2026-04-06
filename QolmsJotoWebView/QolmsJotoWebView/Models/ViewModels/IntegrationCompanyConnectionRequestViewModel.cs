using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携申請画面に渡すビューモデルです。
    /// </summary>
    [Serializable]
    public class IntegrationCompanyConnectionRequestViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public string LinkageSystemName { get; set; } = "企業";

        public string CompanyCode { get; set; } = string.Empty;

        public string EmployeeNo { get; set; } = string.Empty;

        public string FamilyName { get; set; } = string.Empty;

        public string GivenName { get; set; } = string.Empty;

        public string FamilyKanaName { get; set; } = string.Empty;

        public string GivenKanaName { get; set; } = string.Empty;

        public QjSexTypeEnum SexType { get; set; } = QjSexTypeEnum.None;

        public string SexLabel { get; set; } = string.Empty;

        public string BirthYear { get; set; } = string.Empty;

        public string BirthMonth { get; set; } = string.Empty;

        public string BirthDay { get; set; } = string.Empty;

        public string BirthDateLabel { get; set; } = string.Empty;

        public bool IsPremiumMember { get; set; } = false;

        public bool ShareBasicInfo { get; set; } = true;

        public bool ShareContactNotebook { get; set; } = true;

        public bool ShareVitalNotebook { get; set; } = true;

        public bool ShareMedicineNotebook { get; set; } = true;

        public bool ShareExaminationNotebook { get; set; } = true;
    }
}
