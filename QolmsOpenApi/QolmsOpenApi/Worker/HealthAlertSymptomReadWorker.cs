using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アラートと症状情報を読み込む
    /// </summary>
    public class HealthAlertSymptomReadWorker
    {
        IHealthRecordAlertRepository _healthAlertRepo;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="healthRecordAlertRepository"></param>
        public HealthAlertSymptomReadWorker(IHealthRecordAlertRepository healthRecordAlertRepository)
        {
            _healthAlertRepo = healthRecordAlertRepository;
        }

        /// <summary>
        /// 読み込み処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthAlertSymptomReadApiResults Read(QoHealthAlertSymptomReadApiArgs args)
        {
            var results = new QoHealthAlertSymptomReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 開始日チェック
            if(!args.FromDate.CheckArgsConvert(nameof(args.FromDate),DateTime.MinValue, results, out var fromDate))
            {
                return results;
            }

            // 終了日チェック
            var toDate = args.ToDate.TryToValueType(DateTime.MinValue);
            if(toDate == DateTime.MinValue)
            {
                // 未指定または不正値の場合は明日にしておく
                toDate = DateTime.Today.AddDays(1);
            }

            // 連携システム番号に変換
            var linkageSystemNo = args.ExecuteSystemType.ToLinkageSystemNo();

            // データ取得
            if(!TryReadView(accountKey, linkageSystemNo, fromDate, toDate, results,out var entityList))
            {
                return results;
            }

            // データ変換
            if(!TryConvertEntityList(entityList, results, out var apiItems))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.ItemList = apiItems;

            return results;
        }

        bool TryReadView(Guid accountKey, int linkageSystemNo, DateTime fromDate, DateTime toDate,QoApiResultsBase results,  out List<QH_HEALTH_ALERT_SYMPTOM_VIEW> alertSymptomList)
        {
            try
            {
                alertSymptomList = _healthAlertRepo.ReadAlertSymptomView(accountKey, linkageSystemNo, fromDate, toDate);

                return true;
            }
            catch(Exception ex)
            {
                alertSymptomList = null;
                results.Result = QoApiResult.Build(ex, "アラートと症状のリストの取得処理に失敗しました。");
                return false;
            }
        }

        bool TryConvertEntityList(List<QH_HEALTH_ALERT_SYMPTOM_VIEW> entityList, QoApiResultsBase results, out List<QoHealthAlertSymptomItem> apiItems)
        {
            apiItems = null;
            try
            {
                apiItems = entityList.ConvertAll(x => new QoHealthAlertSymptomItem
                {
                    AlertNo = x.ALERTNO,
                    SymptomId = x.SYMPTOMID,
                    RecordDate = x.RECORDDATE.ToApiDateString(),
                    DataType = (QoApiHealthDataTypeEnum)x.DATATYPE,
                    VitalType = x.VITALTYPE,
                    Value1 = x.VALUE1,
                    Value2 = x.VALUE2,
                    AbnormalType = x.ABNORMALTYPE
                });

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "Entityの変換処理に失敗しました。");
                return false;
            }
        }
    }
}