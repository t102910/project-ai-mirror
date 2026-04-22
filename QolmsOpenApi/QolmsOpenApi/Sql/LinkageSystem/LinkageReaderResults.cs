using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システム ID を、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageReaderResults :
        QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        public String LinkageSystemId { get; set; } = string.Empty;

        
        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageReaderResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LinkageReaderResults() : base() { }

        #endregion
    }
}
