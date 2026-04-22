using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 医療用医薬品マスタに関する処理を行います。
    /// </summary>
    public class MasterMedicineWorker
    {
        IMedicineRepository _repo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="medicineRepository"></param>
        public MasterMedicineWorker(IMedicineRepository medicineRepository)
        {
            _repo = medicineRepository;
        }

        /// <summary>
        /// 医薬品を検索します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterMedicineSearchApiResults Search(QoMasterMedicineSearchApiArgs args)
        {
            var result = new QoMasterMedicineSearchApiResults
            {
                IsSuccess = bool.FalseString
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            var pageIndex = args.PageIndex.TryToValueType(0);
            var pageSize = args.PageSize.TryToValueType(100);

            DbEthicalDrugSearcherResults searchResults;
            try
            {
                searchResults = _repo.SearchMedicine(accountKey, args.SearchText, pageIndex, pageSize);
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            try
            {
                result.MedicineN = searchResults.EthicalDrugN.ConvertAll(x => new QoApiEthicalDrugHeaderItem
                {
                    YjCode = x.YjCode,
                    ProductName = x.ProductName,
                    CommonName = x.CommonName,
                    ApprovalCompanyName = x.ApprovalCompanyName,
                    SaleCompanyName = x.SaleCompanyName,
                    GeneralCode = x.GeneralCode,
                    IsGeneric = x.IsGeneric.ToString(),
                    InclutionDate = x.InclutionDate.ToApiDateString()
                });

                result.PageIndex = searchResults.PageIndex.ToString();
                result.MaxPageIndex = searchResults.MaxPageIndex.ToString();

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

        /// <summary>
        /// 医薬品の詳細を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterMedicineDetailReadApiResults DetailRead(QoMasterMedicineDetailReadApiArgs args)
        {
            var result = new QoMasterMedicineDetailReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            if (string.IsNullOrWhiteSpace(args.YjCode))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "YjCodeが不正です。");
                return result;
            }

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            var yjCode = args.YjCode.Trim();

            QH_ETHDRUG_DETAIL_VIEW detailResults;
            try
            {
                detailResults = _repo.ReadDetail(yjCode);
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            try
            {
                result.Medicine = detailResults.ToApiEthDrugDetailItem();

                result.Medicine.ThumbnailKey = result.Medicine.FileKeyN.FirstOrDefault() ?? new QoApiFileKeyItem();
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

        /// <summary>
        /// 医薬品の画像を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterMedicineImageReadApiResults ImageRead(QoMasterMedicineImageReadApiArgs args)
        {
            var result = new QoMasterMedicineImageReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            QH_ETHDRUGFILE_MST entity;
            try
            {
                // DBよりファイル情報を取得。
                entity = _repo.ReadEthDrugFileEntity(args.FileKey.FileKeyReference.ToDecrypedReference<Guid>());
            }
            catch(Exception ex)
            {
                // ファイルが存在しない、DBエラーの場合はエラーとする
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            try
            {
                // 実ファイルデータ取得
                (var base64data, var contentType, var originalName) = _repo.ReadEthDrugFileBlob(entity);

                result.IsSuccess = bool.TrueString;
                result.Image = new QoApiFileItem
                {
                    FileKeyReference = entity.FILEKEY.ToEncrypedReference(),
                    ContentType = contentType,
                    Sequence = "1",
                    OriginalName = originalName,
                    Data = base64data,
                };

                return result;
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.StorageError);
                return result;
            }
        }
    }
}