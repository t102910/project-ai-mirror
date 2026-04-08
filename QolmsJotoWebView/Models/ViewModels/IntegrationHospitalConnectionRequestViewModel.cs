using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 病院連携申請画面に渡すビューモデルです。
    /// </summary>
    [Serializable]
    public class IntegrationHospitalConnectionRequestViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public int LinkageSystemNo { get; set; } = int.MinValue;

        public bool IsHospitalSelectionFixed { get; set; } = false;

        public List<KeyValuePair<int, string>> HospitalList { get; } = new List<KeyValuePair<int, string>>();

        public string HospitalName { get; set; } = "城東区医師会病院";

        public string PatientNo { get; set; } = string.Empty;

        public string FamilyName { get; set; } = string.Empty;

        public string GivenName { get; set; } = string.Empty;

        public string FamilyKanaName { get; set; } = string.Empty;

        public string GivenKanaName { get; set; } = string.Empty;

        public QjSexTypeEnum SexType { get; set; } = QjSexTypeEnum.None;

        public string BirthYear { get; set; } = string.Empty;

        public string BirthMonth { get; set; } = string.Empty;

        public string BirthDay { get; set; } = string.Empty;

        public string BirthDateLabel { get; set; } = string.Empty;

        public string MailAddress { get; set; } = string.Empty;

        public bool IdentityUpdateFlag { get; set; } = false;

        public QjRelationContentTypeEnum RelationContentFlags { get; set; } = QjRelationContentTypeEnum.None;
    }
}
