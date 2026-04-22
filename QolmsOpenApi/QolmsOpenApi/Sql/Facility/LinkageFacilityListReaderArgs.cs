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
    internal sealed class LinkageFacilityListReaderArgs : QsDbReaderArgsBase<QH_FACILITY_MST>
    {


        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 更新日を取得または設定します。
        /// この日時以降に更新されたものを取得したい場合に使用します。
        /// </summary>
        public DateTime UpdatedDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// <see cref="LinkageFacilityListReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageFacilityListReaderArgs() : base()
        {
        }
    }


}