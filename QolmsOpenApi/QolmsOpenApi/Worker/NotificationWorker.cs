using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System.Text.RegularExpressions;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Models;
using Newtonsoft.Json;
using MGF.QOLMS.QolmsOpenApi.Repositories;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// お知らせ送付
    /// </summary>
    public sealed class NotificationWorker
    {
        static NotificationWorker()
        {

        }
        #region "Private Method"
        [Flags()]
        private enum SendTargetOSFlg
        {
            None=0,
            Android=1,
            iOS=2
        }
        //通知対象（OS）を取得
        private static SendTargetOSFlg GetTargetOs(QsDbApplicationTypeEnum toSystemType)
        {
            switch (toSystemType)
            {
                case QsDbApplicationTypeEnum.QolmsiOSApp:
                case QsDbApplicationTypeEnum.JotoiOSApp:
                case QsDbApplicationTypeEnum.CcciOSApp:
                case QsDbApplicationTypeEnum.KagaminoiOSApp:
                case QsDbApplicationTypeEnum.TisiOSApp:
                case QsDbApplicationTypeEnum.QolmsNaviiOsApp:
                case QsDbApplicationTypeEnum.HealthDiaryiOSApp:
                case QsDbApplicationTypeEnum.MeiNaviiOSApp:
                    return SendTargetOSFlg.iOS;
                case QsDbApplicationTypeEnum.QolmsAndroidApp:
                case QsDbApplicationTypeEnum.JotoAndroidApp:
                case QsDbApplicationTypeEnum.CccAndroidApp:
                case QsDbApplicationTypeEnum.KagaminoAndroidApp:
                case QsDbApplicationTypeEnum.TisAndroidApp:
                case QsDbApplicationTypeEnum.QolmsNaviAndroidApp:
                case QsDbApplicationTypeEnum.HealthDiaryAndroidApp:
                case QsDbApplicationTypeEnum.MeiNaviAndroidApp:
                    return SendTargetOSFlg.Android;
                case QsDbApplicationTypeEnum.QolmsTisApp:
                    return SendTargetOSFlg.Android | SendTargetOSFlg.iOS;
                case QsDbApplicationTypeEnum.QolmsNaviApp:
                    return SendTargetOSFlg.Android | SendTargetOSFlg.iOS;
                case QsDbApplicationTypeEnum.HealthDiaryApp:
                    return SendTargetOSFlg.Android | SendTargetOSFlg.iOS;
                case QsDbApplicationTypeEnum.MeiNaviApp:
                    return SendTargetOSFlg.Android | SendTargetOSFlg.iOS;
                default:
                    return SendTargetOSFlg.None;
            }
        }        
        
        /// <summary>
        /// メールアドレス取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        private static string GetMailAddress(Guid accountKey)
        {
            QH_PASSWORDMANAGEMENT_DAT entity = new QH_PASSWORDMANAGEMENT_DAT() { ACCOUNTKEY = accountKey };
            QhPasswordManagementEntityReaderResults results = QsDbManager.Read(new QhPasswordManagementEntityReader(), new QhPasswordManagementEntityReaderArgs() { Data = new List<QH_PASSWORDMANAGEMENT_DAT>() { entity } });
            if(results.IsSuccess && results.Result != null && results.Result.Count==1)
            {
                string encMail = results.Result.FirstOrDefault().PASSWORDRECOVERYMAILADDRESS;
                if (string.IsNullOrWhiteSpace(encMail) == false)
                {
                   using(QsCrypt crypt =new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                   {
                        return crypt.DecryptString(encMail);
                   }
                }
            }
            QoAccessLog.WriteErrorLog("メールアドレス取得に失敗しました。", accountKey);
            return string.Empty;
        }
         
        //アプリケーションタイプとアカウントキー(子アカウントからアプリ用の通知UserId）を返す
        private static  string  GetUserId(QsDbApplicationTypeEnum toSystemType, Guid accountKey)
        {
            int linkageSystemNo = toSystemType.ToLinkageSystemNo();
            if (!linkageSystemNo.IsNaviApp())
            {
                // 医療ナビ系以外は親アカウントキーを通知IDとする
                var repo = new AccountRepository();
                // 親アカウントの取得(親が本人なら本人の)
                var entity = repo.ReadParentOrSelfMasterEntiry(accountKey);
                if(entity == null)
                {
                    return string.Empty;
                }

                return entity.ACCOUNTKEY.ToEncrypedReference();
            }

            // 医療ナビ系はLinkageDatの方に保持しているIDを取得
            var result = QsDbManager.Read(new FamilyParentAccountReader(), new FamilyParentAccountReaderArgs()
            {
                AccountKey = accountKey,
                LinkageSystemNo = linkageSystemNo
            });
            if (result.IsSuccess)
            {               
                return  result.LinkageId.ToEncrypedReference();
            }

            return  string.Empty;
        }
        
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationImageReadApiArgs args)
        {
            if (args.FileType.TryToValueType(QsApiFileTypeEnum.None)==QsApiFileTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
            if(string.IsNullOrWhiteSpace(args.FileKeyReference))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FileKeyReferenceが指定されていません");
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationPersonalReadReceiptWriteApiArgs args)
        {
            if(args.NoticeNo.TryToValueType(long.MinValue) <0)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
           
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationReadApiArgs args)
        {
            DateTime start = args.FromDate.TryToValueType(DateTime.Now);
            DateTime end = args.ToDate.TryToValueType(DateTime.Now);
            if (start > end)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "期間指定が不正です。");

            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationPersonalReadApiArgs args)
        {
            DateTime start = args.FromDate.TryToValueType(DateTime.Now);
            DateTime end = args.ToDate.TryToValueType(DateTime.Now);
            if (start > end)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "期間指定が不正です。");


            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationSendApiArgs args)
        {
            //対象の設定があるか
            if (args.TargetAccountKey.TryToValueType(Guid.Empty) == Guid.Empty 
                || args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None )== QsDbApplicationTypeEnum.None )
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
            if (args.FromSystemType.TryToValueType(QsDbApplicationTypeEnum.None) == QsDbApplicationTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FromSystemTypeを指定してください");
            if (string.IsNullOrWhiteSpace(args.Title) && string.IsNullOrWhiteSpace(args.Contents))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "内容がありません");
            


            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationSendAllApiArgs args)
        {
            //対象の設定があるか
            if ( args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None) == QsDbApplicationTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
            if (args.FromSystemType.TryToValueType(QsDbApplicationTypeEnum.None) == QsDbApplicationTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FromSystemTypeを指定してください");
            if (string.IsNullOrWhiteSpace(args.Title) && string.IsNullOrWhiteSpace(args.Contents))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "内容がありません");
            

            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }

        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoNotificationSendFromPatientIdApiArgs args)
        {
            //対象の設定があるか
            if (args.TargetAccountKey.TryToValueType(Guid.Empty) == Guid.Empty
                || args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None) == QsDbApplicationTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
            if (args.FromSystemType.TryToValueType(QsDbApplicationTypeEnum.None) == QsDbApplicationTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FromSystemTypeを指定してください");
            if (string.IsNullOrWhiteSpace(args.Title) && string.IsNullOrWhiteSpace(args.Contents))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "内容がありません");



            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //スケジュールプッシュキャンセル
        private static async Task SendPushCancel(QsDbApplicationTypeEnum toSystemType, string[] chancelId)
        {
            QoPushNotification _notificationHubService = toSystemType.ToNotificationInstance();
            await  _notificationHubService.RequestScheduleChancelAsync(chancelId);
        }
        //個人向けPush
        private static async Task<bool> SendPushNotification(QsDbApplicationTypeEnum toSystemType, long noticeNo,DateTime schedule, string userId ,  Guid targetAccountKey, string title,string message, string url, string categoryNo)
        {
            QoPushNotification _notificationHubService = toSystemType.ToNotificationInstance();

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
                Url =url,
                Badge = 1,
                Title = title,
                ScheduleDate = schedule
            };
            request.SetTagExpressionJoinAllAnd(new string[] { tag,  "$UserId:{" + userId + "}" });

            QoAccessLog.WriteAccessLog(QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SendPush_For_PersonalNotification",
                                $"個人お知らせ通知送信準備: {request.Title} {request.Text} {request.TagExpression} {request.ScheduleDate}", null, null, null);

            string[] result = await _notificationHubService.RequestNotificationAsync(request).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(result[0]) || !string.IsNullOrEmpty(result[1]))
            {
                // QoAccessLog.WriteInfoLog(string.Format("お知らせPush送信成功:No={0},User={1}", noticeNo, userId ));
                //db更新

                QoAccessLog.WriteAccessLog(QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SendPush_For_PersonalNotification",
                                $"個人お知らせ通知送信成功: NotificationId:{result[0]}/{result[1]} {request.Title} {request.Text} {request.TagExpression} {request.ScheduleDate}", null, null, null);

                if (!SaveNoticePersonalData(noticeNo,true, null, result))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知しましたが結果の更新に失敗しました:{0}", noticeNo), Guid.Empty);

                return true;
            }
            else
            {
                QoAccessLog.WriteErrorLog(string.Format("お知らせPush送信失敗:{0},User={1}", noticeNo,userId), Guid.Empty);
                //db更新
                if (!SaveNoticePersonalData(noticeNo, false, null,null))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知失敗しましたが結果の更新に失敗しました:{0}", noticeNo), Guid.Empty);
                return false;
            }
            
        }
        //子施設のリストを取得する
        private static List<QH_FACILITY_MST> GetFacilityList(QsDbApplicationTypeEnum toSystemType)
        {
            var linkageSystemNo = toSystemType.ToLinkageSystemNo();
            var reader = new LinkageFacilityListReader();
            var readerArgs = new LinkageFacilityListReaderArgs() { LinkageSystemNo = linkageSystemNo };
            try
            {
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                {
                    return readerResults.Result;
                }
                else
                {
                    QoAccessLog.WriteErrorLog(string.Format("[GetFacilityList]施設情報の取得に失敗しました。LinkageSystemNo:{0}", linkageSystemNo), Guid.Empty);
                }
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return null;
        }
        //Linkage施設Keyを取得する
        private static Guid GetFacilityKey(QsDbApplicationTypeEnum toSystemType)
        {
            var linkageSystemNo = toSystemType.ToLinkageSystemNo();
            var reader = new QhLinkageSystemEntityReader();
            var readerArgs = new QhLinkageSystemEntityReaderArgs() { Data =new List<QH_LINKAGESYSTEM_MST>() { new QH_LINKAGESYSTEM_MST() { LINKAGESYSTEMNO = linkageSystemNo } } };
            try
            {
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                {
                    return readerResults.Result.FirstOrDefault().FACILITYKEY  ;
                }
                else
                {
                    QoAccessLog.WriteErrorLog(string.Format("[GetFacilityKey]施設情報の取得に失敗しました。LinkageSystemNo:{0}", linkageSystemNo), Guid.Empty);
                }
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return Guid.Empty;
        }
        //対象施設関連全体Push
        private static async Task<bool> SendPushAllNotification(QsDbApplicationTypeEnum toSystemType, long noticeNo, DateTime pushDate, Guid facilityKey, string title, string message)
        {
            QoPushNotification _notificationHubService = toSystemType.ToNotificationInstance();
            SendTargetOSFlg targetOs = GetTargetOs(toSystemType);

            //            var tags = new List<string>() { "Information", string.Format("Facility_{0}",facilityKey.ToEncrypedReference()) };
            var facilityList = GetFacilityList(toSystemType);
            
            var request = new NotificationRequest
            {
                Extra =  "",
                Silent = false,
                Text = message ,
                Url = string.Format("{0}info?no={1}", toSystemType.ToAppUrlScheme(), noticeNo),
                Badge = 1,
                ScheduleDate =pushDate,
                Title = title 
            };
            if(facilityList!=null && facilityList.Any(m=>m.FACILITYKEY ==facilityKey))
                request.SetTagExpressionJoinAllAnd(new string[]{ "Information", string.Format("Facility_{0}", facilityKey.ToEncrypedReference())});
            else
                request.SetTagExpressionJoinAllAnd(new string[]{ "Information"});

            string[] result;
            if (targetOs.HasFlag(SendTargetOSFlg.Android) && targetOs.HasFlag(SendTargetOSFlg.iOS))
                result = await _notificationHubService.RequestNotificationAsync(request);//.ConfigureAwait(false);
            else if (targetOs.HasFlag(SendTargetOSFlg.Android))
                result = new string[] { await _notificationHubService.RequestAndroidNotificationAsync(request) , string.Empty};
            else if (targetOs.HasFlag(SendTargetOSFlg.iOS))
                result = new string[] {string.Empty, await _notificationHubService.RequestiOsNotificationAsync(request) };
            else
                result = new string[] { string.Empty, string.Empty };
            if (!string.IsNullOrEmpty(result[0]) || !string.IsNullOrEmpty(result[1]) )
            {
                //QoAccessLog.WriteInfoLog(string.Format("お知らせPush送信成功:{0}", noticeNo));
                //db更新
                if (!SaveNoticeData(noticeNo, true, result))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知しましたが結果の更新に失敗しました:{0}", noticeNo), Guid.Empty);

                return true;
            }
            else
            {
                QoAccessLog.WriteErrorLog(string.Format("お知らせPush送信失敗:{0}", noticeNo), Guid.Empty);
                    
                //db更新
                if (!SaveNoticeData(noticeNo, false, null))
                    QoAccessLog.WriteErrorLog(string.Format("Push通知失敗しましたが結果の更新に失敗しました:{0}", noticeNo),Guid.Empty);
                
                return false;
            }

        }
        
        //個人向けお知らせ情報を更新
        private static bool SaveNoticePersonalData(long noticeNo, Nullable<bool> pushSend, Nullable<bool> mailSend, string[] noticeId)
        {
            QhNoticePersonalEntityReaderArgs readerArgs = new QhNoticePersonalEntityReaderArgs() { Data = new List<QH_NOTICEPERSONAL_DAT>() { new QH_NOTICEPERSONAL_DAT { NOTICENO = noticeNo } } };
            QhNoticePersonalEntityReaderResults readerResults = QsDbManager.Read(new QhNoticePersonalEntityReader(), readerArgs);
            if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
            {
                QH_NOTICEPERSONAL_DAT entity = readerResults.Result.FirstOrDefault();
                if(pushSend.HasValue)
                    entity.PUSHSENDFLAG = (bool)pushSend;
                if(mailSend.HasValue)
                    entity.MAILSENDFLAG = (bool)mailSend;
                var serialiser = new QsJsonSerializer();
                if (noticeId != null && noticeId.Any())
                {
                    QhNoticeDataSetOfJson json = serialiser.Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
                    if (json == null)
                        json = new QhNoticeDataSetOfJson() { AttachedFileN = new List<QhAttachedFileOfJson>(), LinkN = new List<QhNoticeLinkItemOfJson>(), PushSendN = new List<QhNoticePushSendItemOfJson>() };
                    foreach (var item in noticeId)
                    {
                        if (!string.IsNullOrEmpty(item))
                            json.PushSendN.Add(new QhNoticePushSendItemOfJson() { PushDate = entity.STARTDATE.ToApiDateString(), PushId = item });
                    }
                    entity.NOTICEDATASET = serialiser.Serialize(json);
                }
                entity.UPDATEDDATE = DateTime.Now;
                QhNoticePersonalEntityWriterArgs writerArgs = new QhNoticePersonalEntityWriterArgs() { Data = new List<QH_NOTICEPERSONAL_DAT>() { entity } };
                QhNoticePersonalEntityWriterResults writerResults = QsDbManager.Write(new QhNoticePersonalEntityWriter(), writerArgs);
                return writerResults.IsSuccess;
            }
            return false;

        }
        //個人向けお知らせ情報を保存
        private static long SaveNoticePersonalData(QoNotificationSendApiArgs args, Guid targetAccountKey)
        {
            Guid authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            long noticeNo = args.NoticeNo.TryToValueType(long.MinValue);
            if (noticeNo < 0)
                noticeNo = long.MinValue;   //仮に-1とかを入れてきたとしてもおそらく新規の意図なのでlong.MinValueに指定しなおしてやる

            //test
            if(authorKey==Guid.Empty)
                authorKey = Guid.Parse("1CC67806-913A-44C8-809E-F6B92F6764A9");

            QhNoticeDataSetOfJson json = new QhNoticeDataSetOfJson()
            {
                AttachedFileN = new List<QhAttachedFileOfJson>(),
                LinkN = new List<QhNoticeLinkItemOfJson>(),
                PushSendN =new List<QhNoticePushSendItemOfJson>(),
                SenderDispFlag = args.SenderDispFlag
            };
            var writerArgs = new DbNoticePersonalWriterArgs()
            {
                AuthorKey = authorKey,
                DeleteFlag = false,
                NoticeData = new QH_NOTICEPERSONAL_DAT()
                {
                    ACCOUNTKEY= targetAccountKey,
                    CATEGORYNO = args.CategoryNo.TryToValueType(byte.MinValue),
                    CONTENTS = args.Contents,
                    CREATEDACCOUNTKEY = authorKey,
                    CREATEDDATE = DateTime.Now,
                    DELETEFLAG = false,
                    ENDDATE = args.EndDate.TryToValueType(DateTime.MaxValue),
                    STARTDATE = args.StartDate.TryToValueType(DateTime.MinValue),
                    FACILITYKEY = args.FromFacilityKey.TryToValueType(Guid.Empty),
                    FROMSYSTEMTYPE = args.FromSystemType.TryToValueType(byte.MinValue),
                    NOTICEDATASET = new QsJsonSerializer().Serialize( json),
                    NOTICENO = noticeNo,
                    PRIORITYNO = args.PriorityNo.TryToValueType<byte>(3),
                    PUSHSENDFLAG = args.IsPush.TryToValueType(false),
                    MAILSENDFLAG =args.IsMail.TryToValueType(false),
                    TITLE = args.Title,
                    TOSYSTEMTYPE = args.TargetSystemType.TryToValueType(byte.MinValue),
                    UPDATEDACCOUNTKEY = authorKey,
                    UPDATEDDATE = DateTime.Now
                }
            };
            var readerResults = QsDbManager.Write(new DbNoticePersonalWriter(), writerArgs);
            if (readerResults.IsSuccess && readerResults != null)
                return readerResults.NoticeNo;

            return long.MinValue;
        }

        //個人向けお知らせ情報を保存
        private static long SaveNoticePersonalData(QoNotificationSendFromPatientIdApiArgs args, Guid targetAccountKey)
        {
            Guid authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            long noticeNo = args.NoticeNo.TryToValueType(long.MinValue);
            if (noticeNo < 0)
                noticeNo = long.MinValue;   //仮に-1とかを入れてきたとしてもおそらく新規の意図なのでlong.MinValueに指定しなおしてやる

            //test
            if (authorKey == Guid.Empty)
                authorKey = Guid.Parse("1CC67806-913A-44C8-809E-F6B92F6764A9");

            QhNoticeDataSetOfJson json = new QhNoticeDataSetOfJson()
            {
                AttachedFileN = new List<QhAttachedFileOfJson>(),
                LinkN = new List<QhNoticeLinkItemOfJson>(),
                PushSendN = new List<QhNoticePushSendItemOfJson>()
            };
            var writerArgs = new DbNoticePersonalWriterArgs()
            {
                AuthorKey = authorKey,
                DeleteFlag = false,
                NoticeData = new QH_NOTICEPERSONAL_DAT()
                {
                    ACCOUNTKEY = targetAccountKey,
                    CATEGORYNO = args.CategoryNo.TryToValueType(byte.MinValue),
                    CONTENTS = args.Contents,
                    CREATEDACCOUNTKEY = authorKey,
                    CREATEDDATE = DateTime.Now,
                    DELETEFLAG = false,
                    ENDDATE = args.EndDate.TryToValueType(DateTime.MaxValue),
                    STARTDATE = args.StartDate.TryToValueType(DateTime.MinValue),
                    FACILITYKEY = args.FromFacilityKey.TryToValueType(Guid.Empty),
                    FROMSYSTEMTYPE = args.FromSystemType.TryToValueType(byte.MinValue),
                    NOTICEDATASET = new QsJsonSerializer().Serialize(json),
                    NOTICENO = noticeNo,
                    PRIORITYNO = args.PriorityNo.TryToValueType<byte>(3),
                    PUSHSENDFLAG = args.IsPush.TryToValueType(false),
                    MAILSENDFLAG = args.IsMail.TryToValueType(false),
                    TITLE = args.Title,
                    TOSYSTEMTYPE = args.TargetSystemType.TryToValueType(byte.MinValue),
                    UPDATEDACCOUNTKEY = authorKey,
                    UPDATEDDATE = DateTime.Now
                }
            };
            var readerResults = QsDbManager.Write(new DbNoticePersonalWriter(), writerArgs);
            if (readerResults.IsSuccess && readerResults != null)
                return readerResults.NoticeNo;

            return long.MinValue;
        }

        //お知らせ情報を更新
        private static bool SaveNoticeData(long noticeNo, bool pushSend, string[] noticeId)
        {
            QhNoticeEntityReaderArgs readerArgs = new QhNoticeEntityReaderArgs() { Data = new List<QH_NOTICE_DAT>() { new QH_NOTICE_DAT {NOTICENO=noticeNo } } };
            QhNoticeEntityReaderResults readerResults= QsDbManager.Read(new QhNoticeEntityReader(), readerArgs);
            if(readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count ==1)
            {
                QH_NOTICE_DAT entity = readerResults.Result.FirstOrDefault();
                var serialiser = new QsJsonSerializer();
                if (noticeId!=null && noticeId.Any())
                {
                    QhNoticeDataSetOfJson json = serialiser.Deserialize< QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
                    foreach (var item in noticeId)
                    {
                        if (!string.IsNullOrEmpty(item))
                            json.PushSendN.Add(new QhNoticePushSendItemOfJson() { PushDate = entity.STARTDATE.ToApiDateString(), PushId = item });
                    }
                    entity.NOTICEDATASET = serialiser.Serialize(json);
                }
                entity.PUSHSENDFLAG = pushSend;
                entity.UPDATEDDATE = DateTime.Now;
                QhNoticeEntityWriterArgs writerArgs= new QhNoticeEntityWriterArgs() { Data = new List<QH_NOTICE_DAT>() { entity } };
                QhNoticeEntityWriterResults writerResults = QsDbManager.Write(new QhNoticeEntityWriter(), writerArgs);
                return writerResults.IsSuccess;
            }
            return false;

        }
        //お知らせ情報を保存
        private static long SaveNoticeData(QoNotificationSendAllApiArgs args)
        {
            Guid authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            long noticeNo = args.NoticeNo.TryToValueType(long.MinValue);
            if (noticeNo < 0)
                noticeNo = long.MinValue;   //仮に-1とかを入れてきたとしてもおそらく新規の意図なのでlong.MinValueに指定しなおしてやる

            //test
            if(authorKey==Guid.Empty)
                authorKey = Guid.Parse("1CC67806-913A-44C8-809E-F6B92F6764A9");

            QhNoticeDataSetOfJson json = new QhNoticeDataSetOfJson()
            {
                AttachedFileN = args.FileKeyN.ConvertAll(m => { return new QhAttachedFileOfJson() { FileKey = m }; }),
                LinkN = args.LinkN.ConvertAll(n => { return new QhNoticeLinkItemOfJson() { LinkText = n.Title, LinkUrl = n.Url }; }),
                PushSendN =args.PushN.ConvertAll(l=> { return new QhNoticePushSendItemOfJson() { PushDate = l.PushDate, PushId = l.PushId }; })
            };
            var writerArgs = new DbNoticeListWriterArgs()
            {
                AuthorKey = authorKey,
                DeleteFlag = false,
                NoticeData = new QH_NOTICE_DAT() {
                    CATEGORYNO = args.CategoryNo.TryToValueType(byte.MinValue),
                    CONTENTS = args.Contents,
                    CREATEDACCOUNTKEY = authorKey,
                    CREATEDDATE = DateTime.Now,
                    DELETEFLAG = false,
                    ENDDATE =args.EndDate.TryToValueType(DateTime.MaxValue),
                    STARTDATE=args.StartDate.TryToValueType(DateTime.MinValue),
                    FACILITYKEY =args.TargetFacilityKey.TryToValueType(Guid.Empty),
                    FROMSYSTEMTYPE =args.FromSystemType.TryToValueType(byte.MinValue),
                    NOTICEDATASET =new QsJsonSerializer().Serialize(json),
                    NOTICENO = noticeNo,
                    PRIORITYNO =args.PriorityNo.TryToValueType<byte>(3),
                    PUSHSENDFLAG = args.IsPush.TryToValueType<bool>(false),
                    SHOWANONYMOUSFLAG =false,
                    TITLE =args.Title ,
                    TOSYSTEMTYPE =args.TargetSystemType.TryToValueType(byte.MinValue),
                    UPDATEDACCOUNTKEY =authorKey,
                    UPDATEDDATE =DateTime.Now
                }
            };
            var readerResults = QsDbManager.Write(new DbNoticeListWriter(), writerArgs );
            if(readerResults.IsSuccess && readerResults !=null)
                return readerResults.NoticeNo;

            return long.MinValue;
        }
        
        //お知らせリスト取得
        private static (List<QoApiNoticeItem>, int,int) GetNoticeData(QsApiSystemTypeEnum systemType,
            Guid accountKey,
            List<byte>categoryNoList, List<Guid>facilityKeyList,DateTime startDate, DateTime endDate,
            int pageIndex, int pageSize)
        {
            List<QoApiNoticeItem> results = new List<QoApiNoticeItem>();
            int maxPageIndex = 0;
            int resultPageIndex = 0;
            List<byte> toSystemType = systemType.ToApplicationTypeByteList();
            DbNoticeListReaderArgs args = new DbNoticeListReaderArgs()
            {
                AccountKey =accountKey,
                ToSystemTypeList = toSystemType,
                PageIndex = pageIndex,
                PageSize = pageSize,
                NoticeStartDate = startDate,
                NoticeEndDate = endDate,
                FacilityKeyN = facilityKeyList,
                FilterFlag =  QsDbNoticeFilterTypeEnum.ToSystem | QsDbNoticeFilterTypeEnum.Facility | QsDbNoticeFilterTypeEnum.NoticeDate
            };
            if (categoryNoList.Any())
            {
                args.CategoryNoN = categoryNoList;
                args.FilterFlag = args.FilterFlag | QsDbNoticeFilterTypeEnum.CategoryNo;
            }
            try
            {
                var readerResult = QsDbManager.Read(new DbNoticeListReader(), args);
                if (readerResult.IsSuccess && readerResult.NoticeListN != null)
                {
                    maxPageIndex = readerResult.MaxPageIndex;
                    resultPageIndex = readerResult.PageIndex;
                    foreach (var item in readerResult.NoticeListN)
                    {
                        results.Add(ToApiNoticeItem(item));
                    }
                };
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, accountKey);
            }
            
            
            return (results,maxPageIndex,resultPageIndex);
        }
        //DB Entity からAPIの戻り値への変換
        private static QoApiNoticeItem ToApiNoticeItem(QH_NOTICE_DAT entity)
        {
            QhNoticeDataSetOfJson dataset = new QsJsonSerializer().Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
            List<QoApiFileKeyItem> files = new List<QoApiFileKeyItem>();
            if (dataset != null && dataset.AttachedFileN != null)
            {
                int sq = 0;
                foreach (var item in dataset.AttachedFileN)
                {
                    files.Add(new QoApiFileKeyItem() { Sequence = sq.ToString(), FileKeyReference = item.FileKey.ToEncrypedReference() });
                    sq++;
                }
            }
            List<QoApiLinkItem> linkes = new List<QoApiLinkItem>();
            if (dataset != null && dataset.LinkN != null)
            {
                foreach (var item in dataset.LinkN)
                    linkes.Add(new QoApiLinkItem() { Title = item.LinkText, Url = item.LinkUrl });
            }
            return new QoApiNoticeItem()
            {
                CategoryNo = entity.CATEGORYNO.ToString(),
                Contents = entity.CONTENTS,
                EndDate = entity.ENDDATE.ToApiDateString(),
                FacilityKeyReference = entity.FACILITYKEY.ToEncrypedReference(),
                FileKeyN = files,
                LinkN = linkes,
                NoticeNo = entity.NOTICENO.ToString(),
                PriorityNo = entity.PRIORITYNO.ToString(),
                StartDate = entity.STARTDATE.ToApiDateString(),
                Title = entity.TITLE
            };
            
        }
        //お知らせ取得
        private static QoApiNoticeItem GetNoticeData(long noticeNo)
        {
            var readerResults = QsDbManager.Read(new QhNoticeEntityReader(), new QhNoticeEntityReaderArgs() { Data = new List<QH_NOTICE_DAT>() { new QH_NOTICE_DAT() { NOTICENO = noticeNo } } });
            if(readerResults !=null && readerResults.IsSuccess && readerResults.Result.Count == 1)
            {
                return ToApiNoticeItem(readerResults.Result.FirstOrDefault());
            }
            return null;
        }

        //実際にプッシュ通知できる状態かどうかチェック（過去にプッシュ済みをはじく、スケジュールをキャンセルする）
        private static bool CheckPush(long noticeNo,  bool isPush, out List<string> cancelNoticeId)
        {
            cancelNoticeId = new List<string>();
            if (noticeNo < 0)
                return isPush;
            var readerResults = QsDbManager.Read(new QhNoticeEntityReader(), new QhNoticeEntityReaderArgs() { Data = new List<QH_NOTICE_DAT>() { new QH_NOTICE_DAT() { NOTICENO = noticeNo } } });
            if (readerResults != null && readerResults.IsSuccess && readerResults.Result.Count == 1)
            {
                var entity = readerResults.Result.FirstOrDefault();
                if (!entity.PUSHSENDFLAG)
                {
                    // 編集前のお知らせがプッシュ通知OFFの場合 プッシュ通知可能
                    return isPush;
                }
                else
                {
                    //過去にPush済みの場合
                    if (entity.STARTDATE < DateTime.Now)
                        return false;   //何もしないで
                    else
                    {//過去にPushしてるけどスケジュールプッシュらしい場合
                        //Cancel依頼用のリストを入れて返す
                        QhNoticeDataSetOfJson json = new QsJsonSerializer().Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
                        foreach (var item in json.PushSendN)
                        {
                            if (item.PushDate.TryToValueType(DateTime.MinValue) > DateTime.Now)
                                cancelNoticeId.Add(item.PushId);
                        }
                        return isPush;
                    }
                }
            }
            return isPush;
        }
        //FacilityKeyリスト取得(所属＋Empty）
        private static List<Guid> GetUserFacilityKeyList(Guid accountKey)
        {
            List<Guid> results =new List<Guid>() { Guid.Empty };
            var readerResults = QsDbManager.Read(new LinkagePatientCardReader(), new LinkagePatientCardReaderArgs() { ActorKey = accountKey, AuthorKey = accountKey });
            if (readerResults.IsSuccess)
            {
                if (readerResults.PatientCardItemN == null)
                    return results;

                List<MGF.QOLMS.QolmsOpenApi.Sql.DbPatientCardItem> cardList = readerResults.PatientCardItemN.Where(n => n.StatusType == 2).ToList();
                
                if (cardList.Count > 0)
                {
                    foreach (var item in cardList)
                    {
                        results.Add(item.FacilityKey);
                    }
                    
                }
            }
            return results;
        }

        //家族アカウントキーリスト取得
        private static List<Guid> GetAccountKeyList(Guid publicAccountKey)
        {
            var entityList = AccountFamilyWorker.ReadAccountFamily(publicAccountKey);
            var result = entityList.Select(m=>m.ACCOUNTKEY).ToList();
            result.Add(publicAccountKey);
            return result;
        }
        
        //個人あてお知らせリスト取得
        private static (List<QoApiNoticeItem>, int, int) GetNoticePersonalData(QsApiSystemTypeEnum systemType,List<Guid> accountKeyList, 
            DateTime startDate, DateTime endDate, bool onlyUnread,
            int pageIndex, int pageSize, byte category)
        {
            List<QoApiNoticeItem> results = new List<QoApiNoticeItem>();
            int maxPageIndex = 0;
            int resultPageIndex = 0;
            List<byte> toSystemType = systemType.ToApplicationTypeByteList();
            DbNoticePersonalListReaderArgs args = new DbNoticePersonalListReaderArgs()
            {
                ToSystemTypeList = toSystemType,
                PageIndex = pageIndex,
                PageSize = pageSize,
                NoticeStartDate = startDate,
                NoticeEndDate = endDate,     
                AccountKeyN  = accountKeyList,                
                FilterFlag = QsDbNoticeFilterTypeEnum.ToSystem | QsDbNoticeFilterTypeEnum.AccountKey | QsDbNoticeFilterTypeEnum.NoticeDate
            };
            if (onlyUnread) //未読のみ指定
            {
                args.IsAlreadyRead = false;
                args.FilterFlag |= QsDbNoticeFilterTypeEnum.AlreadyRead;
            }
            // カテゴリーが指定されている場合
            if(category != byte.MinValue)
            {
                args.CategoryNo = category;
                args.FilterFlag |= QsDbNoticeFilterTypeEnum.CategoryNo;
            }
            
            var readerResult = QsDbManager.Read(new DbNoticePersonalListReader(), args);

            if (readerResult.IsSuccess && readerResult.NoticeListN != null)
            {
                maxPageIndex = readerResult.MaxPageIndex;
                resultPageIndex = readerResult.PageIndex;
                foreach (var item in readerResult.NoticeListN)
                {
                    results.Add(new QoApiNoticeItem()
                    {
                        CategoryNo = item.Category.ToString(),
                        Contents = item.Contents,
                        EndDate = item.EndDate.ToApiDateString(),
                        FacilityKeyReference = item.FacilityKey.ToEncrypedReference(),
                        NoticeNo = item.NoticeNo.ToString(),
                        PriorityNo = item.Priority.ToString(),
                        StartDate = item.StartDate.ToApiDateString(),
                        Title = item.Title,
                        ReadFlag =item.AlreadyReadFlag.ToString(),
                        TargetAccountKeyReference =item.AccountKey.ToEncrypedReference()
                    });
                }
            };
            return (results, maxPageIndex, resultPageIndex);
        }
        //お知らせ取得
        private static QoApiNoticeItem GetNoticePersonalData(long noticeNo)
        {
            var readerResults = QsDbManager.Read(new QhNoticePersonalEntityReader(), new QhNoticePersonalEntityReaderArgs() { Data = new List<QH_NOTICEPERSONAL_DAT>() { new QH_NOTICEPERSONAL_DAT() { NOTICENO = noticeNo } } });
            if (readerResults != null && readerResults.IsSuccess && readerResults.Result.Count == 1)
            {
                var entity = readerResults.Result.FirstOrDefault();
                return new QoApiNoticeItem()
                {
                    CategoryNo = entity.CATEGORYNO.ToString(),
                    Contents = entity.CONTENTS,
                    EndDate = entity.ENDDATE.ToApiDateString(),
                    FacilityKeyReference = entity.FACILITYKEY.ToEncrypedReference(),
                    NoticeNo = entity.NOTICENO.ToString(),
                    PriorityNo = entity.PRIORITYNO.ToString(),
                    StartDate = entity.STARTDATE.ToApiDateString(),
                    ReadFlag = GetAlredyReadFlag(noticeNo).ToString(),
                    TargetAccountKeyReference =entity.ACCOUNTKEY.ToEncrypedReference(),
                    Title = entity.TITLE
                };
            }
            return null;
        }
        //実際にプッシュ通知できる状態かどうかチェック（過去にプッシュ済みをはじく、スケジュールをキャンセルする）
        private static (bool push, bool mail) CheckNoticePersonalPushAndMail(long noticeNo, bool isPush, bool isMail, out List<string> cancelNoticeId)
        {
            (bool push, bool mail) result =(isPush,isMail);
            
            cancelNoticeId = new List<string>();
            if (noticeNo < 0)
                return result ;
            var readerResults = QsDbManager.Read(new QhNoticePersonalEntityReader(), new QhNoticePersonalEntityReaderArgs() { Data = new List<QH_NOTICEPERSONAL_DAT>() { new QH_NOTICEPERSONAL_DAT() { NOTICENO = noticeNo } } });
            if (readerResults != null && readerResults.IsSuccess && readerResults.Result.Count == 1)
            {
                var entity = readerResults.Result.FirstOrDefault();
               
                if (entity.MAILSENDFLAG)
                    result.mail = false;
                
                if(entity.PUSHSENDFLAG )
                {
                    //過去にPush済みの場合
                    if (entity.STARTDATE < DateTime.Now)
                        result.push= false;   //何もしないで
                    else
                    {//過去にPushしてるけどスケジュールプッシュらしい場合
                        //Cancel依頼用のリストを入れて返す
                        QhNoticeDataSetOfJson json = new QsJsonSerializer().Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
                        foreach (var item in json.PushSendN)
                        {
                            if (item.PushDate.TryToValueType(DateTime.MinValue) > DateTime.Now)
                                cancelNoticeId.Add(item.PushId);
                        }
                    }
                }
            }
            return result;
        }
        //既読フラグ取得
        private static bool GetAlredyReadFlag(long noticeNo)
        {
            var result = QsDbManager.Read(new QhNoticePersonalAlreadyReadEntityReader(),
                new QhNoticePersonalAlreadyReadEntityReaderArgs() { Data = new List<QH_NOTICEPERSONALALREADYREAD_DAT>() { new QH_NOTICEPERSONALALREADYREAD_DAT() { NOTICENO = noticeNo } } });
            if (result.IsSuccess && result.Result.Count ==1 )
                return result.Result.FirstOrDefault().ALREADYREADFLAG;
            return false;
        }
        /// <summary>
        /// お知らせメールを送信します。
        /// </summary>
        /// <param name="settingsname"></param>
        /// <param name="mailAddress"></param>
        /// <param name="bodyPath"></param>
        /// <param name="footerPath"></param>
        /// <param name="noticeNo"></param>
        /// <param name="accountKey"></param>
        /// <param name="urlScheme"></param>
        /// <returns></returns>
        private static  async Task<bool> SendPersonalMessageNoticeMail(string settingsName, string mailAddress,string bodyPath, string footerPath,long noticeNo,Guid accountKey, string urlScheme)
        {
                        
            QoAccessLog.WriteInfoLog(bodyPath);

            bool res =await new PersonalMessageNoticeClient(new PersonalMessageNoticeClientArgs(settingsName,mailAddress, urlScheme, bodyPath, footerPath)).SendAsync();
            if (!res)
                SaveNoticePersonalData(noticeNo, null, res,null);
            return res;
        }

        //患者ID、LinkageSystemNoをもとにプッシュ通知対象情報を検索。
        private static Tuple<Guid, Guid> GetNotificationTargetFromPatientId(string patientId, string linkageSystemNo)
        {
            var reader = new NotificationTargetFromPatientIdReader();
            try
            {
                var readerArgs = new NotificationTargetFromPatientIdReaderArgs() { PatientId = patientId, LinkageSystemNo = linkageSystemNo};
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count >= 0)
                {
                    return Tuple.Create(readerResults.TargetAccountKey, readerResults.FromFacilityKey);
                }
                else
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[GetNotificationTargetFromPatientId]プッシュ通知対象情報の取得に失敗しました。PatientId:{patientId}, LinkageSystemNo:{linkageSystemNo}"), Guid.Empty);
                    return Tuple.Create(Guid.Empty, Guid.Empty);                }
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return null;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// お知らせの登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNotificationSendApiResults Send(QoNotificationSendApiArgs args)
        {
            var result = new QoNotificationSendApiResults() { IsSuccess = bool.FalseString , Result =CheckArgs(args)};
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;

            Guid accountkey = args.TargetAccountKey.TryToValueType(Guid.Empty);
            (bool isPush, bool isMail) = CheckNoticePersonalPushAndMail(args.NoticeNo.TryToValueType(long.MinValue) ,
                args.IsPush.TryToValueType(false) ,　args.IsMail.TryToValueType(false), out List<string> cancelNoticeId);
            
            QsDbApplicationTypeEnum systemType = args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None);
            //保存            
            long noticeNo = SaveNoticePersonalData(args, accountkey);
            //必要なら既存スケジュールキャンセル
            if (cancelNoticeId.Any())
                Task.Run(() => { return SendPushCancel(systemType, cancelNoticeId.ToArray()); }).GetAwaiter().GetResult();
            if (noticeNo > 0)
            {
                result.NoticeNo = noticeNo.ToString();
                result.IsSuccess = bool.TrueString;
                result.IsSendPush = isPush.ToString();
                result.IsSendMail = isMail.ToString();
                string url = string.Format("{0}notification?no={1}&user={2}", systemType.ToAppUrlScheme(), noticeNo, accountkey.ToEncrypedReference());
                if (isPush)
                {
                    //Push通知                //失敗時はdb更新
                    isPush = Task.Run(() => {
                        return SendPushNotification(systemType, noticeNo, args.StartDate.TryToValueType(DateTime.MinValue),
                            GetUserId(args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None),accountkey),
                         accountkey, args.Title, args.Contents, url, args.CategoryNo);
                    }).GetAwaiter().GetResult();

                    result.IsSendPush = isPush.ToString();
                    if (!isPush)
                    {
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "Push通知失敗");
                        result.IsSuccess = bool.FalseString;
                    }
                }
                if (isMail)
                {
                    //Mail
                    string mailAddress = GetMailAddress(accountkey);
                    if (!string.IsNullOrEmpty(mailAddress))
                    {
                        string param = systemType.ToUrlParam();
                        url = string.Format("?n={0}&k={1}&p={2}", noticeNo, accountkey.ToEncrypedReference(), param);
                        string bodyPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailBodyPersonalMessage_{0}.txt", param));
                        string footerPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailFooter_{0}.txt", param));
                        string settingsName = string.Format("MailSettingsNamePersonalMessage_{0}",param);
                        isMail = Task.Run(() => {
                            return SendPersonalMessageNoticeMail(settingsName,mailAddress,bodyPath,footerPath,noticeNo,accountkey ,url);
                        }).GetAwaiter().GetResult();
                        result.IsSendMail = isMail.ToString();
                    }
                    if (string.IsNullOrEmpty(mailAddress) || !isMail)
                    {
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "Mail通知失敗");
                        result.IsSuccess = bool.FalseString;
                    }
                }
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "保存に失敗");
                result.IsSuccess = bool.FalseString;
            }

            return result;
        }

        /// <summary>
        /// お知らせの登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNotificationSendAllApiResults SendAll(QoNotificationSendAllApiArgs args)
        {
            var result = new QoNotificationSendAllApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };

            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;
            QsDbApplicationTypeEnum systemType = args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None);
            bool isPush = CheckPush(args.NoticeNo.TryToValueType(long.MinValue),args.IsPush.TryToValueType(false) ,out List<string>cancelNoticeId);
            long noticeNo = SaveNoticeData(args);
            //必要なら既存スケジュールキャンセル
            if (cancelNoticeId.Any())
                Task.Run(() => { return SendPushCancel(systemType, cancelNoticeId.ToArray()); }).GetAwaiter().GetResult();
            //保存
            if (noticeNo>0)
            {
                result.NoticeNo = noticeNo.ToString();
                result.IsSuccess = bool.TrueString;
                //Pushあり
                if (isPush)
                {
                    //Push通知
                    isPush = Task.Run(() => { return SendPushAllNotification(systemType,noticeNo, args.StartDate.TryToValueType(DateTime.MinValue),
                        args.TargetFacilityKey.TryToValueType(Guid.Empty), args.Title, args.Contents); }).GetAwaiter().GetResult();
                    result.IsSendPush = isPush.ToString();
                    if(!isPush)
                    {
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "Push通知失敗");
                        result.IsSuccess = bool.FalseString;
                    }
                }
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError,"保存に失敗");
                result.IsSuccess = bool.FalseString;
            }
                                                
            return result;
        }

        /// <summary>
        /// お知らせの登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNotificationSendFromPatientIdApiResults SendFromPatientId(QoNotificationSendFromPatientIdApiArgs args)
        {
            //プッシュ通知対象検索
            if (string.IsNullOrEmpty(args.PatientId) && string.IsNullOrEmpty(args.LinkageSystemNo))
            {
                return new QoNotificationSendFromPatientIdApiResults() { IsSuccess = bool.FalseString, 
                    Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "患者IDまたはLinkageSystemNoが設定されていません。")}; 
            }
            Tuple<Guid, Guid> Target = GetNotificationTargetFromPatientId(args.PatientId, args.LinkageSystemNo);
            //SQL実行エラーの場合
            if (Target == null && (Target.Item1 != Guid.Empty && Target.Item2 != Guid.Empty))
            {
                return new QoNotificationSendFromPatientIdApiResults()
                {
                    IsSuccess = bool.FalseString,
                    Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "GetNotificationTargetFromPatientIdの実行に失敗しました。")
                };
            }
            //通知対象情報が欠落している場合
            if (Target.Item1 == Guid.Empty || Target.Item2 == Guid.Empty)
            {
                return new QoNotificationSendFromPatientIdApiResults()
                {
                    IsSuccess = bool.FalseString,
                    Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "プッシュ通知対象者を特定することができませんでした。")
                };
            }
            //検索結果を引数クラスに設定
            args.TargetAccountKey = Target.Item1.ToApiGuidString();
            args.FromFacilityKey = Target.Item2.ToApiGuidString();

            var result = new QoNotificationSendFromPatientIdApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;

            Guid accountkey = args.TargetAccountKey.TryToValueType(Guid.Empty);
            (bool isPush, bool isMail) = CheckNoticePersonalPushAndMail(args.NoticeNo.TryToValueType(long.MinValue),
                args.IsPush.TryToValueType(false), args.IsMail.TryToValueType(false), out List<string> cancelNoticeId);

            QsDbApplicationTypeEnum systemType = args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None);
            //保存            
            long noticeNo = SaveNoticePersonalData(args, accountkey);
            //必要なら既存スケジュールキャンセル
            if (cancelNoticeId.Any())
                Task.Run(() => { return SendPushCancel(systemType, cancelNoticeId.ToArray()); }).GetAwaiter().GetResult();
            if (noticeNo > 0)
            {
                result.NoticeNo = noticeNo.ToString();
                result.IsSuccess = bool.TrueString;
                result.IsSendPush = isPush.ToString();
                result.IsSendMail = isMail.ToString();
                string url = string.Format("{0}notification?no={1}&user={2}", systemType.ToAppUrlScheme(), noticeNo, accountkey.ToEncrypedReference());
                if (isPush)
                {
                    //Push通知                //失敗時はdb更新
                    isPush = Task.Run(() => {
                        return SendPushNotification(systemType, noticeNo, args.StartDate.TryToValueType(DateTime.MinValue),
                            GetUserId(args.TargetSystemType.TryToValueType(QsDbApplicationTypeEnum.None), accountkey),
                         accountkey, args.Title, args.Contents, url, args.CategoryNo);
                    }).GetAwaiter().GetResult();

                    result.IsSendPush = isPush.ToString();
                    if (!isPush)
                    {
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "Push通知失敗");
                        result.IsSuccess = bool.FalseString;
                    }
                }
                if (isMail)
                {
                    //Mail
                    string mailAddress = GetMailAddress(accountkey);
                    if (!string.IsNullOrEmpty(mailAddress))
                    {
                        string param = systemType.ToUrlParam();
                        if (param == AppWorker.UrlParam_TisApp) url = string.Format("?n={0}&k={1}&p={2}", noticeNo, accountkey.ToEncrypedReference(), param);
                        string bodyPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailBodyPersonalMessage_{0}.txt", param));
                        string footerPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailFooter_{0}.txt", param));
                        string settingsName = string.Format("MailSettingsNamePersonalMessage_{0}", param);
                        isMail = Task.Run(() => {
                            return SendPersonalMessageNoticeMail(settingsName, mailAddress, bodyPath, footerPath, noticeNo, accountkey, url);
                        }).GetAwaiter().GetResult();
                        result.IsSendMail = isMail.ToString();
                    }
                    if (string.IsNullOrEmpty(mailAddress) || !isMail)
                    {
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError, "Mail通知失敗");
                        result.IsSuccess = bool.FalseString;
                    }
                }
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "保存に失敗");
                result.IsSuccess = bool.FalseString;
            }

            return result;
        }

        /// <summary>
        /// お知らせのリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止しました。代わりにNotificationReadWorker.Readを使ってください。")]
        public static QoNotificationReadApiResults Read(QoNotificationReadApiArgs args)
        {
            var result = new QoNotificationReadApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;
            int maxPageindex=0;
            int pageIndex = 0;
            List<byte> categoryNoList = new List<byte>();
            if (args.CategoryNo != null && args.CategoryNo.Any())
                args.CategoryNo.ForEach(category => { categoryNoList.Add(category.TryToValueType(byte.MinValue)); });
            List<Guid> facilityKeyList = new List<Guid>() { Guid.Empty, GetFacilityKey(args.ExecuteSystemType.TryToValueType(QsDbApplicationTypeEnum.None)) };
            if(!string.IsNullOrEmpty(args.FacilityKeyReference))
            { 
                Guid addFacilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
                if (addFacilityKey != Guid.Empty)
                    facilityKeyList.Add(addFacilityKey);
            }
            if (args.NoticeNo.TryToValueType(long.MinValue) < 0)
            {
                (result.NoticeN , maxPageindex,  pageIndex )= GetNoticeData(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None),
                   args.ActorKey.TryToValueType(Guid.Empty), categoryNoList,facilityKeyList,
                   args.FromDate.TryToValueType(DateTime.MinValue), args.ToDate.TryToValueType(DateTime.MaxValue),
                   args.PageIndex.TryToValueType(int.MinValue), args.PageSize.TryToValueType(int.MinValue));
            }
            else
            {
                result.NoticeN = new List<QoApiNoticeItem>() { GetNoticeData(args.NoticeNo.TryToValueType(long.MinValue)) };
            }
            result.MaxPageIndex = maxPageindex.ToString();
            result.PageIndex = pageIndex.ToString();
            result.IsSuccess = bool.TrueString;
            return result;
        }

        /// <summary>
        /// 個人あてのお知らせリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止しました。代わりに NotificationPersonalReadWorker.Read を使ってください。")]
        public static QoNotificationPersonalReadApiResults PersonalRead(QoNotificationPersonalReadApiArgs args)
        {
            var result = new QoNotificationPersonalReadApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;
            int maxPageindex = 0;
            int pageIndex = 0;
            if (args.NoticeNo.TryToValueType(long.MinValue) < 0)
            {
                (result.NoticeN, maxPageindex, pageIndex) = GetNoticePersonalData(
                    args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None),
                    GetAccountKeyList(args.ActorKey.TryToValueType(Guid.Empty)),
                    args.FromDate.TryToValueType(DateTime.MinValue),
                    args.ToDate.TryToValueType(DateTime.MaxValue),
                    args.OnlyUnread.TryToValueType(false),
                    args.PageIndex.TryToValueType(int.MinValue),
                    args.PageSize.TryToValueType(int.MinValue),
                    args.CategoryNo.TryToValueType(byte.MinValue));
            }
            else
            {
                result.NoticeN = new List<QoApiNoticeItem>() { GetNoticePersonalData(args.NoticeNo.TryToValueType(long.MinValue)) };
            }
            result.MaxPageIndex = maxPageindex.ToString();
            result.PageIndex = pageIndex.ToString();
            result.IsSuccess = bool.TrueString;
            return result;
        }

        /// <summary>
        /// お知らせ添付画像を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNotificationImageReadApiResults ImageRead(QoNotificationImageReadApiArgs args)
        {
            var result = new QoNotificationImageReadApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;

            var fileItem = new QoApiFileItem();
            Guid fileKey = args.FileKeyReference.ToDecrypedReference<Guid>();
            if (fileKey != Guid.Empty)
            {
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
                    fileItem.OriginalName = apiResults.OriginalName;
                    fileItem.ContentType = apiResults.ContentType;
                    fileItem.Data = apiResults.Data;
                    result.Image = fileItem;
                    result.IsSuccess = bool.TrueString;
                }
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FileKeyが不正です。");
            }
            return result;
        }
        
        /// <summary>
        /// 既読の書き込みをします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoNotificationPersonalReadReceiptWriteApiResults PersonalReadReceiptWrite(QoNotificationPersonalReadReceiptWriteApiArgs args)
        {
            var result = new QoNotificationPersonalReadReceiptWriteApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;
            var entity = new QH_NOTICEPERSONALALREADYREAD_DAT()
            {
                ACCOUNTKEY = args.ActorKey.TryToValueType(Guid.Empty),
                NOTICENO = args.NoticeNo.TryToValueType(long.MinValue),
                ALREADYREADFLAG = true,
                CREATEDDATE = DateTime.Now,
                DELETEFLAG = false,
                UPDATEDDATE = DateTime.Now
            };
            var writerResult = QsDbManager.Write(new QhNoticePersonalAlreadyReadEntityWriter(), new QhNoticePersonalAlreadyReadEntityWriterArgs() { Data = new List<QH_NOTICEPERSONALALREADYREAD_DAT>() { entity } });
            if (!writerResult.IsSuccess)
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
            }
            else
                result.IsSuccess = bool.TrueString;
            return result;
        }
        #endregion
    }
}