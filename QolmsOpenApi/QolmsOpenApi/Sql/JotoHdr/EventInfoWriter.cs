using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Transactions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class EventInfoWriter : QsDbWriterBase,
        IQsDbDistributedWriter<MGF_NULL_ENTITY, EventInfoWriterArgs, EventInfoWriterResults>
    {
        public EventInfoWriterResults ExecuteByDistributed(EventInfoWriterArgs args)
        {
            var result = new EventInfoWriterResults { IsSuccess = false };
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            args.Entity.CREATEDDATE = actionDate;
            args.Entity.UPDATEDDATE = actionDate;

            // 更新 or 挿入
            if (!UpsertEntity(args.Entity))
            {
                return result;
            }

            // 処理件数
            result.Result = 1;
            // 操作日時
            result.ActionDate = actionDate;
            // 操作キー
            result.ActionKey = actionKey;
            // 成功
            result.IsSuccess = true;

            return result;
        }

        private bool UpsertEntity(QJ_EVENTINFO_DAT entity)
        {
            var writer = new QjEventInfoEntityWriter();
            var writerArgs = new QjEventInfoEntityWriterArgs() { Data = new List<QJ_EVENTINFO_DAT>() { entity } };
            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                return true;
            }

            return false;
        }
    }
}
