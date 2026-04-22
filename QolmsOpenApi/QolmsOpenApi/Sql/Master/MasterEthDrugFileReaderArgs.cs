using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 医薬品ファイル情報 引数クラス
    /// </summary>
    public class MasterEthDrugFileReaderArgs: QsDbReaderArgsBase<QH_ETHDRUGFILE_MST>
    {
        /// <summary>
        /// ファイルキー
        /// </summary>
        public Guid FileKey { get; set; }
    }
}