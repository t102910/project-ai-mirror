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
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Smapa退会処理
    /// </summary>
    public class PaymentSmapaRevokeWorker
    {
        ISmapaApiRepository _smapaApiRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="smapaApiRepository"></param>
        public PaymentSmapaRevokeWorker(ISmapaApiRepository smapaApiRepository)
        {
            _smapaApiRepo = smapaApiRepository;
        }

        /// <summary>
        /// 退会処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoPaymentSmapaRevokeApiResults> Revoke(QoPaymentSmapaRevokeApiArgs args)
        {
            var results = new QoPaymentSmapaRevokeApiResults
            {
                IsSuccess = bool.FalseString
            };
            
            if(!await TryRevoke(args, results).ConfigureAwait(false))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        async Task<bool> TryRevoke(QoPaymentSmapaRevokeApiArgs args, QoPaymentSmapaRevokeApiResults results)
        {
            try
            {
                var smapaResults = await _smapaApiRepo.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiRevokeUrl).ConfigureAwait(false);

                if (smapaResults.Error != "0")
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "SmapaApi内部エラーです。");
                    results.IsSmapaSuccess = false;
                    results.SmapaErrorCode = smapaResults.ErrorFlag;
                    results.SmapaErrorDetail = smapaResults.ErrorDetail;
                    return false;
                }

                results.IsSmapaSuccess = true;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "Smapa API 接続処理でエラーが発生しました。");
                results.IsSmapaSuccess = false;
                return false;
            }
        }
    }
}