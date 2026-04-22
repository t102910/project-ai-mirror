using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 検索項目をもとにログインIDを取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class AccountIdForgetReaderResults :
        QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// AccountIdを取得または設定します。
        /// </summary>
        public List<string> AccountId { get; set; }


        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="AccountIdForgetReaderResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AccountIdForgetReaderResults() : base() { }

        #endregion
    }
}
