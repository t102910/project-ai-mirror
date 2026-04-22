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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class NotificationReadWorkerFixture
    {
        Mock<INoticeRepository> _noticeRepo;
        Mock<ILinkageRepository> _linkageRepo;

        NotificationReadWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _facilityKey = Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB");
        Guid _parentFacilityKey = Guid.NewGuid();
        Guid _fileKey1 = Guid.NewGuid();
        Guid _fileKey2 = Guid.NewGuid();
        DateTime _fromDate = new DateTime(2023, 7, 20, 12, 30, 0);
        DateTime _toDate = new DateTime(2024, 6, 30, 23, 59, 59);
        Action<List<Guid>> _callBackFacility;
        Action<List<byte>> _callBackSystemType;
        Action<List<byte>> _callBackCategory;

        [TestInitialize]
        public void Initialize()
        {
            _noticeRepo = new Mock<INoticeRepository>();
            _linkageRepo = new Mock<ILinkageRepository>();
            _worker = new NotificationReadWorker(_noticeRepo.Object, _linkageRepo.Object);
            _callBackFacility = null;
            _callBackSystemType = null;
            _callBackCategory = null;
        }

        [TestMethod]
        public void 開始日が終了日よりも大きければエラー()
        {
            var args = GetValidArgs();
            // 1ミリ秒だけ大きい
            args.FromDate = _toDate.AddMilliseconds(1).ToApiDateString();

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("期間指定が不正です").IsTrue();
        }

        [TestMethod]
        public void 親施設の取得に失敗するとエラー()
        {
            var args = GetValidArgs();            
            
            args.FacilityKeyReference = _facilityKey.ToEncrypedReference();
            // 親施設フラグはデフォルトでTrueとなる
            args.IncludeParentFacility = "";

            // 親施設の取得で例外を起こす
            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("親施設情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void リスト取得処理で例外が起きるとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            _noticeRepo.Setup(m => m.ReadList(_accountKey, It.IsAny<List<byte>>(), It.IsAny<List<Guid>>(), It.IsAny<List<byte>>(), _fromDate, _toDate, 0, 10)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お知らせリストの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void お知らせ単体取得処理で例外が起きるとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            args.NoticeNo = "100";

            _noticeRepo.Setup(m => m.ReadById(100)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お知らせリストの取得に失敗しました").IsTrue();
        }


        [TestMethod]
        public void フィルター無しで正常終了()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.PageIndex.Is("0");
            results.MaxPageIndex.Is("3");

            // 全2件取得
            results.NoticeN.Count.Is(2);

            var item = results.NoticeN[0];

            // 要素1が正しく変換されている
            item.CategoryNo.Is("0");
            item.Contents.Is("hoge");
            item.Title.Is("Title1");
            item.FacilityKeyReference.Is(_facilityKey.ToEncrypedReference());
            item.NoticeNo.Is("100");
            item.PriorityNo.Is("3");
            item.StartDate.Is(new DateTime(2023, 7, 1, 12, 30, 15).ToApiDateString());
            item.EndDate.Is(new DateTime(2025, 12, 31, 23, 59, 59).ToApiDateString());
            item.FileKeyN.Count.Is(2);
            item.FileKeyN[0].FileKeyReference.Is(_fileKey1.ToEncrypedReference());
            item.FileKeyN[0].Sequence.Is("0");
            item.FileKeyN[1].FileKeyReference.Is(_fileKey2.ToEncrypedReference());
            item.FileKeyN[1].Sequence.Is("1");
            item.LinkN.Count.Is(2);
            item.LinkN[0].Title.Is("Link");
            item.LinkN[0].Url.Is("https://abc.com");
            item.LinkN[1].Title.Is("Link2");
            item.LinkN[1].Url.Is("https://xyz.com");

            item = results.NoticeN[1];

            // 要素2が正しく変換されている
            item.CategoryNo.Is("1");
            item.Contents.Is("fuga");
            item.Title.Is("Title2");
            item.FacilityKeyReference.Is(_parentFacilityKey.ToEncrypedReference());
            item.NoticeNo.Is("101");
            item.PriorityNo.Is("1");
            item.StartDate.Is(new DateTime(2023, 5, 1).ToApiDateString());
            item.EndDate.Is(DateTime.MaxValue.ToApiDateString());
            item.FileKeyN.Count.Is(0);            
            item.LinkN.Count.Is(0);

            // 親施設取得処理は実行されなかった（施設キー指定がないため）
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Never);
        }

        [TestMethod]
        public void 施設を指定して正常終了()
        {
            var args = GetValidArgs();

            // 施設1指定
            args.FacilityKeyReference = _facilityKey.ToEncrypedReference();
            SetUpValidMethods(args);

            _callBackFacility = fList =>
            {
                // ReadListに渡された施設リストは2件
                fList.Count.Is(2);
                // 対象の施設キーが渡された
                fList[0].Is(_facilityKey);
                // 親の施設キーも渡された
                fList[1].Is(_parentFacilityKey);
            };

            _callBackSystemType = sList =>
            {
                // ToSystemTypeとして2件
                sList.Count.Is(2);
                // iOS
                sList[0].Is((byte)QsApiSystemTypeEnum.TisiOSApp);
                // 共通
                sList[1].Is((byte)QsApiSystemTypeEnum.QolmsTisApp);
            };             

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.PageIndex.Is("0");
            results.MaxPageIndex.Is("3");

            // 全2件取得
            results.NoticeN.Count.Is(2);

            // 親施設取得処理が実行された（親施設フラグ デフォルトTrueの証明）
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Once);
        }

        [TestMethod]
        public void 施設を指定しかつ親施設取得を設定したが親施設が存在せずに正常終了()
        {
            var args = GetValidArgs();

            // 施設1指定
            args.FacilityKeyReference = _facilityKey.ToEncrypedReference();
            SetUpValidMethods(args);

            // 親施設が存在しない
            _linkageRepo.Setup(m => m.GetParentLinkageMst(_facilityKey)).Returns(default(QH_LINKAGESYSTEM_MST));

            _callBackFacility = fList =>
            {
                // ReadListに渡された施設リストは1件
                fList.Count.Is(1);
                // 対象の施設キーが渡された
                fList[0].Is(_facilityKey);
            };

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.PageIndex.Is("0");
            results.MaxPageIndex.Is("3");

            // 全2件取得
            results.NoticeN.Count.Is(2);

            // 親施設取得処理が実行された
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Once);
        }

        [TestMethod]
        public void 施設を指定しかつ親施設を取得しない設定で正常終了()
        {
            var args = GetValidArgs();

            // 施設1指定
            args.FacilityKeyReference = _facilityKey.ToEncrypedReference();
            // 親施設を含めない
            args.IncludeParentFacility = bool.FalseString;

            SetUpValidMethods(args);            

            _callBackFacility = fList =>
            {
                // ReadListに渡された施設リストは1件
                fList.Count.Is(1);
                // 対象の施設キーが渡された
                fList[0].Is(_facilityKey);
                // (親のキーは渡されていない）
            };

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.PageIndex.Is("0");
            results.MaxPageIndex.Is("3");

            // 全2件取得
            results.NoticeN.Count.Is(2);            

            // 親施設取得処理は実行されなかった
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Never);
        }

        [TestMethod]
        public void 施設を指定しかつアプリお知らせを含む指定で正常終了()
        {
            var args = GetValidArgs();

            // 施設1指定
            args.FacilityKeyReference = _facilityKey.ToEncrypedReference();
            // 親施設を含める
            args.IncludeParentFacility = bool.TrueString;
            // アプリお知らせを含める
            args.IncludeAppNotification = bool.TrueString;

            SetUpValidMethods(args);

            _callBackFacility = fList =>
            {
                // ReadListに渡された施設リストは3件
                fList.Count.Is(3);
                // 対象の施設キーが渡された
                fList[0].Is(_facilityKey);
                // ブランク施設キーが渡された(アプリお知らせ用)
                fList[1].Is(Guid.Empty);
                // 親施設キーが渡された
                fList[2].Is(_parentFacilityKey);
            };

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.PageIndex.Is("0");
            results.MaxPageIndex.Is("3");

            // 全2件取得
            results.NoticeN.Count.Is(2);

            // 親施設取得処理が実行された
            _linkageRepo.Verify(m => m.GetParentLinkageMst(_facilityKey), Times.Once);
        }

        [TestMethod]
        public void カテゴリを指定して正常終了()
        {
            var args = GetValidArgs();

            // 医療ナビ(Android)指定
            args.ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsNaviAndroidApp}";
            // カテゴリ指定
            args.CategoryNo = new List<string> { "0","1" };

            SetUpValidMethods(args);

            _callBackSystemType = sList =>
            {
                // ToSystemTypeとして2件
                sList.Count.Is(2);
                // 医療ナビAndroid
                sList[0].Is((byte)QsApiSystemTypeEnum.QolmsNaviAndroidApp);
                // 医療ナビ共通
                sList[1].Is((byte)QsApiSystemTypeEnum.QolmsNaviApp);
            };

            _callBackCategory = cList =>
            {
                // ReadListにカテゴリが正しく渡された
                cList.Count.Is(2);
                cList[0].Is((byte)0);
                cList[1].Is((byte)1);
            };            

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 全てのフィルターを指定して正常終了()
        {
            var args = GetValidArgs();
            args.IncludeAppNotification = bool.TrueString;
            args.IncludeParentFacility = bool.TrueString;
            args.FacilityKeyReference = _facilityKey.ToEncrypedReference();
            args.CategoryNo = new List<string> { "0" };

            SetUpValidMethods(args);

            _callBackCategory = cList =>
            {
                // ReadListにカテゴリが正しく渡された
                cList.Count.Is(1);
                cList[0].Is((byte)0);
            };

            _callBackSystemType = sList =>
            {
                // ToSystemTypeとして2件
                sList.Count.Is(2);
                // iOS
                sList[0].Is((byte)QsApiSystemTypeEnum.TisiOSApp);
                // 共通
                sList[1].Is((byte)QsApiSystemTypeEnum.QolmsTisApp);
            };

            _callBackFacility = fList =>
            {
                // ReadListに渡された施設リストは3件
                fList.Count.Is(3);
                // 対象の施設キーが渡された
                fList[0].Is(_facilityKey);
                // ブランク施設キーが渡された(アプリお知らせ用)
                fList[1].Is(Guid.Empty);
                // 親施設キーが渡された
                fList[2].Is(_parentFacilityKey);
            };

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        void SetUpValidMethods(QoNotificationReadApiArgs args)
        {
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference<Guid>();
            _linkageRepo.Setup(m => m.GetParentLinkageMst(facilityKey)).Returns(new QH_LINKAGESYSTEM_MST
            {
                FACILITYKEY = _parentFacilityKey
            });

            var dataSet1 = new QsJsonSerializer().Serialize(new QhNoticeDataSetOfJson
            {
                AttachedFileN = new List<QhAttachedFileOfJson>
                {
                    new QhAttachedFileOfJson
                    {
                        FileKey = _fileKey1.ToApiGuidString(),                        
                    },
                    new QhAttachedFileOfJson
                    {
                        FileKey = _fileKey2.ToApiGuidString(),
                    }
                },
                LinkN = new List<QhNoticeLinkItemOfJson>
                {
                    new QhNoticeLinkItemOfJson
                    {
                        LinkText = "Link",
                        LinkUrl = "https://abc.com"
                    },
                    new QhNoticeLinkItemOfJson
                    {
                        LinkText = "Link2",
                        LinkUrl = "https://xyz.com"
                    }
                },                
            });
            var dataSet2 = new QsJsonSerializer().Serialize(new QhNoticeDataSetOfJson
            {
                AttachedFileN = new List<QhAttachedFileOfJson>(),
                LinkN = new List<QhNoticeLinkItemOfJson>()
            });

            var entityList = new List<QH_NOTICE_DAT>
            {
                new QH_NOTICE_DAT
                {
                    NOTICENO = 100,
                    CATEGORYNO = 0,
                    CONTENTS = "hoge",
                    FACILITYKEY = _facilityKey,
                    PRIORITYNO = 3,
                    TITLE = "Title1",
                    STARTDATE = new DateTime(2023,7,1,12,30,15),
                    ENDDATE = new DateTime(2025,12,31,23,59,59),
                    NOTICEDATASET = dataSet1,                    
                },
                new QH_NOTICE_DAT
                {
                    NOTICENO = 101,
                    CATEGORYNO = 1,
                    CONTENTS = "fuga",
                    FACILITYKEY = _parentFacilityKey,
                    PRIORITYNO = 1,
                    TITLE = "Title2",
                    STARTDATE = new DateTime(2023,5,1),
                    ENDDATE = DateTime.MaxValue,
                    NOTICEDATASET = dataSet2
                }
            };

            _noticeRepo.Setup(m => m.ReadList(_accountKey, It.IsAny<List<byte>>(), It.IsAny<List<Guid>>(), It.IsAny<List<byte>>(), _fromDate, _toDate, 0, 10)).Returns(
                (entityList, 0, 3)).Callback<Guid, List<byte>, List<Guid>, List<byte>, DateTime, DateTime,int,int>((a,b,c,d,e,f,g,h) =>
                {
                    _callBackSystemType?.Invoke(b);
                    _callBackFacility?.Invoke(c);
                    _callBackCategory?.Invoke(d);
                });
        }

        QoNotificationReadApiArgs GetValidArgs()
        {
            return new QoNotificationReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                PageIndex = "0",
                PageSize = "10",
                FromDate = _fromDate.ToApiDateString(),
                ToDate = _toDate.ToApiDateString(),                
            };
        }
    }
}
