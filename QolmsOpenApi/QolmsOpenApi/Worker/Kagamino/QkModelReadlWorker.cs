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
    /// モデルデータ取得Worker
    /// </summary>
    public class QkModelReadlWorker
    {
        IQkModelRepository _modelRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="modelRepository"></param>
        public QkModelReadlWorker(IQkModelRepository modelRepository)
        {
            _modelRepo = modelRepository;
        }

        /// <summary>
        /// モデル情報リストを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoModelListReadApiResults ReadList(QoModelListReadApiArgs args)
        {
            var result = new QoModelListReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アカウントキーが不正です。");
                return result;
            }

            try
            {
                var entities = _modelRepo.ReadList(args.ContainsPaid);

                result.ModelItems = entities.ConvertAll(x => ConvertItem(x));
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
        /// モデル情報を取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoModelReadApiResults Read(QoModelReadApiArgs args)
        {
            var result = new QoModelReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アカウントキーが不正です。");
                return result;
            }

            try
            {
                var entity = _modelRepo.Read(args.ModelId);

                
                if(entity != null)
                {
                    result.ModelItem = ConvertItem(entity);
                    result.IsDeleted = entity.DELETEFLAG;
                }
                // entityがnullだとしても正常扱いとする
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

        /// <summary>
        /// モデルファイルを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoModelFileReadApiResults ReadFile(QoModelFileReadApiArgs args)
        {
            var result = new QoModelFileReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アカウントキーが不正です。");
                return result;
            }

            try
            {
                var entity = _modelRepo.Read(args.ModelId);
                if (entity == null || entity.DELETEFLAG)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "モデルが存在しません。");
                    return result;
                }

                var lastUpdatedDate = args.LastUpdatedDate.TryToValueType(DateTime.MinValue);
                if(entity.UPDATEDDATE <= lastUpdatedDate)
                {
                    // 更新不要として正常終了
                    result.IsDataNoChanged = true;
                    result.UpdatedDate = entity.UPDATEDDATE.ToApiDateString();
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    result.IsSuccess = bool.TrueString;
                    return result;
                }

                var data = _modelRepo.ReadFile(entity.FILEKEY);
                result.Data = data;
                result.IsDataNoChanged = false;
                result.UpdatedDate = entity.UPDATEDDATE.ToApiDateString();
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

        QkModelItem ConvertItem(QK_MODEL_MST item)
        {
            return new QkModelItem
            {
                ModelId = item.MODELID,
                DisplayName = item.DISPLAYNAME,
                Description = item.DESCRIPTION,
                FileKey = item.FILEKEY.ToEncrypedReference(),
                ThumbnailKey = item.THUMBNAILKEY.ToEncrypedReference(),
                IsPaid = item.ISPAID,
                SortOrder = item.SORTORDER,
                UpdatedDate = item.UPDATEDDATE.ToApiDateString()
            };
        }
    }
}