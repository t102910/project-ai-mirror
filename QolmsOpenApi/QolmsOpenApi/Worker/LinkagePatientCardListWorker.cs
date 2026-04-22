using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 診察券リスト取得処理
    /// このクラスを修正する場合は必ず対応するテストも修正し全てパスさせるようにしてください。
    /// </summary>
    public class LinkagePatientCardListWorker
    {
        IPatientCardRepository _cardRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="patientCardRepository"></param>
        public LinkagePatientCardListWorker(IPatientCardRepository patientCardRepository)
        {
            _cardRepo = patientCardRepository;
        }

        /// <summary>
        /// 診察券リストを取得する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoLinkagePatientCardListReadApiResults Read(QoLinkagePatientCardListReadApiArgs args)
        {
            var results = new QoLinkagePatientCardListReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // AuthorKey ActorKey変換チェック
            if(!args.AuthorKey.CheckArgsConvert(nameof(args.AuthorKey),Guid.Empty,results, out var authorKey))
            {                
                return results;
            }
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {               
                return results;
            }

            // 連携システム番号を変換
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);

            // StatusTypeFilterを変換。未指定・無効値は0に変換
            var statusType = args.StatusTypeFilter.TryToValueType<byte>(0);


            // 診察券生データ取得
            if(!TryGetPatientCardList(accountKey, statusType, linkageSystemNo, results, out var entityList))
            {
                return results;
            }

            // 診察券データをAPI形式に変換            
            if(!TryConvertItems(entityList, results, out var cardItems))
            {
                return results;
            }

            results.PatientCardItemN = cardItems;
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            
            return results;            
        }

        // DBから診察券リスト取得
        bool TryGetPatientCardList(Guid accountKey, byte statusType,int parentLinkageNo, QoApiResultsBase results, out List<QH_PATIENTCARD_FACILITY_VIEW> entityList)
        {
            try
            {
                entityList = _cardRepo.ReadPatientCardList(accountKey, statusType, parentLinkageNo);
                return true;
            }
            catch(Exception ex)
            {
                entityList = null;
                results.Result = QoApiResult.Build(ex, "DBからの診察券情報の取得に失敗しました。");
                return false;
            }
        }        

        // 生診察券情報をAPI形式に変換
        bool TryConvertItems(List<QH_PATIENTCARD_FACILITY_VIEW> entityList, QoApiResultsBase results, out List<QoApiPatientCardItem> items)
        {
            try
            {
                items = entityList.ConvertAll(x => x.ToApiPatientCardItem(QoPatientCardBarcodeHelper.CreateCustomBarcode));
                return true;
            }
            catch(Exception ex)
            {
                items = null;
                results.Result = QoApiResult.Build(ex, "診察券情報の変換に失敗しました。");
                return false;
            }
        }           
    }
}