
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsOpenApi.Repositories;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// JOTOネイティブアプリに関する機能を提供するAPIコントローラです。
    /// </summary>
    public class JotoAppController : QoApiControllerBase
    {
        /// <summary>
        /// Joto ネイティブアプリのホーム画面表示情報を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QoApiTypeEnum.JotoAppPhrReadForHomeRead)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("PhrReadForHome")]
        public QoJotoAppPhrReadForHomeReadApiResults PostPhrReadForHomeRead([FromBody] QoJotoAppPhrReadForHomeReadApiArgs args)
        {
            var worker = new JotoAppWorker(new PhrForHomeRepository());
            return this.ExecuteWorkerMethod(args, worker.PhrReadForHomeRead);
        }

        /// <summary>
        /// メンタル情報を書き込みます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("MentalImport")]
        public QoJotoAppMentalImportApiResults PostMentalImport(QoJotoAppMentalImportApiArgs args)
        {
            var worker = new JotoAppWorker(new EventRepository());
            return ExecuteWorkerMethod(args, worker.MentalImport);
        }


        /// <summary>
        /// PHR情報を削除します。
        /// DataType="Vital"  : HealthRecordRepository（バイタル削除）＋ JotoAppRepository（CalorieBurn削除）の両方が必要
        /// DataType="Exercise": JotoAppRepository（QH_EXERCISEEVENT2_DAT, LINKAGESYSTEMNO=47003 で削除）
        /// DataType="Mental"  : JotoAppRepository（カレンダーイベント削除）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("PhrDelete")]
        public QoJotoAppPhrDeleteApiResults PostPhrDelete(QoJotoAppPhrDeleteApiArgs args)
        {
            // 取得種別チェック
            if (!(args != null && args.DataType != null))
            {
                var result = new QoJotoAppPhrDeleteApiResults();
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            JotoAppWorker worker;

            switch (args.DataType)
            {
                case "Vital":
                    // VitalType=CalorieBurn の場合は _JotoAppRepo.DeleteExercise() を使用するため
                    // HealthRecordRepository と JotoAppRepository の両方を渡す
                    worker = new JotoAppWorker(new HealthRecordRepository(), new JotoAppRepository());
                    break;
                case "Exercise":
                case "Mental":
                    worker = new JotoAppWorker(new JotoAppRepository());
                    break;
                default:
                    var result = new QoJotoAppPhrDeleteApiResults();
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                    return result;
            }
            return ExecuteWorkerMethod(args, worker.PhrDelete);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けPHR情報を取得します。
        /// DataType="Vital"  : HealthRecordRepository（QH_HEALTHRECORD_DAT から取得）
        /// DataType="Exercise": JotoAppRepository（QH_EXERCISEEVENT2_DAT, LINKAGESYSTEMNO=47003 から取得）
        /// DataType="Mental"  : JotoAppRepository（カレンダーイベントから取得）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("PhrRead")]
        public QoJotoAppPhrReadApiResults PostPhrRead(QoJotoAppPhrReadApiArgs args)
        {
            // 取得種別チェック
            if (!(args != null && args.DataType != null))
            {
                var result = new QoJotoAppPhrReadApiResults();
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            JotoAppWorker worker;

            switch (args.DataType)
            {
                case "Vital":
                    // QH_HEALTHRECORD_DAT から取得（BodyWeight は BMI 計算のため身長も取得）
                    // ※ VitalType=SleepingTime も同テーブルのため Vital ケースで取得可能
                    worker = new JotoAppWorker(new HealthRecordRepository());
                    break;
                case "Exercise":
                case "Mental":
                    worker = new JotoAppWorker(new JotoAppRepository());
                    break;
                default:
                    var result = new QoJotoAppPhrReadApiResults();
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                    return result;
            }
            return ExecuteWorkerMethod(args, worker.PhrRead);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けお知らせ（グループ）情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("NoticeGroupRead")]
        public QoJotoAppNoticeGroupReadApiResults PostNoticeGroupRead([FromBody] QoJotoAppNoticeGroupReadApiArgs args)
        {
            var worker = new JotoAppWorker(new NoticeGroupRepository());
            return this.ExecuteWorkerMethod(args, worker.NoticeGroupRead);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けお知らせ（グループ）詳細情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("NoticeGroupDetailRead")]
        public QoJotoAppNoticeGroupDetailReadApiResults PostNoticeGroupDetailRead([FromBody] QoJotoAppNoticeGroupDetailReadApiArgs args)
        {
            var worker = new JotoAppWorker(new NoticeGroupRepository());
            return this.ExecuteWorkerMethod(args, worker.NoticeGroupDetailRead);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けお知らせ（グループ）既読状態を更新します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("NoticeGroupTargetWrite")]
        public QoJotoAppNoticeGroupTargetWriteApiResults PostNoticeGroupTargetWrite([FromBody] QoJotoAppNoticeGroupTargetWriteApiArgs args)
        {
            var worker = new JotoAppWorker(new NoticeGroupRepository());
            return this.ExecuteWorkerMethod(args, worker.NoticeGroupTargetWrite);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けお知らせ（グループ）画像を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("NoticeGroupImageRead")]
        public QoJotoAppNoticeGroupImageReadApiResults PostNoticeGroupImageRead([FromBody] QoJotoAppNoticeGroupImageReadApiArgs args)
        {
            var worker = new JotoAppWorker(new NoticeGroupRepository());
            return this.ExecuteWorkerMethod(args, worker.NoticeGroupImageRead);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けアプリイベント一覧を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("AppEventRead")]
        public QoJotoAppAppEventReadApiResults PostAppEventRead([FromBody] QoJotoAppAppEventReadApiArgs args)
        {
            var worker = new JotoAppWorker(new AppEventRepository());
            return this.ExecuteWorkerMethod(args, worker.AppEventRead);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けアプリイベント詳細を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("AppEventDetailRead")]
        public QoJotoAppAppEventDetailReadApiResults PostAppEventDetailRead([FromBody] QoJotoAppAppEventDetailReadApiArgs args)
        {
            var worker = new JotoAppWorker(new AppEventRepository());
            return this.ExecuteWorkerMethod(args, worker.AppEventDetailRead);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けアプリイベント画像を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("AppEventImageRead")]
        public QoJotoAppAppEventImageReadApiResults PostImageRead(QoJotoAppAppEventImageReadApiArgs args)
        {
            var worker = new JotoAppWorker(new AppEventRepository());
            return this.ExecuteWorkerMethod(args, worker.AppEventImageRead);
        }

        /// <summary>
        /// 運動マスタ一覧を取得します（LINKAGESYSTEMNO=47003 固定）。
        /// </summary>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("ExerciseItemList")]
        public QoJotoAppExerciseItemListApiResults PostExerciseItemList(
            [FromBody] QoJotoAppExerciseItemListApiArgs args)
        {
            var worker = new JotoAppWorker(new JotoAppRepository());
            return this.ExecuteWorkerMethod(args, worker.ExerciseItemList);
        }

        /// <summary>
        /// 運動データを登録します（新方式: LINKAGESYSTEMNO=47003）。
        /// </summary>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("ExerciseImport")]
        public QoJotoAppExerciseImportApiResults PostExerciseImport(
            [FromBody] QoJotoAppExerciseImportApiArgs args)
        {
            var worker = new JotoAppWorker(new JotoAppRepository());
            return this.ExecuteWorkerMethod(args, worker.ExerciseImport);
        }


        /// <summary>
        /// 睡眠データを登録します。
        /// クライアントは必ず JST(UTC+9) に変換して BedTime / WakeupTime を送信してください。
        /// 睡眠データの更新は「PhrDelete(DataType=Vital, VitalType=SleepingTime) → SleepImport」の順で行ってください。
        /// 睡眠データの取得は「PhrRead(DataType=Vital, VitalType=SleepingTime)」で行えます。
        /// </summary>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("SleepImport")]
        public QoJotoAppSleepImportApiResults PostSleepImport(
            [FromBody] QoJotoAppSleepImportApiArgs args)
        {
            var worker = new JotoAppWorker(new HealthRecordRepository());
            return this.ExecuteWorkerMethod(args, worker.SleepImport);
        }

        /// <summary>
        /// Calomeal の食事履歴を 1 アカウント分だけ同期します。
        /// </summary>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("CalomealMealSync")]
        public QoJotoAppCalomealMealSyncApiResults PostCalomealMealSync(
            [FromBody] QoJotoAppCalomealMealSyncApiArgs args)
        {
            var worker = new JotoAppWorker(new CalomealMealSyncRepository());
            return this.ExecuteWorkerMethod(args, worker.CalomealMealSync);
        }

        /// <summary>
        /// JOTOネイティブアプリ向けPHR最新情報を取得します。
        /// DataType="Vital"  : HealthRecordRepository（QH_HEALTHRECORD_DAT から取得）
        /// DataType="Exercise": JotoAppRepository（QH_EXERCISEEVENT2_DAT, LINKAGESYSTEMNO=47003 から取得）
        /// DataType="Mental"  : JotoAppRepository（カレンダーイベントから取得）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("PhrLatestRead")]
        public QoJotoAppPhrLatestReadApiResults PostPhrLatestRead(QoJotoAppPhrLatestReadApiArgs args)
        {
            // 取得種別チェック
            if (!(args != null && args.DataType != null))
            {
                var result = new QoJotoAppPhrLatestReadApiResults();
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            JotoAppWorker worker;

            switch (args.DataType)
            {
                case "Vital":
                    // QH_HEALTHRECORD_DAT から取得（BodyWeight は BMI 計算のため身長も取得）
                    // ※ VitalType=SleepingTime も同テーブルのため Vital ケースで取得可能
                    worker = new JotoAppWorker(new HealthRecordRepository());
                    break;
                case "Exercise":
                case "Mental":
                default:
                    var result = new QoJotoAppPhrLatestReadApiResults();
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                    return result;
            }
            return ExecuteWorkerMethod(args, worker.PhrLatestRead);
        }

        /// <summary>
        /// ホーム画面のバナー画像情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("BannerImageRead")]
        public QoJotoAppBannerImageReadApiResults PostBannerImageRead([FromBody] QoJotoAppBannerImageReadApiArgs args)
        {
            var result = new QoJotoAppBannerImageReadApiResults
            {
                IsSuccess = bool.TrueString,
                Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success),
                BannerNum = "0",
                BannerURLs = new List<string>(),
                Images = new List<QoApiFileItem>()
            };
            return result;
        }

        /// <summary>
        /// アカウント移行を実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("AccoutMigration")]
        public QoJotoAppAccoutMigrationApiResults PostAccoutMigration([FromBody] QoJotoAppAccoutMigrationApiArgs args)
        {

            // モックです。正常系リターンのみ行います。

            var result = new QoJotoAppAccoutMigrationApiResults();
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return result;
        }

        /// <summary>
        /// JOTOポイントを付与します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("JotoPointGive")]
        public QoJotoAppJotoPointGiveApiResults PostJotoPointGive([FromBody] QoJotoAppJotoPointGiveApiArgs args)
        {

            // モックです。正常系リターンのみ行います。

            var result = new QoJotoAppJotoPointGiveApiResults();
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return result;
        }

            /// <summary>
            /// システム設定を取得します。
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
            [HttpGet]
            [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoApp)]
            [ActionName("SystemConfigRead")]
            public QoJotoAppSystemConfigReadApiResults GetSystemConfigRead([FromUri] QoJotoAppSystemConfigReadApiArgs args)
            {

                // モックです。正常系リターンのみ行います。

                var result = new QoJotoAppSystemConfigReadApiResults();
                result.IsSuccess = bool.TrueString;
                result.Result = new QoApiResultItem()
                {
                    Code = "200",
                    Detail = "正常に終了しました。"
                };
                result.Config = new QoApiJotoAppSystemConfigItem
                {
                    CheckupReserveBtnDispFlag = "0",
                    CheckupReserveSsoURLDomain = "deveyetell.qolms.com",
                    WeatherURL = "https://pkg.navitime.co.jp/joto/course/detail/weather?joto_id={{joto_id}}&lat={{lat}}&lon={{lon}}&coord={{coord}}"
                };
                return result;
            }
    }
}
