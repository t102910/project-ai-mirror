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
    public class MasterDrugSearchWorkerFixture
    {
        Mock<IOtcDrugRepository> _repo;
        MasterDrugSearchWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IOtcDrugRepository>();
            _worker = new MasterDrugSearchWorker(_repo.Object);
        }

        [TestMethod]
        public void OTC医薬品検索でDBエラーが発生したら失敗する()
        {
            var keyword = "ロキソニン";
            var pageIndex = 0;
            var pageSize = 5;

            var args = new QoMasterDrugSearchApiArgs
            {
                SearchText = keyword,
                PageIndex = pageIndex.ToString(),
                PageSize = pageSize.ToString(),
            };

            // DBからの取得でエラーを発生させる
            _repo.Setup(m => m.SearchDrug(keyword, pageIndex, pageSize)).Throws(new Exception());


            var ret = _worker.Search(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
        }

        [TestMethod]
        public void OTC医薬品検索でその他のエラーが発生したら失敗する()
        {
            var keyword = "ロキソニン";
            var pageIndex = 0;
            var pageSize = 5;

            var args = new QoMasterDrugSearchApiArgs
            {
                SearchText = keyword,
                PageIndex = pageIndex.ToString(),
                PageSize = pageSize.ToString(),
            };

            // null参照エラーが発生するような値を返すようにする
            _repo.Setup(m => m.SearchDrug(keyword, pageIndex, pageSize)).Returns((null,0,0));

            var ret = _worker.Search(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1005"); // 実行エラー
        }

        [TestMethod]
        public void OTC医薬品検索で正しく結果を取得できる()
        {
            var keyword = "ロキソニン";
            var pageIndex = 0;
            var pageSize = 5;

            var args = new QoMasterDrugSearchApiArgs
            {
                SearchText = keyword,
                PageIndex = pageIndex.ToString(),
                PageSize = pageSize.ToString(),
            };

            var dbList = new List<DbOtcDrugSearchItem>
            {
                new DbOtcDrugSearchItem
                {
                    ItemCode = "item1",
                    ItemCodeType = "typeA",
                    MakerOfficialItemName = "ロキソニン",
                    FullWidthContent = 15.25m,
                    FullWidthContentUnit = "mg",
                    FullWidthQuantityUnit = "個",
                },
                new DbOtcDrugSearchItem
                {
                    ItemCode = "item2",
                    ItemCodeType = "typeA",
                    MakerOfficialItemName = "エスタロンモカ",
                    FullWidthContent = 15.25m,
                    FullWidthContentUnit = "mg",
                    FullWidthQuantityUnit = "個",
                },
            };

            // DBからの情報を返す設定
            _repo.Setup(m => m.SearchDrug(keyword, pageIndex, pageSize)).Returns((dbList,0,5));

            var ret = _worker.Search(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // ページ情報が正しく返された
            ret.PageIndex.Is("0");
            ret.MaxPageIndex.Is("5");
            // OTC医薬品が2件取得された
            ret.OtcDrugN.Count.Is(2);
            // 1件目のデータが正しく変換されている
            ret.OtcDrugN[0].ItemCode.Is("item1");
            ret.OtcDrugN[0].ItemCodeType.Is("typeA");
            ret.OtcDrugN[0].MakerName.Is("ロキソニン");
            ret.OtcDrugN[0].ItemName.Is("ロキソニン");
            ret.OtcDrugN[0].ContentQuantity.Is("15.25");
            ret.OtcDrugN[0].ContentUnit.Is("mg");
            ret.OtcDrugN[0].ItemUnit.Is("個");
            // 2件目のデータが正しく変換されている
            ret.OtcDrugN[1].ItemCode.Is("item2");
            ret.OtcDrugN[1].ItemName.Is("エスタロンモカ");
        }
    }
}
