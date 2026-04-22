using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// マスターに関する機能を提供する API コントローラ です。
    /// </summary>
    public class MasterController : QoApiControllerBase
    {
        /// <summary>
        /// 指定条件の医療機関のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("MedicalFacilityListRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master)]
        public QoMasterMedicalFacilityListReadApiResults PostMedicalFacilityListRead(QoMasterMedicalFacilityListReadApiArgs args)
        {
            var worker = new MasterMedicalFacilityListWorker(new FacilityRepository());
            return base.ExecuteWorkerMethod(args,worker.Read);
        }


        /// <summary>
        /// 薬局を検索します。 
        /// TODO: 薬局検索なのにFacilitySearchとなっているのはややこしいので再考の余地あり。
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("FacilitySearch")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterFacilitySearchApiResults PostFacilitySearch(QoMasterFacilitySearchApiArgs args)
        {
            var worker = new MasterPharmacyWorker(new FacilityRepository());
            return ExecuteWorkerMethod(args, worker.Search);
        }

        /// <summary>
        /// 薬局の詳細情報を取得します。
        /// TODO: これもメソッド名がややこしいので再考の余地あり。
        /// </summary>
        /// 
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("FacilityDetailRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterFacilityDetailReadApiResults PostFacilityDetailRead(QoMasterFacilityDetailReadApiArgs args)
        {
            var worker = new MasterPharmacyWorker(new FacilityRepository());
            return ExecuteWorkerMethod(args, worker.DetailRead);
        }

        /// <summary>
        /// 医療用医薬品マスタを検索します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("MedicineSearch")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterMedicineSearchApiResults PostMedicineSearch(QoMasterMedicineSearchApiArgs args)
        {
            var worker = new MasterMedicineWorker(new MedicineRepository());
            return ExecuteWorkerMethod(args, worker.Search);
        }

        /// <summary>
        /// 医療用医薬品マスタ詳細情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("MedicineDetailRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterMedicineDetailReadApiResults PostMedicineDetailRead(QoMasterMedicineDetailReadApiArgs args)
        {
            var worker = new MasterMedicineWorker(new MedicineRepository());
            return ExecuteWorkerMethod(args, worker.DetailRead);
        }

        /// <summary>
        /// 医療用医薬品マスタ製剤写真を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("MedicineImageRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterMedicineImageReadApiResults PostMedicineImageRead(QoMasterMedicineImageReadApiArgs args)
        {
            var worker = new MasterMedicineWorker(new MedicineRepository());
            return ExecuteWorkerMethod(args, worker.ImageRead);
        }

        /// <summary>
        /// OTC医薬品マスタを検索します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("DrugSearch")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterDrugSearchApiResults PostDrugSearch(QoMasterDrugSearchApiArgs args)
        {
            var worker = new MasterDrugSearchWorker(new OtcDrugRepository());
            return ExecuteWorkerMethod(args, worker.Search);
        }

        /// <summary>
        /// OTC医薬品マスタ詳細情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("DrugDetailRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master | QoApiFunctionTypeEnum.Guest)]
        public QoMasterDrugDetailReadApiResults PostDrugDetailRead(QoMasterDrugDetailReadApiArgs args)
        {
            var worker = new MasterDrugDetailReadWorker(new OtcDrugRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 医療機関のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("MedicalFacilitySearchRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master)]
        public QoMasterMedicalFacilitySearchApiResults PostMedicalFacilitySearchRead(QoMasterMedicalFacilitySearchApiArgs args)
        {
            return base.ExecuteWorkerMethod(args, MasterWorker.MedicalFacilitySearchRead);
        }

        /// <summary>
        /// お知らせ一覧を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("NoticeRead")]
        public QoMasterNoticeReadApiResults PostNoticeRead(QoMasterNoticeReadApiArgs args)
        {
            var worker = new MasterNoticeWorker(new NoticeRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 施設毎の言語リソースを取得します。
        /// </summary>
        [ActionName("FacilityLanguageRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Master)]
        public QoMasterFacilityLanguageReadApiResults PostFacilityLanguageRead(QoMasterFacilityLanguageReadApiArgs args)
        {
            var worker = new MasterFacilityLanguageReadWorker(new FacilityRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }
    }
}
