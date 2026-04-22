using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class EventInfoWriterArgs: QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 対象のEntityを設定する
        /// </summary>
        public QJ_EVENTINFO_DAT Entity { get; set; }
    }
}
