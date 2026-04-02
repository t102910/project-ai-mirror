using System;
using System.Collections.Generic;
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
    public sealed class ExaminationGroupItem
    {
        #region Public Property

        /// <summary>
        /// グループ番号を取得または設定します。
        /// </summary>
        public int GroupNo { get; set; } = int.MinValue;

        /// <summary>
        /// グループ名を取得または設定します。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// コメントを取得または設定します。
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// 検査グループ情報のリストを取得または設定します。
        /// </summary>
        public List<ExaminationGroupItem> ChildN { get; set; } = new List<ExaminationGroupItem>();

        /// <summary>
        /// 検査結果情報のリストを取得または設定します。
        /// </summary>
        public List<ExaminationItem> ExaminationN { get; set; } = new List<ExaminationItem>();

        [Obsolete("検討中")]
        public string ParentNo { get; set; } = string.Empty;

        [Obsolete("検討中")]
        public string DispOrder { get; set; } = string.Empty;

        /// <summary>
        /// 判定を取得または設定します。
        /// </summary>
        public ExaminationJudgementItem Judgement { get; set; } = new ExaminationJudgementItem();

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationGroupItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ExaminationGroupItem()
        {
        }

        #endregion
    }
}
