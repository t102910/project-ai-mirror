using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システムの連携IDからアカウントキーを、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageUserReaderArgs :
        QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// 連携システムNoを取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// FacilityKeyを取得または設定します。連携システムNoが不明の場合にこちらを利用します。
        /// </summary>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 連携システムID（患者IDなど）を取得または設定します。
        /// </summary>
        public string LinkageSystemId { get; set; } = string.Empty;

        
        /// <summary>
        /// 連携ステータス番号を取得または設定します。デフォルトは２です。
        /// </summary>
        public byte StatusNo { get; set; } = 2;
        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageUserReaderArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LinkageUserReaderArgs() : base() { }

        #endregion
    }
}
