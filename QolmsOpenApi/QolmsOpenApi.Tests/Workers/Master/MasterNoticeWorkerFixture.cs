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

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class MasterNoticeWorkerFixture
    {
        Mock<INoticeRepository> _repo;
        MasterNoticeWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<INoticeRepository>();
            _worker = new MasterNoticeWorker(_repo.Object);
        }

        [TestMethod]
        public void お知らせリスト取得でDBエラー時は失敗とする()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var from = new DateTime(2022, 11, 1);
            var to = new DateTime(2022, 11, 30);

            var args = new QoMasterNoticeReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FromDate = from.ToApiDateString(),
                ToDate = to.ToApiDateString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsiOSApp.ToString(),
                PageIndex = "0",
                PageSize = "5"
            };


            // DB処理で例外を発生させる
            _repo.Setup(m => m.ReadList(accountKey, QsApiSystemTypeEnum.QolmsiOSApp, from, to, 0, 5)).Throws(new Exception());


            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
        }

        [TestMethod]
        public void お知らせリスト取得でその他のエラー発生時は失敗とする()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var from = new DateTime(2022, 11, 1);
            var to = new DateTime(2022, 11, 30);

            var args = new QoMasterNoticeReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FromDate = from.ToApiDateString(),
                ToDate = to.ToApiDateString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsiOSApp.ToString(),
                PageIndex = "0",
                PageSize = "5"
            };


            // null参照例外が出るように設定
            _repo.Setup(m => m.ReadList(accountKey, QsApiSystemTypeEnum.QolmsiOSApp, from, to, 0, 5)).Returns(new DbNoticeListReaderResults
            {
                NoticeListN = null
            });


            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1005"); // 実行エラー
        }

        [TestMethod]
        public void お知らせリスト取得で正常に取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var from = new DateTime(2022, 11, 1);
            var to = new DateTime(2022, 11, 30);

            var args = new QoMasterNoticeReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                FromDate = from.ToApiDateString(),
                ToDate = to.ToApiDateString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsiOSApp.ToString(),
                PageIndex = "0",
                PageSize = "5"
            };


            // 正常な値を返すように設定
            _repo.Setup(m => m.ReadList(accountKey, QsApiSystemTypeEnum.QolmsiOSApp, from, to, 0, 5)).Returns(new DbNoticeListReaderResults
            {
                PageIndex = 0,
                MaxPageIndex = 5,
                NoticeListN = new List<QH_NOTICE_DAT>
                {
                    new QH_NOTICE_DAT
                    {
                        NOTICENO = 10,
                        CATEGORYNO = 1,
                        PRIORITYNO = 2,
                        TITLE = "タイトル",
                        CONTENTS = "コンテンツ",
                        STARTDATE = new DateTime(2022,11,1),
                        ENDDATE = new DateTime(2023,12,31)
                    },
                    new QH_NOTICE_DAT
                    {
                        NOTICENO = 11,
                        CATEGORYNO = 2,
                        PRIORITYNO = 2,
                        TITLE = "タイトル2",
                        CONTENTS = "コンテンツ2",
                        STARTDATE = new DateTime(2022,11,5),
                        ENDDATE = new DateTime(2023,12,31)
                    },
                }
            });


            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // 2件取得できた
            ret.NoticeN.Count.Is(2);
            // 1件目のデータが正常に変換できている
            ret.NoticeN[0].NoticeNo.Is("10");
            ret.NoticeN[0].CategoryNo.Is("1");
            ret.NoticeN[0].PriorityNo.Is("2");
            ret.NoticeN[0].Title.Is("タイトル");
            ret.NoticeN[0].Contents.Is("コンテンツ");
            ret.NoticeN[0].StartDate.Is(new DateTime(2022, 11, 1).ToApiDateString());
            ret.NoticeN[0].EndDate.Is(new DateTime(2023, 12, 31).ToApiDateString());
            // 2件目のデータも正常に変換できている
            ret.NoticeN[1].NoticeNo.Is("11");
            ret.NoticeN[1].CategoryNo.Is("2");
            ret.NoticeN[1].Title.Is("タイトル2");
            ret.NoticeN[1].Contents.Is("コンテンツ2");
            ret.NoticeN[1].StartDate.Is(new DateTime(2022, 11, 5).ToApiDateString());

            // アカウントキーが使われた
            _repo.Verify(m => m.ReadList(accountKey, QsApiSystemTypeEnum.QolmsiOSApp, from, to, 0, 5), Times.Once);
        }

        [TestMethod]
        public void お知らせリスト取得でExecutorを使って正常に取得できる()
        {
            var accountKey = Guid.Empty;
            var executor = Guid.NewGuid();
            var from = new DateTime(2022, 11, 1);
            var to = new DateTime(2022, 11, 30);

            var args = new QoMasterNoticeReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                Executor = executor.ToApiGuidString(),
                FromDate = from.ToApiDateString(),
                ToDate = to.ToApiDateString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsiOSApp.ToString(),
                PageIndex = "0",
                PageSize = "5"
            };

            _repo.Setup(m => m.ReadList(executor, QsApiSystemTypeEnum.QolmsiOSApp, from, to, 0, 5)).Returns(new DbNoticeListReaderResults
            {
                PageIndex = 0,
                MaxPageIndex = 5,
                NoticeListN = new List<QH_NOTICE_DAT>
                {
                    new QH_NOTICE_DAT
                    {
                        NOTICENO = 10,
                        CATEGORYNO = 1,
                        PRIORITYNO = 2,
                        TITLE = "タイトル",
                        CONTENTS = "コンテンツ",
                        STARTDATE = new DateTime(2022,11,1),
                        ENDDATE = new DateTime(2023,12,31)
                    }
                }
            });


            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // 1件取得できた
            ret.NoticeN.Count.Is(1);
            // 1件目のデータが正常に変換できている
            ret.NoticeN[0].NoticeNo.Is("10");
            ret.NoticeN[0].CategoryNo.Is("1");
            ret.NoticeN[0].PriorityNo.Is("2");
            ret.NoticeN[0].Title.Is("タイトル");
            ret.NoticeN[0].Contents.Is("コンテンツ");
            ret.NoticeN[0].StartDate.Is(new DateTime(2022, 11, 1).ToApiDateString());
            ret.NoticeN[0].EndDate.Is(new DateTime(2023, 12, 31).ToApiDateString());
            

            // Executorが使われた
            _repo.Verify(m => m.ReadList(executor, QsApiSystemTypeEnum.QolmsiOSApp, from, to, 0, 5), Times.Once);
        }
    }
}
