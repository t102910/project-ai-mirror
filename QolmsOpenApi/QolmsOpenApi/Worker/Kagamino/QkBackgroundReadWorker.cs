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
    /// 背景データ取得Worker
    /// </summary>
    public class QkBackgroundReadWorker
    {
        IQkBackgroundRepository _backgroundRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="backgroundRepository"></param>
        public QkBackgroundReadWorker(IQkBackgroundRepository backgroundRepository)
        {
            _backgroundRepo = backgroundRepository;
        }

        /// <summary>
        /// 背景情報リストを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoBackgroundListReadApiResults ReadList(QoBackgroundListReadApiArgs args)
        {
            var result = new QoBackgroundListReadApiResults
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
                var entities = _backgroundRepo.ReadList();

                result.BackgroundItems = entities.ConvertAll(x => ConvertItem(x));
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
        /// モデル情報を取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoBackgroundReadApiResults Read(QoBackgroundReadApiArgs args)
        {
            var result = new QoBackgroundReadApiResults
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
                var entity = _backgroundRepo.Read(args.BackgroundId);


                if (entity != null)
                {
                    result.BackgroundItem = ConvertItem(entity);
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
        public QoBackgroundFileReadApiResults ReadFile(QoBackgroundFileReadApiArgs args)
        {
            var result = new QoBackgroundFileReadApiResults
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
                var entity = _backgroundRepo.Read(args.BackgroundId);
                if (entity == null || entity.DELETEFLAG)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "背景が存在しません。");
                    return result;
                }

                var lastUpdatedDate = args.LastUpdatedDate.TryToValueType(DateTime.MinValue);
                if (entity.UPDATEDDATE <= lastUpdatedDate)
                {
                    // 更新不要として正常終了
                    result.IsDataNoChanged = true;
                    result.UpdatedDate = entity.UPDATEDDATE.ToApiDateString();
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    result.IsSuccess = bool.TrueString;
                    return result;
                }

                var data = _backgroundRepo.ReadFile(entity.FILEKEY);
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

        QkBackgroundItem ConvertItem(QK_BACKGROUND_MST item)
        {
            return new QkBackgroundItem
            {
                BackgroundId = item.BACKGROUNDID,
                DisplayName = item.DISPLAYNAME,
                Description = item.DESCRIPTION,
                FileKey = item.FILEKEY.ToEncrypedReference(),                
                SortOrder = item.SORTORDER,
                UpdatedDate = item.UPDATEDDATE.ToApiDateString()
            };
        }
    }
}