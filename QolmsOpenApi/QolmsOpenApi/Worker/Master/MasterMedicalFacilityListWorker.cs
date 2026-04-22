using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 医療機関リスト取得処理
    /// </summary>
    public class MasterMedicalFacilityListWorker
    {
        IFacilityRepository _facilityRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="facilityRepository"></param>
        public MasterMedicalFacilityListWorker(IFacilityRepository facilityRepository)
        {
            _facilityRepo = facilityRepository;
        }

        /// <summary>
        /// 医療機関の一覧を取得する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterMedicalFacilityListReadApiResults Read(QoMasterMedicalFacilityListReadApiArgs args)
        {
            var results = new QoMasterMedicalFacilityListReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);

            var facilityKeyList = args.FacilityKeyReferenceList.ConvertAll(x => x.ToDecrypedReference().TryToValueType(Guid.Empty));

            var updatedDate = args.UpdatedDate.TryToValueType(DateTime.MinValue);

            // サーバーデータ取得時間を保持
            var readDate = DateTime.Now;

            // 連携システム番号も施設キーリストも指定されていない場合は空リストを返す
            // エラーにはしない
            if (linkageSystemNo == int.MinValue && !facilityKeyList.Any())
            {
                results.FacilityN = new List<QoApiMedicalFacilityListItem>();
                results.ReadDate = readDate.ToApiDateString();
                results.IsSuccess = bool.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return results;
            }

            // 医療機関リストをDBから取得
            if (!TryReadFacilityData(linkageSystemNo, facilityKeyList,updatedDate, results, out var entityList))
            {
                return results;
            }

            // データ変換
            if(!TryConvertApiData(entityList, results, out var apiItems))
            {
                return results;
            }

            results.FacilityN = apiItems;
            results.ReadDate = readDate.ToApiDateString();
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;            
        }

        bool TryReadFacilityData(int linkageSystemNo,List<Guid> facilityKeyList, DateTime updatedDate, QoApiResultsBase results, out List<QH_FACILITY_ALL_VIEW> entityList)
        {
            try
            {
                if(linkageSystemNo > int.MinValue)
                {
                    entityList = _facilityRepo.ReadMedicalFacilityListByLinkageSystemNo(linkageSystemNo, updatedDate);
                }
                else
                {                    
                    entityList = _facilityRepo.ReadMedicalFacilityListByKey(updatedDate, facilityKeyList.ToArray());
                }

                return true;
            }
            catch(Exception ex)
            {
                entityList = null;
                results.Result = QoApiResult.Build(ex, "DBからの医療機関情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryConvertApiData(List<QH_FACILITY_ALL_VIEW> entityList, QoApiResultsBase results, out List<QoApiMedicalFacilityListItem> apiItems)
        {
            try
            {
                apiItems = entityList.ConvertAll(x => ConvertApiItem(x));
                return true;
            }
            catch(Exception ex)
            {
                apiItems = null;
                results.Result = QoApiResult.Build(ex, "医療機関情報の変換に失敗しました。");
                return false;
            }
        }

        // QH_FACILITY_ALL_VIEW -> QoApiMEdicalFacilityListItem変換
        QoApiMedicalFacilityListItem ConvertApiItem(QH_FACILITY_ALL_VIEW entity)
        {
            // SQLクエリ上はパフォーマンス重視で並べ替えをしていないので
            // ここで並べ替えを行う
            var tmpFileN = entity.FileList?.OrderBy(x => x.Sequence)
                ?.Select(x => new QoApiFileKeyItem
                {
                    Sequence = x.Sequence.ToString(),
                    FileKeyReference = x.FileKey.ToEncrypedReference()
                })?.ToList() ?? new List<QoApiFileKeyItem>();

            return new QoApiMedicalFacilityListItem
            {
                FacilityKeyReference = entity.FACILITYKEY.ToEncrypedReference(),
                MedicalFacilityCode = entity.MEDICALFACILITYCODE,
                Name = entity.FACILITYNAME,
                NameKana = entity.FACILITYKANANAME,
                Address1 = entity.ADDRESS1,
                Address2 = entity.ADDRESS2,
                PostalCode = entity.POSTALCODE,
                Location = new QoApiLocationItem
                {
                    Latitude = entity.LATITUDE.ToString(),
                    Longitude = entity.LONGITUDE.ToString()
                },
                MedicalDepartmentN = entity.DepertmentList?.OrderBy(x => x.DispOrder)?.Select(x => new QoApiMedicalDepartmentItem
                {
                    DepartmentNo = x.DepartmentNo.ToString(),
                    DepartmentName = x.DepartmentName,
                    LocalCode = x.LocalCode,
                    LocalName = x.LocalName
                })?.ToList() ?? new List<QoApiMedicalDepartmentItem>(),
                FileKeyN = tmpFileN,
                PhoneN = entity.ContactList?.OrderBy(x => x.DispOrder)?.Select(x => ConvertPhoneListItem(x))?.ToList() ?? new List<QoApiFacilityPhoneListItem>(),
                UrlN = entity.UrlList?.OrderBy(x => x.DispOrder)?.Select(x => new QoApiFacilityUriListItem
                {
                    UriTypeNo = x.UrlType.ToString(),
                    Uri = x.Url
                })?.ToList() ?? new List<QoApiFacilityUriListItem>(),
                ThumbnailKey = tmpFileN.FirstOrDefault() ?? new QoApiFileKeyItem()
            };
        }

        // ContactInfo -> QoApiFacilityPhoneListItem変換
        QoApiFacilityPhoneListItem ConvertPhoneListItem(QH_FACILITY_ALL_VIEW.ContactInfo contactInfo)
        {
            return new QoApiFacilityPhoneListItem
            {
                PhoneNo = contactInfo.TelFull,
                ContactInformationTypeNo = contactInfo.ContactInformationType.ToString(),
                Comment = contactInfo.Comment.Comment,
                Title = contactInfo.Comment.Title,
                ReceptionTimeN = contactInfo.Comment.ReceptionTimeN.ConvertAll(x => new QoApiFacilityPhoneReceptionTimeItem
                {
                    Tag = x.Tag,
                    TimeText = x.TimeText
                })
            };
        }
    }
}