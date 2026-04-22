using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class HealthRecordAlertWriterArgs: QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 対象のEntityを設定する
        /// </summary>
        public QH_HEALTHRECORDALERT_DAT Entity { get; set; }

        /// <summary>
        /// 物理削除するかどうかを設定する
        /// </summary>
        public bool IsPhysicalDelete { get; set; } = false;
    }
}