using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Providers;
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
    public class PushWorkerFixture
    {
        Mock<ILinkageRepository> _repo;
        Mock<IReproRepository> _reproRepo;
        Mock<INoticeGroupRepository> _noticeRepo;
        Mock<IQoPushNotification> _pushNotification;
        Mock<IDateTimeProvider> _dateTimePro;
        PushWorker _worker;

        string _contentType = "application/octet-stream";
        //Guid _filekey = Guid.NewGuid();
        Guid _accountkey = Guid.NewGuid();
        byte _processing = 1;

        [TestInitialize]
        public void Initialize()
        {
            _dateTimePro = new Mock<IDateTimeProvider>();
            _repo = new Mock<ILinkageRepository>();
            _reproRepo = new Mock<IReproRepository>();
            _pushNotification = new Mock<IQoPushNotification>();
            _noticeRepo = new Mock<INoticeGroupRepository>();
            _worker = new PushWorker(_repo.Object, _reproRepo.Object, _noticeRepo.Object, _pushNotification.Object, _dateTimePro.Object);
        }

        [TestMethod]
        public async Task 引数が不正の場合はエラー()
        {
            var args = GetValidArgs();
            args.LinkageIds = new List<string>();
            var results = _worker.Send(args);

            // 失敗1002引数エラー
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            string.IsNullOrWhiteSpace(results.Result.Detail).IsFalse();//暫定なんかメッセージ入ってればOKにする
        }


        //[TestMethod]
        //public async Task ファイル名でエラーだった場合はエラーとする()
        //{
        //    var args = GetValidArgs();
        //    args.OriginalName = "123456789012345678901234567890123456789012345678901";//51文字

        //    var results = _worker.FileUpload(args);

        //    // 失敗1002引数エラー
        //    results.IsSuccess.Is(bool.FalseString);
        //    results.Result.Code.Is("1002");
        //    results.Result.Detail.Contains("ファイル形式が不正です。").IsTrue();
        //}


        //[TestMethod]
        //public async Task ブロブに登録失敗エラーだった場合はエラーとする()
        //{
        //    var args = GetValidArgs();

        //    _repo.Setup(m => m.UproadPostingFileBlob(_contentType, Convert.FromBase64String(args.FileData), string.Empty, args.OriginalName, args.LinkageSystemNo.TryToValueType(int.MinValue), args.AuthorKey.TryToValueType(Guid.Empty))).Returns(Guid.Empty);
        //    //_repo.Setup(m => m.UproadFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue), long.MinValue, byte.MinValue, string.Empty, DateTime.Now)).Returns(true);//要確認

        //    var results = _worker.FileUpload(args);

        //    // 失敗1003引数エラー
        //    results.IsSuccess.Is(bool.FalseString);
        //    results.Result.Code.Is("1003");
        //    results.Result.Detail.Contains("登録に失敗しました。").IsTrue();
        //}

        //[TestMethod]
        //public async Task DBに登録失敗エラーだった場合はエラーとする()
        //{
        //    var args = GetValidArgs();

        //    var now = DateTime.Now;
        //    _repo.Setup(m => m.UproadPostingFileBlob(_contentType, Convert.FromBase64String(args.FileData), string.Empty, args.OriginalName, args.LinkageSystemNo.TryToValueType(int.MinValue), args.AuthorKey.TryToValueType(Guid.Empty))).Returns(_filekey);
        //    _repo.Setup(m => m.InsertPostingFileDb(args.LinkageSystemNo.TryToValueType(int.MinValue), _filekey, args.OriginalName, _contentType, _processing, string.Empty, false, now, args.AuthorKey.TryToValueType(Guid.Empty))).Returns(false);
        //    _repo.Setup(m => m.DeletePostingFileBlob(_filekey)).Returns(true);

        //    var results = _worker.FileUpload(args);

        //    // 失敗1003引数エラー
        //    results.IsSuccess.Is(bool.FalseString);
        //    results.Result.Code.Is("1003");
        //    results.Result.Detail.Contains("SQL実行エラー 登録に失敗しました。").IsTrue();
        //}

        [TestMethod]
        public async Task 正常に登録できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var results = _worker.Send(args);
            
            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.Result.Detail.Contains("正常に終了しました。").IsTrue();
        }

        void SetupValidMethods(QoPushSendApiArgs args)
        {
            _dateTimePro.Setup(m => m.Now).Returns(DateTime.Now);
            _repo.Setup(m => m.ReadAccountKeys(args.LinkageSystemNo.TryToValueType(int.MinValue), args.LinkageIds)).Returns(new List<Guid>() { _accountkey });

            //リプロPush
            _reproRepo.Setup(m => m.ReproPushApiCall(args.Contents, new List<Guid>() { _accountkey }, args.StartDate.TryToValueType(DateTime.MinValue), args.DeeplinkUrl)).Returns(true);
            _reproRepo.Setup(m => m.InsertPushSend(new List<Guid>() { _accountkey }, _dateTimePro.Object.Now)).Returns(true);

            //ネイティブPush
            var noticeno = 1;
            _noticeRepo.Setup(m => m.InsertNoticeGroupData(It.IsAny<QH_NOTICEGROUP_DAT>())).Returns(noticeno);

            var request = new NotificationRequest
            {
                Extra = "",
                Silent = false,
                Text = args.Contents,
                Url = args.DeeplinkUrl,
                Badge = 1,
                Title = args.Title,
                ScheduleDate = args.StartDate.TryToValueType(DateTime.MinValue)
            };

            _pushNotification.Setup(m => m.RequestNotificationAsync(It.IsAny<NotificationRequest>())).ReturnsAsync(new string[] { "1", "2" });
            List<string[]> pushresults = new List<string[]>() { new string[] { "1", "2" } };
            var list = (_accountkey, new string[] { "1", "2" });
            _noticeRepo.Setup(m => m.UpdateNoticeGroupData(noticeno, true, null, It.IsAny<List<string[]>>())).Returns(true);
            _noticeRepo.Setup(m => m.WriteGroupTarget(noticeno, It.IsAny< List<(Guid,string[])>>(), args.StartDate.TryToValueType(DateTime.MinValue), _dateTimePro.Object.Now)).Returns(true); ;

        }

        QoPushSendApiArgs GetValidArgs()
        {
            return new QoPushSendApiArgs
            {
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "JotoGinowan",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.JotoGinowan}",
                LinkageSystemNo = "47900021",
                LinkageIds = new List<string>(){
                    "kuQmpH94",
                    "Gx8SB7Gi",
                    "abcdefg",
                    "hVSt97Ch"
                },
                Title = "テストPushAPI",
                Contents= "てすと本文です。",
                PriorityNo = "3",
                CategoryNo = "1",
                StartDate = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd"),
                EndDate = DateTime.Now.AddMonths(1).ToString("yyyy/MM/dd"),
                DeeplinkUrl = "https://devjoto.qolms.com/start/LoginById",
            };
        }

    }
}
