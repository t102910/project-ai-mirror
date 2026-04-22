using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システムの連携IDからレコード更新日時を、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageUpdatedReaderArgs :
        QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// 施設キーを取得または設定します。
        /// </summary>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 連携システムID（患者IDなど）を取得または設定します。
        /// </summary>
        public string LinkageSystemId { get; set; } = string.Empty;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageUpdatedReaderArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LinkageUpdatedReaderArgs() : base() { }

        #endregion
    }
}
