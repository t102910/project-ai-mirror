using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 検索項目をもとに連携IDを取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class AccountIdForgetReaderArgs :
        QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// メールアドレスを取得または設定します。
        /// </summary>
        public string MailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 漢字姓を取得または設定します。
        /// </summary>
        public string FamilyName { get; set; } = string.Empty;

        /// <summary>
        /// 漢字名を取得または設定します。
        /// </summary>
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// 生年月日を取得または設定します。
        /// </summary>
        public DateTime Birthday { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 性別を取得または設定します。
        /// </summary>
        public byte Sex { get; set; } = byte.MinValue;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="AccountIdForgetReaderArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AccountIdForgetReaderArgs() : base() { }

        #endregion
    }
}
