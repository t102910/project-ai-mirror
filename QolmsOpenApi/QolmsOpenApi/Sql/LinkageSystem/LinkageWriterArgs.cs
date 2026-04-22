using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    ///   
    internal sealed class LinkageWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 対象のEntityを設定する。
        /// </summary>
        public QH_LINKAGE_DAT Entity { get; set; }

        /// <summary>
        /// <see cref="LinkageWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageWriterArgs() : base()
        {
        }
    }
}