using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// ビュー 内に展開する 問診システム 情報への参照 パラメータ を表します。
    /// この クラス は継承できません。
    /// </summary>
    [DataContract]
    public sealed class MonshinArgsJsonParamater : QjJsonParameterBase
    {
        #region Public Property

        /// <summary>
        /// 診察券番号 を取得または設定します。
        /// </summary>
        [DataMember]
        public string patientID { get; set; } = string.Empty;

        /// <summary>
        /// 氏名（JOTO登録情報）※全角を取得または設定します。
        /// </summary>
        [DataMember]
        public string patientName { get; set; } = string.Empty;

        /// <summary>
        /// カナ姓名（JOTO登録情報）※全角を取得または設定します。
        /// </summary>
        [DataMember]
        public string patientNameKana { get; set; } = string.Empty;

        /// <summary>
        /// 性別（JOTO登録情報）※半角数値（1=男、2=女、3=不明） を取得または設定します。
        /// </summary>
        [DataMember]
        public string gendar { get; set; } = string.Empty;

        /// <summary>
        /// 生年月日（JOTO登録情報）※半角（YYYY-MM-DD）を取得または設定します。
        /// </summary>
        [DataMember]
        public string birthday { get; set; } = string.Empty;

        /// <summary>
        /// 呼び出し日時※半角（yyyyMMddHHmmss）を取得または設定します。
        /// </summary>
        [DataMember]
        public string timestamp { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="MonshinArgsJsonParamater" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public MonshinArgsJsonParamater()
            : base()
        {
        }

        #endregion
    }
}
