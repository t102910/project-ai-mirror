using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class FacilityFixture
    {
        IFacilityRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new FacilityRepository();
        }

        [TestMethod]
        public void 薬局検索できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var ret = _repo.SearchPharmacyByFiltering(
                accountKey,
                "フローラ",
                decimal.Zero,
                decimal.Zero,
                14,
                int.MinValue,
                DateTime.Now,
                false,
                0,
                5
                );

            var facilityN = _repo.SearchPharmacyByLocation(accountKey,34.6761785m, 135.5578905m, DateTime.Now, false, 0, 500);
        }

        [TestMethod]
        public void 施設詳細を取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var facilityKey = Guid.Parse("5deb2f7c-4c60-4f64-ba98-4d4cf7966dec");

            var ret = _repo.ReadFacility(facilityKey, accountKey);

        }

        [TestMethod]
        public void 施設情報を主キーのみで取得()
        {
            var facilityKey = Guid.Parse("5deb2f7c-4c60-4f64-ba98-4d4cf7966dec");

            var ret = _repo.ReadFacility(facilityKey);
        }

        [TestMethod]
        public void 祝日を判定できる()
        {
            // 勤労感謝の日
            var ret = _repo.IsHoliday(new DateTime(2021, 11, 23,23,59,59));

            ret.IsTrue();

            // 平日
            ret = _repo.IsHoliday(new DateTime(2021, 11, 1, 13, 10, 10));

            ret.IsFalse();
        }

        [TestMethod]
        public void 画像キーを取得()
        {
            var ret = _repo.GetFacilityImageKey(Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));
        }

        [TestMethod]
        public void 医療機関リストを連携システム番号から取得する()
        {
            var list = _repo.ReadMedicalFacilityListByLinkageSystemNo(27003, DateTime.MinValue);
        }

        [TestMethod]
        public void 医療機関リストを施設キーから取得する()
        {
            var list = _repo.ReadMedicalFacilityListByKey(DateTime.MinValue,
                Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB"),
                Guid.Parse("0544A136-84FB-4AA8-80BF-7EA48B93B50F"));
                // Guid.Parse("3473FEF8-58C7-409A-9039-000445430625"));

            var json = JsonConvert.SerializeObject(list);
        }

        [TestMethod]
        public void 施設言語リソースを取得する()
        {
            //var list = _repo.ReadFacilityLanguage(Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB"));
            var list = _repo.ReadFacilityLanguage(Guid.Parse("c9add86d-3343-4fea-9cb4-23132f075194"));
            var json = list.Where(x => x.LANGUAGEKEY == "Push_Medicine_Ready").FirstOrDefault().VALUE;
            var langResource = JsonConvert.DeserializeObject<LangResource[]>(json);
        }

        [TestMethod]
        public void 施設設定を取得()
        {
            var key = new Guid("711e9a95-c8d1-4f82-a1b4-2f521b1b7f84");
            var result = _repo.ReadFacilityConfig(key);
            var json = new QsJsonSerializer().Deserialize<QhFacilityConfigSetOfJson>(result.CONFIGSET);
        }

        private class LangResource
        {
            public int Language { get; set; }
            public string Value { get; set; }
        }
    }
}
