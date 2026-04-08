using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「地域 ポイント履歴」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class LocalHistoryViewModel : QjLocalPageViewModelBase
    {
        #region "Public Property"

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int FromPageNoType { get; set; } = int.MaxValue;

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MaxValue;

        /// <summary>
        /// 直近の有効期限を取得または設定します。
        /// </summary>
        public DateTime ClosestExprirationDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 直近の有効期限で失効するポイントを取得または設定します。
        /// </summary>
        public int ClosestExprirationPoint { get; set; } = int.MaxValue;

        /// <summary>
        /// 表示年を取得または設定します。
        /// </summary>
        public int Year { get; set; } = int.MaxValue;

        /// <summary>
        /// 表示月を取得または設定します。
        /// </summary>
        public int Month { get; set; } = int.MaxValue;

        /// <summary>
        /// au契約状態を取得または設定します。
        /// </summary>
        public bool IsMobileSubscriberOfAu { get; set; } = false;

        /// <summary>
        /// 日付毎の JOTO ポイント ログ情報のリストを取得または設定します。
        /// </summary>
        public List<JotoPointDailyLogItem> PointDailyLogN { get; set; } = new List<JotoPointDailyLogItem>();

        /// <summary>
        /// auIDかどうかを取得または設定します。
        /// </summary>
        public bool IsAuId { get; set; } = false;

        /// <summary>
        /// プレミアム会員かどうかを取得または設定します。
        /// </summary>
        public bool IsPremium { get; set; } = false;
        /// <summary>
        /// 法人連携かどうかを取得または設定します。
        /// </summary>
        public bool IsforBiz { get; set; } = false;
        /// <summary>
        /// 病院連携済みかどうかを取得または設定します。
        /// </summary>
        public bool IsConnectedHospital { get; set; } = false;

        /// <summary>
        /// 参加中のチャレンジのリストを取得または設定します。
        /// </summary>
        public Dictionary<Guid, string> ChallengeEntryList { get; set; } = new Dictionary<Guid, string>();

        #endregion

        #region "Constructor"

        public LocalHistoryViewModel(QolmsJotoModel mainModel) : base(mainModel, QjPageNoTypeEnum.PointLocalHistory) { }

        //検証用
        public LocalHistoryViewModel() : base() { }

        #endregion
    }
}
