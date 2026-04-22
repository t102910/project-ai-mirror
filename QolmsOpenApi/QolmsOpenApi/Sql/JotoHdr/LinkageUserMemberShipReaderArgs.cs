using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LinkageUserMemberShipReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// </summary>
        public string AuSystemId { get; set; } = string.Empty;

        /// <summary>
        /// </summary>
        public string LinkageId { get; set; } = string.Empty;

        /// <summary>
        /// </summary>
        public DateTime TargetDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 実行者アカウントキーを取得または設定します。
        /// </summary>
        public Guid Executor = Guid.Empty;


        /// <summary>
        /// <see cref="LinkageUserMemberShipReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageUserMemberShipReaderArgs() : base()
        {
        }
    }


}