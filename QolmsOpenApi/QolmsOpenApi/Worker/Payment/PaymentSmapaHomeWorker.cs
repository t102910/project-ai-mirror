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
    /// SmapaHome取得処理
    /// </summary>
    public class PaymentSmapaHomeWorker
    {
        ISmapaApiRepository _smapaApiRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PaymentSmapaHomeWorker(ISmapaApiRepository smapaApiRepository)
        {
            _smapaApiRepo = smapaApiRepository;
        }

        /// <summary>
        /// Smapa Home Url取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoPaymentSmapaHomeApiResults> GetSmapaHome(QoPaymentSmapaHomeApiArgs args)
        {
            var results = new QoPaymentSmapaHomeApiResults
            {
                IsSuccess = bool.FalseString
            };

            // SmapaAPIよりURLを取得
            var (isSuccess, url) = await TryGetUrl(args, results).ConfigureAwait(false);
            if (!isSuccess)
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.Url = url;

            return results;
        }


        async Task<(bool isSuccess, string url)> TryGetUrl(QoPaymentSmapaHomeApiArgs args, QoApiResultsBase results)
        {
            try
            {
                var smapaResults = await _smapaApiRepo.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiHomeUrl).ConfigureAwait(false);

                if(smapaResults.Error != "0")
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, smapaResults.ErrorDetail, "SmapaApi内部エラーです。" + smapaResults.ErrorDetail);
                    return (false, string.Empty);
                }

                if (string.IsNullOrEmpty(smapaResults.Url))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "SmapaWebのURLが取得できませんでした。");
                    return (false, string.Empty);
                }

                return (true, smapaResults.Url);
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "Smapa API 接続処理でエラーが発生しました。");
                return (false, string.Empty);
            }
        }

        
    }
}