using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class LinkagePatientCardDeleteWorkerFixture
    {
        LinkagePatientCardDeleteWorker _worker;
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IAccountRepository> _accountRepo;

        Guid _accountKey = Guid.NewGuid();
        Guid _executor = Guid.NewGuid();
        Guid _facilityKey = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec".ToDecrypedReference().TryToValueType(Guid.Empty);
        Guid _parentFacilityKey = Guid.NewGuid();

        [TestInitialize]
        public void Initlaize()
        {
            _linkageRepo = new Mock<ILinkageRepository>();
            _accountRepo = new Mock<IAccountRepository>();
            _worker = new LinkagePatientCardDeleteWorker(_linkageRepo.Object, _accountRepo.Object);
        }

        [TestMethod]
        public void AccountKeyが不正でエラー()
        {
            var args = GetValidArgs();
            args.ActorKey = "";

            var results = _worker.Delete(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
        }

        [TestMethod]
        public void AuthorKeyが不正でエラー()
        {
            var args = GetValidArgs();
            args.Executor = "";

            var results = _worker.Delete(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.AuthorKey)).IsTrue();
        }

        [TestMethod]
        public void 施設キーが不正でエラー()
        {
            var args = GetValidArgs();
            args.FacilityKeyReference = "";

            var results = _worker.Delete(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains(nameof(args.FacilityKeyReference)).IsTrue();
        }

        [TestMethod]
        public void 施設キーから連携システム番号に変換できなければエラー終了()
        {
            var args = GetValidArgs();

            // 変換失敗
            _linkageRepo.Setup(m => m.GetLinkageNo(_facilityKey)).Returns(-1);

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("施設キーを連携システム番号に変換できませんでした").IsTrue();
        }

        [TestMethod]
        public void 診察券削除処理で失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, string.Empty, 1, true)).Returns(("hoge",null));

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("診察券の登録に失敗しました").IsTrue();
            results.Result.Detail.Contains("hoge").IsTrue();
        }

        [TestMethod]
        public void 診察券削除処理で例外が発生するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, string.Empty, 1, true)).Throws(new Exception());

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("診察券削除処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 親連携削除処理で例外が発生するとエラーとなる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 親連携削除処理内で例外を起こす
            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Throws(new Exception());

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("上位施設の連携解除処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 親連携解除処理は連携システム番号が指定されていなければ実行されない()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            args.LinkageSystemNo = "";

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 親連携情報取得処理は実行されなかった（スキップの証明）
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Never());
        }

        [TestMethod]
        public void 親連携解除処理は親が存在しなければ実行されない()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 親無し
            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Returns(default(QH_LINKAGESYSTEM_MST));

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 後続処理は実行されなかった（スキップの証明）
            _linkageRepo.Verify(m => m.ReadChildLinkageList(_accountKey, _parentFacilityKey), Times.Never());
        }

        [TestMethod]
        public void 親連携解除処理はルート連携番号と同一の場合は実行されない()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = 27003, // ルート連携番号
                FACILITYKEY = _parentFacilityKey
            });

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 後続処理は実行されなかった（スキップの証明）
            _linkageRepo.Verify(m => m.ReadChildLinkageList(_accountKey, _parentFacilityKey), Times.Never());
        }

        [TestMethod]
        public void 親連携解除処理は子の連携が残っていれば実行されない
            ()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 子連携が残っている
            _linkageRepo.Setup(m => m.ReadChildLinkageList(_accountKey, _parentFacilityKey)).Returns(new List<QH_LINKAGE_DAT>
            {
                new QH_LINKAGE_DAT(),new QH_LINKAGE_DAT()
            });

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 後続処理は実行されなかった（スキップの証明）
            _linkageRepo.Verify(m => m.ReadEntity(_accountKey, 33333), Times.Never());
        }

        [TestMethod]
        public void 親連携解除処理は親連携レコードが存在しなければ実行されない()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 親連携レコードが存在しない
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 33333)).Returns(default(QH_LINKAGE_DAT));

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 後続処理は実行されなかった（スキップの証明）
            _linkageRepo.Verify(m => m.DeleteEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Never());
        }

        [TestMethod]
        public void 全ての処理に成功する()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 診察券が削除された
            _linkageRepo.Verify(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, string.Empty, 1, true), Times.Once);

            // 親連携レコードが解除された
            _linkageRepo.Verify(m => m.DeleteEntity(It.IsAny<QH_LINKAGE_DAT>()), Times.Once);
        }

        void SetupValidMethods(QoLinkagePatientCardDeleteApiArgs args)
        {
            // 施設キー変換
            _linkageRepo.Setup(m => m.GetLinkageNo(_facilityKey)).Returns(27004);

            // 診察券削成功
            _linkageRepo.Setup(m => m.WriteLinkagePatientCard(_executor, _accountKey, 27004, _facilityKey, string.Empty, 1, true)).Returns(("",null));

            // 親連携システム情報
            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                LINKAGESYSTEMNO = 33333,
                FACILITYKEY = _parentFacilityKey
            });

            // 子連携無し
            _linkageRepo.Setup(m => m.ReadChildLinkageList(_accountKey, _parentFacilityKey)).Returns(new List<QH_LINKAGE_DAT>());

            // 親連携レコードあり
            _linkageRepo.Setup(m => m.ReadEntity(_accountKey, 33333)).Returns(new QH_LINKAGE_DAT());
        }

        QoLinkagePatientCardDeleteApiArgs GetValidArgs()
        {
            return new QoLinkagePatientCardDeleteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                LinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO.ToString(),
                FacilityKeyReference = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec"
            };
        }
    }
}
