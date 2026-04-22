using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 健康手帳のバイタル情報読み取りを処理します。
    /// 要テストコード 
    /// HealthRecordReadWorkerFixtureも併せて修正・追加すること
    /// </summary>
    public class HealthRecordReadWorker
    {
        readonly IHealthRecordRepository _repo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository"></param>
        public HealthRecordReadWorker(IHealthRecordRepository repository)
        {
            _repo = repository;
        }

        /// <summary>
        /// バイタル値を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthRecordReadApiResults Read(QoHealthRecordReadApiArgs args)
        {
            var results = new QoHealthRecordReadApiResults
            {
                IsSuccess = bool.FalseString,
                VitalDataN = new List<QhApiVitalValueItem>()
            };

            // 引数の検証
            if (!ValidateArguments(args, results, out var accountKey, out var fromDate, out var toDate))
            {
                return results;
            }

            // VitalTypeの分類
            if (!ClassifyVitalTypes(args.VitalTypes, results, 
                out var dbVitalTypes, out var needsPulse, out var needsMets, out var needsBloodOxygen,
                out var needsCalorieBurn))
            {
                return results;
            }

            // データ取得
            var allData = FetchVitalData(
                accountKey, fromDate, toDate,
                dbVitalTypes, needsPulse, needsMets, needsBloodOxygen, needsCalorieBurn,
                results);

            // エラーが発生していれば早期リターン
            if (results.IsSuccess != bool.TrueString &&
                results.Result != null &&
                !string.IsNullOrEmpty(results.Result.Code))
            {
                return results;
            }

            // 日付順にソート
            allData.Sort((a, b) => a.RecordDate.CompareTo(b.RecordDate));

            // API型に変換
            results.VitalDataN = ConvertToApiFormat(allData);
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        /// <summary>
        /// 引数のバリデーションを行います。
        /// </summary>
        private bool ValidateArguments(
            QoHealthRecordReadApiArgs args,
            QoHealthRecordReadApiResults results,
            out Guid accountKey,
            out DateTime fromDate,
            out DateTime toDate)
        {
            accountKey = Guid.Empty;
            fromDate = DateTime.MinValue;
            toDate = DateTime.MinValue;

            // null チェック
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // VitalTypesのデフォルト値設定（未設定の場合は全バイタルタイプを取得）
            if (args.VitalTypes == null || !args.VitalTypes.Any())
            {
                args.VitalTypes = Enum.GetValues(typeof(QsDbVitalTypeEnum))
                    .Cast<byte>()
                    .Where(v => v != (byte)QsDbVitalTypeEnum.None)
                    .ToList();
            }

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out accountKey))
            {
                return false;
            }

            // 日付範囲のバリデーション
            if (!DateTime.TryParse(args.FromDate, out fromDate))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FromDate/ToDateの形式が不正です。");
                return false;
            }

            // ToDateのデフォルト値設定（未設定の場合は現在時刻を使用）
            if (!DateTime.TryParse(args.ToDate, out toDate))
            {
                toDate = DateTime.Now;
            }

            if (fromDate > toDate)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FromDateはToDate以前の日時を指定してください。");
                return false;
            }

            var daysDiff = (toDate.Date - fromDate.Date).Days;
            if (daysDiff >= 7)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "取得期間は最大7日間です。");
                return false;
            }

            return true;
        }

        /// <summary>
        /// VitalTypeをDB用とStorage用に分類します。
        /// </summary>
        private bool ClassifyVitalTypes(
            List<byte> vitalTypes,
            QoHealthRecordReadApiResults results,
            out List<byte> dbVitalTypes,
            out bool needsPulse,
            out bool needsMets,
            out bool needsBloodOxygen,
            out bool needsCalorieBurn)
        {
            dbVitalTypes = new List<byte>();
            needsPulse = false;
            needsMets = false;
            needsBloodOxygen = false;
            needsCalorieBurn = false;

            foreach (var vitalType in vitalTypes)
            {
                switch ((QsDbVitalTypeEnum)vitalType)
                {
                    case QsDbVitalTypeEnum.Pulse:
                        needsPulse = true;
                        break;
                    case QsDbVitalTypeEnum.Mets:
                        needsMets = true;
                        break;
                    case QsDbVitalTypeEnum.BloodOxygen:
                        needsBloodOxygen = true;
                        break;
                    case QsDbVitalTypeEnum.CalorieBurn:
                        needsCalorieBurn = true;
                        break;
                    default:
                        dbVitalTypes.Add(vitalType);
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// DBおよびAzure Storageからバイタルデータを取得します。
        /// </summary>
        private List<DbVitalValueItem> FetchVitalData(
            Guid accountKey,
            DateTime fromDate,
            DateTime toDate,
            List<byte> dbVitalTypes,
            bool needsPulse,
            bool needsMets,
            bool needsBloodOxygen,
            bool needsCalorieBurn,
            QoHealthRecordReadApiResults results)
        {
            var allData = new List<DbVitalValueItem>();

            // DBからデータ取得
            if (dbVitalTypes.Any())
            {
                var dbData = _repo.ReadRange(accountKey, fromDate, toDate, dbVitalTypes.ToArray());
                var convertedData = ConvertDbRecordsToVitalItems(dbData);
                allData.AddRange(convertedData);
            }

            // Azure Storageからデータ取得
            if (needsPulse)
            {
                var pulseData = FetchStorageData(
                    () => _repo.ReadStoragePulse(accountKey, fromDate, toDate),
                    "Pulse",
                    accountKey,
                    results);
                if (pulseData == null) return allData;
                allData.AddRange(pulseData);
            }

            if (needsMets)
            {
                var metsData = FetchStorageData(
                    () => _repo.ReadStorageMets(accountKey, fromDate, toDate),
                    "Mets",
                    accountKey,
                    results);
                if (metsData == null) return allData;
                allData.AddRange(metsData);
            }

            if (needsBloodOxygen)
            {
                var bloodOxygenData = FetchStorageData(
                    () => _repo.ReadStorageBloodOxygen(accountKey, fromDate, toDate),
                    "BloodOxygen",
                    accountKey,
                    results);
                if (bloodOxygenData == null) return allData;
                allData.AddRange(bloodOxygenData);
            }

            if (needsCalorieBurn)
            {
                var calorieBurnData = FetchStorageData(
                    () => _repo.ReadExerciseRange(accountKey, fromDate, toDate),
                    "CalorieBurn",
                    accountKey,
                    results);
                if (calorieBurnData == null) return allData;
                allData.AddRange(calorieBurnData);
            }

            return allData;
        }

        /// <summary>
        /// DBレコードをDbVitalValueItemに変換します。
        /// </summary>
        private IEnumerable<DbVitalValueItem> ConvertDbRecordsToVitalItems(List<QH_HEALTHRECORD_DAT> dbRecords)
        {
            return dbRecords.Select(item => new DbVitalValueItem
            {
                RecordDate = item.RECORDDATE,
                VitalType = (QsDbVitalTypeEnum)item.VITALTYPE,
                Value1 = item.VALUE1,
                Value2 = item.VALUE2,
                Value3 = decimal.MinusOne,
                Value4 = decimal.MinusOne,
                ConditionType = item.CONDITIONTYPE
            });
        }

        /// <summary>
        /// Azure Storageからデータを取得します（エラーハンドリング付き）。
        /// </summary>
        private List<DbVitalValueItem> FetchStorageData(
            Func<List<DbVitalValueItem>> fetchFunc,
            string dataType,
            Guid accountKey,
            QoHealthRecordReadApiResults results)
        {
            try
            {
                return fetchFunc();
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog($"{dataType} データ取得エラー: {ex.Message}", accountKey);
                results.Result = QoApiResult.Build(ex, $"{dataType}データの取得に失敗しました。");
                return null;
            }
        }
        
        /// <summary>
        /// DbVitalValueItemをAPI形式に変換します。
        /// </summary>
        private List<QhApiVitalValueItem> ConvertToApiFormat(List<DbVitalValueItem> vitalData)
        {
            return vitalData.Select(item => new QhApiVitalValueItem
            {
                RecordDate = item.RecordDate.ToString("yyyy/MM/dd HH:mm:ss"),
                VitalType = ((byte)item.VitalType).ToString(),
                Value1 = item.Value1 >= 0 ? item.Value1.ToString("G29") : string.Empty,
                Value2 = item.Value2 > 0 ? item.Value2.ToString("G29") : string.Empty,
                Value3 = item.Value3 > 0 ? item.Value3.ToString("G29") : string.Empty,
                Value4 = item.Value4 > 0 ? item.Value4.ToString("G29") : string.Empty,
                ConditionType = item.ConditionType > 0 ? item.ConditionType.ToString() : string.Empty
            }).ToList();
        }
    }
}
