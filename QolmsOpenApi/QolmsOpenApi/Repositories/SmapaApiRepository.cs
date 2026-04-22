using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// Smapa Api に接続するためのリポジトリ
    /// </summary>
    public interface ISmapaApiRepository
    {
        /// <summary>
        /// SmapaApiを実行する
        /// </summary>
        /// <param name="hospCode">医療機関コード</param>
        /// <param name="linkageSystemId">連携システムID(診察券番号・患者ID)</param>
        /// <param name="url">APIのURL</param>
        /// <returns></returns>
        Task<SmapaApiResults> ExecuteSmapaApi(string hospCode, string linkageSystemId, string url);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class SmapaApiRepository: ISmapaApiRepository
    {
        IQoHttpClientFactory _httpClientFactory;
        IDailySequenceRepository _dailySequenceRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SmapaApiRepository()
        {
            _httpClientFactory = new QoHttpClientFactory();
            _dailySequenceRepo = new DailySequenceRepository();
        }

        /// <inheritdoc/>
        public async Task<SmapaApiResults> ExecuteSmapaApi(string hospCode, string linkageSystemId, string url)
        {
            var client = _httpClientFactory.GetClient();
            client.DefaultRequestHeaders.Add("x-api-key", QoApiConfiguration.SmapaApiKey);

            int seq;
            try
            {
                seq = _dailySequenceRepo.GetDailySequence();
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                throw new Exception("連番の生成に失敗しました。", ex);
            }

            var smapaArgs = new SmapaApiArgs
            {
                TranNo = seq,
                DateTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                HospCode = hospCode,
                LocalId = linkageSystemId
            };

            var content = CreateContent(smapaArgs);

            var response = await client.PostAsync(url, content).ConfigureAwait(false);

            var body = await response.Content.ReadAsStringAsync();
            var smapaResults = JsonConvert.DeserializeObject<SmapaApiResults>(body);

            if (smapaResults.Error == "1")
            {
                smapaResults.ErrorDetail = ConvertErrorMessege(smapaResults.ErrorFlag);
            }

            return smapaResults;
        }

        static string ConvertErrorMessege(string smapaError)
        {
            switch (smapaError)
            {
                case "1001":
                    return "リクエスト値/フォーマットを見直してください。";
                case "1901":
                    return "一時的なサービスダウンです。";
                case "2901":
                    return "DBサーバー異常。一時的なサービスダウン。";
                case "2902":
                    return "データ異常。要調査。";
                case "6904":
                    return "再送信を試してください。";
                case "8002":
                    return "tokenを再要求してください。";
                case "8004":
                    return "tokenを再要求してください。";
                case "1282":
                    return "当該患者IDは未登録または無効です。WTR01を試してください。 WTR09のリクエストで発生した場合はアカウント削除済。";
                case "1101":
                    return "医療機関コードを確認してください。";
                default:
                    return "未定義のエラーです。";
            }
        }

        static StringContent CreateContent(SmapaApiArgs args)
        {
            var json = JsonConvert.SerializeObject(args);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            return content;
        }
    }
}