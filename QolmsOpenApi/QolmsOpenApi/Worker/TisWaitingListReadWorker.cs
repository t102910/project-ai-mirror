using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbEntityV1;
using DataType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.DataTypeEnum;
using StatusType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.StatusTypeEnum;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// TIS用順番待ち情報取得処理
    /// </summary>
    public class TisWaitingListReadWorker
    {
        IWaitingRepository _waitingRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="waitingListRepository"></param>
        public TisWaitingListReadWorker(IWaitingRepository waitingListRepository)
        {
            _waitingRepo = waitingListRepository;
        }

        /// <summary>
        /// 最新の順番待ち情報を取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoTisWaitingListReadApiResults ReadLatest(QoTisWaitingListReadApiArgs args)
        {
            var result = new QoTisWaitingListReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            try
            {
                var accountKey = args.ActorKey.TryToValueType(Guid.Empty);

                //対象の設定があるか
                if (accountKey == Guid.Empty)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
                    return result;
                }

                var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

                if (facilityKey == Guid.Empty)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "施設が不正です。");
                    return result;
                }

                var targetDate = args.TargetDate.TryToValueType(DateTime.Now);
                var dataType = args.DataType.TryToValueType((byte)DataType.None);

                List<QH_WAITINGLIST_DAT> waitingEntityN;
                try
                {
                    // DBより順番待ち情報を取得
                    waitingEntityN = _waitingRepo.ReadLatestWaitingList(accountKey, facilityKey, targetDate, dataType);
                }
                catch (Exception ex)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, $"順番待ち情報取得処理エラー: {ex.Message}");
                    return result;
                }

                // 正常なレコードのみ抽出
                waitingEntityN = waitingEntityN.Where(x => x.IsKeysValid()).ToList();

                var hasMedicineWithPayment = false;
                // 薬待ちシステムを使わない場合で例外的に薬が用意されている場合があり
                // その場合は薬待ちでStatusが7のレコードが存在する。
                // 存在すれば薬ありと判定する
                var hasMedicineEntity = waitingEntityN.FirstOrDefault(x => x.DATATYPE == (byte)DataType.Dispensing && x.STATUSTYPE == (byte)StatusType.HasMedicine);
                if(hasMedicineEntity != null)
                {
                    waitingEntityN.Remove(hasMedicineEntity);
                    hasMedicineWithPayment = true;
                }

                if (waitingEntityN == null || !waitingEntityN.Any())
                {
                    // レコードがない場合は待ち状態無しとする
                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    return result;
                }
                
                // 最新の待ち状態を取得
                var latestEntity = GetLatestEntity(waitingEntityN);
                // 状態が待ち状態であるかの判定
                var isWaiting = IsWaiting(latestEntity);

                if (!isWaiting)
                {
                    // 待ち状態無し
                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    return result;
                }

                int count = 0;
                try
                {
                    count = _waitingRepo.GetWaitingCount(accountKey, facilityKey, targetDate, latestEntity.DATATYPE);
                }
                catch (Exception ex)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, $"待ち人数取得処理エラー:{ex.Message}");
                    return result;
                }


                // 結果を戻り値に変換
                var detail = new QsJsonSerializer().Deserialize<QhWaitingListValueOfJson>(latestEntity.VALUE);
                result.Sequence = latestEntity.SEQUENCE.ToString();

                result.DepartmentCode = latestEntity.DEPARTMENTCODE;
                result.DepartmentName = detail.DepartmentName;
                result.DataType = latestEntity.DATATYPE.ToString();
                result.ReceptionNo = latestEntity.RECEPTIONNO;
                result.ReceptionTime = latestEntity.RECEPTIONDATE.ToApiDateString();
                result.ReservationNo = latestEntity.RESERVATIONNO;
                result.ReservationTime = latestEntity.RESERVATIONDATE.ToApiDateString();
                result.StatusType = latestEntity.STATUSTYPE.ToString();
                result.WaitingDate = latestEntity.WAITINGDATE.ToApiDateString();
                result.WaitingCount = count.ToString();
                result.HasMedicineWithPayment = hasMedicineWithPayment.ToString();
                result.ForeignKey = latestEntity.FOREIGNKEY;

                result.HasMedicineWithPayment = bool.FalseString;
                // 最新が会計呼び出し状態の時は、特殊薬待ちの状態を反映する
                if (latestEntity.DATATYPE == (byte)DataType.Payment && latestEntity.STATUSTYPE == (byte)StatusType.Called)
                {
                    result.HasMedicineWithPayment = hasMedicineWithPayment.ToString();
                }

                result.Detail = new QoApiWaitingDetailItem
                {
                    DepartmentName = detail.DepartmentName,
                    DoctorCode = detail.DoctorCode,
                    DoctorName = detail.DoctorName,
                    DosingSlipNo = detail.DosingSlipNo,
                    DosingSlipType = detail.DosingSlipType,
                    InOutType = detail.InOutType,
                    MedicalActCode = detail.MedicalActCode,
                    MedicalActName = detail.MedicalActName,
                    RoomCode = detail.RoomCode,
                    RoomName = detail.RoomName,
                    SameDaySequence = detail.SameDaySequence,
                    DefferedPaymentFlg = detail.DefferedPaymentFlg,
                    ChartWaitNumber = detail.ChartWaitNumber
                };

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


        internal QH_WAITINGLIST_DAT GetLatestEntity(List<QH_WAITINGLIST_DAT> entityN)
        {
            entityN.Sort(CompareLatestEntity);

            return entityN.First();            
        }

        bool IsWaiting(QH_WAITINGLIST_DAT entity)
        {
            switch ((DataType)entity.DATATYPE)
            {
                case DataType.MedicalTreatment:
                    return entity.STATUSTYPE != 30;
                case DataType.Dispensing:
                case DataType.Payment:
                    return !(entity.STATUSTYPE == 8 || entity.STATUSTYPE == 9);
                default:
                    return false;
            }
        }

        // 優先度順に並べ替えるComparison
        // 1を返すとyの方が優先、-1を返すとxが優先、0を返すと同一と判定される
        int CompareLatestEntity (QH_WAITINGLIST_DAT x, QH_WAITINGLIST_DAT y)
        {
            var dataTypeX = (DataType)x.DATATYPE;
            var dataTypeY = (DataType)y.DATATYPE;           

            // 診察 / 診察
            if(dataTypeX == DataType.MedicalTreatment && dataTypeY == DataType.MedicalTreatment)
            {
                return CompareMedicalAndMedical(x, y);
            }
            // 診察 / 薬
            else if (dataTypeX == DataType.MedicalTreatment && dataTypeY == DataType.Dispensing)
            {
                return CompareMedicalAndPaymentDispensing(x, y);
            }
            // 診察 / 会計
            else if(dataTypeX == DataType.MedicalTreatment && dataTypeY == DataType.Payment)
            {
                return CompareMedicalAndPaymentDispensing(x, y);
            }            
            // 会計 / 診察
            else if(dataTypeX == DataType.Payment && dataTypeY == DataType.MedicalTreatment)
            {
                return CompareMedicalAndPaymentDispensing(y, x) * -1;
            }            
            // 会計 / 薬
            else if (dataTypeX == DataType.Payment && dataTypeY == DataType.Dispensing)
            {
                return ComparePaymentAndDispensing(x, y);
            }
            // 会計 / 会計
            else if (dataTypeX == DataType.Payment && dataTypeY == DataType.Payment)
            {
                return CompareSamePaymentDispensing(x, y);
            }
            // 薬 / 診察
            else if (dataTypeX == DataType.Dispensing && dataTypeY == DataType.MedicalTreatment)
            {
                return CompareMedicalAndPaymentDispensing(y, x) * -1;
            }
            // 薬 / 薬
            else if (dataTypeX == DataType.Dispensing && dataTypeY == DataType.Dispensing)
            {
                return CompareSamePaymentDispensing(x, y);
            }
            // 薬 / 会計
            else if (dataTypeX == DataType.Dispensing && dataTypeY == DataType.Payment)
            {
                return ComparePaymentAndDispensing(y, x) * -1;
            }

            return 0;
        }

        // 診察待ちと診察待ちを比較するComparison
        int CompareMedicalAndMedical(QH_WAITINGLIST_DAT x, QH_WAITINGLIST_DAT y)
        {
            // どちらかが終了している場合は終了していない方優先
            if (x.STATUSTYPE == 30 && y.STATUSTYPE != 30)
            {
                return 1;
            }
            if(x.STATUSTYPE != 30 && y.STATUSTYPE == 30)
            {
                return -1;
            }

            // 同じ状態でない場合
            if (x.STATUSTYPE != y.STATUSTYPE)
            {                
                // StatusTypeの大きい方が優先(10 受付,20 もうすぐ,25 呼び出しの優先順位)
                return x.STATUSTYPE < y.STATUSTYPE ? 1 : -1;                 
            }

            // どちらも受付済みの場合
            if(x.STATUSTYPE == 10)
            {
                // 予約時間が異なる場合
                if(x.RESERVATIONDATE != y.RESERVATIONDATE)
                {
                    // 予約時間が若い方が優先
                    // 飛び込みの場合は予約時間がMinValueのため飛び込みが優先となる
                    return x.RESERVATIONDATE < y.RESERVATIONDATE ? -1 : 1;
                } 
            }

            // 同じ状態で受付時間が同じ
            if (x.RECEPTIONDATE == y.RECEPTIONDATE)
            {
                // 更新日時の遅い方が優先
                if (x.UPDATEDDATE < y.UPDATEDDATE)
                {
                    return 1;
                }
                else
                {
                    // ここまで同じであれば差は無しとみなす(レアケース)
                    return x.UPDATEDDATE == y.UPDATEDDATE ? 0 : -1;
                }
            }

            // 同じ状態の時は受付時間の新しい方優先
            return x.RECEPTIONDATE < y.RECEPTIONDATE ? 1 : -1;    
        }

        
        // 診察待ちと薬・会計を比較するComparison
        int CompareMedicalAndPaymentDispensing(QH_WAITINGLIST_DAT m, QH_WAITINGLIST_DAT pd)
        {      
            // 診察が終了していなくて診察受付時間の方が薬・会計より新しい場合
            if (m.STATUSTYPE != 30 &&
                m.RECEPTIONDATE >= pd.RECEPTIONDATE)
            {
                // 同日再診の診察待ち
                return -1;
            }
            // どちらも終了済みの場合
            if (m.STATUSTYPE == 30 && (pd.STATUSTYPE == 8 || pd.STATUSTYPE == 9))
            {
                // 受付時間が同じなら
                if(m.RECEPTIONDATE == pd.RECEPTIONDATE)
                {
                    // 薬・会計優先
                    return 1;
                }

                // 受付時間の新しい方優先
                return m.RECEPTIONDATE < pd.RECEPTIONDATE ? 1 : -1;
            }
            else
            {
                // 新しい投薬待ち、会計待ちが出来ている時点で診察は終わっているはず。診察終了していなくても強制的に上書き
                return 1;
            }            
        }

        // 会計と薬を比較するComparison
        int ComparePaymentAndDispensing(QH_WAITINGLIST_DAT p, QH_WAITINGLIST_DAT d)
        {
            //var value = new QsJsonSerializer().Deserialize<QhWaitingListValueOfJson>(d.VALUE);
            //if ((value.DefferedPaymentFlg == "1"))
            //{
            //    // 後払い用の処方カードなら薬優先
            //    return 1;
            //}
            if ((p.STATUSTYPE == 8 || p.STATUSTYPE == 9) &&
                        !(d.STATUSTYPE == 8 || d.STATUSTYPE == 9))
            {
                // 会計が完了、薬が未完了なら薬優先
                return 1;
            }
            else if(!(p.STATUSTYPE == 8 || p.STATUSTYPE == 9) &&
                        (d.STATUSTYPE == 8 || d.STATUSTYPE == 9))
            {
                // 会計が未完了、薬が完了なら会計優先
                return -1;
            }
            else if ((p.STATUSTYPE == 8 || p.STATUSTYPE == 9) &&
                (d.STATUSTYPE == 8 || d.STATUSTYPE == 9))
            {
                // 両方完了なら投薬待ち優先（投薬待ち管理をしない場合、薬受け取って帰ってくださいという状態に留めたい）
                return 1;
            }
            else if (!(p.STATUSTYPE == 8 || p.STATUSTYPE == 9) &&
                !(d.STATUSTYPE == 8 || d.STATUSTYPE == 9))
            {
                // 両方未完了なら会計待ち優先                
                return -1;
            }
            else
            {
                return 0;
            }
        }

        // 薬と薬、または会計と会計を比較するComparison
        int CompareSamePaymentDispensing(QH_WAITINGLIST_DAT x, QH_WAITINGLIST_DAT y)
        {
            if ((x.STATUSTYPE == 8 || x.STATUSTYPE == 9) &&
                        !(y.STATUSTYPE == 8 || y.STATUSTYPE == 9))
            {
                // 比較元が完了、比較先が未完了なら比較先優先
                return 1;
            }
            else if (!(x.STATUSTYPE == 8 || x.STATUSTYPE == 9) &&
                        (y.STATUSTYPE == 8 || y.STATUSTYPE == 9))
            {
                // 比較元が未完了、比較先が完了なら比較元優先
                return -1;
            }
            else
            {
                // 受付時間が同じ
                if (x.RECEPTIONDATE == y.RECEPTIONDATE)
                {
                    // 更新日時の遅い方が優先
                    if (x.UPDATEDDATE < y.UPDATEDDATE)
                    {
                        return 1;
                    }
                    else
                    {
                        // ここまで同じであれば差は無しとみなす(レアケース)
                        return x.UPDATEDDATE == y.UPDATEDDATE ? 0 : -1;
                    }
                }

                // 受付時間が新しいもの優先
                return x.RECEPTIONDATE < y.RECEPTIONDATE ? 1 : -1;
            }
        }
    }
}