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

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class TisWaitingListReadWorkerFixture
    {
        Mock<IWaitingRepository> _repo;
        TisWaitingListReadWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IWaitingRepository>();
            _worker = new TisWaitingListReadWorker(_repo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正で失敗が返る()
        {
            var args = new QoTisWaitingListReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString()
            };

            var ret = _worker.ReadLatest(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("対象が特定できません").IsTrue();
        }

        [TestMethod]
        public void 施設キーが不正で失敗が返る()
        {
            var args = new QoTisWaitingListReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                FacilityKeyReference = ""
            };

            var ret = _worker.ReadLatest(args);

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

            var args = new QoTisWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 例外を設定
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Throws(new Exception("hoge"));

            var ret = _worker.ReadLatest(args);

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

            var args = new QoTisWaitingListReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                TargetDate = targetDate.ToApiDateString(),
                DataType = "0"
            };

            // 0件
            _repo.Setup(m => m.ReadLatestWaitingList(accountKey, facilityKey, targetDate, 0)).Returns(new List<QH_WAITINGLIST_DAT>());

            var ret = _worker.ReadLatest(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            ret.Sequence.Is("");
            // 待ち人数取得処理には到達しない
            _repo.Verify(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, It.IsAny<byte>()), Times.Never);
        }

        [TestMethod]
        public void 順番待ち情報が存在するが全て不正データの場合はデータ無しとして成功を返す()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoTisWaitingListReadApiArgs
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

            var ret = _worker.ReadLatest(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            ret.Sequence.Is("");
            // 待ち人数取得処理には到達しない
            _repo.Verify(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, It.IsAny<byte>()), Times.Never);
        }

        [TestMethod]
        public void 待ち人数取得処理の例外発生で失敗が返る()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoTisWaitingListReadApiArgs
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

            // 待ち人数取得で例外
            _repo.Setup(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, 1)).Throws(new Exception("hoge"));

            var ret = _worker.ReadLatest(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
            ret.Result.Detail.Contains("待ち人数取得処理エラー").IsTrue();
        }

        [TestMethod]
        public void 正常に終了する_会計時薬なし()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoTisWaitingListReadApiArgs
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

            // 待ち人数取得で1を返す設定
            _repo.Setup(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, 1)).Returns(1);

            
            var ret = _worker.ReadLatest(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // データが正しく設定されている
            ret.Sequence.Is("0");
            ret.DepartmentCode.Is("03");
            ret.DepartmentName.Is("内科");
            ret.DataType.Is("1");
            ret.ReceptionNo.Is("11");
            ret.ReceptionTime.Is(targetDate.ToApiDateString());
            ret.ReservationNo.Is("22");
            ret.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            ret.StatusType.Is("10");
            ret.WaitingDate.Is(targetDate.ToApiDateString());
            ret.HasMedicineWithPayment.Is(bool.FalseString);
            ret.Detail.DepartmentName.Is("内科");
            ret.Detail.DoctorCode.Is("01");
            ret.Detail.DoctorName.Is("医者");
            ret.Detail.DosingSlipNo.Is("1");
            ret.Detail.DosingSlipType.Is("2");
            ret.Detail.InOutType.Is("3");
            ret.Detail.MedicalActCode.Is("4");
            ret.Detail.MedicalActName.Is("診察");
            ret.Detail.RoomCode.Is("5");
            ret.Detail.RoomName.Is("診察室");
            ret.Detail.SameDaySequence.Is("6");
            ret.Detail.DefferedPaymentFlg.Is("7");
            ret.Detail.ChartWaitNumber.Is((sbyte)8);
            ret.WaitingCount.Is("1");
        }

        [TestMethod]
        public void 正常に終了する_会計時薬あり()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var targetDate = new DateTime(2022, 11, 11);

            var args = new QoTisWaitingListReadApiArgs
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

            // 待ち人数取得で1を返す設定
            _repo.Setup(m => m.GetWaitingCount(accountKey, facilityKey, targetDate, 3)).Returns(1);


            var ret = _worker.ReadLatest(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // データが正しく設定されている
            ret.Sequence.Is("0");
            ret.DepartmentCode.Is("03");
            ret.DepartmentName.Is("内科");
            ret.DataType.Is("3");
            ret.ReceptionNo.Is("11");
            ret.ReceptionTime.Is(targetDate.ToApiDateString());
            ret.ReservationNo.Is("22");
            ret.ReservationTime.Is(new DateTime(2022, 10, 11).ToApiDateString());
            ret.StatusType.Is("2");
            ret.WaitingDate.Is(targetDate.ToApiDateString());
            ret.HasMedicineWithPayment.Is(bool.TrueString); // 会計時薬あり
            ret.Detail.DepartmentName.Is("内科");
            ret.Detail.DoctorCode.Is("01");
            ret.Detail.DoctorName.Is("医者");
            ret.Detail.DosingSlipNo.Is("1");
            ret.Detail.DosingSlipType.Is("2");
            ret.Detail.InOutType.Is("3");
            ret.Detail.MedicalActCode.Is("4");
            ret.Detail.MedicalActName.Is("診察");
            ret.Detail.RoomCode.Is("5");
            ret.Detail.RoomName.Is("診察室");
            ret.Detail.SameDaySequence.Is("6");
            ret.Detail.DefferedPaymentFlg.Is("7");
            ret.Detail.ChartWaitNumber.Is((sbyte)8);
            ret.WaitingCount.Is("1");
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

        QH_WAITINGLIST_DAT GetEntity(DataType dataType, StatusType statusType, TimeSpan receptionTime, int sequence = 0, DateTime? updatedAt = null)
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
                DEPARTMENTCODE = "03",
                STATUSTYPE = (byte)statusType,
                RECEPTIONNO = "11",
                RESERVATIONNO = "22",
                RESERVATIONDATE = new DateTime(2022,10,11),
                RECEPTIONDATE = new DateTime(2022, 11, 11) + receptionTime,
                UPDATEDDATE = updatedAt ?? DateTime.Now,
                VALUE = json
            };
        }
    }
}
