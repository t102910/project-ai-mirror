using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.DataTypeEnum;
using StatusType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.StatusTypeEnum;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 順番待ち情報リスト書き込み処理
    /// 要テストコード
    /// </summary>
    public class WaitingListWriteWorker
    {
        // 順番待ちを示すタグ
        internal const string TAG_WAITING = "Waiting";

        internal const string MSG_TITLE = "順番待ち";

        // FacilityRepository からメッセージを取得するためのキー
        internal const string KEY_EXAMINATION_SOON = "Push_Examination_Soon";
        internal const string KEY_EXAMINATION_READY = "Push_Examination_Ready";
        internal const string KEY_MEDICINE_READY = "Push_Medicine_Ready";
        internal const string KEY_ACCOUNTING_READY = "Push_Accounting_Ready";
        internal const string KEY_MEDICINE_READY_SPECIAL = "Push_Medicine_Ready_Special";
        internal const string KEY_MEDICINE_POSTPAY_EXTERNAL = "Push_Medicine_Postpay_External";
        internal const string KEY_MEDICINE_POSTPAY_READY = "Push_Medicine_Postpay_Ready";

        IWaitingRepository _waitingRepo;
        ILinkageRepository _linkageRepo;
        IQoPushNotification _pushNotification;
        IFacilityRepository _facilityRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="waitingRepository">順番待ちテーブルを扱うリポジトリ</param>
        /// <param name="linkageRepository">連携システムデータを扱うリポジトリ</param>
        /// <param name="qoPushNotification">Push 通知インターフェース</param>
        /// <param name="facilityRepo">施設情報を扱うリポジトリ</param>
        public WaitingListWriteWorker(IWaitingRepository waitingRepository, ILinkageRepository linkageRepository, IQoPushNotification qoPushNotification, IFacilityRepository facilityRepo)
        {
            _waitingRepo = waitingRepository;
            _linkageRepo = linkageRepository;
            _pushNotification = qoPushNotification;
            _facilityRepo = facilityRepo;
        }

        /// <summary>
        /// 順番待ち情報リストを登録し通知を送信する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoWaitingListWriteApiResults> ListWrite(QoWaitingListWriteApiArgs args)
        {
            var results = new QoWaitingListWriteApiResults
            {
                IsSuccess = bool.FalseString
            };
            
            // 必須チェック DataType / FacilityKey / LinkageSystemNo
            if (!args.DataType.CheckArgsRequired(nameof(args.DataType), results) ||
               !args.FacilityKey.CheckArgsRequired(nameof(args.FacilityKey), results) ||
               !args.LinkageSystemNo.CheckArgsRequired(nameof(args.LinkageSystemNo), results))
            {
                return results;
            }

            // 順番待ちリストのチェック
            if(!CheckArgsWaitingList(args.WaitingListN, results))
            {
                return results;
            }

            // 親連携システム番号変換
            var rootLinkageSystemNo = args.RootLinkageSystemNo.TryToValueType(int.MinValue);
            if(rootLinkageSystemNo == int.MinValue)
            {
                // デフォルトはHOSPA
                rootLinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO;
            }          

            var facilityKey = args.FacilityKey.TryToValueType(Guid.Empty);
            var dataType = args.DataType.TryToValueType((byte)DataType.None);
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(0);

            // 施設言語取得
            if (!TryCreateFacilityLanguageResource(facilityKey, results, out var messageResource))
            {
                return results;
            }                        

            // QH_WAITING_DATに変換
            if (!TryConvertEntity(args.WaitingListN,facilityKey,dataType,linkageSystemNo,results, out var entities))
            {
                return results;
            }

            // 順番待ち情報リストをDBに登録
            if(!TryWriteWaitingEntities(entities, results, out var oldList))
            {
                return results;
            }

            // 順番待ち人数情報をDBに登録しPush通知対象を取得する
            if(!TryWriteOrderEntities(entities, results, out var pushList))
            {
                return results;
            }

            // Push通知（成否はAPI結果に影響しない）
            var pushErrors = await NotifyIfNeeds(pushList, oldList, rootLinkageSystemNo, linkageSystemNo, messageResource).ConfigureAwait(false);

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.ErrorMessageN = pushErrors;
            return results;
        }

        /// <summary>
        /// 対象を指定し、診察呼出プッシュ通知を実施する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<QoWaitingListPushSendApiResults> SendPush(QoWaitingListPushSendApiArgs args)
        {
            QoAccessLog.WriteInfoLog($"検証ログ:プッシュ通知処理受信");

            var results = new QoWaitingListPushSendApiResults
            {
                IsSuccess = bool.FalseString
            };

            var tmpresults = new QoWaitingListWriteApiResults
            {
                IsSuccess = bool.FalseString,
                ErrorMessageN = new List<string>(),
                Result = null
            };

            var rootLinkageSystemNo = args.RootLinkageSystemNo.TryToValueType(int.MinValue);
            if (rootLinkageSystemNo == int.MinValue)
            {
                rootLinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO;
            }

            var facilityKey = args.FacilityKey.TryToValueType(Guid.Empty);
            var dataType = args.DataType.TryToValueType((byte)DataType.None);
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(0);
            var messageResource = CreateFacilityLanguageResource(facilityKey);

            // QH_WAITING_DATに変換
            if (!TryConvertEntity(args.WaitingListN, facilityKey, dataType, linkageSystemNo, tmpresults, out var entities))
            {
                results.ErrorMessageN = tmpresults.ErrorMessageN;
                results.Result = tmpresults.Result;
                return results;
            }

            // NotificationHubを初期化
            _pushNotification.Initialize(rootLinkageSystemNo.ToNotificationHubSettings());

            // ユーザーのリストを抽出
            var userList = entities.Select(x => x.LINKAGESYSTEMID).Distinct().ToList();

            // プッシュ通知用の情報をDBより取得
            var notificationUserEntities = _linkageRepo.ReadPushNotificationUserView(rootLinkageSystemNo, linkageSystemNo, userList);

            // Dictionary化（LinkageSystemIdをキーとしてレコードを取得しやすくするため）
            var dict = notificationUserEntities.ToDictionary(x => x.LINKAGESYSTEMID, y => y);

            var errors = new List<string>();
            foreach (var entity in entities)
            {

                // 通知用データを抽出
                if (!dict.TryGetValue(entity.LINKAGESYSTEMID, out var info))
                {
                    errors.Add($"{entity.LINKAGESYSTEMID}の通知IDが見つかりませんでした。");
                    continue;
                }

                // payloadを一旦作成
                var payload = CreateWaitingPayload(WaitingEventType.None, info == null ? Guid.Empty : info.ACCOUNTKEY, entity);

                // Tag情報を作成
                var tags = new List<string>
                {
                    TAG_WAITING,
                    CreateUserIdTag(info.NOTIFICATIONUSERID)
                };

                // url情報を作成
                var url = $"{rootLinkageSystemNo.ToUrlScheme()}home?user={info.ACCOUNTKEY.ToEncrypedReference()}";

                // 呼出通知を送出
                switch (dataType)
                {
                    case (byte)QH_WAITINGLIST_DAT.DataTypeEnum.MedicalTreatment:
                        await SendPush(payload, tags, messageResource[KEY_EXAMINATION_READY], url, false).ConfigureAwait(false);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.DataTypeEnum.Payment:
                        await SendPush(payload, tags, messageResource[KEY_ACCOUNTING_READY], url, false).ConfigureAwait(false);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.DataTypeEnum.Dispensing:
                        await SendPush(payload, tags, messageResource[KEY_MEDICINE_READY], url, false).ConfigureAwait(false);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.DataTypeEnum.None:
                        // noneで渡したらサイレント通知
                        await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                        break;
                }
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.ErrorMessageN = errors;
            return results;

        }

        bool CheckArgsWaitingList(List<QoApiWaitingListItem> waitingListItemN, QoApiResultsBase results)
        {
            if (waitingListItemN == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(QoWaitingListWriteApiArgs.WaitingListN)}は必須です。");
                return false;
            }

            foreach (var item in waitingListItemN)
            {
                //Time は、HHss
                if (!string.IsNullOrWhiteSpace(item.ReceptionTime) && (item.ReceptionTime.Length != 4
                    || !byte.TryParse(item.ReceptionTime.Substring(0, 2), out byte recH) || !byte.TryParse(item.ReceptionTime.Substring(2, 2), out byte recM) ||
                    recH >= 24 || recM >= 60))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(QoWaitingListWriteApiArgs.WaitingListN)}の受付時間の設定が間違っています");
                    return false;
                }
                if (!string.IsNullOrWhiteSpace(item.ReservationTime) && (item.ReservationTime.Length != 4
                    || !byte.TryParse(item.ReservationTime.Substring(0, 2), out byte resH) || !byte.TryParse(item.ReservationTime.Substring(2, 2), out byte resM) ||
                    resH >= 24 || resM >= 60))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(QoWaitingListWriteApiArgs.WaitingListN)}の予約時間の設定が間違っています");
                    return false;
                }

                // 日付形式チェック
                if(!DateTime.TryParse(item.WaitingDate, out var _))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(QoWaitingListWriteApiArgs.WaitingListN)}の{nameof(item.WaitingDate)}が不正です。");
                    return false;
                }
            }

            return true;
        }

        bool TryConvertEntity(List<QoApiWaitingListItem> waitingListItems, Guid facilityKey, byte dataType, int linkageSystemNo, QoWaitingListWriteApiResults results, out List<QH_WAITINGLIST_DAT> entities)
        {
            try
            {
                entities = waitingListItems
                    .Select(x => BuildWaitingListEntity(facilityKey, dataType, linkageSystemNo, x))
                    .Where(x => x != null && x.IsKeysValid()).ToList();

                return true;
            }
            catch(Exception ex)
            {
                entities = null;
                results.Result = QoApiResult.Build(ex, "順番待ちリストの変換に失敗しました。");
                return false;
            }
        }

        bool TryWriteWaitingEntities(List<QH_WAITINGLIST_DAT> entities, QoWaitingListWriteApiResults results, out List<QH_WAITINGLIST_DAT> oldList)
        {
            try
            {
                (var isSuccess, var old, var errorMessages) = _waitingRepo.WriteList(entities);
                if (!isSuccess)
                {                    
                    results.ErrorMessageN = errorMessages;
                    throw new Exception();
                }

                oldList = old;
                return true;
            }
            catch(Exception ex)
            {
                oldList = null;
                results.Result = QoApiResult.Build(ex, "順番待ち情報リストのDB書き込み処理に失敗しました。");
                return false;
            }
        }
        
        bool TryWriteOrderEntities(List<QH_WAITINGLIST_DAT> entities, QoWaitingListWriteApiResults results, out List<(QH_WAITINGLIST_DAT entity, bool pushFlag)> pushList)
        {           
            pushList = new List<(QH_WAITINGLIST_DAT, bool)>();
            
            try
            {               
                //QH_WAITINGORDERLIST_DAT（診察順番取得情報テーブル）に書込み
                foreach (var entity in entities)
                {                    
                    //医師コード取得
                    var value = new QsJsonSerializer().Deserialize<QhWaitingListValueOfJson>(entity.VALUE);
                    var doctorCode = value != null ? value.DoctorCode : string.Empty;
                    var sameDaySequence = value != null ? value.SameDaySequence : string.Empty;

                    int djKbn = int.MinValue;
                    int.TryParse(sameDaySequence, out djKbn);

                    //中待ち通知を行うか判定
                    var pushFlag = false;

                    switch (entity.STATUSTYPE)
                    {
                        //順番待ち情報取得テーブルから診察室呼出、診察キャンセルがあった患者を削除
                        case (byte)StatusType.None:
                        case (byte)StatusType.Accepted:
                        case (byte)StatusType.CalledToRoom:
                        case (byte)StatusType.EndOfExamination:
                        case (byte)StatusType.Absence:
                        case (byte)StatusType.UnderInspection:
                            //削除対象が一度も情報取得テーブルに書き込まれていない場合通常の診察受付のため中待ち通知は行わない
                            if (_waitingRepo.DeleteWaitingOrderList(entity.LINKAGESYSTEMNO, entity.DEPARTMENTCODE, doctorCode, entity.LINKAGESYSTEMID, djKbn))
                            {
                                pushFlag = true;
                            }
                            break;
                        //カルテ提出時データ登録
                        case (byte)StatusType.SubmittedKarte:
                            _waitingRepo.UpsertWaitingOrderList(
                                new QH_WAITINGORDERLIST_DAT
                                {
                                    LINKAGESYSTEMNO = entity.LINKAGESYSTEMNO,
                                    DEPARTMENTCODE = entity.DEPARTMENTCODE,
                                    DOCTORCODE = doctorCode,
                                    LINKAGESYSTEMID = entity.LINKAGESYSTEMID,
                                    DJKBN = sameDaySequence,
                                    PUSHSENDFLAG = false,
                                    RESERVATIONDATE = (entity.RESERVATIONDATE.Hour == 0 && entity.RESERVATIONDATE.Minute == 0 && entity.RESERVATIONDATE.Second == 0) ?
                                        DateTime.Parse(entity.RESERVATIONDATE.ToString("yyyy-MM-dd 23:59:59")) : entity.RESERVATIONDATE,
                                    RECEPTIONDATE = entity.RECEPTIONDATE,
                                    RECEPTIONNO = entity.RECEPTIONNO,
                                    DELETEFLAG = false,
                                    CREATEDDATE = DateTime.Now,
                                    UPDATEDDATE = DateTime.Now
                                }
                            );
                            pushFlag = true;                            
                            break;
                        // カルテ再提出は中待ちには関与しない
                        case (byte)StatusType.ResubmittedKarte:
                            break;
                        default:
                            break;
                    }

                    pushList.Add((entity, pushFlag));
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "順番待ち人数情報のDB書き込み処理に失敗しました。");
                return false;
            }
        }

        internal async Task<List<string>> NotifyIfNeeds(List<(QH_WAITINGLIST_DAT entity, bool pushFlag)> entities, List<QH_WAITINGLIST_DAT> oldList, int rootLinkageSystemNo, int linkageSystemNo, Dictionary<string, string> messageResource)
        {
            var errors = new List<string>();
            try
            {
                // NotificationHubを初期化
                _pushNotification.Initialize(rootLinkageSystemNo.ToNotificationHubSettings());

                // ユーザーのリストを抽出
                var userList = entities.Select(x => x.entity.LINKAGESYSTEMID).Distinct().ToList();

                // プッシュ通知用の情報をDBより取得
                var notificationUserEntities = _linkageRepo.ReadPushNotificationUserView(rootLinkageSystemNo, linkageSystemNo, userList);

                // Dictionary化（LinkageSystemIdをキーとしてレコードを取得しやすくするため）
                var dict = notificationUserEntities.ToDictionary(x => x.LINKAGESYSTEMID, y => y);

                foreach (var items in entities)
                {
                    var item = items.entity;
                    var pushFlag = items.pushFlag;
                    var old = oldList?.FirstOrDefault(m => m.FOREIGNKEY == item.FOREIGNKEY);
                    // 更新データでStatusTypeにも削除フラグにも変化がなければスキップ
                    // ただし診察待ち特殊の場合はスキップしない
                    if (old != null && old.STATUSTYPE == item.STATUSTYPE && old.DELETEFLAG == item.DELETEFLAG && item.STATUSTYPE != (byte)StatusType.WaitingForExamination)
                    {                       
                        continue;
                    }

                    // 通知用データを抽出
                    if (!dict.TryGetValue(item.LINKAGESYSTEMID, out var info))
                    {
                        QoAccessLog.WriteAccessLog(QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SendPush_ExistError",
                                string.Format("即時通知失敗 LinkageSystemId:{0} / {1}", item.LINKAGESYSTEMID, $"{item.LINKAGESYSTEMID}の通知IDが見つかりませんでした。"), null, null, null);
                        errors.Add($"{item.LINKAGESYSTEMID}の通知IDが見つかりませんでした。");
                    }
                    //else
                    //{
                    //    QoAccessLog.WriteAccessLog(QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SendPush_Start",
                    //                string.Format("即時通知開始 NotificationId:{0} ", info.NOTIFICATIONUSERID), null, null, null);
                    //}

                    if (item.DELETEFLAG && info != null)
                    {
                        // 削除フラグがある場合は削除通知処理へ
                        await SendDeletePush(info, item, rootLinkageSystemNo).ConfigureAwait(false);
                    }
                    else
                    {
                        // 通常イベント通知処理
                        await SendEventPush(info, item, rootLinkageSystemNo, messageResource).ConfigureAwait(false);                        
                        if (pushFlag)
                        {
                            // 中待ち対象の場合は中待ち送信処理へ
                            await SendOrderPush(item, rootLinkageSystemNo, messageResource, errors).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                errors.Add("Push通知処理内でエラーが発生しました。");
            }

            return errors;            
        }

        async Task SendEventPush(QH_LINKAGE_PUSHNOTIFICATION_VIEW pushInfoView, QH_WAITINGLIST_DAT item, int rootLinkageSystemNo, Dictionary<string, string> messageResource)
        {
            // ユーザー情報が無い場合は何もしない
            if (pushInfoView == null)
            {
                return;
            }

            // 一旦Noneで作成
            var payload = CreateWaitingPayload(WaitingEventType.None, pushInfoView == null? Guid.Empty : pushInfoView.ACCOUNTKEY, item);            

            var tags = new List<string>
            {
                TAG_WAITING,
                CreateUserIdTag(pushInfoView.NOTIFICATIONUSERID)
            };

            var url = $"{rootLinkageSystemNo.ToUrlScheme()}home?user={pushInfoView.ACCOUNTKEY.ToEncrypedReference()}";

            switch (item.STATUSTYPE)
            {
                case (byte)StatusType.Accepted:
                    //受付済
                    payload.EventType = WaitingEventType.ExaminationQueueStart;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false); 
                    break;

                case (byte)StatusType.SubmittedKarte:
                    // カルテ提出済み アプリには順番待ち変更として通知する
                    payload.EventType = WaitingEventType.ExaminationQueueChanged;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    break;
                case (byte)StatusType.WaitingForExamination:
                    // 診察待ち アプリには順番待ち変更として通知する
                    payload.EventType = WaitingEventType.ExaminationQueueChangedSpecial;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    break;

                case (byte)StatusType.CalledToRoom:
                    //診察室呼び出し済み
                    payload.EventType = WaitingEventType.ExaminationReady;
                    // 通常の通知
                    await SendPush(payload, tags, messageResource[KEY_EXAMINATION_READY], url, false).ConfigureAwait(false);
                    break;

                case (byte)StatusType.EndOfExamination:
                    //診察終了
                    payload.EventType = WaitingEventType.ExaminationEnd;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    break;

                case (byte)StatusType.Absence:
                    // 診察時不在
                    payload.EventType = WaitingEventType.ExaminationAbsence;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    break;

                case (byte)StatusType.UnderInspection:
                    // 診察（検査中）
                    payload.EventType = WaitingEventType.ExaminationInTest;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    break;

                case (byte)StatusType.ResubmittedKarte:
                    // カルテ再提出
                    payload.EventType = WaitingEventType.ExaminationResume;
                    // サイレントで通知
                    await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    break;

                case (byte)StatusType.Pending:
                    // 実際の電子カルテからはこれは呼ばれない(デバッグ用)
                    // 薬受付
                    if (item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicineQueueStart;
                        // サイレント通知
                        await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    }
                    // 会計受付
                    if (item.DATATYPE == (byte)DataType.Payment)
                    {                        
                        payload.EventType = WaitingEventType.AccountingQueueStart;
                        // サイレント通知
                        await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    }
                    break;

                case (byte)StatusType.Called:
                    // 薬呼び出し
                    if (item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicineReady;
                        // 通常の通知
                        await SendPush(payload, tags, messageResource[KEY_MEDICINE_READY], url, false).ConfigureAwait(false);
                    }
                    // 会計呼び出し
                    if (item.DATATYPE == (byte)DataType.Payment)
                    {
                        payload.EventType = WaitingEventType.AccountingReady;
                        // 通常の通知
                        await SendPush(payload, tags, messageResource[KEY_ACCOUNTING_READY], url, false).ConfigureAwait(false); 
                    }
                    break;

                case (byte)StatusType.Completed:
                    // 薬完了
                    if (item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicineEnd;
                        // サイレント通知
                        await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    }
                    // 会計完了
                    if (item.DATATYPE == (byte)DataType.Payment)
                    {
                        payload.EventType = WaitingEventType.AccountingEnd;
                        // サイレント通知
                        await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    }
                    break;

                case (byte)StatusType.HasMedicine:
                    // 薬待ちのシステムがない場合で薬の準備がある特殊な状態
                    if (item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicineReadySpecial;
                        // 通常通知
                        await SendPush(payload, tags, messageResource[KEY_MEDICINE_READY_SPECIAL], url, false).ConfigureAwait(false); 
                    }
                    break;
                case (byte)StatusType.PostpayMedicinePending:
                    // 後払い会計 薬お渡し準備中
                    if(item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicinePostpayQueueStart;
                        // サイレント通知
                        await SendPush(payload, tags, string.Empty, url, true).ConfigureAwait(false);
                    }
                    break;
                case (byte)StatusType.PostpayMedicineCalled:
                    // 後払い会計 薬お渡し準備完了
                    if (item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicinePostpayReady;
                        // 通常通知
                        await SendPush(payload, tags, messageResource[KEY_MEDICINE_POSTPAY_READY], url, false).ConfigureAwait(false);
                    }
                    break;
                case (byte)StatusType.PostpayExternalPrescription:
                    // 院外処方
                    if(item.DATATYPE == (byte)DataType.Dispensing)
                    {
                        payload.EventType = WaitingEventType.MedicinePostpayExternal;
                        // 通常通知
                        await SendPush(payload, tags, messageResource[KEY_MEDICINE_POSTPAY_EXTERNAL], url, false).ConfigureAwait(false);
                    }
                    break;
                default:
                    break;

            }
        }

        async Task SendOrderPush(QH_WAITINGLIST_DAT item, int rootLinkageSystemNo, Dictionary<string, string> messageResource, List<string> errors)
        {
            // 医師コード取得
            var doctorCode = new QsJsonSerializer().Deserialize<QhWaitingListValueOfJson>(item.VALUE).DoctorCode;
            int sameDaySequence = 0;
            int.TryParse(new QsJsonSerializer().Deserialize<QhWaitingListValueOfJson>(item.VALUE).SameDaySequence, out sameDaySequence);

            // 診療科設定取得
            var config = _waitingRepo.GetMedicalDepartmentConfig(item.LINKAGESYSTEMNO, item.DEPARTMENTCODE);
            if (config.FACILITYKEY == Guid.Empty)
            {
                return;
            }

            // ソート順設定を読み込み
            var appConfig = _waitingRepo.GetMedicalDepartmentAppConfig(item.LINKAGESYSTEMNO, item.DEPARTMENTCODE);
            var priorityType = _waitingRepo.GetWaitingPriorityType(appConfig.VALUE);

            //順番情報取得
            var orderList = _waitingRepo.GetWaitingOrderListEntity(item.LINKAGESYSTEMNO, item.DEPARTMENTCODE, doctorCode, config, item.RESERVATIONDATE, priorityType, sameDaySequence);

            // ユーザーのリストを抽出
            var userList = orderList.Select(x => x.LINKAGESYSTEMID).Distinct().ToList();

            // プッシュ通知用の情報をDBより取得
            var notificationUserEntities = _linkageRepo.ReadPushNotificationUserView(rootLinkageSystemNo, item.LINKAGESYSTEMNO, userList);

            // Dictionary化（LinkageSystemIdをキーとしてレコードを取得しやすくするため）
            var dict = notificationUserEntities.ToDictionary(x => x.LINKAGESYSTEMID, y => y);


            //対象者に通知処理
            foreach (var user in orderList)
            {
                if (user.PUSHSENDFLAG)
                {
                    // 通知済みならスキップ
                    continue;
                }                
                
                if (!dict.TryGetValue(user.LINKAGESYSTEMID, out var pushView))
                {
                    errors.Add($"{user.LINKAGESYSTEMID}の通知IDが見つかりませんでした。");
                    // Push通知ユーザー情報が存在しなければスキップ
                    continue;
                }                

                // 中待ち通知を送信
                var sendAccountKey = pushView.ACCOUNTKEY.ToEncrypedReference();
                var payload = new WaitingNotificationPayload
                {
                    AccountKeyReference = sendAccountKey,
                    EventType = WaitingEventType.ExaminationSoon,
                    FacilityKeyReference = item.FACILITYKEY.ToEncrypedReference(),
                    ReceiptNumber = user.RECEPTIONNO,
                    ReservationNo = string.Empty, // 予約番号は取れないので空
                    CreatedAt = DateTime.Now
                };

                await SendPush(
                    payload,
                    new List<string> { TAG_WAITING, CreateUserIdTag(pushView.NOTIFICATIONUSERID) },
                    messageResource[KEY_EXAMINATION_SOON],
                    $"{rootLinkageSystemNo.ToUrlScheme()}home?user={sendAccountKey}",
                    false).ConfigureAwait(false);
                //通知フラグ更新

                user.PUSHSENDFLAG = true;
                _waitingRepo.UpsertWaitingOrderList(user);
            }
        }

        async Task SendDeletePush(QH_LINKAGE_PUSHNOTIFICATION_VIEW pushInfoView, QH_WAITINGLIST_DAT item,int rootLinkageSystemNo)
        {
            WaitingEventType waitingEventType;
            switch ((DataType)item.DATATYPE)
            {
                case DataType.MedicalTreatment:
                    waitingEventType = WaitingEventType.ExaminationEnd;
                    break;
                case DataType.Dispensing:
                    waitingEventType = WaitingEventType.MedicineEnd;
                    break;
                case DataType.Payment:
                    waitingEventType = WaitingEventType.AccountingEnd;
                    break;
                default:
                    waitingEventType = WaitingEventType.None;
                    break;
            }

            var payload = CreateWaitingPayload(waitingEventType, pushInfoView.ACCOUNTKEY, item);

            var tags = new List<string>
            {
                TAG_WAITING,
                CreateUserIdTag(pushInfoView.NOTIFICATIONUSERID)
            };

            var url = $"{rootLinkageSystemNo.ToUrlScheme()}home?user={pushInfoView.ACCOUNTKEY.ToEncrypedReference()}";

            // 削除された場合は中断とみなしサイレントで終了通知を送る
            await SendPush(payload ,tags, string.Empty, url, true).ConfigureAwait(false);
            return;
        }

        async Task SendPush(WaitingNotificationPayload payload, List<string> tags, string message = "",string url = "", bool isSilent = false)
        {
            var payloadJson = CreatePayloadJson(payload);

            var request = new NotificationRequest
            {
                Extra = payloadJson,
                Silent = isSilent,
                Text = message,
                Url = url,
                Badge = 1,
                Title = MSG_TITLE,
            };
            request.SetTagExpressionJoinAllAnd(tags.ToArray());

            string[] result = await _pushNotification.RequestNotificationAsync(request).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result[0]) && string.IsNullOrEmpty(result[1]))
                QoAccessLog.WriteErrorLog(string.Format("順番待ちPush送信失敗:{0}", payload), Guid.Empty);
        }

        static string CreateUserIdTag(string userId)
        {
            return "$UserId:{" + userId.ToEncrypedReference() + "}";
        }

        static WaitingNotificationPayload CreateWaitingPayload(WaitingEventType eventType, Guid accountkey, QH_WAITINGLIST_DAT waitingItem)
        {
            return new WaitingNotificationPayload
            {
                AccountKeyReference = accountkey.ToEncrypedReference(),
                EventType = eventType,
                FacilityKeyReference = waitingItem.FACILITYKEY.ToEncrypedReference(),
                ReceiptNumber = waitingItem.RECEPTIONNO,
                ReservationNo = waitingItem.RESERVATIONNO,
                CreatedAt = DateTime.UtcNow
            };
        }

        static string CreatePayloadJson(WaitingNotificationPayload payload)
        {
            var result = string.Empty;
            try
            {
                result = new QsJsonSerializer().Serialize(payload);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
            return result;
        }

        private static QH_WAITINGLIST_DAT BuildWaitingListEntity(Guid facilityKey, byte dataType, int linkageSystemNo, QoApiWaitingListItem target)
        {
            if (target == null)
            {
                return null;                
            }

            var waitingDate = target.WaitingDate.ToValueType<DateTime>().Date;
            var jsonValue = new QhWaitingListValueOfJson();

            if (target.Detail != null)
            {
                jsonValue = new QhWaitingListValueOfJson()
                {
                    InOutType = target.Detail.InOutType,
                    SameDaySequence = target.Detail.SameDaySequence,
                    DoctorCode = target.Detail.DoctorCode,
                    DoctorName = target.Detail.DoctorName,
                    DepartmentName = target.DepartmentName,
                    MedicalActCode = target.Detail.MedicalActCode,
                    MedicalActName = target.Detail.MedicalActName,
                    RoomCode = target.Detail.RoomCode,
                    RoomName = target.Detail.RoomName,
                    DosingSlipNo = target.Detail.DosingSlipNo,
                    DosingSlipType = target.Detail.DosingSlipType,
                    DefferedPaymentFlg = target.Detail.DefferedPaymentFlg,
                    ChartWaitNumber = target.Detail.ChartWaitNumber
                };
            }

            return new QH_WAITINGLIST_DAT()
            {
                FACILITYKEY = facilityKey,
                WAITINGDATE = waitingDate,
                DATATYPE = dataType,
                SEQUENCE = 0, // 現時点では未確定、IsKeyValidが通るように初期値0
                LINKAGESYSTEMNO = linkageSystemNo,
                LINKAGESYSTEMID = target.LinkageSystemId,
                DEPARTMENTCODE = target.DepartmentCode,
                // STATUSTYPEは未定義値が来ても受け入れる
                STATUSTYPE = target.StatusType.TryToValueType((byte)StatusType.None),
                RECEPTIONDATE = AddTime(waitingDate, target.ReceptionTime),
                RECEPTIONNO = target.ReceptionNo,
                RESERVATIONDATE = AddTime(waitingDate, target.ReservationTime),
                RESERVATIONNO = target.ReservationNo,
                FOREIGNKEY = target.ForeignKey,
                VALUE = new QsJsonSerializer().Serialize(jsonValue),
                DELETEFLAG = target.DeleteFlag.TryToValueType(false),
                CREATEDDATE = DateTime.Now,
                UPDATEDDATE = DateTime.Now
            };
        }

        private static DateTime AddTime(DateTime date, string time)
        {
            DateTime result = DateTime.MinValue;
            string value = string.Format("{0}{1}00", date.ToString("yyyyMMdd"), time);
            DateTime d = DateTime.MinValue;
            if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out d))
            {
                result = d;
            }

            return result;
        }

        /// <summary>
        /// 施設固有の言語リソースを作成する
        /// </summary>
        /// <param name="facilityKey">施設キー</param>
        /// <returns>言語リソース (日本語限定)</returns>
        internal bool TryCreateFacilityLanguageResource(Guid facilityKey, QoApiResultsBase results, out Dictionary<string, string> resource)
        {
            try
            {
                resource = CreateFacilityLanguageResource(facilityKey);
                return true;
            }
            catch (Exception ex)
            {
                resource = null;
                results.Result = QoApiResult.Build(ex, "施設言語取得処理に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// 施設固有の言語リソースを作成する
        /// </summary>
        /// <param name="facilityKey">施設キー</param>
        /// <returns>言語リソース (日本語限定)</returns>
        internal Dictionary<string, string> CreateFacilityLanguageResource(Guid facilityKey)
        {
            return _facilityRepo.ReadFacilityLanguage(facilityKey)
                .Select(x => new 
                    { 
                        Key = x.LANGUAGEKEY, 
                        Value = JsonConvert.DeserializeObject<FacilityLanguageResourceItem[]>(x.VALUE) 
                    })
                .ToDictionary(x => x.Key, y => y.Value.FirstOrDefault(a => a.Language == 0).Value); // 日本語のみ
        }

        /// <summary>
        /// 言語リソースのアイテム
        /// </summary>
        internal class FacilityLanguageResourceItem
        {
            /// <summary>
            /// 言語種別 (0: 日本語, 1: 英語)
            /// </summary>
            public int Language { get; set; }

            /// <summary>
            /// リソース値
            /// </summary>
            public string Value { get; set; }
        }

    }
}