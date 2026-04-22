using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 外部サービス連携情報書き込み処理
    /// 要テストコード
    /// </summary>
    public class ExternalServiceWriteWorker
    {

        static IExternalServiceLinkageRepository _externalRepo;
        static ILinkageRepository _linkageRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="linkageRepository">施設連携情報を扱うリポジトリ</param>
        /// <param name="externalRepository">外部サービス連携情報を扱うリポジトリ</param>
        public ExternalServiceWriteWorker(ILinkageRepository linkageRepository, IExternalServiceLinkageRepository externalRepository)
        {
            _linkageRepo = linkageRepository;
            _externalRepo = externalRepository;
        }

        /// <summary>
        /// 外部サービス連携情報リストを登録する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoExternalServiceWriteApiResults Write(QoExternalServiceWriteApiArgs args)
        {
            var results = new QoExternalServiceWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 対象者（実行者）情報は必須
            var actorKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (actorKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "実行者が不正です。");
                return results;
            }

            // 施設キーは必須
            var facilityKey = args.FacilityKey.TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象施設が指定されていません。");
                return results;
            }

            // 外部サービス連携情報リストのチェック
            if (!CheckArgsExternalServiceList(args.ExternalLinkageN, results))
            {
                return results;
            }

            // QH_EXTERNALSERVICELINKAGE_DATに変換
            if (!TryConvertEntity(args.ExternalLinkageN, facilityKey, results, out var entities))
            {
                return results;
            }

            QoAccessLog.WriteInfoLog($"検証ログ:外部連携変換終了");
            QoAccessLog.WriteInfoLog($"検証ログ:外部連携 entities.Count {entities.Count}");
            if (entities.Count > 0)
            {
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 accountKey {entities[0].ACCOUNTKEY}");
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 externalServiceType {entities[0].EXTERNALSERVICETYPE}");
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 targetServiceType　{entities[0].TARGETSERVICETYPE}");
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 value {entities[0].VALUE}");
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 deleteFlag {entities[0].DELETEFLAG}");
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 createdDate {entities[0].CREATEDDATE}");
                QoAccessLog.WriteInfoLog($"検証ログ:外部連携 updatedDate {entities[0].UPDATEDDATE}");
            }

            if (!TryWriteEntities(entities, ref results))
            {
                return results;
            }

            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;

        }

        private static bool CheckArgsExternalServiceList(List<QoApiExternalServiceListItem> externalServiceListItemN, QoApiResultsBase results)
        {
            if (externalServiceListItemN == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(QoExternalServiceWriteApiArgs.ExternalLinkageN)}は必須です。");
            }

            return true;

        }

        private bool TryConvertEntity(List<QoApiExternalServiceListItem> externalServiceListItems, Guid facilityKey,
                              QoExternalServiceWriteApiResults results, out List<QH_EXTERNALSERVICELINKAGE_DAT> entities)
        {
            try
            {
                entities = externalServiceListItems
                    .Select(x => BuildExternalServiceListEntity(x, facilityKey))
                    .Where(x => x != null && x.IsKeysValid()).ToList();

                 return true;
            } catch (Exception ex)
            {
                entities = null;
                results.Result = QoApiResult.Build(ex, "外部サービス連携リストの変換に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// DB書き込み情報の作成
        /// </summary>
        /// <param name="target"></param>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        private static QH_EXTERNALSERVICELINKAGE_DAT BuildExternalServiceListEntity(QoApiExternalServiceListItem target, Guid facilityKey)
        {
            if (target == null)
            {
                return null;
            }

            return new QH_EXTERNALSERVICELINKAGE_DAT()
            {
                ACCOUNTKEY = _linkageRepo.GetAccountKey(target.LinkageSystemId, facilityKey),
                EXTERNALSERVICETYPE = target.ExternalServiceType,
                TARGETSERVICETYPE = target.TargetServiceType,
                VALUE = target.valueJson,
                DELETEFLAG = false,
                CREATEDDATE = DateTime.Now,
                UPDATEDDATE = DateTime.Now
            };
        }

        private bool TryWriteEntities(List<QH_EXTERNALSERVICELINKAGE_DAT> entities, ref QoExternalServiceWriteApiResults results)
        {
            try
            {
                (var isSuccess, var errorMessages) = _externalRepo.WriteList(entities);

                //bool isSuccess = false;
                //List<string> errorMessages = new List<string>();

                //foreach (var entity in entities)
                //{
                //    _externalRepo.UpsertEntity(entity);
                //}

                //isSuccess = true;

                results.IsSuccess = isSuccess.ToString();
                results.ErrorMessageN = errorMessages;
                return true;

            } catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "外部サービス連携情報のDB書き込み処理に失敗しました。");
                return false;
            }
        }

    }
}