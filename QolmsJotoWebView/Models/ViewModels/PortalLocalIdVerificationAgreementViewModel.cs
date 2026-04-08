using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「規約同意」画面ビュー モデルを表します。
    /// </summary>
    [Serializable]
    public sealed class PortalLocalIdVerificationAgreementViewModel
    {
        /// <summary>
        /// 規約本文を取得または設定します。
        /// </summary>
        public string Terms { get; set; }
    }
}