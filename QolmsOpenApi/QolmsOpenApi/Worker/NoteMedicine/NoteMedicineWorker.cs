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
    /// お薬手帳の情報を処理します。
    /// </summary>
    public sealed class NoteMedicineWorker
    {
        #region "Constant"
        private static readonly int MAX_COUNT_ATTACHED_FILE = 3;
        private static readonly string SYSTEM_ID = ConfigurationManager.AppSettings["EKusuLinkApiSystemId"]; // "MG10" 'これが自社用？　

        /// <summary>
        /// お薬手帳データに紐づくカレンダー イベントカテゴリ種別を表します。
        /// </summary>
        private const int MEDICINE_EVENT_CATEGORYNO = 4;

        #endregion

        INoteMedicineRepository _noteMedicineRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepo"></param>
        public NoteMedicineWorker(INoteMedicineRepository noteMedicineRepo)
        {
            _noteMedicineRepo = noteMedicineRepo;
        }

        #region "Private Method"
        private bool AddMedicine(QoNoteMedicineAddApiArgs args, ref string dataId, ref string refErrorMessage)
        {
            var result = false;
            var now = DateTime.Now;
            refErrorMessage = string.Empty;

            // 1
            // 引数チェック
            if (args.Data == null)
            {
                refErrorMessage = "Data: お薬手帳情報が不正です。";
            }
            else if (string.IsNullOrWhiteSpace(args.Data.DataType))
            {
                refErrorMessage = "Data.DataType: データ種別が不正です。";
            }
            else if (string.IsNullOrWhiteSpace(args.Data.OwnerType))
            {
                refErrorMessage = "Data.OwnerType: 情報提供元種別が不正です。";
            }

            QH_MEDICINE_DAT.DataTypeEnum dataType = (QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.DataType);
            QH_MEDICINE_DAT.OwnerTypeEnum ownerType = (QH_MEDICINE_DAT.OwnerTypeEnum)byte.Parse(args.Data.OwnerType);

            if (!Enum.IsDefined(typeof(QH_MEDICINE_DAT.DataTypeEnum), dataType) || dataType == QH_MEDICINE_DAT.DataTypeEnum.None)
            {
                refErrorMessage = "Data.DataType: データ種別の値が範囲外です。";
            }
            else if ((!Enum.IsDefined(typeof(QH_MEDICINE_DAT.OwnerTypeEnum), ownerType) || ownerType == QH_MEDICINE_DAT.OwnerTypeEnum.None))
            {
                refErrorMessage = "Data.OwnerType: 情報提供元種別の値が範囲外です。";
            }

            if (dataType != QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto && string.IsNullOrWhiteSpace(args.Data.ConvertedMedicineSet))
            {
                refErrorMessage = "Data.ConvertedMedicineSet: お薬手帳データが不正です。";
            }
            // エラーありなら中断
            if (!string.IsNullOrWhiteSpace(refErrorMessage)) return false;

            try
            {
                var ser = new QsJsonSerializer();
                var recordDate = DateTime.MinValue;
                var sequence = int.MinValue;
                var pharmacyNo = int.MinValue;
                string tmpDataId = dataId;
                var medicineSet = string.Empty;
                string convertedMedicineSet = args.Data.ConvertedMedicineSet;
                var isRegisterEvent = false;
                JMVersionTypeEnum version = JMVersionTypeEnum.None;

                // 2
                // エンティティ生成
                switch (dataType)
                {
                    case QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug:
                    case QH_MEDICINE_DAT.DataTypeEnum.OtcDrug:
                        // 調剤薬、市販薬（JAHISベース）
                        // JAHIS Json文字列のデシリアライズ
                        JM_Message jahis = new QsJsonSerializer().Deserialize<JM_Message>(args.Data.ConvertedMedicineSet);
                        jahis.Validate(JMOutputTypeEnum.None, ref version);

                        if (jahis.ValidationResults.Any())
                        {
                            // JAHIS検証エラーあり
                            var message = new StringBuilder();
                            foreach (JMValidationResult item in jahis.ValidationResults)
                            {
                                message.AppendLine(item.Message);
                            }
                            refErrorMessage = message.ToString().Replace(Environment.NewLine, " ");
                            // 中断
                            return false;
                        }

                        // 調剤日
                        if (!DateTime.TryParseExact(jahis.Prescription_List.First().No005.No005_2, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out recordDate))
                        {
                            if (args.Data.OtcMedicineSet != null && !string.IsNullOrWhiteSpace(args.Data.OtcMedicineSet.RecordDate))
                            {
                                recordDate = args.Data.OtcMedicineSet.RecordDate.ToValueType<DateTime>();
                            }
                            else if (args.Data.OtcPhotoMedicineSet != null && !string.IsNullOrWhiteSpace(args.Data.OtcPhotoMedicineSet.RecordDate))
                            {
                                recordDate = args.Data.OtcPhotoMedicineSet.RecordDate.ToValueType<DateTime>();
                            }
                            else
                            {
                                refErrorMessage = "Data.RecordDate: 調剤日・購入日が不正です。";
                                return false;
                            }
                        }

                        // 薬局 医療機関番号（あれば）
                        if (jahis.Prescription_List != null && jahis.Prescription_List.Any() && jahis.Prescription_List.First().No011 != null)
                        {
                            pharmacyNo = jahis.Prescription_List.First().No011.No011_5.TryToValueType<int>(int.MinValue);
                        }

                        // 表示セット（未使用）
                        if (dataType == QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug)
                        {
                            var jsonObj = new QhMedicineSetEthicalDrugOfJson();
                            jsonObj.PrescriptionDate = recordDate.ToApiDateString();
                            jsonObj.PharmacyId = pharmacyNo.ToString();

                            JM_Prescription prescriptionItem = jahis.Prescription_List.First();

                            foreach (JM_RpSet rpSetItem in prescriptionItem.RpSet_List)
                            {
                                // 処方
                                if (rpSetItem.Rp_List != null)
                                {
                                    foreach (JM_Rp rpItem in rpSetItem.Rp_List)
                                    {
                                        var medicineUsage = new QhMedicineSetUsageItemOfJson()
                                        {
                                            MedicineN = new List<QhMedicineSetEthicalDrugItemOfJson>()
                                        };

                                        // 処方―医師
                                        if (rpSetItem.No055 != null)
                                        {
                                            medicineUsage.DoctorId = string.Empty;
                                            medicineUsage.DoctorName = rpSetItem.No055.No055_2;
                                            medicineUsage.RepresentedOrganizationName = rpSetItem.No055.No055_3;
                                        }
                                        // 用法
                                        if (rpItem.No301 != null)
                                        {
                                            medicineUsage.DosageForm = rpItem.No301.No301_6.TryToValueType<QsDbDosageFormTypeEnum>(QsDbDosageFormTypeEnum.Other);
                                            medicineUsage.Usage = rpItem.No301.No301_3;
                                            medicineUsage.Days = rpItem.No301.No301_4;
                                            medicineUsage.Unit = rpItem.No301.No301_5;
                                        }
                                        if (rpItem.Medicine_List != null)
                                        {
                                            foreach (JM_Medicine medicineItem in rpItem.Medicine_List)
                                            {
                                                if (medicineItem.No201 != null)
                                                {
                                                    var medicine = new QhMedicineSetEthicalDrugItemOfJson();
                                                    // medicine.BrandCode = "";
                                                    // medicine.BrandName = "";
                                                    medicine.CodeSystem = medicineItem.No201.No201_6.TryToValueType<byte>(1);
                                                    // medicine.Comment = "";
                                                    medicine.Dose = medicineItem.No201.No201_4;
                                                    //'  medicine.IsGeneric = false;
                                                    medicine.MedicineCode = medicineItem.No201.No201_7;
                                                    medicine.MedicineName = medicineItem.No201.No201_3;
                                                    medicine.ReceiptNumber = medicineItem.No201.No201_2.TryToValueType<byte>(1);
                                                    medicine.Unit = medicineItem.No201.No201_5;

                                                    medicineUsage.MedicineN.Add(medicine);
                                                }
                                            }
                                        }

                                        jsonObj.MedicineUsageN.Add(medicineUsage);
                                    }
                                }
                            }

                            // 処方―医療機関
                            if (prescriptionItem.No051 != null)
                            {
                                jsonObj.FacilityName = prescriptionItem.No051.No051_2;
                                jsonObj.FacilityId = prescriptionItem.No051.No051_5;
                            }

                            // 調剤ー医療機関
                            if (prescriptionItem.No011 != null)
                            {
                                jsonObj.PharmacyName = prescriptionItem.No011.No011_2;
                            }

                            // 調剤-薬剤師
                            if (prescriptionItem.No015 != null)
                            {
                                jsonObj.PharmacistName = prescriptionItem.No015.No015_2;
                            }

                            // 残薬確認、レコード作成者
                            if (prescriptionItem.No421_List != null && prescriptionItem.No421_List.Count > 0)
                            {
                                foreach (JM_No421 item in prescriptionItem.No421_List)
                                {
                                    jsonObj.LeftoverMedicine += item.No421_2;
                                    if (string.IsNullOrWhiteSpace(jsonObj.LeftoverMedicineAuthor))
                                    {
                                        jsonObj.LeftoverMedicineAuthor = item.No421_3;
                                    }
                                }
                            }

                            // 特記事項複数ありそう？とりあえずまとめて入れとく。
                            if (prescriptionItem.No501_List != null && prescriptionItem.No501_List.Count > 0)
                            {
                                foreach (JM_No501 item in prescriptionItem.No501_List)
                                {
                                    jsonObj.SpecialNotes += item.No501_2;
                                }
                            }


                            // 患者メモ
                            if (prescriptionItem.No601_List != null && prescriptionItem.No601_List.Count > 0)
                            {
                                foreach (JM_No601 item in prescriptionItem.No601_List)
                                {
                                    jsonObj.Memo += item.No601_2;
                                }
                            }

                            medicineSet = ser.Serialize<QhMedicineSetEthicalDrugOfJson>(jsonObj);
                            isRegisterEvent = true;
                        }
                        else
                        {
                            var jsonObj = new QhMedicineSetOtcDrugOfJson();
                            jsonObj.PharmacyNo = pharmacyNo;

                            JM_Prescription prescriptionItem = jahis.Prescription_List.First();

                            // 薬局名
                            if (prescriptionItem.No011 != null)
                            {
                                jsonObj.PharmacyName = prescriptionItem.No011.No011_2;
                            }

                            // お薬メモ
                            //if (prescriptionItem.No601_List != null && prescriptionItem.No601_List.Any())
                            //{
                            //    foreach (JM_No601 item in prescriptionItem.No601_List)
                            //    {
                            //        jsonObj.Comment += item.No601_2;
                            //    }
                            //}


                            // 市販薬は004にいれる
                            if (jahis.No004_List != null && jahis.No004_List.Any())
                            {
                                foreach (JM_No004 item in jahis.No004_List)
                                {
                                    jsonObj.Comment += item.No004_2;
                                }
                            }

                            // お薬情報
                            if (dataType == QH_MEDICINE_DAT.DataTypeEnum.OtcDrug &&
                                args.Data.OtcMedicineSet != null &&
                                args.Data.OtcMedicineSet.OtcDrugN.Any())
                            {
                                foreach (QoApiMedicineOtcItem drug in args.Data.OtcMedicineSet.OtcDrugN)
                                {
                                    jsonObj.MedicineN.Add(new QhMedicineSetOtcDrugItemOfJson()
                                    {
                                        AttachedFileN = args.Data.OtcPhotoMedicineSet.FileKeyN.ConvertAll(i =>
                                       {
                                           return new QhAttachedFileOfJson() { FileKey = i.FileKeyReference.ToDecrypedReference() };
                                       }),
                                        ItemCode = drug.ItemCode,
                                        ItemCodeType = drug.ItemCodeType,
                                        MedicineName = drug.MedicineName
                                    });
                                }

                            }
                            else
                            {
                                // 旧来処理
                                if (jahis.No003_List != null)
                                {
                                    foreach (JM_No003 item in jahis.No003_List)
                                    {
                                        var medicineItem = new QhMedicineSetOtcDrugItemOfJson()
                                        {
                                            MedicineName = item.No003_2,
                                            AttachedFileN = new List<QhAttachedFileOfJson>()
                                        };
                                        jsonObj.MedicineN.Add(medicineItem);
                                    }
                                }
                            }
                            medicineSet = ser.Serialize<QhMedicineSetOtcDrugOfJson>(jsonObj);
                        }
                        break;
                    case QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto:
                        // 市販薬写真（ファイル、その他文字情報）

                        if (args.Data.OtcPhotoMedicineSet == null)
                        {

                        }
                        else if (args.Data.OtcPhotoMedicineSet.FileKeyN == null || !args.Data.OtcPhotoMedicineSet.FileKeyN.Any())
                        {
                            refErrorMessage = "Data.OtcPhotoMedicineSet.FileKeyN: 添付ファイルは必須です。";
                            return false;
                        }
                        else if (args.Data.OtcPhotoMedicineSet.FileKeyN.Count > NoteMedicineWorker.MAX_COUNT_ATTACHED_FILE)
                        {
                            refErrorMessage = string.Format("Data.FileKeyN: 添付ファイル数は{0}個までです。", NoteMedicineWorker.MAX_COUNT_ATTACHED_FILE);
                            return false;
                        }

                        if (!string.IsNullOrWhiteSpace(args.Data.OtcPhotoMedicineSet.RecordDate))
                        {
                            recordDate = args.Data.OtcPhotoMedicineSet.RecordDate.ToValueType<DateTime>();
                        }
                        else
                        {
                            refErrorMessage = "Data.RecordDate: 調剤日・購入日が不正です。";
                            return false;
                        }

                        var json = new QhMedicineSetOtcDrugPhotoOfJson()
                        {
                            PharmacyName = args.Data.OtcPhotoMedicineSet.PharmacyName,
                            Memo = args.Data.OtcPhotoMedicineSet.Memo,
                            AttachedFileN = args.Data.OtcPhotoMedicineSet.FileKeyN.ConvertAll(i =>
                           {
                               return new QhAttachedFileOfJson()
                               {
                                   FileKey = i.FileKeyReference.ToDecrypedReference()
                               };
                           })
                        };

                        medicineSet = ser.Serialize<QhMedicineSetOtcDrugPhotoOfJson>(json);
                        break;
                    default:
                        break;
                }

                // 3
                // 登録処理へ
                var (wret,ret,seq) = _noteMedicineRepo.WriteMedicine(
                    args.ActorKey.TryToValueType<Guid>(Guid.Empty),
                    args.AuthorKey.TryToValueType<Guid>(Guid.Empty),
                    convertedMedicineSet,
                    (byte)dataType,
                    recordDate,
                    args.LinkageSystemNo.TryToValueType<int>(int.MinValue),
                    medicineSet,
                    (int)ownerType,
                    pharmacyNo,
                    ref tmpDataId,
                    ref refErrorMessage
                    );

                if (wret)
                {
                    dataId = tmpDataId;
                    result = ret;

                    if (ret && isRegisterEvent)
                    {

                        try
                        {
                            // お薬イベントを登録する
                            if (!_noteMedicineRepo.RegisterMedicineEvent(args, recordDate, seq))
                            {
                                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Error, string.Empty, "お薬のイベント登録に失敗しました。");
                            }
                        }
                        catch (Exception ex)
                        {
                            AccessLogWorker.WriteErrorLogAsync(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, string.Empty, ex);
                            refErrorMessage = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                refErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 編集データが処理可能なデータかチェックする（引数チェック）
        /// </summary>
        /// <param name="args"></param>
        /// <param name="refErrorMessage"></param>
        /// <returns></returns>
        private static bool CheckEditMedicineData(QoNoteMedicineEditApiArgs args, ref string refErrorMessage)
        {
            var result = false;
            refErrorMessage = string.Empty;

            // 引数チェック
            if (args.Data == null)
            {
                refErrorMessage = "Data: お薬手帳情報が不正です。";
            }
            else if (string.IsNullOrWhiteSpace(args.Data.DataType))
            {
                refErrorMessage = "Data.DataType: データ種別が不正です。";
            }
            else if (string.IsNullOrWhiteSpace(args.Data.OwnerType))
            {
                refErrorMessage = "Data.OwnerType: 情報提供元種別が不正です。";
            }
            else if (string.IsNullOrWhiteSpace(args.DataIdReference))
            {
                refErrorMessage = "DataIdReference: キー情報がありません。";
            }
            else
            {
                // データIDの実態は　RECEIPTNO = string.Format("{0}{1}", entity.RECORDDATE.ToString("yyyyMMdd"), entity.SEQUENCE.ToString("d6")) *お薬手帳アプリから入れた場合
                var dataid = args.DataIdReference.ToDecrypedReference();

                const int MIN_LENGTH = 10; // テーブル主キー（RecordDateとSequence）から生成する際に一意が保証されるであろう桁数

                // const int MAX_LENGTH  = 14; // JAHISでサポートされる分割制御データ固有IDの最大長（DBの最大長は20）
                //if( dataid.Length < MIN_LENGTH  || MAX_LENGTH < dataid.Length ) refErrorMessage = "データIDの桁数が不正です。" ;

                //14で制限すると医誠会データ(15)がぜんぶ引っかかる。どのみち一意にならなければ編集成立させないのでとりあえず20桁まで許容しておく
                if (dataid.Length < MIN_LENGTH || 20 < dataid.Length)
                {
                    refErrorMessage = "データIDの桁数が不正です。";
                }
            }

            // エラーありなら中断
            if (!string.IsNullOrWhiteSpace(refErrorMessage)) return false;

            // 種別のチェック
            QH_MEDICINE_DAT.DataTypeEnum dataType = (QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.DataType);
            QH_MEDICINE_DAT.OwnerTypeEnum ownerType = (QH_MEDICINE_DAT.OwnerTypeEnum)byte.Parse(args.Data.OwnerType);

            if (!Enum.IsDefined(typeof(QH_MEDICINE_DAT.DataTypeEnum), dataType) || dataType == QH_MEDICINE_DAT.DataTypeEnum.None)
            {
                refErrorMessage = "Data.DataType: データ種別の値が範囲外です。";
            }
            else if ((!Enum.IsDefined(typeof(QH_MEDICINE_DAT.OwnerTypeEnum), ownerType) || ownerType == QH_MEDICINE_DAT.OwnerTypeEnum.None))
            {
                refErrorMessage = "Data.OwnerType: 情報提供元種別の値が範囲外です。";
            }

            // エラーありなら中断
            if (!string.IsNullOrWhiteSpace(refErrorMessage)) return false;


            // 種別ごとのチェック
            switch (dataType)
            {
                case QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug:
                    // 調剤薬
                    if (string.IsNullOrWhiteSpace(args.Data.ConvertedMedicineSet))
                    {
                        refErrorMessage = "Data.ConvertedMedicineSet: お薬手帳データが不正です。";
                    }
                    break;
                case QH_MEDICINE_DAT.DataTypeEnum.OtcDrug:
                    // 市販薬
                    if (string.IsNullOrWhiteSpace(args.Data.ConvertedMedicineSet))
                    {
                        refErrorMessage = "Data.ConvertedMedicineSet: お薬手帳データが不正です。";
                    }
                    // これ入れたいが、いれると古いアプリからだと常にエラーとなってしまう。
                    //if (string.IsNullOrWhiteSpace(args.Data.OtcMedicineSet))
                    //{
                    //    refErrorMessage = "Data.OtcMedicineSet: お薬手帳データが不正です。";
                    //}
                    break;
                case QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto:
                    // 市販薬写真
                    if (args.Data.OtcPhotoMedicineSet != null)
                    {
                        refErrorMessage = "Data.OtcPhotoMedicineSet: お薬手帳データが不正です。";
                    }
                    else if (args.Data.OtcPhotoMedicineSet.FileKeyN != null ||
                        !args.Data.OtcPhotoMedicineSet.FileKeyN.Any())
                    {
                        refErrorMessage = "Data.OtcPhotoMedicineSet.FileKeyN: 添付ファイルは必須です。";
                    }
                    else if (args.Data.OtcPhotoMedicineSet.FileKeyN.Count > NoteMedicineWorker.MAX_COUNT_ATTACHED_FILE)
                    {
                        refErrorMessage = string.Format("Data.FileKeyN: 添付ファイル数は{0}個までです。", NoteMedicineWorker.MAX_COUNT_ATTACHED_FILE);
                    }
                    else if (string.IsNullOrWhiteSpace(args.Data.OtcPhotoMedicineSet.RecordDate))
                    {
                        refErrorMessage = "Data.RecordDate: 調剤日・購入日が不正です。";
                    }
                    break;
                case QH_MEDICINE_DAT.DataTypeEnum.Ssmix:
                    // SSMIX
                    if (string.IsNullOrWhiteSpace(args.Data.ConvertedMedicineSet))
                    {
                        refErrorMessage = "Data.ConvertedMedicineSet: お薬手帳データが不正です。";
                    }
                    break;
                default:
                    // JAHIS??
                    refErrorMessage = "Data.DataType: 対応不能なデータ種別です。";
                    break;
            }

            return string.IsNullOrWhiteSpace(refErrorMessage);
        }


        private bool EditMedicine(QoNoteMedicineEditApiArgs args, ref string refErrorMessage)
        {
            var result = false;
            var now = DateTime.Now;
            refErrorMessage = string.Empty;

            // 引数チェック
            if (!CheckEditMedicineData(args, ref refErrorMessage)) return false;


            QH_MEDICINE_DAT.DataTypeEnum dataType = (QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.DataType);
            QH_MEDICINE_DAT.OwnerTypeEnum ownerType = (QH_MEDICINE_DAT.OwnerTypeEnum)byte.Parse(args.Data.OwnerType);

            try
            {
                var ser = new QsJsonSerializer();
                var recordDate = DateTime.MinValue;
                var sequence = int.MinValue;
                var pharmacyNo = int.MinValue;
                string tmpDataId = args.DataIdReference.ToDecrypedReference();
                var medicineSet = string.Empty;
                string convertedMedicineSet = args.Data.ConvertedMedicineSet;
                var isRegisterEvent = false;
                JMVersionTypeEnum version = JMVersionTypeEnum.None;

                // 2
                // エンティティ生成
                switch (dataType)
                {
                    case QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug:
                    case QH_MEDICINE_DAT.DataTypeEnum.OtcDrug:
                    case QH_MEDICINE_DAT.DataTypeEnum.Ssmix:
                        // 調剤薬、市販薬（JAHISベース）
                        // JAHIS Json文字列のデシリアライズ
                        JM_Message jahis = new QsJsonSerializer().Deserialize<JM_Message>(args.Data.ConvertedMedicineSet);
                        jahis.Validate(JMOutputTypeEnum.None, ref version);

                        if (jahis.ValidationResults.Any())
                        {
                            // JAHIS検証エラーあり
                            var message = new StringBuilder();
                            foreach (JMValidationResult item in jahis.ValidationResults)
                            {
                                message.AppendLine(item.Message);
                            }

                            refErrorMessage = message.ToString().Replace(Environment.NewLine, " ");

                            // 中断
                            return false;
                        }

                        // 調剤日
                        if (!DateTime.TryParseExact(jahis.Prescription_List.First().No005.No005_2, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out recordDate))
                        {
                            refErrorMessage = "Data.RecordDate: 調剤日・購入日が不正です。";
                            return false;
                        }

                        // 薬局 医療機関番号（あれば）
                        if (jahis.Prescription_List != null && jahis.Prescription_List.Any() && jahis.Prescription_List.First().No011 != null)
                        {
                            pharmacyNo = jahis.Prescription_List.First().No011.No011_5.TryToValueType<int>(int.MinValue);
                        }

                        // 表示セット（未使用）
                        if (dataType == QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug ||
                            dataType == QH_MEDICINE_DAT.DataTypeEnum.Ssmix)
                        {
                            var jsonObj = new QhMedicineSetEthicalDrugOfJson();
                            jsonObj.PrescriptionDate = recordDate.ToApiDateString();
                            jsonObj.PharmacyId = pharmacyNo.ToString();

                            JM_Prescription prescriptionItem = jahis.Prescription_List.First();

                            foreach (JM_RpSet rpSetItem in prescriptionItem.RpSet_List)
                            {
                                // 処方
                                if (rpSetItem.Rp_List != null)
                                {
                                    foreach (JM_Rp rpItem in rpSetItem.Rp_List)
                                    {
                                        var medicineUsage = new QhMedicineSetUsageItemOfJson()
                                        {
                                            MedicineN = new List<QhMedicineSetEthicalDrugItemOfJson>()
                                        };

                                        // 処方―医師
                                        if (rpSetItem.No055 != null)
                                        {
                                            medicineUsage.DoctorId = string.Empty;
                                            medicineUsage.DoctorName = rpSetItem.No055.No055_2;
                                            medicineUsage.RepresentedOrganizationName = rpSetItem.No055.No055_3;
                                        }
                                        // 用法
                                        if (rpItem.No301 != null)
                                        {
                                            medicineUsage.DosageForm = rpItem.No301.No301_6.TryToValueType<QsDbDosageFormTypeEnum>(QsDbDosageFormTypeEnum.Other);
                                            medicineUsage.Usage = rpItem.No301.No301_3;
                                            medicineUsage.Days = rpItem.No301.No301_4;
                                            medicineUsage.Unit = rpItem.No301.No301_5;
                                        }
                                        if (rpItem.Medicine_List != null)
                                        {
                                            foreach (JM_Medicine medicineItem in rpItem.Medicine_List)
                                            {
                                                if (medicineItem.No201 != null)
                                                {
                                                    var medicine = new QhMedicineSetEthicalDrugItemOfJson();
                                                    // medicine.BrandCode = "";
                                                    // medicine.BrandName = "";
                                                    medicine.CodeSystem = medicineItem.No201.No201_6.TryToValueType<byte>(1);
                                                    // medicine.Comment = "";
                                                    medicine.Dose = medicineItem.No201.No201_4;
                                                    //'  medicine.IsGeneric = false;
                                                    medicine.MedicineCode = medicineItem.No201.No201_7;
                                                    medicine.MedicineName = medicineItem.No201.No201_3;
                                                    medicine.ReceiptNumber = medicineItem.No201.No201_2.TryToValueType<byte>(1);
                                                    medicine.Unit = medicineItem.No201.No201_5;

                                                    medicineUsage.MedicineN.Add(medicine);
                                                }
                                            }
                                        }

                                        jsonObj.MedicineUsageN.Add(medicineUsage);
                                    }
                                }
                            }

                            // 処方―医療機関
                            if (prescriptionItem.No051 != null)
                            {
                                jsonObj.FacilityName = prescriptionItem.No051.No051_2;
                                jsonObj.FacilityId = prescriptionItem.No051.No051_5;
                            }

                            // 調剤ー医療機関
                            if (prescriptionItem.No011 != null)
                            {
                                jsonObj.PharmacyName = prescriptionItem.No011.No011_2;
                            }

                            // 調剤-薬剤師
                            if (prescriptionItem.No015 != null)
                            {
                                jsonObj.PharmacistName = prescriptionItem.No015.No015_2;
                            }

                            // 残薬確認、レコード作成者
                            if (prescriptionItem.No421_List != null && prescriptionItem.No421_List.Count > 0)
                            {
                                foreach (JM_No421 item in prescriptionItem.No421_List)
                                {
                                    jsonObj.LeftoverMedicine += item.No421_2;
                                    if (string.IsNullOrWhiteSpace(jsonObj.LeftoverMedicineAuthor))
                                    {
                                        jsonObj.LeftoverMedicineAuthor = item.No421_3;
                                    }
                                }
                            }

                            // 特記事項複数ありそう？とりあえずまとめて入れとく。
                            if (prescriptionItem.No501_List != null && prescriptionItem.No501_List.Count > 0)
                            {
                                foreach (JM_No501 item in prescriptionItem.No501_List)
                                {
                                    jsonObj.SpecialNotes += item.No501_2;
                                }
                            }


                            // 患者メモ
                            if (prescriptionItem.No601_List != null && prescriptionItem.No601_List.Count > 0)
                            {
                                foreach (JM_No601 item in prescriptionItem.No601_List)
                                {
                                    jsonObj.Memo += item.No601_2;
                                }
                            }

                            medicineSet = ser.Serialize<QhMedicineSetEthicalDrugOfJson>(jsonObj);
                            isRegisterEvent = true;
                        }
                        else
                        {
                            var jsonObj = new QhMedicineSetOtcDrugOfJson();
                            jsonObj.PharmacyNo = pharmacyNo;

                            JM_Prescription prescriptionItem = jahis.Prescription_List.First();

                            // 薬局名
                            if (prescriptionItem.No011 != null)
                            {
                                jsonObj.PharmacyName = prescriptionItem.No011.No011_2;
                            }

                            // お薬メモ
                            //if (prescriptionItem.No601_List != null && prescriptionItem.No601_List.Any())
                            //{
                            //    foreach (JM_No601 item in prescriptionItem.No601_List)
                            //    {
                            //        jsonObj.Comment += item.No601_2;
                            //    }
                            //}


                            // 市販薬は004にいれる
                            if (jahis.No004_List != null && jahis.No004_List.Any())
                            {
                                foreach (JM_No004 item in jahis.No004_List)
                                {
                                    jsonObj.Comment += item.No004_2;
                                }
                            }

                            // お薬情報
                            if (dataType == QH_MEDICINE_DAT.DataTypeEnum.OtcDrug &&
                                args.Data.OtcMedicineSet != null &&
                                args.Data.OtcMedicineSet.OtcDrugN.Any())
                            {
                                foreach (QoApiMedicineOtcItem drug in args.Data.OtcMedicineSet.OtcDrugN)
                                {
                                    jsonObj.MedicineN.Add(new QhMedicineSetOtcDrugItemOfJson()
                                    {
                                        AttachedFileN = args.Data.OtcPhotoMedicineSet.FileKeyN.ConvertAll(i =>
                                        {
                                            return new QhAttachedFileOfJson() { FileKey = i.FileKeyReference.ToDecrypedReference() };
                                        }),
                                        ItemCode = drug.ItemCode,
                                        ItemCodeType = drug.ItemCodeType,
                                        MedicineName = drug.MedicineName
                                    });
                                }

                            }
                            else
                            {
                                // 旧来処理
                                if (jahis.No003_List != null)
                                {
                                    foreach (JM_No003 item in jahis.No003_List)
                                    {
                                        var medicineItem = new QhMedicineSetOtcDrugItemOfJson()
                                        {
                                            MedicineName = item.No003_2,
                                            AttachedFileN = new List<QhAttachedFileOfJson>()
                                        };
                                        jsonObj.MedicineN.Add(medicineItem);
                                    }
                                }
                            }
                            medicineSet = ser.Serialize<QhMedicineSetOtcDrugOfJson>(jsonObj);
                        }
                        break;
                    case QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto:
                        // 市販薬写真（ファイル、その他文字情報）

                        recordDate = args.Data.OtcPhotoMedicineSet.RecordDate.ToValueType<DateTime>();

                        var json = new QhMedicineSetOtcDrugPhotoOfJson()
                        {
                            PharmacyName = args.Data.OtcPhotoMedicineSet.PharmacyName,
                            Memo = args.Data.OtcPhotoMedicineSet.Memo,
                            AttachedFileN = args.Data.OtcPhotoMedicineSet.FileKeyN.ConvertAll(i =>
                            {
                                return new QhAttachedFileOfJson()
                                {
                                    FileKey = i.FileKeyReference.ToDecrypedReference()
                                };
                            })
                        };

                        medicineSet = ser.Serialize<QhMedicineSetOtcDrugPhotoOfJson>(json);
                        break;
                    default:
                        break;
                }

                // 3
                // 登録処理へ
                var (wret,ret, seq) = _noteMedicineRepo.WriteMedicine(
                   args.ActorKey.TryToValueType<Guid>(Guid.Empty),
                   args.AuthorKey.TryToValueType<Guid>(Guid.Empty),
                   convertedMedicineSet,
                   (byte)dataType,
                   recordDate,
                   args.LinkageSystemNo.TryToValueType<int>(int.MinValue),
                   medicineSet,
                   (int)ownerType,
                   pharmacyNo,
                   ref tmpDataId,
                   ref refErrorMessage
                   );

                if (wret)
                {
                    sequence = seq;
                    result = ret;

                    //if (ret && isRegisterEvent)
                    //{
                    //    try
                    //    {
                    //        // お薬イベントを登録する
                    //        if (!NoteMedicineWorker.RegisterMedicineEvent(args, recordDate, seq))
                    //        {
                    //            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Error, string.Empty, "お薬のイベント登録に失敗しました。");
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        AccessLogWorker.WriteErrorLogAsync(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, string.Empty, ex);
                    //        refErrorMessage = ex.Message;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {

                refErrorMessage = ex.Message;
            }
            return result;
        }

        private bool DeleteMedicine(QoNoteMedicineDeleteApiArgs args)
        {
            var result = false;

            var (ret, deletedKeys) = _noteMedicineRepo.DeleteMedicine(args.ActorKey.ToValueType<Guid>(), args.DataIdReference);

            if (ret)
            {
                if (deletedKeys != null && deletedKeys.Any())
                {
                    result = true;

                    //データ種別が調剤薬
                    var query = deletedKeys.
                        Where(x => x.DataType == (byte)QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug).ToList();
                    if (query != null && query.Any())
                    {
                        // イベントも削除
                        result = _noteMedicineRepo.DeleteMedicineEvent(args, deletedKeys);
                    }
                }               
            }

            return result;
        }

        private static string CreateOnetimeCode()
        {
            // Const SYSTEM_ID As String = "QS02" 'TODO：CCC用。web.configへ
            var result = string.Empty;

            // とりあえず最大5回リトライできるようにしておく
            for (int i = 0; i <= 5; i++)
            {
                // 乱数でワンタイムコードを生成
                var otc = string.Format("{0}X{1}", SYSTEM_ID, new Random().Next(1, 999999).ToString("d6")).ToUpper();

                // 未使用かどうかチェック
                if (NoteMedicineWorker.ReadOnetimeCode(otc) == Guid.Empty)
                {
                    result = otc;
                    break;
                }
                else
                {
                    // 使用中のワンタイムコード。リトライ
                    continue;
                }
            }

            return result;
        }

        private static Guid ReadOnetimeCode(string otc)
        {
            var result = Guid.Empty;

            var reader = new OnetimeCodeReader();
            var readerArgs = new OnetimeCodeReaderArgs() { OnetimeCode = otc };
            OnetimeCodeReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null)
            {
                if (readerResults.IsSuccess &&
                    readerResults.Result != null &&
                    readerResults.Result.Count == 1 &&
                    readerResults.Result.First() != null)
                {
                    result = readerResults.Result.First().ACCOUNTKEY;
                }
            }

            return result;
        }

        private static List<JM_Message> DecryptMedicineSet(List<QH_MEDICINE_DAT> entities)
        {
            var result = new List<JM_Message>();
            var ser = new QsJsonSerializer();

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                foreach (QH_MEDICINE_DAT entity in entities)
                {
                    JMVersionTypeEnum version = JMVersionTypeEnum.None;
                    JM_Message jahis = ser.Deserialize<JM_Message>(crypt.DecryptString(entity.CONVERTEDMEDICINESET));
                    if (jahis.Validate(JMOutputTypeEnum.None, ref version)) result.Add(jahis);
                }
            }

            return result;
        }

        private List<QoApiRecentMedicineItem> GetRecentMedicine(List<JM_Message> messages)
        {
            const string CODE_TYPE_YJ = "4"; // 薬品コード種別（4:YJコード）

            var result = new List<QoApiRecentMedicineItem>();
            var dic = new Dictionary<string, List<QoApiFileKeyItem>>();
            var dic2 = new Dictionary<string, DbEthicalDrugCategoryItem>();

            if (messages != null && messages.Any())
            {
                foreach (JM_Message message in messages)
                {
                    if (message.Prescription_List != null && message.Prescription_List.Any())
                    {
                        foreach (JM_Prescription prescription in message.Prescription_List)
                        {
                            if (prescription.RpSet_List != null && prescription.RpSet_List.Any())
                            {
                                foreach (JM_RpSet rpSet in prescription.RpSet_List)
                                {
                                    if (rpSet.Rp_List != null && rpSet.Rp_List.Any())
                                    {
                                        foreach (JM_Rp rp in rpSet.Rp_List)
                                        {
                                            if (rp.Medicine_List != null && rp.Medicine_List.Any())
                                            {
                                                foreach (JM_Medicine medicine in rp.Medicine_List)
                                                {
                                                    try
                                                    {
                                                        var item = new QoApiRecentMedicineItem()
                                                        {
                                                            DepartmentName = rpSet.No055 != null ? rpSet.No055.No055_3 : string.Empty,
                                                            HospitalName = prescription.No051 != null ? prescription.No051.No051_2 : string.Empty,
                                                            JanCode = string.Empty,
                                                            MedicinalEffectCode = string.Empty,
                                                            MedicinalEffectName = string.Empty,
                                                            MedicineName = medicine.No201.No201_3,
                                                            PharmacyName = prescription.No011 != null ? prescription.No011.No011_2 : string.Empty,
                                                            Quantity = medicine.No201.No201_4,
                                                            QuantityUnit = medicine.No201.No201_5,
                                                            RecordDate = prescription.No005.No005_2,
                                                            YjCode = medicine.No201.No201_6.CompareTo(CODE_TYPE_YJ) == 0 ? medicine.No201.No201_7.Trim() : string.Empty,
                                                            FileKeyN = new List<QoApiFileKeyItem>()
                                                        };

                                                        if (!string.IsNullOrWhiteSpace(item.YjCode) && !dic.ContainsKey(item.YjCode))
                                                        {
                                                            dic.Add(item.YjCode, null);
                                                            if (dic[item.YjCode] == null)
                                                            {
                                                                dic[item.YjCode] = _noteMedicineRepo.ReadEthicalDrugFileKey(item.YjCode).ConvertAll(i =>
                                                               {
                                                                   return new QoApiFileKeyItem()
                                                                   {
                                                                       Sequence = "",
                                                                       FileKeyReference = i.ToEncrypedReference()
                                                                   };
                                                               });
                                                            }

                                                            if (!dic2.ContainsKey(item.YjCode)) dic2.Add(item.YjCode, null);
                                                            if (dic2[item.YjCode] == null)
                                                            {
                                                                var yakkoList = _noteMedicineRepo.ReadYakkoList(new List<string>() { item.YjCode });
                                                                if(yakkoList != null && yakkoList.Any())
                                                                {
                                                                    dic2[item.YjCode] = yakkoList.First();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                // New DbEthicalDrugCategoryItem() With {.YjCode = "", .SubClassCode = "0000", .SubClassName = "未分類", .MinorClassCode = "000", .MinorClassName = "未分類", .MiddleClassCode = "00", .MiddleClassName = "未分類"}
                                                            }

                                                            if(dic2[item.YjCode] != null)
                                                            {
                                                                item.MedicinalEffectCode = dic2[item.YjCode].MinorClassCode;
                                                                item.MedicinalEffectName = dic2[item.YjCode].MinorClassName;
                                                            }
                                                            item.FileKeyN = dic[item.YjCode];

                                                            result.Add(item);
                                                        }
                                                        else
                                                        {
                                                            // YJコードなし（画像、薬効分類の紐付け不可）、or 追加済み
                                                        }
                                                    }
                                                    catch (Exception)
                                                    {

                                                        throw;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return result;
        }

        private static QoApiFileItem GetNoteMedicineImage(Guid fileKey, string actorKey, string executeSystemType, string executor, string executorName, QsApiFileTypeEnum fileType)
        {
            var result = new QoApiFileItem() { Sequence = "0", FileKeyReference = fileKey.ToEncrypedReference() };
            if (fileKey == Guid.Empty) throw new ArgumentNullException("FileKey", "ファイルキーが不正です。");

            QhBlobStorageReadApiResults apiResult = QoBlobStorage.Read<QH_UPLOADFILE_DAT>(
                new QhBlobStorageReadApiArgs()
                {
                    ActorKey = actorKey,
                    ApiType = QhApiTypeEnum.BlobStorageRead.ToString(),
                    ExecuteSystemType = executeSystemType,
                    Executor = executor,
                    ExecutorName = executorName,
                    FileKey = fileKey.ToApiGuidString(),
                    FileRelationType = QsApiFileRelationTypeEnum.MedicinePhoto.ToString(),
                    FileType = fileType.ToString()
                });

            if (apiResult != null)
            {
                if (apiResult.IsSuccess == bool.TrueString &&
                    !string.IsNullOrWhiteSpace(apiResult.Data) &&
                    !string.IsNullOrWhiteSpace(apiResult.ContentType))
                {
                    result.OriginalName = apiResult.OriginalName;
                    result.ContentType = apiResult.ContentType;
                    result.Data = apiResult.Data;
                }
            }
            return result;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// お薬手帳情報アクセスキーを取得します。（一般利用者向け）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNoteMedicineAccessKeyGenerateApiResults AccessKeyGenerate(QoNoteMedicineAccessKeyGenerateApiArgs args)
        {
            var result = new QoNoteMedicineAccessKeyGenerateApiResults { IsSuccess = bool.FalseString };
            var accountKey = args.ActorKey.TryToValueType<Guid>(Guid.Empty);

            // アカウントの存在確認
            if (AccountWorker.IsExistsAccount(accountKey))
            {
                result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(AccessKeyWorker.GetEncExeCutor(args.Executor), accountKey, Guid.Empty, (int)QoApiFunctionTypeEnum.CccAppUser, DateTime.UtcNow.AddDays(180));
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }

            return result;
        }

        /// <summary>
        /// お薬手帳情報アクセスキーを再生成します。（一般利用者向け）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNoteMedicineAccessKeyRefreshApiResults AccessKeyRefresh(QoNoteMedicineAccessKeyRefreshApiArgs args)
        {
            var result = new QoNoteMedicineAccessKeyRefreshApiResults { IsSuccess = bool.FalseString };
            var accountKey = args.ActorKey.TryToValueType<Guid>(Guid.Empty);

            // アカウントの存在確認
            if (AccountWorker.IsExistsAccount(accountKey))
            {
                result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(AccessKeyWorker.GetEncExeCutor(args.Executor), accountKey, Guid.Empty, (int)QoApiFunctionTypeEnum.CccAppUser, DateTime.UtcNow.AddDays(180));
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }

            return result;
        }

        /// <summary>
        /// お薬手帳情報の取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineReadApiResults Read(QoNoteMedicineReadApiArgs args)
        {
            var result = new QoNoteMedicineReadApiResults() { IsSuccess = bool.FalseString };

            List<QH_MEDICINE_DAT> entities;
            bool isModified;
            DateTime lastAccessDate;
            int pageIndex;
            int maxPageIndex;
            try
            {
                entities = _noteMedicineRepo.ReadMedicineList(args, out isModified, out lastAccessDate, out pageIndex, out maxPageIndex);
                if(entities == null)
                {
                    throw new Exception();
                }
            }
            catch(Exception ex)
            {
                //DB失敗
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, ex.Message);
                return result;
            }

            try 
            {
                var ser = new QsJsonSerializer();
                var dic = new Dictionary<DateTime, List<QH_MEDICINE_DAT>>();

                foreach (var entity in entities)
                {
                    if (!dic.ContainsKey(entity.RECORDDATE))
                    {
                        dic.Add(entity.RECORDDATE, new List<QH_MEDICINE_DAT>());
                    }
                    dic[entity.RECORDDATE].Add(entity);
                }

                if (dic.Any())
                {
                    using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                    {
                        foreach (KeyValuePair<DateTime, List<QH_MEDICINE_DAT>> kvp in dic)
                        {
                            result.DataN.Add(new QoApiNoteMedicineHeaderItem()
                            {
                                RecordDate = kvp.Key.ToApiDateString(),
                                MedicineSetN = kvp.Value.ConvertAll(i =>
                                {
                                    var item = new QoApiNoteMedicineDetailItem()
                                    {
                                        DataType = i.DATATYPE.ToString(),
                                        OwnerType = i.OWNERTYPE.ToString(),
                                        DataIdReference = i.RECEIPTNO.ToEncrypedReference(),
                                        ConvertedMedicineSet = "",
                                        OtcPhotoMedicineSet = new QoApiMedicineSetOtcPhotoItem()
                                    };

                                    if (!string.IsNullOrWhiteSpace(i.CONVERTEDMEDICINESET))
                                    {
                                        item.ConvertedMedicineSet = crypt.DecryptString(i.CONVERTEDMEDICINESET);
                                    }

                                    if (i.DATATYPE == (byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrug && !string.IsNullOrWhiteSpace(i.MEDICINESET))
                                    {
                                        item.OtcMedicineSet = ser.Deserialize<QhMedicineSetOtcDrugOfJson>(crypt.DecryptString(i.MEDICINESET)).ToApiMedicineSetOtcItem(kvp.Key);
                                    }

                                    if (i.DATATYPE == (byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto && !string.IsNullOrWhiteSpace(i.MEDICINESET))
                                    {
                                        item.OtcPhotoMedicineSet = ser.Deserialize<QhMedicineSetOtcDrugPhotoOfJson>(crypt.DecryptString(i.MEDICINESET)).ToApiMedicineSetOtcPhotoItem(kvp.Key);
                                    }

                                    return item;
                                })
                            });
                        }
                    }
                }

                result.IsForceReload = isModified.ToString();
                result.LastAccessDate = lastAccessDate.ToString();
                result.PageIndex = pageIndex.ToString();
                result.MaxPageIndex = maxPageIndex.ToString();                

                if (isModified)
                {
                    result.Result = new QoApiResultItem()
                    {
                        Code = "2001",
                        Detail = "データが修正されています。全てのデータを再読み込みしてください。"
                    };
                }
                else
                {
                    result.Result = new QoApiResultItem()
                    {
                        Code = "0200",
                        Detail = "正常に終了しました。"
                    };
                }                
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError,ex.Message);
                return result;
            }

            result.IsSuccess = bool.TrueString;
            return result;
        }

        /// <summary>
        /// お薬手帳情報の追加
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineAddApiResults Add(QoNoteMedicineAddApiArgs args)
        {
            if (args.Data == null) throw new ArgumentNullException("args.Data", "お薬手帳情報が不正です。");

            var result = new QoNoteMedicineAddApiResults() { IsSuccess = bool.FalseString };
            var errorMessage = string.Empty;

            var dataId = string.Empty;
            result.IsSuccess = this.AddMedicine(args, ref dataId, ref errorMessage).ToString();

            if (result.IsSuccess == bool.TrueString)
            {
                result.DataIdReference = dataId.ToEncrypedReference();
                result.Result = new QoApiResultItem() { Code = "0200", Detail = "正常に終了しました。" };
            }
            else if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                result.Result = new QoApiResultItem() { Code = "2011", Detail = errorMessage };
            }
            else
            {
                result.Result = new QoApiResultItem() { Code = "2099", Detail = "お薬手帳情報登録でエラーが発生しました。" };
            }

            return result;
        }

        /// <summary>
        /// お薬手帳情報の更新
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineEditApiResults Edit(QoNoteMedicineEditApiArgs args)
        {
            if (args.Data == null) throw new ArgumentNullException("args.Data", "お薬手帳情報が不正です。");

            var result = new QoNoteMedicineEditApiResults() { IsSuccess = bool.FalseString };
            var errorMessage = string.Empty;

            var dataId = string.Empty;
            result.IsSuccess = this.EditMedicine(args, ref errorMessage).ToString();

            if (result.IsSuccess == bool.TrueString)
            {
                result.DataIdReference = args.DataIdReference;
                result.Result = new QoApiResultItem() { Code = "0200", Detail = "正常に終了しました。" };
            }
            else if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                result.Result = new QoApiResultItem() { Code = "2011", Detail = errorMessage };
            }
            else
            {
                result.Result = new QoApiResultItem() { Code = "2099", Detail = "お薬手帳情報更新でエラーが発生しました。" };
            }

            return result;
        }

        /// <summary>
        /// お薬手帳情報の削除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineDeleteApiResults Delete(QoNoteMedicineDeleteApiArgs args)
        {
            var result = new QoNoteMedicineDeleteApiResults() { IsSuccess = bool.FalseString };

            var isSuccess = this.DeleteMedicine(args);

            if (isSuccess)
            {
                result.IsSuccess = isSuccess.ToString();
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError);
            }

            return result;
        }

        /// <summary>
        /// ユーザーIDの取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //public static QoNoteMedicineUserIdReadApiResults UserIdRead(QoNoteMedicineUserIdReadApiArgs args)
        //{

        //}

        /// <summary>
        /// e薬Link用のワンタイムコードを生成。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNoteMedicineOnetimeCodeGenerateApiResults OnetimeCodeGenerate(QoNoteMedicineOnetimeCodeGenerateApiArgs args)
        {
            var result = new QoNoteMedicineOnetimeCodeGenerateApiResults() { IsSuccess = bool.FalseString };

            result.OnetimeCode = NoteMedicineWorker.WriteELinkOnetimeCode(args.ActorKey.TryToValueType<Guid>(Guid.Empty));
            result.Expires = DateTime.Now.AddMinutes(30).ToApiDateString();
            result.IsSuccess = bool.TrueString;
            result.Result = new QoApiResultItem() { Code = "0200", Detail = "正常に終了しました。" };

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public static string WriteELinkOnetimeCode(Guid accountKey)
        {
            var result = string.Empty;
            var now = DateTime.Now;
            string otc = NoteMedicineWorker.CreateOnetimeCode();

            // TODO: 運用にあたっては、期限切れコードを削除していく機能も必要となる
            var entity = new QH_ELINKONETIMECODE_DAT()
            {
                ACCOUNTKEY = accountKey,
                REQUESTEDDATE = now,
                ONETIMECODE = otc,
                EXPIRES = now.AddMinutes(30),
                DELETEFLAG = false,
                CREATEDDATE = now,
                UPDATEDDATE = now
            };

            var writer = new QhELinkOnetimeCodeEntityWriter();
            var writerArgs = new QhELinkOnetimeCodeEntityWriterArgs() { Data = new List<QH_ELINKONETIMECODE_DAT>() { entity } };
            QhELinkOnetimeCodeEntityWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

            if (writerResults != null)
            {
                if (writerResults.IsSuccess && writerResults.Result == 1)
                {
                    result = otc;
                }
            }
            return result;
        }

        /// <summary>
        /// 最近のお薬情報の取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineRecentMedicineReadApiResults RecentMedicineRead(QoNoteMedicineRecentMedicineReadApiArgs args)
        {
            var result = new QoNoteMedicineRecentMedicineReadApiResults() { IsSuccess = bool.FalseString };

            try
            {
                var isModified = false;
                var lastAccessDate = DateTime.MinValue;
                var pageIndex = int.MinValue;
                var maxPageIndex = int.MinValue;

                // とりあえず直近90日分をリストアップしてみる
                List<QH_MEDICINE_DAT> entities = _noteMedicineRepo.ReadMedicineList(
                    new QoNoteMedicineReadApiArgs()
                    {
                        ActorKey = args.ActorKey,
                        ApiType = QoApiTypeEnum.NoteMedicineRead.ToString(),
                        DataType = "1",
                        DaysPerPage = "180",
                        ExecuteSystemType = args.ExecuteSystemType,
                        Executor = args.Executor,
                        ExecutorName = args.ExecutorName,
                        LastAccessDate = DateTime.Now.ToApiDateString(),
                        PageIndex = "0"
                    },
                    out isModified,
                    out lastAccessDate,
                    out pageIndex,
                    out maxPageIndex
                );

                if (entities == null)
                {
                    // DB失敗
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                }
                else
                {
                    // 成功

                    // JAHISエンティティにパース
                    // 最近のお薬を取得（薬効分類、ファイルキー）
                    result.DataN = this.GetRecentMedicineItems(entities);

                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// お薬情報リストから最近のお薬情報リストを取得
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<QoApiRecentMedicineItem> GetRecentMedicineItems(List<QH_MEDICINE_DAT> entities)
        {
            // JAHISエンティティにパース
            List<JM_Message> medicineSetN = NoteMedicineWorker.DecryptMedicineSet(entities);

            // 最近のお薬を取得（薬効分類、ファイルキー）
            return this.GetRecentMedicine(medicineSetN);
        }

        /// <summary>
        /// お薬手帳画像の取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNoteMedicineImageReadApiResults ImageRead(QoNoteMedicineImageReadApiArgs args)
        {
            var result = new QoNoteMedicineImageReadApiResults() { IsSuccess = bool.FalseString };
            if (args.FileKey == null) throw new ArgumentNullException("FileKey", "ファイルキー情報が不正です。");
            if (string.IsNullOrWhiteSpace(args.FileKey.FileKeyReference)) throw new ArgumentNullException("FileKey.FileKeyReference", "ファイルキー参照文字列が不正です。");

            result.Image = NoteMedicineWorker.GetNoteMedicineImage(
                args.FileKey.FileKeyReference.ToDecrypedReference<Guid>(),
                args.ActorKey,
                args.ExecuteSystemType,
                args.Executor,
                args.ExecutorName,
                QsApiFileTypeEnum.Original
            );
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return result;
        }

        /// <summary>
        /// お薬手帳画像の登録
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNoteMedicineImageWriteApiResults ImageWrite(QoNoteMedicineImageWriteApiArgs args)
        {
            var result = new QoNoteMedicineImageWriteApiResults() { IsSuccess = bool.FalseString };

            QhBlobStorageWriteApiResults apiResults = QoBlobStorage.Write<QH_UPLOADFILE_DAT>(
                new QhBlobStorageWriteApiArgs()
                {
                    ActorKey = args.ActorKey,
                    ApiType = QhApiTypeEnum.BlobStorageWrite.ToString(),
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName,
                    OriginalName = args.Image.OriginalName,
                    FileRelationType = QsApiFileRelationTypeEnum.MedicinePhoto.ToString(),
                    ContentType = args.Image.ContentType,
                    Data = args.Image.Data
                }
                );

            if (apiResults != null)
            {
                if (apiResults.IsSuccess == bool.TrueString && !string.IsNullOrWhiteSpace(apiResults.FileKey))
                {
                    var fileKey = apiResults.FileKey.TryToValueType<Guid>(Guid.Empty);
                    if (fileKey != Guid.Empty)
                    {
                        result.FileKey = new QoApiFileKeyItem() { Sequence = "0", FileKeyReference = fileKey.ToEncrypedReference() };
                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// お薬手帳用の利用規約を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineTermsReadApiResults TermsRead(QoNoteMedicineTermsReadApiArgs args)
        {
            var result = new QoNoteMedicineTermsReadApiResults() { IsSuccess = bool.FalseString };

            QoApiNoteMedicineTermsItem termItem;
            try
            {
                termItem = _noteMedicineRepo.ReadTermsItem((byte)args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.QolmsiOSApp));

                if (termItem == null)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "データが存在しません。");
                    return result;

                }

                result.TermsItem = termItem;
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            return result;
        }

        #endregion
    }
}