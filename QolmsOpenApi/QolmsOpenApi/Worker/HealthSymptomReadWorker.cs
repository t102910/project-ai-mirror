using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アラートと症状情報を読み込む
    /// </summary>
    public class HealthSymptomReadWorker
    {
        ISymptomRepository _symptomRepository;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="symptomRepository"></param>
        public HealthSymptomReadWorker(ISymptomRepository symptomRepository)
        {
            _symptomRepository = symptomRepository;
        }

        /// <summary>
        /// 読み込み処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthSymptomReadApiResults Read(QoHealthSymptomReadApiArgs args)
        {
            var results = new QoHealthSymptomReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 開始日
            var fromDate = args.FromDate.TryToValueType(DateTime.MinValue);

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
            if(!TryReadView(accountKey, linkageSystemNo, fromDate, toDate, args.Offset, args.Fetch, results,out var entityList))
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

        bool TryReadView(Guid accountKey, int linkageSystemNo, DateTime fromDate, DateTime toDate, int offset, int fetch, QoApiResultsBase results,  out List<QH_SYMPTOM_DAT> symptomList)
        {
            try
            {
                symptomList = _symptomRepository.ReadEntities(accountKey, fromDate, toDate, offset, fetch, linkageSystemNo);

                return true;
            }
            catch(Exception ex)
            {
                symptomList = null;
                results.Result = QoApiResult.Build(ex, "症状のリストの取得処理に失敗しました。");
                return false;
            }
        }

        bool TryConvertEntityList(List<QH_SYMPTOM_DAT> entityList, QoApiResultsBase results, out List<QoHealthSymptomItem> apiItems)
        {
            apiItems = null;
            try
            {
                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    apiItems = entityList.ConvertAll(x => new QoHealthSymptomItem
                    {
                        RecordDate = x.RECORDDATE.ToApiDateString(),
                        Symptoms = JsonConvert.DeserializeObject<List<int>>(x.SYMPTOMS) ?? new List<int>(),
                        OtherDetail = crypt.TryDecrypt(x.OTHERDETAIL),
                        Memo = crypt.TryDecrypt(x.MEMO)
                    });
                }

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