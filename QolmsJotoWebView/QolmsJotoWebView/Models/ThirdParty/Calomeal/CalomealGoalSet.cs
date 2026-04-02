using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// カロミルの目標設定情報を格納するクラスです。
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class CalomealGoalSet
    {
        #region Public Property

        /// <summary>
        /// 目標対象の日付
        /// </summary>
        [DataMember]
        public DateTime TargetDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 1日の摂取目標カロリー
        /// </summary>
        [DataMember]
        public int BasisAllCalorie { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="CalomealGoalSet" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CalomealGoalSet()
        {
        }

        #endregion
    }

}