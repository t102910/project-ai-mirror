using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「地域ポイント履歴」画面の入力モデルを表します。
    /// </summary>
    [Serializable]
    public sealed class PointLocalHistoryInputModel : QjLocalPageViewModelBase, IValidatableObject
    {
        #region Constant

        private const string RANGE_ERROR_MESSAGE = "{0}が不正です。";

        #endregion

        #region Public Property

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
        [DisplayName("年")]
        public int Year { get; set; } = int.MaxValue;

        /// <summary>
        /// 表示月を取得または設定します。
        /// </summary>
        [DisplayName("月")]
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

        #region Constructor

        /// <summary>
        /// <see cref="PointLocalHistoryInputModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PointLocalHistoryInputModel() : base() { }

        /// <summary>
        /// メイン モデルを指定して、
        /// <see cref="PointLocalHistoryInputModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        public PointLocalHistoryInputModel(QolmsJotoModel mainModel)
            : base(mainModel, QjPageNoTypeEnum.PointLocalHistory)
        {
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 入力モデルの内容を現在のインスタンスに反映します。
        /// </summary>
        /// <param name="inputModel">入力モデル。</param>
        public void UpdateByInput(PointLocalHistoryInputModel inputModel)
        {
            if (inputModel == null)
            {
                return;
            }

            Year = inputModel.Year;
            Month = inputModel.Month;
        }

        #endregion

        #region IValidatableObject Support

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Year < 1 || Year > 9999)
            {
                result.Add(new ValidationResult(
                    string.Format(RANGE_ERROR_MESSAGE, "年"),
                    new[] { nameof(Year) }
                ));
            }

            if (Month < 1 || Month > 12)
            {
                result.Add(new ValidationResult(
                    string.Format(RANGE_ERROR_MESSAGE, "月"),
                    new[] { nameof(Month) }
                ));
            }

            return result;
        }

        #endregion
    }
}
