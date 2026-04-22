using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Workers = MGF.QOLMS.QolmsLibraryV1.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Common;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// バイタル値入出力インターフェース
    /// </summary>
    public interface IHealthRecordRepository
    {
        /// <summary>
        /// バイタル値をDBに書き込む
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="vitals"></param>
        void WriteVitals(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vitals);

        /// <summary>
        /// 運動データをDBに書き込む
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="exercises"></param>
        void WriteExercise(Guid accountKey, Guid authorKey, List<DbVitalValueItem> exercises);

        /// <summary>
        /// バイタル値をStorageに書き込む
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="vitals"></param>
        void WriteStorage(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vitals);

        /// <summary>
        /// 範囲データを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalTypes"></param>
        /// <returns></returns>
        List<QH_HEALTHRECORD_DAT> ReadRange(Guid accountKey, DateTime fromDate, DateTime toDate, params byte[] vitalTypes);

        /// <summary>
        /// 最新データを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalTypes"></param>
        /// <returns></returns>
        List<QH_HEALTHRECORD_DAT> ReadNew(Guid accountKey, DateTime toDate, byte vitalType);

        /// <summary>
        /// Azure Table Storageから心拍データを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="fromDate">取得開始日時</param>
        /// <param name="toDate">取得終了日時</param>
        /// <returns>バイタル値アイテムリスト</returns>
        List<DbVitalValueItem> ReadStoragePulse(Guid accountKey, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Azure Table Storageからメッツデータを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="fromDate">取得開始日時</param>
        /// <param name="toDate">取得終了日時</param>
        /// <returns>バイタル値アイテムリスト</returns>
        List<DbVitalValueItem> ReadStorageMets(Guid accountKey, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Azure Table Storageから血中酸素濃度データを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="fromDate">取得開始日時</param>
        /// <param name="toDate">取得終了日時</param>
        /// <returns>バイタル値アイテムリスト</returns>
        List<DbVitalValueItem> ReadStorageBloodOxygen(Guid accountKey, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// 消費カロリーデータを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        List<DbVitalValueItem> ReadExerciseRange(Guid accountKey, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// バイタル値を削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="vitals"></param>
        void DeleteVitals(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vital);
    }

    /// <summary>
    /// バイタル値入出力実装
    /// </summary>
    public class HealthRecordRepository: QsDbReaderBase, IHealthRecordRepository
    {
        /// <summary>
        /// バイタル値をDBに書き込む
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="vitals"></param>
        public void WriteVitals(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vitals)
        {
            var writer = new HealthRecordWriter();
            var args = new HealthRecordWriterArgs
            {
                ActorKey = accountKey,
                AuthorKey = authorKey,
                VitalValueN = vitals,
            };

            var result = QsDbManager.Write(writer, args);

            if (!result.IsSuccess)
            {
                throw new Exception();
            }

            if(result.Result != vitals.Count)
            {
                throw new Exception();
            }

            // 成功時、睡眠以外のデータがある場合は歩数・カロリーの月平均を登録する
            if(vitals.Any(x => x.VitalType != QsDbVitalTypeEnum.SleepingTime))
            {
                StartWriteMonthlyAvarage(accountKey, authorKey, vitals);
            }
        }

        void StartWriteMonthlyAvarage(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vitals)
        {            
            var yHash = new HashSet<int>();
            var ymHash = new HashSet<Tuple<int,int>>();

            foreach (var item in vitals)
            {
                if(item.RecordDate == DateTime.MinValue)
                {
                    continue;
                }

                if(item.VitalType == QsDbVitalTypeEnum.Steps)
                {
                    yHash.Add(item.RecordDate.Year);
                }

                ymHash.Add(new Tuple<int, int>(item.RecordDate.Year, item.RecordDate.Month));
            }

            // 非同期で歩数値の月平均を登録
            if (yHash.Any())
            {
                _ = Task.Run(() =>
                {
                    Workers.HealthRecord.HealthRecordWorker.WriteMonthlyAverage(authorKey, accountKey, yHash.ToList());
                });
            }

            // 非同期でカロリー値の月平均を登録
            if (ymHash.Any())
            {                
                _ = Task.Run(() =>
                {
                      Workers.HealthRecord.HealthRecordWorker.WriteCalorieMonthlyAverage(
                          authorKey, accountKey, ymHash.ToList());
                });
            }         
        }

        /// <summary>
        /// 運動データをDBに書き込む
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="exercises"></param>
        public void WriteExercise(Guid accountKey, Guid authorKey, List<DbVitalValueItem> exercises)
        {
            var writer = new ExerciseEventWriter();
            var args = new ExerciseEventWriterArgs
            {
                ActorKey = accountKey,
                AuthorKey = authorKey,
                VitalValueN = exercises,
            };

            var result = QsDbManager.Write(writer, args);

            if (!result.IsSuccess)
            {
                throw new Exception();
            }

            if (result.Result != exercises.Count)
            {
                throw new Exception();
            }

        }

        /// <summary>
        /// バイタル値をStorageに書き込む
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="exercises"></param>
        public void WriteStorage(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vitals)
        {
            var pulseEntitis = new List<QhHealthRecordPulseTableEntity>();
            var metsEntitis = new List<QhHealthRecordMetsTableEntity>();
            var boEntitis = new List<QhHealthRecordBloodOxygenTableEntity>();

            foreach (var vital in vitals)
            {
                switch (vital.VitalType)
                {
                    //心拍
                    case QsDbVitalTypeEnum.Pulse:
                        var puls = new QhHealthRecordPulseTableEntity()
                        {
                            Value1 = (double)vital.Value1,
                            Value2 = (double)vital.Value2,
                            CreateDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };
                        puls.SetPartitionKey(accountKey);
                        puls.SetRowKey(vital.RecordDate);
                        pulseEntitis.Add(puls);
                        break;
                    //運動強度
                    case QsDbVitalTypeEnum.Mets:
                        var mets = new QhHealthRecordMetsTableEntity()
                        {
                            Value1 = (double)vital.Value1,
                            Value2 = (double)vital.Value2,
                            CreateDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };
                        mets.SetPartitionKey(accountKey);
                        mets.SetRowKey(vital.RecordDate);
                        metsEntitis.Add(mets);
                        break;
                    //Spo2
                    case QsDbVitalTypeEnum.BloodOxygen:
                        var bo = new QhHealthRecordBloodOxygenTableEntity()
                        {
                            Value1 = (double)vital.Value1,
                            Value2 = (double)vital.Value2,
                            CreateDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };
                        bo.SetPartitionKey(accountKey);
                        bo.SetRowKey(vital.RecordDate);
                        boEntitis.Add(bo);
                        break;
                    default:
                        break;
                }
            }
            if (pulseEntitis.Count > 0)
            {
                var storagePulseWriter = new HealthRecordTableEntityWriter<QhHealthRecordPulseTableEntity>();
                var storagePulseWriterArgs = new HealthRecordTableEntityWriterArgs<QhHealthRecordPulseTableEntity>()
                {
                    Entities = pulseEntitis
                };
                HealthRecordTableEntityWriterResults storagePulseWriterResults = QsAzureStorageManager.Write(storagePulseWriter, storagePulseWriterArgs);

                if (!storagePulseWriterResults.IsSuccess)
                {
                    throw new Exception();
                }

                if (storagePulseWriterResults.Result != pulseEntitis.Count)
                {
                    throw new Exception();
                }

            }

            if (metsEntitis.Count > 0)
            {
                var storageMetsWriter = new HealthRecordTableEntityWriter<QhHealthRecordMetsTableEntity>();
                var storageMetsWriterArgs = new HealthRecordTableEntityWriterArgs<QhHealthRecordMetsTableEntity>()
                {
                    Entities = metsEntitis
                };
                HealthRecordTableEntityWriterResults storageMetsWriterResults = QsAzureStorageManager.Write(storageMetsWriter, storageMetsWriterArgs);

                if (!storageMetsWriterResults.IsSuccess)
                {
                    throw new Exception();
                }

                if (storageMetsWriterResults.Result != metsEntitis.Count)
                {
                    throw new Exception();
                }

            }

            if (boEntitis.Count > 0)
            {
                var storageBloodOxygenWriter = new HealthRecordTableEntityWriter<QhHealthRecordBloodOxygenTableEntity>();
                var storageBloodOxygenWriterArgs = new HealthRecordTableEntityWriterArgs<QhHealthRecordBloodOxygenTableEntity>()
                {
                    Entities = boEntitis
                };
                HealthRecordTableEntityWriterResults storageBloodOxygenWriterResults = QsAzureStorageManager.Write(storageBloodOxygenWriter, storageBloodOxygenWriterArgs);

                if (!storageBloodOxygenWriterResults.IsSuccess)
                {
                    throw new Exception();
                }

                if (storageBloodOxygenWriterResults.Result != boEntitis.Count)
                {
                    throw new Exception();
                }

            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalTypes"></param>
        /// <returns></returns>
        public List<QH_HEALTHRECORD_DAT> ReadRange(Guid accountKey, DateTime fromDate, DateTime toDate, params byte[] vitalTypes)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", fromDate),
                    CreateParameter(con, "@p3", toDate),
                    CreateParameter(con, "@p4", string.Join(",",vitalTypes))
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_HEALTHRECORD_DAT)}
                    WHERE {nameof(QH_HEALTHRECORD_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_HEALTHRECORD_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_HEALTHRECORD_DAT.VITALTYPE)} IN (SELECT value FROM STRING_SPLIT(@p4,','))
                    AND   {nameof(QH_HEALTHRECORD_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                return ExecuteReader<QH_HEALTHRECORD_DAT>(con, null, sql, paramList);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="vitalType"></param>
        /// <returns></returns>
        public List<QH_HEALTHRECORD_DAT> ReadNew(Guid accountKey, DateTime toDate ,byte vitalType)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2",toDate),
                    CreateParameter(con, "@p3",vitalType)
                };

                var sql = $@"
                    SELECT TOP (1) *
                    FROM     {nameof(QH_HEALTHRECORD_DAT)}
                    WHERE    {nameof(QH_HEALTHRECORD_DAT.ACCOUNTKEY)} = @p1
                    AND      {nameof(QH_HEALTHRECORD_DAT.RECORDDATE)} <= @p2
                    AND      {nameof(QH_HEALTHRECORD_DAT.VITALTYPE)} = @p3
                    AND      {nameof(QH_HEALTHRECORD_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_HEALTHRECORD_DAT.RECORDDATE)} DESC

                ";

                con.Open();

                return ExecuteReader<QH_HEALTHRECORD_DAT>(con, null, sql, paramList);
            }
        }

        /// <summary>
        /// Azure Table Storageから心拍データを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="fromDate">取得開始日時</param>
        /// <param name="toDate">取得終了日時</param>
        /// <returns>バイタル値アイテムリスト</returns>
        public List<DbVitalValueItem> ReadStoragePulse(Guid accountKey, DateTime fromDate, DateTime toDate)
        {
            var reader = new HealthRecordTableEntityReader<QhHealthRecordPulseTableEntity>();
            var args = new HealthRecordTableEntityReaderArgs<QhHealthRecordPulseTableEntity>
            {
                AccountKey = accountKey,
                FromDate = fromDate,
                ToDate = toDate
            };

            var results = QsAzureStorageManager.Read(reader, args);

            if (!results.IsSuccess)
            {
                throw new Exception("Pulse データの取得に失敗しました。");
            }

            return results.Result.Select(e => new DbVitalValueItem
            {
                RecordDate = DateTime.ParseExact(e.RowKey, "yyyyMMddHHmmssfffffff", null),
                VitalType = QsDbVitalTypeEnum.Pulse,
                Value1 = (decimal)e.Value1,
                Value2 = decimal.MinusOne,
                Value3 = decimal.MinusOne,
                Value4 = decimal.MinusOne,
                ConditionType = (byte)QsDbVitalConditionTypeEnum.None
            }).ToList();
        }

        /// <summary>
        /// Azure Table Storageからメッツ(運動強度)データを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="fromDate">取得開始日時</param>
        /// <param name="toDate">取得終了日時</param>
        /// <returns>バイタル値アイテムリスト</returns>
        public List<DbVitalValueItem> ReadStorageMets(Guid accountKey, DateTime fromDate, DateTime toDate)
        {
            var reader = new HealthRecordTableEntityReader<QhHealthRecordMetsTableEntity>();
            var args = new HealthRecordTableEntityReaderArgs<QhHealthRecordMetsTableEntity>
            {
                AccountKey = accountKey,
                FromDate = fromDate,
                ToDate = toDate
            };

            var results = QsAzureStorageManager.Read(reader, args);

            if (!results.IsSuccess)
            {
                throw new Exception("Mets データの取得に失敗しました。");
            }

            return results.Result.Select(e => new DbVitalValueItem
            {
                RecordDate = DateTime.ParseExact(e.RowKey, "yyyyMMddHHmmssfffffff", null),
                VitalType = QsDbVitalTypeEnum.Mets,
                Value1 = (decimal)e.Value1,
                Value2 = decimal.MinusOne,
                Value3 = decimal.MinusOne,
                Value4 = decimal.MinusOne,
                ConditionType = (byte)QsDbVitalConditionTypeEnum.None
            }).ToList();
        }

        /// <summary>
        /// Azure Table Storageから血中酸素濃度データを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="fromDate">取得開始日時</param>
        /// <param name="toDate">取得終了日時</param>
        /// <returns>バイタル値アイテムリスト</returns>
        public List<DbVitalValueItem> ReadStorageBloodOxygen(Guid accountKey, DateTime fromDate, DateTime toDate)
        {
            var reader = new HealthRecordTableEntityReader<QhHealthRecordBloodOxygenTableEntity>();
            var args = new HealthRecordTableEntityReaderArgs<QhHealthRecordBloodOxygenTableEntity>
            {
                AccountKey = accountKey,
                FromDate = fromDate,
                ToDate = toDate
            };

            var results = QsAzureStorageManager.Read(reader, args);

            if (!results.IsSuccess)
            {
                throw new Exception("BloodOxygen データの取得に失敗しました。");
            }

            return results.Result.Select(e => new DbVitalValueItem
            {
                RecordDate = DateTime.ParseExact(e.RowKey, "yyyyMMddHHmmssfffffff", null),
                VitalType = QsDbVitalTypeEnum.BloodOxygen,
                Value1 = (decimal)e.Value1,
                Value2 = decimal.MinusOne,
                Value3 = decimal.MinusOne,
                Value4 = decimal.MinusOne,
                ConditionType = (byte)QsDbVitalConditionTypeEnum.None
            }).ToList();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<DbVitalValueItem> ReadExerciseRange(Guid accountKey, DateTime fromDate, DateTime toDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_EXERCISEEVENT2_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", fromDate),
                    CreateParameter(con, "@p3", toDate),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_EXERCISEEVENT2_DAT)}
                    WHERE {nameof(QH_EXERCISEEVENT2_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_EXERCISEEVENT2_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_EXERCISEEVENT2_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var records = ExecuteReader<QH_EXERCISEEVENT2_DAT>(con, null, sql, paramList);

                return records.Select(e => new DbVitalValueItem
                {
                    RecordDate = e.RECORDDATE,
                    VitalType = QsDbVitalTypeEnum.CalorieBurn,
                    Value1 = e.CALORIE,
                    Value2 = decimal.MinusOne,
                    Value3 = decimal.MinusOne,
                    Value4 = decimal.MinusOne,
                    ConditionType = (byte)QsDbVitalConditionTypeEnum.None
                }).ToList();
            }
        }

        /// <summary>
        /// バイタル値を削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="vital"></param>
        public void DeleteVitals(Guid accountKey, Guid authorKey, List<DbVitalValueItem> vitals)
        {
            var writer = new HealthRecordDeleteWriter();
            var args = new HealthRecordDeleteWriterArgs
            {
                ActorKey = accountKey,
                AuthorKey = authorKey,
                VitalValueN = vitals,
            };

            var result = QsDbManager.Write(writer, args);

            if (!result.IsSuccess)
            {
                throw new Exception();
            }

            if (result.Result != vitals.Count)
            {
                throw new Exception();
            }

        }

    }
}