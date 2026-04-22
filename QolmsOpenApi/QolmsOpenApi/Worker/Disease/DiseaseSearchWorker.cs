using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.MorphologyParser;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 病名検索処理
    /// </summary>
    public class DiseaseSearchWorker
    {
        // 形態素スコア最大取得件数
        const int MaxScoreListCount = 50;
        // 病名最大取得件数
        const int MaxDiseaseListCount = 20;

        IMedisRepository _medisRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="medisRepository"></param>
        public DiseaseSearchWorker(IMedisRepository medisRepository)
        {
            _medisRepo = medisRepository;
        }

        /// <summary>
        /// 病名検索
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoDiseaseSearchApiResults Search(QoDiseaseSearchApiArgs args)
        {
            var results = new QoDiseaseSearchApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // キーワード必須チェック
            if (string.IsNullOrWhiteSpace(args.Keyword))
            {
                results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Keyword)}が未設定です。");
                return results;
            }

            // 形態素解析実行
            if(!TryMorphologyParse(args.Keyword, results, out var tokens))
            {
                return results;
            }

            var allMorphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>();

            // 形態素ごとに索引検索とスコア計算
            foreach(var token in tokens)
            {
                if(!TryReadMorphemeScoreList(args.Keyword, token,results, out var scoreList))
                {
                    return results;
                }
                allMorphemeScoreList.AddRange(scoreList);
            }

            // Scoreでソートし、重複を排除して最大取得件数を抽出
            var sortedList = allMorphemeScoreList
                .OrderByDescending(x => x.SCORE)
                .GroupBy(x => x.REFERENCECODE)
                .Select(g => g.First())
                .Take(MaxDiseaseListCount)
                .ToList();

            // 標準病名マスタ情報取得
            if(!TryReadDiseaseList(sortedList, results, out var diseaseList))
            {
                return results;
            }   

            try {
                results.DiseaseList = diseaseList.Select(d => new QoDiseaseSearchItem
                {
                    DiseaseMasterCode = d.DISEASEMASTERCODE,
                    DiseaseMasterName = d.DISEASENAME,
                    DiseaseExchangeCode = d.DISEASEEXCHANGECODE,
                    ICD10Code = d.ICD10CODE,
                    ICD10MultiCode = d.ICD10MULTICODE,
                }).ToList();
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "検索結果の整形に失敗しました。");
                return results;
            }
            
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.Success);
            return results; 
        }

        bool TryMorphologyParse(string keyword, QoApiResultsBase results, out List<TokenInfo> tokens)
        {
            tokens = new List<TokenInfo>();
            try
            {
                tokens = QsMorphologyParser.AnalyzeDetailed(keyword);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "形態素解析に失敗しました。");
                return false;
            }
        }

        bool TryReadMorphemeScoreList(string keyword, TokenInfo token, QoApiResultsBase results, out List<QH_MEDISMORPHEME_SCORE_VIEW> scoreList)
        {
            scoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>();
            try
            {
                scoreList = _medisRepo.ReadMorphemeScoreList(keyword, token.Surface, token.Reading,MaxScoreListCount);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "形態素マッピングマスタからのデータ取得に失敗しました。");
                return false;
            }
        }

        bool TryReadDiseaseList(List<QH_MEDISMORPHEME_SCORE_VIEW> morphemeScoreList, QoApiResultsBase results, out List<QH_MEDISDISEASE_MST> diseaseList)
        {
            diseaseList = new List<QH_MEDISDISEASE_MST>();
            try
            {
                diseaseList = _medisRepo.ReadDiseaseListByScoreView(morphemeScoreList);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "標準病名マスタからのデータ取得に失敗しました。");
                return false;
            }
        }
    }    
}