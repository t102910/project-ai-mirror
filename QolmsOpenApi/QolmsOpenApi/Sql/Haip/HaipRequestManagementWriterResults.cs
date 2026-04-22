using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// HAIPデータ取得依頼情報 を、
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class HaipRequestManagementWriterResults : QsDbWriterResultsBase
    {

        /// <summary>
        /// <see cref="HaipRequestManagementWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public HaipRequestManagementWriterResults() : base()
        {
        }
    }


}