using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 施設の情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LinkageSystemMasterReaderArgs : QsDbReaderArgsBase<QH_LINKAGESYSTEM_MST>
    {


        /// <summary>
        /// FacilityKeyを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public Guid FacilityKey { get; set; } = Guid.Empty;



        /// <summary>
        /// <see cref="LinkageSystemMasterReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageSystemMasterReaderArgs() : base()
        {
        }
    }


}