using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class LinkageFixture
    {
        ILinkageRepository _repo;
        Guid _testAccountKey = Guid.Parse("dbd5ef29-f7d4-42f3-8af0-ec3a96052be8");
        Guid _testAccountKey2 = Guid.Parse("dbd5ef29-f7d4-42f3-8af0-ec3a96052be9");

        [TestInitialize]
        public void Initialize()
        {
            _repo = new LinkageRepository();
        }

        [TestMethod]
        public void LinkageSystemIdと施設キーからアカウントを取得できる()
        {
            var accountKey = _repo.GetAccountKey("00001300", Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"));
        }

        [TestMethod]
        public void QH_LINKAGE_DATにINSERTできる()
        {
            var entity = new QH_LINKAGE_DAT
            {
                ACCOUNTKEY = _testAccountKey,
                LINKAGESYSTEMNO = 99001,
                LINKAGESYSTEMID = "Test",
                STATUSTYPE = 2,
            };

            _repo.InsertEntity(entity);
        }

        [TestMethod]
        public void QH_LINKAGE_DATを1件取得()
        {
            var entity = _repo.ReadEntity(_testAccountKey, 88888);
        }

        [TestMethod]
        public void トランザクションの動作確認()
        {
            // トランザクション開始
            using (var scope = new QoTransaction())
            {
                var entity = new QH_LINKAGE_DAT
                {
                    ACCOUNTKEY = _testAccountKey2,
                    LINKAGESYSTEMNO = 99001,
                    LINKAGESYSTEMID = "Test2",
                    STATUSTYPE = 2,
                };

                _repo.InsertEntity(entity);

                entity = new QH_LINKAGE_DAT
                {
                    ACCOUNTKEY = _testAccountKey,
                    LINKAGESYSTEMNO = 99001,
                    LINKAGESYSTEMID = "Test",
                    STATUSTYPE = 2,
                };

                _repo.InsertEntity(entity);

                scope.Commit();
            }           
        }

        [TestMethod]
        public void トランザクション中の読み取りテスト()
        {
            using (var scope = new QoTransaction())
            {
                var entity = new QH_LINKAGE_DAT
                {
                    ACCOUNTKEY = _testAccountKey,
                    LINKAGESYSTEMNO = 99001,
                    LINKAGESYSTEMID = "Test",
                    STATUSTYPE = 2,
                };

                _repo.DeleteEntity(entity);

                // トランザクション中の変更を取得できる
                var readed = _repo.ReadEntity(_testAccountKey, 999001);

                readed.IsNull();

                //scope.Commit();
            }
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var key = Guid.Parse("d33c3215-1b21-4531-8603-7f564f2dcd69");
            var keyref = key.ToEncrypedReference();
        }

        [TestMethod]
        public void QH_LINKAGE_DATを更新できる()
        {
            var entity = new QH_LINKAGE_DAT
            {
                ACCOUNTKEY = _testAccountKey,
                LINKAGESYSTEMNO = 99001,
                LINKAGESYSTEMID = "Test Edit",
                STATUSTYPE = 2,
            };

            _repo.UpdateEntity(entity);
        }

        [TestMethod]
        public void QH_LINKAGE_DATを削除できる()
        {
            var entity = new QH_LINKAGE_DAT
            {
                ACCOUNTKEY = _testAccountKey,
                LINKAGESYSTEMNO = 99001,
                LINKAGESYSTEMID = "Test Edit Deleted",
                STATUSTYPE = 2,
            };

            _repo.DeleteEntity(entity);
        }

        [TestMethod]
        public void 連携データの更新日時を取得できる()
        {
            // 対象のアカウント
            var accountKey = Guid.Parse("9c1c613d-af67-4fc5-809d-fdbdb930df66");
            // 対象の施設
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var ret = _repo.GetLinkageUpdated(facilityKey, "88888009");    
            // retをQH_LINKAGE_DATと比較して合っていればOK
        }

        [TestMethod]
        public void カードの重複確認ができる()
        {
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var cardNo = "99990070";
            var ret = _repo.IsAvailableCard(facilityKey, cardNo);

            ret.IsFalse();

            ret = _repo.IsAvailableCard(facilityKey, "99990099");
            ret.IsTrue();
        }

        [TestMethod]
        public void 親の連携システム番号を取得できる()
        {
            // 和泉
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var ret = _repo.GetParentLinkageMst(facilityKey);

            // TISの連携システム番号がとれた
            ret.LINKAGESYSTEMNO.Is(27003);

            // 存在しない施設キー
            facilityKey = Guid.Parse("11dc3f56-5652-4d08-0000-1575c1723edb");
            ret = _repo.GetParentLinkageMst(facilityKey);

            // 該当無し
            ret.IsNull();

            // 和泉上位組織のTIS
            facilityKey = Guid.Parse("b57cd983-d8f1-4b42-9aa4-d45e5e2f0dda");
            ret = _repo.GetParentLinkageMst(facilityKey);

            // TISの上位はMGFだがMGFは特殊で連携システム番号をもたないので該当無しとなる
            ret.IsNull();
        }

        [TestMethod]
        public void 同じ親を持つ連携情報を取得する()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var parentFacility = Guid.Parse("B57CD983-D8F1-4B42-9AA4-D45E5E2F0DDA");

            var ret = _repo.ReadChildLinkageList(accountKey, parentFacility);
        }

        [TestMethod]
        public void 診察券の登録()
        {
            var facilityKey = Guid.Parse("d33c3215-1b21-4531-8603-7f564f2dcd69");
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var ret = _repo.WriteLinkagePatientCard(accountKey, accountKey, 99001, facilityKey, "88881234", int.MinValue, false);
        }

        [TestMethod]
        public void 診察券の削除()
        {
            var facilityKey = Guid.Parse("d33c3215-1b21-4531-8603-7f564f2dcd69");
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var ret = _repo.WriteLinkagePatientCard(accountKey, accountKey, 99001, facilityKey, "88881234", 1, true);
        }

        [TestMethod]
        public void 通知用ユーザーIDの取得()
        {
            var list = new List<string>
            {
                "00001300","99999002","99999005"
            };

            var ret = _repo.ReadPushNotificationUserView(99001, 28001, list);
        }
    }
}
