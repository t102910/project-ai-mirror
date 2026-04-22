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
    public class QkRandomAdviceReadWorkerFixture
    {
        QkRandomAdviceReadWorker _worker;
        Mock<IQkRandomAdviceRepository> _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IQkRandomAdviceRepository>();
            _worker = new QkRandomAdviceReadWorker(_repo.Object);
        }

        [TestMethod]
        public void ReadList_アカウントキーが不正で失敗が返る()
        {
            var args = new QoRandomAdviceListReadApiArgs
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
        public void Read_アカウントキーが不正で失敗が返る()
        {
            var args = new QoRandomAdviceReadApiArgs
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
        public void ReadList_データ取得処理で例外がでたらエラーが返る()
        {
            var args = new QoRandomAdviceListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "0"
            };

            _repo.Setup(m => m.ReadList("0")).Throws(new Exception());

            var ret = _worker.ReadList(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("0500");
        }

        [TestMethod]
        public void ReadList_正常にデータが取得できる()
        {
            var args = new QoRandomAdviceListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "0"
            };

            var entityList = new List<QK_RANDOMADVICE_MST>
            {
                new QK_RANDOMADVICE_MST
                {
                    ID = 1,
                    MODELID = "0",
                    ADVICE = "hoge",
                    CATEGORYTYPE = 1,                    
                    MOTIONTYPE = 1,
                    TIMETYPE = 1,                    
                },
                new QK_RANDOMADVICE_MST
                {
                    ID = 2,
                    MODELID = "0",
                    ADVICE = "fuga",
                    CATEGORYTYPE = 2,
                    MOTIONTYPE = 2,
                    TIMETYPE = 2,
                },
            };

            _repo.Setup(m => m.ReadList("0")).Returns(entityList);

            var ret = _worker.ReadList(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.AdviceItems.Count.Is(2);

            var item1 = ret.AdviceItems[0];
            var entity1 = entityList[0];
            item1.ID.Is(entity1.ID);
            item1.ModelId.Is(entity1.MODELID);
            item1.Advice.Is(entity1.ADVICE);
            item1.CategoryType.Is((QkAdviceCategoryType)entity1.CATEGORYTYPE);
            item1.MotionType.Is((QkModelMotionType)entity1.MOTIONTYPE);
            item1.TimeType.Is((QkAdviceTimeType)entity1.TIMETYPE);

            var item2 = ret.AdviceItems[1];
            var entity2 = entityList[1];
            item2.ID.Is(entity2.ID);
            item2.ModelId.Is(entity2.MODELID);
            item2.Advice.Is(entity2.ADVICE);
            item2.CategoryType.Is((QkAdviceCategoryType)entity2.CATEGORYTYPE);
            item2.MotionType.Is((QkModelMotionType)entity2.MOTIONTYPE);
            item2.TimeType.Is((QkAdviceTimeType)entity2.TIMETYPE);
        }

        [TestMethod]
        public void Read_季節に対応したアドバイスが正常に取得できる()
        {
            var args = new QoRandomAdviceListReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "0",
                IsFilterCurrentSeason = true
            };

            var now = DateTime.Now;            

            var entityList = new List<QK_RANDOMADVICE_MST>
            {
                new QK_RANDOMADVICE_MST
                {
                    ID = 1,
                    MODELID = "0",
                    ADVICE = "hoge",
                    CATEGORYTYPE = 1,
                    MOTIONTYPE = 1,
                    TIMETYPE = 1,
                    M1 = now.Month == 1,
                    M2 = now.Month == 2,
                    M3 = now.Month == 3,
                    M4 = now.Month == 4,
                    M5 = now.Month == 5,
                    M6 = now.Month == 6,
                    M7 = now.Month == 7,
                    M8 = now.Month == 8,
                    M9 = now.Month == 9,
                    M10 = now.Month == 10,
                    M11 = now.Month == 11,
                    M12 = now.Month == 12,
                },
                new QK_RANDOMADVICE_MST
                {
                    ID = 3,
                    MODELID = "0",
                    ADVICE = "Piyo",
                    CATEGORYTYPE = 3,
                    MOTIONTYPE = 3,
                    TIMETYPE = 3,
                    M1 = now.Month == 1,
                    M2 = now.Month == 2,
                    M3 = now.Month == 3,
                    M4 = now.Month == 4,
                    M5 = now.Month == 5,
                    M6 = now.Month == 6,
                    M7 = now.Month == 7,
                    M8 = now.Month == 8,
                    M9 = now.Month == 9,
                    M10 = now.Month == 10,
                    M11 = now.Month == 11,
                    M12 = now.Month == 12,
                }
            };

            _repo.Setup(m => m.ReadSeasonList("0", now.Month)).Returns(entityList);

            var ret = _worker.ReadList(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.AdviceItems.Count.Is(2);

            var item1 = ret.AdviceItems[0];
            var entity1 = entityList[0];
            item1.ID.Is(entity1.ID);
            item1.ModelId.Is(entity1.MODELID);
            item1.Advice.Is(entity1.ADVICE);
            item1.CategoryType.Is((QkAdviceCategoryType)entity1.CATEGORYTYPE);
            item1.MotionType.Is((QkModelMotionType)entity1.MOTIONTYPE);
            item1.TimeType.Is((QkAdviceTimeType)entity1.TIMETYPE);
            item1.M1.Is(now.Month == 1);
            item1.M2.Is(now.Month == 2);
            item1.M3.Is(now.Month == 3);
            item1.M4.Is(now.Month == 4);
            item1.M5.Is(now.Month == 5);
            item1.M6.Is(now.Month == 6);
            item1.M7.Is(now.Month == 7);
            item1.M8.Is(now.Month == 8);
            item1.M9.Is(now.Month == 9);
            item1.M10.Is(now.Month == 10);
            item1.M11.Is(now.Month == 11);
            item1.M12.Is(now.Month == 12);

            var item2 = ret.AdviceItems[1];
            var entity2 = entityList[1];
            item2.ID.Is(entity2.ID);
            item2.ModelId.Is(entity2.MODELID);
            item2.Advice.Is(entity2.ADVICE);
            item2.CategoryType.Is((QkAdviceCategoryType)entity2.CATEGORYTYPE);
            item2.MotionType.Is((QkModelMotionType)entity2.MOTIONTYPE);
            item2.TimeType.Is((QkAdviceTimeType)entity2.TIMETYPE);
            item2.M1.Is(now.Month == 1);
            item2.M2.Is(now.Month == 2);
            item2.M3.Is(now.Month == 3);
            item2.M4.Is(now.Month == 4);
            item2.M5.Is(now.Month == 5);
            item2.M6.Is(now.Month == 6);
            item2.M7.Is(now.Month == 7);
            item2.M8.Is(now.Month == 8);
            item2.M9.Is(now.Month == 9);
            item2.M10.Is(now.Month == 10);
            item2.M11.Is(now.Month == 11);
            item2.M12.Is(now.Month == 12);

            // ReadSeasonListが呼ばれた
            _repo.Verify(m => m.ReadSeasonList("0", now.Month), Times.Once);
        }

        [TestMethod]
        public void Read_ID指定で正常にデータを取得できる()
        {
            var args = new QoRandomAdviceReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                AdviceId = 1,
                ModelId = "0",
                TargetDateTime = DateTime.Now.ToApiDateString()
            };

            var entity = new QK_RANDOMADVICE_MST
            {
                ID = 1,
                MODELID = "0",
                ADVICE = "hoge",
                CATEGORYTYPE = 1,
                MOTIONTYPE = 1,
                TIMETYPE = 1,
            };

            _repo.Setup(m => m.Read(1)).Returns(entity);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.AdviceItem.ID.Is(entity.ID);
            ret.AdviceItem.ModelId.Is(entity.MODELID);
            ret.AdviceItem.Advice.Is(entity.ADVICE);
            ret.AdviceItem.CategoryType.Is((QkAdviceCategoryType)entity.CATEGORYTYPE);
            ret.AdviceItem.MotionType.Is((QkModelMotionType)entity.MOTIONTYPE);
            ret.AdviceItem.TimeType.Is((QkAdviceTimeType)entity.TIMETYPE);
        }

        [TestMethod]
        public void Read_ID指定で該当データ無しは正常とする()
        {
            var args = new QoRandomAdviceReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                AdviceId = 1,
                ModelId = "0",
                TargetDateTime = DateTime.Now.ToApiDateString()
            };            

            _repo.Setup(m => m.Read(1)).Returns(default(QK_RANDOMADVICE_MST));

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.AdviceItem.IsNull();
        }

        [TestMethod]
        public void Read_ランダムなアドバイスが返る_時間指定無し()
        {
            var args = new QoRandomAdviceReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "0",
                ExcludeAdviceIdList = new List<int> { 1}
            };

            // 中身はなんでも良い
            var entityList = new List<QK_RANDOMADVICE_MST>
            {
                new QK_RANDOMADVICE_MST
                {
                    ID = 1,
                    MODELID = "0",
                    ADVICE = "hoge",
                    CATEGORYTYPE = 1,
                    MOTIONTYPE = 1,
                    TIMETYPE = 1,
                },
                new QK_RANDOMADVICE_MST
                {
                    ID = 2,
                    MODELID = "0",
                    ADVICE = "fuga",
                    CATEGORYTYPE = 2,
                    MOTIONTYPE = 2,
                    TIMETYPE = 2,
                },
            };

            var now = DateTime.Now;
            var hour = TimeSpan.FromHours(now.Hour);
            QkAdviceTimeType timeType;
            if (TimeSpan.FromHours(5) <= hour && hour < TimeSpan.FromHours(12))
            {
                timeType = QkAdviceTimeType.Morning;
            }
            else if (TimeSpan.FromHours(12) <= hour && hour < TimeSpan.FromHours(18))
            {
                timeType = QkAdviceTimeType.Afternoon;
            }
            else if (TimeSpan.FromHours(18) <= hour && hour < TimeSpan.FromHours(24))
            {
                timeType = QkAdviceTimeType.Evening;
            }
            else
            {
                timeType = QkAdviceTimeType.Night;
            }

            _repo.Setup(m => m.ReadRandomList("0", now.Month, timeType, args.ExcludeAdviceIdList)).Returns(entityList);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.AdviceItem.IsNotNull();

            // モデルIDと現在月と時間帯を正しく指定した関数が呼ばれた
            _repo.Verify(m => m.ReadRandomList("0", now.Month, timeType, args.ExcludeAdviceIdList), Times.Once);
        }

        [TestMethod]
        public void Read_ランダムなアドバイスが返る_時間指定あり()
        {
            var args = new QoRandomAdviceReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ModelId = "0",
                TargetDateTime = new DateTime(2023,3,20,2,15,0).ToApiDateString()
            };

            // 中身はなんでも良い
            var entityList = new List<QK_RANDOMADVICE_MST>
            {
                new QK_RANDOMADVICE_MST
                {
                    ID = 1,
                    MODELID = "0",
                    ADVICE = "hoge",
                    CATEGORYTYPE = 1,
                    MOTIONTYPE = 1,
                    TIMETYPE = 1,
                },
                new QK_RANDOMADVICE_MST
                {
                    ID = 2,
                    MODELID = "0",
                    ADVICE = "fuga",
                    CATEGORYTYPE = 2,
                    MOTIONTYPE = 2,
                    TIMETYPE = 2,
                },
            };

            _repo.Setup(m => m.ReadRandomList("0", 3, QkAdviceTimeType.Night,args.ExcludeAdviceIdList)).Returns(entityList);

            var ret = _worker.Read(args);

            ret.IsSuccess.Is(bool.TrueString);
            ret.AdviceItem.IsNotNull();

            // モデルIDと指定月と時間帯を正しく指定した関数が呼ばれた
            _repo.Verify(m => m.ReadRandomList("0", 3, QkAdviceTimeType.Night,args.ExcludeAdviceIdList), Times.Once);
        }
    }
}
