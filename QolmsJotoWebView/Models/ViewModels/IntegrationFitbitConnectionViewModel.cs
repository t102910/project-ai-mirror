using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「Fitbit連携」画面のモック表示用ビュー モデルです。
    /// API 連携前の画面移植で利用します。
    /// </summary>
    [Serializable]
    public class IntegrationFitbitConnectionViewModel
    {
        /// <summary>
        /// 遷移元の画面番号を取得または設定します。
        /// </summary>
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.PortalConnectionSetting;

        /// <summary>
        /// Fitbit の連携状態を取得または設定します。
        /// </summary>
        public bool FitbitConnectedFlag { get; set; } = false;
    }
}
