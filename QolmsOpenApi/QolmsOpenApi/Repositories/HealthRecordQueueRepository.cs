using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// HealthRecord QueueStorage 入出力インターフェース
    /// </summary>
    public interface IHealthRecordQueueRepository
    {
        /// <summary>
        /// HealthRecordキューを追加する
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="vitalList">バイタルのリスト</param>
        /// <returns></returns>
        bool Enqueue(Guid accountKey, List<QhApiVitalValueItem> vitalList);
    }

    /// <summary>
    /// HealthRecord QueueStorage 入出力を行う
    /// </summary>
    public class HealthRecordQueueRepository: IHealthRecordQueueRepository
    {
        /// <summary>
        /// HealthRecordキューを追加する
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="vitalList">バイタルのリスト</param>
        /// <returns></returns>
        public bool Enqueue(Guid accountKey, List<QhApiVitalValueItem> vitalList)
        {
            try
            {
                var args = new HealthRecordQueueWriterArgs
                {
                    Entity = new QoHealthRecordQueueEntity
                    {
                        Message = new QoHealthRecordQueueMessage
                        {
                            ACCOUNTKEY = accountKey.ToString(),
                            VitalDataJson = JsonConvert.SerializeObject(vitalList)
                        }
                    }
                };

                var writer = new HealthRecordQueueWriter(true);

                var result = QsAzureStorageManager.Write(writer, args);

                return result.IsSuccess;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}