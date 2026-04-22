using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppDownloadImageWorkerFixture
    {
        AppDownloadImageWorker _worker;
        Mock<IMedicineStorageRepository> _medStorageRepo;
        Mock<IStorageRepository> _storageRepo;

        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _storageRepo = new Mock<IStorageRepository>();
            _medStorageRepo = new Mock<IMedicineStorageRepository>();
            _worker = new AppDownloadImageWorker(_storageRepo.Object, _medStorageRepo.Object);
        }

        [TestMethod]
        public async Task オリジナル画像を正常にダウンロードできる()
        {
            var fileKey = Guid.NewGuid();
            var fileKeyReference = fileKey.ToEncrypedReference();

            _storageRepo.Setup(m => m.ReadImage(fileKey, QsApiFileTypeEnum.Original)).Returns(new QoApiFileItem
            {
                ContentType = "image/jpeg",
                OriginalName = "model.jpg",
                Data = "hoge"
            });

            var response = _worker.DownloadImage(fileKeyReference);

            var data = await response.Content.ReadAsByteArrayAsync();

            Convert.ToBase64String(data).Is("hoge");
            response.Content.Headers.ContentType.ToString().Is("image/jpeg");
        }

        [TestMethod]
        public async Task 加工された画像を正常にダウンロードできる()
        {
            var fileKey = Guid.NewGuid();
            var fileKeyReference = fileKey.ToEncrypedReference();

            _storageRepo.Setup(m => m.ReadImage(fileKey, QsApiFileTypeEnum.Edited)).Returns(new QoApiFileItem
            {
                ContentType = "image/jpeg",
                OriginalName = "model.jpg",
                Data = "hoge"
            });

            var response = _worker.DownloadImage(fileKeyReference, QsApiFileTypeEnum.Edited);

            var data = await response.Content.ReadAsByteArrayAsync();

            Convert.ToBase64String(data).Is("hoge");
            response.Content.Headers.ContentType.ToString().Is("image/jpeg");
        }

        [TestMethod]
        public async Task サムネイル画像を正常にダウンロードできる()
        {
            var fileKey = Guid.NewGuid();
            var fileKeyReference = fileKey.ToEncrypedReference();

            _storageRepo.Setup(m => m.ReadImage(fileKey, QsApiFileTypeEnum.Thumbnail)).Returns(new QoApiFileItem
            {
                ContentType = "image/jpeg",
                OriginalName = "model.jpg",
                Data = "hoge"
            });

            var response = _worker.DownloadImage(fileKeyReference,QsApiFileTypeEnum.Thumbnail);

            var data = await response.Content.ReadAsByteArrayAsync();

            Convert.ToBase64String(data).Is("hoge");
            response.Content.Headers.ContentType.ToString().Is("image/jpeg");
        }

        [TestMethod]
        public async Task 調剤薬画像を正常にダウンロードできる()
        {
            var yjCode = "1124017F2046";
            var seq = 1;

            _medStorageRepo.Setup(m => m.ReadEthImage(yjCode, seq)).Returns(new QoApiFileItem
            {
                ContentType = "image/jpeg",
                OriginalName = "model.jpg",
                Data = "hoge"
            });

            var response = _worker.DownloadImage(yjCode, QsApiFileTypeEnum.Original,QsApiFileCategoryTyepEnum.EthDrug, seq);

            var data = await response.Content.ReadAsByteArrayAsync();

            Convert.ToBase64String(data).Is("hoge");
            response.Content.Headers.ContentType.ToString().Is("image/jpeg");
        }

        [TestMethod]
        public async Task 市販薬画像を正常にダウンロードできる()
        {
            var itemCode = "45010910";
            var itemCodeType = "J";
            var seq = 1;

            _medStorageRepo.Setup(m => m.ReadOtcImage(itemCode,itemCodeType,seq)).Returns(new QoApiFileItem
            {
                ContentType = "image/jpeg",
                OriginalName = "model.jpg",
                Data = "hoge"
            });

            var response = _worker.DownloadImage($"{itemCode}{itemCodeType}", QsApiFileTypeEnum.Original, QsApiFileCategoryTyepEnum.OtcDrug, seq);

            var data = await response.Content.ReadAsByteArrayAsync();

            Convert.ToBase64String(data).Is("hoge");
            response.Content.Headers.ContentType.ToString().Is("image/jpeg");
        }

        [TestMethod]
        public void 画像の取得に失敗するとNotFoundが返る()
        {
            var fileKey = Guid.NewGuid();
            var fileKeyReference = fileKey.ToEncrypedReference();

            _storageRepo.Setup(m => m.ReadImage(fileKey, QsApiFileTypeEnum.Thumbnail)).Throws(new Exception());

            var response = _worker.DownloadImage(fileKeyReference, QsApiFileTypeEnum.Thumbnail);

            response.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void 調剤薬画像の取得に失敗するとNotFoundが返る()
        {
            var yjCode = "1124017F2046";
            var seq = 1;

            _medStorageRepo.Setup(m => m.ReadEthImage(yjCode, seq)).Throws(new Exception());

            var response = _worker.DownloadImage(yjCode, QsApiFileTypeEnum.Original,QsApiFileCategoryTyepEnum.EthDrug,seq);

            response.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void 市販薬画像の取得に失敗するとNotFoundが返る()
        {
            var itemCode = "45010910";
            var itemCodeType = "J";
            var seq = 1;

            _medStorageRepo.Setup(m => m.ReadOtcImage(itemCode, itemCodeType, seq)).Throws(new Exception());

            var response = _worker.DownloadImage($"{itemCode}{itemCodeType}", QsApiFileTypeEnum.Original, QsApiFileCategoryTyepEnum.OtcDrug, seq);

            response.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
        }

    }
}
