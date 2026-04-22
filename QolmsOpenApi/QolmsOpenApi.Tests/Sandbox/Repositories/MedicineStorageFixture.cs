using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class MedicineStorageFixture
    {
        IMedicineStorageRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new MedicineStorageRepository();
        }

        [TestMethod]
        public void 画像が取得できる()
        {
            var yjCode = "1124017F2046";
            var itemCode = "45010910";
            var itemCodeType = "J";
            var seq = 1;

            var ethfile = _repo.ReadEthImage(yjCode, seq);
            var otcfile = _repo.ReadOtcImage(itemCode, itemCodeType, seq);
        }
        [TestMethod]
        public void 調剤薬情報エンティティが取得できる()
        {
            var yjCode = "1129009F2340";
            var seq = 1;

            // DBの値を取得
            var entity = _repo.ReadEthFileEntity(yjCode, seq);  
            // Blob取得
            var data = _repo.ReadEthBlobEntity(entity.FILEKEY);

        }

        [TestMethod]
        public void 市販薬情報エンティティが取得できる()
        {
            var itemCode = "45010910";
            var itemCodeType = "J";
            var seq = 1;

            // DBの値を取得
            var entity = _repo.ReadOtcFileEntity(itemCode, itemCodeType, seq);
            // ブロブの取得
            var data = _repo.ReadOtcBlobEntity(entity.FILEKEY);

        }
    }
}
