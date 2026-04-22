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
    /// WaitingListWriteWorker WriteListメソッド テストクラス
    /// 下準備用パーシャルクラス
    /// </summary>
    public partial class WaitingListWriteWorkerFixture
    {
        public QoWaitingListWriteApiArgs GetIndividualArgs(QoApiWaitingListItem item)
        {
            return new QoWaitingListWriteApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                RootLinkageSystemNo = "27003",
                LinkageSystemNo = "27004",
                FacilityKey = "11DC3F56-5652-4D08-9147-1575C1723EDB",
                DataType = item.DataType,
                WaitingListN = new List<QoApiWaitingListItem> { item }
            };
        }

        

        public void SetUpPushRequestAction(IList<NotificationRequest> requests)
        {
            _pushNotification.Setup(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>()))
               .ReturnsAsync(new string[] { "1", "2" })
               .Callback<NotificationRequest>(request =>
               {
                   requests.Add(request);
               });
        }

        public QoWaitingListWriteApiArgs GetValidArgs()
        {
            var waitingList = new List<QoApiWaitingListItem>
            {
                new QoApiWaitingListItem
                {
                    Detail = new QoApiWaitingDetailItem
                    {
                        InOutType = "IN",
                        SameDaySequence = "1",
                        DoctorCode = "Dr01",
                        DoctorName = "医者太郎",
                        DepartmentName = "小児科",
                        MedicalActCode = "M01",
                        MedicalActName = "診察",
                        RoomCode = "R01",
                        RoomName = "診察室1",
                        DosingSlipNo = "001",
                        DosingSlipType = "Hoge",
                        DefferedPaymentFlg = "D0",
                        ChartWaitNumber = 9
                    },
                    WaitingDate = _targetDate.ToApiDateString(),
                    LinkageSystemId = "99999001",
                    DepartmentCode = "Dp01",
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
                        DoctorCode = "Dr02",
                        DoctorName = "医者次郎",
                        DepartmentName = "小児科",
                        MedicalActCode = "M02",
                        MedicalActName = "検査",
                        RoomCode = "R02",
                        RoomName = "診察室2",
                        DosingSlipNo = "002",
                        DosingSlipType = "Fuga",
                        DefferedPaymentFlg = "D1",
                        ChartWaitNumber = 10
                    },
                    WaitingDate = _targetDate.ToApiDateString(),
                    LinkageSystemId = "99999002",
                    DepartmentCode = "Dp02",
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

            return new QoWaitingListWriteApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                RootLinkageSystemNo = "27003",
                LinkageSystemNo = "27004",
                FacilityKey = "11DC3F56-5652-4D08-9147-1575C1723EDB",
                DataType = $"{(byte)DataType.MedicalTreatment}",
                WaitingListN = waitingList
            };
        }

        public void SetUpValidMethods(QoWaitingListWriteApiArgs args)
        {
            // 施設言語設定
            _facilityRepo.Setup(m => m.ReadFacilityLanguage(It.Is<Guid>(x => x == _facilityKey1))).Returns(GetFacilityLanguageMst());
            // WaitingList書き込み設定
            _waitingRepo.Setup(m => m.WriteList(It.IsAny<List<QH_WAITINGLIST_DAT>>())).Returns((true, new List<QH_WAITINGLIST_DAT>(), new List<string>()));

            // NotificationHub設定
            _pushNotification.Setup(m => m.Initialize(It.IsAny<NotificationHubsSettings>())).Callback<NotificationHubsSettings>((hubSettings) =>
            {
                hubSettings.HubConnectionString.Is(QoApiConfiguration.TisNotificationHubConnectionString);
                hubSettings.HubName.Is(QoApiConfiguration.TisNotificationHubName);
            });

            _pushNotification.Setup(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>())).ReturnsAsync(new string[] { "1", "2" });

            // Push通知ユーザー一覧設定
            _linkageRepo.Setup(m => m.ReadPushNotificationUserView(27003, 27004, It.IsAny<List<string>>())).Returns(GetPushUserList());

            // 診療科設定情報
            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT
            {
                FACILITYKEY = Guid.NewGuid()
            };
            _waitingRepo.Setup(m => m.GetMedicalDepartmentConfig(27004, It.IsAny<string>())).Returns(config);

            // 待ち人数リスト設定
            _waitingRepo.Setup(m => m.GetWaitingOrderListEntity(27004, It.IsAny<string>(), It.IsAny<string>(), config, It.IsAny<DateTime>(), QsDbWaitingPriorityTypeEnum.None, It.IsAny<int>())).Returns(GetWaitingOrderList());

            // 削除対象はなしとする
            _waitingRepo.Setup(m => m.DeleteWaitingOrderList(27004, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
        }

        public QH_WAITINGLIST_DAT GetWaitingListEntity(string userId, StatusType statusType, DataType dataType, bool isDelete = false, string foreignKey = null)
        {
            return new QH_WAITINGLIST_DAT
            {
                STATUSTYPE = (byte)statusType,
                DATATYPE = (byte)dataType,
                FACILITYKEY = _facilityKey1,
                RECEPTIONNO = "1",
                RESERVATIONNO = "2",
                LINKAGESYSTEMNO = 27004,
                LINKAGESYSTEMID = userId,
                DELETEFLAG = isDelete,
                FOREIGNKEY = foreignKey,
            };
        }

        QoApiWaitingListItem CreateListItem(string linkageSystemId, int statusType, int dataType, int reservationNo, int receptionNo , string departmentCode, string doctorCode, string reservationTime = null, string foreignKey = null)
        {
            return new QoApiWaitingListItem
            {
                Detail = CreateWaitingDetailItem(doctorCode,"内科"),
                WaitingDate = _targetDate.ToApiDateString(),
                LinkageSystemId = linkageSystemId,
                StatusType = $"{(byte)statusType}",
                DataType = $"{(byte)dataType}",
                ReservationNo = reservationNo.ToString(),
                ReceptionNo = receptionNo.ToString(),
                DepartmentCode = departmentCode,  
                ReservationTime = reservationTime,
                ForeignKey = foreignKey
            };
        }

        QoApiWaitingListItem CreateListItem(string linkageSystemId, StatusType statusType, DataType dataType, int reservationNo, int receptionNo, string departmentCode, string doctorCode, string reservationTime = null, string foreignKey = null, bool deleteFlag = false)
        {
            return new QoApiWaitingListItem
            {
                Detail = CreateWaitingDetailItem(doctorCode, "内科"),
                WaitingDate = _targetDate.ToApiDateString(),
                LinkageSystemId = linkageSystemId,
                StatusType = $"{(byte)statusType}",
                DataType = $"{(byte)dataType}",
                ReservationNo = reservationNo.ToString(),
                ReceptionNo = receptionNo.ToString(),
                DepartmentCode = departmentCode,
                ReservationTime = reservationTime,
                ForeignKey = foreignKey,
                DeleteFlag = deleteFlag ? bool.TrueString : bool.FalseString
            };
        }

        QoApiWaitingDetailItem CreateWaitingDetailItem(string doctorCode, string department)
        {
            return new QoApiWaitingDetailItem
            {
                DoctorCode = doctorCode,
                DepartmentName = department,
                DoctorName = "医者太郎"
            };
        }

        public List<QH_WAITINGORDERLIST_DAT> GetWaitingOrderList()
        {
            return new List<QH_WAITINGORDERLIST_DAT>
            {
                new QH_WAITINGORDERLIST_DAT
                {
                    LINKAGESYSTEMID = "99999001",
                    RECEPTIONNO = "1",
                    PUSHSENDFLAG = true
                },
                new QH_WAITINGORDERLIST_DAT
                {
                    LINKAGESYSTEMID = "99999002",
                    RECEPTIONNO = "2",
                    PUSHSENDFLAG = false
                }
            };
        }

        public List<QH_LINKAGE_PUSHNOTIFICATION_VIEW> GetPushUserList()
        {
            return new List<QH_LINKAGE_PUSHNOTIFICATION_VIEW>
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
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999003",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId3"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999004",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId4"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999005",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId5"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999006",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId6"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999007",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId7"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999008",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId8"
                },
                new QH_LINKAGE_PUSHNOTIFICATION_VIEW
                {
                    LINKAGESYSTEMID = "99999009",
                    ACCOUNTKEY = Guid.NewGuid(),
                    NOTIFICATIONUSERID = "pushId9"
                },

            };
        }

        public List<QH_FACILITYLANGUAGE_MST> GetFacilityLanguageMst()
        {
            return new List<QH_FACILITYLANGUAGE_MST>
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
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_MEDICINE_POSTPAY_EXTERNAL,
                    VALUE = @"[{""Language"": 0, ""Value"": ""処方箋をお受け取りください""},{""Language"": 1, ""Value"": """"}]"
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _facilityKey1,
                    DELETEFLAG = false,
                    LANGUAGEKEY = WaitingListWriteWorker.KEY_MEDICINE_POSTPAY_READY,
                    VALUE = @"[{""Language"": 0, ""Value"": ""お薬カウンターにお越しください""},{""Language"": 1, ""Value"": """"}]"
                }
            };
        }
    }
}
