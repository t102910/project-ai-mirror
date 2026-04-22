using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システムの連携IDからアカウントキーを、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageUserReaderResults :
        QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        public Guid AccountKey { get; set; } = Guid.Empty;

        
        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageUserReaderResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LinkageUserReaderResults() : base() { }

        #endregion
    }
}
