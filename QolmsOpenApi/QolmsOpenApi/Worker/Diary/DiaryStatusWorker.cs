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
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 日記状態取得処理
    /// </summary>
    public class DiaryStatusWorker
    {
        IEventRepository _eventRepo;

        /// <summary>
        /// 
        /// </summary>
        public DiaryStatusWorker(IEventRepository eventRepository)
        {
            _eventRepo = eventRepository;
        }

        /// <summary>
        /// 取得処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoDiaryStatusApiResults Read(QoDiaryStatusApiArgs args)
        {
            var results = new QoDiaryStatusApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // いいね数取得
            if(!TryReadLikeInfo(accountKey, results, out var likeEntity))
            {
                return results;
            }

            // 気分取得
            if(!TryReadFeelingInfo(accountKey, results, out var feelingType))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);      

            results.StatusItem = new QoDiaryStatusItem
            {
                TotalLike = likeEntity.TOTAL,
                TodayLike = likeEntity.TODAY,
                FeelingType = feelingType
            };           

            return results;
        }

        bool TryReadLikeInfo(Guid accountKey, QoApiResultsBase results, out QH_EVENTREACTION_LIKE_VIEW entity)
        {
            try
            {
                entity = _eventRepo.GetLikeCount(accountKey);
                return true;
            }
            catch(Exception ex)
            {
                entity = null;
                results.Result = QoApiResult.Build(ex, "いいね数取得処理に失敗しました。");
                return false;
            }
        }

        bool TryReadFeelingInfo(Guid accountKey, QoApiResultsBase results, out QoApiDiaryFeelingTypeEnum feelingType)
        {
            feelingType = QoApiDiaryFeelingTypeEnum.None;
            try
            {
                var entity = _eventRepo.GetDiaryPostCount(accountKey);

                if(entity.TODAY > 0)
                {
                    feelingType = QoApiDiaryFeelingTypeEnum.Good;
                }
                else if(entity.DAY1TO2 > 0)
                {
                    feelingType = QoApiDiaryFeelingTypeEnum.Normal;
                }
                else if(entity.DAY3TO6 > 0)
                {
                    feelingType = QoApiDiaryFeelingTypeEnum.NotGood;
                }
                else if (entity.TODAY == 0 && entity.DAY1TO2 == 0 && entity.DAY3TO6 == 0)
                {
                    feelingType = QoApiDiaryFeelingTypeEnum.Normal;
                }
                else
                {
                    feelingType = QoApiDiaryFeelingTypeEnum.Bad;
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "日記投稿数情報取得処理に失敗しました。");
                return false;
            }
        }
    }
}