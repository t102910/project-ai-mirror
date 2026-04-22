using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    ///  利用者カードのリストを、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkagePatientCardReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// 所有者アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 対象者アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ActorKey { get; set; } = Guid.Empty;



        /// <summary>
        /// <see cref="LinkagePatientCardReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkagePatientCardReaderArgs() : base()
        {
        }
    }


}