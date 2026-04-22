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
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 順番待ち情報デバッグ書き込み処理
    /// </summary>
    public class WaitingListDebugWriteWorker
    {
        WaitingListWriteWorker _writeWorker;
        ILinkageRepository _linkageRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitingRepository"></param>
        /// <param name="linkageRepository"></param>
        /// <param name="qoPushNotification"></param>
        /// <param name="facilityRepository">施設情報リポジトリ</param>
        public WaitingListDebugWriteWorker(IWaitingRepository waitingRepository, ILinkageRepository linkageRepository, IQoPushNotification qoPushNotification, IFacilityRepository facilityRepository)
        {
            _writeWorker = new WaitingListWriteWorker(waitingRepository, linkageRepository, qoPushNotification, facilityRepository);
            _linkageRepo = linkageRepository;
        }

        /// <summary>
        /// デバッグ用の情報を書き込みます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoWaitingListDebugWriteApiResults> ListDegugWrite(QoWaitingListDebugWriteApiArgs args)
        {
            var results = new QoWaitingListDebugWriteApiResults
            {
                IsSuccess = bool.FalseString
            };


            try
            {
                // 待ち受け日時が未指定なら現在時間を設定
                var waitingDate = string.IsNullOrWhiteSpace(args.WaitingDate) ? DateTime.Now.ToApiDateString() : args.WaitingDate;
                // 受付時間が未指定なら現在時間を設定
                var recepionTime = string.IsNullOrWhiteSpace(args.ReceptionTime) ? DateTime.Now.ToString("HHmm") : args.ReceptionTime;

                var facilityKey = args.FacilityKeyReference.ToDecrypedReference();
                QoAccessLog.WriteInfoLog($"WaitingListDebug facilitykey : {facilityKey}");

                // 連携システム番号が無ければ施設キーより逆引きする
                if (string.IsNullOrWhiteSpace(args.LinkageSystemNo))
                {
                    var linkageSystemNo =_linkageRepo.GetLinkageNo(facilityKey.TryToValueType(Guid.Empty));
                    QoAccessLog.WriteInfoLog($"WaitingListDebug linkageSystemNo : {linkageSystemNo}");
                    args.LinkageSystemNo = linkageSystemNo.ToString();
                }                


                var writeArgs = new QoWaitingListWriteApiArgs
                {
                    ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                    DataType = args.DataType,
                    FacilityKey = facilityKey,
                    LinkageSystemNo = args.LinkageSystemNo,
                    RootLinkageSystemNo = args.RootLinkageSystemNo
                };

                var waitingItem = new QoApiWaitingListItem
                {
                    DepartmentCode = args.DepartmentCode,
                    DepartmentName = args.DepartmentName,
                    DeleteFlag = args.IsDelete.TryToValueType(false).ToString(),
                    ForeignKey = args.ForeignKey,
                    LinkageSystemId = args.LinkageSystemId,
                    ReceptionNo = args.ReceptionNo,
                    ReceptionTime = recepionTime,
                    ReservationNo = args.ReservationNo,
                    ReservationTime = args.ReservationTime,
                    StatusType = args.StatusType,
                    WaitingDate = waitingDate,
                    Detail = new QoApiWaitingDetailItem
                    {
                        DepartmentName = args.DepartmentName,
                        DoctorCode = args.DoctorCode,
                        DoctorName = args.DoctorName,
                        DosingSlipNo = "",
                        DosingSlipType = "",
                        InOutType = "",
                        MedicalActCode = args.MedicalActCode,
                        MedicalActName = args.MedicalActName,
                        RoomCode = "",
                        RoomName = "",
                        SameDaySequence = "0",
                        DefferedPaymentFlg = args.DefferedPaymentFlg,
                        ChartWaitNumber = args.ChartWaitNumber.TryToValueType(sbyte.MinValue),
                    }
                };

                var reader = new DbWaitingListDebugReaderCore();
                if (string.IsNullOrWhiteSpace(args.ForeignKey))
                {
                    var maxKey = reader.GetMaxDebugForeignKey() + 1;
                    waitingItem.ForeignKey = $"99{maxKey:D10}";
                }

                if (string.IsNullOrWhiteSpace(args.ReceptionNo))
                {
                    var nextNo = reader.GetMaxReceptionNoInDay(waitingDate.TryToValueType(DateTime.MinValue)) + 1;
                    waitingItem.ReceptionNo = $"{nextNo:D4}";
                }

                writeArgs.WaitingListN = new List<QoApiWaitingListItem>
                {
                    waitingItem
                };

                var writeResult = await _writeWorker.ListWrite(writeArgs);

                if (writeResult.IsSuccess == bool.FalseString)
                {
                    results.Result = writeResult.Result;
                    return results;
                }

                results.ForeignKey = waitingItem.ForeignKey;
                results.ReceptionNo = waitingItem.ReceptionNo;
                results.IsSuccess = bool.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);             
                if(writeResult.ErrorMessageN?.Any() ?? false)
                {
                    results.Result.Detail += $"ただしPush通知処理内でエラーがありました。{string.Join(",", writeResult.ErrorMessageN)}";
                }

                return results;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex,"順番待ちのデバッグ書き込みに失敗しました。");
                return results;
            }
        }
    }
}