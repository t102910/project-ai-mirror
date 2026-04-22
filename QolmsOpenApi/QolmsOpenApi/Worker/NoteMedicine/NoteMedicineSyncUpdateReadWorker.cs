
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>‘
    /// お薬手帳 同期更新情報取得用
    /// </summary>
    public class NoteMedicineSyncUpdateReadWorker : NoteMedicineSyncWorkerBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        public NoteMedicineSyncUpdateReadWorker(INoteMedicineRepository noteMedicineRepository) : base(noteMedicineRepository) 
        {
        }

        /// <summary>
        /// 同期更新情報取得処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineSyncUpdateReadApiResults Read(QoNoteMedicineSyncUpdateReadApiArgs args)
        {
            var results = new QoNoteMedicineSyncUpdateReadApiResults
            {
                IsSuccess = bool.FalseString
            };


            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 開始日チェック
            if (!args.SyncStartDate.CheckArgsConvert(nameof(args.SyncStartDate), DateTime.MinValue, results, out var syncStartDate))
            {
                return results;
            }

            // 終了日チェック
            var syncEndDate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(args.SyncEndDate))
            {
                if (!args.SyncEndDate.CheckArgsConvert(nameof(args.SyncEndDate), DateTime.MinValue, results, out syncEndDate))
                {
                    return results;
                }
            }
            else
            {
                syncEndDate = DateTime.Now; // 省略時現在時刻
            }

            // 期間指定チェック
            if (syncStartDate > syncEndDate)
            {
                results.Result =  QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "期間指定が不正です。");
                return results;
            }

            // お薬手帳情報取得（データタイプ1,2,100　削除済みデータも含む）
            if (!TryReadNoteUpdateData(accountKey, syncStartDate, syncEndDate, results, out var noteHistoryList))
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

            // 調剤薬の薬効分類と画像をセット
            foreach (var item in ethMedicineItems)
            {
                if (yakkoDic.TryGetValue(item.YjCode, out var categoryItem))
                {
                    item.EffectCategoryCode = categoryItem.MinorClassCode;
                    item.EffectCategoryName = categoryItem.MinorClassName;
                }
            }

            results.NextSyncStartDate = syncEndDate.AddMilliseconds(1).ToApiDateString();
            results.ItemList = noteMedicineItems;

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryReadNoteUpdateData(Guid accountKey, DateTime startDate, DateTime endDate, QoApiResultsBase results, out List<QH_MEDICINE_DAT> noteHistoryList)
        {
            noteHistoryList = null;
            try
            {
                // お薬手帳情報取得 データタイプ1,2,100 (削除済みデータも含む)
                noteHistoryList = _noteRepo.ReadNoteUpdate(accountKey, startDate, endDate);

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お薬手帳データの取得に失敗しました。");
                return false;
            }
        }
    }
}