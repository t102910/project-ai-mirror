using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Transactions;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class EventReactionWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, EventReactionWriterArgs, EventReactionWriterResults>
    {
        public EventReactionWriterResults ExecuteByDistributed(EventReactionWriterArgs args)
        {
            var result = new EventReactionWriterResults { IsSuccess = false };
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            if (args.Entity.DataState == QsDbEntityStateTypeEnum.Added)
            {
                args.Entity.CREATEDDATE = actionDate;
            }
            args.Entity.UPDATEDDATE = actionDate;

            // 物理削除の場合
            if (args.IsPhysicalDelete)
            {
                // 物理削除
                if (!PhysicalDelete(args.Entity))
                {
                    return result;
                }
            }
            // それ以外
            else
            {
                // 更新 or 挿入
                if (!UpsertEntity(args.Entity))
                {
                    return result;
                }
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

        private bool UpsertEntity(QH_EVENTREACTION_DAT entity)
        {
            var writer = new QhEventReactionEntityWriter();
            var writerArgs = new QhEventReactionEntityWriterArgs() { Data = new List<QH_EVENTREACTION_DAT>() { entity } };
            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                return true;
            }

            return false;
        }

        private bool PhysicalDelete(QH_EVENTREACTION_DAT entity)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_EVENTREACTION_DAT>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                    CreateParameter(connection, "@p2", entity.LINKAGESYSTEMNO),
                    CreateParameter(connection, "@p3", entity.EVENTDATE),
                    CreateParameter(connection, "@p4", entity.EVENTSEQUENCE),
                    CreateParameter(connection, "@p5", entity.FROMACCOUNTKEY),
                };

                var sql = $@"
                    DELETE {nameof(QH_EVENTREACTION_DAT)}
                    WHERE {nameof(QH_EVENTREACTION_DAT.ACCOUNTKEY)} = @p1
                    AND {nameof(QH_EVENTREACTION_DAT.LINKAGESYSTEMNO)} = @p2
                    AND {nameof(QH_EVENTREACTION_DAT.EVENTDATE)} = @p3
                    AND {nameof(QH_EVENTREACTION_DAT.EVENTSEQUENCE)} = @p4
                    AND {nameof(QH_EVENTREACTION_DAT.FROMACCOUNTKEY)} = @p5
                ";

                connection.Open();

                var ret = ExecuteNonQuery(connection, null, sql, param);
                return ret == 1;
            }
        }
    }
}