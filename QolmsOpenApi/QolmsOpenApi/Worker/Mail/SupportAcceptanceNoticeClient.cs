namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ユーザへの お問い合わせ受理メール を送信する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SupportAcceptanceNoticeClient : QoNoticeClientBase<SupportAcceptanceNoticeClientArgs>
    {
        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SupportAcceptanceNoticeClient() : base() {}

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="PersonalMessageNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public SupportAcceptanceNoticeClient(SupportAcceptanceNoticeClientArgs noticeArgs) : base(noticeArgs) {}
    }
}