using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    public class LinkageSystemIdWriteWorker
    {
        ILinkageRepository _linkageRepository;

        public LinkageSystemIdWriteWorker(ILinkageRepository linkageRepository)
        {
            this._linkageRepository = linkageRepository;
        }

        /// <summary>
        /// 診察券追加実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoLinkageLinkageSystemIdWriteApiResults Write(QoLinkageLinkageSystemIdWriteApiArgs args)
        {
            var results = new QoLinkageLinkageSystemIdWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // ActorKey, Birthday変換チェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            if (args.LinkageSystemNo.CheckArgsConvert(nameof(args.LinkageSystemNo), int.MinValue, results, out var linkageSystemNo))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.LinkageSystemNo)}が不正です。");
                return results;
            }

            //if (string.IsNullOrWhiteSpace(args.LinkageSystemId))
            //{
            //    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.LinkageSystemId)}が不正です。");
            //    return results;
            //}

            // 診察券の連携システム番号を取得
            var linkageMst = _linkageRepository.GetFacilitykey(linkageSystemNo);
            if (linkageMst.LINKAGESYSTEMNO <= 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "指定した連携がマスタに存在しません");
                return results;
            }

            // 利用者カード重複チェック
            if (!CheckCardNo(linkageMst.FACILITYKEY, args.LinkageSystemId, results))
            {
                return results;
            }

            try
            {
                var targetAccountKey = accountKey;

                // 診察券登録処理
                if (!AddPatientCard(args.AuthorKey.TryToValueType(Guid.Empty), targetAccountKey, linkageSystemNo, linkageMst.FACILITYKEY, args.LinkageSystemId, results, out var seq))
                {
                    return results;
                }
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "その他のエラーにより診察券追加処理に失敗しました。");
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        /// <summary>
        /// カードが利用可能かどうかチェック
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="cardNo"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool CheckCardNo(Guid facilityKey, string cardNo, QoApiResultsBase results)
        {
            try
            {
                if (!_linkageRepository.IsAvailableCard(facilityKey, cardNo))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.PatientCardDuplicate, "この診察券番号は既に使われています。");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex);
                return false;
            }
        }


        /// <summary>
        /// 診察券追加処理
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="accountKey"></param>
        /// <param name="cardLinkageNo"></param>
        /// <param name="facilityKey"></param>
        /// <param name="cardLinkageId"></param>
        /// <param name="apiResult"></param>
        /// <param name="sequence">追加した診察券のSEQUENCE番号</param>
        /// <returns></returns>
        protected bool AddPatientCard(Guid authorKey, Guid accountKey, int cardLinkageNo, Guid facilityKey, string cardLinkageId, QoApiResultsBase apiResult, out int sequence)
        {
            sequence = int.MinValue;
            try
            {

                (var errors, var entity) = _linkageRepository.WriteLinkagePatientCard(authorKey, accountKey, cardLinkageNo, facilityKey, cardLinkageId, int.MinValue, false);

                if (string.IsNullOrWhiteSpace(errors))
                {
                    sequence = entity.SEQUENCE;
                    return true;
                }

                if (errors.Contains("指定された種類のカードは、すでに登録があります"))
                {
                    apiResult.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.PatientCardDuplicate, errors);
                    return false;
                }

                apiResult.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"診察券の登録に失敗しました。{errors}");
                return false;
            }
            catch (Exception ex)
            {
                apiResult.Result = QoApiResult.Build(ex);
                return false;
            }
        }
    }
}