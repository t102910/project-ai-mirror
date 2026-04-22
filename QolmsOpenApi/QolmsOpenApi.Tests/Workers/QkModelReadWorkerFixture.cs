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
    public class QkModelReadWorkerFixture
    {
        QkModelReadlWorker _worker;
        Mock<IQkModelRepository> _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IQkModelRepository>();
            _worker = new QkModelReadlWorker(_repo.Object);
        }

        [TestMethod]
        public void リスト取得_アカウントキーが不正で失敗が返る()
        {
            var args = new QoModelListReadApiArgs
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
            var args = new QoModelListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ContainsPaid = true
            };

            _repo.Setup(m => m.ReadList(true)).Throws(new Exception());

            
            var results = _worker.ReadList(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
        }

        [TestMethod]
        public void リスト取得_データ変換して正常に返却される()
        {
            var args = new QoModelListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ContainsPaid = true
            };

            var targetEntities = new List<QK_MODEL_MST>
            {
                new QK_MODEL_MST
                {
                    MODELID = "hoge1",
                    DISPLAYNAME = "hogefuga",
                    DESCRIPTION = "hogedesc",
                    FILEKEY = Guid.NewGuid(),
                    THUMBNAILKEY = Guid.NewGuid(),
                    ISPAID = false,
                    SORTORDER = 0,
                    UPDATEDDATE = new DateTime(2023,4,4,16,15,10)
                },
                new QK_MODEL_MST
                {
                    MODELID = "hoge2",
                    DISPLAYNAME = "hoge2fuga",
                    DESCRIPTION = "hoge2desc",
                    FILEKEY = Guid.NewGuid(),
                    THUMBNAILKEY = Guid.NewGuid(),
                    ISPAID = true,
                    SORTORDER = 1,
                    UPDATEDDATE = new DateTime(2023,4,4,15,20,15)
                }
            };
            _repo.Setup(m => m.ReadList(true)).Returns(targetEntities);

            var results = _worker.ReadList(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            
            var item1 = results.ModelItems[0];
            item1.ModelId.Is(targetEntities[0].MODELID);
            item1.DisplayName.Is(targetEntities[0].DISPLAYNAME);
            item1.Description.Is(targetEntities[0].DESCRIPTION);
            item1.FileKey.Is(targetEntities[0].FILEKEY.ToEncrypedReference());
            item1.ThumbnailKey.Is(targetEntities[0].THUMBNAILKEY.ToEncrypedReference());
            item1.IsPaid.Is(targetEntities[0].ISPAID);
            item1.SortOrder.Is(targetEntities[0].SORTORDER);
            item1.UpdatedDate.Is(targetEntities[0].UPDATEDDATE.ToApiDateString());

            var item2 = results.ModelItems[1];
            item2.ModelId.Is(targetEntities[1].MODELID);
            item2.DisplayName.Is(targetEntities[1].DISPLAYNAME);
            item2.Description.Is(targetEntities[1].DESCRIPTION);
            item2.FileKey.Is(targetEntities[1].FILEKEY.ToEncrypedReference());
            item2.ThumbnailKey.Is(targetEntities[1].THUMBNAILKEY.ToEncrypedReference());
            item2.IsPaid.Is(targetEntities[1].ISPAID);
            item2.SortOrder.Is(targetEntities[1].SORTORDER);
            item2.UpdatedDate.Is(targetEntities[1].UPDATEDDATE.ToApiDateString());
        }

        [TestMethod]
        public void Read_アカウントキーが不正でエラーが返る()
        {
            var args = new QoModelReadApiArgs
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
            var args = new QoModelReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
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
            var args = new QoModelReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Returns(default(QK_MODEL_MST));

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.ModelItem.IsNull();
            ret.IsDeleted.IsFalse();
        }

        [TestMethod]
        public void Read_削除済みデータも正常扱いで返る()
        {
            var args = new QoModelReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
            };

            var entity = new QK_MODEL_MST
            {
                MODELID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = Guid.NewGuid(),
                THUMBNAILKEY = Guid.NewGuid(),
                ISPAID = false,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2023, 4, 4, 16, 15, 10),
                DELETEFLAG = true
            };

            _repo.Setup(m => m.Read("hoge")).Returns(entity);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.ModelItem.IsNotNull();
            ret.IsDeleted.IsTrue(); // 削除済み判定

            // 正常にコンバートされている
            var item = ret.ModelItem;
            item.ModelId.Is(entity.MODELID);
            item.DisplayName.Is(entity.DISPLAYNAME);
            item.Description.Is(entity.DESCRIPTION);
            item.FileKey.Is(entity.FILEKEY.ToEncrypedReference());
            item.ThumbnailKey.Is(entity.THUMBNAILKEY.ToEncrypedReference());
            item.IsPaid.Is(entity.ISPAID);
            item.SortOrder.Is(entity.SORTORDER);
            item.UpdatedDate.Is(entity.UPDATEDDATE.ToApiDateString());
        }

        [TestMethod]
        public void Read_正常パターン()
        {
            var args = new QoModelReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
            };

            var entity = new QK_MODEL_MST
            {
                MODELID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = Guid.NewGuid(),
                THUMBNAILKEY = Guid.NewGuid(),
                ISPAID = false,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(2023, 4, 4, 16, 15, 10),
                DELETEFLAG = false
            };

            _repo.Setup(m => m.Read("hoge")).Returns(entity);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.ModelItem.IsNotNull();
            ret.IsDeleted.IsFalse(); 

            // 正常にコンバートされている
            var item = ret.ModelItem;
            item.ModelId.Is(entity.MODELID);
            item.DisplayName.Is(entity.DISPLAYNAME);
            item.Description.Is(entity.DESCRIPTION);
            item.FileKey.Is(entity.FILEKEY.ToEncrypedReference());
            item.ThumbnailKey.Is(entity.THUMBNAILKEY.ToEncrypedReference());
            item.IsPaid.Is(entity.ISPAID);
            item.SortOrder.Is(entity.SORTORDER);
            item.UpdatedDate.Is(entity.UPDATEDDATE.ToApiDateString());
        }

        [TestMethod]
        public void ReadFile_アカウントキーが不正でエラーが返る()
        {
            var args = new QoModelFileReadApiArgs
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
            var args = new QoModelFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
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
            var args = new QoModelFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Returns(default(QK_MODEL_MST));

            var ret = _worker.ReadFile(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1006"); // not found
        }

        [TestMethod]
        public void ReadFile_モデルが削除されていてもエラー()
        {
            var args = new QoModelFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge"
            };

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_MODEL_MST
            {
                MODELID = "hoge",
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
            var args = new QoModelFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge",
                LastUpdatedDate = new DateTime(2023,4,4,21,38,15).ToApiDateString()
            };

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_MODEL_MST
            {
                MODELID = "hoge",
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
            var args = new QoModelFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge",
                LastUpdatedDate = new DateTime(2023, 4, 4, 21, 38, 15).ToApiDateString()
            };

            var fileKey = Guid.NewGuid();

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_MODEL_MST
            {
                MODELID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = fileKey,
                THUMBNAILKEY = Guid.NewGuid(),
                ISPAID = false,
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
            var args = new QoModelFileReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "hoge",
            };

            var fileKey = Guid.NewGuid();

            _repo.Setup(m => m.Read("hoge")).Returns(new QK_MODEL_MST
            {
                MODELID = "hoge",
                DISPLAYNAME = "hogefuga",
                DESCRIPTION = "hogedesc",
                FILEKEY = fileKey,
                THUMBNAILKEY = Guid.NewGuid(),
                ISPAID = false,
                SORTORDER = 0,
                UPDATEDDATE = new DateTime(1999,1,1,1,1,0) // 相当古い
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
