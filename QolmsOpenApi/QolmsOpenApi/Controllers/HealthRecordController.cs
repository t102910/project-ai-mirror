using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// バイタル手帳に関する機能を提供するAPIコントローラです。
    /// </summary>
    public class HealthRecordController: QoApiControllerBase
    {
        /// <summary>
        /// バイタル手帳に情報を書き込むジョブを登録します。
        /// 実際にDBに書き込むのはWebJobが行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("ImportBatch")]
        public QoHealthRecordImportApiResults PostImportBatch(QoHealthRecordImportApiArgs args)
        {
            var worker = new HealthRecordBatchWorker(new HealthRecordQueueRepository(), new HealthRecordValidator());
            return ExecuteWorkerMethod(args, worker.ImportBatch);
        }

        /// <summary>
        /// バイタル手帳に情報を書き込みます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey | QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("Import")]
        public QoHealthRecordImportApiResults PostImport(QoHealthRecordImportApiArgs args)
        {
            var worker = new HealthRecordWorker(new HealthRecordRepository(), new HealthRecordValidator());
            return ExecuteWorkerMethod(args, worker.Import);
        }

        /// <summary>
        /// バイタル手帳から情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey | QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("Read")]
        public QoHealthRecordReadApiResults PostRead(QoHealthRecordReadApiArgs args)
        {
            var worker = new HealthRecordReadWorker(new HealthRecordRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 今日の健康情報サマリを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("TodaySummary")]
        public QoHealthRecordTodaySummaryApiResults PostTodaySummary(QoHealthRecordTodaySummaryApiArgs args)
        {
            var worker = new HealthRecordTodaySummaryWorker(new HealthRecordRepository());
            return ExecuteWorkerMethod(args, worker.ReadSummary);
        }

        /// <summary>
        /// 健康情報アラートを書き込みます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("AlertWrite")]
        public QoHealthAlertWriteApiResults PostAlertWrite(QoHealthAlertWriteApiArgs args)
        {
            var worker = new HealthAlertWriteWorker(new HealthRecordAlertRepository(), new HealthRecordValidator());
            return ExecuteWorkerMethod(args, worker.Write);
        }

        /// <summary>
        /// 健康情報 症状(違和感)を書き込みます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("SymptomWrite")]
        public QoHealthSymptomWriteApiResults PostSymptomWrite(QoHealthSymptomWriteApiArgs args)
        {
            var worker = new HealthSymptomWriteWorker(new SymptomRepository());

            return ExecuteWorkerMethod(args, worker.Write);
        }

        /// <summary>
        /// 健康情報 アラートと症状(違和感)を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("AlertSymptomRead")]
        public QoHealthAlertSymptomReadApiResults PostAlertSymptomRead(QoHealthAlertSymptomReadApiArgs args)
        {
            var worker = new HealthAlertSymptomReadWorker(new HealthRecordAlertRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 健康情報 アラートを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("AlertRead")]
        public QoHealthAlertReadApiResults PostAlertRead(QoHealthAlertReadApiArgs args)
        {
            var worker = new HealthAlertReadWorker(new HealthRecordAlertRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 健康情報 症状(違和感)を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HealthRecord)]
        [ActionName("SymptomRead")]
        public QoHealthSymptomReadApiResults PostSymptomRead(QoHealthSymptomReadApiArgs args)
        {
            var worker = new HealthSymptomReadWorker(new SymptomRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// SPHR用バイタル情報 を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey, QoApiFunctionTypeEnum.Tis)]
        [ActionName("SphrVitalRead")]
        public QoHealthRecordSphrVitalReadApiResults PostSphrVitalRead(QoHealthRecordSphrVitalReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, HealthRecordSphrVitalWorker.SphrVitalRead);
        }

    }
}