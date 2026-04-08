using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// 連携の情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public sealed class LinkageItem
    {
        #region Public Property

        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 連携システム ID を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// データセットを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Dataset { get; set; } = string.Empty;

        /// <summary>
        /// ステータスを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public QjLinkageStatusTypeEnum StatusType { get; set; } = QjLinkageStatusTypeEnum.None;

        /// <summary>
        /// 施設キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid Facilitykey { get; set; } = Guid.Empty;

        /// <summary>
        /// 施設名を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemName { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="LinkageItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageItem()
        {
        }

        #endregion
    }

}