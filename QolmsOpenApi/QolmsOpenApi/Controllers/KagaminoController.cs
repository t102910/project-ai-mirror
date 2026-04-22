using MGF.QOLMS.QolmsApiCoreV1;
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
    /// Kagamino専用のAPIを取り扱うコントローラー
    /// </summary>
    public class KagaminoController: QoApiControllerBase
    {
        /// <summary>
        /// モデル情報のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("ModelListRead")]
        public QoModelListReadApiResults PostModelListRead(QoModelListReadApiArgs args)
        {
            var worker = new QkModelReadlWorker(new QkModelRepository());
            return ExecuteWorkerMethod(args ,worker.ReadList);
        }

        /// <summary>
        /// モデル情報を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("ModelRead")]
        public QoModelReadApiResults PostModelRead(QoModelReadApiArgs args)
        {
            var worker = new QkModelReadlWorker(new QkModelRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// モデルファイルを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("ModelFileRead")]
        public QoModelFileReadApiResults PostModelFileRead(QoModelFileReadApiArgs args)
        {
            var worker = new QkModelReadlWorker(new QkModelRepository());
            return ExecuteWorkerMethod(args, worker.ReadFile);
        }

        /// <summary>
        /// ランダムなアドバイスを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("RandomAdviceRead")]
        public QoRandomAdviceReadApiResults PostRandomAdviceRead(QoRandomAdviceReadApiArgs args)
        {
            var worker = new QkRandomAdviceReadWorker(new QkRandomAdviceRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// ランダムアドバイスリストを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("RandomAdviceListRead")]
        public QoRandomAdviceListReadApiResults PostRandomAdviceListRead(QoRandomAdviceListReadApiArgs args)
        {
            var worker = new QkRandomAdviceReadWorker(new QkRandomAdviceRepository());
            return ExecuteWorkerMethod(args, worker.ReadList);
        }

        /// <summary>
        /// 背景情報のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("BackgroundListRead")]
        public QoBackgroundListReadApiResults PostBackgroundListRead(QoBackgroundListReadApiArgs args)
        {
            var worker = new QkBackgroundReadWorker(new QkBackgroundRepository());
            return ExecuteWorkerMethod(args, worker.ReadList);
        }

        /// <summary>
        /// 背景情報を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("BackgroundRead")]
        public QoBackgroundReadApiResults PostBackgroundRead(QoBackgroundReadApiArgs args)
        {
            var worker = new QkBackgroundReadWorker(new QkBackgroundRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 背景ファイルを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("BackgroundFileRead")]
        public QoBackgroundFileReadApiResults PostBackgroundFileRead(QoBackgroundFileReadApiArgs args)
        {
            var worker = new QkBackgroundReadWorker(new QkBackgroundRepository());
            return ExecuteWorkerMethod(args, worker.ReadFile);
        }

        /// <summary>
        /// Kagaminoログイン後に必要なデータを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Kagamino)]
        [ActionName("Init")]
        public QoKagaminoInitApiResults PostInit(QoKagaminoInitApiArgs args)
        {
            var worker = new KagaminoInitWorker(new QkModelRepository(), new QkBackgroundRepository());
            return ExecuteWorkerMethod(args, worker.GetInitData);
        }
    }
}