using System;
using System.Collections.Generic;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// 検査項目グループの情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class ExaminationGroupNameReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        ///     検査項目グループ名を取得または設定します。
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks>key:ItenCode Value(Tuple1:GroupNo Tuple2:GroupName Tuple3:ItemName Tuple4:Comment)</remarks>
        public Dictionary<string, Tuple<int, string, string, string>> GroupNameList { get; set; }

        /// <summary>
        /// <see cref="ExaminationGroupNameReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExaminationGroupNameReaderResults() : base()
        {
        }
    }


}