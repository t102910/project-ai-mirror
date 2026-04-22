using System;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.JAHISMedicineEntityV1;
using System.Collections.Generic;
using System.Linq;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System.Text;
using System.Configuration;
using MGF.QOLMS.QolmsOpenApi.Repositories;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// お薬手帳の情報を読み込む処理を行います。
    /// NoteMedicineWorkerにあるRead系はこちらに移動予定。
    /// </summary>
    public class NoteMedicineReadWorker
    {
        INoteMedicineRepository _repo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        public NoteMedicineReadWorker(INoteMedicineRepository noteMedicineRepository)
        {
            _repo = noteMedicineRepository;
        }

        /// <summary>
        /// 指定日のお薬手帳の情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineDayReadApiResults DayRead(QoNoteMedicineDayReadApiArgs args)
        {
            var result = new QoNoteMedicineDayReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var targetDate = args.TargetDate.TryToValueType(DateTime.MinValue);
            if(targetDate == DateTime.MinValue)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "取得対象の日付が不正です。");
                return result;
            }            

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(0);

            if(accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アカウントキーが不正です。");
                return result;
            }            

            var dataType = (QH_MEDICINE_DAT.DataTypeEnum)args.DataType.TryToValueType((byte)QH_MEDICINE_DAT.DataTypeEnum.None);
            if(dataType == QH_MEDICINE_DAT.DataTypeEnum.None)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "データタイプが不正です。");
                return result;
            }

            var dataTypeList = new List<byte>
            {
                (byte)dataType
            };

            switch (dataType)
            {
                case QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug:
                    // 調剤薬ならSSMIXから取り込んだデータも対象に
                    dataTypeList.Add((byte)QH_MEDICINE_DAT.DataTypeEnum.Ssmix);
                    break;
                case QH_MEDICINE_DAT.DataTypeEnum.OtcDrug:
                    // 市販薬写真も取得対象に
                    dataTypeList.Add((byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto);
                    break;
            }

            List<QH_MEDICINE_DAT> entityN;
            try
            {
                entityN = _repo.ReadMedicineDayList(accountKey, targetDate, dataTypeList, facilityKey, linkageSystemNo);
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, ex.Message);
                return result;
            }

            try
            {
                if (entityN.Any())
                {
                    result.Data = ConvertMedicineItem(entityN);
                }                
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return result;
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                return result;
            }
        }

        QoApiNoteMedicineHeaderItem ConvertMedicineItem(List<QH_MEDICINE_DAT> entityN)
        {
            using(var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                return new QoApiNoteMedicineHeaderItem
                {
                    RecordDate = entityN.First().RECORDDATE.ToApiDateString(),
                    MedicineSetN = entityN.ConvertAll(x => ConvertMedicineDetailItem(x, crypt))
                };
            };           
        }

        QoApiNoteMedicineDetailItem ConvertMedicineDetailItem(QH_MEDICINE_DAT entity, QsCrypt crypt)
        {
            var qsJson = new QsJsonSerializer();            

            var item = new QoApiNoteMedicineDetailItem
            {
                DataType = entity.DATATYPE.ToString(),
                OwnerType = entity.OWNERTYPE.ToString(),
                DataIdReference = entity.RECEIPTNO.ToEncrypedReference(),
                ConvertedMedicineSet = "",
                OtcPhotoMedicineSet = new QoApiMedicineSetOtcPhotoItem(),
            };

            if (!string.IsNullOrWhiteSpace(entity.CONVERTEDMEDICINESET))
            {
                item.ConvertedMedicineSet = crypt.DecryptString(entity.CONVERTEDMEDICINESET);
            }

            if(entity.DATATYPE == (byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrug && 
                !string.IsNullOrWhiteSpace(entity.MEDICINESET))
            {
                item.OtcMedicineSet = qsJson.Deserialize<QhMedicineSetOtcDrugOfJson>(crypt.DecryptString(entity.MEDICINESET)).ToApiMedicineSetOtcItem(entity.RECORDDATE);
            }

            if(entity.DATATYPE == (byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto && !string.IsNullOrWhiteSpace(entity.MEDICINESET))
            {
                item.OtcPhotoMedicineSet = qsJson.Deserialize<QhMedicineSetOtcDrugPhotoOfJson>(crypt.DecryptString(entity.MEDICINESET)).ToApiMedicineSetOtcPhotoItem(entity.RECORDDATE);
            }

            return item;
        }
    }
}