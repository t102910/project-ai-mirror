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

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class KagaminoInitWorkerFixture
    {
        Mock<IQkModelRepository> _modelRepo;
        Mock<IQkBackgroundRepository> _backgroundRepo;
        KagaminoInitWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _modelRepo = new Mock<IQkModelRepository>();
            _backgroundRepo = new Mock<IQkBackgroundRepository>();

            _worker = new KagaminoInitWorker(_modelRepo.Object, _backgroundRepo.Object);
        }

        [TestMethod]
        public void 初期化データを正常に取得できる()
        {
            var args = new QoKagaminoInitApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge",
                BackgroundId = "fuga"
            };

            var modelFileKey = Guid.NewGuid();

            _modelRepo.Setup(m => m.Read("hoge")).Returns(new QK_MODEL_MST
            {
                MODELID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = modelFileKey,
                THUMBNAILKEY = Guid.NewGuid(),
                ISPAID = false,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2000, 1, 1, 1, 1, 0) // 相当古い
            });

            _modelRepo.Setup(m => m.ReadFile(modelFileKey)).Returns("modelData");

            var backFileKey = Guid.NewGuid();

            _backgroundRepo.Setup(m => m.Read("fuga")).Returns(new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "fuga",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = backFileKey,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(1999, 1, 1, 1, 1, 0) // 相当古い
            });

            _backgroundRepo.Setup(m => m.ReadFile(backFileKey)).Returns("backData");

            var result = _worker.GetInitData(args);

            result.IsSuccess.Is(bool.TrueString);
            result.ModelData.Is("modelData");
            result.ModelUpdatedDate.Is(new DateTime(2000, 1, 1, 1, 1, 0).ToApiDateString());
            result.IsModelDataNoChanged.IsFalse();
            result.BackgroundData.Is("backData");
            result.BackgroundUpdatedDate.Is(new DateTime(1999, 1, 1, 1, 1, 0).ToApiDateString());
            result.IsBackgroundDataNoChanged.IsFalse();
        }

        [TestMethod]
        public void モデルデータ取得でエラーがあればエラーを返す()
        {
            var args = new QoKagaminoInitApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge",
                BackgroundId = "fuga"
            };

            _modelRepo.Setup(m => m.Read("hoge")).Throws(new Exception("hoge"));

            var result = _worker.GetInitData(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("0500");
        }

        [TestMethod]
        public void 背景データ取得でエラーがあればエラーを返す()
        {
            var args = new QoKagaminoInitApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge",
                BackgroundId = "fuga"
            };

            var modelFileKey = Guid.NewGuid();

            _modelRepo.Setup(m => m.Read("hoge")).Returns(new QK_MODEL_MST
            {
                MODELID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = modelFileKey,
                THUMBNAILKEY = Guid.NewGuid(),
                ISPAID = false,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2000, 1, 1, 1, 1, 0) // 相当古い
            });

            _modelRepo.Setup(m => m.ReadFile(modelFileKey)).Returns("modelData");


            _backgroundRepo.Setup(m => m.Read("fuga")).Throws(new Exception("fuga"));

            var result = _worker.GetInitData(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("0500");
        }
    }
}
