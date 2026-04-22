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
using System.Configuration;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Providers;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using System.Threading.Tasks;
using System.Threading;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// JOTO（宜野湾）関連 の公開API用Worker
    /// </summary>
    public class PushWorker
    {

        #region "インターフェース"

        ILinkageRepository _linkageRepo;
        IReproRepository _reproRepo;
        INoticeGroupRepository _noticeGroupRepo;
        IQoPushNotification _pushNotification;
        IDateTimeProvider _datetimeProv;

        #endregion

        #region "constructor"

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="linkageRepository"></param>
        /// <param name="dateTimeProvider"></param>
        public PushWorker(ILinkageRepository linkageRepository, IReproRepository reproRepository, INoticeGroupRepository noticeGroupRepo, IQoPushNotification pushNotification, IDateTimeProvider dateTimeProvider)
        {
            _linkageRepo = linkageRepository;
            _reproRepo = reproRepository;
            _noticeGroupRepo = noticeGroupRepo;
            _pushNotification = pushNotification;
            _datetimeProv = dateTimeProvider;
        }

        #endregion

        #region "Private Method"

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="linkageSystemNo"></param>
        ///// <param name="lineUserId"></param>
        ///// <returns></returns>
        //private static List<Guid> ReadAccountKeys(int linkageSystemNo, string LinkageIds)
        //{
        //    var reader = new LinkageUserCheckReader();

        //    try
        //    {
        //        var readerArgs = new LinkageUserCheckReaderArgs() { LinkageSystemNo = linkageSystemNo, LinkageIds = LinkageIds };

        //        //読込
        //        var readerResults = QsDbManager.Read(reader, readerArgs);
        //        if (readerResults != null && readerResults.IsSuccess && readerResults.AccountKeys.Count > 0)
        //        {
        //            //結果格納
        //            return readerResults.AccountKeys;
        //        }
        //        else
        //        {
        //            QoAccessLog.WriteInfoLog(readerResults.AccountKeys.Count.ToString());
        //            QoAccessLog.WriteErrorLog(string.Format($"[ReadAccountKeys]QH_LINKAGE_DAT情報の取得に失敗しました。"), Guid.Empty);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        QoAccessLog.WriteErrorLog(ex, Guid.Empty);
        //    }

        //    return new List<Guid>();
        //}
       
       
        
        private static QsDbApplicationTypeEnum GetFeomSystem(QsApiSystemTypeEnum systemtype)
        {
            switch (systemtype)
            {
                case QsApiSystemTypeEnum.Qolms:
                    return QsDbApplicationTypeEnum.Qolms;
                case QsApiSystemTypeEnum.QolmsViewer:
                    return QsDbApplicationTypeEnum.QolmsViewer;
                case QsApiSystemTypeEnum.QolmsManagement:
                    return QsDbApplicationTypeEnum.QolmsManagement;

                case QsApiSystemTypeEnum.QolmsJoto:
                case QsApiSystemTypeEnum.QolmsJotoApi:
                case QsApiSystemTypeEnum.JotoiOSApp:
                case QsApiSystemTypeEnum.JotoAndroidApp:
                case QsApiSystemTypeEnum.JotoNativeiOSApp:
                case QsApiSystemTypeEnum.JotoNativeAndroidApp:
                case QsApiSystemTypeEnum.JotoNative:
                    return QsDbApplicationTypeEnum.JotoNative;

                case QsApiSystemTypeEnum.QolmsWebJob:
                    return QsDbApplicationTypeEnum.QolmsWebJob;

                case QsApiSystemTypeEnum.JotoWebJob:
                    return QsDbApplicationTypeEnum.JotoWebJob;

                case QsApiSystemTypeEnum.JotoMex:
                case QsApiSystemTypeEnum.JotoNavitime:
                case QsApiSystemTypeEnum.JotoLLT:
                case QsApiSystemTypeEnum.JotoGinowan:
                case QsApiSystemTypeEnum.Other:
                default:
                    return QsDbApplicationTypeEnum.Other;
            }

            return QsDbApplicationTypeEnum.None;
        }

        /// <summary>
        /// 連携システム番号から、対象のシステムに置き換えます。
        /// JOTO以外にPushが必要になった場合は拡張が必要です。
        /// </summary>
        /// <param name="linkage"></param>
        /// <returns></returns>
        private static QsDbApplicationTypeEnum GetTargetSystem(int linkage)
        {
            if (!string.IsNullOrEmpty(QoApiConfiguration.NoticeGroupToSystemJotoList))
            {
                var jotoList = QoApiConfiguration.NoticeGroupToSystemJotoList.Split(',').Select(i => i.TryToValueType(int.MinValue));

                if (jotoList.Contains(linkage))
                {
                    return QsDbApplicationTypeEnum.JotoNative;
                }
            }

            return QsDbApplicationTypeEnum.None;
        }

        /// <summary>
        /// 連携システム番号から、対象のシステムに置き換えます。
        /// JOTOJOB以外から呼ばれるようになったら考え直した方がいいです。
        /// </summary>
        /// <returns></returns>
        private static QsDbApplicationTypeEnum GetTargetSystem(QsApiSystemTypeEnum fromSystemType)
        {
            switch (fromSystemType)
            {
                case QsApiSystemTypeEnum.JotoWebJob:
                    return QsDbApplicationTypeEnum.JotoNative;
                default:
                    break;
            }

            return QsDbApplicationTypeEnum.None;
        }

        private static QoApiResultItem CheckArgs(QoPushSendApiArgs args)
        {
            // チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                QoAccessLog.WriteInfoLog($"LinkageSystemNo Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }
            var systemType = (QsApiSystemTypeEnum)Enum.Parse(typeof(QsApiSystemTypeEnum), args.ExecuteSystemType);
            if (systemType.ToString() != QsApiSystemTypeEnum.JotoGinowan.ToString())
            {
                QoAccessLog.WriteInfoLog($"ExecuteSystemType Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }
            if (string.IsNullOrWhiteSpace(args.Contents))
            {
                QoAccessLog.WriteInfoLog($"Contents Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (args.LinkageIds.Count <= 0)
            {
                QoAccessLog.WriteInfoLog($"LinkageIdsCount Error {args.LinkageIds.Count}");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (!DateTime.TryParse(args.StartDate, out DateTime startDate) || startDate < DateTime.Now)
            {
                QoAccessLog.WriteInfoLog($"StartDate Error startDate:{startDate} DateTimeNow:{DateTime.Now}");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }
                
            return new QoApiResultItem();
        }

        private static QoApiResultItem CheckArgs(QoPushSendAccountApiArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Contents))
            {
                QoAccessLog.WriteInfoLog($"Contents Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (string.IsNullOrWhiteSpace(args.PriorityNo))
            {
                QoAccessLog.WriteInfoLog($"PriorityNo Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (string.IsNullOrWhiteSpace(args.CategoryNo))
            {
                QoAccessLog.WriteInfoLog($"CategoryNo Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (args.Accountkeys != null && args.Accountkeys.Count <= 0)
            {
                QoAccessLog.WriteInfoLog($"LinkageIdsCount Error {args.Accountkeys.Count}");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (!DateTime.TryParse(args.StartDate, out DateTime startDate) || startDate < DateTime.Now)
            {
                QoAccessLog.WriteInfoLog($"StartDate Error startDate:{startDate} DateTimeNow:{DateTime.Now}");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            return new QoApiResultItem();
        }
        private QH_NOTICEGROUP_DAT CreateNoticeGroup(QoPushSendApiArgs args, QsDbApplicationTypeEnum apptype) 
        {
            var sr = new QsJsonSerializer();

            DateTime startDate = DateTime.Parse(args.StartDate);
            DateTime endDate = DateTime.Parse(args.EndDate);
            DateTime now = _datetimeProv.Now;

            QhNoticeDataSetOfJson json = new QhNoticeDataSetOfJson()
            {
                AttachedFileN = new List<QhAttachedFileOfJson>(),
                LinkN = new List<QhNoticeLinkItemOfJson>() { new QhNoticeLinkItemOfJson() { LinkText = "", LinkUrl = args.DeeplinkUrl } },
                PushSendN = new List<QhNoticePushSendItemOfJson>() { new QhNoticePushSendItemOfJson() { PushDate = "", PushId = "" } }
            };

            return new QH_NOTICEGROUP_DAT()
            {
                NOTICENO = long.MinValue,
                TITLE = args.Title,
                CONTENTS = args.Contents,
                CATEGORYNO = byte.Parse(args.CategoryNo),
                PRIORITYNO = byte.Parse(args.PriorityNo),
                FROMSYSTEMTYPE =(byte)GetFeomSystem(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)),
                TOSYSTEMTYPE = (byte)apptype,
                FACILITYKEY = Guid.Empty,//現状施設の指定はなし
                TARGETTYPE = 2,
                STARTDATE = startDate,
                ENDDATE = endDate,
                MAILSENDFLAG = false,
                PUSHSENDFLAG = true,
                SCHEDULENO = 0,
                NOTICEDATASET = sr.Serialize(json),
                DELETEFLAG = false,
                CREATEDDATE = now,
                CREATEDACCOUNTKEY = Guid.Parse(args.Executor),
                UPDATEDDATE = now,
                UPDATEDACCOUNTKEY = Guid.Parse(args.Executor)
            };
        }

        private QH_NOTICEGROUP_DAT CreateNoticeGroup(QoPushSendAccountApiArgs args, QsDbApplicationTypeEnum apptype)
        {
            var sr = new QsJsonSerializer();

            DateTime startDate = DateTime.Parse(args.StartDate);
            DateTime endDate = DateTime.Parse(args.EndDate);
            DateTime now = _datetimeProv.Now;

            QhNoticeDataSetOfJson json = new QhNoticeDataSetOfJson()
            {
                AttachedFileN = new List<QhAttachedFileOfJson>(),
                LinkN = new List<QhNoticeLinkItemOfJson>() { new QhNoticeLinkItemOfJson() { LinkText = "", LinkUrl = args.DeeplinkUrl } },
                PushSendN = new List<QhNoticePushSendItemOfJson>() { new QhNoticePushSendItemOfJson() { PushDate = "", PushId = "" } }
            };

            return new QH_NOTICEGROUP_DAT()
            {
                NOTICENO = long.MinValue,
                TITLE = args.Title,
                CONTENTS = args.Contents,
                CATEGORYNO = byte.Parse(args.CategoryNo),
                PRIORITYNO = byte.Parse(args.PriorityNo),
                FROMSYSTEMTYPE = (byte)GetFeomSystem(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)),
                TOSYSTEMTYPE = (byte)apptype,
                FACILITYKEY = Guid.Empty,//現状施設の指定はなし
                TARGETTYPE = 2,
                STARTDATE = startDate,
                ENDDATE = endDate,
                MAILSENDFLAG = false,
                PUSHSENDFLAG = true,
                SCHEDULENO = 0,
                NOTICEDATASET = sr.Serialize(json),
                DELETEFLAG = false,
                CREATEDDATE = now,
                CREATEDACCOUNTKEY = Guid.Parse(args.Executor),
                UPDATEDDATE = now,
                UPDATEDACCOUNTKEY = Guid.Parse(args.Executor)
            };
        }

        /// <summary>
        /// 複数ユーザーへの並列プッシュ送信
        /// </summary>
        private async Task<List<(Guid, string[])>> SendPushToUsersAsync(
            List<Guid> accountkeys,
            NotificationRequest request,
            string tag)
        {
            var semaphore = new SemaphoreSlim(5);//設定
            var tasks = new List<Task<string[]>>();
            var results = new System.Collections.Concurrent.ConcurrentBag<(Guid acc, string[] pushid)>();
            var failedUsers = new System.Collections.Concurrent.ConcurrentBag<(Guid acc, Exception ex)>();

            foreach (var acc in accountkeys)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        request.SetTagExpressionJoinAllAnd(new string[] { tag, "$UserId:{" + acc.ToEncrypedReference() + "}" });
                        string[] result = await _pushNotification.RequestNotificationAsync(request);
                        results.Add((acc,result));
                        return result;
                    }
                    catch (Exception ex)
                    {
                        QoAccessLog.WriteErrorLog(
                            string.Format("プッシュ送信エラー: UserId={0}, Error={1}", acc.ToEncrypedReference(), ex.Message),
                            Guid.Empty);
                        failedUsers.Add((acc, ex));
                        return new string[] { };
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(
                    string.Format("プッシュ送信の並列処理エラー: {0}", ex.Message),
                    Guid.Empty);
            }

            return results.ToList();
        }


        private async Task<List<(Guid acc, string[] pushid)>> SendPushAsync(QsDbApplicationTypeEnum toSystemType, long noticeNo, DateTime schedule,List<Guid> accountkey, string title, string message, string url, string categoryNo)
        {
            //toSystemType = QsDbApplicationTypeEnum.JotoiOSApp;//暫定

            _pushNotification = toSystemType.ToNotificationInstance();

            string tag = "Information";
            // CategoryNo = 13 システム通知(予約) の場合 Reservationにタグ変更
            if (categoryNo == "13")
            {
                tag = "Reservation";
            }

            var request = new NotificationRequest
            {
                Extra = "",
                Silent = false,
                Text = message,
                Url = url,
                Badge = 1,
                Title = title,
                ScheduleDate = schedule
            };

            // 並列処理でユーザーごとの通知を送信
            List<(Guid acc,string[] pushid)> results = await SendPushToUsersAsync(accountkey, request, tag);

            if (results.Any())
            {
                results = results.Where(i => !string.IsNullOrEmpty(i.pushid[0]) || !string.IsNullOrEmpty(i.pushid[1])).ToList();
                // QoAccessLog.WriteInfoLog(string.Format("お知らせPush送信成功:No={0},User={1}", noticeNo, userId ));
                //db更新
                if (!_noticeGroupRepo.UpdateNoticeGroupData(noticeNo, true, null, results.Select(i => i.pushid).ToList()))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知しましたが結果の更新に失敗しました:{0}", noticeNo), Guid.Empty);

                return results;
            }
            else
            {
                //QoAccessLog.WriteErrorLog(string.Format("お知らせPush送信失敗:{0},User={1}", noticeNo, userId), Guid.Empty);
                QoAccessLog.WriteErrorLog(string.Format("お知らせPush送信失敗:{0},User={1}", noticeNo, string.Join(",", results.Select(i => i.acc).ToList())), Guid.Empty);
                //db更新
                if (!_noticeGroupRepo.UpdateNoticeGroupData(noticeNo, false, null, null))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知失敗しましたが結果の更新に失敗しました:{0}", noticeNo), Guid.Empty);
                return results;
            }
        }

        /// <summary>
        /// ネイティブ用お知らせ処理
        /// </summary>
        /// <returns></returns>
        private bool GroupNothificartionCall(QoPushSendApiArgs args ,List<Guid> accountKeys) 
        {
            var toSystemType = GetTargetSystem(args.LinkageSystemNo.TryToValueType(int.MinValue));

            //① QH_NOTICEGROUP_DAT にお知らせ内容を登録
            var entity = CreateNoticeGroup(args, toSystemType);
            var noticeno = _noticeGroupRepo.InsertNoticeGroupData(entity);

            if (noticeno > 0)
            {
                //②プッシュ通知依頼
                //③ QH_NOTICEGROUP_DAT にPush通知情報を更新
                var results = Task.Run(() => {
                    return SendPushAsync(toSystemType, noticeno, entity.STARTDATE, accountKeys, entity.TITLE, entity.CONTENTS, args.DeeplinkUrl, args.CategoryNo);
                }).GetAwaiter().GetResult();

                //お知らせ既読管理登録(送信が失敗してもお知らせには出すので登録する）
                //④ QH_NOTICEGROUPTARGET_DAT へ対象ユーザーと既読管理を登録
                return _noticeGroupRepo.WriteGroupTarget(noticeno, results, entity.STARTDATE, _datetimeProv.Now);
            }

            return false;
        }

        /// <summary>
        /// ネイティブ用お知らせ処理
        /// </summary>
        /// <returns></returns>
        private bool GroupNothificartionCall(QoPushSendAccountApiArgs args, List<Guid> accountKeys)
        {
            //JOB用に暫定で向き先を指定。
            var toSystemType = GetTargetSystem(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));

            //① QH_NOTICEGROUP_DAT にお知らせ内容を登録
            var entity = CreateNoticeGroup(args, toSystemType);
            var noticeno = _noticeGroupRepo.InsertNoticeGroupData(entity);

            if (noticeno > 0)
            {
                //②プッシュ通知依頼
                //③ QH_NOTICEGROUP_DAT にPush通知情報を更新
                var results = Task.Run(() => {
                    return SendPushAsync(toSystemType, noticeno, entity.STARTDATE, accountKeys, entity.TITLE, entity.CONTENTS, args.DeeplinkUrl, args.CategoryNo);
                }).GetAwaiter().GetResult();

                //お知らせ既読管理登録(送信が失敗してもお知らせには出すので登録する）
                //④ QH_NOTICEGROUPTARGET_DAT へ対象ユーザーと既読管理を登録
                return _noticeGroupRepo.WriteGroupTarget(noticeno, results, entity.STARTDATE, _datetimeProv.Now);
            }

            return false;
        }
        #endregion

        #region "Public Method"

        /// <summary>
        /// JOTOユーザーへプッシュ通知を行う。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoPushSendApiResults Send(QoPushSendApiArgs args)
        {
            QoPushSendApiResults result = new QoPushSendApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            var checkargs = CheckArgs(args);

            if (checkargs.Code == ((int)QoApiResultCodeTypeEnum.ArgumentError).ToString())
            {
                // 引数エラー
                result.Result = checkargs;
                return result;
            }

            try
            {
                int linkageSystemNo = int.Parse(args.LinkageSystemNo);
                DateTime startDate = DateTime.Parse(args.StartDate);

                // Linkageからアカウントキーへ
                var accountKeys = _linkageRepo.ReadAccountKeys(linkageSystemNo, args.LinkageIds);

                if (accountKeys.Count <= 0)
                {
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                    return result;
                }
                foreach(var key in accountKeys) QoAccessLog.WriteInfoLog($"{key}");

                var now = _datetimeProv.Now;

                var reproPushResult = false;
                DateTime.TryParse(QoApiConfiguration.ReproPushEndDate, out DateTime enddate);
                if (enddate > now)//リプロ終了の日程
                {
                    //リプロPushApiを呼び出す
                    reproPushResult = _reproRepo.ReproPushApiCall(args.Contents, accountKeys, startDate, args.DeeplinkUrl);

                    if (!reproPushResult)
                    {
                        QoAccessLog.WriteErrorLog("リプロPushApiの呼び出しに失敗しました。", Guid.Empty);
                    }
                    //QoAccessLog.WriteInfoLog($"リプロPushApiを呼び出し完了");
                }

                var nativePushResult = false;
                DateTime.TryParse(QoApiConfiguration.JotoNativePushStartDate, out DateTime startdate);
                if (startdate < now)//ネイティブ呼び出し開始の設定
                {
                    // ネイティブ用Push通知を呼び出し
                    nativePushResult = GroupNothificartionCall(args, accountKeys);
                    if (!nativePushResult)
                    {
                        QoAccessLog.WriteErrorLog("ネイティブ用Pushの実行に失敗しました。", Guid.Empty);
                    }
                }

                //どちらかが成功してれば成功扱い
                if (reproPushResult || nativePushResult)
                {
                    //ログ登録
                    _reproRepo.InsertPushSend(accountKeys,_datetimeProv.Now);

                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    result.IsSuccess = Boolean.TrueString;
                    QoAccessLog.WriteInfoLog($"処理終了");
                }
                else
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "登録に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return result;
        }


        /// <summary>
        /// JOTOユーザーへプッシュ通知を行う。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoPushSendAccountApiResults SendAccount(QoPushSendAccountApiArgs args)
        {

            QoPushSendAccountApiResults result = new QoPushSendAccountApiResults()
            {
                IsSuccess = bool.FalseString,
                Result = null
            };

            var checkargs = CheckArgs(args);

            if (checkargs.Code == ((int)QoApiResultCodeTypeEnum.ArgumentError).ToString())
            {
                // 引数エラー
                result.Result = checkargs;
                return result;
            }
      
            try
            {
                //アカウントキーのリストを取り出す(変換に失敗したものは無視する）
                var accountKeys = args.Accountkeys.Select(i => i.TryToValueType(Guid.Empty)).Where(i => i != Guid.Empty).ToList();
                DateTime startDate = DateTime.Parse(args.StartDate);

                foreach (var key in accountKeys) QoAccessLog.WriteInfoLog($"{key}");

                var now = _datetimeProv.Now;

                var reproPushResult = false;
                DateTime.TryParse(QoApiConfiguration.ReproPushEndDate, out DateTime enddate);
                if (enddate > now)//リプロ終了の日程
                {
                    //リプロPushApiを呼び出す
                    reproPushResult = _reproRepo.ReproPushApiCall(args.Contents, accountKeys, startDate, args.DeeplinkUrl);

                    if (!reproPushResult)
                    {
                        QoAccessLog.WriteErrorLog("リプロPushApiの呼び出しに失敗しました。", Guid.Empty);
                    }
                    //QoAccessLog.WriteInfoLog($"リプロPushApiを呼び出し完了");
                }

                var nativePushResult = false;
                DateTime.TryParse(QoApiConfiguration.JotoNativePushStartDate, out DateTime startdate);
                if (startdate < now)//ネイティブ呼び出し開始の設定
                {
                    // ネイティブ用Push通知を呼び出し
                    nativePushResult = GroupNothificartionCall(args, accountKeys);
                    if (!nativePushResult)
                    {
                        QoAccessLog.WriteErrorLog("ネイティブ用Pushの実行に失敗しました。", Guid.Empty);
                    }
                }

                //どちらかが成功してれば成功扱い
                if (reproPushResult || nativePushResult)
                {
                    //ログ登録
                    _reproRepo.InsertPushSend(accountKeys, _datetimeProv.Now);

                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    result.IsSuccess = Boolean.TrueString;
                    QoAccessLog.WriteInfoLog($"処理終了");
                }
                else
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "登録に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return result;
        }

        #endregion

    }
}