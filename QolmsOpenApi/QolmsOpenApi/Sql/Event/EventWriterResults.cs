using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class EventWriterResults : QsDbWriterResultsBase
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
    }
}