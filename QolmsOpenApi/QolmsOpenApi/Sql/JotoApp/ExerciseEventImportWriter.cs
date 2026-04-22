using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// JOTOアプリからの運動データを QH_EXERCISEEVENT2_DAT へ登録する（新方式）。
    /// 旧 ExerciseEventWriter (LINKAGESYSTEMNO=99999) とは別クラスとして並列実装。
    /// LINKAGESYSTEMNO は 47003 固定。
    /// </summary>
    public class ExerciseEventImportWriter
        : QsDbWriterBase,
          IQsDbDistributedWriter<MGF_NULL_ENTITY, ExerciseEventImportWriterArgs, ExerciseEventImportWriterResults>
    {
        /// <summary>JOTO 用リンケージシステム番号（固定値）</summary>
        private const int JotoLinkageSystemNo = 47003;

        /// <summary>
        /// 分散トランザクションを使用してデータベースへ運動データを登録します。
        /// </summary>
        public ExerciseEventImportWriterResults ExecuteByDistributed(ExerciseEventImportWriterArgs args)
        {
            var actionDate = DateTime.Now;
            var actionKey  = Guid.NewGuid();

            var result = new ExerciseEventImportWriterResults
            {
                IsSuccess  = false,
                ActionDate = actionDate,
                ActionKey  = actionKey,
            };

            if (args?.ExerciseEventN == null || !args.ExerciseEventN.Any())
            {
                Worker.QoAccessLog.WriteInfoLog("ExerciseEventImportWriter: ExerciseEventN が空です。");
                return result;
            }

            var (dataEntityN, logEntityN) = CreateEntities(
                args.ExerciseEventN, args.ActorKey, actionDate, actionKey);

            if (!dataEntityN.Any())
            {
                Worker.QoAccessLog.WriteInfoLog("ExerciseEventImportWriter: 有効なデータがありません。");
                return result;
            }

            // データ書込
            var dataWriter = new QhExerciseEvent2EntityWriter();
            var dataArgs   = new QhExerciseEvent2EntityWriterArgs { Data = dataEntityN };

            var logWriter  = new QhExerciseEventHist2EntityWriter();
            var logArgs    = new QhExerciseEventHist2EntityWriterArgs { Data = logEntityN };

            Worker.QoAccessLog.WriteInfoLog($"ExerciseEventImportWriter: Insert 開始 件数={dataEntityN.Count}");
            try
            {
                var dataResult = QsDbManager.WriteByCurrent(dataWriter, dataArgs);
                Worker.QoAccessLog.WriteInfoLog(
                    $"ExerciseEventImportWriter: dataResult IsSuccess={dataResult.IsSuccess}, Count={dataResult.Result}");

                if (!dataResult.IsSuccess || dataResult.Result <= 0)
                    return result;

                var logResult = QsDbManager.WriteByCurrent(logWriter, logArgs);
                Worker.QoAccessLog.WriteInfoLog(
                    $"ExerciseEventImportWriter: logResult IsSuccess={logResult.IsSuccess}, Count={logResult.Result}");

                if (!logResult.IsSuccess || logResult.Result <= 0)
                    return result;

                result.IsSuccess = true;
                result.Result    = dataResult.Result;
            }
            catch (Exception ex)
            {
                Worker.QoAccessLog.WriteErrorLog(
                    $"ExerciseEventImportWriter 例外: {ex.Message}", Guid.Empty);
            }

            return result;
        }

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// SEQUENCE 採番：同一 ACCOUNTKEY + LINKAGESYSTEMNO + RECORDDATE(日) + EXERCISETYPE の
        /// 最大 SEQUENCE + 1 を返す。DbCommand.ExecuteScalar を使用して型安全に取得。
        /// </summary>
        private int GetMaxSequenceFromDb(
            DbConnection con, Guid accountKey, DateTime recordDate, byte exerciseType)
        {
            try
            {
                var dayStart = new DateTime(recordDate.Year, recordDate.Month, recordDate.Day, 0,  0,  0, 0);
                var dayEnd   = new DateTime(recordDate.Year, recordDate.Month, recordDate.Day, 23, 59, 59, 999);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"
                        SELECT ISNULL(MAX({nameof(QH_EXERCISEEVENT2_DAT.SEQUENCE)}), 0)
                        FROM   {nameof(QH_EXERCISEEVENT2_DAT)}
                        WHERE  {nameof(QH_EXERCISEEVENT2_DAT.ACCOUNTKEY)}      = @p1
                        AND    {nameof(QH_EXERCISEEVENT2_DAT.LINKAGESYSTEMNO)} = @p2
                        AND    {nameof(QH_EXERCISEEVENT2_DAT.RECORDDATE)}      BETWEEN @p3 AND @p4
                        AND    {nameof(QH_EXERCISEEVENT2_DAT.EXERCISETYPE)}    = @p5
                        AND    {nameof(QH_EXERCISEEVENT2_DAT.DELETEFLAG)}      = 0
                    ";

                    var p1 = CreateParameter(con, "@p1", accountKey);
                    var p2 = CreateParameter(con, "@p2", JotoLinkageSystemNo);
                    var p3 = CreateParameter(con, "@p3", dayStart);
                    var p4 = CreateParameter(con, "@p4", dayEnd);
                    var p5 = CreateParameter(con, "@p5", exerciseType);

                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    cmd.Parameters.Add(p3);
                    cmd.Parameters.Add(p4);
                    cmd.Parameters.Add(p5);

                    var scalar = cmd.ExecuteScalar();
                    return (scalar != null && scalar != DBNull.Value) ? Convert.ToInt32(scalar) : 0;
                }
            }
            catch (Exception ex)
            {
                Worker.QoAccessLog.WriteInfoLog(
                    $"ExerciseEventImportWriter.GetMaxSequenceFromDb 例外（SEQUENCE=0 でフォールバック）: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// データエンティティとログエンティティを生成します。
        /// </summary>
        private (List<QH_EXERCISEEVENT2_DAT> data, List<QH_EXERCISEEVENTHIST2_LOG> log)
            CreateEntities(
                List<ExerciseEventImportItem> items,
                Guid actorKey,
                DateTime actionDate,
                Guid actionKey)
        {
            var dataList = new List<QH_EXERCISEEVENT2_DAT>();
            var logList  = new List<QH_EXERCISEEVENTHIST2_LOG>();

            // SEQUENCE: 同日同種別ごとに連番付与
            // キー: (RecordDate の日付, ExerciseType)
            var seqMap = new Dictionary<(DateTime date, byte type), int>();

            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                con.Open();

                foreach (var item in items)
                {
                    // バリデーション
                    if (!ValidateItem(item))
                    {
                        Worker.QoAccessLog.WriteInfoLog(
                            $"ExerciseEventImportWriter: バリデーションNG ExerciseType={item.ExerciseType}, Calorie={item.Calorie}");
                        continue;
                    }

                    var dateKey = (item.RecordDate.Date, item.ExerciseType);
                    if (!seqMap.ContainsKey(dateKey))
                    {
                        // DB上の最大SEQUENCE を取得して初期値を決定
                        seqMap[dateKey] = GetMaxSequenceFromDb(con, actorKey, item.RecordDate, item.ExerciseType);
                    }
                    seqMap[dateKey]++;
                    int seq = seqMap[dateKey];

                    var data = new QH_EXERCISEEVENT2_DAT
                    {
                        ACCOUNTKEY      = actorKey,
                        LINKAGESYSTEMNO = JotoLinkageSystemNo,   // 47003 固定
                        RECORDDATE      = item.RecordDate,
                        EXERCISETYPE    = item.ExerciseType,     // 0以外はすべて有効（255=その他）
                        SEQUENCE        = seq,
                        STARTDATE       = item.StartDate,
                        ENDDATE         = item.EndDate,
                        ITEMNAME        = item.ItemName ?? string.Empty,
                        CALORIE         = item.Calorie,          // 直接入力値
                        FOREIGNKEY      = item.ForeignKey ?? string.Empty,
                        VALUE           = item.Value ?? string.Empty,
                        DELETEFLAG      = false,
                        CREATEDDATE     = actionDate,
                        UPDATEDDATE     = actionDate,
                    };
                    dataList.Add(data);

                    logList.Add(new QH_EXERCISEEVENTHIST2_LOG
                    {
                        ACCOUNTKEY      = data.ACCOUNTKEY,
                        LINKAGESYSTEMNO = data.LINKAGESYSTEMNO,
                        RECORDDATE      = data.RECORDDATE,
                        EXERCISETYPE    = data.EXERCISETYPE,
                        SEQUENCE        = data.SEQUENCE,
                        ACTIONDATE      = actionDate,
                        ACTIONKEY       = actionKey,
                        HISTTYPE        = (byte)QH_EXERCISEEVENTHIST2_LOG.HistTypeEnum.Added,
                        STARTDATE       = data.STARTDATE,
                        ENDDATE         = data.ENDDATE,
                        ITEMNAME        = data.ITEMNAME,
                        CALORIE         = data.CALORIE,
                        FOREIGNKEY      = data.FOREIGNKEY,
                        VALUE           = data.VALUE,
                    });
                }
            }

            return (dataList, logList);
        }

        /// <summary>
        /// 1件分のバリデーション。
        /// ExerciseType=0 のみ不正。255（その他）は直接カロリー入力として有効。
        /// </summary>
        private bool ValidateItem(ExerciseEventImportItem item)
        {
            if (item.ExerciseType == 0)       return false; // 0 のみ不正
            if (item.Calorie < 1)             return false; // カロリーは 1 以上
            if (item.Calorie > 9999)          return false; // 上限
            if (item.RecordDate == default)   return false; // 日付必須
            return true;
        }
    }
}
