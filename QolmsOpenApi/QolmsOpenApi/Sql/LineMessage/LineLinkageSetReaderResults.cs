using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// QH_LINAKGESYSTEM_MSTの情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LineLinkageSetReaderResults : QsDbReaderResultsBase<QH_LINKAGESYSTEM_MST>
    {


        /// <summary>
        /// <see cref="LineLinkageSetReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineLinkageSetReaderResults() : base()
        {
        }
    }


}