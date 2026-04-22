using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class NoticeFixture
    {
        INoticeRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new NoticeRepository();
        }

        [TestMethod]
        public void お知らせリストを取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            
            var ret = _repo.ReadList(
                accountKey,
                QsApiSystemTypeEnum.QolmsiOSApp,
                new DateTime(2020, 10, 1, 0, 0, 0),
                DateTime.Now,
                0,
                5
            );            
        }

        [TestMethod]
        public void お知らせリストをフィルター適用して取得()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var now = new DateTime(2023,7,19,11,5,0);

            // HOSPA iOSと共通
            var toSystemList = QsApiSystemTypeEnum.TisiOSApp.ToApplicationTypeByteList();

            var ret = _repo.ReadList(accountKey, toSystemList, new List<Guid>(), new List<byte>(), now, now, 0, 10);
        }

        [TestMethod]
        public void お知らせリストをフィルター適用して取得2()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var now = new DateTime(2023, 7, 19, 11, 5, 0);

            // HOSPA iOSと共通
            var toSystemList = QsApiSystemTypeEnum.TisiOSApp.ToApplicationTypeByteList();

            // 施設絞り込み
            var facilityList = new List<Guid> { Guid.Parse("B57CD983-D8F1-4B42-9AA4-D45E5E2F0DDA") };

            var ret = _repo.ReadList(accountKey, toSystemList, facilityList, new List<byte>(), now, now, 0, 10);
        }

        [TestMethod]
        public void お知らせリストをフィルター適用して取得3()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var now = new DateTime(2023, 7, 19, 11, 5, 0);

            // HOSPA iOSと共通
            var toSystemList = QsApiSystemTypeEnum.TisiOSApp.ToApplicationTypeByteList();

            // 施設絞り込み
            var facilityList = new List<Guid> { Guid.Parse("B57CD983-D8F1-4B42-9AA4-D45E5E2F0DDA") };

            // カテゴリー指定
            var catList = new List<byte> { 1 };

            var ret = _repo.ReadList(accountKey, toSystemList, facilityList, catList, now, now, 0, 10);
        }

        [TestMethod]
        public void お知らせをIDで取得()
        {
            var ret = _repo.ReadById(718);

            // 削除済みはとれない
            ret = _repo.ReadById(670);
            ret.IsNull();
        }
    }
}
