using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LinkageUserMemberShipReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// アカウントキーをを取得または設定します。　
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 会員タイプを取得または設定します。
        /// </summary>
        public byte MemberShipType { get; set; } = byte.MinValue;

        /// <summary>
        /// 連携用の共通IDを取得または設定します。
        /// </summary>
        public string LinkageId { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="LinkageUserMemberShipReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageUserMemberShipReaderResults() : base()
        {
        }
    }


}