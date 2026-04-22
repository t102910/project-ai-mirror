using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.DataTypeEnum;
using StatusType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.StatusTypeEnum;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Newtonsoft.Json;
using static MGF.QOLMS.QolmsOpenApi.Worker.WaitingListWriteWorker;

namespace QolmsOpenApi.Tests.Workers
{
    /// <summary>
    /// WaitingListWriteWorker WriteListメソッド テストクラス
    /// 引数などの準備用の定義はSubの方に配置
    /// </summary>
    [TestClass]
    public partial class WaitingListWriteWorkerFixture
    {
        Mock<IWaitingRepository> _waitingRepo;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IQoPushNotification> _pushNotification;
        Mock<IFacilityRepository> _facilityRepo;
        WaitingListWriteWorker _worker;

        Guid _facilityKey1 = Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB");
        Guid _accountKey1 = Guid.NewGuid();
        Guid _accountKey2 = Guid.NewGuid();
        DateTime _targetDate = new DateTime(2024, 10, 10);
        Dictionary<string, string> _messages;

        [TestInitialize]
        public void Initialize()
        {
            _waitingRepo = new Mock<IWaitingRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _pushNotification = new Mock<IQoPushNotification>();
            _facilityRepo = new Mock<IFacilityRepository>();
            _worker = new WaitingListWriteWorker(_waitingRepo.Object, _linkageRepo.Object, _pushNotification.Object, _facilityRepo.Object);

            _messages = GetFacilityLanguageMst()
                .Select(x => new
                {
                    Key = x.LANGUAGEKEY,
                    Value = JsonConvert.DeserializeObject<FacilityLanguageResourceItem[]>(x.VALUE)
                })
                .ToDictionary(x => x.Key, y => y.Value.FirstOrDefault(a => a.Language == 0).Value); // 日本語のみ
        }

        [TestMethod]
        public async Task 必須項目が未設定でエラー()
        {
            // DataType未設定
            var args = GetValidArgs();
            args.DataType = "";

            var results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.DataType)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // FacilityKey未設定
            args = GetValidArgs();
            args.FacilityKey = "";

            results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FacilityKey)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // LinkageSystemNo未設定
            args = GetValidArgs();
            args.LinkageSystemNo = "";

