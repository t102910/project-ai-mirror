using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 病院連携詳細画面に渡すビューモデルです。
    /// </summary>
    [Serializable]
    public class IntegrationHospitalConnectionViewModel
    {
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        public int LinkageSystemNo { get; set; } = int.MinValue;

        public string HospitalName { get; set; } = "城東区医師会病院";

        public string PatientNo { get; set; } = string.Empty;

        public QjLinkageStatusTypeEnum StatusType { get; set; } = QjLinkageStatusTypeEnum.None;

        public string DisapprovedReason { get; set; } = string.Empty;

        public QjRelationContentTypeEnum ShowType { get; set; } = QjRelationContentTypeEnum.None;

        public bool HospitalConnectedFlag { get; set; } = false;

        public bool ExaminationConnectedFlag { get; set; } = true;
    }
}
