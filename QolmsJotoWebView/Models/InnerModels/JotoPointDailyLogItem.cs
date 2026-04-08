using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{

    /// <summary>
    /// 日付毎の JOTO ポイント ログ情報を表します。
    /// このクラスは継承できません。
    public sealed class JotoPointDailyLogItem
    {
        #region "Public Property"

        /// <summary>
        /// 操作日を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 日付内の JOTO ポイント ログ情報のリストを取得または設定します。
        /// </summary>
        public List<JotoPointLogItem> PointLogN { get; set; } = new List<JotoPointLogItem>();

        /// <summary>
        /// 日付内の ポイント合計を取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        #endregion

        #region "Constructor"

        public JotoPointDailyLogItem() : base() { }

        #endregion

    }
}