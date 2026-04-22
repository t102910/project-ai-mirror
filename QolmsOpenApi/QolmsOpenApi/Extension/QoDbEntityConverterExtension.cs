using System.Runtime.CompilerServices;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// 「コルムス OpenAPI サイト」で使用する DB 用エンティティクラスを、
    /// 異なる形式のエンティティクラスへ変換するための拡張機能を提供します。
    /// </summary>
    internal static class QoDbEntityConverterExtension
    {
        #region "Public Method"

        public static QoApiMedicineSetOtcPhotoItem ToApiMedicineSetOtcPhotoItem(this QhMedicineSetOtcDrugPhotoOfJson target,DateTime recordDate)
        {
            var result = new QoApiMedicineSetOtcPhotoItem();

            if (target != null)
            {
                result.RecordDate = recordDate.ToApiDateString();
                result.PharmacyName = target.PharmacyName;
                result.Memo = target.Memo;
                result.FileKeyN = target.AttachedFileN.ConvertAll(i =>
                {
                   return new QoApiFileKeyItem()
                   {
                       Sequence = "0",
                       FileKeyReference = i.FileKey.ToEncrypedReference()
                   };
                });
            }

            return result;
        }

        public static QoApiMedicineSetOtcItem ToApiMedicineSetOtcItem(this QhMedicineSetOtcDrugOfJson target, DateTime recordDate)
        {
            var result = new QoApiMedicineSetOtcItem();

            if (target != null)
            {
                result.RecordDate = recordDate.ToApiDateString();
                result.PharmacyName = target.PharmacyName;
                result.Memo = target.Comment;
                result.OtcDrugN = new List<QoApiMedicineOtcItem>();
                foreach (QhMedicineSetOtcDrugItemOfJson item in target.MedicineN)
                {
                    result.OtcDrugN.Add(new QoApiMedicineOtcItem()
                    {
                        FileKeyN = item.AttachedFileN.ConvertAll(i =>
                        {
                            return new QoApiFileKeyItem()
                            {
                                Sequence = "0",
                                FileKeyReference = i.FileKey.ToEncrypedReference()
                            };
                        }),
                        ItemCode = item.ItemCode,
                        ItemCodeType = item.ItemCodeType,
                        MedicineName = item.MedicineName

                    });
                }
            }

            return result;
        }

        /// <summary>
        /// QH_ETHDRUG_DETAIL_VIEWからQoApiEthicalDrugDetailItemに変換する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static QoApiEthicalDrugDetailItem ToApiEthDrugDetailItem(this QH_ETHDRUG_DETAIL_VIEW entity)
        {
            return new QoApiEthicalDrugDetailItem
            {
                YjCode = entity.YJCODE,
                ProductName = entity.PRODUCTNAME,
                CommonName = entity.COMMONNAME,
                ApprovalCompanyName = entity.APPROVALCOMPANYNAME,
                SaleCompanyName = entity.SALESCOMPANYNAME,
                Ingredients = entity.INGREDIENTS,
                GeneralCode = entity.GENERALCODE,
                ActionA = entity.ACTIONA,
                ActionB = entity.ACTIONB,
                ActionC1 = entity.ACTIONC1,
                ActionC2 = entity.ACTIONC2,
                Interaction = entity.INTERACTION,
                Precautions = entity.PRECAUTIONS,
                DrugOrFood = entity.DRUGORFOOD,
                FileKeyN = entity.FileEntityN?.Select(x => new QoApiFileKeyItem
                {
                    FileKeyReference = x.FILEKEY.ToEncrypedReference(),
                    Sequence = x.SEQUENCE.ToString()
                })?.ToList() ?? new List<QoApiFileKeyItem>(),
            };
        }

        /// <summary>
        /// QH_PATIENTCARD_FACILITY_VIEWからQoApiPatientCardItemに変換する
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="customCardCodeFunc">カスタムコード生成関数</param>
        /// <returns></returns>
        public static QoApiPatientCardItem ToApiPatientCardItem(this QH_PATIENTCARD_FACILITY_VIEW entity,Func<QH_PATIENTCARD_FACILITY_VIEW,string> customCardCodeFunc = null)
        {
            return new QoApiPatientCardItem
            {
                FacilityKeyReference = entity.FACILITYKEY.ToEncrypedReference(),
                CreatedDate = entity.CREATEDDATE.ToApiDateString(),
                LinkUserId = entity.CARDNO,
                Sequence = entity.SEQUENCE.ToString(),
                StatusType = entity.STATUSTYPE.ToString(),
                CustomCardCode = customCardCodeFunc?.Invoke(entity) ?? string.Empty,
                // 添付ファイルが必要になれば取得する処理が必要だが
                // 現時点で不要なのでパフォーマンス重視で省略
                AttachedFileN = new List<QhApiAttachedFileItem>(),
            };
        }

        /// <summary>
        /// QH_NOTICE_DATをQoApiNoticeItemに変換する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static QoApiNoticeItem ToApiNoticeItem(this QH_NOTICE_DAT entity)
        {
            var dataset = new QsJsonSerializer().Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
            var files = new List<QoApiFileKeyItem>();
            if (dataset != null && dataset.AttachedFileN != null)
            {
                int sq = 0;
                foreach (var item in dataset.AttachedFileN)
                {
                    files.Add(new QoApiFileKeyItem() { Sequence = sq.ToString(), FileKeyReference = item.FileKey.ToEncrypedReference() });
                    sq++;
                }
            }
            var linkes = new List<QoApiLinkItem>();
            if (dataset != null && dataset.LinkN != null)
            {
                foreach (var item in dataset.LinkN)
                    linkes.Add(new QoApiLinkItem() { Title = item.LinkText, Url = item.LinkUrl });
            }
            return new QoApiNoticeItem()
            {
                CategoryNo = entity.CATEGORYNO.ToString(),
                Contents = entity.CONTENTS,
                EndDate = entity.ENDDATE.ToApiDateString(),
                FacilityKeyReference = entity.FACILITYKEY.ToEncrypedReference(),
                FileKeyN = files,
                LinkN = linkes,
                NoticeNo = entity.NOTICENO.ToString(),
                PriorityNo = entity.PRIORITYNO.ToString(),
                StartDate = entity.STARTDATE.ToApiDateString(),
                Title = entity.TITLE
            };

        }

        /// <summary>
        /// DbNoticeGroupItem から QoApiJotoAppNoticeGroupItem に変換する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static QoApiJotoAppNoticeGroupItem ToApiJotoAppNoticeGroupItem(this DbNoticeGroupItem target)
        {
            if (target == null)
                return null;

            return new QoApiJotoAppNoticeGroupItem()
            {
                NoticeNo = target.NoticeNo.ToString(),
                Title = target.Title,
                Contents = target.Contents,
                CategoryNo = target.CategoryNo.ToString(),
                PriorityNo = target.PriorityNo.ToString(),
                TargetType = target.TargetType.ToString(),
                StartDate = target.StartDate.ToApiDateString(),
                EndDate = target.EndDate.ToApiDateString(),
                AlreadyReadFlag = target.AlreadyReadFlag.ToString(),
                AlreadyReadDate = target.AlreadyReadDate.ToApiDateString()
            };
        }

        /// <summary>
        /// DbNoticeGroupItem から QoApiJotoAppNoticeGroupItem に変換する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static QoApiJotoAppNoticeGroupDetailItem ToApiJotoAppNoticeGroupDetailItem(this DbNoticeGroupItem target)
        {
            if (target == null)
                return null;

            var dataset = new QsJsonSerializer().Deserialize<QhNoticeDataSetOfJson>(target.NoticeDataSet);
            var files = new List<QoApiFileKeyItem>();
            if (dataset != null && dataset.AttachedFileN != null)
            {
                int sequence = 0;
                foreach (var item in dataset.AttachedFileN)
                {
                    files.Add(new QoApiFileKeyItem()
                    {
                        Sequence = sequence.ToString(),
                        FileKeyReference = item.FileKey.ToEncrypedReference()
                    });
                    sequence++;
                }
            }

            var links = new List<QoApiLinkItem>();
            if (dataset != null && dataset.LinkN != null)
            {
                foreach (var item in dataset.LinkN)
                {
                    links.Add(new QoApiLinkItem()
                    {
                        Title = item.LinkText,
                        Url = item.LinkUrl
                    });
                }
            }

            return new QoApiJotoAppNoticeGroupDetailItem()
            {
                NoticeNo = target.NoticeNo.ToString(),
                Title = target.Title,
                Contents = target.Contents,
                CategoryNo = target.CategoryNo.ToString(),
                PriorityNo = target.PriorityNo.ToString(),
                TargetType = target.TargetType.ToString(),
                StartDate = target.StartDate.ToApiDateString(),
                EndDate = target.EndDate.ToApiDateString(),
                LinkN = links,
                FileKeyN = files,
                AlreadyReadFlag = target.AlreadyReadFlag.ToString(),
                AlreadyReadDate = target.AlreadyReadDate.ToApiDateString()
            };
        }

        /// <summary>
        /// DbAppEventItem から QoApiJotoAppAppEventItem に変換します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static QoApiJotoAppAppEventItem ToApiJotoAppAppEventItem(this DbAppEventItem target)
        {
            if (target == null)
            {
                return null;
            }

            var set = new QsJsonSerializer().Deserialize<QhAppEventSetOfJson>(target.AppEventSet);
            var thumbnails = new List<QoApiFileKeyItem>();
            if (set != null && set.ThumbnailN != null)
            {
                int sequence = 0;
                thumbnails = set.ThumbnailN.ConvertAll(x => new QoApiFileKeyItem()
                {
                    Sequence = (sequence++).ToString(),
                    FileKeyReference = x.FileKey.ToEncrypedReference()
                });
            }

            return new QoApiJotoAppAppEventItem()
            {
                EventKey = target.EventKey.ToString(),
                AppType = target.AppType.ToString(),
                EventCode = target.EventCode,
                LinkageSystemNo = target.LinkageSystemNo.ToString(),
                Title = target.Title,
                Contents = target.Contents,
                StartDate = target.StartDate.ToApiDateString(),
                EndDate = target.EndDate.ToApiDateString(),
                PublishStartDate = target.PublishStartDate.ToApiDateString(),
                PublishEndDate = target.PublishEndDate.ToApiDateString(),
                ThumbnailN = thumbnails
            };
        }

        /// <summary>
        /// DbAppEventItem から QoApiJotoAppAppEventDetailItem に変換します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static QoApiJotoAppAppEventDetailItem ToApiJotoAppAppEventDetailItem(this DbAppEventItem target)
        {
            if (target == null)
            {
                return null;
            }

            var set = new QsJsonSerializer().Deserialize<QhAppEventSetOfJson>(target.AppEventSet);
            var banners = new List<QoApiFileKeyItem>();
            if (set != null && set.BannerFileN != null)
            {
                int sequence = 0;
                banners = set.BannerFileN.ConvertAll(x => new QoApiFileKeyItem()
                {
                    Sequence = (sequence++).ToString(),
                    FileKeyReference = x.FileKey.ToEncrypedReference()
                });
            }

            var links = new List<QoApiLinkItem>();
            if (set != null && set.LinkN != null)
            {
                links = set.LinkN.ConvertAll(x => new QoApiLinkItem() { Title = x.LinkText, Url = x.LinkUrl });
            }

            var entryLinks = new List<QoApiLinkItem>();
            if (set != null && set.EntryLinkN != null)
            {
                entryLinks = set.EntryLinkN.ConvertAll(x => new QoApiLinkItem() { Title = x.LinkText, Url = x.LinkUrl });
            }

            return new QoApiJotoAppAppEventDetailItem()
            {
                EventKey = target.EventKey.ToString(),
                AppType = target.AppType.ToString(),
                EventCode = target.EventCode,
                LinkageSystemNo = target.LinkageSystemNo.ToString(),
                Title = target.Title,
                Contents = target.Contents,
                StartDate = target.StartDate.ToApiDateString(),
                EndDate = target.EndDate.ToApiDateString(),
                PublishStartDate = target.PublishStartDate.ToApiDateString(),
                PublishEndDate = target.PublishEndDate.ToApiDateString(),
                BannerFileN = banners,
                LinkN = links,
                EntryLinkN = entryLinks
            };
        }

        #endregion
    }
}