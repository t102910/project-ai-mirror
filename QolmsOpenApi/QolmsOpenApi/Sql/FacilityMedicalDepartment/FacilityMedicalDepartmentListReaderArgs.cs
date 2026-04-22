using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 施設のURL情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityMedicalDepartmentListReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// 施設きーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid Facilitykey { get; set; } = Guid.Empty;

        

        /// <summary>
        /// <see cref="FacilityMedicalDepartmentListReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentListReaderArgs() : base()
        {
        }
    }


}