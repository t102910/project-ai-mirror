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
    /// 健康情報症状書き込み処理
    /// </summary>
    public class HealthSymptomWriteWorker
    {
        ISymptomRepository _symptomRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symptomRepository"></param>
        public HealthSymptomWriteWorker(ISymptomRepository symptomRepository)
        {
            _symptomRepo = symptomRepository;
        }

        /// <summary>
        /// 書き込み処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthSymptomWriteApiResults Write(QoHealthSymptomWriteApiArgs args)
        {
            var results = new QoHealthSymptomWriteApiResults
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

            // 記録日時変換チェック
            if(!args.RecordDate.CheckArgsConvert(nameof(args.RecordDate),DateTime.MinValue,results, out var recordDate))
            {
                return results;
            }

            // 症状リストのチェック
            foreach(var symptom in args.Symptoms)
            {
                if (!symptom.IsDefined<QsDbSymptomTypeEnum>())
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "症状の値が不正です。");
                    return results;
                }
            }

            // 症状が1つ以上あるか
            if (!args.Symptoms.Any())
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "症状が設定されていません。");
                return results;
            }

            // 症状リストを重複排除してJson化
            var json = new QsJsonSerializer().Serialize(args.Symptoms.Distinct().ToList());

            var entity = new QH_SYMPTOM_DAT
            {
                ID = args.Id,
                ACCOUNTKEY = accountKey,
                RECORDDATE = recordDate,
                LINKAGESYSTEMNO = linkageSystemNo,
                SYMPTOMS = json,
                OTHERDETAIL = args.OtherDetail,
                MEMO = args.Memo,
            };

            // 書き込み処理
            if(!TryWriteSymptiom(entity, results))
            {
                return results;
            }

            results.Id = entity.ID; // 処理対象IDを返す
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;            
        }

        bool TryWriteSymptiom(QH_SYMPTOM_DAT entity, QoApiResultsBase results)
        {
            try
            {
                if (entity.ID == Guid.Empty)
                {
                    // 新規
                    entity.ID = Guid.NewGuid(); // ID生成
                    _symptomRepo.InsertEntity(entity);
                }
                else
                {
                    // 更新
                    _symptomRepo.UpdateEntity(entity);
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "症状の書き込み処理でエラーが発生しました。");
                return false;
            }
        }
    }

    
}