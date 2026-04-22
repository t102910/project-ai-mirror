using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// OpenApiからのバイタルデータを
    /// データベース テーブルへ登録するための機能を提供します。
    /// </summary>
    public class HealthRecordDeleteWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, HealthRecordDeleteWriterArgs, HealthRecordDeleteWriterResults>
    {
        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス</param>
        /// <returns>DB 戻り値クラス</returns>
        public HealthRecordDeleteWriterResults ExecuteByDistributed(HealthRecordDeleteWriterArgs args)
        {
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            var result = new HealthRecordDeleteWriterResults
            {
                IsSuccess = false,
                ActionDate = actionDate,
                ActionKey = actionKey,                
            };

            var (dataEntityN, logEntityN) = CreateEntities(args.VitalValueN, args.ActorKey, actionDate, actionKey);

            if (!dataEntityN.Any())
            {
                return result;
            }

            var dataWriter = new QhHealthRecordEntityWriter();
            var dataArgs = new QhHealthRecordEntityWriterArgs
            {
                Data = dataEntityN
            };

            var logWriter = new QhHealthRecordHistEntityWriter();
            var logArgs = new QhHealthRecordHistEntityWriterArgs
            {
                Data = logEntityN
            };

            var dataResult = QsDbManager.WriteByCurrent(dataWriter, dataArgs);

            if (!dataResult.IsSuccess || dataResult.Result <= 0)
            {
                return result;
            }

            var logResult = QsDbManager.WriteByCurrent(logWriter, logArgs);

            if (!logResult.IsSuccess || logResult.Result <= 0)
            {
                return result;
            }

            result.IsSuccess = true;
            result.Result = dataResult.Result;
            return result;
        }


        (List<QH_HEALTHRECORD_DAT> dataEntityN, List<QH_HEALTHRECORDHIST_LOG> logEntityN) CreateEntities(
            List<DbVitalValueItem> vitalValueN,
            Guid actorKey,
            DateTime actionDate,
            Guid actionKey)
        {
            var dataEntityN = new List<QH_HEALTHRECORD_DAT>();
            var logEntityN = new List<QH_HEALTHRECORDHIST_LOG>();
           
            foreach (var item in vitalValueN)
            {
                var dataEntity = new QH_HEALTHRECORD_DAT
                {
                    ACCOUNTKEY = actorKey,
                    RECORDDATE = item.RecordDate,
                    VITALTYPE = (byte)item.VitalType,
                    VALUE1 = item.Value1,
                    VALUE2 = item.Value2,
                    CONDITIONTYPE = item.ConditionType,
                    DELETEFLAG = true,
                    CREATEDDATE = actionDate,
                    UPDATEDDATE = actionDate,
                    IsEmpty = false,
                    DataState = QsDbEntityStateTypeEnum.Modified
                };

                dataEntityN.Add(dataEntity);

                var logEntity = new QH_HEALTHRECORDHIST_LOG
                {
                    ACCOUNTKEY = actorKey,
                    RECORDDATE = dataEntity.RECORDDATE,
                    VITALTYPE = dataEntity.VITALTYPE,
                    ACTIONDATE = actionDate,
                    ACTIONKEY = actionKey,
                    HISTTYPE = (byte)QH_HEALTHRECORDHIST_LOG.HistTypeEnum.Deleted,
                    VALUE1 = decimal.MinusOne,
                    VALUE2 = decimal.MinusOne,
                    CONDITIONTYPE = (byte)QH_HEALTHRECORDHIST_LOG.ConditionTypeEnum.None,
                    IsEmpty = false,
                    DataState = QsDbEntityStateTypeEnum.Added
                };

                logEntityN.Add(logEntity);
            }

            return (dataEntityN, logEntityN);
        }

        
    }
}