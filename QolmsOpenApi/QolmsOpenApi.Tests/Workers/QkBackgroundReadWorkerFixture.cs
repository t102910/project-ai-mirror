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
    public class QkBackgroundReadWorkerFixture
    {
        QkBackgroundReadWorker _worker;
        Mock<IQkBackgroundRepository> _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IQkBackgroundRepository>();
            _worker = new QkBackgroundReadWorker(_repo.Object);
        }

        [TestMethod]
        public void リスト取得_アカウントキーが不正で失敗が返る()
        {
            var args = new QoBackgroundListReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString()
            };

            var ret = _worker.ReadList(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("アカウントキーが不正です。").IsTrue();
        }

        [TestMethod]
        public void リスト取得_例外が発生するとエラーが返る()
        {
            var args = new QoBackgroundListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
            };

            _repo.Setup(m => m.ReadList()).Throws(new Exception());


            var results = _worker.ReadList(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void リスト取得_データ変換して正常に返却される()
        {
            var args = new QoBackgroundListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
            };

            var targetEntities = new List<QK_BACKGROUND_MST>
            {
                new QK_BACKGROUND_MST
                {
                    BACKGROUNDID = "hoge1",
                    DISPLAYNAME = "hogefuga",
                    DESCRIPTION = "hogedesc",
                    FILEKEY = Guid.NewGuid(),
                    SORTORDER = 0,
                    UPDATEDDATE = new DateTime(2023,4,4,16,15,10)
                },
                new QK_BACKGROUND_MST
                {
                    BACKGROUNDID = "hoge2",
                    DISPLAYNAME = "hoge2fuga",
                    DESCRIPTION = "hoge2desc",
                    FILEKEY = Guid.NewGuid(),
                    SORTORDER = 1,
                    UPDATEDDATE = new DateTime(2023,4,4,15,20,15)
                }
            };
            _repo.Setup(m => m.ReadList()).Returns(targetEntities);

            var results = _worker.ReadList(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            var item1 = results.BackgroundItems[0];
            item1.BackgroundId.Is(targetEntities[0].BACKGROUNDID);
            item1.DisplayName.Is(targetEntities[0].DISPLAYNAME);
            item1.Description.Is(targetEntities[0].DESCRIPTION);
            item1.FileKey.Is(targetEntities[0].FILEKEY.ToEncrypedReference());
            item1.SortOrder.Is(targetEntities[0].SORTORDER);
            item1.UpdatedDate.Is(targetEntities[0].UPDATEDDATE.ToApiDateString());

            var item2 = results.BackgroundItems[1];
            item2.BackgroundId.Is(targetEntities[1].BACKGROUNDID);
            item2.DisplayName.Is(targetEntities[1].DISPLAYNAME);
            item2.Description.Is(targetEntities[1].DESCRIPTION);
            item2.FileKey.Is(targetEntities[1].FILEKEY.ToEncrypedReference());
            item2.SortOrder.Is(targetEntities[1].SORTORDER);
            item2.UpdatedDate.Is(targetEntities[1].UPDATEDDATE.ToApiDateString());
        }

        [TestMethod]
        public void Read_アカウントキーが不正でエラーが返る()
        {
            var args = new QoBackgroundReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString()
            };

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("アカウントキーが不正です。").IsTrue();
        }

        [TestMethod]
        public void Read_例外発生でエラーが返る()
        {
            var args = new QoBackgroundReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Throws(new Exception());

            var ret = _worker.Read(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("0500");
        }

        [TestMethod]
        public void Read_該当データ無しで正常扱い()
        {
            var args = new QoBackgroundReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Returns(default(QK_BACKGROUND_MST));

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.BackgroundItem.IsNull();
            ret.IsDeleted.IsFalse();
        }

        [TestMethod]
        public void Read_削除済みデータも正常扱いで返る()
        {
            var args = new QoBackgroundReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            var entity = new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = Guid.NewGuid(),
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2023, 4, 4, 16, 15, 10),
                DELETEFLAG = true
            };

            _repo.Setup(m => m.Read("hoge")).Returns(entity);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.BackgroundItem.IsNotNull();
            ret.IsDeleted.IsTrue(); // 削除済み判定

            // 正常にコンバートされている
            var item = ret.BackgroundItem;
            item.BackgroundId.Is(entity.BACKGROUNDID);
            item.DisplayName.Is(entity.DISPLAYNAME);
            item.Description.Is(entity.DESCRIPTION);
            item.FileKey.Is(entity.FILEKEY.ToEncrypedReference());
            item.SortOrder.Is(entity.SORTORDER);
            item.UpdatedDate.Is(entity.UPDATEDDATE.ToApiDateString());
        }

        [TestMethod]
        public void Read_正常パターン()
        {
            var args = new QoBackgroundReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            var entity = new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = Guid.NewGuid(),
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2023, 4, 4, 16, 15, 10),
                DELETEFLAG = false
            };

            _repo.Setup(m => m.Read("hoge")).Returns(entity);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.BackgroundItem.IsNotNull();
            ret.IsDeleted.IsFalse();

            // 正常にコンバートされている
            var item = ret.BackgroundItem;
            item.BackgroundId.Is(entity.BACKGROUNDID);
            item.DisplayName.Is(entity.DISPLAYNAME);
            item.Description.Is(entity.DESCRIPTION);
            item.FileKey.Is(entity.FILEKEY.ToEncrypedReference());
            item.SortOrder.Is(entity.SORTORDER);
            item.UpdatedDate.Is(entity.UPDATEDDATE.ToApiDateString());
        }

        [TestMethod]
        public void ReadFile_アカウントキーが不正でエラーが返る()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString()
            };

            var ret = _worker.ReadFile(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("アカウントキーが不正です。").IsTrue();
        }

        [TestMethod]
        public void ReadFile_例外発生でエラーが返る()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            // Readで例外を発生させる
            _repo.Setup(m => m.Read("hoge")).Throws(new Exception());

            var ret = _worker.ReadFile(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("0500");
        }

        [TestMethod]
        public void ReadFile_モデルが存在しない場合はエラー()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Returns(default(QK_BACKGROUND_MST));

            var ret = _worker.ReadFile(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1006"); // not found
        }

        [TestMethod]
        public void ReadFile_モデルが削除されていてもエラー()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "hoge",
                DELETEFLAG = true
            });

            var ret = _worker.ReadFile(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1006"); // not found
        }

        [TestMethod]
        public void ReadFile_更新されていなかったらフラグを立てて正常終了()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge",
                LastUpdatedDate = new DateTime(2023, 4, 4, 21, 38, 15).ToApiDateString()
            };

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "hoge",
                UPDATEDDATE = new DateTime(2023, 4, 4, 21, 38, 15) // 同時刻
            });

            var ret = _worker.ReadFile(args);

            // 正常終了
            ret.IsSuccess.Is(bool.TrueString);
            ret.UpdatedDate.Is(new DateTime(2023, 4, 4, 21, 38, 15).ToApiDateString());
            ret.IsDataNoChanged.IsTrue();
            ret.Data.IsNull();
        }

        [TestMethod]
        public void ReadFile_更新されていた場合は正常としてデータを返す()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge",
                LastUpdatedDate = new DateTime(2023, 4, 4, 21, 38, 15).ToApiDateString()
            };

            var fileKey = Guid.NewGuid();

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = fileKey,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2023, 4, 4, 21, 38, 16) // 1秒新しい
            });

            _repo.Setup(m => m.ReadFile(fileKey)).Returns("data");

            var ret = _worker.ReadFile(args);

            // 正常終了
            ret.IsSuccess.Is(bool.TrueString);
            ret.IsDataNoChanged.IsFalse();
            ret.UpdatedDate.Is(new DateTime(2023, 4, 4, 21, 38, 16).ToApiDateString());
            ret.Data.Is("data");
        }

        [TestMethod]
        public void ReadFile_LastUpdatedDataが未指定の場合は確実にデータを返す()
        {
            var args = new QoBackgroundFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                BackgroundId = "hoge",
            };

            var fileKey = Guid.NewGuid();

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_BACKGROUND_MST
            {
                BACKGROUNDID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = fileKey,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(1999, 1, 1, 1, 1, 0) // 相当古い
            });

            _repo.Setup(m => m.ReadFile(fileKey)).Returns("data");

            var ret = _worker.ReadFile(args);

            // 正常終了
            ret.IsSuccess.Is(bool.TrueString);
            // LastUpdateDateがデフォルトなので常にデータを返す
            ret.IsDataNoChanged.IsFalse();
            ret.UpdatedDate.Is(new DateTime(1999, 1, 1, 1, 1, 0).ToApiDateString());
            ret.Data.Is("data");
        }
    }
}
