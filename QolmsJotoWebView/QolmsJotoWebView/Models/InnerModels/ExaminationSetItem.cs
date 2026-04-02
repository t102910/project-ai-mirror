using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「コルムス サイト」が使用する、
    /// 日付および日付内連番ごとの検査結果情報を表します。
    /// この クラス は継承できません。
    /// </summary>
    [Serializable]
    public sealed class ExaminationSetItem
    {
        #region Public Property

        /// <summary>
        /// キャッシュ キー を取得または設定します。
        /// この値は、健診 CDA ファイル 名になり、暗号化して ビュー に展開します。
        /// </summary>
        public string CacheKey { get; set; } = string.Empty;

        /// <summary>
        /// 検査分類IDを取得または設定します。
        /// </summary>
        public int CategoryId { get; set; } = int.MinValue;

        /// <summary>
        /// 検査日を取得または設定します。
        /// </summary>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 日付内連番を取得または設定します。
        /// </summary>
        public int Sequence { get; set; } = int.MinValue;

        /// <summary>
        /// 連携先番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 施設 キー を取得または設定します。
        /// </summary>
        public string OrganizationKey { get; set; } = string.Empty;

        /// <summary>
        /// 施設名を取得または設定します。
        /// </summary>
        [DataMember]
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// 施設電話番号を取得または設定します。
        /// </summary>
        [DataMember]
        public string OrganizationTel { get; set; } = string.Empty;

        /// <summary>
        /// 検査結果情報の リスト を取得または設定します。
        /// </summary>
        [DataMember]
        public List<ExaminationItem> ExaminationN { get; set; } = new List<ExaminationItem>();

        /// <summary>
        /// 総合所見 PDF ファイル を格納する ブロブ キー の リスト を取得または設定します。
        /// </summary>
        [Obsolete("実装中")]
        public List<Guid> OverallAssessmentPdfKeyN { get; set; } = new List<Guid>();

        /// <summary>
        /// DICOM健診画像アクセスキー を格納する ブロブ キー の リスト を取得または設定します。
        /// </summary>
        [Obsolete("実装中")]
        public List<string> DicomUrlAccessKeyN { get; set; } = new List<string>();

        /// <summary>
        /// 健康年齢を取得または設定します（JOTO 用）。
        /// </summary>
        [DataMember]
        public int HealthAge { get; set; } = int.MinValue;

        /// <summary>
        /// 総合所見 CSV ファイル から取得した、
        /// 検査所見・判定情報の リスト を取得または設定します（JOTO 用）。
        /// </summary>
        [DataMember]
        public List<ExaminationJudgementItem> ExaminationJudgementN { get; set; } = new List<ExaminationJudgementItem>();

        /// <summary>
        /// 総合所見 PDF ファイル を格納する ブロブ キー の リスト を取得または設定します。
        /// </summary>
        public List<ExaminationAssociatedFileItem> OverallAssessmentPdfN { get; set; } = new List<ExaminationAssociatedFileItem>();

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationSetItem" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public ExaminationSetItem()
        {
        }

        #endregion
    }
}
