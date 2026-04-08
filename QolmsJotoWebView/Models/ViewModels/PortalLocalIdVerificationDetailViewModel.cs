using MGF.QOLMS.QolmsJotoWebView.Models;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 市民確認申請詳細画面のビュー モデルを表します。
    /// </summary>
    [Serializable]
    public class PortalLocalIdVerificationDetailViewModel
    {
        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; }

        /// <summary>
        /// 連携システム名を取得または設定します。
        /// </summary>
        public string LinkageSystemName { get; set; }

        /// <summary>
        /// 申請ステータスを取得または設定します。
        /// </summary>
        public QjLinkageStatusTypeEnum Status { get; set; }

        /// <summary>
        /// 非承認理由などのメッセージを取得または設定します。
        /// </summary>
        public string Reason { get; set; }
    }
}