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
using SPCryptoEngine;
using MGF.QOLMS.QolmsOpenApi.Models;

using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Tis関連の処理
    /// 汎用のWaitingApiに統合されます。
    /// </summary>
    [Obsolete("廃止予定です。それぞれ統合化された汎用Workerを使ってください。")]
    public sealed class TisWorker
    {
        private static QoPushNotification _notificationHubService = new QoPushNotification(QoApiConfiguration.TisNotificationHubConnectionString, QoApiConfiguration.TisNotificationHubName);

        private static readonly char[] _separator= new [] {'#'};

        #region "Private Method"
        //医療機関コードにより、FacilityマスタからFacilityKeyを取得して返す。
        private static string GetFacilityKeyReference(string medicalFacilityKey)
        {
            string result = string.Empty;
            var readerArgs = new FacilityMasterReaderArgs() { MedicalFacilityCode = medicalFacilityKey };
            var readerResult = QsDbManager.Read(new FacilityMasterReader(), readerArgs);
            if(readerResult!=null && readerResult.IsSuccess && readerResult.Result!=null && readerResult.Result.Count==1)
            {
                result = readerResult.Result.First().FACILITYKEY.ToEncrypedReference();
            }
            else
            {
                QoAccessLog.WriteErrorLog(string.Format("Facility情報の取得に失敗しました 医療機関番号:{0}",medicalFacilityKey), Guid.Empty);
            }
            return result;
        }

        //SSI 復号
        private static string Decrypt(string qrData)
        {
            try
            {
                KeyGenerator generator = new KeyGenerator();
                return generator.GetDecryptoKey(qrData);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, string.Format("暗号化されたQrコードの復号に失敗：{0}", qrData), Guid.Empty);
            }
            return string.Empty;
        }

        //SSIの性別をQOLMSの性別に変換
        private static string GetQolmsSexType(string ssiSexType)
        {
            switch (ssiSexType.Trim() )
            {
                case "1":
                    return "1";
                case "3":
                    return "2";

                default:
                    break;
            }
            return "0";
        }

        /// <summary>
        /// 有効期限をチェックします。
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        private static bool CheckExpiration(string createdDate)
        {
            if (DateTime.TryParseExact(createdDate, "yyyyMMddHHmm",null, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime d))
            {
                int h = QoApiConfiguration.TisQrExpirationHour.ReceptionExpirationHour;
                if (d.AddHours(h) >= DateTime.Now)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// API用の順番待ち情報をテーブル エンティティに変換します。
        /// </summary>
        /// <param name="facilityKey">施設キー。</param>
        /// <param name="dataType">順番待ちデータ種別。</param>
        /// <param name="linkageSystemNo">連携システム番号。</param>
        /// <param name="target">変換元順番待ち情報。</param>
        /// <returns>順番待ち データ テーブル エンティティ。</returns>
        private static QH_WAITINGLIST_DAT BuildWaitingListEntity(Guid facilityKey, byte dataType, int linkageSystemNo, QoApiWaitingListItem target)
        {
            QH_WAITINGLIST_DAT result = null;

            if (target != null)
            {
                DateTime waitingDate = target.WaitingDate.ToValueType<DateTime>().Date ;
                QhWaitingListValueOfJson jsonValue = new QhWaitingListValueOfJson();

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
                        DosingSlipType = target.Detail.DosingSlipType
                    };
                }

                result = new QH_WAITINGLIST_DAT()
                {
                    FACILITYKEY = facilityKey,
                    WAITINGDATE = waitingDate,
                    DATATYPE = dataType,
                    SEQUENCE = 0, // 現時点では未確定、IsKeyValidが通るように初期値0
                    LINKAGESYSTEMNO = linkageSystemNo,
                    LINKAGESYSTEMID = target.LinkageSystemId,
                    DEPARTMENTCODE = target.DepartmentCode,
                    STATUSTYPE = target.StatusType.ToValueType<byte>(),
                    RECEPTIONDATE = TisWorker.AddTime(waitingDate, target.ReceptionTime),
                    RECEPTIONNO = target.ReceptionNo,
                    RESERVATIONDATE = TisWorker.AddTime(waitingDate, target.ReservationTime),
                    RESERVATIONNO = target.ReservationNo,
                    FOREIGNKEY = target.ForeignKey,
                    VALUE = new QsJsonSerializer().Serialize(jsonValue),
                    DELETEFLAG = target.DeleteFlag.ToValueType<bool>(),
                    CREATEDDATE = DateTime.Now,
                    UPDATEDDATE = DateTime.Now
                };
            }

            return result;
        }

        /// <summary>
        /// 順番待ち対象日に受付時間、予約時間を付加します。
        /// </summary>
        /// <param name="date">順番待ち対象日。</param>
        /// <param name="time">受付時間、予約時間(HHmm形式)の文字列。</param>
        /// <returns>DateTime形式の受付時間、予約時間。</returns>
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
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoTisWaitingListWriteApiArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.DataType) || string.IsNullOrWhiteSpace(args.FacilityKey) || string.IsNullOrWhiteSpace(args.LinkageSystemNo) ||
               args.WaitingListN==null)
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "引数が不足しています");

            foreach (var item in args.WaitingListN)
            {
                //Time は、HHss
                if (!string.IsNullOrWhiteSpace(item.ReceptionTime) && (item.ReceptionTime.Length != 4
                    || !byte.TryParse(item.ReceptionTime.Substring(0, 2), out byte recH) || !byte.TryParse(item.ReceptionTime.Substring(2, 2), out byte recM) ||
                    recH > 24 || recM > 60 ))
                    return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "時間の設定が間違っています");
                if(!string.IsNullOrWhiteSpace(item.ReservationTime ) &&( item.ReservationTime .Length !=4 
                    || !byte.TryParse(item.ReservationTime .Substring(0,2), out byte resH) || !byte.TryParse(item.ReservationTime.Substring(2,2), out byte resM) ||
                    resH>24 || resM>60))
                    return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "時間の設定が間違っています");

            }
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }
        //パラメータチェック
        private static QoApiResultItem CheckArgs(QoTisWaitingListReadApiArgs args)
        {
            //対象の設定があるか
            if (args.ActorKey.TryToValueType(Guid.Empty) == Guid.Empty
                || args.AuthorKey.TryToValueType(Guid.Empty )==Guid.Empty )
                return QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "対象が特定できません");
            
            
           
            return QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
        }

        //連携システム番号と患者番号から アカウントキー(子アカウントの可能性あり)とTIS連携ID（アプリ用のUserId）を返す
        internal static (Guid accountKey, string userId) ChangePatientIdToAccount(int linkageSystemNo, string patientId)
        {
            var patientLinkageresult = QsDbManager.Read(new LinkageUserReader(), new LinkageUserReaderArgs() { LinkageSystemId = patientId, LinkageSystemNo = linkageSystemNo, StatusNo = 2 });
            if(patientLinkageresult.IsSuccess )
            {
                var tisLinkageResult = QsDbManager.Read(new FamilyParentAccountReader(), new FamilyParentAccountReaderArgs()
                {
                    AccountKey = patientLinkageresult.AccountKey,
                    LinkageSystemNo =QoLinkage.TIS_LINKAGE_SYSTEM_NO 
                });
                if(tisLinkageResult.IsSuccess)
                {
                    return (patientLinkageresult.AccountKey, tisLinkageResult.LinkageId);
                }

            }    
            return( Guid.Empty, string.Empty);
        }

        //順番待ち用 Payload 作成
        internal static string CreateWaitingNotificationPayload(TisWaitingEventType eventType, Guid accountkey, Guid facilityKey, string receiptNumber, string reservationNo = "")
        {
            TisWaitingNotificationPayload payload = new TisWaitingNotificationPayload
            {
                AccountKeyReference = accountkey.ToEncrypedReference(),
                EventType = eventType,
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                ReceiptNumber = receiptNumber,
                ReservationNo = reservationNo,
                CreatedAt = DateTime.UtcNow
            };
            var result = string.Empty;
            try
            {
                result =new QsJsonSerializer().Serialize(payload);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
            return result;
        }
        //Push
        internal static async Task SendPush(string payload, Guid accountKey, List<string> tags,  string message = "", bool isSilent = false)
        {
            
            var request = new NotificationRequest
            {
                Extra = payload,
                Silent = isSilent,
                Text = message,
                Url = $"heart-plus://home?user={accountKey.ToEncrypedReference()}",
                Badge = 1,
                Title = "順番待ち",
            };
            request.SetTagExpressionJoinAllAnd(tags.ToArray());
            string[] result = await _notificationHubService.RequestNotificationAsync(request);
            if (string.IsNullOrEmpty(result[0]) && string.IsNullOrEmpty(result[1]) )
                QoAccessLog.WriteErrorLog(string.Format("順番待ちPush送信失敗:{0}", payload),Guid.Empty );
        }


        //必要なら通知を実行
        private static async Task NotifyIfNeeds(List<QH_WAITINGLIST_DAT> entities, List<QH_WAITINGLIST_DAT>  oldList ) 
        {
            foreach (var item in entities)
            {
                var old = oldList.FirstOrDefault(m => m.FOREIGNKEY == item.FOREIGNKEY);
                // 更新データでStatusTypeにも削除フラグにも変化がなければスキップ
                if(old != null && old.STATUSTYPE == item.STATUSTYPE && old.DELETEFLAG == item.DELETEFLAG)
                {
                    continue;
                }

                (Guid accountKey, string userId) = ChangePatientIdToAccount(item.LINKAGESYSTEMNO, item.LINKAGESYSTEMID);

                if (item.DELETEFLAG)
                {
                    TisWaitingEventType waitingEventType;
                    switch ((QH_WAITINGLIST_DAT.DataTypeEnum)item.DATATYPE)
                    {
                        case QH_WAITINGLIST_DAT.DataTypeEnum.MedicalTreatment:
                            waitingEventType = TisWaitingEventType.ExaminationEnd;
                            break;
                        case QH_WAITINGLIST_DAT.DataTypeEnum.Dispensing:
                            waitingEventType = TisWaitingEventType.MedicineEnd;
                            break;                        
                        case QH_WAITINGLIST_DAT.DataTypeEnum.Payment:
                            waitingEventType = TisWaitingEventType.AccountingEnd;
                            break;
                        default:
                            waitingEventType = TisWaitingEventType.None;
                            break;
                    }

                    // 削除された場合は中断とみなし終了通知を送る
                    await SendPush(CreateWaitingNotificationPayload(waitingEventType, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                    new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, string.Empty, true);
                    return;
                }

                switch (item.STATUSTYPE)
                {
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.Accepted:
                        //受付済
                        await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.ExaminationQueueStart, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO),accountKey,
                                    new List<string>() {"Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, string.Empty, true);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.SubmittedKarte:
                        //カルテ提出済み
                            await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.ExaminationSoon, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO),accountKey,
                                    new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, "まもなく診察が始まります。待合でお待ちください。", false);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.CalledToRoom:
                        //診察室呼び出し済み
                        await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.ExaminationReady, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO),accountKey,
                                    new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, "診察が始まります。診察室にお入りください。", false);
                           
                        //TODO: 一人呼び出したらその後ろ的にはステータス変更?
                        //eventType = TisWaitingEventType.ExaminationQueueChanged;
                        //isSilent = true;
                        break;
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.EndOfExamination:
                        //診察終了
                        await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.ExaminationEnd, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, string.Empty, true);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.Called:
                        //呼出し済み
                        if (item.DATATYPE == (byte)QH_WAITINGLIST_DAT.DataTypeEnum.Dispensing)
                            await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.MedicineReady, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, "お薬の準備が整いました。受け取りにお越しください。", false);
                        if(item.DATATYPE==(byte)QH_WAITINGLIST_DAT.DataTypeEnum.Payment)
                            await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.AccountingReady, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, "ご精算が可能になりましたので、自動精算機にお越しください。", false);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.Completed:
                        // 薬完了
                        if (item.DATATYPE == (byte)QH_WAITINGLIST_DAT.DataTypeEnum.Dispensing)
                            await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.MedicineEnd, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, string.Empty, true);
                        // 会計完了
                        if (item.DATATYPE == (byte)QH_WAITINGLIST_DAT.DataTypeEnum.Payment)
                            await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.AccountingEnd, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, string.Empty, true);
                        break;
                    case (byte)QH_WAITINGLIST_DAT.StatusTypeEnum.HasMedicine:
                        // 薬待ちのシステムがない場合で薬の準備がある特殊な状態
                        if(item.DATATYPE == (byte)QH_WAITINGLIST_DAT.DataTypeEnum.Dispensing)
                        {
                            await SendPush(CreateWaitingNotificationPayload(TisWaitingEventType.MedicineReadySpecial, accountKey, item.FACILITYKEY, item.RECEPTIONNO, item.RESERVATIONNO), accountKey,
                                new List<string>() { "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" }, "お薬が用意されています。受け取りにお越しください。", false);
                        }
                        break;
                    default:
                        break;
                }
                //TODO: Pushのメッセージを英語にしたりしないとならないので、最終的にはマスタ化する必要がある
            }            
        }
        
    #endregion

        #region "Public Method"

        /// <summary>
        /// 暗号化されたデータの解析を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。代わりにLinkageQrReadWorkerを使ってください。")]
        public static QoTisQrCodeReadApiResults QrCodeRead(QoTisQrCodeReadApiArgs args)
        {
            var result = new QoTisQrCodeReadApiResults() { IsSuccess = bool.FalseString };

            //データがないのは拒否
            if (string.IsNullOrWhiteSpace(args.QRData) && int.TryParse(args.Version,out _))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }
            QoAccessLog.WriteInfoLog(args.QRData);

            string qr = Decrypt(args.QRData);
            QoAccessLog.WriteInfoLog(qr);
            if(!string.IsNullOrEmpty(qr))
            {
                string[] sp = qr.Split(_separator);
                if (sp.Length > 7 )
                {
                    result.LinkUserId = sp[2];
                    // 患者ID99から始まる場合はテスト患者とし有効期限チェックをスキップする。
                    if (result.LinkUserId.StartsWith("99") || CheckExpiration(sp[1].Trim()))
                    {　
                        result.Birthday = sp[0];
                        result.CreatedDate = sp[1];
                        result.SexType = GetQolmsSexType(sp[3]);
                        result.FacilityKeyReference = GetFacilityKeyReference(sp[4]);
                        result.MedicalFacilityCode = sp[4];
                        string[] namesp = sp[5].Split(new[] { '　' }, StringSplitOptions.RemoveEmptyEntries);
                        if (namesp.Length > 1)
                        {
                            result.FamilyName = namesp[0];
                            result.GivenName = namesp[1];
                        }
                        else    //区切られてなかったら全部FamilyNameに入れてくれとのこと。
                        {
                            result.FamilyName = sp[5];
                        }
                        result.FamilyKanaName = Microsoft.VisualBasic.Strings.StrConv(sp[6].Trim(), Microsoft.VisualBasic.VbStrConv.Wide, 0x411);
                        result.GivenKanaName = Microsoft.VisualBasic.Strings.StrConv(sp[7].Trim(), Microsoft.VisualBasic.VbStrConv.Wide, 0x411);

                        result.IsSuccess = bool.TrueString;
                    }
                    else
                    {
                        //有効期限エラー
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountRegisterExpired);
                    }
                }
                else
                {
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,"データ解析失敗");
                }
            }
            else
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,"復号失敗");
            }
           
            return result;
        }

        /// <summary>
        /// 順番待ち情報を書き込みます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。代わりにWaitingListWriteWorkerを使用してください。")]
        public static QoTisWaitingListWriteApiResults WaitingListWrite(QoTisWaitingListWriteApiArgs args)
        {
            QoTisWaitingListWriteApiResults result = new QoTisWaitingListWriteApiResults() { IsSuccess = bool.FalseString ,
                Result = CheckArgs(args)
            };
            if (result.Result.Code.TryToValueType(QoApiResultCodeTypeEnum.None) != QoApiResultCodeTypeEnum.Success)
                return result;

            List<QH_WAITINGLIST_DAT> entities = new List<QH_WAITINGLIST_DAT>();
            Guid facilityKey = args.FacilityKey.ToValueType<Guid>();
            byte dataType = args.DataType.ToValueType<byte>();
            int linkageSystemNo = args.LinkageSystemNo.ToValueType<int>();
            List<string> errorMessages = new List<string>();

            if (args.WaitingListN != null && args.WaitingListN.Any())
            {
                args.WaitingListN.ForEach((i) => {
                    // テーブル エンティティに変換
                    QH_WAITINGLIST_DAT entity = TisWorker.BuildWaitingListEntity(facilityKey, dataType, linkageSystemNo, i);
                    if (entity != null && entity.IsKeysValid())
                    {
                        entities.Add(entity);
                    }
                });
            }
            List<QH_WAITINGLIST_DAT> oldList = new List<QH_WAITINGLIST_DAT>();
            // DBに書き込み       
            try
            {
                bool isSuccess = QolmsLibraryV1.WaitingListWorker.Write(entities,ref oldList, ref errorMessages  );

                result.IsSuccess = isSuccess.ToString();
                if (isSuccess)
                {
                    //Push通知                //失敗時はdb更新
                    Task.Run(() => {
                        return NotifyIfNeeds(entities, oldList);
                    }).GetAwaiter().GetResult();

                }
                else 
                { 
                    result.ErrorMessageN = errorMessages;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "順番待ち情報登録APIの実行に失敗しました。詳細はエラーメッセージを確認してください。");
                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
                QoAccessLog.WriteInfoLog(string.Format("args:{0}", new QsJsonSerializer().Serialize(args) ));               
            }


                return result;

        }

        /// <summary>
        /// 順番待ち情報を書き込みます。（デバッグ用）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoTisWaitingListDebugWriteApiResults WaitingListDebugWrite(QoTisWaitingListDebugWriteApiArgs args)
        {
            var result = new QoTisWaitingListDebugWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            try
            {                
                // 待ち受け日時が未指定なら現在時間を設定
                var waitingDate = string.IsNullOrWhiteSpace(args.WaitingDate) ? DateTime.Now.ToApiDateString() : args.WaitingDate;
                // 受付時間が未指定なら現在時間を設定
                var recepionTime = string.IsNullOrWhiteSpace(args.ReceptionTime) ? DateTime.Now.ToString("HHmm") : args.ReceptionTime;

                var writeArgs = new QoTisWaitingListWriteApiArgs
                {
                    ApiType = QoApiTypeEnum.TisWaitingListWrite.ToString(),
                    ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                    DataType = args.DataType,
                    FacilityKey = args.FacilityKeyReference.ToDecrypedReference(),
                    LinkageSystemNo = args.LinkageSystemNo
                };

                var waitingItem = new QoApiWaitingListItem
                {
                    DepartmentCode = "99",
                    DepartmentName = args.DepartmentName,
                    DeleteFlag = args.IsDelete.TryToValueType(false).ToString(),
                    ForeignKey = args.ForeignKey,
                    LinkageSystemId = args.LinkageSystemId,
                    ReceptionNo = args.ReceptionNo,
                    ReceptionTime = recepionTime,
                    ReservationNo = "999",
                    ReservationTime = args.ReservationTime,
                    StatusType = args.StatusType,
                    WaitingDate = waitingDate,                    
                    Detail = new QoApiWaitingDetailItem
                    {
                        DepartmentName = args.DepartmentName,
                        DoctorCode = "99",
                        DoctorName = args.DoctorName,
                        DosingSlipNo = "",
                        DosingSlipType = "",
                        InOutType = "",
                        MedicalActCode = "",
                        MedicalActName = "",
                        RoomCode = "",
                        RoomName = "",
                        SameDaySequence = "0",                        
                    }
                };

                var reader = new DbWaitingListDebugReaderCore();
                if (string.IsNullOrWhiteSpace(args.ForeignKey))
                {                   
                    var maxKey = reader.GetMaxDebugForeignKey() + 1;
                    waitingItem.ForeignKey = $"99{maxKey:D10}";
                }

                if (string.IsNullOrWhiteSpace(args.ReceptionNo))
                {
                    var nextNo = reader.GetMaxReceptionNoInDay(waitingDate.TryToValueType(DateTime.MinValue)) + 1;
                    waitingItem.ReceptionNo = $"{nextNo:D4}";
                }

                writeArgs.WaitingListN = new List<QoApiWaitingListItem>
                {
                    waitingItem
                };

                var writeResult = WaitingListWrite(writeArgs);

                if(writeResult.IsSuccess == bool.FalseString)
                {
                    result.Result = writeResult.Result;
                    return result;
                }

                result.ForeignKey = waitingItem.ForeignKey;
                result.ReceptionNo = waitingItem.ReceptionNo;
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

                return result;
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                return result;
            }            
        }
         #endregion
    }
}