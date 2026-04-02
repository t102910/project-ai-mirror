using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 医療機関の情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class MedicalInstitutionItem
    {
        #region Public Property

        /// <summary>
        /// 病院コードを取得または設定します。
        /// </summary>
        public int CodeNo { get; set; } = int.MinValue;

        /// <summary>
        /// カナ名称を取得または設定します。
        /// </summary>
        public string KanaName { get; set; } = string.Empty;

        /// <summary>
        /// 医療機関名称を取得または設定します。
        /// </summary>
        public string InstitutionName { get; set; } = string.Empty;

        /// <summary>
        /// 郵便番号を取得または設定します。
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// 住所を取得または設定します。
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 診療科のリストを取得または設定します。
        /// </summary>
        public List<string> DepartmentN { get; set; } = new List<string>();

        /// <summary>
        /// 各フラグを取得または設定します。
        /// </summary>
        public int OptionFlags { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="MedicalInstitutionItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MedicalInstitutionItem()
        {
        }

        #endregion
    }
}
