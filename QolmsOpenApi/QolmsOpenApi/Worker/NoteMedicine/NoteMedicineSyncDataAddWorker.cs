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

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// お薬手帳 同期データ追加用
    /// </summary>
    public class NoteMedicineSyncDataAddWorker : NoteMedicineSyncWorkerBase
    {
        struct ConvertedArgs
        {
            public Guid AccountKey { get; set; }
            public Guid AuthorKey { get; set; }
            public DataTypeEnum DataType { get; set; }
            public OwnerTypeEnum OwnerType { get; set; }
            public int LinkageSystemNo { get; set; }
            public string JahisData { get; set; }
            public List<string> JahisMemoList { get; set; }
            public QoNoteMedicineDetail Data { get; set; }
        }

        private IAccountRepository _accountRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        /// <param name="accountRepository"></param>
        public NoteMedicineSyncDataAddWorker(INoteMedicineRepository noteMedicineRepository, IAccountRepository accountRepository) : base(noteMedicineRepository)
        {
            _accountRepo = accountRepository;
        }

        /// <summary>
        /// 追加処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNoteMedicineSyncDataAddApiResults Add(QoNoteMedicineSyncDataAddApiArgs args)
        {
            var results = new QoNoteMedicineSyncDataAddApiResults
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

            // オーナータイプチェック
            if (!args.OwnerType.CheckArgsEnumConvert(nameof(args.OwnerType), OwnerTypeEnum.None, results, out var ownerType))
            {
                return results;
            }

            // データタイプチェック
            if(!args.DataType.CheckArgsEnumConvert(nameof(args.DataType),DataTypeEnum.None, results, out var dataType))
            {
                return results;
            }

            // オーナータイプ 非対応値チェック
            if(ownerType != OwnerTypeEnum.Oneself && ownerType != OwnerTypeEnum.QrCode)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.OwnerType)}が未対応です。");
                return results;
            }

            if(!CheckDataType(ownerType, dataType))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.OwnerType)}と{nameof(args.DataType)}の組み合わせが不正です。");
                return results;
            }

            var convertedArgs = new ConvertedArgs
            {
                AccountKey = accountKey,
                AuthorKey = authorKey,
                LinkageSystemNo = args.LinkageSystemNo,
                DataType = dataType,
                OwnerType = ownerType,
                JahisData = args.JahisData,
                JahisMemoList = args.JahisMemoList,
                Data = args.Data
            };

            if (!string.IsNullOrEmpty(args.JahisData))
            {
                // JahisDataがある場合はJahisからの登録処理へ
                return AddFromJhais(convertedArgs, results);
            }

            // それ以外はデータからの登録処理へ
            return AddFromData(convertedArgs, results);
        }

        private QoNoteMedicineSyncDataAddApiResults AddFromJhais(ConvertedArgs args,QoNoteMedicineSyncDataAddApiResults results)
        {
            if(args.DataType == DataTypeEnum.OtcDrug)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"JahisDataは市販薬には対応していません。");
                return results;
            }

            // Jashiフォーマットにデシリアライズ
            var jahis = new QsJsonSerializer().Deserialize<JM_Message>(args.JahisData);
            if(jahis == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Jahisフォーマットの変換に失敗しました。");
                return results;
            }

            var prescription = jahis.Prescription_List.FirstOrDefault();
            // 処方情報チェック
            if (prescription == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処方情報が存在しません。");
                return results;
            }

            // 患者等記入情報を設定
            for(var i = 0; i < jahis.Prescription_List.Count; i++)
            {
                var item = jahis.Prescription_List[i];
                var memo = args.JahisMemoList.ElementAtOrDefault(i);
                if(memo == null)
                {
                    continue;
                }

                // 改行コードで分割する
                var memoList = SpliteBreakLine(memo);

                item.No601_List = memoList.Select(x => new JM_No601
                {
                    No601_1 = "601",
                    No601_2 = x,
                    No601_3 = DateTime.Today.ToString("yyyyMMdd")
                }).ToList();
            }
            // JahisDataに反映
            args.JahisData = new QsJsonSerializer().Serialize(jahis);

            // 調剤日 抽出
            if (!DateTime.TryParseExact(prescription.No005?.No005_2, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var recordDate))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "調剤日が不正です。");
                return results;
            }

            try
            {
                // バリデーション実行
                var version = JMVersionTypeEnum.None;
                jahis.Validate(JMOutputTypeEnum.None, ref version);
                if (jahis.ValidationResults.Any())
                {
                    var err = string.Join("/", jahis.ValidationResults.Select(x => x.Message));
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"JahisDataの検証でエラーが発生しました。Error:{err}");
                    return results;
                }
            }
            catch (Exception ex)
            {
                // Validate内部で例外が発生することがある 必須項目でnullが設定されているなど
                results.Result = QoApiResult.Build(ex, "JahisDataの検証で想定外のエラーが発生しました。");
                return results;
            }

            // 薬局 医療機関番号（ない場合はint.MinValue)
            var pharmacyNo = prescription.No011?.No011_5?.TryToValueType(int.MinValue) ?? int.MinValue;

            // 調剤薬用MedicineSet生成
            var medicineSet = CreateEthMedicineSet(recordDate, pharmacyNo, jahis);
            

            // データ登録
            if (!TryWriteEntity(args, recordDate, pharmacyNo, medicineSet, results, out var seq))
            {
                return results;
            }            

            // 登録した情報を取得
            if(!TryReadEntity(args.AccountKey, recordDate, seq, results, out var entity))
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

        private QoNoteMedicineSyncDataAddApiResults AddFromData(ConvertedArgs args, QoNoteMedicineSyncDataAddApiResults results)
        {
            // RecordDateチェック
            if (!args.Data.RecordDate.CheckArgsConvert(nameof(args.Data.RecordDate), DateTime.MinValue, results, out var recordDate))
            {
                return results;
            }

            // 薬局番号
            var pharmacyNo = args.Data.PharmacyNo;

            // ユーザー情報取得
            if (!TryReadAccountEntity(args.AccountKey, results, out var accountDat))
            {
                return results;
            }
            

            JM_Message jahis;
            string medicineSet;
            try
            {
                switch (args.DataType)
                {
                    case DataTypeEnum.EthicalDrug:
                        // jahis変換
                        jahis = EthDataToJahis(args.Data, accountDat);
                        // MedicineSet変換
                        medicineSet = CreateEthMedicineSet(recordDate, pharmacyNo, jahis);
                        break;
                    case DataTypeEnum.OtcDrug:
                        jahis = OtcDataToJahis(args.Data, accountDat);
                        medicineSet = CreateOtcMedicineSet(pharmacyNo, args.Data);
                        break;
                    default:
                        // 上流でチェックしているので基本的にはここは通らない
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"データの追加はDataTypeが調剤薬または市販薬のみが対象です。");
                        return results;
                }
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "データの変換に失敗しました。");
                return results;
            }
            try
            {
                // バリデーション実行
                var version = JMVersionTypeEnum.Latest;
                jahis.Validate(JMOutputTypeEnum.None, ref version);
                if (jahis.ValidationResults.Any())
                {
                    var err = string.Join("/", jahis.ValidationResults.Select(x => x.Message));
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"JahisDataの検証でエラーが発生しました。Error:{err}");
                    return results;
                }
            }
            catch(Exception ex)
            {
                // Validate内部で例外が発生することがある 必須項目でnullが設定されているなど
                results.Result = QoApiResult.Build(ex, "JahisDataの検証で想定外のエラーが発生しました。");
                return results;
            }
            // Jahis Jsonデータをセット
            args.JahisData = new QsJsonSerializer().Serialize(jahis);

            // データ登録
            if (!TryWriteEntity(args, recordDate, pharmacyNo, medicineSet, results, out var seq))
            {
                return results;
            }

            // 登録した情報を取得
            if (!TryReadEntity(args.AccountKey, recordDate, seq, results, out var entity))
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

            if (args.DataType == DataTypeEnum.EthicalDrug)
            {
                // 薬効情報を適用
                if (!TryApplyYakkoData(new List<QoNoteMedicineItem> { noteMedicineItem }, results))
                {
                    // 部分的成功
                    // 登録自体は正常に完了しているので一応正常として返す
                    results.IsSuccess = bool.TrueString;
                    results.Result.Code = $"{(int)QoApiResultCodeTypeEnum.NoteMedicineAddPartialSuccess:D4}";
                    results.Result.Detail = $"正常に登録できましたが、戻り値用のデータは取得できませんでした。Error: {results.Result.Detail}";
                }
            }            

            results.Data = noteMedicineItem;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsSuccess = bool.TrueString;

            return results;
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
        

        private bool TryWriteEntity(ConvertedArgs args, DateTime recordDate, int pharmacyNo, string medicineSet,  QoApiResultsBase results, out int seq)
        {
            seq = 0;
            try
            {
                string writeError = string.Empty;
                string dataId = string.Empty;

                var (isSuccess, isInnerSuccess, newSeq) = _noteRepo.WriteMedicine(
                    args.AccountKey,
                    args.AuthorKey,
                    args.JahisData,
                    (byte)args.DataType,
                    recordDate,
                    args.LinkageSystemNo,
                    medicineSet,
                    (int)args.OwnerType,
                    pharmacyNo,
                    ref dataId,
                    ref writeError
                    );

                if (!isSuccess || !isInnerSuccess)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "お薬手帳データの登録処理に失敗しました。");
                    return false;
                }

                seq = newSeq;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お薬手帳情報の登録処理でエラーが発生しました。");
                return false;
            }
        }

        private bool TryReadAccountEntity(Guid accountKey, QoApiResultsBase results, out QH_ACCOUNTINDEX_DAT entity)
        {
            entity = null;
            try
            {
                entity = _accountRepo.ReadAccountIndexDat(accountKey);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "ユーザーが存在しません。");
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ユーザー情報の取得に失敗しました。");
                return false;
            }
        }

        private JM_Message EthDataToJahis(QoNoteMedicineDetail data, QH_ACCOUNTINDEX_DAT accountDat)
        {
            var builder = new JahisMessageBuilder(JMVersionTypeEnum.Latest);

            // 患者情報レコード
            builder.SetPatientData(
                $"{accountDat.FAMILYNAME}　{accountDat.GIVENNAME}",
                $"{accountDat.FAMILYKANANAME}　{accountDat.GIVENKANANAME}",
                accountDat.SEXTYPE,
                accountDat.BIRTHDAY
            );           

            // 調剤情報
            builder.AddPrescription(preCfg =>
            {
                preCfg
                // 調剤年月日
                .SetDate(data.RecordDate.TryToValueType(DateTime.MinValue))
                // 薬局、薬剤師
                .SetPharmacy(data.PharmacyName, data.PharmacistName, data.PharmacyNo)
                // 病院
                .SetHospital(data.HospitalName)
                // 患者等記入レコード
                .SetPatientMemo(SpliteBreakLine(data.Memo));

                // 医師情報
                preCfg.AddRpSet(rpSetCfg =>
                {
                    rpSetCfg
                    // 医師・診療科
                    .SetDoctor(data.DoctorName)
                    .SetDepartment(data.DepartmentName);

                    foreach (var rp in data.RpItems) 
                    {
                        // RP情報
                        rpSetCfg.AddRp(rpCfg =>
                        {
                            rpCfg
                            // 用法名称・用法補足
                            .SetUsage(rp.UsageName, SpliteBreakLine(rp.UsageSupplement))
                            // 剤形コード
                            .SetDosageFormCode(rp.DosageFormCode)
                            // 調剤数量・単位
                            .SetQuantity(rp.Quantity, rp.QuantityUnit);

                            foreach(var med in rp.MedicineItems)
                            {
                                // 薬品レコード
                                rpCfg.AddMedicine(
                                    med.Name,
                                    med.Quantity,
                                    med.QuantityUnit,
                                    med.YjCode
                                );
                            }
                                
                        });
                    }                  
                });
            });

            return builder.Build();
        }

        private JM_Message OtcDataToJahis(QoNoteMedicineDetail data, QH_ACCOUNTINDEX_DAT accountDat)
        {
            var builder = new JahisMessageBuilder(JMVersionTypeEnum.Latest);
            
            builder
            // 患者情報レコード
            .SetPatientData(
                $"{accountDat.FAMILYNAME}　{accountDat.GIVENNAME}",
                $"{accountDat.FAMILYKANANAME}　{accountDat.GIVENKANANAME}",
                accountDat.SEXTYPE,
                accountDat.BIRTHDAY
            )
            // 手帳メモ情報
            .SetNoteMemo(SpliteBreakLine(data.Memo));

            // 市販薬情報
            foreach(var med in data.MedicineItems)
            {
                builder.AddOtcMedicine(med.Name);
            };           


            // 調剤情報
            builder.AddPrescription(preCfg =>
            {
                preCfg
                // 調剤年月日
                .SetDate(data.RecordDate.TryToValueType(DateTime.MinValue))
                // 薬局、薬剤師
                .SetPharmacy(data.PharmacyName, data.PharmacistName, data.PharmacyNo);                
            });

            return builder.Build();
        }
    }
}