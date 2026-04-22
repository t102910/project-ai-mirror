using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Providers;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class ManagementWorkerFixture
    {
        Mock<IManagementRepository> _repo;
        Mock<IDateTimeProvider> _dateTimePro;
        ManagementWorker _worker;

        string _contentType = "application/octet-stream";
        Guid _filekey = Guid.NewGuid();
        byte _processing = 1;

        [TestInitialize]
        public void Initialize()
        {
            _dateTimePro = new Mock<IDateTimeProvider>();
            _repo = new Mock<IManagementRepository>();
            _worker = new ManagementWorker(_repo.Object, _dateTimePro.Object);
        }

        [TestMethod]
        public async Task ファイルの内容でエラーだった場合はエラーとする()
        {
            var args = GetValidArgs();
            args.FileData = "";

            var results = _worker.FileUpload(args);

            // 失敗1002引数エラー
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("ファイル形式が不正です。").IsTrue();
        }


        [TestMethod]
        public async Task ファイル名でエラーだった場合はエラーとする()
        {
            var args = GetValidArgs();
            args.OriginalName = "123456789012345678901234567890123456789012345678901";//51文字

            var results = _worker.FileUpload(args);

            // 失敗1002引数エラー
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("ファイル形式が不正です。").IsTrue();
        }


        [TestMethod]
        public async Task ブロブに登録失敗エラーだった場合はエラーとする()
        {
            var args = GetValidArgs();

            _repo.Setup(m => m.UproadPostingFileBlob(_contentType, Convert.FromBase64String(args.FileData), string.Empty, args.OriginalName, args.LinkageSystemNo.TryToValueType(int.MinValue), args.AuthorKey.TryToValueType(Guid.Empty))).Returns(Guid.Empty);
            //_repo.Setup(m => m.UproadFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue), long.MinValue, byte.MinValue, string.Empty, DateTime.Now)).Returns(true);//要確認

            var results = _worker.FileUpload(args);

            // 失敗1003引数エラー
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1003");
            results.Result.Detail.Contains("登録に失敗しました。").IsTrue();
        }

        [TestMethod]
        public async Task DBに登録失敗エラーだった場合はエラーとする()
        {
            var args = GetValidArgs();

            var now = DateTime.Now;
            _repo.Setup(m => m.UproadPostingFileBlob(_contentType, Convert.FromBase64String(args.FileData), string.Empty, args.OriginalName, args.LinkageSystemNo.TryToValueType(int.MinValue), args.AuthorKey.TryToValueType(Guid.Empty))).Returns(_filekey);
            _repo.Setup(m => m.InsertPostingFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue), _filekey, args.OriginalName, _contentType, _processing, string.Empty, false, now, args.AuthorKey.TryToValueType(Guid.Empty))).Returns(false);
            _repo.Setup(m => m.DeletePostingFileBlob(_filekey)).Returns(true);

            var results = _worker.FileUpload(args);

            // 失敗1003引数エラー
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1003");
            results.Result.Detail.Contains("SQL実行エラー 登録に失敗しました。").IsTrue();
        }


        [TestMethod]
        public async Task 正常に登録できる()
        {
            var args = GetValidArgs();

            _dateTimePro.Setup(m => m.Now).Returns(DateTime.Now);
            _repo.Setup(m => m.UproadPostingFileBlob(_contentType, Convert.FromBase64String(args.FileData),string.Empty,args.OriginalName,args.LinkageSystemNo.TryToValueType(int.MinValue), args.AuthorKey.TryToValueType(Guid.Empty))).Returns(_filekey);
            _repo.Setup(m => m.InsertPostingFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue), _filekey, args.OriginalName, _contentType, _processing, string.Empty, false, _dateTimePro.Object.Now, args.AuthorKey.TryToValueType(Guid.Empty))).Returns(true);

            var results = _worker.FileUpload(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.Result.Detail.Contains("正常に終了しました。").IsTrue();
        }

        QoManagementFileUploadWriteApiArgs GetValidArgs()
        {
            return new QoManagementFileUploadWriteApiArgs
            {
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "JotoGinowan",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.JotoGinowan}",
                LinkageSystemNo = "47900021",
                OriginalName = "hoge",
                FileData = "UEsDBBQAAAgAANp5VlsAAAAAAAAAAAAAAAAoACAA44OV44Kh44Kk44Or44Ki44OD44OX44Ot44O844OJ44OG44K544OIL3V4CwABBAAAAAAEAAAAAFVUDQAHXHb4aEp5+GgjdvhoUEsDBBQACAgIAF17VlsAAAAAAAAAAAAAAAAxACAA44OV44Kh44Kk44Or44Ki44OD44OX44Ot44O844OJ44OG44K544OIL3Rlc3QxLmNzdnV4CwABBAAAAAAEAAAAAFVUDQAHMnn4aEF5+GhAdvhoK0ktLtEpARIAUEsHCNzOdGMJAAAACQAAAFBLAwQUAAgICABje1ZbAAAAAAAAAAAAAAAAMQAgAOODleOCoeOCpOODq+OCouODg+ODl+ODreODvOODieODhuOCueODiC90ZXN0Mi5jc3Z1eAsAAQQAAAAABAAAAABVVA0ABzp5+GhBefhoVXb4aCtJLS7RKQESRgBQSwcIPFTTlQoAAAAKAAAAUEsBAhQDFAAACAAA2nlWWwAAAAAAAAAAAAAAACgAGAAAAAAAAAAAAP9BAAAAAOODleOCoeOCpOODq+OCouODg+ODl+ODreODvOODieODhuOCueODiC91eAsAAQQAAAAABAAAAABVVAUAAVx2+GhQSwECFAMUAAgICABde1Zb3M50YwkAAAAJAAAAMQAYAAAAAAAAAAAAtoFmAAAA44OV44Kh44Kk44Or44Ki44OD44OX44Ot44O844OJ44OG44K544OIL3Rlc3QxLmNzdnV4CwABBAAAAAAEAAAAAFVUBQABMnn4aFBLAQIUAxQACAgIAGN7Vls8VNOVCgAAAAoAAAAxABgAAAAAAAAAAAC2ge4AAADjg5XjgqHjgqTjg6vjgqLjg4Pjg5fjg63jg7zjg4njg4bjgrnjg4gvdGVzdDIuY3N2dXgLAAEEAAAAAAQAAAAAVVQFAAE6efhoUEsFBgAAAAADAAMAXAEAAHcBAAAAAA=="
            };
        }
    }
}
