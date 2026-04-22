using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{


    /// <summary>
    /// 施設診療科JSON設定情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityMedicalDepartmentAppConfigReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 診療科設定情報 を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT MedicalDepartMentAppConfigEntity { get; set; }



        /// <summary>
        /// <see cref="FacilityMedicalDepartmentAppConfigReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentAppConfigReaderResults() : base()
        {
        }
    }


}