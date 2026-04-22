using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設のTEL情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityContactInformationListReaderResults : QsDbReaderResultsBase<QH_FACILITYCONTACTINFORMATION_DAT>
    {

        
        /// <summary>
        /// <see cref="FacilityContactInformationListReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityContactInformationListReaderResults() : base()
        {
        }
    }


}