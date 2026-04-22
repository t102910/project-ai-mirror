using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class DailySequenceGenerateWriterResults: QsDbWriterResultsBase
    {
        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 操作キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ActionKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 自動採番された番号を取得または設定します。
        /// </summary>
        public int Sequence { get; set; } = 0;
    }
}