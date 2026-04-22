using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class HealthRecordAlertWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, HealthRecordAlertWriterArgs, HealthRecordAlertWriterResults>
    {
        public HealthRecordAlertWriterResults ExecuteByDistributed(HealthRecordAlertWriterArgs args)
        {
            var result = new HealthRecordAlertWriterResults { IsSuccess = false };
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
            // 追加のみ
            else if (args.Entity.DataState == QsDbEntityStateTypeEnum.Added)
            {
                // 挿入
                if (!Insert(args.Entity))
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


        private bool UpsertEntity(QH_HEALTHRECORDALERT_DAT entity)
        {
            var writer = new QhHealthRecordAlertEntityWriter();
            var writerArgs = new QhHealthRecordAlertEntityWriterArgs() { Data = new List<QH_HEALTHRECORDALERT_DAT>() { entity } };
            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                return true;
            }

            return false;
        }

        private bool Insert(QH_HEALTHRECORDALERT_DAT entity)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(connection, "@P1", entity.ACCOUNTKEY),
                    CreateParameter(connection, "@P2", entity.RECORDDATE),
                    CreateParameter(connection, "@P3", entity.VITALTYPE),
                    CreateParameter(connection, "@P4", entity.LINKAGESYSTEMNO),
                    CreateParameter(connection, "@P5", entity.VALUE1),
                    CreateParameter(connection, "@P6", entity.VALUE2),
                    CreateParameter(connection, "@P7", entity.ALERTNO),
                    CreateParameter(connection, "@P8", entity.EMERGENCYTYPE),
                    CreateParameter(connection, "@P9", entity.ABNORMALTYPE),
                    CreateParameter(connection, "@P10", entity.ALERTTYPE),
                    CreateParameter(connection, "@P11", entity.MESSAGE),
                    CreateParameter(connection, "@P12", entity.DELETEFLAG),
                    CreateParameter(connection, "@P13", entity.CREATEDDATE),
                    CreateParameter(connection, "@P14", entity.UPDATEDDATE)

                };

                var sql = $@"
                    BEGIN
                     IF (SELECT COUNT(*)
                         FROM {nameof(QH_HEALTHRECORDALERT_DAT)}
                         WHERE {nameof(QH_HEALTHRECORDALERT_DAT.ACCOUNTKEY)} = @P1 AND {nameof(QH_HEALTHRECORDALERT_DAT.RECORDDATE)} = @P2
                         AND {nameof(QH_HEALTHRECORDALERT_DAT.VITALTYPE)} = @P3 AND {nameof(QH_HEALTHRECORDALERT_DAT.LINKAGESYSTEMNO)} = @P4) <> 0
                     BEGIN
                     END
                     ELSE
                     BEGIN
                      INSERT INTO {nameof(QH_HEALTHRECORDALERT_DAT)}
                      ({nameof(QH_HEALTHRECORDALERT_DAT.ACCOUNTKEY)}, {nameof(QH_HEALTHRECORDALERT_DAT.RECORDDATE)}, {nameof(QH_HEALTHRECORDALERT_DAT.VITALTYPE)},
                        {nameof(QH_HEALTHRECORDALERT_DAT.LINKAGESYSTEMNO)}, {nameof(QH_HEALTHRECORDALERT_DAT.VALUE1)}, {nameof(QH_HEALTHRECORDALERT_DAT.VALUE2)},
                        {nameof(QH_HEALTHRECORDALERT_DAT.ALERTNO)}, {nameof(QH_HEALTHRECORDALERT_DAT.EMERGENCYTYPE)}, {nameof(QH_HEALTHRECORDALERT_DAT.ABNORMALTYPE)},
                        {nameof(QH_HEALTHRECORDALERT_DAT.ALERTTYPE)}, {nameof(QH_HEALTHRECORDALERT_DAT.MESSAGE)}, {nameof(QH_HEALTHRECORDALERT_DAT.DELETEFLAG)},
                        {nameof(QH_HEALTHRECORDALERT_DAT.CREATEDDATE)}, {nameof(QH_HEALTHRECORDALERT_DAT.UPDATEDDATE)})
                      VALUES
                      (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10, @P11, @P12, @P13, @P14)
                     END
                    END
                ";

                connection.Open();

                var ret = ExecuteNonQuery(connection, null, sql, param);
                return ret == 1;
            }
        }

        private bool PhysicalDelete(QH_HEALTHRECORDALERT_DAT entity)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                    CreateParameter(connection, "@p2", entity.RECORDDATE),
                    CreateParameter(connection, "@p3", entity.VITALTYPE),
                    CreateParameter(connection, "@p4", entity.LINKAGESYSTEMNO)
                };

                var sql = $@"
                    DELETE {nameof(QH_HEALTHRECORDALERT_DAT)}
                    WHERE {nameof(QH_HEALTHRECORDALERT_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.RECORDDATE)} = @p2
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.VITALTYPE)} = @p3
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.LINKAGESYSTEMNO)} = @p4
                ";

                connection.Open();

                var ret = ExecuteNonQuery(connection, null, sql, param);
                return ret == 1;
            }
        }
    }
}