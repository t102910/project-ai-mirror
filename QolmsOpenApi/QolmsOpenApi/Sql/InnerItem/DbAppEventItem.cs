using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// アプリイベント情報を表します。
    /// </summary>
    public sealed class DbAppEventItem
    {
        /// <summary>
        /// イベントキーを取得または設定します。
        /// </summary>
        public Guid EventKey { get; set; } = Guid.Empty;

        /// <summary>
        /// アプリケーション種別を取得または設定します。
        /// </summary>
        public byte AppType { get; set; } = byte.MinValue;

        /// <summary>
        /// 公開用イベントコードを取得または設定します。
        /// </summary>
        public string EventCode { get; set; } = string.Empty;

        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// タイトルを取得または設定します。
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 本文を取得または設定します。
        /// </summary>
        public string Contents { get; set; } = string.Empty;

        /// <summary>
        /// イベント開始日時を取得または設定します。
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// イベント終了日時を取得または設定します。
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// イベント掲載開始日時を取得または設定します。
        /// </summary>
        public DateTime PublishStartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// イベント掲載終了日時を取得または設定します。
        /// </summary>
        public DateTime PublishEndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// イベント追加情報セット(JSON文字列)を取得または設定します。
        /// </summary>
        public string AppEventSet { get; set; } = string.Empty;
    }
}
