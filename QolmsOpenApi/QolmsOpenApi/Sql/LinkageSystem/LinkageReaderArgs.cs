using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システム ID を、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageReaderArgs :
        QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// 連携システムNoを取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// AccountKeyを取得または設定します。
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 連携ステータス番号を取得または設定します。デフォルトは２です。
        /// </summary>
        public byte StatusType { get; set; } = 2;
        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageReaderArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LinkageReaderArgs() : base() { }

        #endregion
    }
}
