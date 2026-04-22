using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsReproApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// JOTO 関連の公開API
    /// </summary>
    public class JotoHdrWorker
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static readonly Guid ViewerAccountKey = Guid.Parse("3ec7272b-0e81-4ced-a0dc-cba7044dc1c5");
        private static readonly Guid ViewerFacilitykey = Guid.Parse("cd36b7ad-66ea-4747-8c69-bd6f78394cbe");

        /*
        private static readonly Regex REGEX_OPEN_ID = new Regex("^https://.+/*$", RegexOptions.IgnoreCase);
        private static readonly string LINKAGE_SYSTEM_NO = "47004";
        private static readonly string LINKAGE_SYSTEM_NO_AIT = "47007";
        private static readonly string LINKAGE_SYSTEM_NO_NAVITIME = "47006";  // ALKOO

        private static readonly int EXPIRED_DAYS = 30;
        private static readonly string URI_AUSYSTEMID_PREFIX = "https://connect.auone.jp/net/id/hny_rt_net/cca/a/";



        private static QoApiResultItem PointWriteArgsCheck(QoJotoHdrPointWriteApiArgs args)
        {

            // 連携先番号のチェック決め打ち
            if (args.LinkageSystemNo.Trim().CompareTo(JotoHdrWorker.LINKAGE_SYSTEM_NO_NAVITIME) != 0)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です。");

            if (string.IsNullOrEmpty(args.LinkageId))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageIdが不正です。");

            // If String.IsNullOrEmpty(args.ActionServiceNo) OrElse args.ActionServiceNo.Trim().CompareTo(JotoHdrWorker.LINKAGE_SYSTEM_NO_NAVITIME) <> 0 Then
            // Return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "ActionServiceNoが不正です。")
            // End If

            if (args.ActionPoint.TryToValueType(0) == 0)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "ActionPointが不正です。");

            if (args.ActionDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "ActionDateが不正です。");

            return null;
        }

        //引数チェック
        private static QoApiResultItem ChallengeEntryWriteArgsCheck(QoJotoHdrChallengeEntryWriteApiArgs args)
        {

            // 連携先番号のチェック決め打ち
            if (args.LinkageSystemNo.Trim().CompareTo(JotoHdrWorker.LINKAGE_SYSTEM_NO_NAVITIME) != 0)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です。");

            if (string.IsNullOrEmpty(args.LinkageId))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageIdが不正です。");


            if (args.EntryEventKey.TryToValueType(Guid.Empty) == Guid.Empty)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "EntryEventKeyが不正です。");


            if (string.IsNullOrEmpty(args.EntryCode))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "EntryCodeが不正です。");

            if (args.StartDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "StartDateが不正です。");
            if (args.EndDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "EndtDateが不正です。");

            return null;
        }

        public static QoJotoHdrMemberStatusApiResults MemberStatus(QoJotoHdrMemberStatusApiArgs args)
        {
            QoJotoHdrMemberStatusApiResults result = new QoJotoHdrMemberStatusApiResults() { IsSuccess = bool.FalseString };

            QoApiJotoHdrMemberStatusTypeEnum statusNo = QoApiJotoHdrMemberStatusTypeEnum.None;
            string errorMessage = string.Empty;

            DateTime now = DateTime.Now;
            DateTime d = DateTime.MinValue;

            // // チェック、判定ここから //

            // 1.判定日のチェック
            if (!string.IsNullOrEmpty(args.TargetDate) && DateTime.TryParseExact(args.TargetDate + string.Format("{0:HHmmss}", now), "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, d))
            {
                if (d < now.AddDays(-JotoHdrWorker.EXPIRED_DAYS))
                {
                    statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                    errorMessage = string.Format("TargetDateは{0}日以内で指定してください。", JotoHdrWorker.EXPIRED_DAYS);
                }
                else if (now < d)
                {
                    statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                    errorMessage = "未来のTargetDateは指定できません。";
                }
            }
            else
            {
                statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                errorMessage = "TargetDateが不正です。";
            }

            // 2.連携先番号のチェック
            if (statusNo != QoApiJotoHdrMemberStatusTypeEnum.Error && args.LinkageSystemNo.Trim().CompareTo(JotoHdrWorker.LINKAGE_SYSTEM_NO) != 0)
            {
                statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                errorMessage = "LinkageSystemNoが不正です。";
            }

            // 3.オープンIDのチェック
            if (statusNo != QoApiJotoHdrMemberStatusTypeEnum.Error)
            {
                if (JotoHdrWorker.REGEX_OPEN_ID.IsMatch(args.OpenId))
                {

                    // APES問い合わせ、会員ステータスの判定
                    string auId = "";
                    int status = 9;
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings("AuOwlUri")))
                        status = AuApesAccessWorker.IsMobileSubscriberOfAu(args.OpenId, args.TargetDate, auId);
                    else
                        status = AuOwlAccessWorker.IsMobileSubscriberOfAu(args.OpenId, args.TargetDate, auId);
                    switch (status)
                    {
                        case 1:
                            {
                                // 非契約者
                                statusNo = QoApiJotoHdrMemberStatusTypeEnum.NonMembership;
                                break;
                            }

                        case 2:
                            {
                                // 契約者
                                // Linkage作成・取得（非会員、エラー時は取れない）
                                string uriAuId = URI_AUSYSTEMID_PREFIX + auId;
                                LinkageUserMemberShipReader reader = new LinkageUserMemberShipReader();
                                LinkageUserMemberShipReaderArgs readerArgs = new LinkageUserMemberShipReaderArgs() { AuSystemId = uriAuId, Executor = args.Executor.TryToValueType(Guid.Empty), LinkageSystemNo = LINKAGE_SYSTEM_NO.TryToValueType(int.MinValue), TargetDate = d };
                                LinkageUserMemberShipReaderResults readerResult = QsDbManager.Read(reader, readerArgs);
                                if (readerResult.IsSuccess)
                                {
                                    result.LinkageID = readerResult.LinkageId;
                                    if (string.IsNullOrEmpty(result.LinkageID))
                                    {
                                        // Linkageができてないときは新しいIDを作ってLinkage作成
                                        string linkageId = Guid.NewGuid().ToString("N");
                                        LinkageRegisterWriter writer = new LinkageRegisterWriter();
                                        LinkageRegisterWriterArgs writerArgs = new LinkageRegisterWriterArgs()
                                        {
                                            AccountKey = readerResult.AccountKey,
                                            Executor = readerArgs.Executor,
                                            LinkageNo = readerArgs.LinkageSystemNo,
                                            LinkCardID = linkageId
                                        };
                                        LinkageRegisterWriterResults writerResult = QsDbManager.Write(writer, writerArgs);
                                        if (writerResult.IsSuccess)
                                            result.LinkageID = linkageId;
                                    }
                                    switch (readerResult.MemberShipType)
                                    {
                                        case object _ when QsDbMemberShipTypeEnum.Free:
                                            {
                                                statusNo = QoApiJotoHdrMemberStatusTypeEnum.FreeMembership;
                                                break;
                                            }

                                        case object _ when QsDbMemberShipTypeEnum.LimitedTime:
                                        case object _ when QsDbMemberShipTypeEnum.Premium:
                                        case object _ when QsDbMemberShipTypeEnum.Business:
                                            {
                                                statusNo = QoApiJotoHdrMemberStatusTypeEnum.PremiumMembership;
                                                break;
                                            }
                                    }
                                }
                                else
                                    // こちらのDBに存在しないAu契約者
                                    statusNo = QoApiJotoHdrMemberStatusTypeEnum.NonMembership;
                                break;
                            }

                        case 9:
                            {
                                // エラー
                                statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                                break;
                            }
                    }
                }
                else
                {
                    statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                    errorMessage = "OpenIdが不正です。";
                }
            }

            // // チェック、判定ここまで //

            // この時点でNoneのままだと判定漏れがある
            if (statusNo == QoApiJotoHdrMemberStatusTypeEnum.None)
            {
                statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                errorMessage = "不明なエラーです。";
            }



            {
                var withBlock = result;
                withBlock.IsSuccess = bool.TrueString;
                // .LinkageID = String.Empty
                withBlock.StatusNo = Convert.ToByte(statusNo).ToString();
                withBlock.ErrorMessage = errorMessage;
            }

            try
            {
                string log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "testlog.txt");
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", now, MakeLogData(args, result), Constants.vbCrLf));
            }
            catch (Exception ex)
            {
            }
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, result)
    );
            return result;
        }

        [Obsolete("AIトレーニング用ですが、あちらのリリースが保留になりました")]
        public static QoJotoHdrMemberShipApiResults MemberShip(QoJotoHdrMemberShipApiArgs args)
        {
            QoJotoHdrMemberShipApiResults result = new QoJotoHdrMemberShipApiResults() { IsSuccess = bool.FalseString };

            QoApiJotoHdrMemberStatusTypeEnum statusNo = QoApiJotoHdrMemberStatusTypeEnum.None;
            string errorMessage = string.Empty;

            DateTime now = DateTime.Now;
            DateTime d = DateTime.MinValue;

            // // チェック、判定ここから //

            // 連携先番号のチェック(AIトレーニング47007決め打ち
            if (args.LinkageSystemNo.Trim().CompareTo(JotoHdrWorker.LINKAGE_SYSTEM_NO_AIT) != 0)
            {
                statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                errorMessage = "LinkageSystemNoが不正です。";
            }

            if (string.IsNullOrEmpty(args.ActorKey))
            {
                statusNo = QoApiJotoHdrMemberStatusTypeEnum.Error;
                errorMessage = "対象者を特定できません。";
            }

            if (statusNo != QoApiJotoHdrMemberStatusTypeEnum.Error)
            {
                // 会員ステータスの判定
                // Linkage作成・取得（非会員、エラー時は取れない）
                LinkageUserMemberShipReader reader = new LinkageUserMemberShipReader();
                LinkageUserMemberShipReaderArgs readerArgs = new LinkageUserMemberShipReaderArgs()
                {
                    AccountKey = args.ActorKey.TryToValueType(Guid.Empty),
                    Executor = args.Executor.TryToValueType(Guid.Empty),
                    LinkageSystemNo = LINKAGE_SYSTEM_NO.TryToValueType(int.MinValue),
                    TargetDate = DateTime.Now
                };
                LinkageUserMemberShipReaderResults readerResult = QsDbManager.Read(reader, readerArgs);
                if (readerResult.IsSuccess)
                {
                    result.LinkageID = readerResult.LinkageId;
                    if (string.IsNullOrEmpty(result.LinkageID))
                    {
                        // Linkageができてないときは新しいIDを作ってLinkage作成
                        string linkageId = Guid.NewGuid().ToString("N");
                        LinkageRegisterWriter writer = new LinkageRegisterWriter();
                        LinkageRegisterWriterArgs writerArgs = new LinkageRegisterWriterArgs()
                        {
                            AccountKey = readerResult.AccountKey,
                            Executor = readerArgs.Executor,
                            LinkageNo = readerArgs.LinkageSystemNo,
                            LinkCardID = linkageId
                        };
                        LinkageRegisterWriterResults writerResult = QsDbManager.Write(writer, writerArgs);
                        if (writerResult.IsSuccess)
                            result.LinkageID = linkageId;
                    }
                    switch (readerResult.MemberShipType)
                    {
                        case object _ when QsDbMemberShipTypeEnum.Free:
                            {
                                statusNo = QoApiJotoHdrMemberStatusTypeEnum.FreeMembership;
                                break;
                            }

                        case object _ when QsDbMemberShipTypeEnum.LimitedTime:
                        case object _ when QsDbMemberShipTypeEnum.Premium:
                        case object _ when QsDbMemberShipTypeEnum.Business:
                            {
                                statusNo = QoApiJotoHdrMemberStatusTypeEnum.PremiumMembership;
                                break;
                            }
                    }
                }
                else
                    // こちらのDBに存在しない(ほとんどありえない）
                    statusNo = QoApiJotoHdrMemberStatusTypeEnum.NonMembership;
            }
            // // チェック、判定ここまで //

            // この時点でNoneのままだと判定漏れがある
            if (statusNo == QoApiJotoHdrMemberStatusTypeEnum.None)
            {
                result.StatusNo = Convert.ToByte(QoApiJotoHdrMemberStatusTypeEnum.Error).ToString();
                result.ErrorMessage = "不明なエラーです。";
            }
            else
            {
                var withBlock = result;
                withBlock.IsSuccess = bool.TrueString;
                withBlock.Result = new QoApiResultItem() { Code = "0200", Detail = "正常に終了しました。" };
                // .LinkageID = String.Empty
                withBlock.StatusNo = Convert.ToByte(statusNo).ToString();
                withBlock.ErrorMessage = errorMessage;
            }




            try
            {
                string log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "testlog.txt");
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", now, MakeLogdata(args, result), Constants.vbCrLf));
            }
            catch (Exception ex)
            {
            }
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogdata(args, result)
    );
            return result;
        }

        /// <summary>
        /// アクセスキー取得を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [Obsolete("AIトレーニング用ですが、あちらのリリースが保留になりました")]
        public static QoJotoHdrAccessKeyGenerateApiResults AccessKeyGenerate(QoJotoHdrAccessKeyGenerateApiArgs args)
        {
            QoJotoHdrAccessKeyGenerateApiResults result = new QoJotoHdrAccessKeyGenerateApiResults() { IsSuccess = bool.FalseString };
            Guid accountKey = Guid.Empty;
            DebugLog("AccessKeyGenerate");
            // auID Tokenでの認証、アカウントキーを特定する
            string auId = AuMagiAccessWorker.GetAuSystemId(args.AuIdToken);
            if (string.IsNullOrEmpty(auId) == false)
            {
                string uriAuId = URI_AUSYSTEMID_PREFIX + auId;
                DebugLog(uriAuId);
                LinkageUserMemberShipReader reader = new LinkageUserMemberShipReader();
                LinkageUserMemberShipReaderArgs readerArgs = new LinkageUserMemberShipReaderArgs() { AuSystemId = uriAuId, Executor = args.Executor.TryToValueType(Guid.Empty), LinkageSystemNo = LINKAGE_SYSTEM_NO.TryToValueType(int.MinValue), TargetDate = DateTime.Now };
                LinkageUserMemberShipReaderResults readerResult = QsDbManager.Read(reader, readerArgs);
                if (readerResult.IsSuccess)
                {
                    DebugLog(readerResult.AccountKey.ToString());
                    accountKey = readerResult.AccountKey;
                }
            }
            else
            {
            }




            if (accountKey != Guid.Empty)
            {
                // アクセスキー生成（有効期限1時間、ExerciseEventとJotoHdrにアクセス権あり）
                result.AccessKey = JwtAuthenticateWorker.CreateAccessKey(QoApiAuthorizeTypeEnum.JwtAccessKey, accountKey, QoApiFunctionTypeEnum.JotoHdrAppUser, DateTime.UtcNow.AddHours(1), Guid.Empty
    );
                DebugLog(result.AccessKey);
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            else
            {
            }

            return result;
        }

        /// <summary>
        /// JOTO ALKOOからのポイント付与対応
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrPointWriteApiResults PointWrite(QoJotoHdrPointWriteApiArgs args)
        {
            QoJotoHdrPointWriteApiResults results = new QoJotoHdrPointWriteApiResults() { ExecutedDate = DateTime.Now.ToApiDateString, GrantedPoint = "0", IsSuccess = bool.FalseString, Result = null, SerialNo = args.SerialNo };
            QoApiJotoHdrMemberStatusTypeEnum statusNo = QoApiJotoHdrMemberStatusTypeEnum.None;
            string errorMessage = string.Empty;

            // // チェック
            results.Result = PointWriteArgsCheck(args);
            if (results.Result != null)
                return results;


            try
            {
                Guid accountKey = Guid.Empty;
                bool isPremium = false;
                int actionPoint = args.ActionPoint.TryToValueType(0);
                // 会員ステータスの判定
                // Linkageからアカウントキーへ
                LinkageUserMemberShipReader reader = new LinkageUserMemberShipReader();
                LinkageUserMemberShipReaderArgs readerArgs = new LinkageUserMemberShipReaderArgs()
                {
                    LinkageId = args.LinkageId,
                    Executor = args.Executor.TryToValueType(Guid.Empty),
                    LinkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue),
                    TargetDate = args.ActionDate.TryToValueType(DateTime.Now)
                };
                LinkageUserMemberShipReaderResults readerResult = QsDbManager.Read(reader, readerArgs);
                if (readerResult.IsSuccess)
                {
                    accountKey = readerResult.AccountKey;
                    switch (readerResult.MemberShipType)
                    {
                        case object _ when QsDbMemberShipTypeEnum.Free:
                            {
                                isPremium = false;
                                break;
                            }

                        case object _ when QsDbMemberShipTypeEnum.LimitedTime:
                        case object _ when QsDbMemberShipTypeEnum.Premium:
                        case object _ when QsDbMemberShipTypeEnum.Business:
                            {
                                isPremium = true;
                                break;
                            }
                    }
                }
                else
                {
                    // こちらのDBに存在しない(ほとんどありえない）
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                    return results;
                }

                // ポイント付与
                QoApiQolmsPointGrantActionItem actionItem = new QoApiQolmsPointGrantActionItem() { AccountKey = accountKey.ToApiGuidString(), ActionDate = args.ActionDate, ActionPoint = actionPoint.ToString(), ActionReason = args.ActionReason, PointExpirationDate = args.PointExpirationDate, PointItemNo = args.PointItemNo, PointTargetDate = args.PointTargetDate, SerialNo = args.SerialNo, UpdateFlag = args.UpdateFlag };
                QoQolmsPointWriteApiResults pointWriteResult = QolmsPointWorker.Write(new QoQolmsPointWriteApiArgs() { ActionDataList = new List<QoApiQolmsPointGrantActionItem>() { actionItem }, ActionServiceNo = args.ActionServiceNo, ActorKey = args.ActorKey, ApiType = args.ApiType, ExecuteSystemType = args.ExecuteSystemType, Executor = args.Executor, ExecutorName = args.ExecutorName });
                if (pointWriteResult.IsSuccess.TryToValueType(false) && pointWriteResult.ResultList != null && pointWriteResult.ResultList.FirstOrDefault != null)
                {
                    {
                        var withBlock = pointWriteResult.ResultList.FirstOrDefault;
                        results.ExecutedDate = withBlock.ExecutedDate;
                        results.GrantedPoint = withBlock.GrantedPoint;
                        results.IsSuccess = withBlock.GrantSuccess;
                        if (withBlock.ErrorCode.TryToValueType(0) > 0)
                            results.Result = QoApiResult.Build(withBlock.ErrorCode.TryToValueType(0), "PointWriteに失敗しました。");
                        else
                            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                        results.SerialNo = withBlock.SerialNo;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex);
            }

            AccessLogWorker.WriteAccessLog(null/, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results)
    );
            return results;
        }

        /// <summary>
        /// JOTO ALKOOからのイベント登録情報登録
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrChallengeEntryWriteApiResults ChallengeEntryWrite(QoJotoHdrChallengeEntryWriteApiArgs args)
        {
            QoJotoHdrChallengeEntryWriteApiResults results = new QoJotoHdrChallengeEntryWriteApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };
            // チェック
            results.Result = ChallengeEntryWriteArgsCheck(args);
            if (results.Result == null)
            {
                try
                {
                    Guid accountKey = Guid.Empty;
                    // Linkageからアカウントキーへ
                    LinkageUserMemberShipReader reader = new LinkageUserMemberShipReader();
                    LinkageUserMemberShipReaderArgs readerArgs = new LinkageUserMemberShipReaderArgs()
                    {
                        LinkageId = args.LinkageId,
                        Executor = args.Executor.TryToValueType(Guid.Empty),
                        LinkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue),
                        TargetDate = args.StartDate.TryToValueType(DateTime.Now)
                    };
                    LinkageUserMemberShipReaderResults readerResult = QsDbManager.Read(reader, readerArgs);
                    if (readerResult.IsSuccess)
                        accountKey = readerResult.AccountKey;
                    else
                    {
                        // こちらのDBに存在しない(ほとんどありえない）
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                        return results;
                    }

                    ChallengeEntryWriterArgs WriterArgs = new ChallengeEntryWriterArgs() { ActorKey = accountKey, AuthorKey = args.AuthorKey.TryToValueType(Guid.Empty), Challengekey = args.EntryEventKey.TryToValueType(Guid.Empty), PassCode = args.EntryCode, EntryDate = args.StartDate.TryToValueType(DateTime.MinValue), EndDate = args.EndDate.TryToValueType(DateTime.MinValue) };

                    ChallengeEntryWriterResults WriterResult = QsDbManager.Write(new ChallengeEntryWriter(), WriterArgs);

                    if (WriterResult.IsSuccess)
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    else
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "登録に失敗しました。");
                }
                catch (Exception ex)
                {
                    results.Result = QoApiResult.Build(ex);
                }
            }
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results)
    );
            return results;
        }
        */

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

        private static List<QoApiHeartRateWarningTargetListItem> GetAlertList(long alertNo,DateTime startDate, DateTime endDate, 
            string executor)
        {
            var results = new List<QoApiHeartRateWarningTargetListItem>();

            var readrResults = QsDbManager.Read(new HealthRecordAlertReader(), new HealthRecordAlertReaderArgs()
            {
                AlertNo = alertNo,
                LinkageSystemNo = QoLinkage.JOTO_LINKAGE_SYSTEM_NO_FITBIT,
                TargetStartRecordDate = startDate,
                TargetEndRecordDate = endDate,
                VitalType = (byte)QsDbVitalTypeEnum.Pulse
            });
            if (readrResults.IsSuccess && readrResults.AlertList!=null )
            {
                foreach (var item in readrResults.AlertList )
                {
                    // アラート 番号を指定している時のみ QolmsViewer 用の SSO トークン を セット（常に トークン を生成すると重いので）
                    string viewerToken = string.Empty;

                    if (alertNo > 0)
                    {
                        viewerToken = new QsJwtTokenProvider().CreateViewerJwtSsoKey(
                            executor,
                            ViewerAccountKey,
                            ViewerFacilitykey,
                            item.AccountKey,
                            QoLinkage.JOTO_LINKAGE_SYSTEM_NO,
                            (int)QsApiSystemTypeEnum.QolmsOpenApi,
                            16,
                            endDate.AddMinutes(60)
                        );
                    }

                    results.Add(new QoApiHeartRateWarningTargetListItem()
                    {
                        AlertNo = item.AlertNo.ToString(),
                        Name = item.Name,
                        FamilyTel = item.FamilyTel,
                        HeartRateValue = item.Value1.ToString(),
                        Mail = item.Mail,
                        Message = item.Message,
                        RecordDate = item.RecordDate.ToApiDateString(),
                        Tel = item.Tel,
                        Token = viewerToken,
                        FamilyName=item.FamilyName,
                        Address =item.Address ,
                        FamilyRelationship =item.FamilyRelationship                        
                    });
                }
                
            }
            return results;
        }


        private static QoApiResultItem ChallengeEntryWriteArgsCheck(QoJotoHdrChallengeEntryWriteApiArgs args)
        {
            //連携先番号のチェック決め打ち
            if (args.LinkageSystemNo.Trim().CompareTo(QoLinkage.JOTO_LINKAGE_SYSTEM_NO_NAVITIME.ToString()) != 0)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です。");
            }
            if (string.IsNullOrEmpty(args.LinkageId))
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageIdが不正です。");
            }
            if (args.EntryEventKey.TryToValueType(Guid.Empty) == Guid.Empty)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "EntryEventKeyが不正です。");
            }
            if (string.IsNullOrEmpty(args.EntryCode))
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "EntryCodeが不正です。");
            }
            if (args.StartDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "StartDateが不正です。");
            }
            if (args.EndDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "EndtDateが不正です。");
            }

            return null;
        }

        private static QoApiResultItem PushSendArgsCheck(QoJotoHdrPushSendApiArgs args)
        {
            //連携先番号のチェック決め打ち
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty,$"PushSendArgsCheck {args.LinkageSystemNo} {args.LinkageId} {args.Message}" );
            if (args.LinkageSystemNo.Trim().CompareTo(QoLinkage.JOTO_LINKAGE_SYSTEM_NO_CALOMEAL.ToString()) != 0)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です。");
            }
            if (string.IsNullOrEmpty(args.LinkageId))
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageIdが不正です。");
            }
            if (string.IsNullOrEmpty(args.Message))
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "Messageが不正です。");
            }

            return null;
        }

        private static bool ReproPushApiCall(string pushMessage ,Guid accountkey,string deepLink)
        {
            //固定値でテストできるようにいったん（検証環境用）
            var pushKey =  string.IsNullOrEmpty(ConfigurationManager.AppSettings["PushKey"])? "xv9kyj84" : ConfigurationManager.AppSettings["PushKey"];
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, accountkey, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty,$"pushKey:{pushKey}" );

            //var set =  string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReproPushApiIfUri"]);
            //var set =  string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReproApiToken"]);

            pushApiArgs args = new pushApiArgs(pushKey,new List<Guid>() { accountkey })
            {
                notification = new Notification() { message = pushMessage,deeplink_url= deepLink}
            };

            pushApiResults res = QsReproApiManager.ExecuteAsync<pushApiArgs, pushApiResults>(args).Result;

            if (res.IsSuccess)
            {
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, accountkey, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty,$"{res.RequestString} {res.ResponseString}" );
                return true;
            }

            return false;
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤から イベント 情報パラメータチェックを行います
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        private static QoApiResultItem EventInfoArgsCheck(QoJotoHdrEventInfoWriteApiArgs args)
        {
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"EventInfoArgsCheck {args.LinkageSystemNo} {args.EventCode}");

            if (args.LinkageSystemNo.Trim().CompareTo(QoLinkage.JOTO_GINOWAN_SYSTEM_NO.ToString()) != 0)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です。");
            }
            if (string.IsNullOrWhiteSpace(args.EventCode))
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "EventCodeが不正です。");
            }
            if (string.IsNullOrWhiteSpace(args.EventName))
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "EventNameが不正です。");
            }
            DateTime startDate = args.StartDate.TryToValueType(DateTime.MinValue);
            DateTime endDate = args.EndDate.TryToValueType(DateTime.MinValue);
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue || startDate > endDate)
            {
                return QoApiResultHelper.Build(QoApiResultCodeTypeEnum.ArgumentError, "StartDate もしくは EndDate が不正です。");
            }

            return null;
        }

        /// <summary>
        /// HTTPリクエストメッセージを生成する内部メソッドです。
        /// </summary>
        /// <param name="httpMethod">HTTPメソッドのオブジェクト</param>
        /// <param name="requestEndPoint">通信先のURL</param>
        /// <returns>HttpRequestMessage</returns>
        private static HttpRequestMessage CreateRequest(HttpMethod httpMethod, string requestEndPoint)
        {
            var request = new HttpRequestMessage(httpMethod, requestEndPoint);
            return request;
        }

        /// <summary>
        /// 外部 API を呼び出します。
        /// </summary>
        /// <param name="method">HTTP メソッド ("GET" または "POST")</param>
        /// <param name="requestEndPoint">リクエスト先の URL</param>
        /// <param name="contentType">コンテンツタイプ (例: "application/json")</param>
        /// <param name="headerN">HTTP ヘッダーのディクショナリ</param>
        /// <param name="body">リクエストボディ (POST の場合のみ使用)</param>
        /// <returns>レスポンスボディとHTTPステータスコードのタプル</returns>
        public static async Task<(string body, HttpStatusCode statusCode)> CallApi(
            string method, 
            string requestEndPoint, 
            string contentType, 
            Dictionary<string, string> headerN, 
            string body = "")
        {
            HttpMethod httpMethod = method.ToUpper() == "POST" ? HttpMethod.Post : HttpMethod.Get;
            
            HttpRequestMessage request = CreateRequest(httpMethod, requestEndPoint);
            httpClient.DefaultRequestHeaders.Authorization = null;
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Clear();
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; // TLS 1.2
            foreach (var header in headerN) httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

            // POST の場合のみボディを設定
            if (method.ToUpper() == "POST" && !string.IsNullOrWhiteSpace(body))
            {
                request.Content = new StringContent(body, new System.Text.UTF8Encoding(false), contentType);
            }

            string resBodyStr;
            HttpStatusCode resStatusCode = HttpStatusCode.NotFound;
            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(request).ConfigureAwait(false);
                resBodyStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                resStatusCode = response.StatusCode;
            }
            catch (HttpRequestException e)
            {
                // 通信失敗のエラー
                return (e.Message, HttpStatusCode.InternalServerError);
            }

            if (!resStatusCode.Equals(HttpStatusCode.OK))
            {
                // レスポンスが200 OK以外の場合も本文を保持して呼び出し元で原因判別できるようにする
                return (string.Format("Request : {0} Response StatusCode : {1} Body : {2}", requestEndPoint, resStatusCode, resBodyStr), resStatusCode);
            }
            if (String.IsNullOrWhiteSpace(resBodyStr))
            {
                return ("Response Bodyが空です", resStatusCode);
            }
            //取得成功
            return (resBodyStr, resStatusCode);
        }

        /// <summary>
        /// OCCAuthAPIを実行します。
        /// </summary>
        /// <returns>
        /// OCCAuthAPIの実行結果
        /// </returns>
        private static OCCAuthApiResultsOfJson ExecuteOCCAuthApi()
        {
            var apiArgs = new OCCAuthApiArgsOfJson()
            {
                AuthFlow = QoApiConfiguration.JotoGinowanOccAuthFlow,
                ClientId = QoApiConfiguration.JotoGinowanClientId,
                AuthParameters = new OCCAuthApiArgsInnerOfJson()
                {
                    USERNAME = QoApiConfiguration.JotoGinowanUserName,
                    PASSWORD = QoApiConfiguration.JotoGinowanPassword
                }
            };

            var contentType = QoApiConfiguration.JotoGinowanOccContentType;
            var headerN = new Dictionary<string, string>();
            headerN.Add("Accept", contentType);
            headerN.Add("X-Amz-Target", QoApiConfiguration.JotoGinowanOccAmzTarget);

            var ser = new QsJsonSerializer();
            var strArgs = ser.Serialize(apiArgs);

            var (apiResults, statusCode) = JotoHdrWorker.CallApi("POST", QoApiConfiguration.JotoGinowanAuthApiUri, contentType, headerN, strArgs).Result;

            var results = ser.Deserialize<OCCAuthApiResultsOfJson>(apiResults);

            results = results == null ? new OCCAuthApiResultsOfJson() : results;
            results.Result = apiResults;

            return results;

        }

        /// <summary>
        /// OCC 合計地域ポイント取得 API を実行します。
        /// </summary>
        /// <returns>
        /// OCC 合計地域ポイント取得 API の実行結果
        /// </returns>
        public static QoJotoHdrLocalPointSummaryReadApiResults ExecuteOCCLocalPointSummaryApi(string linkageId, string token)
        {
            var contentType = "application/json";
            var headerN = new Dictionary<string, string>();
            headerN.Add("Accept", contentType);

            headerN.Add("Authorization", $"Bearer {token}");
            headerN.Add("X-App-Id", $"{QoApiConfiguration.JotoGinowanXApiId}");

            // イベントコードで Uri を生成する。
            var baseUri = new Uri(QoApiConfiguration.JotoGinowanPointApiBaseUri.TrimEnd('/') + "/");
            var uri = new Uri(baseUri, $"{linkageId}/summary");

            var (apiResults, statusCode) = JotoHdrWorker.CallApi("GET", uri.ToString(), contentType, headerN).Result;

            QoJotoHdrLocalPointSummaryReadApiResults results = new QoJotoHdrLocalPointSummaryReadApiResults();
            if (statusCode == HttpStatusCode.OK)
            {
                var ser = new QsJsonSerializer();
                results = ser.Deserialize<QoJotoHdrLocalPointSummaryReadApiResults>(apiResults);
                // Deserialize が null を返す可能性があるのでチェック
                if (results == null)
                {
                    results = new QoJotoHdrLocalPointSummaryReadApiResults();
                }
            }

            // Result を初期化
            if (results.Result == null)
            {
                results.Result = new QoApiResultItem();
            }

            results.Result.Code = statusCode.ToString();
            results.Result.Detail = apiResults;

            return results;
        }

        /// <summary>
        /// OCC ポイント交換 API を実行します。
        /// </summary>
        /// <returns>
        /// OCC 合計地域ポイント取得 API の実行結果
        /// </returns>
        public static QoJotoHdrLocalPointRedeemReadApiResults ExecuteOCCLocalPointRedeemApi(string linkageId, string points, string idempotencyKey, string token)
        {
            var contentType = "application/json";
            var headerN = new Dictionary<string, string>();
            headerN.Add("Accept", contentType);

            headerN.Add("Authorization", $"Bearer {token}");
            headerN.Add("X-App-Id", $"{QoApiConfiguration.JotoGinowanXApiId}");
            headerN.Add("Idempotency-Key", idempotencyKey);

            // イベントコードで Uri を生成する。
            var baseUri = new Uri(QoApiConfiguration.JotoGinowanPointApiBaseUri.TrimEnd('/') + "/");
            var uri = new Uri(baseUri, $"{linkageId}/redeem");

            // リクエストボディを生成
            var pointsValue = points.TryToValueType(int.MinValue);
            var requestBody = $@"{{ ""Points"": {pointsValue} }}";

            var (apiResults, statusCode) = JotoHdrWorker.CallApi("POST", uri.ToString(), contentType, headerN, requestBody).Result;

            QoJotoHdrLocalPointRedeemReadApiResults results = new QoJotoHdrLocalPointRedeemReadApiResults();
            if (statusCode == HttpStatusCode.OK)
            {
                var ser = new QsJsonSerializer();
                results = ser.Deserialize<QoJotoHdrLocalPointRedeemReadApiResults>(apiResults);
                // Deserialize が null を返す可能性があるのでチェック
                if (results == null)
                {
                    results = new QoJotoHdrLocalPointRedeemReadApiResults();
                }
            }

            // Result を初期化
            if (results.Result == null)
            {
                results.Result = new QoApiResultItem();
            }

            results.Result.Code = statusCode.ToString();
            results.Result.Detail = apiResults;

            return results;
        }

        /// <summary>
        /// OCC 地域ポイント履歴取得 API を実行します。
        /// </summary>
        /// <returns>
        /// OCC 地域ポイント履歴取得 API の実行結果
        /// </returns>
        public static QoJotoHdrLocalPointHistoryReadApiResults ExecuteOCCLocalPointHistoryApi(string linkageId, string month, string token)
        {
            var contentType = "application/json";
            var headerN = new Dictionary<string, string>();
            headerN.Add("Accept", contentType);

            headerN.Add("Authorization", $"Bearer {token}");
            headerN.Add("X-App-Id", $"{QoApiConfiguration.JotoGinowanXApiId}");

            // パラメータの生成
            DateTime monthDateTime = month.TryToValueType(DateTime.MinValue);
            string yyyyMm = monthDateTime.ToString("yyyy-MM");

            // イベントコードで Uri を生成する。
            var baseUri = new Uri(QoApiConfiguration.JotoGinowanPointApiBaseUri.TrimEnd('/') + "/");
            var uri = new Uri(baseUri, $"{linkageId}/history?month={yyyyMm}");

            var (apiResults, statusCode) = JotoHdrWorker.CallApi("GET", uri.ToString(), contentType, headerN).Result;

            QoJotoHdrLocalPointHistoryReadApiResults results = new QoJotoHdrLocalPointHistoryReadApiResults();

            if (statusCode == HttpStatusCode.OK)
            {
                var ser = new QsJsonSerializer();
                results = ser.Deserialize<QoJotoHdrLocalPointHistoryReadApiResults>(apiResults);
                // Deserialize が null を返す可能性があるのでチェック
                if (results == null)
                {
                    results = new QoJotoHdrLocalPointHistoryReadApiResults();
                }
            }

            // Result を初期化
            if (results.Result == null)
            {
                results.Result = new QoApiResultItem();
            }

            results.Result.Code = statusCode.ToString();
            results.Result.Detail = apiResults;

            return results;
        }

        // OCC 側で LinkageId 未登録時は 404 が返るため、空データ返却へ切り替える判定に使う。
        private static bool IsOccLinkageNotFound(QoApiResultItem result)
        {
            if (result == null)
            {
                return false;
            }

            return string.Equals(result.Code, HttpStatusCode.NotFound.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        // 地域ポイント未連携時の画面表示用に、0ポイントの正常結果を組み立てる。
        private static QoJotoHdrLocalPointSummaryReadApiResults BuildEmptyLocalPointSummaryReadResults()
        {
            return new QoJotoHdrLocalPointSummaryReadApiResults()
            {
                IsSuccess = bool.TrueString,
                Status = "SUCCESS",
                Data = new QoApiLocalPointSummaryItem()
                {
                    TotalAvailablePoints = "0",
                    ExpireEndofMonth = "0"
                },
                Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success)
            };
        }

        // 地域ポイント未連携時の画面表示用に、空履歴の正常結果を組み立てる。
        private static QoJotoHdrLocalPointHistoryReadApiResults BuildEmptyLocalPointHistoryReadResults(string linkageId, string month)
        {
            return new QoJotoHdrLocalPointHistoryReadApiResults()
            {
                IsSuccess = bool.TrueString,
                Status = "SUCCESS",
                LinkageID = linkageId,
                TotalRecords = "0",
                Month = month,
                History = new List<QoApiLocalPointHistoryItem>(),
                Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success)
            };
        }

        // 地域ポイント未連携時でも交換画面側で異常終了させないため、0件の正常結果を組み立てる。
        private static QoJotoHdrLocalPointRedeemReadApiResults BuildEmptyLocalPointRedeemReadResults(string linkageId)
        {
            return new QoJotoHdrLocalPointRedeemReadApiResults()
            {
                IsSuccess = bool.TrueString,
                Status = "SUCCESS",
                LinkageID = linkageId,
                RedeemedPoints = "0",
                RemainingBalance = "0",
                RedemptionDetails = new List<QoApiLocalPointRedemptionDetailItem>(),
                Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success)
            };
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// JOTO 心臓見守りサービスのアプリからの問い合わせにアラートデータを返す。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoJotoHdrHeartRateWarningReadApiResults HeartRateWarningRead(QoJotoHdrHeartRateWarningReadApiArgs args)
        {
            DateTime checkDate = args.CheckDate.TryToValueType(DateTime.MinValue);
            long alertNo = args.AlertNo.TryToValueType(long.MinValue);
            if (checkDate < DateTime.Now.AddDays(-7) && alertNo < 0)
                return new QoJotoHdrHeartRateWarningReadApiResults() { IsSuccess =bool.FalseString, 
                                                                        Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "引数が不正です。")};

            DateTime checkEndDate = DateTime.Now;
         
            return new QoJotoHdrHeartRateWarningReadApiResults()
            {
                IsSuccess = bool.TrueString,
                ExecutedDate = checkEndDate.ToApiDateString(),
                TargetList = GetAlertList(alertNo,checkDate,checkEndDate,args.Executor)
            };
    
        }


        /// <summary>
        /// JOTO ALKOOからのイベント登録情報登録
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrPushSendApiResults PushSend(QoJotoHdrPushSendApiArgs args)
        {
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty,$"{args.LinkageSystemNo} {args.LinkageId} {args.Message}" );
            QoJotoHdrPushSendApiResults results = new QoJotoHdrPushSendApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };
            // チェック
            results.Result = PushSendArgsCheck(args);
            if (results.Result == null)
            {
                try
                {
                    Guid accountKey = Guid.Empty;
                    // Linkageからアカウントキーへ
                    LinkageUserMemberShipReader reader = new LinkageUserMemberShipReader();
                    LinkageUserMemberShipReaderArgs readerArgs = new LinkageUserMemberShipReaderArgs()
                    {
                        LinkageId = args.LinkageId,
                        Executor = args.Executor.TryToValueType(Guid.Empty),
                        LinkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue),
                        TargetDate = DateTime.Now
                    };
                    LinkageUserMemberShipReaderResults readerResult = QsDbManager.Read(reader, readerArgs);
                    if (readerResult.IsSuccess)
                        accountKey = readerResult.AccountKey;
                    else
                    {
                        // こちらのDBに存在しない(ほとんどありえない）
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                        return results;
                    }

                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, accountKey, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty,$"pushKey:{accountKey}" );
                    //リプロPushApiを呼び出す
                    var res = ReproPushApiCall(args.Message, accountKey, args.DeeplinkUrl);

                    if (res)
                    {
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                        results.IsSuccess = Boolean.TrueString;
                    }
                    else
                    { 
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "登録に失敗しました。");
                    }
                }
                catch (Exception ex)
                {
                    results.Result = QoApiResult.Build(ex);
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, ex.Message);
                }
            }
            else
            {
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
            }
            return results;
        }

        /// <summary>
        /// JOTO ぎのわん PJ データ 基盤から イベント 情報登録
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrEventInfoWriteApiResults EventInfoWrite(QoJotoHdrEventInfoWriteApiArgs args)
        {
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"EventInfoWrite IN {args.LinkageSystemNo} {args.EventCode}");

            QoJotoHdrEventInfoWriteApiResults results = new QoJotoHdrEventInfoWriteApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            // パラメータチェック
            results.Result = EventInfoArgsCheck(args);

            if (results.Result == null)
            {
                try
                {
                    EventInfoWriter reader = new EventInfoWriter();
                    EventInfoWriterArgs readerArgs = new EventInfoWriterArgs()
                    {
                        Entity = new QJ_EVENTINFO_DAT()
                        {
                            EVENTNAME = args.EventName,
                            EVENTCODE = args.EventCode,
                            LINKAGESYSTEMNO = args.LinkageSystemNo.TryToValueType(int.MinValue),
                            STARTDATE = args.StartDate.TryToValueType(DateTime.MinValue),
                            ENDDATE = args.EndDate.TryToValueType(DateTime.MinValue),
                            // とりあえず、現状は仕様未定義のデータセット
                            //DATASET = new QsJsonSerializer().Serialize(new QjEventInfoSetOfJson() { })
                            DATASET = string.Empty
                        }
                    };
                    EventInfoWriterResults readerResult = QsDbManager.Write(reader, readerArgs);

                    if (readerResult.IsSuccess)
                    {
                        results.IsSuccess = Boolean.TrueString;
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                    else
                    {
                        // こちらのDBに存在しない(ほとんどありえない）
                        results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                        return results;
                    }
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"EventInfoWrite OK EventCode:{args.EventCode}");
                }
                catch (Exception ex)
                {
                    results.Result = QoApiResult.Build(ex);
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, ex.Message);
                }
            }
            else
            {
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
            }
            return results;
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤 API を呼び出し、合計地域ポイントを受け取ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrLocalPointSummaryReadApiResults LocalPointSummaryRead(QoJotoHdrLocalPointSummaryReadApiArgs args)
        {
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointSummaryRead IN {args.LinkageId}");

            QoJotoHdrLocalPointSummaryReadApiResults results = new QoJotoHdrLocalPointSummaryReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            // パラメータチェック
            if (string.IsNullOrWhiteSpace(args.LinkageId))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageIdが不正です。");
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                return results;
            }

            try
            {
                // ぎのわん PJ データ 基盤 API を呼び出し、合計地域ポイントを取得
                var auth = JotoHdrWorker.ExecuteOCCAuthApi();
                if (auth.AuthenticationResult == null || string.IsNullOrWhiteSpace(auth.AuthenticationResult.AccessToken))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "OCC 認証　API Cognito に失敗しました。");
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                    return results;
                }

                var apiResults = JotoHdrWorker.ExecuteOCCLocalPointSummaryApi(args.LinkageId, auth.AuthenticationResult.AccessToken);
                // LinkageId 未登録は業務上の正常系として扱い、0ポイントを返す。
                if (JotoHdrWorker.IsOccLinkageNotFound(apiResults.Result))
                {
                    results = JotoHdrWorker.BuildEmptyLocalPointSummaryReadResults();
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointSummaryRead OCC 404 fallback LinkageId:{args.LinkageId}");
                    return results;
                }

                if (apiResults.Status != "SUCCESS")
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, $"OCC ポイント合計取得 API 実行に失敗しました Result {apiResults.Result.Detail}");
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                    return results;
                }

                results = apiResults;
                results.IsSuccess = Boolean.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointSummaryRead OK LinkageId:{args.LinkageId}");
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, ex.Message);
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, ex.Message);
            }

            return results;
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤 API を呼び出し、ポイント交換を行い、結果を受け取ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrLocalPointRedeemReadApiResults LocalPointRedeemRead(QoJotoHdrLocalPointRedeemReadApiArgs args)
        {
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointRedeemRead IN {args.LinkageId}");

            QoJotoHdrLocalPointRedeemReadApiResults results = new QoJotoHdrLocalPointRedeemReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            // パラメータチェック
            if (
                string.IsNullOrWhiteSpace(args.LinkageId) || 
                string.IsNullOrWhiteSpace(args.Points) ||
                !int.TryParse(args.Points, out int pointsValue) ||
                string.IsNullOrWhiteSpace(args.IdempotencyKey) ||
                !Guid.TryParse(args.IdempotencyKey, out Guid _)
                )
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータが不正です。[{args.LinkageId}], [{args.Points}], [{args.IdempotencyKey}]" );
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                return results;
            }

            try
            {
                // ぎのわん PJ データ 基盤 API を呼び出し、ポイント交換を実行
                var auth = JotoHdrWorker.ExecuteOCCAuthApi();
                if (auth.AuthenticationResult == null || string.IsNullOrWhiteSpace(auth.AuthenticationResult.AccessToken))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "OCC 認証　API Cognito に失敗しました。");
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                    return results;
                }

                var apiResults = JotoHdrWorker.ExecuteOCCLocalPointRedeemApi(args.LinkageId, args.Points, args.IdempotencyKey, auth.AuthenticationResult.AccessToken);
                // LinkageId 未登録は業務上の正常系として扱い、交換0件を返す。
                if (JotoHdrWorker.IsOccLinkageNotFound(apiResults.Result))
                {
                    results = JotoHdrWorker.BuildEmptyLocalPointRedeemReadResults(args.LinkageId);
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointRedeemRead OCC 404 fallback LinkageId:{args.LinkageId}");
                    return results;
                }

                if (apiResults.Status != "SUCCESS")
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, $"OCC ポイント交換 API 実行に失敗しました Result {apiResults.Result.Detail}");
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                    return results;
                }

                results = apiResults;
                results.IsSuccess = Boolean.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointRedeemRead OK LinkageId:{args.LinkageId}");
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, ex.Message);
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, ex.Message);
            }

            return results;
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤 API を呼び出し、地域ポイント履歴を受け取ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoJotoHdrLocalPointHistoryReadApiResults LocalPointHistoryRead(QoJotoHdrLocalPointHistoryReadApiArgs args)
        {
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointHistoryRead IN {args.LinkageId}");

            QoJotoHdrLocalPointHistoryReadApiResults results = new QoJotoHdrLocalPointHistoryReadApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };


            // パラメータチェック
            if (
                string.IsNullOrWhiteSpace(args.LinkageId) ||
                string.IsNullOrWhiteSpace(args.Month) ||
                args.Month.TryToValueType(DateTime.MinValue) == DateTime.MinValue
                )
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"パラメータがが不正です。[{ args.LinkageId }],[{ args.Month }]");
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                return results;
            }

            try
            {
                // 外部APIを呼び出してポイント履歴データを取得
                var auth = JotoHdrWorker.ExecuteOCCAuthApi();
                if (auth.AuthenticationResult == null || string.IsNullOrWhiteSpace(auth.AuthenticationResult.AccessToken))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "OCC 認証　API Cognito に失敗しました。");
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                    return results;
                }

                var apiResults = JotoHdrWorker.ExecuteOCCLocalPointHistoryApi(args.LinkageId, args.Month, auth.AuthenticationResult.AccessToken);
                // LinkageId 未登録は業務上の正常系として扱い、空履歴を返す。
                if (JotoHdrWorker.IsOccLinkageNotFound(apiResults.Result))
                {
                    results = JotoHdrWorker.BuildEmptyLocalPointHistoryReadResults(args.LinkageId, args.Month);
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointHistoryRead OCC 404 fallback LinkageId:{args.LinkageId}");
                    return results;
                }

                if (apiResults.Status != "SUCCESS")
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, $"OCC ポイント履歴取得 API 実行に失敗しました Result {apiResults.Result.Detail}");
                    AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, MakeLogData(args, results));
                    return results;
                }

                results = apiResults;
                results.IsSuccess = Boolean.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"LocalPointHistoryRead OK LinkageId:{args.LinkageId}");
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, ex.Message);
                AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, args.AuthorKey.TryToValueType(Guid.Empty), DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, ex.Message);
            }

            return results;
        }
        #endregion
    }
}
