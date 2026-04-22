using System;
using System.Linq;

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsCryptV1;

using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using System.Configuration;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// JOTO 関連の公開API
    /// </summary>
    public class JotoAppWorker
    {

        readonly IJotoAppRepository _JotoAppRepo;
        readonly IHealthRecordRepository _HealthRecordRepo;
        readonly IEventRepository _EventRepo;
        readonly INoticeGroupRepository _NoticeGroupRepo;
        readonly IAppEventRepository _AppEventRepo;
        readonly IPhrForHomeRepository _PhrForHomeRepo;
        readonly CalomealMealSyncRepository _CalomealMealSyncRepo;

        /// <summary>
        /// 
        /// </summary>
        public JotoAppWorker(IJotoAppRepository repo)
        {
            _JotoAppRepo = repo;
        }

        /// <summary>
        /// 
        /// </summary>
        public JotoAppWorker(IHealthRecordRepository repo)
        {
            _HealthRecordRepo = repo;
        }

        /// <summary>
        /// 
        /// </summary>
        public JotoAppWorker(IEventRepository repo)
        {
            _EventRepo = repo;
        }

        /// <summary>
        /// 
        /// </summary>
        public JotoAppWorker(INoticeGroupRepository repo)
        {
            _NoticeGroupRepo = repo;
        }

        /// <summary>
        ///
        /// </summary>
        public JotoAppWorker(IAppEventRepository repo)
        {
            _AppEventRepo = repo;
        }

        /// <summary>
        ///
        /// </summary>
        internal JotoAppWorker(IPhrForHomeRepository repo)
        {
            _PhrForHomeRepo = repo;
        }

        /// <summary>
        ///
        /// </summary>
        internal JotoAppWorker(CalomealMealSyncRepository repo)
        {
            _CalomealMealSyncRepo = repo;
        }

        /// <summary>
        /// PHR削除（Vital）用コンストラクタ。
        /// CalorieBurn (_JotoAppRepo) とその他 Vital (_HealthRecordRepo) の
        /// 両方の処理に対応するために両リポジトリを受け取ります。
        /// </summary>
        public JotoAppWorker(IHealthRecordRepository healthRepo, IJotoAppRepository jotoRepo)
        {
            _HealthRecordRepo = healthRepo;
            _JotoAppRepo = jotoRepo;
        }


        #region "Private Method"

        private static string MakeLogData<TArgs, TResults>(TArgs args, TResults results)
            where TArgs : QoApiArgsBase
            where TResults : QoApiResultsBase, new()
        {
            // Dim cryptData As String
            string jsonStr = "";

            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                QsJsonSerializer js = new QsJsonSerializer();
                jsonStr = args == null ? "" : js.Serialize<TArgs>(args);
                jsonStr += results == null ? "" : js.Serialize<TResults>(results);
            }
            return jsonStr;
        }

        /// <summary>
        /// アカウントインデックスデータテーブルエンティティを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="includeDelete">
        /// 削除済みも対象にするかのフラグ（オプショナル）。
        /// 対象にするなら True、
        /// 対象にしない False（デフォルト）を指定します。</param>
        /// <returns>データが存在するなら該当するテーブルエンティティ、存在しないなら Nothing。</returns>
        private static QH_ACCOUNTINDEX_DAT SelectAccountIndexEntity(Guid accountKey, bool includeDelete = false)
        {
            var entity = new QH_ACCOUNTINDEX_DAT() { ACCOUNTKEY = accountKey };
            var reader = new QhAccountIndexEntityReader();
            var readerArgs = new QhAccountIndexEntityReaderArgs() { Data = new List<QH_ACCOUNTINDEX_DAT>() { entity } };
            QhAccountIndexEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess &&
                readerResults.Result.Count == 1 &&
                (includeDelete || !readerResults.Result.First().DELETEFLAG))
            {
                return readerResults.Result.First();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// JOTO ネイティブ 版 ホーム 画面用の医療・健康情報取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// ホーム 画面用の医療・健康情報
        /// </returns>
        public QoJotoAppPhrReadForHomeReadApiResults PhrReadForHomeRead(QoJotoAppPhrReadForHomeReadApiArgs args)
        {
            QoJotoAppPhrReadForHomeReadApiResults results = new QoJotoAppPhrReadForHomeReadApiResults()
            {
                TargetDate = DateTime.Now.ToApiDateString(),
                IsSuccess = bool.FalseString,
                Result = null
            };

            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            DateTime targetDate = args.TargetDate.TryToValueType(DateTime.MinValue);

            // パラメータチェック
            if (
                accountKey == Guid.Empty ||
                targetDate == DateTime.MinValue
                )
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.ActorKey}], [{args.TargetDate}]");
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                return results;
            }

            if (_PhrForHomeRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "PhrForHomeRepository が設定されていません。");
                return results;
            }

            // Repository経由で医療・健康情報を取得
            PhrForHomeReaderResults readerResults = _PhrForHomeRepo.ReadForHome(accountKey, targetDate);

            if (readerResults != null && readerResults.IsSuccess)
            {
                // 取得したデータを結果にマッピング
                results.TargetDate = readerResults.TargetDate;
                results.DailyCaloricIntake = readerResults.DailyCaloricIntake != int.MinValue ? readerResults.DailyCaloricIntake.ToString() : string.Empty;
                results.CaloriesBurned = readerResults.CaloriesBurned != int.MinValue ? readerResults.CaloriesBurned.ToString() : string.Empty;
                results.Steps = readerResults.Steps != decimal.MinValue ? readerResults.Steps.ToString() : string.Empty;
                results.Exercise = readerResults.Exercise != int.MinValue ? readerResults.Exercise.ToString() : string.Empty;
                results.CaloryInMeal = readerResults.CaloryInMeal != int.MinValue ? readerResults.CaloryInMeal.ToString() : string.Empty;
                results.MealType = readerResults.MealType;
                results.BodyWeight = readerResults.BodyWeight != decimal.MinValue ? readerResults.BodyWeight.ToString() : string.Empty;
                results.BMI = readerResults.BMI != decimal.MinValue ? readerResults.BMI.ToString() : string.Empty;
                results.Feelings = readerResults.Feelings;
                results.BedTime = readerResults.BedTime;
                results.WakeupTime = readerResults.WakeupTime;
                results.SleepTime = readerResults.SleepTime != decimal.MinValue ? readerResults.SleepTime.ToString() : string.Empty;
                results.SBP1 = readerResults.SBP1 != decimal.MinValue ? readerResults.SBP1.ToString() : string.Empty;
                results.DBP1 = readerResults.DBP1 != decimal.MinValue ? readerResults.DBP1.ToString() : string.Empty;
                results.SBP2 = readerResults.SBP2 != decimal.MinValue ? readerResults.SBP2.ToString() : string.Empty;
                results.DBP2 = readerResults.DBP2 != decimal.MinValue ? readerResults.DBP2.ToString() : string.Empty;
                results.BloodSugarLevel1 = readerResults.BloodSugarLevel1 != decimal.MinValue ? readerResults.BloodSugarLevel1.ToString() : string.Empty;
                results.BloodSugarLevelTiming1 = readerResults.BloodSugarLevelTiming1;
                results.BloodSugarLevelLogTime1 = readerResults.BloodSugarLevelLogTime1;
                results.BloodSugarLevel2 = readerResults.BloodSugarLevel2 != decimal.MinValue ? readerResults.BloodSugarLevel2.ToString() : string.Empty;
                results.BloodSugarLevelTiming2 = readerResults.BloodSugarLevelTiming2;
                results.BloodSugarLevelLogTime2 = readerResults.BloodSugarLevelLogTime2;
                results.IsSuccess = bool.TrueString;
            }
    
            return results;
        }

        /// <summary>
        /// PHR情報取得を行う。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoJotoAppPhrReadApiResults PhrRead(QoJotoAppPhrReadApiArgs args)
        {
            QoJotoAppPhrReadApiResults result = new QoJotoAppPhrReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };
            // チェック
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                QoAccessLog.WriteInfoLog($"LinkageSystemNo Error");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"LinkageSystemNo Error");
                return result;
            }
            //var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            //if (systemType.ToString() != QsApiSystemTypeEnum.JotoGinowan.ToString())
            //{
            //    QoAccessLog.WriteInfoLog($"ExecuteSystemType Error");
            //    result.IsSuccess = bool.FalseString;
            //    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            //    return result;
            //}
            if (string.IsNullOrWhiteSpace(args.StartDate))
            {
                QoAccessLog.WriteInfoLog($"StartDate Error");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"StartDate Error");
                return result;
            }
            if (string.IsNullOrWhiteSpace(args.EndDate))
            {
                QoAccessLog.WriteInfoLog($"StartDate Error");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"StartDate Error");
                return result;
            }

            if (!DateTime.TryParse(args.StartDate, out DateTime startDate))
            {
                QoAccessLog.WriteInfoLog($"StartDate Error startDate:{args.StartDate}");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"StartDate Error startDate:{startDate}");
                return result;
            }
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0);

            if (!DateTime.TryParse(args.EndDate, out DateTime endDate))
            {
                QoAccessLog.WriteInfoLog($"EndDate Error endDate:{args.EndDate}");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"EndDate Error endDate:{endDate}");
                return result;
            }
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

            if (startDate > endDate)
            {
                QoAccessLog.WriteInfoLog($"StartDate > EndDate Error startDate:{startDate}, endDate:{endDate}");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"StartDate > EndDate Error startDate:{startDate}, endDate:{endDate}");
                return result;
            }
            
            try
            {
                var data = new List<QoApiPhrValueItem>();
                switch (args.DataType)
                {   
                    case "Vital":
                        // VitalType パースは Vital case のみで実施
                        // Exercise / Mental では VitalType を使用しないため、switch の外でのパースは行わない
                        if (!Enum.TryParse<QvApiVitalTypeEnum>(args.VitalType, out QvApiVitalTypeEnum vitalType))
                        {
                            QoAccessLog.WriteInfoLog($"VitalType Error, VitalType:{args.VitalType}");
                            result.IsSuccess = bool.FalseString;
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"VitalType Error, VitalType:{args.VitalType}");
                            return result;
                        }

                        var bodyHeight = decimal.MinValue;
                        var vital = _HealthRecordRepo.ReadRange(accountKey, startDate, endDate, new byte[] { (byte)vitalType });

                        switch (vitalType)
                        {
                            case QvApiVitalTypeEnum.BodyWeight:
                                //最新の身長取得
                                var vitalHeight = _HealthRecordRepo.ReadNew(accountKey, endDate, (byte)QvApiVitalTypeEnum.BodyHeight);
                                if (vitalHeight != null && vitalHeight.Count() == 1 && vitalHeight.First().VALUE1 > 0)
                                {
                                    bodyHeight = vitalHeight.First().VALUE1 / 100m;// cm から m に変換
                                }
                                break;
                            default:
                                break;
                        }
                        
                        foreach (var row in vital)
                        {
                            data.Add(new QoApiPhrValueItem()
                            {
                                RecordDate = row.RECORDDATE.ToApiDateString(),
                                ItemType = row.VITALTYPE.ToString(),
                                Value1 = row.VALUE1.ToString(),
                                Value2 = bodyHeight == decimal.MinValue ? 
                                   row.VALUE2.ToString() : Math.Round(row.VALUE1 / (bodyHeight * bodyHeight), 1).ToString(),
                                Value3 = string.Empty,
                                Value4 = string.Empty,
                                ConditionType = row.CONDITIONTYPE.ToString(),
                            });
                        }
                        result.Data = data;
                        break;
                    case "Exercise":
                        // LINKAGESYSTEMNO フィルタなし（旧:99999 / 新:47003 両方取得）
                        // ItemType=ExerciseType, Value1=Calorie, Value2=StartDate,
                        // Value3=EndDate, Value4=ItemName, ConditionType=Sequence
                        var exercise = _JotoAppRepo.ReadExerciseRange(accountKey, startDate, endDate);
                        foreach (var row in exercise)
                        {
                            data.Add(new QoApiPhrValueItem()
                            {
                                RecordDate    = row.RECORDDATE.ToApiDateString(),
                                ItemType      = row.EXERCISETYPE.ToString(),
                                Value1        = row.CALORIE.ToString(),
                                Value2        = row.STARTDATE.ToApiDateString(),
                                Value3        = row.ENDDATE.ToApiDateString(),
                                Value4        = row.ITEMNAME ?? string.Empty,
                                ConditionType = row.SEQUENCE.ToString(),
                            });
                        }
                        result.Data = data;
                        break;
                    case "Mental":
                        var mental = _JotoAppRepo.ReadMentalRange(accountKey, startDate, endDate, linkageSystemNo);
                        QsJsonSerializer ser = new QsJsonSerializer();

                        foreach (var row in mental)
                        {
                            var set = ser.Deserialize<QhQolmsDiaryEventSetOfJson>(row.EVENTSET);
                            if (!Enum.TryParse<QoApiDiaryFeelingTypeEnum>(set.FeelingType, out QoApiDiaryFeelingTypeEnum FeelingType))
                            {
                                FeelingType = QoApiDiaryFeelingTypeEnum.None;
                            }
                            data.Add(new QoApiPhrValueItem()
                            {
                                RecordDate = row.EVENTDATE.ToApiDateString(),
                                ItemType = string.Empty,
                                Value1 = set.Contents,
                                Value2 = row.EVENTSEQUENCE.ToString(),
                                Value3 = string.Empty,
                                Value4 = string.Empty,
                                ConditionType = FeelingType.ToString(),
                            });
                        }
                        result.Data = data;
                        break;
                    default:
                        result.IsSuccess = bool.FalseString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"DataType Error {args.DataType}");
                        return result;
                }

                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return result;

            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }

        }

        /// <summary>
        /// お知らせ（グループ）リストを取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// お知らせ（グループ）情報リスト
        /// </returns>
        public QoJotoAppNoticeGroupReadApiResults NoticeGroupRead(QoJotoAppNoticeGroupReadApiArgs args)
        {
            QoJotoAppNoticeGroupReadApiResults results = new QoJotoAppNoticeGroupReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            byte targetType = args.TargetType.TryToValueType(byte.MinValue); // 0=すべて、1=全員、 2=個人(あなた)
            byte categoryNo = args.CategoryNo.TryToValueType(byte.MinValue); // 0=すべて、1=お知らせ、 2=メンテナンス
            byte alreadyRead = args.AlreadyRead.TryToValueType(byte.MinValue); // 0=すべて、1=未読、 2=既読
            int pageIndex = args.PageIndex.TryToValueType(int.MinValue);
            int pageSize = args.PageSize.TryToValueType(int.MinValue);

            // パラメータチェック
            if (accountKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.ActorKey}]");
                return results;
            }

            if (_NoticeGroupRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "NoticeGroupRepository が設定されていません。");
                return results;
            }

            // Repository経由でお知らせ情報を取得
            List<DbNoticeGroupItem> noticeGroups = _NoticeGroupRepo.ReadNoticeGroupList(accountKey, targetType, categoryNo, alreadyRead, pageIndex, pageSize);
            if (noticeGroups != null)
            {
                results.NoticeGroupN = noticeGroups.ConvertAll(x => x.ToApiJotoAppNoticeGroupItem());
                results.IsSuccess = bool.TrueString;
            }

            return results;
        }

        /// <summary>
        /// お知らせ（グループ）詳細情報を取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// お知らせ（グループ）詳細情報
        /// </returns>
        public QoJotoAppNoticeGroupDetailReadApiResults NoticeGroupDetailRead(QoJotoAppNoticeGroupDetailReadApiArgs args)
        {
            QoJotoAppNoticeGroupDetailReadApiResults results = new QoJotoAppNoticeGroupDetailReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            long noticeNo = args.NoticeNo.TryToValueType(long.MinValue);

            // パラメータチェック
            if (accountKey == Guid.Empty || noticeNo == long.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.ActorKey}], [{args.NoticeNo}]");
                return results;
            }

            if (_NoticeGroupRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "NoticeGroupRepository が設定されていません。");
                return results;
            }

            // Repository経由でお知らせ詳細情報を取得
            DbNoticeGroupItem noticeGroup = _NoticeGroupRepo.ReadNoticeGroupDetail(accountKey, noticeNo);
            if (noticeGroup != null)
            {
                results.NoticeGroup = noticeGroup.ToApiJotoAppNoticeGroupDetailItem();
                results.IsSuccess = bool.TrueString;
            }

            return results;

        }

        /// <summary>
        /// お知らせ（グループ）既読状態を更新します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>更新結果</returns>
        public QoJotoAppNoticeGroupTargetWriteApiResults NoticeGroupTargetWrite(QoJotoAppNoticeGroupTargetWriteApiArgs args)
        {
            QoJotoAppNoticeGroupTargetWriteApiResults results = new QoJotoAppNoticeGroupTargetWriteApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            long noticeNo = args.NoticeNo.TryToValueType(long.MinValue);

            if (accountKey == Guid.Empty || noticeNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.ActorKey}], [{args.NoticeNo}]");
                return results;
            }

            if (string.IsNullOrWhiteSpace(args.AlreadyReadFlag))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AlreadyReadFlag が指定されていません。");
                return results;
            }

            byte alreadyReadType = args.AlreadyReadFlag.TryToValueType(byte.MaxValue);
            if (alreadyReadType != 0 && alreadyReadType != 1)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AlreadyReadFlag は 0(未読) または 1(既読) で指定してください。");
                return results;
            }
            bool alreadyReadFlag = alreadyReadType == 1;

            if (_NoticeGroupRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "NoticeGroupRepository が設定されていません。");
                return results;
            }

            DateTime now = DateTime.Now;
            DateTime alreadyReadDate = alreadyReadFlag ? now : DateTime.MinValue;
            bool isSuccess = _NoticeGroupRepo.UpdateNoticeGroupTarget(noticeNo, accountKey, alreadyReadFlag, alreadyReadDate, now);

            if (!isSuccess)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return results;
        }

        /// <summary>
        /// お知らせ（グループ）画像を取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// お知らせ（グループ）画像情報
        /// </returns>
        public QoJotoAppNoticeGroupImageReadApiResults NoticeGroupImageRead(QoJotoAppNoticeGroupImageReadApiArgs args)
        {
            QoJotoAppNoticeGroupImageReadApiResults results = new QoJotoAppNoticeGroupImageReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            if (args == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "引数が不正です。");
                return results;
            }

            if (args.FileType.TryToValueType(QsApiFileTypeEnum.None) == QsApiFileTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
                return results;
            }

            Guid fileKey = args.FileKeyReference.ToDecrypedReference<Guid>();
            if (fileKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FileKeyが不正です。");
                return results;
            }

            QhBlobStorageReadApiResults apiResults = QoBlobStorage.Read<QH_UPLOADFILE_DAT>(new QhBlobStorageReadApiArgs()
            {
                ActorKey = args.ActorKey,
                ApiType = QhApiTypeEnum.FileStorageRead.ToString(),
                ExecuteSystemType = args.ExecuteSystemType,
                Executor = args.Executor,
                ExecutorName = args.ExecutorName,
                FileKey = fileKey.ToApiGuidString(),
                FileRelationType = QsApiFileRelationTypeEnum.Notice.ToString(),
                FileType = args.FileType
            });

            if (apiResults != null && apiResults.IsSuccess == bool.TrueString && !string.IsNullOrWhiteSpace(apiResults.Data) && !string.IsNullOrWhiteSpace(apiResults.ContentType))
            {
                results.Image.OriginalName = apiResults.OriginalName;
                results.Image.ContentType = apiResults.ContentType;
                results.Image.Data = apiResults.Data;
                results.IsSuccess = bool.TrueString;
            }

            return results;
        }

        /// <summary>
        /// アプリイベント一覧情報を取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// アプリイベント情報一覧
        /// </returns>
        public QoJotoAppAppEventReadApiResults AppEventRead(QoJotoAppAppEventReadApiArgs args)
        {
            QoJotoAppAppEventReadApiResults results = new QoJotoAppAppEventReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            int pageIndex = args.PageIndex.TryToValueType(int.MinValue);
            int pageSize = args.PageSize.TryToValueType(int.MinValue);
            Guid accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            if (accountKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.ActorKey}]");
                return results;
            }

            if (_AppEventRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "AppEventRepository が設定されていません。");
                return results;
            }

            // pageIndex/pageSize が有効値のときのみDB側でページングされる。
            // accountKey を渡して、ユーザーが実際にアクセス可能な LinkageSystemNo でフィルタリング。
            List<DbAppEventItem> appEvents = _AppEventRepo.ReadAppEventList(pageIndex, pageSize, accountKey);
            if (appEvents != null)
            {
                results.AppEventN = appEvents.ConvertAll(x => x.ToApiJotoAppAppEventItem());
                results.IsSuccess = bool.TrueString;
            }

            return results;
        }

        /// <summary>
        /// アプリイベント詳細情報を取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// アプリイベント詳細情報
        /// </returns>
        public QoJotoAppAppEventDetailReadApiResults AppEventDetailRead(QoJotoAppAppEventDetailReadApiArgs args)
        {
            QoJotoAppAppEventDetailReadApiResults results = new QoJotoAppAppEventDetailReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            Guid eventKey = args.EventKey.TryToValueType(Guid.Empty);
            if (eventKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.EventKey}]");
                return results;
            }

            if (_AppEventRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "AppEventRepository が設定されていません。");
                return results;
            }

            // 詳細はイベントキーで単一レコードを取得する。
            DbAppEventItem appEvent = _AppEventRepo.ReadAppEventDetail(eventKey);
            if (appEvent != null)
            {
                results.AppEvent = appEvent.ToApiJotoAppAppEventDetailItem();
                results.IsSuccess = bool.TrueString;
            }

            return results;
        }

        /// <summary>
        /// アプリイベント画像を取得します。
        /// </summary>
        /// <param name="args">API パラメータオブジェクト</param>
        /// <returns>
        /// アプリイベント画像情報
        /// </returns>
        public QoJotoAppAppEventImageReadApiResults AppEventImageRead(QoJotoAppAppEventImageReadApiArgs args)
        {
            QoJotoAppAppEventImageReadApiResults results = new QoJotoAppAppEventImageReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            if (args == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "引数が不正です。");
                return results;
            }

            Guid eventKey = args.EventKey.TryToValueType(Guid.Empty);
            if (eventKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "EventKeyが不正です。");
                return results;
            }

            Guid fileKey = args.FileKeyReference.ToDecrypedReference<Guid>();
            if (fileKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FileKeyが不正です。");
                return results;
            }

            if (_AppEventRepo == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "AppEventRepository が設定されていません。");
                return results;
            }

            // EventKey + FileKey で APPEVENTFILE_DAT を照合し、イベント紐づき不一致を除外する。
            QH_APPEVENTFILE_DAT appEventFile = _AppEventRepo.ReadAppEventFile(eventKey, fileKey);
            if (appEventFile == null || appEventFile.EVENTKEY == Guid.Empty || appEventFile.FILEKEY == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "指定されたイベントファイルが存在しません。");
                return results;
            }

            try
            {
                var reader = new AppEventFileBlobEntityReader();
                var readerResults = reader.Execute(new AppEventFileBlobEntityReaderArgs() { Name = fileKey });

                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Data != null && readerResults.Result.Data.Length > 0)
                {
                    results.Image.OriginalName = readerResults.Result.OriginalName;
                    results.Image.ContentType = readerResults.Result.ContentType;
                    results.Image.Data = Convert.ToBase64String(readerResults.Result.Data);
                    results.IsSuccess = bool.TrueString;
                }
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return results;
        }

        /// <summary>
        /// 心の状態を登録する。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoJotoAppMentalImportApiResults MentalImport(QoJotoAppMentalImportApiArgs args)
        {
            QoJotoAppMentalImportApiResults result = new QoJotoAppMentalImportApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            // チェック
            var accountKey = args.ActorKey.TryToValueType<Guid>(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                QoAccessLog.WriteInfoLog($"LinkageSystemNo Error");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"LinkageSystemNo Error");
                return result;
            }
            //var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            //if (systemType.ToString() != QsApiSystemTypeEnum.JotoGinowan.ToString())
            //{
            //    QoAccessLog.WriteInfoLog($"ExecuteSystemType Error");
            //    result.IsSuccess = bool.FalseString;
            //    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            //    return result;
            //}

            if (args.MentalValue == null)
            {
                QoAccessLog.WriteInfoLog($"MentalValue Error None");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"MentalValue Error None");
                return result;
            }

            if (!DateTime.TryParse(args.MentalValue.RecordDate, out DateTime recordDate))
            {
                QoAccessLog.WriteInfoLog($"RecordDate Error recordDate:{recordDate}");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"RecordDate Error recordDate:{recordDate}");
                return result;
            }

            if (!Enum.TryParse<QoApiDiaryFeelingTypeEnum>(args.MentalValue.ConditionType, out QoApiDiaryFeelingTypeEnum feelingType))
            {
                feelingType = QoApiDiaryFeelingTypeEnum.None;
            }

            try
            {
                QsJsonSerializer ser = new QsJsonSerializer();

                DbCalendarEventWriter<QhQolmsDiaryEventSetOfJson> writer = new DbCalendarEventWriter<QhQolmsDiaryEventSetOfJson>();
                DbCalendarEventWriterArgs writerArgs = new DbCalendarEventWriterArgs()
                {
                    AuthorKey = accountKey,
                    ActorKey =accountKey,
                    WriteModeType = DbCalendarWriterCore.EventWriteModeTypeEnum.Added,
                    LinkageSystemNo = linkageSystemNo,
                    EventDate = recordDate,
                    EventSequence = 0,
                    StartDate = recordDate,
                    EndDate = recordDate,
                    EventType = QsDbEventTypeEnum.System,
                    Name = string.Empty,
                    AlldayFlag = true,
                    FinishFlag = false,
                    NoticeFlag = false,
                    OpenFlag = false,
                    Importance = byte.MinValue,
                    CustomStampNo = int.MinValue,
                    EventSetTypeName = "QhQolmsDiaryEventSetOfJson",
                    EventSet = ser.Serialize<QhQolmsDiaryEventSetOfJson>(new QhQolmsDiaryEventSetOfJson() { Contents = args.MentalValue.Value1, FeelingType = feelingType.ToString() }),
                    ForeignKey = ser.Serialize<QhEventForeignKeyOfJson>(new QhEventForeignKeyOfJson()),
                    CategoryNoN = new List<byte>() { 1 },
                    SystemTagNoN = new List<int>() { (int)QsDbCalendarTagTypeEnum.System },
                    CustomTagNoN = new List<int>() { 0 }
                };

                DbCalendarEventWriterResults writerResults = new DbCalendarEventWriterResults();

                var writerResult =  QsDbManager.Write(writer, writerArgs);

                if (writerResult != null && writerResult.IsSuccess)
                {
                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    return result;

                }

                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, $"登録に失敗しました。");

                return result;
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }

            return result;
        }

        /// <summary>
        /// PHR情報取得を行う。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoJotoAppPhrDeleteApiResults PhrDelete(QoJotoAppPhrDeleteApiArgs args)
        {
            QoJotoAppPhrDeleteApiResults result = new QoJotoAppPhrDeleteApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };
            // チェック
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                QoAccessLog.WriteInfoLog($"LinkageSystemNo Error");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"LinkageSystemNo Error");
                return result;
            }
            //var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            //if (systemType.ToString() != QsApiSystemTypeEnum.JotoGinowan.ToString())
            //{
            //    QoAccessLog.WriteInfoLog($"ExecuteSystemType Error");
            //    result.IsSuccess = bool.FalseString;
            //    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            //    return result;
            //}
            if (string.IsNullOrWhiteSpace(args.RecordDate))
            {
                QoAccessLog.WriteInfoLog($"RecordDate Error");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"RecordDate Error");
                return result;
            }

            if (!DateTime.TryParse(args.RecordDate, out DateTime recordDate))
            {
                QoAccessLog.WriteInfoLog($"RecordDate Error recordDate:{args.RecordDate}");
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"RecordDate Error recordDate:{recordDate}");
                return result;
            }

            try
            {
                switch (args.DataType)
                {
                    case "Vital":
                        // VitalType パースは Vital case のみで実施
                        if (!Enum.TryParse<QsDbVitalTypeEnum>(args.VitalType, out QsDbVitalTypeEnum vitalType))
                        {
                            QoAccessLog.WriteInfoLog($"VitalType Error, VitalType:{args.VitalType}");
                            result.IsSuccess = bool.FalseString;
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"VitalType Error, VitalType:{args.VitalType}");
                            return result;
                        }

                        // CalorieBurn は QH_EXERCISEEVENT2_DAT に格納されているため専用削除処理へ委譲
                        // （旧方式データのため ExerciseType 指定なし＝当日全件削除）
                        if (vitalType == QsDbVitalTypeEnum.CalorieBurn)
                        {
                            _JotoAppRepo.DeleteExercise(accountKey, recordDate, 0);
                            result.IsSuccess = bool.TrueString;
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                            return result;
                        }

                        var entites = _HealthRecordRepo.ReadRange(accountKey, recordDate, recordDate, new byte[] { (byte)vitalType });
                        var vitals = new List<DbVitalValueItem>();
                        foreach (var entity in entites)
                        {
                            vitals.Add(new DbVitalValueItem()
                            {
                                RecordDate = entity.RECORDDATE,
                                VitalType = (QsDbVitalTypeEnum)entity.VITALTYPE,
                                Value1 = entity.VALUE1,
                                Value2 = entity.VALUE2,
                                ConditionType = entity.CONDITIONTYPE,
                            });
                        }

                        _HealthRecordRepo.DeleteVitals(accountKey, accountKey, vitals);
                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                        return result;
                    case "Mental":
                        // Mental は VitalType 不使用
                        DbCalendarEventWriter<QhQolmsDiaryEventSetOfJson> writer = new DbCalendarEventWriter<QhQolmsDiaryEventSetOfJson>();
                        DbCalendarEventWriterArgs writerArgs = new DbCalendarEventWriterArgs()
                        {
                            AuthorKey = accountKey,
                            ActorKey = accountKey,
                            WriteModeType = DbCalendarWriterCore.EventWriteModeTypeEnum.Deleted,
                            LinkageSystemNo = linkageSystemNo,
                            EventDate = recordDate,
                            EventSequence = 0
                        };

                        DbCalendarEventWriterResults writerResults = new DbCalendarEventWriterResults();

                        var writerResult = QsDbManager.Write(writer, writerArgs);

                        if (writerResult != null && writerResult.IsSuccess)
                        {
                            result.IsSuccess = bool.TrueString;
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                            return result;
                        }

                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, $"削除に失敗しました。");
                        return result;

                    case "Exercise":
                        // VitalType フィールドを ExerciseType（byte）として利用する。
                        // 数値（例:"1","255"）が送られた場合は個別削除、
                        // 数値でない or 省略の場合は 0（当日全件削除）にフォールバック。
                        // LINKAGESYSTEMNO フィルタなし（旧:99999 / 新:47003 両方が対象）。
                        byte.TryParse(args.VitalType, out byte exerciseType); // 失敗時は 0
                        _JotoAppRepo.DeleteExercise(accountKey, recordDate, exerciseType);
                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                        return result;

                    default:
                        result.IsSuccess = bool.FalseString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"DataType Error {args.DataType}");
                        return result;
                }

            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }

        }




        /// <summary>
        /// 運動マスタ一覧を取得します（LINKAGESYSTEMNO=47003 固定）。
        /// POST /JotoApp/ExerciseItemList
        /// </summary>
        public QoJotoAppExerciseItemListApiResults ExerciseItemList(QoJotoAppExerciseItemListApiArgs args)
        {
            var result = new QoJotoAppExerciseItemListApiResults()
            {
                IsSuccess = bool.FalseString,
                Result    = null,
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,
                    "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            if (_JotoAppRepo == null)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError,
                    "JotoAppRepository が設定されていません。");
                return result;
            }

            try
            {
                var masterList = _JotoAppRepo.ReadExerciseItemList();

                result.ExerciseItemN = masterList.Select(m => new QoJotoAppExerciseItemValue()
                {
                    ExerciseType = m.EXERCISETYPE.ToString(),
                    ExerciseName = m.EXERCISENAME,
                    // EXERCISETYPE=255（その他）はカロリー=0として返す
                    Calorie      = m.EXERCISETYPE == 255 ? "0" : m.CALORIE.ToString(),
                    ForeignKey   = m.FOREIGNKEY ?? string.Empty,
                }).ToList();

                result.IsSuccess = bool.TrueString;
                result.Result    = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return result;
        }

        /// <summary>
        /// 運動データを登録します（新方式: LINKAGESYSTEMNO=47003）。
        /// POST /JotoApp/ExerciseImport
        /// </summary>
        public QoJotoAppExerciseImportApiResults ExerciseImport(QoJotoAppExerciseImportApiArgs args)
        {
            var result = new QoJotoAppExerciseImportApiResults()
            {
                IsSuccess = bool.FalseString,
                Result    = null,
            };

            // ---- 引数チェック ----
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,
                    "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            // LINKAGESYSTEMNO は 47003 固定（ExerciseItemList と統一）
            const int linkageSystemNo = 47003;

            if (args.ExerciseValueN == null || !args.ExerciseValueN.Any())
            {
                QoAccessLog.WriteInfoLog("ExerciseImport: ExerciseValueN が空です。");
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,
                    "ExerciseValueN Error None");
                return result;
            }

            // ---- マスタ取得（ItemName / ForeignKey 補完用）----
            List<QH_EXERCISEITEM2_MST> masterList = null;
            try
            {
                masterList = _JotoAppRepo.ReadExerciseItemList();
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog($"ExerciseImport マスタ取得失敗: {ex.Message}", Guid.Empty);
                result.Result = QoApiResult.Build(ex);
                return result;
            }

            // ---- 入力値をライター引数に変換 ----
            var importItems = new List<ExerciseEventImportItem>();

            foreach (var v in args.ExerciseValueN)
            {
                // ExerciseType パース
                if (!byte.TryParse(v.ExerciseType, out byte exerciseType) || exerciseType == 0)
                {
                    QoAccessLog.WriteInfoLog(
                        $"ExerciseImport: ExerciseType が不正です。ExerciseType={v.ExerciseType}");
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,
                        $"ExerciseType が不正です。ExerciseType={v.ExerciseType}");
                    return result;
                }

                if (!DateTime.TryParse(v.RecordDate, out DateTime recordDate))
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,
                        $"RecordDate Error: {v.RecordDate}");
                    return result;
                }

                if (!DateTime.TryParse(v.StartDate, out DateTime startDate))
                    startDate = recordDate;

                if (!DateTime.TryParse(v.EndDate, out DateTime endDate))
                    endDate = recordDate;

                if (!short.TryParse(v.Calorie, out short calorie) || calorie < 1 || calorie > 9999)
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,
                        $"Calorie が不正です（1〜9999）。Calorie={v.Calorie}");
                    return result;
                }

                // ItemName / ForeignKey: クライアント未指定の場合マスタから補完
                // EXERCISETYPE=255（その他）はマスタ「その他」行を使用
                var master = masterList?.FirstOrDefault(m => m.EXERCISETYPE == exerciseType);

                string itemName   = string.IsNullOrWhiteSpace(v.ItemName)
                                    ? (master?.EXERCISENAME ?? string.Empty)
                                    : v.ItemName;
                string foreignKey = string.IsNullOrWhiteSpace(v.ForeignKey)
                                    ? (master?.FOREIGNKEY ?? string.Empty)
                                    : v.ForeignKey;

                importItems.Add(new ExerciseEventImportItem()
                {
                    RecordDate   = recordDate,
                    ExerciseType = exerciseType,
                    StartDate    = startDate,
                    EndDate      = endDate,
                    ItemName     = itemName,
                    Calorie      = calorie,   // ExerciseType=255 を含む全種別で直接入力値を使用
                    ForeignKey   = foreignKey,
                    Value        = v.Value ?? string.Empty,
                });
            }

            // ---- Writer 呼び出し ----
            try
            {
                var writer     = new ExerciseEventImportWriter();
                var writerArgs = new ExerciseEventImportWriterArgs()
                {
                    AuthorKey    = accountKey,
                    ActorKey     = accountKey,
                    ExerciseEventN = importItems,
                };

                var writerResult = QsDbManager.Write(writer, writerArgs);

                if (writerResult != null && writerResult.IsSuccess)
                {
                    result.IsSuccess = bool.TrueString;
                    result.Result    = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    return result;
                }

                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError,
                    "運動データの登録に失敗しました。");
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return result;
        }


        #region SleepImport

        private static readonly TimeZoneInfo _jstZone = GetJstTimeZone();

        private static TimeZoneInfo GetJstTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"); }
            catch { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo"); }
        }

        /// <summary>
        /// 日時文字列を JST の DateTime としてパースします。
        ///
        /// サーバーのOSタイムゾーン設定に依存せず、受け取った値を
        /// 「JST のローカル時刻」として固定解釈します。
        /// クライアントは必ず JST 変換済みの「日付込み」文字列を送信してください。
        ///   NG: "23:00"              （時刻のみ）
        ///   OK: "2026/03/14 23:00"  （日付 + 時刻）
        ///   OK: "2026-03-14 23:00"  （ハイフン区切りも可）
        /// </summary>
        private static bool TryParseAsJst(string value, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(value)) return false;

            if (!DateTime.TryParse(
                    value,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsed))
                return false;

            if (parsed == DateTime.MinValue) return false;

            // DateTimeKind.Unspecified に固定することで
            // .NET が「サーバーのローカルタイム」として自動変換するのを防ぐ
            result = DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified);
            return true;
        }

        /// <summary>
        /// 睡眠データを登録します。
        ///
        /// サーバーサイドで睡眠時間（分）を計算することで、
        /// クライアントの日またぎ計算ミスを防ぎます。
        ///
        /// ■ タイムゾーン仕様
        ///   クライアントは端末の設定に関係なく日本標準時（JST / UTC+9）に
        ///   変換した BedTime / WakeupTime を送信してください。
        ///   サーバーは受け取った値を JST として固定処理します。
        ///
        /// ■ データソース別クライアント変換責務
        ///   手動入力        : 変換不要（UIが日本語のため既に JST）
        ///   Apple HealthKit : startDate/endDate（端末ローカル）→ JST に変換して送信
        ///   Google Health   : startTime/endTime（UTC）→ +9h して JST に変換して送信
        /// </summary>
        public QoJotoAppSleepImportApiResults SleepImport(QoJotoAppSleepImportApiArgs args)
        {
            var result = new QoJotoAppSleepImportApiResults()
            {
                IsSuccess = bool.FalseString,
                Result    = null
            };

            // ── 1. 認証キーチェック ────────────────────────────────────
            var accountKey = args.ActorKey.TryToValueType<Guid>(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            var authorKey = args.AuthorKey.TryToValueType<Guid>(Guid.Empty);
            if (authorKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    "実行者のアカウントキーが指定されていません。");
                return result;
            }

            // ── 2. DataSource ログ ────────────────────────────────────
            var dataSource = string.IsNullOrWhiteSpace(args.DataSource) ? "Manual" : args.DataSource;
            QoAccessLog.WriteInfoLog($"SleepImport DataSource: {dataSource}");

            // ── 3. BedTime バリデーション＋パース（JST固定）──────────
            if (string.IsNullOrWhiteSpace(args.BedTime))
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    "BedTime が指定されていません。");
                return result;
            }

            // 時刻のみ("23:00")を拒否 ─ 日付が含まれていることを確認
            if (!ContainsDate(args.BedTime))
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"BedTime に日付が含まれていません。形式: yyyy/MM/dd HH:mm (JST)" +
                    $" 受信値: [{args.BedTime}]");
                return result;
            }

            if (!TryParseAsJst(args.BedTime, out DateTime bedTime))
            {
                QoAccessLog.WriteInfoLog($"SleepImport BedTime parse error: {args.BedTime}");
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"BedTime の形式が不正です。形式: yyyy/MM/dd HH:mm (JST)" +
                    $" 受信値: [{args.BedTime}]");
                return result;
            }

            // ── 4. WakeupTime バリデーション＋パース（JST固定）─────────
            if (string.IsNullOrWhiteSpace(args.WakeupTime))
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    "WakeupTime が指定されていません。");
                return result;
            }

            if (!ContainsDate(args.WakeupTime))
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"WakeupTime に日付が含まれていません。形式: yyyy/MM/dd HH:mm (JST)" +
                    $" 受信値: [{args.WakeupTime}]");
                return result;
            }

            if (!TryParseAsJst(args.WakeupTime, out DateTime wakeupTime))
            {
                QoAccessLog.WriteInfoLog($"SleepImport WakeupTime parse error: {args.WakeupTime}");
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"WakeupTime の形式が不正です。形式: yyyy/MM/dd HH:mm (JST)" +
                    $" 受信値: [{args.WakeupTime}]");
                return result;
            }

            // ── 5. 時刻順序チェック ────────────────────────────────────
            // 日またぎの場合: BedTime="2026/03/14 23:00", WakeupTime="2026/03/15 07:00"
            // → wakeupTime > bedTime が成立するため正常に通過する
            if (wakeupTime <= bedTime)
            {
                QoAccessLog.WriteInfoLog(
                    $"SleepImport time order error." +
                    $" BedTime:{bedTime:yyyy/MM/dd HH:mm}, WakeupTime:{wakeupTime:yyyy/MM/dd HH:mm}");
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"WakeupTime は BedTime より後の日時（JST）を指定してください。" +
                    $" BedTime:[{bedTime:yyyy/MM/dd HH:mm}]" +
                    $" WakeupTime:[{wakeupTime:yyyy/MM/dd HH:mm}]" +
                    " ※日またぎの場合は WakeupTime に翌日の日付を指定してください。");
                return result;
            }

            // ── 6. 睡眠時間（分）をサーバーサイドで計算 ──────────────
            // クライアントは計算しない。サーバーが BedTime〜WakeupTime の差分を算出。
            var sleepMinutes = (decimal)(wakeupTime - bedTime).TotalMinutes;

            if (sleepMinutes <= 0m || sleepMinutes > 1440m)
            {
                QoAccessLog.WriteInfoLog(
                    $"SleepImport sleepMinutes out of range: {sleepMinutes}min" +
                    $" BedTime:{bedTime:yyyy/MM/dd HH:mm} WakeupTime:{wakeupTime:yyyy/MM/dd HH:mm}");
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"睡眠時間が不正な値です（許容: 1〜1440分）。" +
                    $" BedTime:[{bedTime:yyyy/MM/dd HH:mm}]" +
                    $" WakeupTime:[{wakeupTime:yyyy/MM/dd HH:mm}]" +
                    $" 計算値: {sleepMinutes}分");
                return result;
            }

            // ── 7. SleepStages ログ（将来の詳細格納のため受信・記録のみ）──
            if (args.SleepStages != null && args.SleepStages.Count > 0)
            {
                QoAccessLog.WriteInfoLog(
                    $"SleepImport SleepStages received. count:{args.SleepStages.Count}" +
                    $" source:{dataSource}");
                foreach (var stage in args.SleepStages)
                {
                    QoAccessLog.WriteInfoLog(
                        $"  Stage: {stage.StageType}" +
                        $" {stage.StartTime} - {stage.EndTime}");
                }
            }

            // ── 8. DB 書き込み ─────────────────────────────────────────
            // PhrForHomeReader 設計に準拠:
            //   RECORDDATE = BedTime  (JST の就寝時刻) ※ Apple/Google 準拠
            //   VALUE1     = 睡眠時間（分、端数切り捨て）
            //   VALUE2     = -1 (未使用)
            try
            {
                var vitalItem = new DbVitalValueItem
                {
                    RecordDate    = bedTime,      // Apple/Google 準拠: RECORDDATE = BedTime（就寝時刻）
                    VitalType     = QsDbVitalTypeEnum.SleepingTime,
                    Value1        = decimal.Floor(sleepMinutes),
                    Value2        = decimal.MinusOne,
                    Value3        = decimal.MinusOne,
                    Value4        = decimal.MinusOne,
                    ConditionType = (byte)QsDbVitalConditionTypeEnum.None
                };

                QoAccessLog.WriteInfoLog(
                    $"SleepImport Writing(JST):" +
                    $" WakeupTime:{wakeupTime:yyyy/MM/dd HH:mm}" +
                    $" BedTime:{bedTime:yyyy/MM/dd HH:mm}" +
                    $" SleepMinutes:{(int)vitalItem.Value1}" +
                    $" DataSource:{dataSource}");

                _HealthRecordRepo.WriteVitals(
                    accountKey,
                    authorKey,
                    new System.Collections.Generic.List<DbVitalValueItem> { vitalItem });

                result.IsSuccess    = bool.TrueString;
                result.Result       = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                result.WakeupTime   = wakeupTime.ToString("yyyy/MM/dd HH:mm:ss");
                result.BedTime      = bedTime.ToString("yyyy/MM/dd HH:mm:ss");
                result.SleepMinutes = ((int)decimal.Floor(sleepMinutes)).ToString();
                return result;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.InternalServerError,
                    "睡眠データの登録に失敗しました。");
                return result;
            }
        }

        /// <summary>
        /// 文字列に日付部分（yyyy/MM/dd または yyyy-MM-dd）が含まれているか確認します。
        /// 時刻のみ（"23:00" 等）を弾くための補助メソッドです。
        /// </summary>
        private static bool ContainsDate(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(
                dateTimeString,
                @"\d{4}[/\-]\d{1,2}[/\-]\d{1,2}");
        }

        #endregion

        /// <summary>
        /// Calomeal の食事履歴を 1 アカウント分だけ同期します。
        /// </summary>
        public QoJotoAppCalomealMealSyncApiResults CalomealMealSync(QoJotoAppCalomealMealSyncApiArgs args)
        {
            var result = new QoJotoAppCalomealMealSyncApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null,
                ErrorMessageN = new List<string>()
            };

            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            if (_CalomealMealSyncRepo == null)
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.InternalServerError,
                    "CalomealMealSyncRepository が設定されていません。");
                return result;
            }

            var targetDateTime = CalomealMealSyncClient.GetCurrentJst();
            if (!string.IsNullOrWhiteSpace(args.TargetDateTime) &&
                !DateTime.TryParse(args.TargetDateTime, out targetDateTime))
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"TargetDateTime が不正です。[{args.TargetDateTime}]");
                return result;
            }

            double timeSpanInHours = 24d;
            if (!string.IsNullOrWhiteSpace(args.TimeSpanInHours) &&
                !double.TryParse(args.TimeSpanInHours, out timeSpanInHours))
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    $"TimeSpanInHours が不正です。[{args.TimeSpanInHours}]");
                return result;
            }

            if (timeSpanInHours <= 0)
            {
                result.Result = QoApiResult.Build(
                    QoApiResultCodeTypeEnum.ArgumentError,
                    "TimeSpanInHours は 0 より大きい値で指定してください。");
                return result;
            }

            if (timeSpanInHours > 24)
            {
                timeSpanInHours = 24;
            }

            try
            {
                var client = new CalomealMealSyncClient();
                var execution = client.SyncMealHistories(_CalomealMealSyncRepo, accountKey, targetDateTime, timeSpanInHours);

                result.TargetDateTime = execution.TargetDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                result.TimeSpanInHours = execution.TimeSpanInHours.ToString("0.##");
                result.ProcessedCount = execution.ProcessedCount.ToString();
                result.SuccessCount = execution.SuccessCount.ToString();
                result.ErrorCount = execution.ErrorCount.ToString();
                result.AddedCount = execution.AddedCount.ToString();
                result.ModifiedCount = execution.ModifiedCount.ToString();
                result.DeletedCount = execution.DeletedCount.ToString();
                result.TokenRefreshed = execution.TokenRefreshed ? bool.TrueString : bool.FalseString;
                result.Message = execution.Message;
                result.ErrorMessageN = execution.ErrorMessages ?? new List<string>();

                if (execution.ErrorCount == 0)
                {
                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success, execution.Message);
                }
                else
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, execution.Message);
                }

                return result;
            }
            catch (ConfigurationErrorsException ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, ex.Message);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, ex.Message);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }
        }

        /// <summary>
        /// PHR情報取得を行う。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoJotoAppPhrLatestReadApiResults PhrLatestRead(QoJotoAppPhrLatestReadApiArgs args)
        {
            QoJotoAppPhrLatestReadApiResults result = new QoJotoAppPhrLatestReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };
            // チェック
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "処理対象者のアカウントキーが指定されていません。");
                return result;
            }

            //if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            //{
            //    QoAccessLog.WriteInfoLog($"LinkageSystemNo Error");
            //    result.IsSuccess = bool.FalseString;
            //    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"LinkageSystemNo Error");
            //    return result;
            //}
            //var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            //if (systemType.ToString() != QsApiSystemTypeEnum.JotoGinowan.ToString())
            //{
            //    QoAccessLog.WriteInfoLog($"ExecuteSystemType Error");
            //    result.IsSuccess = bool.FalseString;
            //    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            //    return result;
            //}

            try
            {
                var data = new List<QoApiPhrValueItem>();
                switch (args.DataType)
                {
                    case "Vital":

                        foreach (var strType in args.VitalTypeN)
                        {

                            // VitalType パースは Vital case のみで実施
                            // Exercise / Mental では VitalType を使用しないため、switch の外でのパースは行わない
                            if (!Enum.TryParse<QvApiVitalTypeEnum>(strType, out QvApiVitalTypeEnum vitalType))
                            {
                                QoAccessLog.WriteInfoLog($"VitalType Error, VitalType:{strType}");
                                continue;
                            }
                            var vital = _HealthRecordRepo.ReadNew(accountKey, DateTime.Now, (byte)vitalType);
                            if (!(vital != null && vital.Count() == 1))
                            {
                                QoAccessLog.WriteInfoLog($"VitalData Error, VitalType:{strType}");
                                continue;
                            }
                            var bodyHeight = decimal.MinValue;
                            var latestValue = vital.First();
                            var bmi = string.Empty;
                            var calorie = string.Empty;
                            switch (vitalType)
                            {
                                case QvApiVitalTypeEnum.BodyWeight:
                                    //最新の身長取得
                                    var vitalHeight = _HealthRecordRepo.ReadNew(accountKey, DateTime.Now, (byte)QvApiVitalTypeEnum.BodyHeight);
                                    if (vitalHeight != null && vitalHeight.Count() == 1 && vitalHeight.First().VALUE1 > 0)
                                    {
                                        bodyHeight = vitalHeight.First().VALUE1;
                                    }

                                    //年齢と性別取得
                                    int age = int.MinValue;
                                    QsDbSexTypeEnum gender = QsDbSexTypeEnum.None;
                                    var index = JotoAppWorker.SelectAccountIndexEntity(accountKey);
                                    if (index != null)
                                    {
                                        age = DateTime.Now.Year - index.BIRTHDAY.Year;
                                        //誕生日がまだ来ていなければ、-1
                                        if (DateTime.Now.Month < index.BIRTHDAY.Month || DateTime.Now.Month == index.BIRTHDAY.Month && DateTime.Now.Day < index.BIRTHDAY.Day)
                                        {
                                            age--;
                                        }
                                        gender = (QsDbSexTypeEnum)index.SEXTYPE;
                                    }

                                    //BMI計算
                                    if(bodyHeight != decimal.MinValue)
                                    {
                                        bmi = Math.Round(latestValue.VALUE1 / ((bodyHeight / 100m) * (bodyHeight / 100m)), 1).ToString();
                                    }
                                    //基礎代謝計算
                                    if (bodyHeight != decimal.MinValue && age != int.MinValue)
                                    {
                                        switch(gender)
                                        {
                                            //男性： 13.397×体重kg＋4.799×身長cm−5.677×年齢+88.362
                                            case QsDbSexTypeEnum.Male:
                                                calorie = (((decimal)13.397 * latestValue.VALUE1) + ((decimal)4.799 * bodyHeight) - ((decimal)5.677 * (decimal)age) + (decimal)88.362).ToString();
                                                break;
                                            //女性： 9.247×体重kg＋3.098×身長cm−4.33×年齢 + 447.593
                                            case QsDbSexTypeEnum.Female:
                                                calorie = (((decimal)9.247 * latestValue.VALUE1) + ((decimal)3.098 * bodyHeight) - ((decimal)4.33 * (decimal)age) + (decimal)447.593).ToString();
                                                break;
                                            default:
                                                break;
                                                
                                        }


                                    }
                                    break;
                                default:
                                    break;
                            }

                            data.Add(new QoApiPhrValueItem()
                            {
                                RecordDate = latestValue.RECORDDATE.ToApiDateString(),
                                ItemType = latestValue.VITALTYPE.ToString(),
                                Value1 = latestValue.VALUE1.ToString(),
                                Value2 = bodyHeight == decimal.MinValue ?
                                   latestValue.VALUE2.ToString() : bodyHeight.ToString(),
                                Value3 = bmi,//BMI
                                Value4 =calorie,//基礎代謝
                                ConditionType = latestValue.CONDITIONTYPE.ToString(),
                            });

                            result.Data = data;
                        }
                        break;
                    case "Exercise":
                    case "Mental":
                    default:
                        result.IsSuccess = bool.FalseString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"DataType Error {args.DataType}");
                        return result;
                }

                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return result;

            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }

        }

        #endregion
    }
}
