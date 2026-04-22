using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using DataTypeEnum = MGF.QOLMS.QolmsDbEntityV1.QH_MEDICINE_DAT.DataTypeEnum;
using OwnerTypeEnum = MGF.QOLMS.QolmsDbEntityV1.QH_MEDICINE_DAT.OwnerTypeEnum;
using MGF.QOLMS.JAHISMedicineEntityV1;
using System.Text;
using System.Text.RegularExpressions;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.Extension;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// お薬手帳 同期データ削除
    /// </summary>
    public class NoteMedicineSyncDataDeleteWorker : NoteMedicineSyncWorkerBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        public NoteMedicineSyncDataDeleteWorker(INoteMedicineRepository noteMedicineRepository) : base(noteMedicineRepository)
        {
        }

        /// <summary>
        /// 削除処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineSyncDataDeleteApiResults Delete(QoNoteMedicineSyncDataDeleteApiArgs args)
        {
            var results = new QoNoteMedicineSyncDataDeleteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // RecordDateチェック
            if (!args.RecordDate.CheckArgsConvert(nameof(args.RecordDate), DateTime.MinValue, results, out var recordDate))
            {
                return results;
            }

            // Sequenceチェック
            if (args.Sequence <= 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Sequence)}が不正です。");
                return results;
            }

            // 対象のお薬手帳レコードを取得
            if (!TryReadEntity(accountKey, recordDate, args.Sequence, results, out var entity))
            {
                return results;
            }


            // OwnerType DataTypeの組み合わせチェック
            if (!CheckDataType((OwnerTypeEnum)entity.OWNERTYPE, (DataTypeEnum)entity.DATATYPE))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"削除不可能なデータです。");
                return results;
            }

            // 削除処理実行
            if (!TryDeleteEntity(entity, results))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        private bool TryDeleteEntity(QH_MEDICINE_DAT entity, QoApiResultsBase results)
        {
            try
            {
                _noteRepo.DeleteEntity(entity);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "データの削除処理に失敗しました。");
                return false;
            }
        }

        private bool CheckDataType(OwnerTypeEnum ownerType, DataTypeEnum dataType)
        {
            switch (ownerType)
            {
                case OwnerTypeEnum.Oneself:
                    return dataType == DataTypeEnum.EthicalDrug || dataType == DataTypeEnum.OtcDrug;
                case OwnerTypeEnum.QrCode:
                    return dataType == DataTypeEnum.EthicalDrug;
                default:
                    return false;
            }
        }
    }
}