using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// SPHR連携対象者情報 を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class ExternalServiceSphrLinkageReaderResults : QsDbReaderResultsBase<QH_LINKAGE_DAT>
    {


        /// <summary>
        /// <see cref="ExternalServiceSphrLinkageReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExternalServiceSphrLinkageReaderResults() : base()
        {
        }
    }


}