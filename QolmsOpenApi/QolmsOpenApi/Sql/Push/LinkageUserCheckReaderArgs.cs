using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LinkageUserCheckReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// </summary>
        public string LinkageIds { get; set; } = string.Empty;

        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// <see cref="LinkageUserCheckReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageUserCheckReaderArgs() : base()
        {
        }
    }


}