            results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.LinkageSystemNo)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();
        }

        [TestMethod]
        public async Task 順番待ちリストの入力が妥当でなければエラー()
        {
            // ListがNull
            var args = GetValidArgs();
            args.WaitingListN = null;

            var results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.WaitingListN)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();

            // 受付時間書式が不正
            args = GetValidArgs();
            args.WaitingListN[0].ReceptionTime = "10:30";

            results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("の受付時間の設定が間違っています").IsTrue();

            // 予約時間書式が不正
            args = GetValidArgs();
            args.WaitingListN[1].ReservationTime = "2460";

            results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("の予約時間の設定が間違っています").IsTrue();

            // WaitingDateが不正
            args = GetValidArgs();
            args.WaitingListN[1].WaitingDate = "AAAA";

            results = await _worker.ListWrite(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(QoApiWaitingListItem.WaitingDate)).IsTrue();
            results.Result.Detail.Contains("不正です").IsTrue();
        }

        [TestMethod]
        public async Task 施設言語キーの取得に失敗するとエラー()
        {
            var args = GetValidArgs();

            // 施設言語取得処理で例外発生
            _facilityRepo.Setup(m => m.ReadFacilityLanguage(_facilityKey1)).Throws(new Exception());

            var results = await _worker.ListWrite(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("施設言語取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public async Task 引数の待受情報リストのDBの書き込み失敗するとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var errorMessages = new List<string> { "hoge", "fuga" };

            // DB書き込み処理で失敗
            _waitingRepo.Setup(m => m.WriteList(It.IsAny<List<QH_WAITINGLIST_DAT>>())).Returns((false, null, errorMessages));

            var results = await _worker.ListWrite(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("順番待ち情報リストのDB書き込み処理に失敗しました").IsTrue();
            // 内部エラーメッセージが返された
            results.ErrorMessageN.IsStructuralEqual(errorMessages);
        }

        [TestMethod]
        public async Task 待ち人数情報の書き込み処理で想定外例外がおこるとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 待ち人数Table書き込み処理で例外
            _waitingRepo.Setup(m => m.DeleteWaitingOrderList(It.IsAny<int>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(), 1)).Throws(new Exception());

            var results = await _worker.ListWrite(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("順番待ち人数情報のDB書き込み処理に失敗しました").IsTrue();
            // エラーメッセージは入らない
            results.ErrorMessageN.Any().IsFalse();
        }

        [TestMethod]
        public async Task 正常に終了しPush通知も成功()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var appConfig = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _waitingRepo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "Dp01")).Returns(appConfig);
            _waitingRepo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "Dp02")).Returns(appConfig);

            var results = await _worker.ListWrite(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.ErrorMessageN.Any().IsFalse(); // 通知関連のエラーもなし        
            

            // 通知処理に入った
            _pushNotification.Verify(m => m.Initialize(It.IsAny<NotificationHubsSettings>()), Times.Once);

            // 通知が3回送信された （10,20,中待ち）
            _pushNotification.Verify(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>()), Times.Exactly(3));

            // 通知用ユーザーVIEWの取得が行われた(RootLinkageSystemNoが正しく利用されている)
            // 通常イベント通知と中待ち通知用で計2回
            _linkageRepo.Verify(m => m.ReadPushNotificationUserView(27003, 27004, It.IsAny<List<string>>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task 正常に終了したがPush通知でエラーがあった()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 通知送信の戻り値が不正
            _pushNotification.Setup(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>())).ReturnsAsync(new string[] { });

            var results = await _worker.ListWrite(args);

            // APIとしては成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.ErrorMessageN.Count.Is(1); // Push通知内でエラー
            results.ErrorMessageN[0].Contains("Push通知処理内でエラーが発生しました").IsTrue();

            // 通知処理に入った
            _pushNotification.Verify(m => m.Initialize(It.IsAny<NotificationHubsSettings>()), Times.Once);
            

            // 通知用ユーザーVIEWの取得が行われた(RootLinkageSystemNoが正しく利用されている)
            _linkageRepo.Verify(m => m.ReadPushNotificationUserView(27003, 27004, It.IsAny<List<string>>()), Times.Once);
        }

        [TestMethod]
        public async Task WaitingOrderListに正しく書き込まれる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 通知送信の戻り値が不正(テスト範囲を絞るためNotifyIfNeedsより先に進ませない)
            _pushNotification.Setup(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>())).ReturnsAsync(new string[] { });

            var list = new List<QoApiWaitingListItem>
            {
                CreateListItem("99999001",10,1,1,1,"Dp01","Dr01"),
                CreateListItem("99999002",20,1,2,2,"Dp02","Dr02","0000"),
                CreateListItem("99999003", 0,1,3,3,"Dp03","Dr03"),
                CreateListItem("99999004",26,1,4,4,"Dp04","Dr04"),
                CreateListItem("99999005",25,1,5,5,"Dp05","Dr05"),
                CreateListItem("99999006",30,1,6,6,"Dp06","Dr06"),
                CreateListItem("99999007",29,1,7,7,"Dp07","Dr07"),
                CreateListItem("99999008",35,1,9,9,"Dp08","Dr08"),
            };

            args.WaitingListN = list;

            _waitingRepo.Setup(m => m.UpsertWaitingOrderList(It.IsAny<QH_WAITINGORDERLIST_DAT>())).Callback((QH_WAITINGORDERLIST_DAT entity) =>
            {
                // 待ち人数情報
                entity.DEPARTMENTCODE.Is("Dp02");
                entity.DOCTORCODE.Is("Dr02");
                entity.RECEPTIONNO.Is("2");
                // 予約時間が"00:00"の場合は23:59:59として登録
                entity.RESERVATIONDATE.Is(_targetDate + new TimeSpan(23, 59, 59));
            });

            var results = await _worker.ListWrite(args);


            // Status 0,10,25,26,29,30 は待ち人数情報削除処理が走る
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp01", "Dr01", "99999001", 0), Times.Once);            
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp03", "Dr03", "99999003", 0), Times.Once);
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp04", "Dr04", "99999004", 0), Times.Once);
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp05", "Dr05", "99999005", 0), Times.Once);
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp06", "Dr06", "99999006", 0), Times.Once);
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp07", "Dr07", "99999007", 0), Times.Once);

            // 20, 35 は削除処理は走らない
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp02", "Dr02", "99999002", 0), Times.Never);
            _waitingRepo.Verify(m => m.DeleteWaitingOrderList(27004, "Dp08", "Dr08", "99999008", 0), Times.Never);

            // 20 で待ち人数情報登録処理が走る(35はとおらない)
            _waitingRepo.Verify(m => m.UpsertWaitingOrderList(It.IsAny<QH_WAITINGORDERLIST_DAT>()), Times.Once);

            // 成否およびNotifyIfNeedsより先はこのメソッドではテスト対象外とする
        }

        [TestMethod]
        public async Task RootLinkageSystemNoを指定しないときはHOSPAとみなされる()
        {
            var args = GetValidArgs();
            args.RootLinkageSystemNo = "";
            SetUpValidMethods(args);

            var appConfig = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _waitingRepo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "Dp02")).Returns(appConfig);

            var results = await _worker.ListWrite(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.ErrorMessageN.Any().IsFalse();
            

            // 通知処理に入った（成否は問わない）
            _pushNotification.Verify(m => m.Initialize(It.IsAny<NotificationHubsSettings>()), Times.Once);

            // 通知用ユーザーVIEWの取得が行われた(RootLinkageSystemNoがHOSPAとして扱われている)
            _linkageRepo.Verify(m => m.ReadPushNotificationUserView(27003, 27004, It.IsAny<List<string>>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task カルテ提出20で順番待ち変更通知と中待ち通知が対象に送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.SubmittedKarte, DataType.MedicalTreatment, 1, 1, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var appConfig1 = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _waitingRepo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "Dp01")).Returns(appConfig1);

            var results = await _worker.ListWrite(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);

            // Push通知は2回発生
            requests.Count.Is(2);

            var request1 = requests[0];
            // 順番待ち変更イベント通知                
            request1.Silent.IsTrue(); // サイレントで通知された
            request1.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request1.Text.Is(string.Empty);
            request1.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request1.Badge.Is(1);
            // 通知宛先が含まれている
            request1.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();
            // Waiting タグが含まれている
            request1.TagExpression.Contains(WaitingListWriteWorker.TAG_WAITING).IsTrue();

            var payload1 = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request1.Extra);

            payload1.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload1.EventType.Is(WaitingEventType.ExaminationQueueChanged);
            payload1.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload1.ReceiptNumber.Is("1");
            payload1.ReservationNo.Is("1");

            var request2 = requests[1];
            // 中待ち通知                
            request2.Silent.IsFalse(); // 通常通知
            request2.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request2.Text.Is("診察室の近くでお待ちください。");
            request2.Url.Is($"heart-plus://home?user={_accountKey2.ToEncrypedReference()}");
            // 通知宛先が含まれている 人数待ちの中から未送信のpushId2が対象
            request2.TagExpression.Contains("pushId2".ToEncrypedReference()).IsTrue();
            // Waiting タグが含まれている
            request2.TagExpression.Contains(WaitingListWriteWorker.TAG_WAITING).IsTrue();

            var payload2 = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request2.Extra);

            payload2.AccountKeyReference.Is(_accountKey2.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload2.EventType.Is(WaitingEventType.ExaminationSoon);
            payload2.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload2.ReceiptNumber.Is("2"); // 対応する受付番号
            payload2.ReservationNo.Is(""); // 予約番号は空でOK
        }

        [TestMethod]
        public async Task カルテ提出20以外でも削除対象であれば中待ち通知が対象に送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Absence, DataType.MedicalTreatment, 1, 1, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            // 待ち人数情報にデータがあり削除対象
            _waitingRepo.Setup(m => m.DeleteWaitingOrderList(27004, "Dp01", "Dr01", "99999001", 0)).Returns(true);

            var appConfig = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _waitingRepo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "Dp01")).Returns(appConfig);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var results = await _worker.ListWrite(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);

            // Push通知は2回発生
            requests.Count.Is(2);

            var request1 = requests[0];
            // イベント通知                
            request1.Silent.IsTrue(); // サイレントで通知された
            request1.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request1.Text.Is(string.Empty);
            request1.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request1.Badge.Is(1);
            // 通知宛先が含まれている
            request1.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();
            // Waiting タグが含まれている
            request1.TagExpression.Contains(WaitingListWriteWorker.TAG_WAITING).IsTrue();

            var payload1 = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request1.Extra);

            payload1.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload1.EventType.Is(WaitingEventType.ExaminationAbsence);
            payload1.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload1.ReceiptNumber.Is("1");
            payload1.ReservationNo.Is("1");

            var request2 = requests[1];
            // 中待ち通知                
            request2.Silent.IsFalse(); // 通常通知
            request2.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request2.Text.Is(_messages[KEY_EXAMINATION_SOON]);
            request2.Url.Is($"heart-plus://home?user={_accountKey2.ToEncrypedReference()}");
            // 通知宛先が含まれている 人数待ちの中から未送信のpushId2が対象
            request2.TagExpression.Contains("pushId2".ToEncrypedReference()).IsTrue();
            // Waiting タグが含まれている
            request2.TagExpression.Contains(WaitingListWriteWorker.TAG_WAITING).IsTrue();

            var payload2 = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request2.Extra);

            payload2.AccountKeyReference.Is(_accountKey2.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload2.EventType.Is(WaitingEventType.ExaminationSoon);
            payload2.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload2.ReceiptNumber.Is("2"); // 対応する受付番号
            payload2.ReservationNo.Is(""); // 予約番号は空でOK
        }

        [TestMethod]
        public async Task 待ち受け情報が更新前と更新後で変化がなければ処理はスキップされる()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Accepted, DataType.MedicalTreatment, 1, 1, "Dp01", "Dr01",null,"01"));
            SetUpValidMethods(args);

            var oldEntity = GetWaitingListEntity("99999001", StatusType.Accepted, DataType.MedicalTreatment,false, "01");

            // 更新前情報を返す
            _waitingRepo.Setup(m => m.WriteList(It.IsAny<List<QH_WAITINGLIST_DAT>>())).Returns((true, new List<QH_WAITINGLIST_DAT> { oldEntity }, new List<string>()));
           

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var results = await _worker.ListWrite(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);

            // Push通知は発生しない
            requests.Count.Is(0);            
        }

        [TestMethod]
        public async Task 待ち受け情報が更新前と更新後で変化がなくてもStatus120の場合は処理はスキップされない()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.WaitingForExamination, DataType.MedicalTreatment, 1, 1, "Dp01", "Dr01", null, "01"));
            SetUpValidMethods(args);

            var oldEntity = GetWaitingListEntity("99999001", StatusType.WaitingForExamination, DataType.MedicalTreatment, false, "01");

            // 更新前情報を返す
            _waitingRepo.Setup(m => m.WriteList(It.IsAny<List<QH_WAITINGLIST_DAT>>())).Returns((true, new List<QH_WAITINGLIST_DAT> { oldEntity }, new List<string>()));


            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var results = await _worker.ListWrite(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);

            // Push通知は行われる
            requests.Count.Is(1);
        }

        [TestMethod]
        public async Task 受付済み10で診察受付開始のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Accepted, DataType.MedicalTreatment, 1, 1, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレントで通知された
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();
            // Waiting タグが含まれている
            request.TagExpression.Contains(WaitingListWriteWorker.TAG_WAITING).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationQueueStart);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("1");
            payload.ReservationNo.Is("1");
        }        

        [TestMethod]
        public async Task 診察呼出25で診察呼び出しのPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.CalledToRoom, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // 通常通知
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[WaitingListWriteWorker.KEY_EXAMINATION_READY]);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationReady);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task 診察終了30で診察終了のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.EndOfExamination, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];


            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());            
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 診察不在26で診察不在のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Absence, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];


            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationAbsence);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 検査後診察あり29で診察検査中のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.UnderInspection, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];


            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationInTest);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task カルテ再提出35で診察再開のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.ResubmittedKarte, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationResume);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");

            // 35は待ち人数カウントには関与しない
            _waitingRepo.Verify(m => m.UpsertWaitingOrderList(It.IsAny<QH_WAITINGORDERLIST_DAT>()), Times.Never);
        }

        [TestMethod]
        public async Task 診察待ち120で診察待ちのPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.WaitingForExamination, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.ExaminationQueueChangedSpecial);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");

            // 120は待ち人数カウントには関与しない
            _waitingRepo.Verify(m => m.UpsertWaitingOrderList(It.IsAny<QH_WAITINGORDERLIST_DAT>()), Times.Never);
        }

        [TestMethod]
        public async Task 薬保留1で薬受付のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Pending, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicineQueueStart);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 会計保留1で会計受付のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Pending, DataType.Payment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.AccountingQueueStart);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 薬呼出2で薬準備OKのPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Called, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];            

            // 通常通知
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[KEY_MEDICINE_READY]);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicineReady);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 後払い薬お渡し準備中101で準備中のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.PostpayMedicinePending, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicinePostpayQueueStart);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 後払い薬お渡し準備完了102で準備完了のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.PostpayMedicineCalled, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // 通常通知
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[KEY_MEDICINE_POSTPAY_READY]);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicinePostpayReady);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 後払い院外処方103で院外処方のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.PostpayExternalPrescription, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // 通常通知
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[KEY_MEDICINE_POSTPAY_EXTERNAL]);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicinePostpayExternal);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 会計呼出2で会計準備OKのPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Called, DataType.Payment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // 通常通知
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[WaitingListWriteWorker.KEY_ACCOUNTING_READY]);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.AccountingReady);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());            
            payload.ReservationNo.Is("1");
            payload.ReceiptNumber.Is("2");
        }

        [TestMethod]
        public async Task 薬完了8で薬完了のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Completed, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicineEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task 会計完了8で会計完了のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Completed, DataType.Payment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.AccountingEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task 薬特殊7で特殊薬準備のPush通知が送信される()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.HasMedicine, DataType.Dispensing, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // 通常通知
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[KEY_MEDICINE_READY_SPECIAL]);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 対応したEventTypeが設定されている
            payload.EventType.Is(WaitingEventType.MedicineReadySpecial);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task アプリ未対応のステータスの場合は何も通知しない()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.SendOrder, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01"));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            // 通知は行われなかった
            requests.Count.Is(0);            

            // プッシュ通知送信処理は未実施
            _pushNotification.Verify(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>()), Times.Never);
        }

        [TestMethod]
        public async Task 診察で削除フラグがある場合は診察終了のPush通知を送信する()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Accepted, DataType.MedicalTreatment, 1, 2, "Dp01", "Dr01",null,null,true));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 診察終了がセットされている
            payload.EventType.Is(WaitingEventType.ExaminationEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task 薬で削除フラグがある場合は薬終了のPush通知を送信する()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Called, DataType.Dispensing, 1, 2, "Dp01", "Dr01",null,null,true));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 薬終了がセットされている
            payload.EventType.Is(WaitingEventType.MedicineEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task 会計で削除フラグがある場合は会計終了のPush通知を送信する()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Called, DataType.Payment, 1, 2, "Dp01", "Dr01",null,null, true));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 会計終了がセットされている
            payload.EventType.Is(WaitingEventType.AccountingEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task プッシュ通知で削除フラグがありDataTypeが未定義であればNoneで通知を送る()
        {
            var args = GetIndividualArgs(CreateListItem("99999001", StatusType.Called, (DataType)99, 1, 2, "Dp01", "Dr01",null,null,true));
            SetUpValidMethods(args);

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            requests.Count.Is(1); // 通知は1回
            var request = requests[0];

            // サイレント通知
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            // 未定義値につきNone扱いとする
            payload.EventType.Is(WaitingEventType.None);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");
        }

        [TestMethod]
        public async Task プッシュ通知がWaitingListEntityの数だけ正常に実施される()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);
            var list = new List<QoApiWaitingListItem>
            {
                CreateListItem("99999001",StatusType.SubmittedKarte, DataType.MedicalTreatment,1,2,"Dp01","Dt01",null,null,true),
                CreateListItem("99999002",StatusType.CalledToRoom, DataType.MedicalTreatment,1,2,"Dp02","Dt02"),
                CreateListItem("99999001",StatusType.Accepted, DataType.MedicalTreatment,1,2,"Dp01","Dt01"),
                CreateListItem("99999002",StatusType.EndOfExamination, DataType.MedicalTreatment,1,2,"Dp02","Dt02"),
            };
            args.WaitingListN = list;

            var requests = new List<NotificationRequest>();
            SetUpPushRequestAction(requests);

            var result = await _worker.ListWrite(args);

            result.IsSuccess.Is(bool.TrueString); // 成功

            // 通知は4回送信された
            requests.Count.Is(4);

            // 1回目の通知
            var request = requests[0];
            request.Silent.IsTrue();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            var payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            payload.EventType.Is(WaitingEventType.ExaminationEnd);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");

            // 2回目の通知
            request = requests[1];
            request.Silent.IsFalse();
            request.Title.Is(WaitingListWriteWorker.MSG_TITLE);
            request.Text.Is(_messages[WaitingListWriteWorker.KEY_EXAMINATION_READY]);
            request.Url.Is($"heart-plus://home?user={_accountKey2.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId2".ToEncrypedReference()).IsTrue();

            payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey2.ToEncrypedReference());
            payload.EventType.Is(WaitingEventType.ExaminationReady);
            payload.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            payload.ReceiptNumber.Is("2");
            payload.ReservationNo.Is("1");

            // 3回目の通知（一部省略）
            request = requests[2];
            request.Silent.IsTrue();
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey1.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId1".ToEncrypedReference()).IsTrue();

            payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey1.ToEncrypedReference());
            payload.EventType.Is(WaitingEventType.ExaminationQueueStart);

            // 4回目の通知（一部省略）
            request = requests[3];
            request.Silent.IsTrue();
            request.Text.Is(string.Empty);
            request.Url.Is($"heart-plus://home?user={_accountKey2.ToEncrypedReference()}");
            request.Badge.Is(1);
            // 通知宛先が含まれている
            request.TagExpression.Contains("pushId2".ToEncrypedReference()).IsTrue();

            payload = JsonConvert.DeserializeObject<WaitingNotificationPayload>(request.Extra);

            payload.AccountKeyReference.Is(_accountKey2.ToEncrypedReference());
            payload.EventType.Is(WaitingEventType.ExaminationEnd);

        }
    }
}
