using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsCryptV1;
using System.Net.Http;
using Newtonsoft.Json;
using MGF.QOLMS.QolmsOpenApi.Models;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Smapa利用規約同意状況更新処理
    /// </summary>
    public class PaymentSmapaTermsWriteWorker
    {
        private IExternalServiceLinkageRepository _externalServiceRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="externalServiceLinkageRepository"></param>
        public PaymentSmapaTermsWriteWorker(IExternalServiceLinkageRepository externalServiceLinkageRepository)
        {
            _externalServiceRepo = externalServiceLinkageRepository;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoPaymentSmapaTermsWriteApiResults Write(QoPaymentSmapaTermsWriteApiArgs args)
        {
            var results = new QoPaymentSmapaTermsWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // TargetServiceTypeチェック
            if (args.TargetServiceType == 0)
            {
                // TargetTypeが未指定の場合はExecuteSystemTypeからOSを問わないタイプのアプリケーションタイプに変換したものを指定する
                args.TargetServiceType = (int)args.ExecuteSystemType.ToApplicationType();
            }

            // 読み取りから更新までトランザクション処理
            using (var tran = new QoTransaction())
            {
                // 最新情報を取得
                if (!TryReadEntity(accountKey, args, results, out var entity))
                {
                    return results;
                }

                QhExternalServiceLinkageOfJson json;
                // VALUEが空だったら新規Jsonを設定
                if (string.IsNullOrEmpty(entity.VALUE))
                {                    
                    json = new QhExternalServiceLinkageOfJson
                    {
                        IsTermsAccepted = false,
                        FacilityList = new List<QhExternalServiceFacilityItem>()
                    };                    
                }
                // それ以外はJsonパース
                else
                {
                    json = new QsJsonSerializer().Deserialize<QhExternalServiceLinkageOfJson>(entity.VALUE);
                    if (json == null)
                    {
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "外部サービス連携情報のVALUEの取得に失敗しました。");
                        return results;
                    }
                }

                // 引数の同意フラグをセット
                json.IsTermsAccepted = args.IsTermsAccepted;

                // Jsonシリアライズ
                var jsonString = new QsJsonSerializer().Serialize(json);
                entity.VALUE = jsonString;

                // 変更を更新
                if (!TryWriteEntity(entity, results))
                {
                    return results;
                }

                // トランザクション コミット
                tran.Commit();

                // 成功
                results.IsSuccess = bool.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return results;
            }                
        }

        bool TryReadEntity(Guid accountKey, QoPaymentSmapaTermsWriteApiArgs args, QoApiResultsBase results, out QH_EXTERNALSERVICELINKAGE_DAT entity)
        {
            entity = null;
            try
            {
                // 1:アルメックス固定
                entity = _externalServiceRepo.ReadEntity(accountKey, 1, args.TargetServiceType);  
                if(entity == null)
                {
                    // 該当レコードがなければ新規作成
                    entity = new QH_EXTERNALSERVICELINKAGE_DAT
                    {
                        ACCOUNTKEY = accountKey,
                        EXTERNALSERVICETYPE = 1,
                        TARGETSERVICETYPE = args.TargetServiceType,
                        VALUE = string.Empty
                    };
                }
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "外部サービス連携情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryWriteEntity(QH_EXTERNALSERVICELINKAGE_DAT entity, QoApiResultsBase results)
        {
            try
            {
                _externalServiceRepo.UpsertEntity(entity);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "外部サービス連携情報の更新に失敗しました。");
                return false;
            }
        }
    }
}