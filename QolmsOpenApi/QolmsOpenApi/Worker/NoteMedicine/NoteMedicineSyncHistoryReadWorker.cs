using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
   
    /// <summary>
    /// お薬手帳 同期履歴取得用
    /// </summary>
    public class NoteMedicineSyncHistoryReadWorker　: NoteMedicineSyncWorkerBase
    {       
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        public NoteMedicineSyncHistoryReadWorker(INoteMedicineRepository noteMedicineRepository): base(noteMedicineRepository) 
        {
        }

        /// <summary>
        /// 同期履歴取得処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineSyncHistoryReadApiResults Read(QoNoteMedicineSyncHistoryReadApiArgs args)
        {
            var results = new QoNoteMedicineSyncHistoryReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 開始日チェック
            if (!args.StartDate.CheckArgsConvert(nameof(args.StartDate),DateTime.MinValue, results, out var startDate))
            {
                return results;
            }

            // 取得日数チェック
            if (args.Days < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Days)}が不正です。");
                return results;
            }
            
            var days = args.Days;
            if (args.Days == 0)
            {
                days = 30; // 省略時30日
            }

            // データ取得日時を取得
            var fetchedAt = DateTime.Now;

            // お薬手帳情報取得
            if (!TryReadNoteHistory(accountKey, startDate, days, results, out var noteHistoryList))
            {
                return results;
            }

            // データ変換
            var noteMedicineItems = ConvertNoteMedicineItems(noteHistoryList);       

            // 調剤薬のリストをまとめる
            var ethMedicineItems = noteMedicineItems
                .SelectMany(x => x.DetailItems
                    .SelectMany(y => y.RpItems
                        .SelectMany(z => z.MedicineItems)));            

            // 取得したデータに含まれる全てのYJコードをリスト化する
            var yjCodeList = ethMedicineItems
                .Select(x => x.YjCode)                
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList();

            // 薬効分類一括取得
            if (!TryReadYakko(yjCodeList, results, out var yakkoDic))
            {
                return results;
            }                 

            // 調剤薬の薬効分類をセット
            foreach (var item in ethMedicineItems)
            {
                if (yakkoDic.TryGetValue(item.YjCode, out var categoryItem))
                {
                    item.EffectCategoryCode = categoryItem.MinorClassCode;
                    item.EffectCategoryName = categoryItem.MinorClassName;
                }              
            }
            
            // 次の取得候補の抽出
            if (!TryReadNoteHistoryNext(accountKey, startDate, days,results, out var nextEntity))
            {
                return results;
            }

            results.FetchedAt = fetchedAt.ToApiDateString();
            results.HasNextData = nextEntity != null;
            results.NextStartDate = nextEntity?.RECORDDATE.ToApiDateString() ?? string.Empty;
            results.HistoryList = noteMedicineItems;

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryReadNoteHistory(Guid accountKey, DateTime startDate, int days,QoApiResultsBase results, out List<QH_MEDICINE_DAT> noteHistoryList)
        {
            noteHistoryList = null;
            try
            {
                // startDateを含めてdays日数分取得するための最終日を計算
                var endDate = startDate.Date.AddDays(-(days - 1));

                noteHistoryList = _noteRepo.ReadNoteHistory(accountKey, startDate.Date, endDate.Date);

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お薬手帳データの取得に失敗しました。");
                return false;
            }
        }

        bool TryReadNoteHistoryNext(Guid accountKey, DateTime startDate, int days, QoApiResultsBase results, out QH_MEDICINE_DAT nextHistoryCandidate)
        {
            nextHistoryCandidate = null;
            try
            {
                // 次の取得対象の開始日を求める
                var target = startDate.Date.AddDays(-days);

                nextHistoryCandidate = _noteRepo.ReadNoteHistoryNext(accountKey, target.Date);

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お薬手帳データの次候補取得に失敗しました。");
                return false;
            }
        }       
    }
}