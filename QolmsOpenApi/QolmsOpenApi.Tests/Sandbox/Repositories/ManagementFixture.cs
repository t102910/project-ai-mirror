using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class ManagementFixture
    {
        IManagementRepository _repo;
        string _contentType = "application/octet-stream";
        byte _processing = 1;

        /// <summary>
        /// 登録時のfailekeyを取得して指定してください。（通しテストできません）
        /// </summary>
        Guid _failekey; //= Guid.Parse("0386e9e1-818f-41b9-beb9-25f529357e11");

        [TestInitialize]
        public void Initialize()
        {
            _repo = new ManagementRepository();
        }

        [TestMethod]
        public void blobにファイルを登録できる()
        {
            var args = GetValidArgs();
            var ret = _repo.UproadPostingFileBlob(_contentType,Convert.FromBase64String(args.FileData),string.Empty,args.OriginalName,args.LinkageSystemNo.TryToValueType(int.MinValue),args.AuthorKey.TryToValueType(Guid.Empty));

            _failekey = ret;
            (ret != Guid.Empty).IsTrue();
        }

        [TestMethod]
        public void qm_checkuppostingfile_datにレコードを追加()
        {
            var args = GetValidArgs();
            var ret = _repo.InsertPostingFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue), _failekey, args.OriginalName, _contentType, _processing, string.Empty, false, DateTime.Now, args.AuthorKey.TryToValueType(Guid.Empty));

            (ret).IsTrue();
        }

        [TestMethod]
        public void blobのファイルを削除できる()
        {
            var ret = _repo.DeletePostingFileBlob(_failekey);

            (ret).IsTrue();
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
