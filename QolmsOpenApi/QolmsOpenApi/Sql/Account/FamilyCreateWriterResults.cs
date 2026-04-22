using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 家族アカウント作成Writer 戻り値
    /// </summary>
    public class FamilyCreateWriterResults: QsDbWriterResultsBase
    {
        /// <summary>
        /// 発行したアカウントキー
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;
    }
}