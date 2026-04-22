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
    /// 今日の健康情報集計処理
    /// </summary>
    public class HealthRecordTodaySummaryWorker
    {
        IHealthRecordRepository _healthRecordRepo;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="healthRecordRepository"></param>
        public HealthRecordTodaySummaryWorker(IHealthRecordRepository healthRecordRepository)
        {
            _healthRecordRepo = healthRecordRepository;
        }

        /// <summary>
        /// 今日の健康情報集計処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthRecordTodaySummaryApiResults ReadSummary(QoHealthRecordTodaySummaryApiArgs args)
        {
            var results = new QoHealthRecordTodaySummaryApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // バイタルタイプのチェック
            if(!CheckVitalTypes(args.VitalTypes,results))
            {
                return results;
            }

            //　対象バイタルデータの取得
            if(!TryReadVitalData(accountKey, args.VitalTypes,args.AverageDays,results, out var entityList))
            {
                return results;
            }

            // 集計処理
            if(!TryCalculateSummaryData(entityList, args.VitalTypes,results,out var summaryDict))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.SummaryList = summaryDict.Select(x => x.Value).ToList();

            return results;
        }

        bool CheckVitalTypes(List<byte> vitalTypes, QoApiResultsBase results)
        {
            if (!vitalTypes.Any())
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "VitalTypeが指定されていません。");
                return false;
            }

            foreach (var vitalNum in vitalTypes)
            {
                var vitalType = (QsDbVitalTypeEnum)vitalNum;

                switch (vitalType)
                {
                    case QsDbVitalTypeEnum.Steps:
                    case QsDbVitalTypeEnum.SleepingTime:
                        break;
                    default:
                        // 歩数・睡眠以外は対象外
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "VitalTypeが対応外です。");
                        return false;
                }
            }

            return true;
        }

        bool TryReadVitalData(Guid accountKey,List<byte> vitals, int days,QoApiResultsBase results, out List<QH_HEALTHRECORD_DAT> entityList)
        {
            entityList = new List<QH_HEALTHRECORD_DAT>();

            try
            {
                var fromDate = DateTime.Today.AddDays(-days + 1);
                var toDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);

                entityList = _healthRecordRepo.ReadRange(accountKey, fromDate, toDate, vitals.ToArray());

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "バイタル取得処理でエラーが発生しました。");
                return false;
            }
        }

        bool TryCalculateSummaryData(List<QH_HEALTHRECORD_DAT> entityList, List<byte> vitals, QoApiResultsBase results, out Dictionary<byte,QoHealthSummaryItem> summaryDict)
        {
            summaryDict = new Dictionary<byte, QoHealthSummaryItem>();
            try
            {
                // 結果用Dictionary初期化
                foreach (var vital in vitals)
                {
                    summaryDict[(byte)vital] = new QoHealthSummaryItem
                    {
                        VitalType = (byte)vital,
                    };
                }

                // 当日集計値
                var todayGroup = entityList
                    .Where(x => x.RECORDDATE.Date == DateTime.Today)
                    .GroupBy(y => y.VITALTYPE);

                foreach (var group in todayGroup)
                {
                    var summary = summaryDict[group.Key];
                    summary.TodayValue = (double)group.Sum(x => x.VALUE1);
                }

                // 前日集計値
                var dayBeforeGroup = entityList
                    .Where(x => x.RECORDDATE.Date == DateTime.Today.AddDays(-1))
                    .GroupBy(y => y.VITALTYPE);

                foreach (var group in dayBeforeGroup)
                {
                    var summary = summaryDict[group.Key];
                    summary.DayBeforeValue = (double)group.Sum(x => x.VALUE1);
                }

                // 平均値
                var averageGroup = entityList
                    .GroupBy(x => x.VITALTYPE);

                foreach (var group in averageGroup)
                {                    
                    var summary = summaryDict[group.Key];

                    // データの日数(データの無い日はカウントしない方式)
                    var days = group.GroupBy(x => x.RECORDDATE.Date).Count();
                    summary.Average =  (double)group.Sum(x => x.VALUE1) / days;
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "サマリー処理でエラーが発生しました。");
                return false;
            }
        }
    }
}