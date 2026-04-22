using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Kagaminoアプリ初期化処理Worker
    /// </summary>
    public class KagaminoInitWorker
    {
        QkModelReadlWorker _modelWorker;
        QkBackgroundReadWorker _backgroundWorker;
        IQkModelRepository _modelRepo;
        IQkBackgroundRepository _backgroundRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="modelRepository"></param>
        /// <param name="backgroundRepository"></param>
        public KagaminoInitWorker(IQkModelRepository modelRepository, IQkBackgroundRepository backgroundRepository)
        {
            _modelRepo = modelRepository;
            _backgroundRepo = backgroundRepository;

            _modelWorker = new QkModelReadlWorker(_modelRepo);
            _backgroundWorker = new QkBackgroundReadWorker(_backgroundRepo);
        }

        /// <summary>
        /// Kagaminoアプリのログイン後の初期データを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoKagaminoInitApiResults GetInitData(QoKagaminoInitApiArgs args)
        {
            var result = new QoKagaminoInitApiResults
            {
                IsSuccess = bool.FalseString
            };

            var modelArgs = new QoModelFileReadApiArgs
            {
                ActorKey = args.ActorKey,
                ModelId = args.ModelId,
                LastUpdatedDate = args.ModelLastUpdatedDate,
            };

            var backgroundArgs = new QoBackgroundFileReadApiArgs
            {
                ActorKey = args.ActorKey,
                BackgroundId = args.BackgroundId,
                LastUpdatedDate = args.BackgroundLastUpdatedDate
            };

            var modelResults = _modelWorker.ReadFile(modelArgs);
            if(modelResults.IsSuccess == bool.FalseString)
            {
                result.Result = modelResults.Result;
                return result;
            }

            var backgroundResults = _backgroundWorker.ReadFile(backgroundArgs);
            if(backgroundResults.IsSuccess == bool.FalseString)
            {
                result.Result = backgroundResults.Result;
                return result;
            }

            result.ModelData = modelResults.Data;
            result.IsModelDataNoChanged = modelResults.IsDataNoChanged;
            result.ModelUpdatedDate = modelResults.UpdatedDate;
            result.BackgroundData = backgroundResults.Data;
            result.IsBackgroundDataNoChanged = backgroundResults.IsDataNoChanged;
            result.BackgroundUpdatedDate = backgroundResults.UpdatedDate;
            result.IsSuccess = bool.TrueString;

            return result;
        }
    }
}