using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 学童健診Blob管理データ を、
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class CheckupAfterSchoolFileWriterResults : QsDbWriterResultsBase
    {

        /// <summary>
        /// <see cref="CheckupAfterSchoolFileWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupAfterSchoolFileWriterResults() : base()
        {
        }
    }


}