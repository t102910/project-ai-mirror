using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 薬局関連の処理を行います。
    /// </summary>
    public class MasterPharmacyWorker
    {
        IFacilityRepository _facilityRepo;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="facilityRepository"></param>
        public MasterPharmacyWorker(IFacilityRepository facilityRepository)
        {
            _facilityRepo = facilityRepository;
        }

        /// <summary>
        /// 薬局検索を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterFacilitySearchApiResults Search(QoMasterFacilitySearchApiArgs args)
        {
            QoMasterFacilitySearchApiResults result = new QoMasterFacilitySearchApiResults
            {               
                IsSuccess = bool.FalseString 
            };

            DateTime now = DateTime.Now;

            var searchType = (QoApiFacilitySearchTypeEnum)Convert.ToInt32(args.SearchType);
            var accountkey = args.ActorKey.TryToValueType(Guid.Empty);
            var prefNo = args.PrefectureNo.TryToValueType(byte.MinValue);
            var cityNo = args.CityNo.TryToValueType(int.MinValue);
            var latitude = decimal.Zero;
            var longitude = decimal.Zero;
            if (args.CurrentLocation != null)
            {
                latitude = args.CurrentLocation.Latitude.TryToValueType(decimal.Zero);
                longitude = args.CurrentLocation.Longitude.TryToValueType(decimal.Zero);
            }
            int pageIndex = args.PageIndex.TryToValueType(int.MinValue);
            int pagesize = args.PageSize.TryToValueType(int.MinValue);

            try
            {
                var facilityN = new List<DbFacilityItem>();
                if (searchType.HasFlag(QoApiFacilitySearchTypeEnum.Map))
                {
                    facilityN = _facilityRepo.SearchPharmacyByLocation(accountkey, latitude, longitude, now, searchType.HasFlag(QoApiFacilitySearchTypeEnum.Contracted), pageIndex, pagesize);
                }
                else
                {
                    var ret = _facilityRepo.SearchPharmacyByFiltering(accountkey, args.SearchText, latitude, longitude, prefNo, cityNo, now, searchType.HasFlag(QoApiFacilitySearchTypeEnum.Contracted), pageIndex, pagesize);

                    facilityN = ret.facilityN;
                    result.PageIndex = ret.pageIndex.ToString();
                    result.MaxPageIndex = ret.maxPageIndex.ToString();
                    result.TotalCount = ret.totalCount.ToString();
                }

                if(facilityN != null && facilityN.Any())
                {
                    result.FacilityN = ConvertApiFacilityItems(facilityN, now);                    
                }

                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
                result.Result = QoApiResult.Build(ex);
            }

            return result;
        }

        /// <summary>
        /// 薬局の詳細情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterFacilityDetailReadApiResults DetailRead(QoMasterFacilityDetailReadApiArgs args)
        {
            var result = new QoMasterFacilityDetailReadApiResults
            {
                IsSuccess = bool.FalseString
            };
            
            var now = DateTime.Now;

            var facilityKey = args.FacilityKeyReference.ToDecrypedReference<Guid>();
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            if(facilityKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "施設キーが不正です。");
                return result;
            }

            DbFacilityItem entity;
            try
            {
                entity = _facilityRepo.ReadFacility(facilityKey, accountKey);
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            try
            { 
                if (entity == null || entity.FacilityKey == Guid.Empty)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "データが存在しません。");
                    return result;
                }

                result.Facility = ConvertApiFacilityItem(entity, now);

                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

                return result;
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                return result;
            }  
        }

        List<QoApiFacilitySearchResultItem> ConvertApiFacilityItems(List<DbFacilityItem> facilityN, DateTime now)
        {
            var list = facilityN.Select(x => ConvertApiFacilityItem(x, now));
            return list.ToList();
        }

        internal QoApiFacilitySearchResultItem ConvertApiFacilityItem(DbFacilityItem item, DateTime now)
        {
            var apiFacility = new QoApiFacilitySearchResultItem
            {
                FacilityKeyReference = item.FacilityKey.ToEncrypedReference(),
                Name = item.FacilityName,  
                NameKana = item.FacilityKanaName,
                Address1 = item.Address1,
                Address2 = item.Address2,
                PostalCode = item.PostCode,
                Tel = item.Tel,
                Fax = "",
                FileKeyN = _facilityRepo.GetFacilityImageKey(item.FacilityKey),
                Location = new QoApiLocationItem()
                {
                    Latitude = item.Latitude.ToString(),
                    Longitude = item.Longitude.ToString()
                },
                TodayOpeningTime = new QoApiFacilityTimeItem()
                {
                    DayOfWeek = item.OfficeHour.DayOfWeek < 0 ? $"{(int)now.DayOfWeek}" : item.OfficeHour.DayOfWeek.ToString(),
                    StartTime = item.OfficeHour.TreatmentStartTime,
                    EndTime = item.OfficeHour.TreatmentEndTime
                },
                OpeningTimeN = item.OfficeHourN.ConvertAll(k => new QoApiFacilityTimeItem()
                {
                    DayOfWeek = k.DayOfWeek < 0 ? $"{(int)now.DayOfWeek}": k.DayOfWeek.ToString(),
                    StartTime = k.TreatmentStartTime,
                    EndTime = k.TreatmentEndTime
                }),
                IsOpening = item.OfficeHour.TreatmentStartTime.TryToValueType(0) <= now.ToString("HHmm").TryToValueType(0) && now.ToString("HHmm").TryToValueType(0) <= item.OfficeHour.TreatmentEndTime.TryToValueType(0) ? bool.TrueString : bool.FalseString,
                IsAccepting = bool.TrueString,
                FlagN = item.FlagN.ConvertAll(j => new QoApiMedicalFlagViewItem()
                {
                    FlagType = j.FlagNo.ToString(),
                    Comment = ""
                }),
                IsFavorite = item.FavoriteFlag.ToString()
            };

            apiFacility.ThumbnailKey = apiFacility.FileKeyN?.FirstOrDefault() ?? new QoApiFileKeyItem();

            return apiFacility;
        }
    }
}