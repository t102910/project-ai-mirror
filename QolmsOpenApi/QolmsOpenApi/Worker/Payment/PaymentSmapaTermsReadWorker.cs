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
    /// Smapa利用規約同意状況取得処理
    /// </summary>
    public class PaymentSmapaTermsReadWorker
    {
        private IExternalServiceLinkageRepository _externalServiceRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="externalServiceLinkageRepository"></param>
        public PaymentSmapaTermsReadWorker(IExternalServiceLinkageRepository externalServiceLinkageRepository)
        {
            _externalServiceRepo = externalServiceLinkageRepository;
        }

        /// <summary>
        /// Read処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoPaymentSmapaTermsReadApiResults Read(QoPaymentSmapaTermsReadApiArgs args)
        {
            var results = new QoPaymentSmapaTermsReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // TargetServiceTypeチェック
            if(args.TargetServiceType == 0)
            {
                // TargetTypeが未指定の場合はExecuteSystemTypeからOSを問わないタイプのアプリケーションタイプに変換したものを指定する
                args.TargetServiceType = (int)args.ExecuteSystemType.ToApplicationType();
            }

            if(!TryReadTermsAccepted(accountKey, args, results, out var isTermsAccepted))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsTermsAccepted = isTermsAccepted;

            return results;
        }
                
        bool TryReadTermsAccepted(Guid accountKey, QoPaymentSmapaTermsReadApiArgs args, QoApiResultsBase results, out bool isTermsAccepted)
        {
            isTermsAccepted = false;
            try
            {
                // 1:アルメックス固定
                var entity = _externalServiceRepo.ReadEntity(accountKey, 1, args.TargetServiceType);
                if(entity == null)
                {
                    // 該当レコードが無かった場合は利用規約未同意とする
                    return true;
                }
                if (string.IsNullOrEmpty(entity.VALUE))
                {
                    // VALUE列が空の場合も利用規約未同意とする
                    return true;
                }

                var json = new QsJsonSerializer().Deserialize<QhExternalServiceLinkageOfJson>(entity.VALUE);
                if(json == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "外部サービス連携情報のVALUEの取得に失敗しました。");
                    return false;
                }

                isTermsAccepted = json.IsTermsAccepted;

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "外部サービス連携情報の取得に失敗しました。");
                return false;
            }
        }
    }
}