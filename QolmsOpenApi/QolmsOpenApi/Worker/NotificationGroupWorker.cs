using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Providers;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi
{
    public class NotificationGroupWorker
    {
        #region "インターフェース"

        INoticeGroupRepository _noticeGroupRepo;
        IQoPushNotification _pushNotification;
        IDateTimeProvider _datetimeProv;

        #endregion

        #region "constructor"

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dateTimeProvider"></param>
        public NotificationGroupWorker(INoticeGroupRepository noticeGroupRepo, IQoPushNotification pushNotification, IDateTimeProvider dateTimeProvider)
        {
            _noticeGroupRepo = noticeGroupRepo;
            _pushNotification = pushNotification;
            _datetimeProv = dateTimeProvider;
        }

        #endregion

        #region "Private Method"

        /// <summary>
        /// 複数ユーザーへの並列プッシュ送信
        /// </summary>
        private async Task<List<(Guid acc, string[] pushid)>> SendPushToUsersAsync(
            List<Guid> accountkeys,
            NotificationRequest request,
            string tag)
        {
            var semaphore = new SemaphoreSlim(QoApiConfiguration.NotificationGroupThreads);//設定
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
                        results.Add((acc, result));
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


        private async Task<List<(Guid acc, string[] pushid)>> SendPushAsync(QsDbApplicationTypeEnum toSystemType, long noticeNo, DateTime schedule, QsApiNoticeGroupSendTargetTypeEnum sendTargetType, List<Guid> accountkeys, string title, string message, string url, byte categoryNo)
        {
            _pushNotification = toSystemType.ToNotificationInstance();

            string tag = "Information";
            // CategoryNo = 13 システム通知(予約) の場合 Reservationにタグ変更
            if (categoryNo == 13)
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

            List<(Guid acc, string[] pushid)> results = new List<(Guid acc, string[] pushid)>();
            if (sendTargetType == QsApiNoticeGroupSendTargetTypeEnum.All)
            {
                request.SetTagExpressionJoinAllAnd(new string[] { tag });
                string[] result = await _pushNotification.RequestNotificationAsync(request);
                results.Add((Guid.Empty,result));
            }
            else
            {
                // 並列処理でユーザーごとの通知を送信
                results = await SendPushToUsersAsync(accountkeys, request, tag);
            }

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
                QoAccessLog.WriteErrorLog(string.Format("お知らせPush送信失敗:{0},User={1}", noticeNo, string.Join(",", accountkeys)), Guid.Empty);
                //db更新
                if (!_noticeGroupRepo.UpdateNoticeGroupData(noticeNo, false, null, null))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知失敗しましたが結果の更新に失敗しました:{0}", noticeNo), Guid.Empty);
                return results;
            }
                
            return results;
        }

        /// <summary>
        /// ネイティブ用お知らせ処理
        /// </summary>
        /// <returns></returns>
        private bool GroupNothificartionCall(long noticeno, QsApiNoticeGroupSendTargetTypeEnum sendTargetType, List<Guid> accountKeys ,ref bool isPush)
        {
            QH_NOTICEGROUP_DAT entity　= _noticeGroupRepo.GetNoticeGroup(noticeno);

            if (entity.NOTICENO > 0)
            {
                QsDbApplicationTypeEnum toSystemType = (QsDbApplicationTypeEnum)entity.TOSYSTEMTYPE;

                var sr = new QsJsonSerializer();
                var json = sr.Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
                var deepLink = json.LinkN.Any() ? json.LinkN.First().LinkUrl : string.Empty;

                var result = new List<(Guid acc, string[] pushid)>() { };

                if (entity.PUSHSENDFLAG)
                {
                    result = Task.Run(() => {
                        return SendPushAsync(toSystemType, noticeno, entity.STARTDATE, sendTargetType, accountKeys, entity.TITLE, entity.CONTENTS, deepLink, entity.CATEGORYNO);
                    }).GetAwaiter().GetResult();

                }
                else
                {
                    result = accountKeys.Select(i => (i, new string[0])).ToList();
                }

                return sendTargetType == QsApiNoticeGroupSendTargetTypeEnum.All ? true : _noticeGroupRepo.WriteGroupTarget(noticeno, result, entity.STARTDATE, _datetimeProv.Now);
            }

            return false;
        }

        private static QoApiResultItem CheckArgs(QoNotificationGroupApiArgs args)
        {
            // チェック
            if (!long.TryParse(args.NoticeNo, out long noticeno))
            {
                QoAccessLog.WriteInfoLog($"LinkageSystemNo Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (args.SendTargetType.TryToValueType(QsApiNoticeGroupSendTargetTypeEnum.None) == QsApiNoticeGroupSendTargetTypeEnum.None)
            {
                QoAccessLog.WriteInfoLog($"SendTargetType Error");
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            }

            if (args.SendTargetType.TryToValueType(QsApiNoticeGroupSendTargetTypeEnum.None) != QsApiNoticeGroupSendTargetTypeEnum.All)
            {
                if (!args.TargetAccountkey.Any())
                {
                    QoAccessLog.WriteInfoLog($"TargetAccountkey Error {args.TargetAccountkey.Count}");
                    return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                }
                else if (args.TargetAccountkey.Where(i => i.TryToValueType(Guid.Empty) == Guid.Empty).Any())
                {
                    QoAccessLog.WriteInfoLog($"TargetAccountkey Error {args.TargetAccountkey.Count}");
                    return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                }
            }

            return new QoApiResultItem();
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public QoNotificationGroupApiResults SendGroup(QoNotificationGroupApiArgs args)
        {
            QoNotificationGroupApiResults result = new QoNotificationGroupApiResults()
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
                
            bool isPush = false;

            if (GroupNothificartionCall(
                args.NoticeNo.TryToValueType(long.MinValue),
                args.SendTargetType.TryToValueType(QsApiNoticeGroupSendTargetTypeEnum.None),
                args.TargetAccountkey != null ? args.TargetAccountkey.Select(i => i.TryToValueType(Guid.Empty)).ToList() : new List<Guid>(),
                ref isPush))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                result.IsSuccess = Boolean.TrueString;
                result.IsSendPush = isPush.ToString();
                QoAccessLog.WriteInfoLog($"処理終了");
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "登録に失敗しました。");
            }

            return result;
        }

        #endregion
    }
}