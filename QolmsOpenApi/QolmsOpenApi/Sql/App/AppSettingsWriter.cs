using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class AppSettingsWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, AppSettingsWriterArgs, AppSettingsWriterResults>
    {
        public AppSettingsWriterResults ExecuteByDistributed(AppSettingsWriterArgs args)
        {
            var result = new AppSettingsWriterResults { IsSuccess = false };
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


        private bool UpsertEntity(QH_APPSETTINGS_DAT entity)
        {
            var writer = new QhAppSettingsEntityWriter();
            var writerArgs = new QhAppSettingsEntityWriterArgs() { Data = new List<QH_APPSETTINGS_DAT>() { entity } };
            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                return true;
            }

            return false;
        }

        private bool PhysicalDelete(QH_APPSETTINGS_DAT entity)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_APPSETTINGS_DAT>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                    CreateParameter(connection, "@p2", entity.APPTYPE),
                };

                var sql = $@"
                    DELETE {nameof(QH_APPSETTINGS_DAT)}
                    WHERE {nameof(QH_APPSETTINGS_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_APPSETTINGS_DAT.APPTYPE)} = @p2
                ";

                connection.Open();

                var ret = ExecuteNonQuery(connection, null, sql, param);
                return ret == 1;
            }
        }
    }
}