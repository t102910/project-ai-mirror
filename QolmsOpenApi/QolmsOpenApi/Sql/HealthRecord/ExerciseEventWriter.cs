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
    /// OpenApiからの運動データを
    /// データベース テーブルへ登録するための機能を提供します。
    /// </summary>
    public class ExerciseEventWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, ExerciseEventWriterArgs, ExerciseEventWriterResults>
    {
        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス</param>
        /// <returns>DB 戻り値クラス</returns>
        public ExerciseEventWriterResults ExecuteByDistributed(ExerciseEventWriterArgs args)
        {
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            var result = new ExerciseEventWriterResults
            {
                IsSuccess = false,
                ActionDate = actionDate,
                ActionKey = actionKey,                
            };

            var (dataEntityN, logEntityN) = CreateEntities(args.VitalValueN, args.ActorKey, actionDate, actionKey);
            Worker.QoAccessLog.WriteInfoLog($"dataEntityN:{dataEntityN.Count}");
            if (!dataEntityN.Any())
            {
                return result;
            }

            var dataWriter = new QhExerciseEvent2EntityWriter();
            var dataArgs = new QhExerciseEvent2EntityWriterArgs
            {
                Data = dataEntityN
            };

            var logWriter = new QhExerciseEventHist2EntityWriter();
            var logArgs = new QhExerciseEventHist2EntityWriterArgs
            {
                Data = logEntityN
            };
            Worker.QoAccessLog.WriteInfoLog($"dataResult Insert");
            var dataResult = new QhExerciseEvent2EntityWriterResults();
            try
            {
                dataResult = QsDbManager.WriteByCurrent(dataWriter, dataArgs);
                Worker.QoAccessLog.WriteInfoLog($"{dataResult.IsSuccess},{dataResult.Result}");
                if (!dataResult.IsSuccess || dataResult.Result <= 0)
                {
                    return result;
                }
            }
            catch(Exception ex)
            {
                Worker.QoAccessLog.WriteInfoLog($"{ex.Message}");
            }

            var logResult = QsDbManager.WriteByCurrent(logWriter, logArgs);
            Worker.QoAccessLog.WriteInfoLog($"{logResult.IsSuccess},{logResult.Result}");
            if (!logResult.IsSuccess || logResult.Result <= 0)
            {
                return result;
            }

            result.IsSuccess = true;
            result.Result = dataResult.Result;
            return result;
        }


        (List<QH_EXERCISEEVENT2_DAT> dataEntityN, List<QH_EXERCISEEVENTHIST2_LOG> logEntityN) CreateEntities(
            List<DbVitalValueItem> exerciseValueN,
            Guid actorKey,
            DateTime actionDate,
            Guid actionKey)
        {
            var dataEntityN = new List<QH_EXERCISEEVENT2_DAT>();
            var logEntityN = new List<QH_EXERCISEEVENTHIST2_LOG>();
           
            foreach (var item in exerciseValueN)
            {
                var dataEntity = new QH_EXERCISEEVENT2_DAT
                {
                    ACCOUNTKEY = actorKey,
                    LINKAGESYSTEMNO = 99999,
                    RECORDDATE = item.RecordDate,
                    EXERCISETYPE = 255,
                    SEQUENCE = 1,
                    STARTDATE = item.RecordDate,
                    ENDDATE = item.RecordDate,
                    ITEMNAME = string.Empty,
                    CALORIE = short.Parse(item.Value1.ToString()),
                    FOREIGNKEY = string.Empty,
                    VALUE = string.Empty,
                    DELETEFLAG = false,
                    CREATEDDATE = actionDate,
                    UPDATEDDATE = actionDate
                };

                dataEntityN.Add(dataEntity);

                var logEntity = new QH_EXERCISEEVENTHIST2_LOG
                {
                    ACCOUNTKEY = dataEntity.ACCOUNTKEY,
                    LINKAGESYSTEMNO = dataEntity.LINKAGESYSTEMNO,
                    RECORDDATE = dataEntity.RECORDDATE,
                    EXERCISETYPE = dataEntity.EXERCISETYPE,
                    SEQUENCE = dataEntity.SEQUENCE,
                    ACTIONDATE = actionDate,
                    ACTIONKEY = actionKey,
                    HISTTYPE = (byte)QH_EXERCISEEVENTHIST2_LOG.HistTypeEnum.Added,
                    STARTDATE = dataEntity.STARTDATE,
                    ENDDATE = dataEntity.ENDDATE,
                    ITEMNAME = dataEntity.ITEMNAME,
                    CALORIE = dataEntity.CALORIE,
                    FOREIGNKEY = dataEntity.FOREIGNKEY,
                    VALUE = dataEntity.VALUE
                };

                logEntityN.Add(logEntity);
            }

            return (dataEntityN, logEntityN);
        }

        
    }
}