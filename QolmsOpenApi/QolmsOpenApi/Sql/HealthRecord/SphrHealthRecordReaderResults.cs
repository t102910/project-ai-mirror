using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// SPHR用のバイタル情報 を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SphrHealthRecordReaderResults : QsDbReaderResultsBase<QH_HEALTHRECORD_DAT>
    {


        /// <summary>
        /// <see cref="SphrHealthRecordReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public SphrHealthRecordReaderResults() : base()
        {
        }
    }


}