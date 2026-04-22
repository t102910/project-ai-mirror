using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{

 
    /// <summary>
    /// 施設の診療科情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityMedicalDepartmentListReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 施設診療科の情報を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<DbMedicalDepartmentItem> MedicalDepartmentList { get; set; } = new List<DbMedicalDepartmentItem>();



        /// <summary>
        /// <see cref="FacilityMedicalDepartmentListReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentListReaderResults() : base()
        {
        }
    }


}