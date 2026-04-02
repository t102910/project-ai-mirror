using System;
using System.Collections.Generic;


namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// QOLMSポイント付与情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class QolmsPointGrantItem
    {
        // 付与ポイントの配列です。サイト側から固定で付与ポイントが決まっている場合はここに追加してください。
        // （通常会員のポイントを記載。減算は0。プレミアム２倍は_pointDoubleItemにQjPointItemTypeEnumのアイテムを追加してください。）
        public static readonly int[] PointValues = { 0, 500, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 0, 0, 100, 0, 0, 0, 0, 1000, 1 };

        private List<QjPointItemTypeEnum> PointDoubleItem
        {
            get
            {
                // 付与ポイント２倍のポイント種別を追加した場合はここも追加してください。
                return new List<QjPointItemTypeEnum>()
            {
                QjPointItemTypeEnum.Login,
                QjPointItemTypeEnum.Walk5k,
                QjPointItemTypeEnum.Walk6k,
                QjPointItemTypeEnum.Walk7k,
                QjPointItemTypeEnum.Walk8k,
                QjPointItemTypeEnum.Walk9k,
                QjPointItemTypeEnum.Walk10k,
                QjPointItemTypeEnum.Exercise,
                QjPointItemTypeEnum.Breakfast,
                QjPointItemTypeEnum.Lunch,
                QjPointItemTypeEnum.Dinner,
                QjPointItemTypeEnum.Snack,
                QjPointItemTypeEnum.Vital,
                QjPointItemTypeEnum.Examination,
                QjPointItemTypeEnum.TanitaConnection,
                QjPointItemTypeEnum.Meal
            };
            }
        }

        #region "Public Property"

        /// <summary>
        /// 付与日を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// シリアル番号を取得または設定します。
        /// </summary>
        /// <remarks>呼び出しシステム内で一意になる40文字以内の文字列</remarks>
        public string SerialCode { get; set; } = string.Empty;

        /// <summary>
        /// ポイント項目番号（ポイント項目マスタに登録されているもの）を取得または設定します。
        /// </summary>
        public int PointItemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 付与ポイント数を取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// ポイント対象日を取得または設定します。
        /// </summary>
        public DateTime PointTargetDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ポイント有効期限を取得または設定します。
        /// </summary>
        /// <remarks>DateTime型にしていますが時間は無視されます</remarks>
        public DateTime PointExpirationDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 付与理由を取得または設定します。
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="QolmsPointGrantItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public QolmsPointGrantItem()
        {
        }

        public QolmsPointGrantItem(
            QjMemberShipTypeEnum memberShipLevel,
            DateTime actionDate,
            string serialCode,
            QjPointItemTypeEnum pointItemType,
            DateTime pointExpirationDate,
            string reason = "")
            : this(memberShipLevel, actionDate, serialCode, pointItemType, pointExpirationDate, actionDate, reason)
        {
        }

        public QolmsPointGrantItem(
            QjMemberShipTypeEnum memberShipLevel,
            DateTime actionDate,
            string serialCode,
            QjPointItemTypeEnum pointItemType,
            DateTime pointExpirationDate,
            DateTime pointTargetDate,
            string reason = "")
        {
            this.ActionDate = actionDate;
            this.SerialCode = serialCode;
            this.PointItemNo = (int)pointItemType;

            switch (memberShipLevel)
            {
                case QjMemberShipTypeEnum.LimitedTime:
                case QjMemberShipTypeEnum.Premium:
                case QjMemberShipTypeEnum.Business:
                    // プレミアム会員であれば500ポイントの以外は2倍ポイント
                    if (PointDoubleItem.IndexOf(pointItemType) >= 0)
                    {
                        this.Point = PointValues[this.PointItemNo] * 2;
                    }
                    else
                    {
                        this.Point = PointValues[this.PointItemNo];
                    }
                    break;

                default:
                    this.Point = PointValues[this.PointItemNo];
                    break;
            }

            this.PointExpirationDate = pointExpirationDate;
            this.PointTargetDate = pointTargetDate;
            this.Reason = reason;
        }

        #endregion
    }
}
