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
    /// ランダムアドバイスの読み込み処理
    /// </summary>
    public class QkRandomAdviceReadWorker
    {
        IQkRandomAdviceRepository _adviceRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="randomAdviceRepository"></param>
        public QkRandomAdviceReadWorker(IQkRandomAdviceRepository randomAdviceRepository)
        {
            _adviceRepo = randomAdviceRepository;
        }

        /// <summary>
        /// ランダムアドバイスのリストを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoRandomAdviceListReadApiResults ReadList(QoRandomAdviceListReadApiArgs args)
        {
            var result = new QoRandomAdviceListReadApiResults
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
                List<QK_RANDOMADVICE_MST> entities = null;
                if (args.IsFilterCurrentSeason)
                {
                    var jstZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    var targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZoneInfo);
                    entities = _adviceRepo.ReadSeasonList(args.ModelId, targetDateTime.Month);
                }
                else
                {
                    entities = _adviceRepo.ReadList(args.ModelId);
                }

                result.AdviceItems = entities.ConvertAll(x => ConvertItem(x));
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
        /// ランダムアドバイスを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoRandomAdviceReadApiResults Read(QoRandomAdviceReadApiArgs args)
        {
            var result = new QoRandomAdviceReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アカウントキーが不正です。");
                return result;
            }

            var targetDateTime = args.TargetDateTime.TryToValueType(DateTime.MinValue);

            try
            {
                QK_RANDOMADVICE_MST entity = null;
                if(args.AdviceId >= 0)
                {
                    entity = _adviceRepo.Read(args.AdviceId);
                }
                else
                {
                    entity = PickRandomEntity(args.ModelId, targetDateTime, args.ExcludeAdviceIdList);
                }

                result.AdviceItem = ConvertItem(entity);
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

        QK_RANDOMADVICE_MST PickRandomEntity(string modelId, DateTime targetDateTime, List<int> excludeIds)
        {
            if (targetDateTime == DateTime.MinValue)
            {
                var jstZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

                targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZoneInfo);
            }

            var hour = TimeSpan.FromHours(targetDateTime.Hour);
            QkAdviceTimeType timeType;
            if (TimeSpan.FromHours(5) <= hour && hour < TimeSpan.FromHours(12))
            {
                timeType = QkAdviceTimeType.Morning;
            }
            else if (TimeSpan.FromHours(12) <= hour && hour < TimeSpan.FromHours(18))
            {
                timeType = QkAdviceTimeType.Afternoon;
            }
            else if (TimeSpan.FromHours(18) <= hour && hour < TimeSpan.FromHours(24))
            {
                timeType = QkAdviceTimeType.Evening;
            }
            else
            {
                timeType = QkAdviceTimeType.Night;
            }

            var entityList = _adviceRepo.ReadRandomList(modelId, targetDateTime.Month, timeType, excludeIds);

            var random = new Random();
            var pickedEntity = entityList.OrderBy(x => random.Next()).FirstOrDefault();

            if(pickedEntity == null)
            {
                return null;
            }

            return pickedEntity;
        }

        QkAdviceItem ConvertItem(QK_RANDOMADVICE_MST item)
        {
            if(item == null)
            {
                return null;
            }

            return new QkAdviceItem
            {
                ID = item.ID,
                ModelId = item.MODELID,
                CategoryType = (QkAdviceCategoryType)item.CATEGORYTYPE,
                TimeType = (QkAdviceTimeType)item.TIMETYPE,
                Advice = item.ADVICE,
                MotionType = (QkModelMotionType)item.MOTIONTYPE,
                M1 = item.M1,
                M2 = item.M2,
                M3 = item.M3,
                M4 = item.M4,
                M5 = item.M5,
                M6 = item.M6,
                M7 = item.M7,
                M8 = item.M8,
                M9 = item.M9,
                M10 = item.M10,
                M11 = item.M11,
                M12 = item.M12
            };
        }
    }
}