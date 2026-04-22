using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// OpenApiからの運動データを
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// </summary>
    public class ExerciseEventWriterResults : QsDbWriterResultsBase
    {        
        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        ///  操作キーを取得または設定します。
        /// </summary>
        public Guid ActionKey { get; set; } = Guid.Empty;
    }
}