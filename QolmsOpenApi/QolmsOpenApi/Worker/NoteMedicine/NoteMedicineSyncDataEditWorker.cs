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
    /// お薬手帳 同期データ編集用
    /// </summary>
    public class NoteMedicineSyncDataEditWorker : NoteMedicineSyncWorkerBase
    {
        enum EditRoleType
        {
            None, 
            Memo,           
            All, 
        }

        struct ConvertedArgs
        {
            public Guid AccountKey { get; set; }
            public Guid AuthorKey { get; set; }
            public DataTypeEnum DataType { get; set; }
            public OwnerTypeEnum OwnerType { get; set; }
            public DateTime RecordDate { get; set; }
            public int Sequence { get; set; }
            public QH_MEDICINE_DAT Entity { get; set; }
            public JM_Message Jahis { get; set; }
            public QoNoteMedicineDetail Data { get; set; }
            public EditRoleType RoleType { get; set; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        public NoteMedicineSyncDataEditWorker(INoteMedicineRepository noteMedicineRepository) : base(noteMedicineRepository)
        {            
        }

        /// <summary>
        /// 編集処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineSyncDataEditApiResults Edit(QoNoteMedicineSyncDataEditApiArgs args)
        {
            var results = new QoNoteMedicineSyncDataEditApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 所有者キーチェック
            if (!args.AuthorKey.CheckArgsConvert(nameof(args.AuthorKey), Guid.Empty, results, out var authorKey))
            {
                return results;
            }

            if(args.Data == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Data)}が不正です。");
                return results;
            }

            // オーナータイプチェック
            if (!args.Data.OwnerType.CheckArgsEnumConvert(nameof(args.Data.OwnerType), OwnerTypeEnum.None, results, out var ownerType))
            {
                return results;
            }

            // データタイプチェック
            if (!args.Data.DataType.CheckArgsEnumConvert(nameof(args.Data.DataType), DataTypeEnum.None, results, out var dataType))
            {
                return results;
            }

            // オーナータイプ 非対応チェック
            switch (ownerType)
            {
                case OwnerTypeEnum.Oneself:
                case OwnerTypeEnum.QrCode:
                case OwnerTypeEnum.Data:
                    break;
                default:
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "OwnerTypeが未対応です");
                    return results;
            }

            // OwnerType DataType組み合わせチェック
            if (!CheckDataType(ownerType, dataType, out _))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Data.OwnerType)}と{nameof(args.Data.DataType)}の組み合わせが不正です。");
                return results;
            }

            // RecordDateチェック
            if (!args.RecordDate.CheckArgsConvert(nameof(args.RecordDate),DateTime.MinValue,results, out var recordDate))
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
            
            // 対象のレコードのOwnerType DataTypeの組み合わせチェック
            if (!CheckDataType((OwnerTypeEnum)entity.OWNERTYPE, (DataTypeEnum)entity.DATATYPE, out var editRoleType))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"編集不可能なデータです。");
                return results;
            }

            // Jahisデータの復号
            var jahisSource = entity.CONVERTEDMEDICINESET.TryDecrypt();
            if (string.IsNullOrEmpty(jahisSource))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "Jahisデータの復元に失敗しました。");
                return results;
            }
            // Jahisデータのデシリアライズ
            var jahis = new QsJsonSerializer().Deserialize<JM_Message>(jahisSource);
            if (jahis == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "Jahisフォーマットの変換に失敗しました。");
                return results;
            }           

            var convertedArgs = new ConvertedArgs
            {
                AccountKey = accountKey,
                AuthorKey = authorKey,
                DataType = dataType,
                OwnerType = ownerType,
                RecordDate = recordDate,
                Sequence = args.Sequence,
                Jahis = jahis,
                Data = args.Data,
                RoleType = editRoleType,
                Entity = entity
            };

            if(dataType == DataTypeEnum.EthicalDrug || dataType == DataTypeEnum.Ssmix)
            {               
                return EditForEth(convertedArgs, results);
            }

            return EditForOtc(convertedArgs, results);
        }

        private QoNoteMedicineSyncDataEditApiResults EditForEth(ConvertedArgs args, QoNoteMedicineSyncDataEditApiResults results)
        {
            // 編集箇所の検索
            (var prescription, var rpSet) = FindTargetJahisItem(args.Jahis, args.Data.PrescriptionNo, args.Data.RpSetNo);
            if (prescription == null || rpSet == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NoteMedicineNoEditJahisData, "編集対象の項目が存在しませんでした。");
                return results;
            }            

            // メモの反映
            var splitMemo = SpliteBreakLine(args.Data.Memo);
            prescription.No601_List = splitMemo.Select(x => new JM_No601
            {
                No601_1 = "601",
                No601_2 = x,
                No601_3 = DateTime.Today.ToString("yyyyMMdd")
            }).ToList();

            // 編集権限がある場合のみ他の項目も反映
            if (args.RoleType == EditRoleType.All)
            {
                var builder = new JahisPrescriptionBuilder();

                // 引数の変更内容が反映された一時的なJM_Prescriptionを生成する
                var newPrescription = builder
                .SetPharmacy(args.Data.PharmacyName, args.Data.PharmacistName, args.Entity.PHARMACYNO)
                .SetHospital(args.Data.HospitalName, prescription.No051?.No051_5 ?? string.Empty)
                .AddRpSet(rpSetCfg =>
                {
                    rpSetCfg
                    .SetDoctor(args.Data.DoctorName)
                    .SetDepartment(args.Data.DepartmentName);

                    foreach (var rpItem in args.Data.RpItems)
                    {
                        rpSetCfg.AddRp(rp =>
                        {
                            rp
                            .SetUsage(rpItem.UsageName, SpliteBreakLine(rpItem.UsageSupplement))
                            .SetDosageFormCode(rpItem.DosageFormCode)
                            .SetQuantity(rpItem.Quantity, rpItem.QuantityUnit);

                            foreach (var med in rpItem.MedicineItems)
                            {
                                rp.AddMedicine(med.Name, med.Quantity, med.QuantityUnit, med.YjCode);
                            }
                        });
                    }
                })
                .Build();

                // 変更箇所を反映
                prescription.No011 = newPrescription.No011;
                prescription.No015 = newPrescription.No015;
                prescription.No051 = newPrescription.No051;
                rpSet.No055 = newPrescription.RpSet_List[0].No055;
                rpSet.Rp_List = newPrescription.RpSet_List[0].Rp_List;
            }

            try
            {
                var jahis = args.Jahis;
                // バリデーション実行
                var version = JMVersionTypeEnum.None;
                jahis.Validate(JMOutputTypeEnum.None, ref version);
                if (jahis.ValidationResults.Any())
                {
                    var err = string.Join("/", jahis.ValidationResults.Select(x => x.Message));
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"JahisDataの検証でエラーが発生しました。Error:{err}");
                    return results;
                }
            }
            catch (Exception ex)
            {
                // Validate内部で例外が発生することがある 必須項目でnullが設定されているなど
                results.Result = QoApiResult.Build(ex, "JahisDataの検証で想定外のエラーが発生しました。");
                return results;
            }

            // 更新したJahisDataをセット
            args.Entity.CONVERTEDMEDICINESET = new QsJsonSerializer().Serialize(args.Jahis);
            // MedicineSetを生成しなおしてセット
            args.Entity.MEDICINESET = CreateEthMedicineSet(args.RecordDate, args.Entity.PHARMACYNO, args.Jahis);

            // DB更新
            if (!TryEditEntity(args, results))
            {
                return results;
            }

            // 登録した情報を取得
            if (!TryReadEntity(args.AccountKey, args.RecordDate, args.Sequence, results, out var entity))
            {
                // 部分的成功
                // 登録自体は正常に完了しているので一応正常として返す
                results.IsSuccess = bool.TrueString;
                results.Result.Code = $"{(int)QoApiResultCodeTypeEnum.NoteMedicineAddPartialSuccess:D4}";
                results.Result.Detail = $"正常に登録できましたが、戻り値用のデータは取得できませんでした。Error: {results.Result.Detail}";


                return results;
            }

            // データ変換
            var noteMedicineItem = ConvertNoteMedicineEntity(entity);

            // 薬効情報を適用
            if (!TryApplyYakkoData(new List<QoNoteMedicineItem> { noteMedicineItem }, results))
            {
                // 部分的成功
                // 登録自体は正常に完了しているので一応正常として返す
                results.IsSuccess = bool.TrueString;
                results.Result.Code = $"{(int)QoApiResultCodeTypeEnum.NoteMedicineAddPartialSuccess:D4}";
                results.Result.Detail = $"正常に登録できましたが、戻り値用のデータは取得できませんでした。Error: {results.Result.Detail}";
            }

            results.Data = noteMedicineItem;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsSuccess = bool.TrueString;

            return results;
        }

        private QoNoteMedicineSyncDataEditApiResults EditForOtc(ConvertedArgs args, QoNoteMedicineSyncDataEditApiResults results)
        {
            // 編集箇所の検索
            var prescription = args.Jahis.Prescription_List.ElementAtOrDefault(0);
            if(prescription == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NoteMedicineNoEditJahisData, "編集対象の項目が存在しませんでした。");
                return results;
            }            

            // メモの反映
            var splitMemo = SpliteBreakLine(args.Data.Memo);
            args.Jahis.No004_List = splitMemo.Select(x => new JM_No004
            {
                No004_1 = "4",
                No004_2 = x,
                No004_3 = DateTime.Today.ToString("yyyyMMdd"),
                No004_4 = "2"
            }).ToList();

            // 編集権限がある場合のみ他の項目も反映
            if (args.RoleType == EditRoleType.All)
            {
                // 市販薬情報 反映
                args.Jahis.No003_List = args.Data.MedicineItems.Select(x => new JM_No003
                {
                    No003_1 = "3",
                    No003_2 = x.Name,
                    No003_3 = string.Empty,
                    No003_4 = string.Empty,
                    No003_5 = "2",
                }).ToList();

                var builder = new JahisPrescriptionBuilder();

                // 引数の変更内容が反映された一時的なJM_Prescriptionを生成する
                var newPrescription = builder
                .SetPharmacy(args.Data.PharmacyName, args.Data.PharmacistName, args.Entity.PHARMACYNO)
                .Build();

                // 変更箇所を反映
                prescription.No011 = newPrescription.No011;
                prescription.No015 = newPrescription.No015;
            }

            try
            {
                var jahis = args.Jahis;
                // バリデーション実行
                var version = JMVersionTypeEnum.None;
                jahis.Validate(JMOutputTypeEnum.None, ref version);
                if (jahis.ValidationResults.Any())
                {
                    var err = string.Join("/", jahis.ValidationResults.Select(x => x.Message));
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"JahisDataの検証でエラーが発生しました。Error:{err}");
                    return results;
                }
            }
            catch (Exception ex)
            {
                // Validate内部で例外が発生することがある 必須項目でnullが設定されているなど
                results.Result = QoApiResult.Build(ex, "JahisDataの検証で想定外のエラーが発生しました。");
                return results;
            }

            // 更新したJahisDataをセット
            args.Entity.CONVERTEDMEDICINESET = new QsJsonSerializer().Serialize(args.Jahis);
            // MedicineSetを生成しなおしてセット
            args.Entity.MEDICINESET = CreateOtcMedicineSet(args.Entity.PHARMACYNO, args.Data);

            // DB更新
            if (!TryEditEntity(args, results))
            {
                return results;
            }

            // 登録した情報を取得
            if (!TryReadEntity(args.AccountKey, args.RecordDate, args.Sequence, results, out var entity))
            {
                // 部分的成功
                // 登録自体は正常に完了しているので一応正常として返す
                results.IsSuccess = bool.TrueString;
                results.Result.Code = $"{(int)QoApiResultCodeTypeEnum.NoteMedicineAddPartialSuccess:D4}";
                results.Result.Detail = $"正常に登録できましたが、戻り値用のデータは取得できませんでした。Error: {results.Result.Detail}";


                return results;
            }

            // データ変換
            var noteMedicineItem = ConvertNoteMedicineEntity(entity);         

            results.Data = noteMedicineItem;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsSuccess = bool.TrueString;

            return results;
        }

        private bool TryEditEntity(ConvertedArgs args, QoApiResultsBase results)
        {
            try
            {
                _noteRepo.UpdateEntity(args.Entity);                
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お薬手帳情報の更新処理でエラーが発生しました。");
                return false;
            }
        }

        private (JM_Prescription prescription, JM_RpSet rpSet) FindTargetJahisItem(JM_Message jahis, int preNo, int rpSetNo)
        {
            JM_RpSet targetRpSet = null;

            var targetPrescription = jahis.Prescription_List.ElementAtOrDefault(preNo - 1);
            if (targetPrescription != null)
            {
                targetRpSet = targetPrescription.RpSet_List.ElementAtOrDefault(rpSetNo - 1);
            }          

            return (targetPrescription, targetRpSet);
        }

        private bool CheckDataType(OwnerTypeEnum ownerType, DataTypeEnum dataType, out EditRoleType editRoleType)
        {
            switch (ownerType)
            {
                case OwnerTypeEnum.Oneself:
                    editRoleType = EditRoleType.All;
                    return dataType == DataTypeEnum.EthicalDrug || dataType == DataTypeEnum.OtcDrug;
                case OwnerTypeEnum.QrCode:
                    editRoleType = EditRoleType.Memo;
                    return dataType == DataTypeEnum.EthicalDrug;
                case OwnerTypeEnum.Data:
                    editRoleType = EditRoleType.Memo;
                    return dataType == DataTypeEnum.Ssmix;
                default:
                    editRoleType = EditRoleType.None;
                    return false;
            }
        }
    }
}