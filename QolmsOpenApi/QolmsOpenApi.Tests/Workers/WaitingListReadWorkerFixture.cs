using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.DataTypeEnum;
using StatusType = MGF.QOLMS.QolmsDbEntityV1.QH_WAITINGLIST_DAT.StatusTypeEnum;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class WaitingListReadWorkerFixture
    {
        Mock<IWaitingRepository> _repo;
        Mock<ILinkageRepository> _linkageRepo;
        WaitingListReadWorker _worker;
        Guid _accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
        Guid _facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
        DateTime _targetDate = new DateTime(2022, 11, 11);


        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IWaitingRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _worker = new WaitingListReadWorker(_repo.Object, _linkageRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正で失敗が返る()
        {
            var args = GetValidArgs();
            args.ActorKey = Guid.Empty.ToApiGuidString();

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("対象が特定できません").IsTrue();
        }

        [TestMethod]
        public void LinkageSystemIdがある場合でアカウントキーの取得に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = Guid.Empty.ToApiGuidString();
            args.LinkageSystemId = "99990001";

            _linkageRepo.Setup(m => m.GetAccountKey("99990001", args.FacilityKeyReference.ToDecrypedReference<Guid>())).Throws(new Exception());

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("0500");
            ret.Result.Detail.Contains("アカウント取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 施設キーが不正で失敗が返る()
        {
            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                FacilityKeyReference = ""
            };

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("施設が不正です").IsTrue();
        }

        [TestMethod]
        public void 順番待ち情報取得処理で例外発生で失敗が返る()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);          

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 例外を設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Throws(new Exception("hoge"));

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003");
            ret.Result.Detail.Contains("順番待ち情報取得処理エラー").IsTrue();
        }

        [TestMethod]
        public void 順番待ち情報が0件の場合はデータ無しとして成功を返す()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 0件
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>());

            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態ではない
            ret.IsWaiting.Is(bool.FalseString);
            ret.WaitingCount.Is("");
            // 待ち人数取得処理には到達しない
            _repo.Verify(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, It.IsAny<byte>()), Times.Never);
        }

        [TestMethod]
        public void 順番待ち情報が存在するが全て不正データの場合はデータ無しとして成功を返す()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // データは存在するがIsKeysValidでfalseとなるデータのみが返る設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                new QH_WAITINGLIST_DAT(),
                new QH_WAITINGLIST_DAT(),
            });

            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態ではない
            ret.IsWaiting.Is(bool.FalseString);
            ret.WaitingCount.Is("");
            // 待ち人数取得処理には到達しない
            _repo.Verify(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, It.IsAny<byte>()), Times.Never);
        }

        [TestMethod]
        public void 待ち人数取得処理の例外発生で失敗が返る()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 有効なデータを返す設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.MedicalTreatment, StatusType.SubmittedKarte, TimeSpan.Zero)
            });

            var appConfig = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "03")).Returns(appConfig);

            // 待ち人数取得処理で例外を起こす
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "03")).Throws(new Exception());

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("0500");
            ret.Result.Detail.Contains("待ち人数取得処理エラー").IsTrue();
        }

        [TestMethod]
        public void 正常に終了する_診察でカルテ提出の場合は待ち人数が返る()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 有効なデータを返す設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.MedicalTreatment, StatusType.SubmittedKarte, TimeSpan.Zero)
            });

            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "03")).Returns(config);

            var appConfig = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "03")).Returns(appConfig);
            _repo.Setup(m => m.GetMedicalDepartmentAppValue(appConfig)).Returns(new QhFacilityDepartmentAppConfigOfJson
            {
                AmbignousNumber = 30,
                WaitingPriority = QsDbWaitingPriorityTypeEnum.PriorityByArrival
            });
            _repo.Setup(m => m.GetWaitingOrderNumber(27004, "03", "01", config, new DateTime(2022, 10, 11), "00001300", QsDbWaitingPriorityTypeEnum.PriorityByArrival, 6)).Returns((4, true));

            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            // 待ち人数4
            ret.WaitingCount.Is("4");
            
            ret.WaitingListN.Count.Is(1);

            // データが正しく設定されている
            var dat = ret.WaitingListN[0];

            // 待ち人数が存在する場合は20:カルテ提出から21:中待ちに変更されている
            dat.StatusType.Is("21");
            dat.HasMedicineWithPayment.Is(bool.FalseString);
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("03");
            dat.DepartmentName.Is("内科");
            dat.DataType.Is("1");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.DataType.Is("1");
            dat.WaitingDate.Is(targetDate.ToApiDateString());
            dat.WaitingCount.Is("4");
            dat.MaxWaitingCount.Is("30");
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
            dat.WaitingCount.Is("4");
        }        

        [TestMethod]
        public void 正常に終了する_診察でカルテ提出の場合でも予約無しはStatusを変更しない()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 有効なデータを返す設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.MedicalTreatment, StatusType.SubmittedKarte, TimeSpan.Zero)
            });

            var config = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "03")).Returns(config);

            var appConfig = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "03")).Returns(appConfig);
            _repo.Setup(m => m.GetMedicalDepartmentAppValue(appConfig)).Returns(new QhFacilityDepartmentAppConfigOfJson
            {
                AmbignousNumber = 30,
                WaitingPriority = QsDbWaitingPriorityTypeEnum.None
            });
            _repo.Setup(m => m.GetWaitingOrderNumber(27004, "03", "01", config, new DateTime(2022, 10, 11), "00001300", QsDbWaitingPriorityTypeEnum.None, 6)).Returns((4, false));

            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            // 待ち人数4
            ret.WaitingCount.Is("4");

            ret.WaitingListN.Count.Is(1);

            // データが正しく設定されている
            var dat = ret.WaitingListN[0];

            // 待ち人数が存在する場合でも予約が無ければ21には変更しない
            dat.StatusType.Is("20");
            dat.HasMedicineWithPayment.Is(bool.FalseString);
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("03");
            dat.DepartmentName.Is("内科");
            dat.DataType.Is("1");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.DataType.Is("1");
            dat.WaitingDate.Is(targetDate.ToApiDateString());
            dat.WaitingCount.Is("4");
            dat.MaxWaitingCount.Is("30");
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
        }

        [TestMethod]
        public void 正常に終了する_会計時薬なし()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 有効なデータを返す設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.MedicalTreatment, StatusType.Accepted, TimeSpan.Zero)
            });            

            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);   
            // 診察でStatus20以外はカウントしない
            ret.WaitingCount.Is("0");
            ret.WaitingListN.Count.Is(1);

            // データが正しく設定されている
            var dat = ret.WaitingListN[0];

            dat.HasMedicineWithPayment.Is(bool.FalseString);
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("03");
            dat.DepartmentName.Is("内科");
            dat.DataType.Is("1");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.StatusType.Is("10");
            dat.DataType.Is("1");
            dat.WaitingDate.Is(targetDate.ToApiDateString());
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
        }

        [TestMethod]
        public void 正常に終了する_LinkageSystemIdを指定した場合()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                LinkageSystemId = "99990001",
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // LinkageSystemIdからアカウントを取得する
            _linkageRepo.Setup(m => m.GetAccountKey("99990001", facilityKey)).Returns(accountKey);

            // 有効なデータを返す設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.MedicalTreatment, StatusType.Accepted, TimeSpan.Zero)
            });           


            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            // Status20の時以外はカウントは0となる
            ret.WaitingCount.Is("0");
            ret.WaitingListN.Count.Is(1);

            // アカウント取得処理が実行された
            _linkageRepo.Verify(m => m.GetAccountKey("99990001", facilityKey), Times.Once);
        }

        [TestMethod]
        public void 正常に終了する_会計時薬あり()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 有効なデータを返す設定（特殊薬待ちあり）
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.Payment, StatusType.Called, TimeSpan.Zero),
                GetEntity(DataType.Dispensing, StatusType.HasMedicine, TimeSpan.Zero)
            });           


            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            // 最新1件のみが入っている
            ret.WaitingListN.Count.Is(1);
            // 診察 / Status20以外はカウントしない
            ret.WaitingCount.Is("0");

            var dat = ret.WaitingListN[0];

            // データが正しく設定されている
            dat.HasMedicineWithPayment.Is(bool.TrueString); // 会計時薬あり
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("03");
            dat.DepartmentName.Is("内科");
            dat.DataType.Is("3");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.StatusType.Is("2");
            dat.DataType.Is("3");
            dat.LinkageSystemId.Is("00001300");
            dat.ForeignKey.Is("99999");
            dat.DeleteFlag.Is(bool.FalseString);
            dat.WaitingDate.Is(targetDate.ToApiDateString());            
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
        }

        [TestMethod]
        public void 正常に終了する_全件取得指定()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0",
                IsAll = bool.TrueString
            };

            // 有効なデータを返す設定（特殊薬待ちあり）
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>
            {
                GetEntity(DataType.Payment, StatusType.Called, TimeSpan.Zero),
                GetEntity(DataType.Dispensing, StatusType.HasMedicine, TimeSpan.Zero,1),
                GetEntity(DataType.MedicalTreatment, StatusType.SubmittedKarte, TimeSpan.FromHours(2)),
                GetEntity(DataType.MedicalTreatment, StatusType.SubmittedKarte,TimeSpan.FromHours(1),0,null, "04"),
                GetEntity(DataType.MedicalTreatment,StatusType.Accepted, TimeSpan.Zero),
                 GetEntity(DataType.MedicalTreatment,StatusType.EndOfExamination, TimeSpan.Zero),
            });

            var config03 = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "03")).Returns(config03);

            var config04 = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "04")).Returns(config04);

            var appConfig1 = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "03")).Returns(appConfig1);
            _repo.Setup(m => m.GetMedicalDepartmentAppValue(appConfig1)).Returns(new QhFacilityDepartmentAppConfigOfJson
            {
                AmbignousNumber = 30,
                WaitingPriority = QsDbWaitingPriorityTypeEnum.PriorityByReserve
            });
            var appConfig2 = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentAppConfig(27004, "04")).Returns(appConfig2);
            _repo.Setup(m => m.GetMedicalDepartmentAppValue(appConfig2)).Returns(new QhFacilityDepartmentAppConfigOfJson
            {
                AmbignousNumber = 60,
                WaitingPriority = QsDbWaitingPriorityTypeEnum.PriorityByArrival
            });

            _repo.Setup(m => m.GetWaitingOrderNumber(27004, "03", "01", config03, new DateTime(2022, 10, 11), "00001300", QsDbWaitingPriorityTypeEnum.PriorityByReserve, 6)).Returns((4, true));
            _repo.Setup(m => m.GetWaitingOrderNumber(27004, "04", "01", config04, new DateTime(2022, 10, 11), "00001300", QsDbWaitingPriorityTypeEnum.PriorityByArrival, 6)).Returns((6, true));

            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            // 全件が入っている
            ret.WaitingListN.Count.Is(6); 
            ret.WaitingCount.Is(""); // Allのときはカウントは無し

            // 並び順は優先度でソート済み
            // 1件目 カルテ提出なので順番待ちあり
            var dat = ret.WaitingListN[0];
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("03");
            dat.DepartmentName.Is("内科");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.AddHours(2).ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.StatusType.Is("21"); // 中待ちは21に変更される
            dat.DataType.Is("1");
            dat.LinkageSystemId.Is("00001300");
            dat.ForeignKey.Is("99999");
            dat.DeleteFlag.Is(bool.FalseString);
            dat.WaitingDate.Is(targetDate.ToApiDateString());            
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
            dat.WaitingCount.Is("4"); // 4人待ち
            dat.MaxWaitingCount.Is("30"); // 最大30人

            // 2件目 カルテ提出なので順番待ちあり 1件目とは別診療科
            dat = ret.WaitingListN[1];
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("04");
            dat.DepartmentName.Is("内科");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.AddHours(1).ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.StatusType.Is("21"); // 中待ちは21に変更される
            dat.DataType.Is("1");
            dat.LinkageSystemId.Is("00001300");
            dat.ForeignKey.Is("99999");
            dat.DeleteFlag.Is(bool.FalseString);
            dat.WaitingDate.Is(targetDate.ToApiDateString());
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
            dat.WaitingCount.Is("6"); // 6人待ち
            dat.MaxWaitingCount.Is("60"); // 最大60人

            // 3件目 カルテ提出ではないので順番待ちはなし
            dat = ret.WaitingListN[2];
            dat.DepartmentCode.Is("03");
            dat.StatusType.Is("10");
            dat.DataType.Is("1");
            dat.WaitingCount.Is("0");

            // 4件目 会計データが正しく設定されている
            dat = ret.WaitingListN[3];
            dat.HasMedicineWithPayment.Is(bool.TrueString); // 会計時薬あり
            dat.Sequence.Is("0");
            dat.DepartmentCode.Is("03");
            dat.DepartmentName.Is("内科");
            dat.DataType.Is("3");
            dat.ReceptionNo.Is("11");
            dat.ReceptionTime.Is(targetDate.ToApiDateString());
            dat.ReservationNo.Is("22");
            dat.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            dat.StatusType.Is("2");
            dat.DataType.Is("3");
            dat.LinkageSystemId.Is("00001300");
            dat.ForeignKey.Is("99999");
            dat.DeleteFlag.Is(bool.FalseString);
            dat.WaitingDate.Is(targetDate.ToApiDateString());
            dat.Detail.DepartmentName.Is("内科");
            dat.Detail.DoctorCode.Is("01");
            dat.Detail.DoctorName.Is("医者");
            dat.Detail.DosingSlipNo.Is("1");
            dat.Detail.DosingSlipType.Is("2");
            dat.Detail.InOutType.Is("3");
            dat.Detail.MedicalActCode.Is("4");
            dat.Detail.MedicalActName.Is("診察");
            dat.Detail.RoomCode.Is("5");
            dat.Detail.RoomName.Is("診察室");
            dat.Detail.SameDaySequence.Is("6");
            dat.Detail.DefferedPaymentFlg.Is("7");
            dat.Detail.ChartWaitNumber.Is((sbyte)8);
            dat.WaitingCount.Is("0");

            // 5件目 
            dat = ret.WaitingListN[4];
            dat.DepartmentCode.Is("03");
            dat.StatusType.Is("30");
            dat.DataType.Is("1");
            dat.WaitingCount.Is("0");


            // 5件目 特殊薬レコード
            dat = ret.WaitingListN[5];
            dat.Sequence.Is("1");
            dat.DataType.Is("2"); // 薬
            dat.StatusType.Is("7"); // 特殊薬あり
            dat.WaitingCount.Is("");
        }
                

        [TestMethod]
        public void 全権取得_診察消し込みテスト()
        {
            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                FacilityKeyReference = _facilityKey.ToEncrypedReference(),
                TargetDate = _targetDate.ToApiDateString(),
                DataType = "0",
                IsAll = bool.TrueString
            };

            var config03 = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "03")).Returns(config03);

            var config04 = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT();
            _repo.Setup(m => m.GetMedicalDepartmentConfig(27004, "04")).Returns(config04);

            _repo.Setup(m => m.GetWaitingOrderNumber(27004, "03", "01", config03, new DateTime(2022, 10, 11), "00001300", QsDbWaitingPriorityTypeEnum.PriorityByReserve, 0)).Returns((4, true));
            _repo.Setup(m => m.GetWaitingOrderNumber(27004, "04", "01", config04, new DateTime(2022, 10, 11), "00001300", QsDbWaitingPriorityTypeEnum.PriorityByReserve, 0)).Returns((6, true));

            var waitingList = new List<QH_WAITINGLIST_DAT>
            {
                // 診察 受付 10:00
                GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(10,0,0)),
            };

            var random = new Random();
            List<QH_WAITINGLIST_DAT> shuffle(List<QH_WAITINGLIST_DAT> source)
            {
                // sourceをdeepコピー
                var list = JsonConvert.DeserializeObject<List<QH_WAITINGLIST_DAT>>(JsonConvert.SerializeObject(source));
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]); // C# 7.0 以降のタプルスワップ
                }
                return list;
            }

            // 有効なデータを返す設定
            _repo.Setup(m => m.ReadLatestWaitingList(_accountKey, _facilityKey, _targetDate, 0)).Returns(()=>shuffle(waitingList));
             
            //----------------------
            // 1回目
            //----------------------
            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            // 全件が入っている
            ret.WaitingListN.Count.Is(1);

            // 1件目 診察受付中
            var dat = ret.WaitingListN[0];
            dat.DataType.Is("1"); // 診察
            dat.StatusType.Is("10"); // 受付          


            //----------------------
            // 2回目 会計 準備
            //----------------------

            // 会計 準備 10:30
            waitingList.Add(GetEntity(DataType.Payment, StatusType.Pending, new TimeSpan(10, 30, 0)));

            ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            ret.WaitingListN.Count.Is(2);

            // 1件目 会計 準備
            dat = ret.WaitingListN[0];
            dat.DataType.Is("3"); // 会計
            dat.StatusType.Is("1"); // 準備

            // 2件目 診察 会計が発生したことで自動で終了扱いとなっている
            dat = ret.WaitingListN[1];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");

            //----------------------
            // 3回目 薬 準備
            //----------------------

            // 薬 準備 11:00
            waitingList.Add(GetEntity(DataType.Dispensing, StatusType.Pending, new TimeSpan(11, 0, 0)));

            ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            ret.WaitingListN.Count.Is(3);

            // 1件目 会計 準備 会計優先
            dat = ret.WaitingListN[0];
            dat.DataType.Is("3"); // 会計
            dat.StatusType.Is("1"); // 準備

            // 2件目 薬 準備
            dat = ret.WaitingListN[1];
            dat.DataType.Is("2"); // 薬
            dat.StatusType.Is("1"); // 準備

            // 3件目 診察 終了扱い
            dat = ret.WaitingListN[2];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");

            //----------------------
            // 3回目 会計 完了 診察追加
            //----------------------

            // 会計完了
            waitingList[1].STATUSTYPE = (byte)StatusType.Completed;
            // 診察 2回目 呼び出し 11:15
            waitingList.Add(GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(11, 15, 0)));

            ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            ret.WaitingListN.Count.Is(4);

            // 1件目 2回目の診察優先
            dat = ret.WaitingListN[0];
            dat.DataType.Is("1"); 
            dat.StatusType.Is("25"); 

            // 2件目 薬は終わってないので会計より優先
            dat = ret.WaitingListN[1];
            dat.DataType.Is("2"); // 薬
            dat.StatusType.Is("1"); // 準備

            // 3件目 会計 完了しているので優先度低い
            dat = ret.WaitingListN[2];
            dat.DataType.Is("3"); 
            dat.StatusType.Is("8");

            // 4件目 診察 終了扱い
            dat = ret.WaitingListN[3];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");

            //----------------------
            // 4回目 会計追加
            //----------------------

            // 会計 2回目 準備 11:30
            waitingList.Add(GetEntity(DataType.Payment, StatusType.Pending, new TimeSpan(11, 30, 0)));

            ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            ret.WaitingListN.Count.Is(5);

            // 1件目 2回目の会計優先
            dat = ret.WaitingListN[0];
            dat.DataType.Is("3");
            dat.StatusType.Is("1");

            // 2件目 2回目の診察 終了していないが終了扱いとなっている
            dat = ret.WaitingListN[1];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");
            dat.ReceptionTime.Is((_targetDate + new TimeSpan(11,15,00)).ToApiDateString());

            // 3件目 薬はそのまま
            dat = ret.WaitingListN[2];
            dat.DataType.Is("2"); // 薬
            dat.StatusType.Is("1"); // 準備

            // 4件目 1回目の完了済み会計
            dat = ret.WaitingListN[3];
            dat.DataType.Is("3");
            dat.StatusType.Is("8");
            

            // 5件目 1回目診察 終了扱い
            dat = ret.WaitingListN[4];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");
            dat.ReceptionTime.Is((_targetDate + new TimeSpan(10, 0, 00)).ToApiDateString());

            //----------------------
            // 5回目 3回目の診察追加
            //----------------------

            // 診察 3回目 検査中 12:00
            waitingList.Add(GetEntity(DataType.MedicalTreatment, StatusType.UnderInspection, new TimeSpan(12, 0, 0)));

            ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 待ち状態である
            ret.IsWaiting.Is(bool.TrueString);
            ret.WaitingListN.Count.Is(6);

            // 1件目 3回目の診察優先 会計よりも後に受付でかつ終了していなければ会計より優先
            dat = ret.WaitingListN[0];
            dat.DataType.Is("1");
            dat.StatusType.Is("29");

            // 2件目 2回目の会計 準備のまま
            dat = ret.WaitingListN[1];
            dat.DataType.Is("3"); 
            dat.StatusType.Is("1");

            // 3件目 2回目の診察 終了していないが終了扱いとなっている
            dat = ret.WaitingListN[2];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");
            dat.ReceptionTime.Is((_targetDate + new TimeSpan(11, 15, 00)).ToApiDateString());

            // 4件目 薬はそのまま
            dat = ret.WaitingListN[3];
            dat.DataType.Is("2");
            dat.StatusType.Is("1");           


            // 5件目 1回目の会計
            dat = ret.WaitingListN[4];
            dat.DataType.Is("3");
            dat.StatusType.Is("8");            

            // 6件目 1回目診察 終了扱い
            dat = ret.WaitingListN[5];
            dat.DataType.Is("1");
            dat.StatusType.Is("30");
            dat.ReceptionTime.Is((_targetDate + new TimeSpan(10, 0, 00)).ToApiDateString());

        }

        [TestMethod]
        public void 診察待ちレコードのみ存在する場合はそのレコードを最新とする()
        {
            
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { medical };

            var entity = _worker.GetLatestEntity(list);

            // 診察が最新の状態として返された
            entity.Is(medical);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計待ちレコードのみ存在する場合はそのレコードを最新とする()
        {

            var payment = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 30, 0));

            var list = new List<QH_WAITINGLIST_DAT> { payment };

            var entity = _worker.GetLatestEntity(list);

            // 会計が最新の状態として返された
            entity.Is(payment);
            entity.DATATYPE.Is<byte>(3);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 薬待ちレコードのみ存在する場合はそのレコードを最新とする()
        {

            var dispensing = GetEntity(DataType.Dispensing, StatusType.Called, new TimeSpan(16, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { dispensing };

            var entity = _worker.GetLatestEntity(list);

            // 薬が最新の状態として返された
            entity.Is(dispensing);
            entity.DATATYPE.Is<byte>(2);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計_診察が存在した場合は受付時間が遅い方が優先される()
        {
            var payment = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 30, 0));            
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { payment, medical };

            var entity = _worker.GetLatestEntity(list);

            // 受付時間が会計が後なので会計が最新の状態として返される
            entity.Is(payment);
            entity.DATATYPE.Is<byte>(3);
            entity.SEQUENCE.Is(0);

            // 会計の受付時間を診察より前に指定
            SetReceptionTime(payment, 14, 59);

            entity = _worker.GetLatestEntity(list);
            // 診察が最新の状態として返される
            entity.Is(medical);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 薬_診察が存在した場合は受付時間が遅い方が優先される()
        {
            var dispensing = GetEntity(DataType.Dispensing, StatusType.Called, new TimeSpan(16, 45, 0));
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { dispensing, medical };

            var entity = _worker.GetLatestEntity(list);

            // 受付時間が薬が後なので薬が最新の状態として返される
            entity.Is(dispensing);
            entity.DATATYPE.Is<byte>(2);
            entity.SEQUENCE.Is(0);

            // 薬の受付時間を診察より前に指定
            SetReceptionTime(dispensing, 14, 59);

            entity = _worker.GetLatestEntity(list);
            // 診察が最新の状態として返される
            entity.Is(medical);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計_診察_薬が存在した場合1()
        {
            // 診察 呼び出し 15:00
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));
            // 薬 呼び出し 16:45
            var dispensing = GetEntity(DataType.Dispensing, StatusType.Called, new TimeSpan(16, 45, 0));
            // 会計 呼び出し 16:30
            var payment = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 30, 0));
                       
            var list = new List<QH_WAITINGLIST_DAT> { payment, medical, dispensing };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 診察より会計・薬が後でかつ両方待ち受け状態なら会計優先なので会計が返される
            entity.Is(payment);
            entity.DATATYPE.Is<byte>(3);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計_診察_薬が存在した場合2()
        {
            // 診察 呼び出し 17:00
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(17, 0, 0));
            // 薬 呼び出し 16:45
            var dispensing = GetEntity(DataType.Dispensing, StatusType.Called, new TimeSpan(16, 45, 0));
            // 会計 呼び出し 16:30
            var payment = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 30, 0));

            var list = new List<QH_WAITINGLIST_DAT> { payment, medical, dispensing };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 薬・会計の受付時間＜診察の受付時間なので診察が最新の状態になる
            entity.Is(medical);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計_診察_薬が存在した場合3()
        {
            // 診察 呼び出し 15:00
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));
            // 薬 呼び出し 16:45
            var dispensing = GetEntity(DataType.Dispensing, StatusType.Called, new TimeSpan(16, 45, 0));
            // 会計 完了 16:30
            var payment = GetEntity(DataType.Payment, StatusType.Completed, new TimeSpan(16, 30, 0));

            var list = new List<QH_WAITINGLIST_DAT> { payment, medical, dispensing };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 診察＜会計・薬で会計が完了しているので薬が返される
            entity.Is(dispensing);
            entity.DATATYPE.Is<byte>(2);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計_診察_薬が存在した場合4()
        {
            // 診察 呼び出し 15:00
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));
            // 薬 完了 16:45
            var dispensing = GetEntity(DataType.Dispensing, StatusType.Completed, new TimeSpan(16, 45, 0));
            // 会計 完了 16:30
            var payment = GetEntity(DataType.Payment, StatusType.Completed, new TimeSpan(16, 30, 0));

            var list = new List<QH_WAITINGLIST_DAT> { payment, medical, dispensing };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 診察＜会計・薬で、会計・薬が両方完了している場合は薬が優先される
            entity.Is(dispensing);
            entity.DATATYPE.Is<byte>(2);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 会計_診察_薬が存在した場合5()
        {
            // 診察 呼び出し 15:00
            var medical = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0));
            // 後払い 薬準備OK 16:45
            var dispensing = GetEntity(DataType.Dispensing, StatusType.PostpayMedicineCalled, new TimeSpan(16, 45, 0));
            // 会計 呼び出し 16:30
            var payment = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 30, 0));

            var list = new List<QH_WAITINGLIST_DAT> { payment, medical, dispensing };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 診察＜会計・薬で、薬が後払い対応であれば薬が優先
            entity.Is(dispensing);
            entity.DATATYPE.Is<byte>(2);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 診察_薬_会計の運用シミュレート()
        {
            // 診療科1 受付 15:00
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 0);

            var list = new List<QH_WAITINGLIST_DAT> { medical1 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 1つしかないのでmedical1が採用
            entity.Is(medical1);

            // 診療科1 診察終了
            medical1.STATUSTYPE = (byte)StatusType.EndOfExamination;

            // 薬 呼び出し 15:30
            var dispensing1 = GetEntity(DataType.Dispensing, StatusType.Called, new TimeSpan(15, 30, 0), 1);
            list.Add(dispensing1);

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 待ち状態の薬優先
            entity.Is(dispensing1);

            // 会計 呼び出し 16:00
            var payment1 = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 00, 0), 3);
            list.Add(payment1);

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 薬・会計が両方未完了な場合は会計優先
            entity.Is(payment1);

            // 会計が先に完了
            payment1.STATUSTYPE = (byte)StatusType.Completed;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 薬が最新状態となる
            entity.Is(dispensing1);

            // 薬 完了
            dispensing1.STATUSTYPE = (byte)StatusType.Completed;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 薬が最新のまま
            entity.Is(dispensing1);

        }

        [TestMethod]
        public void 複数診療科_診察が複数1()
        {
            // 同じ状態で同時に存在した場合は受付時間の新しい方が優先される

            // 診察 受付 15:00
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0),0);
            // 診察 受付 15:01
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 1, 0),1);
            // 診察 受付 14:59
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(14, 59, 0), 2);


            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2,medical3 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 最後に受付されたmedical2が採用される
            entity.Is(medical2);
        }

        [TestMethod]
        public void 複数診療科_診察が複数2()
        {
            // 同じ状態で同時に存在した場合で受付時間が同じの場合は更新日の新しい方が優先される

            // 診察 受付 15:00 更新 15:00:01
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 0,new DateTime(2022,11,11,15,0,1));
            // 診察 受付 15:00 更新 15:00:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 1, new DateTime(2022,11,11,15,0,0));
            // 診察 受付 15:00 更新 15:00:00
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 2, new DateTime(2022, 11, 11, 15, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2,medical3 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 更新日が新しいmedical1が採用される
            entity.Is(medical1);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 複数診療科_診察が複数3()
        {
            // 同じ状態で同時に存在した場合で受付時間が同じの場合でかつ更新日も同じ場合は
            // 優先度がつけられないため並び順のまま処理される

            // 診察 受付 15:00 更新 15:00:00
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 0, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察 受付 15:00 更新 15:00:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 1, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察 受付 15:00 更新 15:00:00
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 2, new DateTime(2022, 11, 11, 15, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2, medical3 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 優先順位がつけられないので先頭のmedical1が採用される
            entity.Is(medical1);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(0);
        }

        [TestMethod]
        public void 複数診療科_診察が複数4()
        {
            // どちらかが終了している場合は、終了していない方が優先される

            // 診察 終了 15:00 更新 15:00:01
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.EndOfExamination, new TimeSpan(15, 0, 0), 0, new DateTime(2022, 11, 11, 15, 0, 1));
            // 診察 受付 15:00 更新 15:00:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 1, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察 終了 15:00 更新 15:00:00
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.EndOfExamination, new TimeSpan(15, 0, 0), 1, new DateTime(2022, 11, 11, 15, 0, 0));

            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2, medical3 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 更新日が新しいmedical1が採用される
            entity.Is(medical2);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(1);
        }

        [TestMethod]
        public void 複数診療科_診察が複数5()
        {
            // 終了していない異なる状態で複数存在する場合はStatusTypeの大きい方が優先
            // 受付 < もうすぐ呼び出し < 呼び出し の順番

            // 診察 もうすぐ 15:00 更新 15:00:01
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.SubmittedKarte, new TimeSpan(15, 0, 0), 0, new DateTime(2022, 11, 11, 15, 0, 1));
            // 診察 受付 14:00 更新 15:00:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(14, 0, 0), 1, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察 呼び出し 15:00 更新 15:00:00
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.CalledToRoom, new TimeSpan(15, 0, 0), 2, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察 受付 14:00 更新 15:00:00
            var medical4 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(14, 0, 0), 3, new DateTime(2022, 11, 11, 15, 0, 0));
            
            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2, medical3, medical4 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // Statusの一番大きいmedical3が採用される
            entity.Is(medical3);
            entity.DATATYPE.Is<byte>(1);
            entity.SEQUENCE.Is(2);
        }

        [TestMethod]
        public void 複数診療科_診察が複数6()
        {
            // 全て受付済みの場合で予約時間が異なる場合は予約時間の早い方が優先される

            // 診察1 受付 15:00 更新 15:00:00
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 0, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察2 受付 15:00 更新 15:00:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 1, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察3 受付 15:00 更新 15:00:00
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 2, new DateTime(2022, 11, 11, 15, 0, 0));

            // 診察1 予約 15:30
            medical1.RESERVATIONDATE = new DateTime(2022, 11, 11, 15,30,0);
            // 診察２ 予約 15:15
            medical2.RESERVATIONDATE = new DateTime(2022, 11, 11, 15, 15, 0);
            // 診察3 予約15:45 
            medical3.RESERVATIONDATE = new DateTime(2022, 11, 11, 15, 45, 0);


            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2, medical3 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 予約時間が一番早いmedical2が優先
            entity.Is(medical2);
        }

        [TestMethod]
        public void 複数診療科_診察が複数7()
        {
            // 全て受付済みの場合で予約時間が異なる場合は予約時間の早い方が優先される
            // ただし、飛び込み(予約無し）が優先される

            // 診察1 受付 15:00 更新 15:00:00
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 0, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察2 受付 15:00 更新 15:00:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 1, new DateTime(2022, 11, 11, 15, 0, 0));
            // 診察3 受付 15:00 更新 15:00:00
            var medical3 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 2, new DateTime(2022, 11, 11, 15, 0, 0));

            // 診察1 予約 15:30
            medical1.RESERVATIONDATE = new DateTime(2022, 11, 11, 15, 30, 0);
            // 診察２ 予約 15:15
            medical2.RESERVATIONDATE = new DateTime(2022, 11, 11, 15, 15, 0);
            // 診察3 は予約無しの飛び込み

            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2, medical3 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 予約時間が未設定のmedical3が優先される
            entity.Is(medical3);
        }

        [TestMethod]
        public void 複数診療科_複雑なケース1()
        {
            // 以下のケース
            // 診療科1 受付 → 診療科2 受付 → 診療科2 もうすぐ → 診療科1 呼び出し →
            // 診療科1 終了 → 会計 呼び出し → 会計 終了

            // 診療科1 受付 15:01
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 1, 0), 0);
            // 診療科2 受付 15:00
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 1);

            var list = new List<QH_WAITINGLIST_DAT> { medical1, medical2};

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 受付時間の新しいmedical1が採用
            entity.Is(medical1);

            // 診療科2 もうすぐ呼び出しに変更
            medical2.STATUSTYPE = (byte)StatusType.SubmittedKarte;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // Statusの優先度の高いmedical2が採用
            entity.Is(medical2);

            // 診療科1 呼び出しに変更
            medical1.STATUSTYPE = (byte)StatusType.CalledToRoom;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // Statusの優先度の高いmedical1が採用
            entity.Is(medical1);

            // 診療科1 診察終了
            medical1.STATUSTYPE = (byte)StatusType.EndOfExamination;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 終了していない方のmedical2が採用される
            entity.Is(medical2);

            // 会計 呼び出し
            var payment1 = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(15, 30, 0), 2);
            list.Add(payment1);

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 受付時間が新しい会計が優先される
            entity.Is(payment1);

            // 会計 終了
            payment1.STATUSTYPE = (byte)StatusType.Completed;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 診療科2は待ち状態ではあるが、会計が診療科2の受付時間より後に発生しているため
            // 診察が終了しているとみなされ、会計が優先される。
            entity.Is(payment1);
        }

        [TestMethod]
        public void 複数診療科_複雑なケース2()
        {
            // 診療科1 受付
            var medical1 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 0, 0), 0);

            var list = new List<QH_WAITINGLIST_DAT> { medical1 };

            // 実行
            var entity = _worker.GetLatestEntity(list);

            // 1つしかないのでmedical1が採用
            entity.Is(medical1);

            // 診療科1 診察終了
            medical1.STATUSTYPE = (byte)StatusType.EndOfExamination;

            // 会計 呼び出し
            var payment1 = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(15, 30, 0), 1);
            list.Add(payment1);

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 受付時間が新しい会計が優先される
            entity.Is(payment1);

            // 会計 終了
            payment1.STATUSTYPE = (byte)StatusType.Completed;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 会計優先
            entity.Is(payment1);

            // 同日再診 受付
            var medical2 = GetEntity(DataType.MedicalTreatment, StatusType.Accepted, new TimeSpan(15, 45, 0), 2);
            list.Add(medical2);

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 会計より後の受付の診察なので再診が優先
            entity.Is(medical2);

            // 再診 終了
            medical2.STATUSTYPE = (byte)StatusType.EndOfExamination;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 診察もどちらも終わっている場合は受付時間の新しい方のmedical2が優先される
            entity.Is(medical2);

            // 会計2 呼び出し
            var payment2 = GetEntity(DataType.Payment, StatusType.Called, new TimeSpan(16, 0, 0), 3);
            list.Add(payment2);

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 待ち状態の会計2優先
            entity.Is(payment2);

            // 会計2 終了
            payment2.STATUSTYPE = (byte)StatusType.Completed;

            // 実行
            entity = _worker.GetLatestEntity(list);

            // 全て完了済みの場合は受付時間の新しいものが優先される
            entity.Is(payment2);
        }

        


        void SetReceptionTime(QH_WAITINGLIST_DAT entity, int hour, int min)
        {
            entity.RECEPTIONDATE = new DateTime(2022, 11, 11, hour, min, 0);
        }

        QH_WAITINGLIST_DAT GetEntity(DataType dataType, StatusType statusType, TimeSpan receptionTime, int sequence = 0, DateTime? updatedAt = null, string departmentCode = "03")
        {
            var detailEntity = new QhWaitingListValueOfJson
            {
                DepartmentName = "内科",
                DoctorCode = "01",
                DoctorName = "医者",
                DosingSlipNo = "1",
                DosingSlipType = "2",
                InOutType = "3",
                MedicalActCode = "4",
                MedicalActName = "診察",
                RoomCode = "5",
                RoomName = "診察室",
                SameDaySequence = "6",
                DefferedPaymentFlg = "7",
                ChartWaitNumber = 8
            };

            var json = new QsJsonSerializer().Serialize(detailEntity);

            return new QH_WAITINGLIST_DAT
            {
                FACILITYKEY = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"),
                DATATYPE = (byte)dataType,
                WAITINGDATE = new DateTime(2022, 11, 11),
                SEQUENCE = sequence,
                LINKAGESYSTEMNO = 27004,
                LINKAGESYSTEMID = "00001300",
                DEPARTMENTCODE = departmentCode,
                STATUSTYPE = (byte)statusType,
                RECEPTIONNO = "11",
                RESERVATIONNO = "22",
                RESERVATIONDATE = new DateTime(2022,10,11),
                RECEPTIONDATE = new DateTime(2022, 11, 11) + receptionTime,
                UPDATEDDATE = updatedAt ?? DateTime.Now,
                VALUE = json,
                FOREIGNKEY = "99999",
                DELETEFLAG = false,
            };
        }

        QoWaitingListReadApiArgs GetValidArgs()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            return new QoWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };
}
    }
}
