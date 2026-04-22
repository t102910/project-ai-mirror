using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi
{

    /// <summary>
    /// SPHR連携対象者情報 を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class ExternalServiceSphrLinkageReaderArgs : QsDbReaderArgsBase<QH_LINKAGE_DAT>
    {

        /// <summary>
        /// AccountKey を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 対象連携の連携システム番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int CommunitySystemNo { get; set; } = int.MinValue;


        /// <summary>
        /// <see cref="ExternalServiceSphrLinkageReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExternalServiceSphrLinkageReaderArgs() : base()
        {
        }
    }


}