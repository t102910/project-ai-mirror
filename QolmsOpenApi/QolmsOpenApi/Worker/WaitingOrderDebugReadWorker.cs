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
    /// 
    /// </summary>
    public class WaitingOrderDebugReadWorker
    {
        IWaitingRepository _waitingRepo;
        ILinkageRepository _linkageRepo;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitingRepository"></param>
        /// <param name="linkageRepository"></param>
        public WaitingOrderDebugReadWorker(IWaitingRepository waitingRepository, ILinkageRepository linkageRepository)
        {
            _waitingRepo = waitingRepository;
            _linkageRepo = linkageRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoWaitingOrderDebugReadApiResults DebugRead(QoWaitingOrderDebugReadApiArgs args)
        {
            var results = new QoWaitingOrderDebugReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            try
            {
                var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);

                var targetDate = args.TargetDate.TryToValueType(DateTime.MinValue);

                var linkageSystemNo = _linkageRepo.GetLinkageNo(facilityKey);

                var config = _waitingRepo.GetMedicalDepartmentConfig(linkageSystemNo, args.DepartmentCode);

                var reservedEntityList = _waitingRepo.GetWaitingOrderListEntity(linkageSystemNo, args.DepartmentCode, args.DoctorCode, args.Limit, true, targetDate);

                var walkInEntityList = _waitingRepo.GetWaitingOrderListEntity(linkageSystemNo, args.DepartmentCode, args.DoctorCode, args.Limit, false, targetDate);


                results.ReservedList = reservedEntityList.ConvertAll(x => ConvertItem(x));
                results.WalkInList = walkInEntityList.ConvertAll(x => ConvertItem(x));
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                results.IsSuccess = bool.TrueString;

                return results;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "例外が発生しました。");
                return results;
            }
        }

        QoApiWaitingOrderItem ConvertItem(QH_WAITINGORDERLIST_DAT entity)
        {
            return new QoApiWaitingOrderItem
            {
                LinkageSystemId = entity.LINKAGESYSTEMID,
                ReceptionNo = entity.RECEPTIONNO,
                PushSendFlag = entity.PUSHSENDFLAG
            };
        }
    }
}