using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// 検査結果の情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class ExaminationDetailReaderResults : QsDbReaderResultsBase<QH_EXAMINATION_DAT>
    {


        /// <summary>
        /// <see cref="ExaminationDetailReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExaminationDetailReaderResults() : base()
        {
        }
    }


}