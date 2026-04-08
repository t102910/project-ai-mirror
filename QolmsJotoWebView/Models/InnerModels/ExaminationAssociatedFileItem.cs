using System;
using System.Runtime.Serialization;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 検査グループ情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [DataContract]
    [Serializable]
    public sealed class ExaminationAssociatedFileItem
    {
        #region Public Property

        /// <summary>
        /// 連携 システム 番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 連携 システム ID を取得または設定します。
        /// </summary>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// 検査日を取得または設定します。
        /// </summary>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 施設 キー を取得または設定します。
        /// </summary>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// データタイプ を取得または設定します。
        /// </summary>
        public byte DataType { get; set; } = byte.MinValue;

        /// <summary>
        /// データキー を取得または設定します。
        /// </summary>
        public Guid DataKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 追加 キー を取得または設定します。
        /// </summary>
        public string AdditionalKey { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationAssociatedFileItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ExaminationAssociatedFileItem()
        {
        }

        #endregion
    }
}
