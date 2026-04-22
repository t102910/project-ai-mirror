using MGF.QOLMS.QolmsAzureStorageCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// HealthRecordキューに登録するための引数
    /// </summary>
    public class HealthRecordQueueWriterArgs: QsAzureQueueStorageWriterArgsBase<QoHealthRecordQueueEntity, QoHealthRecordQueueMessage>
    {
    }
}