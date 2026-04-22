using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class ExternalServiceLinkageWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        public QH_EXTERNALSERVICELINKAGE_DAT Entity { get; set; }
    }
}