using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{


    /// <summary>
    /// 診察呼び出し順番情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class WaitingOrderListReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 診察呼び出し順番情報 を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_WAITINGORDERLIST_DAT> WaitingOrderListEntity { get; set; }



        /// <summary>
        /// <see cref="WaitingOrderListReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public WaitingOrderListReaderResults() : base()
        {
        }
    }


}