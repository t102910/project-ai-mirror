using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Models;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// カレンダーのイベント情報を処理します。
    /// </summary>
    public sealed class CalendarWorker
    {
        /// <summary>
        /// お薬手帳データに紐づくカレンダー イベントカテゴリ種別を表します。
        /// </summary>
        private const int MEDICINE_EVENT_CATEGORYNO = 4;
        /// <summary>
        /// 通院カレンダーイベントカテゴリ種別を表します。
        /// </summary>
        private const int MEDICAL_EVENT_CATEGORYNO = 11;
       
        #region "Private Method"

        /// <summary>
        ///  デフォルトカレンダーを取得する。デフォルトカレンダーが存在しない場合、作成後に取得する。
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="actorKey"></param>
        /// <param name="linkagSystemNo"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool ReadDefaultCalendar(Guid authorKey, Guid actorKey, int linkagSystemNo)
        {
            DbCalendarReader reader = new DbCalendarReader();
            DbCalendarReaderArgs readerArgs = new DbCalendarReaderArgs()
            {
                AuthorKey = authorKey,
                ActorKey = actorKey,
                LinkageSystemNo = linkagSystemNo,
                PageNo = byte.MinValue,
                IsInitialize = true
            };
            
            try
            {
                // 追加・更新処理を行っているためWriteを利用(CalendarReaderクラスもWriterBaseを継承して作成)
                return QsDbManager.Write(reader, readerArgs).IsSuccess;
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, actorKey);

            }

            return false;
        }

        /// <summary>
        ///  カレンダーイベントを取得する。
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="actorKey"></param>
        /// <param name="categoryNoN"></param>
        /// <param name="customTagNoN"></param>
        /// <param name="endDate"></param>
        /// <param name="startDate"></param>
        /// <param name="finishStateType"></param>
        /// <param name="systemTagNoN"></param>
        /// <param name="linkagSystemNo"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static DbCalendarDetailReaderResults ReadCalendarEvent(Guid authorKey, Guid actorKey, int linkagSystemNo, 
            DateTime startDate, DateTime endDate, List<string> categoryNoN, List<string> systemTagNoN, List<string> customTagNoN, 
            DbCalendarReaderCore.EventFinishStateTypeEnum finishStateType)
        {
            DateTime.TryParseExact(startDate.ToString("yyyyMMdd0000000000000"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None, out DateTime setStartDate);
            DateTime.TryParseExact(endDate.ToString("yyyyMMdd2359599999999"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None, out DateTime setEndDate);

            DbCalendarDetailReader reader = new DbCalendarDetailReader();
            DbCalendarDetailReaderArgs readerArgs = new DbCalendarDetailReaderArgs()
            {
                AuthorKey = authorKey,
                ActorKey = actorKey,
                LinkageSystemNo = linkagSystemNo,
                StartDate = setStartDate,
                EndDate = setEndDate,
                CategoryNoN = categoryNoN.ConvertAll(i => byte.Parse(i)),
                SystemTagNoN = systemTagNoN.ConvertAll(i => int.Parse(i)),
                CustomTagNoN = customTagNoN.ConvertAll(i => int.Parse(i)),
                FinishStateType = finishStateType
            };
            DbCalendarDetailReaderResults readerResults = new DbCalendarDetailReaderResults();

            try
            {
                readerResults = QsDbManager.Read(reader, readerArgs);
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, actorKey);
            }

            return readerResults;
        }

        /// <summary>
        /// カレンダーイベントの登録を行う。
        /// </summary>
        /// <param name="eventItem"></param>
        /// <param name="actorKey"></param>
        /// <param name="authorKey"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static DbCalendarEventWriterResults WriteCalendarEvent<TEventSet>(QoApiCalendarEventItem  eventItem, Guid authorKey, Guid actorKey) where TEventSet : QsEventSetOfJsonBase, new()
        {
            DbCalendarWriterCore.EventWriteModeTypeEnum writeMode = DbCalendarWriterCore.EventWriteModeTypeEnum.None;
            if (eventItem.DeleteFlag.TryToValueType(false))
                writeMode = DbCalendarWriterCore.EventWriteModeTypeEnum.Deleted;
            else if (eventItem.Sequence.TryToValueType(int.MinValue) < 0)
                writeMode = DbCalendarWriterCore.EventWriteModeTypeEnum.Added;
            else
                writeMode = DbCalendarWriterCore.EventWriteModeTypeEnum.Modified;

            QsJsonSerializer ser = new QsJsonSerializer();
            QhEventForeignKeyOfJson foreignKey = ser.Deserialize<QhEventForeignKeyOfJson>(eventItem.ForeignKey);
            TEventSet eventSet = CalendarWorker.ToDbEventSet<TEventSet>(eventItem, foreignKey);

            DbCalendarEventWriter<TEventSet> writer = new DbCalendarEventWriter<TEventSet>();
            DbCalendarEventWriterArgs writerArgs = new DbCalendarEventWriterArgs()
            {
                AuthorKey = authorKey,
                ActorKey = actorKey,
                WriteModeType = writeMode,
                LinkageSystemNo = eventItem.LinkageSystemNo.TryToValueType(int.MinValue),
                EventDate = eventItem.EventDate.TryToValueType(DateTime.MinValue),
                EventSequence = eventItem.Sequence.TryToValueType(int.MinValue),  //これの設定があればUpdate？
                StartDate = eventItem.StartDate.TryToValueType(DateTime.MinValue),
                EndDate = eventItem.EndDate.TryToValueType(DateTime.MinValue),
                EventType = eventItem.EventType.TryToValueType(QsDbEventTypeEnum.None),
                Name = eventItem.Name,
                AlldayFlag = eventItem.AllDayFlag.TryToValueType(false),
                FinishFlag = eventItem.FinishFlag.TryToValueType(false),
                NoticeFlag = eventItem.NoticeFlag.TryToValueType(false),
                OpenFlag = eventItem.OpenFlag.TryToValueType(false),
                Importance = eventItem.Importance.TryToValueType(byte.MinValue),
                CustomStampNo = eventItem.CustomStampNo.TryToValueType(int.MinValue),
                EventSetTypeName = eventItem.EventSetTypeName,
                EventSet = ser.Serialize<TEventSet>(eventSet),
                ForeignKey = ser.Serialize<QhEventForeignKeyOfJson>(foreignKey),
                CategoryNoN = eventItem.CategoryNoN.ConvertAll(i => byte.Parse(i)),
                SystemTagNoN = eventItem.SystemTagNoN.ConvertAll(i => int.Parse(i)),
                CustomTagNoN = eventItem.CustomTagNoN.ConvertAll(i => int.Parse(i))
            };
            DbCalendarEventWriterResults writerResults = new DbCalendarEventWriterResults();

            return QsDbManager.Write(writer, writerArgs);
            
        }

        private static QoApiCalendarEventSetItem ToApiEventSet<TEventSet>(TEventSet source) where TEventSet : QsEventSetOfJsonBase
        {
            QoApiCalendarEventSetItem result = new QoApiCalendarEventSetItem();

            if (source != null && source.EventContents !=null)
            {
                result.Detail = source.EventContents.Detail;
                result.FileKeyN = new List<QoApiFileKeyItem>();
                if (source.EventContents.AttachedFileN != null && source.EventContents.AttachedFileN.Any())
                result.FileKeyN = source.EventContents.AttachedFileN.ConvertAll(i => new QoApiFileKeyItem() { FileKeyReference = i.FileKey.ToValueType<Guid>().ToEncrypedReference() });

                if (source.EventContents.Place != null)
                {
                    result.Address = source.EventContents.Place.Address;
                    result.Location = new QoApiLocationItem() { Latitude = source.EventContents.Place.Latitude, Longitude = source.EventContents.Place.Longitude };

                    if (!string.IsNullOrWhiteSpace(source.EventContents.Place.FacilityKey))
                        result.FacilityKeyReference = source.EventContents.Place.FacilityKey.ToValueType<Guid>().ToEncrypedReference();

                    if (source.EventContents.Place.PhoneN != null && source.EventContents.Place.PhoneN.Any())
                        result.PhoneNumber = source.EventContents.Place.PhoneN.First().PhoneNumber;
                }
                
            }
            return result;
        }

        private static TEventSet ToDbEventSet<TEventSet>(QoApiCalendarEventItem source, QhEventForeignKeyOfJson foreignKey) where TEventSet : QsEventSetOfJsonBase, new()
        {
            TEventSet result = new TEventSet();

            result.EventContents = new QhEventContentsOfJson() { Detail = source.EventSet.Detail };
            if (source.EventSet.FileKeyN != null && source.EventSet.FileKeyN.Any())
                result.EventContents.AttachedFileN = source.EventSet.FileKeyN.ConvertAll(i => new QhAttachedFileOfJson() { FileKey = i.FileKeyReference.ToDecrypedReference() });

            result.EventContents.Place = new QhPlaceOfJson() { Address = source.EventSet.Address };
            if (!string.IsNullOrWhiteSpace(source.EventSet.FacilityKeyReference))
                result.EventContents.Place.FacilityKey = source.EventSet.FacilityKeyReference.ToDecrypedReference();
            if (source.EventSet.Location != null)
            {
                if (!string.IsNullOrWhiteSpace(source.EventSet.Location.Latitude))
                    result.EventContents.Place.Latitude = source.EventSet.Location.Latitude;
                if (!string.IsNullOrWhiteSpace(source.EventSet.Location.Longitude))
                    result.EventContents.Place.Longitude = source.EventSet.Location.Longitude;
            }
            result.EventContents.Place.PhoneN = new List<QhPhoneOfJson>();
            if (!string.IsNullOrWhiteSpace(source.EventSet.PhoneNumber))
                result.EventContents.Place.PhoneN.Add(new QhPhoneOfJson() { PhoneNumber = source.EventSet.PhoneNumber, PhoneType = Convert.ToByte(QsDbPhoneTypeEnum.None).ToString() });
            result.EventForeignKey = foreignKey;
            
            result.EventStatus = new QhEventStatusOfJson()
            {
                AllDayFlag = source.AllDayFlag,
                FinishFlag = source.FinishFlag,
                Importance = source.Importance,
                NoticeFlag = source.NoticeFlag,
                OpenFlag = source.OpenFlag
            };

            //通院イベントのみの特別変換処理
            if (source.EventSetTypeName==nameof(QhMedicalEventSetOfJson) && source.EventSet.MedicalEvent !=null)
            {
                QhMedicalEventSetOfJson medicalSet = result as QhMedicalEventSetOfJson;
                QoApiMedicalEventItem apiMedicalSet = source.EventSet.MedicalEvent;
                medicalSet.AccountingAmount = apiMedicalSet.AccountingAmount;
                medicalSet.AppointmentPhoneNumber = apiMedicalSet.AppointmentPhoneNumber;
                medicalSet.AppointmentSystemUrl = apiMedicalSet.AppointmentSystemUrl;
                medicalSet.DepartmentCode = apiMedicalSet.DepartmentCode;
                medicalSet.DepartmentName = apiMedicalSet.DepartmentName;
                medicalSet.DoctorName = apiMedicalSet.DoctorName;
                medicalSet.MedicalEventType = apiMedicalSet.MedicalEventType;
                medicalSet.MedicalExaminationCourseName = apiMedicalSet.MedicalExaminationCourseName;
                medicalSet.ReservationNo = apiMedicalSet.ReservationNo;
                medicalSet.IsVagueReservationTime = apiMedicalSet.IsVagueReservation;
                medicalSet.IsAccepted = apiMedicalSet.IsAccepted;
            }
            return result;
        }

        /// <summary>
        /// 対象者アカウントキーを取得します。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="actorKey"></param>
        /// <returns></returns>
        private static Guid GetTargetAccountKey(int linkageSystemNo, string linkageSystemId, string actorKey)
        {
            Guid accountKey =  Guid.Empty;
            
            //アカウントキー指定の場合は素直にかえす。
            //if (accountKey != Guid.Empty)
            //    return accountKey;

            if(linkageSystemNo>int.MinValue && !string.IsNullOrEmpty(linkageSystemId))
            {
                //LinkageSystemNoと LinkageSystemID からアカウントキーを取得する
                var readerArgs = new LinkageUserReaderArgs() { LinkageSystemId = linkageSystemId, LinkageSystemNo = linkageSystemNo };
                accountKey = QsDbManager.Read(new LinkageUserReader(), readerArgs).AccountKey;
                if (accountKey != Guid.Empty)
                    return accountKey;
            }
            
            //ActorKey もしくはJwt>ActorKey(コントローラBaseで変換される）からアカウントキーを取得する
            accountKey = actorKey.TryToValueType(Guid.Empty);
            
            return accountKey;
        }

        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoCalendarImageReadApiArgs args)
        {
            if (args.FileType.TryToValueType(QsApiFileTypeEnum.None) == QsApiFileTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
            if (string.IsNullOrWhiteSpace(args.FileKeyReference))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FileKeyReferenceが指定されていません");
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }

        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoCalendarImageWriteApiArgs args)
        {
            if (args.ImageFile == null)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "画像情報が不正です。");
            if (string.IsNullOrWhiteSpace(args.ImageFile.ContentType))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "画像MIMEタイプが不正です。");
            if (string.IsNullOrWhiteSpace(args.ImageFile.Data))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "画像データが不正です。");

            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        /// <summary>
        /// 引数チェック
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static QoApiResultItem CheckEventReadArgs(QoCalendarEventReadApiArgs args)
        {
            if (args == null)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            if (string.IsNullOrWhiteSpace(args.ActorKey) && string.IsNullOrEmpty(args.LinkageSystemId))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "ActorKeyが取得できません");
            if (string.IsNullOrWhiteSpace(args.AuthorKey))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AuthorKeyが取得できません");
            //if (int.TryParse(args.LinkageSystemNo, out _) == false)
            //    return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です");
            if (args.StartDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "StartDateが不正です");
            if (args.EndDate.TryToValueType(DateTime.MinValue) == DateTime.MinValue)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "EndDateが不正です");
            if (!string.IsNullOrWhiteSpace(args.FinishStateType) && 
                args.FinishStateType.TryToValueType(DbCalendarReaderCore.EventFinishStateTypeEnum.None)== DbCalendarReaderCore.EventFinishStateTypeEnum.None)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,"FinishStateTypeが不正です");
            
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            
        }

        /// <summary>
        /// 引数チェック
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static QoApiResultItem CheckEventWriteArgs(QoCalendarEventWriteApiArgs args)
        {
            if (args == null)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
            if (string.IsNullOrWhiteSpace(args.ActorKey) && string.IsNullOrEmpty(args.Event.LinkageSystemId) )
                return  QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Actorが取得できません");
            if (string.IsNullOrWhiteSpace(args.AuthorKey))
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AuthorKeyが取得できません");
            if (args.Event == null)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Eventが指定されてません");
                
            //if (int.TryParse(args.Event.LinkageSystemNo, out _) == false)
            //    return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNoが不正です");
            
            if (int.TryParse(args.Event.EventType, out _) == false)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "EventTypeが不正です");
                
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }

        //アカウントがもつLinkageNoを全て返す
        private static int[] GetUserLinkageNoList(Guid accountKey)
        {
            //List<int> results = new List<int>() { 99999 };  //デフォルトでQOLMSのカレンダーも取得できるようにする。
            List<int> results = new List<int>();
            var readerResults = QsDbManager.Read(new LinkagePatientCardReader(), new LinkagePatientCardReaderArgs() { ActorKey = accountKey, AuthorKey = accountKey });
            if (readerResults.IsSuccess)
            {
                if (readerResults.PatientCardItemN == null)
                    return results.ToArray();

                List<MGF.QOLMS.QolmsOpenApi.Sql.DbPatientCardItem> cardList = readerResults.PatientCardItemN.Where(n => n.StatusType == 2).ToList();

                if (cardList.Count > 0)
                {
                    foreach (var item in cardList)
                    {
                        if(item.CardCode>0)
                            results.Add(item.CardCode );
                    }

                }
            }
            return results.ToArray();
        }

        private static async Task SendPushForReservation(string systemType, int linkageSystemNo, string linkageSystemId, Guid accountKey, string facilityKeyReference)
        {
            try
            {
                // SystemTypeからシステムを表すLinkageSystemNoへ変換
                var rootLinkageSystemNo = systemType.ToLinkageSystemNo();
                if(rootLinkageSystemNo == 0)
                {
                    // 対応がなければ何もしない
                    return;
                }

                // LinkageSystemNoから対応するNotificationHubsの設定を生成
                var hubSettings = rootLinkageSystemNo.ToNotificationHubSettings();
                if (hubSettings == null)
                {
                    // 対応Hubが無ければ何もしない
                    return;
                }

                var hub = new QoPushNotification(hubSettings);

                var payload = new WaitingNotificationPayload
                {
                    AccountKeyReference = accountKey.ToEncrypedReference(),
                    EventType = WaitingEventType.ReservationNew,
                    FacilityKeyReference = facilityKeyReference,
                    CreatedAt = DateTime.UtcNow
                };

                var payloadJson = new QsJsonSerializer().Serialize(payload);

                var linkageRepo = new LinkageRepository();
                // Push通知用のIDを取得する
                var userList = linkageRepo.ReadPushNotificationUserView(rootLinkageSystemNo, linkageSystemNo, new List<string> { linkageSystemId });

                if (!userList.Any())
                {
                    return;
                }

                var info = userList.First();

                // タグは順番待ち扱いとする
                // 現在はアプリの状態変化のトリガーとして使うのみ
                // 今後通常の通知として使うのであればそれに合わせて考慮する
                var tags = new List<string>
                {
                    "Waiting",
                    "$UserId:{" + info.NOTIFICATIONUSERID.ToEncrypedReference() + "}"
                };

                var request = new NotificationRequest
                {
                    Extra = payloadJson,
                    Silent = true,
                    Text = string.Empty,
                    Title = string.Empty,
                    Badge = 0,
                    Url = string.Empty
                };
                request.SetTagExpressionJoinAllAnd(tags.ToArray());

                var result = await hub.RequestNotificationAsync(request);
                if (string.IsNullOrEmpty(result[0]) && string.IsNullOrEmpty(result[1]))
                {
                    QoAccessLog.WriteErrorLog(string.Format("新規予約Push送信失敗:{0}", payload), Guid.Empty);
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
        }

        #endregion

        #region "Public method"

        /// <summary>
        ///  カレンダーイベントを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCalendarEventReadApiResults EventRead(QoCalendarEventReadApiArgs args)
        {
            QoCalendarEventReadApiResults result = new QoCalendarEventReadApiResults() { IsSuccess = bool.FalseString, Result= CheckEventReadArgs(args)};
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;

            DbCalendarReaderCore.EventFinishStateTypeEnum finishStateType = string.IsNullOrEmpty(args.FinishStateType) ? DbCalendarReaderCore.EventFinishStateTypeEnum.All :  args.FinishStateType.TryToValueType(DbCalendarReaderCore.EventFinishStateTypeEnum.All); // 未指定の場合は全て取得
            int no = args.LinkageSystemNo.TryToValueType(int.MinValue);

            if (!string.IsNullOrWhiteSpace(args.FacilityKeyReference))
            {
                // 施設キーが指定されている場合は施設キーから連携システム番号を取得する
                var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
                no = new LinkageRepository().GetLinkageNo(facilityKey);
            }

            // 対象アカウントキーを取得
            Guid accountKey = GetTargetAccountKey(no, args.LinkageSystemId ,args.ActorKey);
            int[] linkageSystemNoList;
            if (no > 0)
            {
                linkageSystemNoList = new int[] { no };                 //Job等からは指定で来る
            }            
            else
            {
                // どちらも指定されていない場合
                linkageSystemNoList = GetUserLinkageNoList(accountKey); //アプリからは指定できないのでこちらで割り出してやる
            }

            try
            {
                List<QoApiCalendarEventItem> eventN = new List<QoApiCalendarEventItem>();
                List<QoApiCalendarEventCategoryIndexItem> categoryN = new List<QoApiCalendarEventCategoryIndexItem>();
                List<QoApiCalendarEventTagIndexItem> tagN = new List<QoApiCalendarEventTagIndexItem>();

                // デフォルトカレンダー取得 : 使ってなさそうなのでコメントアウトしたが、必要なんだろうか・・・（作成者の意図不明なので何か問題あったら復活させてください）
                /*if (!ReadDefaultCalendar(args.AuthorKey.TryToValueType(Guid.Empty), accountKey, linkageSytemNo))
                {
                    QoAccessLog.WriteErrorLog("デフォルトカレンダーの取得に失敗しました。",args.Executor.TryToValueType(Guid.Empty) );
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "デフォルトカレンダーの取得に失敗しました。");
                    return result;
                }*/
                foreach (var linkageSystemNo in linkageSystemNoList)
                {
                    // カレンダーイベント取得
                    DbCalendarDetailReaderResults readerResults = ReadCalendarEvent(args.AuthorKey.TryToValueType(Guid.Empty), accountKey, linkageSystemNo, args.StartDate.TryToValueType(DateTime.MinValue), args.EndDate.TryToValueType(DateTime.MinValue), args.CategoryNoN, args.SystemTagNoN, args.CustomTagNoN, finishStateType);
                    if (readerResults.IsSuccess == false)
                    {
                        QoAccessLog.WriteErrorLog("カレンダーイベントの取得に失敗しました。", args.Executor.TryToValueType(Guid.Empty));
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError, "カレンダーイベントの取得に失敗しました。");
                        return result;
                    }
                    //成功系
                    foreach (DbEventCategoryIndexItem item in readerResults.EventCategoryIndexN)
                    {
                        categoryN.Add(new QoApiCalendarEventCategoryIndexItem()
                        {
                            LinkageSystemNo = item.LinkageSystemNo.ToString(),
                            CategoryNo = item.CategoryNo.ToString(),
                            EventDate = item.EventDate.ToApiDateString(),
                            Sequence = item.EventSequence.ToString(),
                            StartDate = item.StartDate.ToApiDateString(),
                            EndDate = item.EndDate.ToApiDateString()
                        });
                    }

                    foreach (DbEventTagIndexItem item in readerResults.EventTagIndexN)
                    {
                        tagN.Add(new QoApiCalendarEventTagIndexItem()
                        {
                            LinkageSystemNo = item.LinkageSystemNo.ToString(),
                            TagType = item.TagType.ToString("d"),
                            TagNo = item.TagNo.ToString(),
                            EventDate = item.EventDate.ToApiDateString(),
                            Sequence = item.EventSequence.ToString(),
                            StartDate = item.StartDate.ToApiDateString(),
                            EndDate = item.EndDate.ToApiDateString()
                        });
                    }

                    foreach (DbEventItem cal in readerResults.EventN)
                    {
                        QoApiCalendarEventItem eventItem = new QoApiCalendarEventItem()
                        {
                            LinkageSystemNo = cal.LinkageSystemNo.ToString(),
                            EventDate = cal.EventDate.ToApiDateString(),
                            Sequence = cal.EventSequence.ToString(),
                            StartDate = cal.StartDate.ToApiDateString(),
                            EndDate = cal.EndDate.ToApiDateString(),
                            EventType = cal.EventType.ToString("d"),
                            Name = cal.Name,
                            AllDayFlag = cal.AlldayFlag.ToString(),
                            FinishFlag = cal.FinishFlag.ToString(),
                            NoticeFlag = cal.NoticeFlag.ToString(),
                            OpenFlag = cal.OpenFlag.ToString(),
                            Importance = cal.Importance.ToString(),
                            CustomStampNo = cal.CustomStampNo.ToString(),
                            EventSetTypeName = cal.EventSetTypeName,
                            EventSet = new QoApiCalendarEventSetItem(),
                            DeleteFlag = bool.FalseString,
                            CategoryNoN = new List<string>()
                        };

                        switch (cal.EventSetTypeName)
                        {
                            case nameof(QhDefaultEventSetOfJson):
                                eventItem.EventSet = CalendarWorker.ToApiEventSet(new QsJsonSerializer().Deserialize<QhDefaultEventSetOfJson>(cal.EventSet));
                                break;

                            case nameof(QhMedicineEventSetOfJson):
                                //お薬
                                eventItem.EventSet = CalendarWorker.ToApiEventSet(new QsJsonSerializer().Deserialize<QhMedicineEventSetOfJson>(cal.EventSet));
                                break;

                            case nameof(QhMedicalEventSetOfJson):
                                //通院
                                var dbEvSet = new QsJsonSerializer().Deserialize<QhMedicalEventSetOfJson>(cal.EventSet);
                                eventItem.EventSet = CalendarWorker.ToApiEventSet(dbEvSet);
                                eventItem.EventSet.MedicalEvent = new QoApiMedicalEventItem()
                                {
                                    AccountingAmount = dbEvSet.AccountingAmount,
                                    AppointmentPhoneNumber = dbEvSet.AppointmentPhoneNumber,
                                    AppointmentSystemUrl = dbEvSet.AppointmentSystemUrl,
                                    DepartmentCode = dbEvSet.DepartmentCode,
                                    DepartmentName = dbEvSet.DepartmentName,
                                    DoctorName = dbEvSet.DoctorName,
                                    MedicalEventType = dbEvSet.MedicalEventType,
                                    MedicalExaminationCourseName = dbEvSet.MedicalExaminationCourseName,
                                    ReservationNo = dbEvSet.ReservationNo,
                                    IsVagueReservation = dbEvSet.IsVagueReservationTime,
                                    IsAccepted = dbEvSet.IsAccepted,
                                };
                                break;
                        }

                        eventItem.CategoryNoN.Add(categoryN.Find(i => i.LinkageSystemNo == eventItem.LinkageSystemNo && i.EventDate == eventItem.EventDate && i.Sequence == eventItem.Sequence).CategoryNo);
                        eventItem.SystemTagNoN.AddRange(tagN.Where(i => i.LinkageSystemNo == eventItem.LinkageSystemNo && i.EventDate == eventItem.EventDate && i.Sequence == eventItem.Sequence).Select(j=>j.TagNo.ToString()).ToList());
                        if( !eventN.Any(m=>m.EventDate == eventItem.EventDate && m.Sequence == eventItem.Sequence && m.LinkageSystemNo ==eventItem.LinkageSystemNo  ))//重複を外す
                            eventN.Add(eventItem);
                    }
                }            
                
                result.EventN = eventN;
                result.EventCategoryIndexN = categoryN;
                result.EventTagIndexN = tagN;
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);                
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
                result.Result = QoApiResult.Build(ex);
            }

            return result;
        }

        /// <summary>
        /// カレンダーイベントを登録します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCalendarEventWriteApiResults EventWrite(QoCalendarEventWriteApiArgs args)
        {
            QoCalendarEventWriteApiResults result = new QoCalendarEventWriteApiResults() { IsSuccess = bool.FalseString, Result = CheckEventWriteArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;

            int linkageSystemNo = args.Event.LinkageSystemNo.TryToValueType(int.MinValue);
            // LinkageSystemNo未設定の場合は施設キーからの逆引きを試みる（アプリ用）
            if (linkageSystemNo == int.MinValue)
            {
                linkageSystemNo = new LinkageRepository()
                    .GetLinkageNo(args.Event.EventSet.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty));
                args.Event.LinkageSystemNo = linkageSystemNo == int.MinValue ? string.Empty : linkageSystemNo.ToString();
            }
            //対象アカウントを取得
            Guid accountKey = GetTargetAccountKey( linkageSystemNo,args.Event.LinkageSystemId,args.ActorKey );
            

            try
            {
                // デフォルトカレンダー取得
                if (!ReadDefaultCalendar(args.AuthorKey.TryToValueType(Guid.Empty), accountKey, linkageSystemNo))
                {
                    QoAccessLog.WriteErrorLog("デフォルトカレンダーの取得に失敗しました。", args.Executor.TryToValueType(Guid.Empty));
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError);
                    return result;
                }

                if (string.IsNullOrWhiteSpace(args.Event.EventSetTypeName))
                {
                    switch (args.Event.CategoryNoN.First().TryToValueType<int>(int.MinValue))
                    {
                        case MEDICINE_EVENT_CATEGORYNO:
                            //お薬
                            args.Event.EventSetTypeName = typeof(QhMedicineEventSetOfJson).Name;
                            break;
                        case MEDICAL_EVENT_CATEGORYNO:
                            //通院
                            args.Event.EventSetTypeName = typeof(QhMedicalEventSetOfJson).Name;
                            break;
                        default:
                            args.Event.EventSetTypeName = typeof(QhDefaultEventSetOfJson).Name;
                            break;
                    }
                }
                //カレンダーイベント登録
                //DbCalendarEventWriterResults writerResults = EventSetOfJsonDic[args.Event.EventSetTypeName](args);
                DbCalendarEventWriterResults writerResults;
                switch (args.Event.EventSetTypeName )
                {
                    case nameof(QhMedicineEventSetOfJson):
                        writerResults = WriteCalendarEvent<QhMedicineEventSetOfJson>(args.Event, args.AuthorKey.TryToValueType(Guid.Empty), accountKey);
                        break;
                    case nameof(QhMedicalEventSetOfJson):
                        writerResults = WriteCalendarEvent<QhMedicalEventSetOfJson>(args.Event, args.AuthorKey.TryToValueType(Guid.Empty), accountKey);
                        break;
                    default:
                        writerResults = WriteCalendarEvent<QhDefaultEventSetOfJson>(args.Event, args.AuthorKey.TryToValueType(Guid.Empty), accountKey);
                        break;
                }
                
                // 予約の場合、通知を送信
                if(args.Event.CategoryNoN.Any(x => x == "11") && 
                    args.Event.SystemTagNoN.Any(x => x == "3") && 
                    args.Event.EventSetTypeName == nameof(QhMedicalEventSetOfJson) &&
                    args.Event.EventType == "1" &&
                    args.Event.FinishFlag == bool.FalseString &&
                    args.Event.DeleteFlag == bool.FalseString)
                {

                    _ = SendPushForReservation(args.ExecuteSystemType, linkageSystemNo, args.Event.LinkageSystemId, accountKey, args.Event.EventSet.FacilityKeyReference);
                }

                result.IsSuccess = writerResults.IsSuccess.ToString();

                if (writerResults.IsSuccess == false)
                {
                    QoAccessLog.WriteErrorLog("カレンダーイベントの登録に失敗しました。", args.Executor.TryToValueType(Guid.Empty));
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                }
                else
                {
                    result.Sequence = writerResults.EventSequence.ToString();
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError );
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
                QoAccessLog.WriteInfoLog(string.Format("args:{0}", new QsJsonSerializer().Serialize(args)));
            }

            return result;
        }
       
        /// <summary>
        /// カレンダーの添付画像を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCalendarImageReadApiResults ImageRead(QoCalendarImageReadApiArgs args)
        {
            QoCalendarImageReadApiResults result = new QoCalendarImageReadApiResults() { IsSuccess = bool.FalseString, Result = CheckArgs(args) };
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
        /// カレンダーの画像を登録します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoCalendarImageWriteApiResults ImageWrite(QoCalendarImageWriteApiArgs args)
        {
            QoCalendarImageWriteApiResults result =new QoCalendarImageWriteApiResults() { IsSuccess =bool.FalseString , Result = CheckArgs(args) };
            //パラメータチェックエラー
            if (result.Result.Code.TryToValueType(int.MinValue) != (int)QoApiResultCodeTypeEnum.Success)
                return result;
            
                QhBlobStorageWriteApiResults apiResults = QoBlobStorage.Write<QH_UPLOADFILE_DAT>(new QhBlobStorageWriteApiArgs()
                {
                    ActorKey = args.ActorKey,
                    ApiType = QhApiTypeEnum.FileStorageWrite.ToString(),
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName,
                    OriginalName = args.ImageFile.OriginalName,
                    ContentType = args.ImageFile.ContentType,
                    Data = args.ImageFile.Data,
                    FileRelationType = QsApiFileRelationTypeEnum.PersonPhoto.ToString()
                });

                if (apiResults != null && apiResults.IsSuccess == bool.TrueString && !string.IsNullOrWhiteSpace(apiResults.FileKey))
                {
                    Guid fileKey = apiResults.FileKey.TryToValueType<Guid>(Guid.Empty);
                    if (fileKey != Guid.Empty)
                    {
                        result.FileKeyReference = fileKey.ToEncrypedReference();
                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                }
            

            return result;
        }
        #endregion
    }


}