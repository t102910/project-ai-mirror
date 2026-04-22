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
    internal class MedicineWriter : QsDbWriterBase,
        IQsDbDistributedWriter<MGF_NULL_ENTITY, MedicineWriterArgs, MedicineWriterResults>
    {
        public MedicineWriterResults ExecuteByDistributed(MedicineWriterArgs args)
        {
            var result = new MedicineWriterResults { IsSuccess = false };
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

            // ログテーブル書き込み
            WriteLog(args.Entity, actionDate, actionKey);

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

        private bool UpsertEntity(QH_MEDICINE_DAT entity)
        {
            if(entity.DataState == QsDbEntityStateTypeEnum.Added)
            {
                entity.SEQUENCE = GetSequenceNo(entity.ACCOUNTKEY, entity.RECORDDATE) + 1;
                if (string.IsNullOrWhiteSpace(entity.RECEIPTNO))
                {
                    entity.RECEIPTNO = $"{entity.RECORDDATE.ToString("yyyyMMdd")}{entity.SEQUENCE:d6}";
                }
            }

            var writer = new QhMedicineEntityWriter();
            var writerArgs = new QhMedicineEntityWriterArgs() { Data = new List<QH_MEDICINE_DAT>() { entity } };
            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                return true;
            }

            return false;
        }

        private int GetSequenceNo(Guid accountkey, DateTime recordDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {                
                var param = new List<DbParameter>() {
                    this.CreateParameter(connection, "@p1", accountkey),
                    this.CreateParameter(connection, "@p2", recordDate)
                };


                var sql = $@"
                    SELECT TOP(1) {nameof(QH_MEDICINE_DAT.SEQUENCE)}
                    FROM {nameof(QH_MEDICINE_DAT)}
                    WHERE {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    AND {nameof(QH_MEDICINE_DAT.RECORDDATE)} = @p2
                    ORDER BY {nameof(QH_MEDICINE_DAT.SEQUENCE)} DESC
                ";

                // コネクションオープン
                connection.Open();

                bool isSuccess = false;

                // クエリを実行
                return TryExecuteScalar(connection, null, sql, param,0, ref isSuccess);
            }
        }

        private void WriteLog(QH_MEDICINE_DAT entity, DateTime actionDate, Guid actionKey)
        {
            var logEntity = new QH_MEDICINEHIST_LOG()
            {
                ACCOUNTKEY = entity.ACCOUNTKEY,
                RECORDDATE = entity.RECORDDATE,
                SEQUENCE = entity.SEQUENCE,
                ACTIONDATE = actionDate,
                ACTIONKEY = actionKey,
                HISTTYPE = (byte)entity.DataState,
                PHARMACYNO = entity.PHARMACYNO,
                RECEIPTNO = entity.RECEIPTNO,
                FACILITYKEY = entity.FACILITYKEY,
                ORIGINALFILENAME = entity.ORIGINALFILENAME,
                MEDICINESET = entity.MEDICINESET,
                CONVERTEDMEDICINESET = entity.CONVERTEDMEDICINESET,
                COMMENTSET = entity.COMMENTSET
            };

            QsDbManager.WriteByCurrent(
                new QhMedicineHistEntityWriter(),
                new QhMedicineHistEntityWriterArgs()
                {
                    Data = new List<QH_MEDICINEHIST_LOG>() { logEntity }
                }
            );
        }

        private bool PhysicalDelete(QH_MEDICINE_DAT entity)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                    CreateParameter(connection, "@p2", entity.RECORDDATE),
                    CreateParameter(connection, "@p3", entity.SEQUENCE)
                };

                var sql = $@"
                    DELETE {nameof(QH_MEDICINE_DAT)}
                    WHERE {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_MEDICINE_DAT.RECORDDATE)} = @p2
                    AND   {nameof(QH_MEDICINE_DAT.SEQUENCE)} = @p3
                ";

                connection.Open();

                var ret = ExecuteNonQuery(connection, null, sql, param);
                return ret == 1;
            }
        }
    }
}