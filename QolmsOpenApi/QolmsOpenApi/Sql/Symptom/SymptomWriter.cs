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
    internal class SymptomWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, SymptomWriterArgs, SymptomWriterResults>
    {
        public SymptomWriterResults ExecuteByDistributed(SymptomWriterArgs args)
        {
            var result = new SymptomWriterResults { IsSuccess = false };
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

        private bool UpsertEntity(QH_SYMPTOM_DAT entity)
        {
            var writer = new QhSymptomEntityWriter();
            var writerArgs = new QhSymptomEntityWriterArgs() { Data = new List<QH_SYMPTOM_DAT>() { entity } };
            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                return true;
            }

            return false;
        }

        private bool PhysicalDelete(QH_SYMPTOM_DAT entity)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_SYMPTOM_DAT>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(connection, "@p1", entity.ID),
                };

                var sql = $@"
                    DELETE {nameof(QH_SYMPTOM_DAT)}
                    WHERE {nameof(QH_SYMPTOM_DAT.ID)} = @p1
                ";

                connection.Open();

                var ret = ExecuteNonQuery(connection, null, sql, param);
                return ret == 1;
            }
        }
    }
}