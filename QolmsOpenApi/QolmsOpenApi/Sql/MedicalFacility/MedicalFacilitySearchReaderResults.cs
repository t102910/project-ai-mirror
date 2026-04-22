using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 医療施設検索の情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class MedicalFacilitySearchReaderResults : QsDbReaderResultsBase<QH_FACILITY_MST>
    {


        /// <summary>
        /// <see cref="MedicalFacilitySearchReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public MedicalFacilitySearchReaderResults() : base()
        {
        }
    }


}