using MGF.QOLMS.QolmsAzureStorageCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// HealthRecordキューを登録するための機能
    /// </summary>
    public class HealthRecordQueueWriter : 
        QsAzureQueueStorageWriterBase<QoHealthRecordQueueEntity, QoHealthRecordQueueMessage>,
        IQsAzureQueueStorageWriter<QoHealthRecordQueueEntity, QoHealthRecordQueueMessage, HealthRecordQueueWriterArgs, HealthRecordQueueWriterResult>

    {
        /// <summary>
        ///  コンストラクタ
        /// </summary>
        /// <param name="createIfNotExists">キュー が存在しない場合に作成するなら True、 作成しないなら False を指定。</param>
        public HealthRecordQueueWriter(bool createIfNotExists) : base(createIfNotExists)
        {
        }

        public HealthRecordQueueWriterResult Execute(HealthRecordQueueWriterArgs args)
        {
            var result = new HealthRecordQueueWriterResult();

            result.IsSuccess = this.Enqueue(args.Entity);

            return result;
        }
    }
}