using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class NotificationPersonalReadWorkerFixture
    {
        Mock<IFamilyRepository> _familyRepository;
        Mock<INoticePersonalRepository> _noticePersonalRepository;
        NotificationPersonalReadWorker _worker;

        Guid _actorKey = Guid.NewGuid();
        Guid _familyAccountKey = Guid.NewGuid();
        Guid _targetAccountKey = Guid.NewGuid();
        Guid _facilityKey = Guid.NewGuid();
        DateTime _fromDate = new DateTime(2024, 1, 1, 0, 0, 0);
        DateTime _toDate = new DateTime(2024, 12, 31, 23, 59, 59);
        Action<List<Guid>, List<byte>, bool, byte> _readListCallback;

        [TestInitialize]
        public void Initialize()
        {
            _familyRepository = new Mock<IFamilyRepository>();
            _noticePersonalRepository = new Mock<INoticePersonalRepository>();
            _worker = new NotificationPersonalReadWorker(_noticePersonalRepository.Object, _familyRepository.Object);
            _readListCallback = null;
        }

        [TestMethod]
        public void 開始日が終了日よりも大きければエラー()
        {
            var args = this.GetValidArgs();
            args.FromDate = _toDate.AddMilliseconds(1).ToApiDateString();

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains("期間指定が不正です").IsTrue();
        }

        [TestMethod]
        public void 先頭ページのrow範囲は1からpageSize()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(25, 0, 10);

            result.clampedPageIndex.Is(0);
            result.maxPageIndex.Is(2);
            result.rowStart.Is(1);
            result.rowEnd.Is(10);
        }

        [TestMethod]
        public void ページ2のrow範囲は11から20()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(25, 1, 10);

            result.clampedPageIndex.Is(1);
            result.maxPageIndex.Is(2);
            result.rowStart.Is(11);
            result.rowEnd.Is(20);
        }

        [TestMethod]
        public void 最終ページのrow範囲は21から30()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(25, 2, 10);

            result.clampedPageIndex.Is(2);
            result.maxPageIndex.Is(2);
            result.rowStart.Is(21);
            result.rowEnd.Is(30);
        }

        [TestMethod]
        public void ページ番号が範囲外の場合は最終ページに丸める()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(25, 10, 10);

            result.clampedPageIndex.Is(2);
            result.maxPageIndex.Is(2);
            result.rowStart.Is(21);
            result.rowEnd.Is(30);
        }

        [TestMethod]
        public void マイナスページ番号は先頭ページに丸める()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(25, -5, 10);

            result.clampedPageIndex.Is(0);
            result.maxPageIndex.Is(2);
            result.rowStart.Is(1);
            result.rowEnd.Is(10);
        }

        [TestMethod]
        public void 総件数がpageSizeで割り切れる場合のmaxPageIndex()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(30, 0, 10);

            result.maxPageIndex.Is(2);
            result.rowStart.Is(1);
            result.rowEnd.Is(10);
        }

        [TestMethod]
        public void 件数1のみの場合はmaxPageIndexが0()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(1, 0, 10);

            result.clampedPageIndex.Is(0);
            result.maxPageIndex.Is(0);
            result.rowStart.Is(1);
            result.rowEnd.Is(10);
        }

        [TestMethod]
        public void 境界ちょうどの最終ページ番号は丸まらない()
        {
            var result = NotificationPersonalReadWorker.CalculatePageRange(30, 2, 10);

            result.clampedPageIndex.Is(2);
            result.maxPageIndex.Is(2);
            result.rowStart.Is(21);
            result.rowEnd.Is(30);
        }

        [TestMethod]
        public void FromDate未指定かつToDate指定時は旧仕様どおりNoticeDate条件を適用しない()
        {
            DateTime normalizedStartDate;
            DateTime normalizedEndDate;

            var result = NotificationPersonalReadWorker.TryNormalizeNoticeDateRange(
                DateTime.MinValue,
                new DateTime(2024, 12, 31, 12, 34, 56),
                out normalizedStartDate,
                out normalizedEndDate);

            result.IsFalse();
            normalizedStartDate.Is(DateTime.MinValue);
            normalizedEndDate.Is(DateTime.MinValue);
        }

        [TestMethod]
        public void FromDateとToDateが指定されていれば旧仕様どおり日境界へ丸める()
        {
            DateTime normalizedStartDate;
            DateTime normalizedEndDate;

            var result = NotificationPersonalReadWorker.TryNormalizeNoticeDateRange(
                new DateTime(2024, 4, 10, 12, 34, 56),
                new DateTime(2024, 4, 20, 1, 2, 3),
                out normalizedStartDate,
                out normalizedEndDate);

            result.IsTrue();
            normalizedStartDate.Is(new DateTime(2024, 4, 10, 0, 0, 0));
            normalizedEndDate.Is(DateTime.ParseExact("202404202359599999999", "yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void FromDateのみ指定時は旧仕様どおりNoticeDate条件を適用する()
        {
            DateTime normalizedStartDate;
            DateTime normalizedEndDate;

            var result = NotificationPersonalReadWorker.TryNormalizeNoticeDateRange(
                new DateTime(2024, 4, 10, 12, 34, 56),
                DateTime.MaxValue,
                out normalizedStartDate,
                out normalizedEndDate);

            result.IsTrue();
            normalizedStartDate.Is(new DateTime(2024, 4, 10, 0, 0, 0));
            normalizedEndDate.Is(DateTime.MaxValue);
        }

        [TestMethod]
        public void 単件既読判定は1件だけ存在するときだけ真になる()
        {
            NotificationPersonalReadWorker.ResolveSingleAlreadyReadFlag(0, true).IsFalse();
            NotificationPersonalReadWorker.ResolveSingleAlreadyReadFlag(2, true).IsFalse();
            NotificationPersonalReadWorker.ResolveSingleAlreadyReadFlag(1, false).IsFalse();
            NotificationPersonalReadWorker.ResolveSingleAlreadyReadFlag(1, true).IsTrue();
        }

        [TestMethod]
        public void count取得時の既読joinは未読条件ありのときだけ有効になる()
        {
            NotificationPersonalReadWorker.ShouldJoinAlreadyReadForCount(false).IsFalse();
            NotificationPersonalReadWorker.ShouldJoinAlreadyReadForCount(true).IsTrue();
        }

        [TestMethod]
        public void 一覧取得で送信者表示フラグが真なら送信者名を返す()
        {
            var args = this.GetValidArgs();
            this.SetupValidList(this.CreateRepositoryItem(bool.TrueString, "山田", "太郎"));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");
            result.PageIndex.Is("0");
            result.MaxPageIndex.Is("2");
            result.NoticeN.Count.Is(1);
            result.NoticeN[0].SenderFamilyName.Is("山田");
            result.NoticeN[0].SenderGivenName.Is("太郎");
            result.NoticeN[0].TargetAccountKeyReference.Is(_targetAccountKey.ToEncrypedReference());
        }

        [TestMethod]
        public void 一覧取得で送信者表示フラグが偽なら送信者名は空文字()
        {
            var args = this.GetValidArgs();
            this.SetupValidList(this.CreateRepositoryItem(bool.FalseString, "山田", "太郎"));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
            result.NoticeN[0].SenderFamilyName.Is(string.Empty);
            result.NoticeN[0].SenderGivenName.Is(string.Empty);
        }

        [TestMethod]
        public void 単件取得でも共通変換で送信者名を返す()
        {
            var args = this.GetValidArgs();
            args.NoticeNo = "10";
            _noticePersonalRepository.Setup(m => m.ReadById(10)).Returns(this.CreateRepositoryItem(bool.TrueString, "佐藤", "花子"));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
            result.PageIndex.Is("0");
            result.MaxPageIndex.Is("0");
            result.NoticeN.Count.Is(1);
            result.NoticeN[0].SenderFamilyName.Is("佐藤");
            result.NoticeN[0].SenderGivenName.Is("花子");
        }

        [TestMethod]
        public void 単件取得でReadFlagがALREADYREADFLAGを反映する()
        {
            var args = this.GetValidArgs();
            args.NoticeNo = "10";
            _noticePersonalRepository.Setup(m => m.ReadById(10)).Returns(this.CreateRepositoryItem(bool.TrueString, "佐藤", "花子"));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
            result.NoticeN.Count.Is(1);
            result.NoticeN[0].ReadFlag.Is(bool.TrueString);
        }

        [TestMethod]
        public void 送信者取得に失敗した場合は送信者名を空文字にする()
        {
            var args = this.GetValidArgs();
            this.SetupValidList(this.CreateRepositoryItem(bool.TrueString, string.Empty, string.Empty));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
            result.NoticeN[0].SenderFamilyName.Is(string.Empty);
            result.NoticeN[0].SenderGivenName.Is(string.Empty);
        }

        [TestMethod]
        public void 未読のみとカテゴリ指定を引き継いで一覧取得する()
        {
            var args = this.GetValidArgs();
            args.OnlyUnread = bool.TrueString;
            args.CategoryNo = "2";

            _readListCallback = (accountKeys, systemTypes, onlyUnread, categoryNo) =>
            {
                accountKeys.Count.Is(2);
                accountKeys.Contains(_actorKey).IsTrue();
                accountKeys.Contains(_familyAccountKey).IsTrue();
                systemTypes.Count.Is(2);
                systemTypes[0].Is((byte)QsApiSystemTypeEnum.TisiOSApp);
                systemTypes[1].Is((byte)QsApiSystemTypeEnum.QolmsTisApp);
                onlyUnread.IsTrue();
                categoryNo.Is((byte)2);
            };

            this.SetupValidList(this.CreateRepositoryItem(bool.TrueString, "山田", "太郎"));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
        }

        [TestMethod]
        public void 取得処理で例外が起きるとエラー()
        {
            var args = this.GetValidArgs();
            _familyRepository.Setup(m => m.ReadFamilyList(_actorKey)).Throws(new Exception());

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("0500");
            result.Result.Detail.Contains("個人あてメッセージリストの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void FromDate未指定の場合はDateTime_MinValueがRepositoryに渡る()
        {
            var args = new QoNotificationPersonalReadApiArgs
            {
                ActorKey = _actorKey.ToApiGuidString(),
                ExecuteSystemType = ((int)QsApiSystemTypeEnum.TisiOSApp).ToString(),
                PageIndex = "0",
                PageSize = "10"
            };

            _familyRepository.Setup(m => m.ReadFamilyList(_actorKey)).Returns(new List<QH_ACCOUNTINDEX_DAT>());

            DateTime capturedFromDate = default(DateTime);
            _noticePersonalRepository
                .Setup(m => m.ReadCount(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<bool>(),
                    It.IsAny<byte>()))
                .Returns(0)
                .Callback<List<Guid>, List<byte>, DateTime, DateTime, bool, byte>((accountKeys, systemTypes, fromDate, toDate, onlyUnread, categoryNo) =>
                {
                    capturedFromDate = fromDate;
                });

            _noticePersonalRepository
                .Setup(m => m.ReadList(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<byte>()))
                .Returns(new List<QH_NOTICEPERSONAL_READ_VIEW>());

            _worker.Read(args);

            capturedFromDate.Is(DateTime.MinValue);
        }

        [TestMethod]
        public void ToDate未指定の場合はDateTime_MaxValueがRepositoryに渡る()
        {
            var args = new QoNotificationPersonalReadApiArgs
            {
                ActorKey = _actorKey.ToApiGuidString(),
                ExecuteSystemType = ((int)QsApiSystemTypeEnum.TisiOSApp).ToString(),
                PageIndex = "0",
                PageSize = "10"
            };

            _familyRepository.Setup(m => m.ReadFamilyList(_actorKey)).Returns(new List<QH_ACCOUNTINDEX_DAT>());

            DateTime capturedToDate = default(DateTime);
            _noticePersonalRepository
                .Setup(m => m.ReadCount(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<bool>(),
                    It.IsAny<byte>()))
                .Returns(0)
                .Callback<List<Guid>, List<byte>, DateTime, DateTime, bool, byte>((accountKeys, systemTypes, fromDate, toDate, onlyUnread, categoryNo) =>
                {
                    capturedToDate = toDate;
                });

            _noticePersonalRepository
                .Setup(m => m.ReadList(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<byte>()))
                .Returns(new List<QH_NOTICEPERSONAL_READ_VIEW>());

            _worker.Read(args);

            capturedToDate.Is(DateTime.MaxValue);
        }

        [TestMethod]
        public void 範囲外pageIndexのときworkerで算出したpageIndexとmaxPageIndexを返す()
        {
            var args = this.GetValidArgs();
            args.PageIndex = "99";

            _familyRepository.Setup(m => m.ReadFamilyList(_actorKey)).Returns(new List<QH_ACCOUNTINDEX_DAT>());
            _noticePersonalRepository.Setup(m => m.ReadCount(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    _fromDate.Date,
                    DateTime.ParseExact("202412312359599999999", "yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture),
                    It.IsAny<bool>(),
                    It.IsAny<byte>()))
                .Returns(25);
            _noticePersonalRepository.Setup(m => m.ReadList(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    _fromDate.Date,
                    DateTime.ParseExact("202412312359599999999", "yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture),
                    It.IsAny<bool>(),
                    21,
                    30,
                    It.IsAny<byte>()))
                .Returns(new List<QH_NOTICEPERSONAL_READ_VIEW> { this.CreateRepositoryItem(bool.TrueString, "山田", "太郎") });

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.TrueString);
            result.PageIndex.Is("2");
            result.MaxPageIndex.Is("2");
        }

        QH_NOTICEPERSONAL_READ_VIEW CreateRepositoryItem(string senderDispFlag, string familyName, string givenName)
        {
            return new QH_NOTICEPERSONAL_READ_VIEW
            {
                NOTICENO = 10,
                TITLE = "Title",
                CONTENTS = "Contents",
                CATEGORYNO = 2,
                PRIORITYNO = 1,
                ACCOUNTKEY = _targetAccountKey,
                FACILITYKEY = _facilityKey,
                STARTDATE = new DateTime(2024, 4, 1, 12, 0, 0),
                ENDDATE = new DateTime(2024, 4, 30, 23, 59, 59),
                NOTICEDATASET = new QsJsonSerializer().Serialize(new QhNoticeDataSetOfJson
                {
                    SenderDispFlag = senderDispFlag
                }),
                ALREADYREADFLAG = true,
                SENDERFAMILYNAME = familyName,
                SENDERGIVENNAME = givenName
            };
        }

        QoNotificationPersonalReadApiArgs GetValidArgs()
        {
            return new QoNotificationPersonalReadApiArgs
            {
                ActorKey = _actorKey.ToApiGuidString(),
                ExecuteSystemType = ((int)QsApiSystemTypeEnum.TisiOSApp).ToString(),
                FromDate = _fromDate.ToApiDateString(),
                ToDate = _toDate.ToApiDateString(),
                PageIndex = "0",
                PageSize = "10"
            };
        }

        void SetupValidList(QH_NOTICEPERSONAL_READ_VIEW item)
        {
            _familyRepository.Setup(m => m.ReadFamilyList(_actorKey)).Returns(new List<QH_ACCOUNTINDEX_DAT>
            {
                new QH_ACCOUNTINDEX_DAT
                {
                    ACCOUNTKEY = _familyAccountKey
                }
            });

            _noticePersonalRepository.Setup(m => m.ReadCount(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    _fromDate.Date,
                    DateTime.ParseExact("202412312359599999999", "yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture),
                    It.IsAny<bool>(),
                    It.IsAny<byte>()))
                .Returns(25);

            _noticePersonalRepository.Setup(m => m.ReadList(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<byte>>(),
                    _fromDate.Date,
                    DateTime.ParseExact("202412312359599999999", "yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture),
                    It.IsAny<bool>(),
                    1,
                    10,
                    It.IsAny<byte>()))
                .Returns(new List<QH_NOTICEPERSONAL_READ_VIEW> { item })
                .Callback<List<Guid>, List<byte>, DateTime, DateTime, bool, int, int, byte>((accountKeys, systemTypes, fromDate, toDate, onlyUnread, rowStart, rowEnd, categoryNo) =>
                {
                    _readListCallback?.Invoke(accountKeys, systemTypes, onlyUnread, categoryNo);
                });
        }
    }
}