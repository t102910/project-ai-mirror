using System;
using System.Runtime.Serialization;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 日付および日付内連番ごとの検査結果情報を表します。
    /// この クラス は継承できません。
    /// </summary>
    [Serializable]
    public sealed class ExaminationJudgementItem
    {
        #region Public Property

        /// <summary>
        /// 院内 コード を取得または設定します。
        /// </summary>
        [DataMember]
        public string LocalCode { get; set; } = string.Empty;

        /// <summary>
        /// 主 コース 名称を取得または設定します。
        /// </summary>
        [DataMember]
        public string CourseName { get; set; } = string.Empty;

        /// <summary>
        /// 項目名称を取得または設定します。
        /// </summary>
        [DataMember]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 結果値・所見を取得または設定します。
        /// </summary>
        [DataMember]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 判定 1 を取得または設定します。
        /// </summary>
        [DataMember]
        public string Judgment1 { get; set; } = string.Empty;

        /// <summary>
        /// 判定 2 を取得または設定します。
        /// </summary>
        [DataMember]
        public string Judgment2 { get; set; } = string.Empty;

        /// <summary>
        /// 判定内容 1 を取得または設定します。
        /// </summary>
        [DataMember]
        public string JudgmentContent1 { get; set; } = string.Empty;

        /// <summary>
        /// 判定内容 2 を取得または設定します。
        /// </summary>
        [DataMember]
        public string JudgmentContent2 { get; set; } = string.Empty;

        /// <summary>
        /// 総合判定かを取得または設定します。
        /// </summary>
        [DataMember]
        public bool IsTotalJudgment { get; set; } = false;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationJudgementItem" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public ExaminationJudgementItem()
        {
        }

        #endregion
    }
}
