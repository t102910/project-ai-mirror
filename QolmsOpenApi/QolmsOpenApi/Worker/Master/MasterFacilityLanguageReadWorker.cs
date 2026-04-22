using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 施設言語リソース取得処理
    /// </summary>
    public class MasterFacilityLanguageReadWorker
    {
        private readonly IFacilityRepository _facilityRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MasterFacilityLanguageReadWorker(IFacilityRepository facilityRepository)
        {
            _facilityRepo = facilityRepository;
        }

        /// <summary>
        /// 施設言語リソースの一覧を取得する
        /// </summary>
        public QoMasterFacilityLanguageReadApiResults Read(QoMasterFacilityLanguageReadApiArgs args)
        {
            var results = new QoMasterFacilityLanguageReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var facilityKey = args.FacilityCodeReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (!TryGetFacilityLanguageResourceEntities(facilityKey, results, out var entities))
            {
                return results;
            }

            if (!TryConvertApiData(entities, results, out var apiItems))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.LangResourceItems = apiItems;

            return results;
        }

        /// <summary>
        /// 施設言語リソースのエンティティを取得する
        /// </summary>
        /// <param name="facilityKey">施設キー</param>
        /// <param name="results">API 結果</param>
        /// <param name="entities">施設言語リソースのエンティティ</param>
        /// <returns>エンティティ取得が成功したか否か</returns>
        private bool TryGetFacilityLanguageResourceEntities(Guid facilityKey, QoApiResultsBase results, out List<QH_FACILITYLANGUAGE_MST> entities)
        {
            try
            {
                entities = _facilityRepo.ReadFacilityLanguage(facilityKey);
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "DB からの施設言語リソース取得に失敗しました。");
                entities = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 施設言語リソースのエンティティを API データに変換する
        /// </summary>
        /// <param name="entityList">エンティティ</param>
        /// <param name="results">API 結果</param>
        /// <param name="apiItems">API データ</param>
        /// <returns>データ変換が成功したか否か</returns>
        private bool TryConvertApiData(List<QH_FACILITYLANGUAGE_MST> entityList, QoMasterFacilityLanguageReadApiResults results, out List<QoApiFacilityLanguageItem> apiItems)
        {
            try
            {
                apiItems = entityList.Select(x => new QoApiFacilityLanguageItem
                {
                    Key = x.LANGUAGEKEY,
                    ResourceJson = x.VALUE
                }).OrderBy(x => x.Key).ToList();

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "施設言語リソースのデータ変換に失敗しました。");
                apiItems = null;
                return false;
            }
        }
    }
}