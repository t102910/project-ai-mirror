using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// LineUserIdの情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LineUserIdReaderResults : QsDbReaderResultsBase<QH_OPENIDMANAGEMENT_DAT>
    {


        /// <summary>
        /// <see cref="LineUserIdReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineUserIdReaderResults() : base()
        {
        }
    }


}