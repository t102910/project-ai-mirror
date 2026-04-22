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
    /// 外部サービスSPHR連携に関する処理
    /// </summary>
    public class ExternalServiceSphrLinkageReadWorker
    {

        static IExternalServiceLinkageRepository _externalRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="linkageRepository">施設連携情報を扱うリポジトリ</param>
        /// <param name="externalRepository">外部サービス連携情報を扱うリポジトリ</param>
        public ExternalServiceSphrLinkageReadWorker(IExternalServiceLinkageRepository externalRepository)
        {
            _externalRepo = externalRepository;
        }

        /// <summary>
        /// 外部サービスSPHR連携対象者情報リストを取得する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoExternalServiceSphrLinkageReadApiResults LinkageRead(QoExternalServiceSphrLinkageReadApiArgs args)
        {
            var results = new QoExternalServiceSphrLinkageReadApiResults
            {
                IsSuccess = bool.FalseString
            };
            QoAccessLog.WriteInfoLog($"検証ログ:AccountKeyReference{args.AccountKeyReference}");
            QoAccessLog.WriteInfoLog($"検証ログ:LinkageSystemNo{args.LinkageSystemNo}");
            QoAccessLog.WriteInfoLog($"検証ログ:CommunitySystemNo{args.CommunitySystemNo}");
            //必須チェック
            // AccountKeyReference
            var accountKeyReference = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(args.AccountKeyReference))
            {
                accountKeyReference= args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
                if (accountKeyReference == Guid.Empty)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処理対象者のアカウントキーが指定されていません。");
                    return results;
                }
            }
            // LinkageSystemNo
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);
            if (linkageSystemNo == int.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象施設の連携システム番号が指定されていません。");
                return results;
            }
            
            // CommunitySystemNo
            var communitySystemNo = args.CommunitySystemNo.TryToValueType(int.MinValue);
            if (communitySystemNo == int.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象連携の連携システム番号が指定されていません。");
                return results;
            }
            
            if (!TryReadEntities(accountKeyReference, linkageSystemNo, communitySystemNo, ref results))
            {
                return results;
            }
            QoAccessLog.WriteInfoLog($"検証ログ:取得件数{results.TargetList.Count}");
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;

        }

        private bool TryReadEntities(Guid accountKeyReference, int linkageSystemNo, int communitySystemNo, ref QoExternalServiceSphrLinkageReadApiResults results)
        {
            try
            {
                var entites = _externalRepo.SphrLinkageRead(accountKeyReference, linkageSystemNo, communitySystemNo);
                QoAccessLog.WriteInfoLog($"検証ログ:entites{entites.Count}");
                foreach (var entity in entites)
                {
                    results.TargetList.Add((entity.LINKAGESYSTEMID, entity.DELETEFLAG));
                }
                return true;

            } catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "外部サービスSPHR連携対象情報の取得に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// 外部サービス　連携システム情報 を登録する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoExternalServiceSphrLinkageWriteApiResults LinkageWrite(QoExternalServiceSphrLinkageWriteApiArgs args)
        {
            var results = new QoExternalServiceSphrLinkageWriteApiResults
            {
                IsSuccess = bool.FalseString
            };
            QoAccessLog.WriteInfoLog($"検証ログ:AccountKeyReference{args.AccountKeyReference}");
            QoAccessLog.WriteInfoLog($"検証ログ:LinkageSystemNo{args.LinkageSystemNo}");
            QoAccessLog.WriteInfoLog($"検証ログ:LinkageSystemId{args.LinkageSystemId}");
            QoAccessLog.WriteInfoLog($"検証ログ:DeleteFlag{args.DeleteFlag}");
            //必須チェック
            // AccountKeyReference
            var accountKeyReference = args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (accountKeyReference == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処理対象者のアカウントキーが指定されていません。");
                return results;
            }
            // LinkageSystemNo
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);
            if (linkageSystemNo == int.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象施設の連携システム番号が指定されていません。");
                return results;
            }
            // LinkageSystemId
            var linkageSystemId = args.LinkageSystemId;
            if (string.IsNullOrWhiteSpace(linkageSystemId))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象連携の連携システム番号が指定されていません。");
                return results;
            }

            var deleteFlag = args.DeleteFlag.TryToValueType(true);

            if (!TryWriteEntities(accountKeyReference, linkageSystemNo, linkageSystemId, deleteFlag, ref results))
            {
                return results;
            }
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;

        }

        private bool TryWriteEntities(Guid accountKeyReference, int linkageSystemNo, string linkageSystemId, bool deleteFlag, ref QoExternalServiceSphrLinkageWriteApiResults results)
        {
            try
            {
                var entity = new QH_LINKAGE_DAT()
                {
                    ACCOUNTKEY = accountKeyReference,
                    LINKAGESYSTEMNO = linkageSystemNo,
                    LINKAGESYSTEMID = linkageSystemId,
                    DELETEFLAG = deleteFlag,
                };
                var result = _externalRepo.SphrLinkageWrite(entity);
                QoAccessLog.WriteInfoLog($"検証ログ:登録結果{result}");
                return result;

            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "外部サービス連携システム情報の登録に失敗しました。");
                return false;
            }
        }
    }
}