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

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class MasterPharmacyWorkerFixture
    {
        Mock<IFacilityRepository> _repo;
        MasterPharmacyWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IFacilityRepository>();
            _worker = new MasterPharmacyWorker(_repo.Object);
        }

        [TestMethod]
        public void 詳細情報取得で施設キーがEmptyなら失敗する()
        {
            var args = new QoMasterFacilityDetailReadApiArgs
            {
                FacilityKeyReference = Guid.Empty.ToEncrypedReference(),
            };

            var ret = _worker.DetailRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Detail.Contains("施設キーが不正です。").IsTrue();
        }

        [TestMethod]
        public void 詳細情報取得でDBから取得した値がnullだったら失敗する()
        {
            var facilityKey = Guid.NewGuid();
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterFacilityDetailReadApiArgs
            {
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                ActorKey = "5ebdf528-5da2-495f-8a15-f2223dcc2dcf"
            };


            _repo.Setup(m => m.ReadFacility(facilityKey, accountKey)).Returns(default(DbFacilityItem));

            var ret = _worker.DetailRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1006");
        }

        [TestMethod]
        public void 詳細情報取得でDBから取得した値のFacilityKeyがEmptyだったら失敗する()
        {
            var facilityKey = Guid.NewGuid();
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterFacilityDetailReadApiArgs
            {
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                ActorKey = "5ebdf528-5da2-495f-8a15-f2223dcc2dcf"
            };


            _repo.Setup(m => m.ReadFacility(facilityKey, accountKey)).Returns(new DbFacilityItem
            {
                FacilityKey = Guid.Empty
            });

            var ret = _worker.DetailRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1006");
        }

        [TestMethod]
        public void 詳細情報取得でDBアクセス時に例外が発生したら失敗する()
        {
            var facilityKey = Guid.NewGuid();
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterFacilityDetailReadApiArgs
            {
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                ActorKey = "5ebdf528-5da2-495f-8a15-f2223dcc2dcf"
            };


            var ex = new Exception();
            _repo.Setup(m => m.ReadFacility(facilityKey, accountKey)).Throws(ex);

            var ret = _worker.DetailRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003");
        }

        [TestMethod]
        public void 詳細情報取得で正常に取得できる()
        {
            var facilityKey = Guid.NewGuid();
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterFacilityDetailReadApiArgs
            {
                FacilityKeyReference = facilityKey.ToEncrypedReference(),
                ActorKey = "5ebdf528-5da2-495f-8a15-f2223dcc2dcf"
            };

            _repo.Setup(m => m.GetFacilityImageKey(facilityKey)).Returns(new List<QoApiFileKeyItem>
            {
                new QoApiFileKeyItem
                {
                    FileKeyReference = "image1",
                    Sequence = "0"
                },
                new QoApiFileKeyItem
                {
                    FileKeyReference = "image2",
                    Sequence ="1"
                }
            });

            _repo.Setup(m => m.ReadFacility(facilityKey, accountKey)).Returns(new DbFacilityItem
            {
                FacilityKey = facilityKey,                
            });

            var ret = _worker.DetailRead(args);                       

            ret.IsSuccess.Is(bool.TrueString);
            ret.Facility.FacilityKeyReference.Is(facilityKey.ToEncrypedReference());
            ret.Facility.ThumbnailKey.FileKeyReference.Is("image1");
        }

        [TestMethod]
        public void DbFacilityItemをQoApiFacilitySearchResultItemに正常に変換できる()
        {
            var facilityKey = Guid.NewGuid();
            
            _repo.Setup(m => m.GetFacilityImageKey(facilityKey)).Returns(new List<QoApiFileKeyItem>());

            var now = new DateTime(2022, 11, 15, 11, 30, 0);
            var week = $"{(int)now.DayOfWeek}"; // 火曜日 2

            var facilityItem = new DbFacilityItem
            {
                FacilityKey = facilityKey,
                Address1 = "address1",
                Address2 = "address2",
                PostCode = "5550000",
                CityNo = 10,
                PrefectureNo = 14,
                FacilityName = "name",
                FacilityKanaName = "ネーム",
                Tel = "0699999999",
                Latitude = 42.134m,
                Longitude = 130.135m,
                OfficeHour = new DbFacilityOfficeHourItem
                {
                    DayOfWeek = 1,
                    TreatmentStartTime = "1130",
                    TreatmentEndTime = "1930",
                },
                OfficeHourN = new List<DbFacilityOfficeHourItem>
                {
                    new DbFacilityOfficeHourItem
                    {
                        DayOfWeek = -1,
                        TreatmentStartTime = "1030",
                        TreatmentEndTime = "1830"
                    },
                    new DbFacilityOfficeHourItem
                    {
                        DayOfWeek = 1,
                        TreatmentStartTime = "1130",
                        TreatmentEndTime = "1930"
                    }
                },
                FavoriteFlag = true,
                FlagN = new List<DbFacilityFlagResultsItem>
                {
                    new DbFacilityFlagResultsItem
                    {
                        FlagNo = 1,
                    },
                    new DbFacilityFlagResultsItem
                    {
                        FlagNo = 2,
                    }
                }
            };

            var converted =_worker.ConvertApiFacilityItem(facilityItem, now);

            // 正しく変換されている
            converted.FacilityKeyReference.Is(facilityKey.ToEncrypedReference());
            converted.Address1.Is(facilityItem.Address1);
            converted.Address2.Is(facilityItem.Address2);
            converted.PostalCode.Is(facilityItem.PostCode);
            converted.Name.Is(facilityItem.FacilityName);
            converted.NameKana.Is(facilityItem.FacilityKanaName);
            converted.Tel.Is(facilityItem.Tel);
            converted.Location.Latitude.Is("42.134");
            converted.Location.Longitude.Is("130.135");
            converted.FileKeyN.Count.Is(0);
            converted.ThumbnailKey.FileKeyReference.Is(string.Empty);
            converted.TodayOpeningTime.DayOfWeek.Is("1"); // OfficeHourの値がとられる
            converted.TodayOpeningTime.StartTime.Is("1130");
            converted.TodayOpeningTime.EndTime.Is("1930");
            converted.OpeningTimeN.Count.Is(2);
            converted.OpeningTimeN[0].DayOfWeek.Is(week); // nowの値がとられる
            converted.OpeningTimeN[0].StartTime.Is("1030");
            converted.OpeningTimeN[0].EndTime.Is("1830");
            converted.OpeningTimeN[1].DayOfWeek.Is("1"); // OfficeHourの値がとられる
            converted.OpeningTimeN[1].StartTime.Is("1130");
            converted.OpeningTimeN[1].EndTime.Is("1930");
            converted.IsOpening.Is(bool.TrueString); // Start-Endの間なのでOpen扱い
            converted.IsAccepting.Is(bool.TrueString);
            converted.IsFavorite.Is(bool.TrueString);
            converted.FlagN.Count.Is(2);
            converted.FlagN[0].FlagType.Is("1");
            converted.FlagN[1].FlagType.Is("2");

            // ギリギリ開店前
            now = new DateTime(2022, 11, 15, 11, 29, 59);

            converted = _worker.ConvertApiFacilityItem(facilityItem, now);

            // 閉店扱いとなっている
            converted.IsOpening.Is(bool.FalseString);
        }
    }
}
