using System;
using System.Collections.Generic;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// グループへのお知らせを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class DbNoticeGroupItem
    {
        /// <summary>
        /// お知らせ番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public long NoticeNo { get; set; } = long.MinValue;

        /// <summary>
        /// タイトルを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 内容を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Contents { get; set; } = string.Empty;

        /// <summary>
        /// お知らせカテゴリを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte CategoryNo { get; set; } = byte.MinValue;

        /// <summary>
        /// 優先度を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte PriorityNo { get; set; } = byte.MinValue;

        /// <summary>
        /// 対象タイプを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte TargetType { get; set; } = byte.MinValue;

        /// <summary>
        /// 開始日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 終了日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// お知らせデータセットを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string NoticeDataSet { get; set; } = string.Empty;

        /// <summary>
        /// 既読フラグを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool AlreadyReadFlag { get; set; } = false;

        /// <summary>
        /// 既読日時を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime AlreadyReadDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// <see cref="DbNoticeGroupItem" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbNoticeGroupItem()
        {
        }
    }
}
