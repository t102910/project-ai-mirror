using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 健康手帳のバイタル情報を処理します。
    /// 要テストコード 
    /// HealthRecordWorkerFixtureも併せて修正・追加すること
    /// </summary>
    public class HealthRecordWorker
    {
        readonly IHealthRecordRepository _repo;
        readonly IHealthRecordValidator _validator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="validator"></param>
        public HealthRecordWorker(IHealthRecordRepository repository, IHealthRecordValidator validator)
        {
            _repo = repository;
            _validator = validator;
        }

        /// <summary>
        /// バイタル値を登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthRecordImportApiResults Import(QoHealthRecordImportApiArgs args)
        {
            var result = new QoHealthRecordImportApiResults
            {
                IsSuccess = bool.FalseString
            };
            
            // 引数チェック
            if (args == null)
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrWhiteSpace(args.ActorKey))
            {
                throw new ArgumentException();
            }
            if (args.VitalValueN == null || !args.VitalValueN.Any())
            {
                throw new ArgumentException();
            }
            if (string.IsNullOrWhiteSpace(args.Executor))
            {
                throw new ArgumentException();
            }

            foreach (var c in args.VitalValueN)
            {
                QoAccessLog.WriteInfoLog($"-=-=-=-=-=-=-=-=-=-=-");
                QoAccessLog.WriteInfoLog($"{c.RecordDate}");
                QoAccessLog.WriteInfoLog($"{c.VitalType}");
                QoAccessLog.WriteInfoLog($"{c.Value1}");
            }

            // バイタル値チェック
            var (isValid, error) = _validator.Validate(args.VitalValueN);
            if (!isValid)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, error);
                return result;
            }

            var typedVitalValueN = args.VitalValueN.Select(x => new DbVitalValueItem
            {
                RecordDate = x.RecordDate.TryToValueType(DateTime.MinValue),
                VitalType = (QsDbVitalTypeEnum)x.VitalType.TryToValueType(byte.MinValue),
                Value1 = x.Value1.TryToValueType(decimal.MinusOne),
                Value2 = x.Value2.TryToValueType(decimal.MinusOne),
                Value3 = x.Value3.TryToValueType(decimal.MinusOne),
                Value4 = x.Value4.TryToValueType(decimal.MinusOne),
                ConditionType = x.ConditionType.TryToValueType(byte.MinValue)
            });

            var sortedList = new List<DbVitalValueItem>();

            var sortedExerciseList = new List<DbVitalValueItem>();

            var sortedStorageList = new List<DbVitalValueItem>();
            foreach (var b in typedVitalValueN)
            {
                QoAccessLog.WriteInfoLog($"-=-=-=-=-=-=-=-=-=-=-");
                QoAccessLog.WriteInfoLog($"{b.RecordDate}");
                QoAccessLog.WriteInfoLog($"{b.VitalType}");
                QoAccessLog.WriteInfoLog($"{b.Value1}");
            }
            foreach (var item in typedVitalValueN)
            {
                // 無効なタイプはスキップする
                if (item.VitalType == QsDbVitalTypeEnum.None)
                {
                    continue;
                }
                // データ整形
                foreach(var formattedItem in FormatVitalValueItem(item))
                {
                    switch (formattedItem.VitalType)
                    {
                        //QH_EXERCISEEVENT2_DATへ
                        case QsDbVitalTypeEnum.CalorieBurn:
                            sortedExerciseList.Add(formattedItem);
                            break;
                        //AzureStorageへ
                        case QsDbVitalTypeEnum.Pulse:
                        case QsDbVitalTypeEnum.Mets:
                        case QsDbVitalTypeEnum.BloodOxygen:
                            sortedStorageList.Add(formattedItem);
                            break;
                        //QH_HEALTHRECORD_DATへ
                        case QsDbVitalTypeEnum.BodyWeight:
                        case QsDbVitalTypeEnum.BloodPressure:
                        case QsDbVitalTypeEnum.BloodSugar:
                        default:
                          sortedList.Add(formattedItem);
                          break;
                    }
                }
            }

            // 日付順にソートする
            sortedList.Sort(new Comparison<DbVitalValueItem>((a, b) =>
            {
                if (a.RecordDate < b.RecordDate)
                {
                    return -1;
                }
                else if(a.RecordDate > b.RecordDate)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }));

            sortedExerciseList.Sort(new Comparison<DbVitalValueItem>((a, b) =>
            {
                if (a.RecordDate < b.RecordDate)
                {
                    return -1;
                }
                else if (a.RecordDate > b.RecordDate)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }));

            sortedStorageList.Sort(new Comparison<DbVitalValueItem>((a, b) =>
            {
                if (a.RecordDate < b.RecordDate)
                {
                    return -1;
                }
                else if (a.RecordDate > b.RecordDate)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }));

            if (!sortedList.Any() && !sortedExerciseList.Any() && !sortedStorageList.Any())
            {
                // 処理対象0件 正常とする
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return result;
            }
            // 秒以下を切り捨てた状態で主キーでグループ化
            var groupedList = sortedList.GroupBy(x => new
            {
                date = x.RecordDate.AddTicks(x.RecordDate.Ticks % TimeSpan.TicksPerMinute * -1),
                viatlType = x.VitalType
            });

            var groupedExerciseList = sortedExerciseList.GroupBy(x => new
            {
                date = x.RecordDate.AddTicks(x.RecordDate.Ticks % TimeSpan.TicksPerMinute * -1),
                viatlType = x.VitalType
            });

            var groupedStorageList = sortedStorageList.GroupBy(x => new
            {
                date = x.RecordDate.AddTicks(x.RecordDate.Ticks % TimeSpan.TicksPerMinute * -1),
                viatlType = x.VitalType
            });

            var distinctList = new List<DbVitalValueItem>();
            foreach(var group in groupedList)
            {
                // 分単位で重複しているデータは後勝ちとする
                var latest = group.Last();
                // 秒以下を切り捨てた値で更新
                latest.RecordDate = group.Key.date;
                distinctList.Add(latest);
            }

            var distinctExerciseList = new List<DbVitalValueItem>();
            foreach (var group in groupedExerciseList)
            {
                // 分単位で重複しているデータは後勝ちとする
                var latest = group.Last();
                // 秒以下を切り捨てた値で更新
                latest.RecordDate = group.Key.date;
                distinctExerciseList.Add(latest);
            }

            var distinctStorageList = new List<DbVitalValueItem>();
            foreach (var group in groupedStorageList)
            {
                // 分単位で重複しているデータは後勝ちとする
                var latest = group.Last();
                // 秒以下を切り捨てた値で更新
                latest.RecordDate = group.Key.date;
                distinctStorageList.Add(latest);
            }
            var accountKey = Guid.Parse(args.ActorKey);
            var authorKey = Guid.Parse(args.AuthorKey);
            try
            {
                QoAccessLog.WriteInfoLog($"{distinctList.Count}");
                foreach (var a in distinctList)
                {
                    QoAccessLog.WriteInfoLog($"-=-=-=-=-=-=-=-=-=-=-");
                    QoAccessLog.WriteInfoLog($"{a.RecordDate}");
                    QoAccessLog.WriteInfoLog($"{a.VitalType}");
                    QoAccessLog.WriteInfoLog($"{a.Value1}");
                }
                if (distinctList.Count > 0)
                {
                    _repo.WriteVitals(accountKey, authorKey, distinctList);

                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog($"{ex.Message}");
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError,"バイタル値のDB登録に失敗しました。");
                return result;
            }

            try
            {
                if (distinctExerciseList.Count > 0)
                {
                    _repo.WriteExercise(accountKey, authorKey, distinctExerciseList);

                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "運動値のDB登録に失敗しました。");
                return result;
            }

            try
            {
                if (distinctStorageList.Count > 0)
                {
                    _repo.WriteStorage(accountKey, authorKey, distinctStorageList);

                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex.Message,Guid.Empty);
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "AzureStorageへの登録に失敗しました。");
                return result;
            }

            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return result;
        }

        IEnumerable<DbVitalValueItem> FormatVitalValueItem(DbVitalValueItem item)
        {
            switch (item.VitalType)
            {
                case QsDbVitalTypeEnum.BodyWeight:
                    // 体重項目は体重と身長に分割する
                    var weightItem = new DbVitalValueItem
                    {
                        RecordDate = item.RecordDate,
                        VitalType = QsDbVitalTypeEnum.BodyWeight,
                        Value1 = item.Value1,
                        Value2 = decimal.MinusOne,
                        Value3 = decimal.MinusOne,
                        Value4 = decimal.MinusOne,
                        ConditionType = (byte)QsDbVitalConditionTypeEnum.None
                    };

                    yield return weightItem;

                    var heightItem = new DbVitalValueItem
                    {
                        RecordDate = item.RecordDate,
                        VitalType = QsDbVitalTypeEnum.BodyHeight,
                        Value1 = item.Value1 > 0 ? item.Value2 : decimal.MinusOne,
                        Value2 = decimal.MinusOne,
                        Value3 = decimal.MinusOne,
                        Value4 = decimal.MinusOne,
                        ConditionType = (byte)QsDbVitalConditionTypeEnum.None
                    };

                    yield return heightItem;
                    break;

                case QsDbVitalTypeEnum.BloodPressure:
                    item.Value3 = decimal.MinusOne;
                    item.Value4 = decimal.MinusOne;
                    item.ConditionType = (byte)QsDbVitalConditionTypeEnum.None;
                    yield return item;
                    break;

                case QsDbVitalTypeEnum.BloodSugar:
                    item.Value2 = decimal.MinusOne;
                    item.Value3 = decimal.MinusOne;
                    item.Value4 = decimal.MinusOne;
                    yield return item;
                    break;

                case QsDbVitalTypeEnum.Pulse:
                    item.Value2 = decimal.MinusOne;
                    item.Value3 = decimal.MinusOne;
                    item.Value4 = decimal.MinusOne;
                    item.ConditionType = (byte)QsDbVitalConditionTypeEnum.None;
                    yield return item;
                    break;

                case QsDbVitalTypeEnum.CalorieBurn:
                    item.Value2 = decimal.MinusOne;
                    item.Value3 = decimal.MinusOne;
                    item.Value4 = decimal.MinusOne;
                    item.ConditionType = (byte)QsDbVitalConditionTypeEnum.None;
                    yield return item;
                    break;

                default:
                    item.Value2 = decimal.MinusOne;
                    item.Value3 = decimal.MinusOne;
                    item.Value4 = decimal.MinusOne;
                    item.ConditionType = (byte)QsDbVitalConditionTypeEnum.None;
                    yield return item;
                    break;
            }
        }
    }
}