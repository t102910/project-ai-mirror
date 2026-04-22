using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 健康情報アラート書き込み処理
    /// </summary>
    public class HealthAlertWriteWorker
    {
        IHealthRecordAlertRepository _healthAlertRepo;
        IHealthRecordValidator _validator;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="healthRecordAlertRepository"></param>
        /// <param name="healthRecordValidator"></param>
        public HealthAlertWriteWorker(
            IHealthRecordAlertRepository healthRecordAlertRepository,
            IHealthRecordValidator healthRecordValidator)
        {
            _healthAlertRepo = healthRecordAlertRepository;
            _validator = healthRecordValidator;
        }


        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthAlertWriteApiResults Write(QoHealthAlertWriteApiArgs args)
        {
            var results = new QoHealthAlertWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // SystemTypeからLinkageSystemNoに変換
            var linkageSystemNo = args.ExecuteSystemType.ToLinkageSystemNo();

            var entityList = new List<QH_HEALTHRECORDALERT_DAT>();
            foreach(var item in args.HealthAlertItemN)
            {
                // RecordDate変換チェック
                if (!item.RecordDate.CheckArgsConvert(nameof(item.RecordDate), DateTime.MinValue, results, out var recordDate))
                {
                    return results;
                }
                // 秒以下を切り捨てる
                recordDate = recordDate.AddTicks(recordDate.Ticks % TimeSpan.TicksPerMinute * -1);

                // VitalTypeの変換チェック
                if (!item.VitalType.CheckArgsEnumConvert(nameof(item.VitalType), QsDbVitalTypeEnum.None, results, out var vitalType))
                {
                    return results;
                }

                // AbnormalTypeの変換チェック
                if (!item.AbnormalType.CheckArgsEnumConvert(nameof(item.AbnormalType), QsDbVitalAbnormalTypeEnum.None, results, out var abnormalType))
                {
                    return results;
                }

                // Vitalバリデーション
                var (isValid, error) = _validator.ValidateForAlert(vitalType, item.Value1, item.Value2);
                if (!isValid)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, error);
                    return results;
                }

                var entity = new QH_HEALTHRECORDALERT_DAT
                {
                    ACCOUNTKEY = accountKey,
                    RECORDDATE = recordDate,
                    VITALTYPE = (byte)vitalType,
                    LINKAGESYSTEMNO = linkageSystemNo,
                    VALUE1 = item.Value1,
                    VALUE2 = item.Value2,
                    ABNORMALTYPE = (byte)abnormalType,
                    // 警告固定
                    EMERGENCYTYPE = (byte)QsDbVitalEmergencyTypeEnum.Warning,
                    // JotoHeartRate固定
                    ALERTTYPE = (byte)QsDbVitalAlertTypeEnum.JotoHeartRate,
                    MESSAGE = item.Message
                };

                entityList.Add(entity);
            }

            // 書き込み処理            
            if(!TryWriteAlert(entityList, results))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryWriteAlert(List<QH_HEALTHRECORDALERT_DAT> entityList,QoApiResultsBase results)
        {
            var orderList = entityList.OrderBy(x => x.RECORDDATE);
            try
            {
                // トランザクションスコープを貼ってAlerNoの一貫性を保つ
                using(var trans = new QoTransaction())
                {
                    foreach(var entity in orderList)
                    {
                        // AlertNo採番
                        var alertNo = _healthAlertRepo.GetNewAlertNo();
                        entity.ALERTNO = alertNo;
                        _healthAlertRepo.InsertEntity(entity);
                    }                    

                    trans.Commit();
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アラート書き込み処理でエラーが発生しました。");
                return false;
            }
        }
    }

    
}