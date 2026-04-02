using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// ビュー 内に展開する健診結果付随 ファイル 情報への参照 パラメータ を表します。
    /// この クラス は継承できません。
    /// </summary>
    [DataContract]
    public sealed class AssociatedFileStorageReferenceJsonParameter : QjJsonParameterBase
    {
        #region Public Property

        /// <summary>
        /// アカウント キー を取得または設定します。
        /// </summary>
        [DataMember]
        public string Accountkey { get; set; } = string.Empty;

        /// <summary>
        /// ログイン 日時を取得または設定します。
        /// </summary>
        [DataMember]
        public string LoginAt { get; set; } = string.Empty;

        /// <summary>
        /// 連携 システム 番号を取得または設定します。
        /// </summary>
        [DataMember]
        public string LinkageSystemNo { get; set; } = string.Empty;

        /// <summary>
        /// 連携 システム ID を取得または設定します。
        /// </summary>
        [DataMember]
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// 健診受診を取得または設定します。
        /// </summary>
        [DataMember]
        public string RecordDate { get; set; } = string.Empty;

        /// <summary>
        /// 施設 キー を取得または設定します。
        /// </summary>
        [DataMember]
        public string FacilityKey { get; set; } = string.Empty;

        /// <summary>
        /// データ キー を取得または設定します。
        /// </summary>
        [DataMember]
        public string DataKey { get; set; } = string.Empty;

        /// <summary>
        /// URL 連携の場合に使用する キー を取得または設定します。
        /// </summary>
        [Obsolete("未使用です。")]
        [DataMember]
        public string AdditionalKey { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AssociatedFileStorageReferenceJsonParameter" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AssociatedFileStorageReferenceJsonParameter()
            : base()
        {
        }

        #endregion
    }
}
