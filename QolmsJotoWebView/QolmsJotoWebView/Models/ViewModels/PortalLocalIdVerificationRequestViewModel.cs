using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 市民確認申請画面のビュー モデルを表します。
    /// </summary>
    [Serializable]
    public class PortalLocalIdVerificationRequestViewModel
    {
        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; }

        /// <summary>
        /// 連携システムIDを取得または設定します。
        /// </summary>
        public string LinkageSystemId { get; set; }

        /// <summary>
        /// 連携システム名を取得または設定します。
        /// </summary>
        public string LinkageSystemName { get; set; }

        /// <summary>
        /// 申請ステータスを取得または設定します。
        /// </summary>
        public QjLinkageStatusTypeEnum Status { get; set; }

        /// <summary>
        /// 理由やメッセージを取得または設定します。
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 遷移先URLを取得または設定します。
        /// </summary>
        public string Url { get; set; }
    }
}