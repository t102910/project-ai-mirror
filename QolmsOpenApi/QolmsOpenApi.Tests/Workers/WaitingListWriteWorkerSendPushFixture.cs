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

namespace QolmsOpenApi.Tests.Workers
{
    /// <summary>
    /// WaitingListWriteWorker SendPushメソッド テストクラス
    /// </summary>
    [TestClass]
    public class WaitingListWriteWorkerSendPushFixture
    {
        Mock<IWaitingRepository> _waitingRepo;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IQoPushNotification> _pushNotification;
        Mock<IFacilityRepository> _facilityRepo;
        WaitingListWriteWorker _worker;

        Guid _facilityKey1 = Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB");
        Guid _accountKey1 = Guid.NewGuid();
        Guid _accountKey2 = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _waitingRepo = new Mock<IWaitingRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _pushNotification = new Mock<IQoPushNotification>();
            _facilityRepo = new Mock<IFacilityRepository>();
            _worker = new WaitingListWriteWorker(_waitingRepo.Object, _linkageRepo.Object, _pushNotification.Object, _facilityRepo.Object);
        }

        [TestMethod]
        public async Task プッシュ通知のみを選択的に送信()
        {
            var args = GetValidPushArgs();
            SetUpPushMethods();

            var results = await _worker.SendPush(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.ErrorMessageN.Any().IsFalse(); // 通知関連エラーなし

            // 通知処理に入っているか
            _pushNotification.Verify(m => m.Initialize(It.IsAny<NotificationHubsSettings>()), Times.Once);

            // 通知が二回送信されているか
            _pushNotification.Verify(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>()), Times.Exactly(2));

            // 通知用ユーザーVIEWの取得が行われたか
            _linkageRepo.Verify(m => m.ReadPushNotificationUserView(27003, 27004, It.IsAny<List<string>>()), Times.Once);

        }

        public void SetUpPushMethods()
        {
            var pushUserView = new List<QH_LINKAGE_PUSHNOTIFICATION_VIEW>
            {
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999001",
                    ACCOUNTKEY = _accountKey1,
                    NOTIFICATIONUSERID = "pushId1"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999002",
                    ACCOUNTKEY = _accountKey2,
                    NOTIFICATIONUSERID = "pushId2"
                }
            };

            _pushNotification.Setup(m => m.Initialize(It.IsAny<NotificationHubsSettings>())).Callback<NotificationHubsSettings>((hubSettings) =>
            {
                hubSettings.HubConnectionString.Is(QoApiConfiguration.TisNotificationHubConnectionString);
                hubSettings.HubName.Is(QoApiConfiguration.TisNotificationHubName);
            });

            _pushNotification.Setup(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>())).ReturnsAsync(new string[] { "1", "2" });

            _linkageRepo.Setup(m => m.ReadPushNotificationUserView(27003, 27004, It.IsAny<List<string>>())).Returns(pushUserView);

            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT
            {
                FACILITYKEY = Guid.NewGuid()
            };
            _waitingRepo.Setup(m => m.GetMedicalDepartmentConfig(27004, It.IsAny<string>())).Returns(config);

            _waitingRepo.Setup(m => m.GetWaitingOrderListEntity(27004, It.IsAny<string>(), It.IsAny<string>(), config, It.IsAny<DateTime>(), QsDbWaitingPriorityTypeEnum.PriorityByReserve, 0)).Returns(new List<QH_WAITINGORDERLIST_DAT>());

            _facilityRepo.Setup(m => m.ReadFacilityLanguage(It.Is<Guid>(x => x == _facilityKey1))).Returns(new List<QH_FACILITYLANGUAGE_MST>
            {
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_ACCOUNTING_READY,
                    VALUE = @"[{""Language"": 0, ""Value"": ""自動精算機にお越しください""},{""Language"": 1, ""Value"": ""Please go to the automatic payment machine.""}]"
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_EXAMINATION_READY,
                    VALUE = @"[{""Language"": 0, ""Value"": ""診察が始まります。診察室にお入りください。""},{""Language"": 1, ""Value"": """"}]"
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_EXAMINATION_SOON,
                    VALUE = @"[{""Language"": 0, ""Value"": ""診察室の近くでお待ちください。""},{""Language"": 1, ""Value"": """"}]"
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_MEDICINE_READY,
                    VALUE = @"[{""Language"": 0, ""Value"": ""お薬の準備が整いました。受け取りにお越しください。""},{""Language"": 1, ""Value"": """"}]"
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_MEDICINE_READY_SPECIAL,
                    VALUE = @"[{""Language"": 0, ""Value"": ""お薬が用意されています。受け取りにお越しください。""},{""Language"": 1, ""Value"": """"}]"
                }
            });
        }

        public QoWaitingListPushSendApiArgs GetValidPushArgs()
        {
            var waitingList = new List<QoApiWaitingListItem>
            {
                new QoApiWaitingListItem
                {
                    Detail = new QoApiWaitingDetailItem
                    {
                        InOutType = "IN",
                        SameDaySequence = "1",
                        DoctorCode = "D01",
                        DoctorName = "医者太郎",
                        DepartmentName = "小児科",
                        MedicalActCode = "M01",
                        MedicalActName = "診察",
                        RoomCode = "R01",
                        RoomName = "診察室1",
                        DosingSlipNo = "001",
                        DosingSlipType = "Hoge"
                    },
                    WaitingDate = DateTime.Today.ToApiDateString(),
                    LinkageSystemId = "99999001",
                    DepartmentCode = "DC001",
                    StatusType = "10",
                    ReceptionTime = "1015",
                    ReceptionNo = "1",
                    ReservationTime = "1030",
                    ReservationNo = "2",
                    ForeignKey = "9999991",
                    DeleteFlag = bool.FalseString,
                    DepartmentName = "内科"
                },
                new QoApiWaitingListItem
                {
                    Detail = new QoApiWaitingDetailItem
                    {
                        InOutType = "OUT",
                        SameDaySequence = "2",
                        DoctorCode = "D02",
                        DoctorName = "医者次郎",
                        DepartmentName = "小児科",
                        MedicalActCode = "M02",
                        MedicalActName = "検査",
                        RoomCode = "R02",
                        RoomName = "診察室2",
                        DosingSlipNo = "002",
                        DosingSlipType = "Fuga"
                    },
                    WaitingDate = DateTime.Today.ToApiDateString(),
                    LinkageSystemId = "99999002",
                    DepartmentCode = "DC002",
                    StatusType = "20",
                    ReceptionTime = "1515",
                    ReceptionNo = "2",
                    ReservationTime = "1630",
                    ReservationNo = "3",
                    ForeignKey = "9999992",
                    DeleteFlag = bool.FalseString,
                    DepartmentName = "皮膚科"
                },
            };

            return new QoWaitingListPushSendApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                RootLinkageSystemNo = "27003",
                LinkageSystemNo = "27004",
                FacilityKey = _facilityKey1.ToApiGuidString(),
                DataType = $"{(byte)DataType.MedicalTreatment}",
                WaitingListN = waitingList
            };

        }
    }
}
