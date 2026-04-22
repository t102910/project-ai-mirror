using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
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
    public class MasterMedicineWorkerFixture
    {
        Mock<IMedicineRepository> _repo;
        MasterMedicineWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IMedicineRepository>();
            _worker = new MasterMedicineWorker(_repo.Object);
        }

        [TestMethod]
        public void 医薬品検索でDB処理でエラーがあると失敗する()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var keyword = "hoge";

            _repo.Setup(m => m.SearchMedicine(accountKey, keyword, 0, 100)).Throws(new Exception());

            var args = new QoMasterMedicineSearchApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                SearchText = keyword
            };

            var ret = _worker.Search(args);

            // 失敗する
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
        }

        [TestMethod]
        public void 医薬品検索で正常に結果をうけとれる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var keyword = "hoge";

            _repo.Setup(m => m.SearchMedicine(accountKey, keyword, 0, 100)).Returns(new DbEthicalDrugSearcherResults
            {
                IsSuccess = true,
                PageIndex = 0,
                MaxPageIndex = 5,
                EthicalDrugN = new List<DbEthicalDrugSearchItem>
                {
                    new DbEthicalDrugSearchItem
                    {
                        YjCode = "abc",
                        ProductName = "くすり",
                        CommonName = "こもん",
                        ApprovalCompanyName = "カンパニー",
                        SaleCompanyName = "セール",
                        GeneralCode = "567",
                        IsGeneric = true,
                        InclutionDate = new DateTime(2010,5,5,10,0,0),
                    },
                    new DbEthicalDrugSearchItem
                    {
                        YjCode = "def",
                        ProductName = "くすり",
                        CommonName = "こもん",
                        ApprovalCompanyName = "カンパニー",
                        SaleCompanyName = "セール",
                        GeneralCode = "567",
                        IsGeneric = false,
                        InclutionDate = new DateTime(2010,5,5,10,0,0),
                    }
                }
            });

            var args = new QoMasterMedicineSearchApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                SearchText = keyword
            };

            var ret = _worker.Search(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // ページ情報が正しく取得できている
            ret.PageIndex.Is("0");
            ret.MaxPageIndex.Is("5");
            // データが正しく2件取得できている
            ret.MedicineN.Count.Is(2);
            // データがそれぞれ正しく変換されている
            ret.MedicineN[0].YjCode.Is("abc");
            ret.MedicineN[0].ProductName.Is("くすり");
            ret.MedicineN[0].CommonName.Is("こもん");
            ret.MedicineN[0].ApprovalCompanyName.Is("カンパニー");
            ret.MedicineN[0].SaleCompanyName.Is("セール");
            ret.MedicineN[0].GeneralCode.Is("567");
            ret.MedicineN[0].IsGeneric.Is(bool.TrueString);
            ret.MedicineN[0].InclutionDate.Is(new DateTime(2010, 5, 5, 10, 0, 0).ToApiDateString());
            ret.MedicineN[1].YjCode.Is("def");
            ret.MedicineN[1].IsGeneric.Is(bool.FalseString);
        }

        [TestMethod]
        public void 医薬品詳細でYjCode未指定で失敗する()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterMedicineDetailReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                YjCode = ""
            };

            var ret = _worker.DetailRead(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002"); // 引数エラー
        }

        [TestMethod]
        public void 医薬品詳細でDB処理でエラーがあると失敗する()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterMedicineDetailReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                YjCode = "1234"
            };

            _repo.Setup(m => m.ReadDetail(args.YjCode)).Throws(new Exception());

            var ret = _worker.DetailRead(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
        }

        [TestMethod]
        public void 医薬品詳細で正常に結果を受け取れる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoMasterMedicineDetailReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                YjCode = "1234"
            };

            var dbResult = new QH_ETHDRUG_DETAIL_VIEW
            {
                YJCODE = "1234",
                PRODUCTNAME = "くすり",
                COMMONNAME = "こもん",
                APPROVALCOMPANYNAME = "カンパニー",
                SALESCOMPANYNAME = "セール",
                INGREDIENTS = "abc",
                GENERALCODE = "12",
                ACTIONA = "a",
                ACTIONB = "b",
                ACTIONC1 = "c1",
                ACTIONC2 = "c2",
                INTERACTION = "interaction",
                PRECAUTIONS = "cautions",
                DRUGORFOOD = "food",
                FileEntityN = new List<QH_ETHDRUGFILE_MST>
                {
                    new QH_ETHDRUGFILE_MST
                    {
                        FILEKEY = Guid.NewGuid(),
                        SEQUENCE = 1
                    },
                    new QH_ETHDRUGFILE_MST
                    {
                        FILEKEY = Guid.NewGuid(),
                        SEQUENCE = 2
                    }
                }
            };

            _repo.Setup(m => m.ReadDetail(args.YjCode)).Returns(dbResult);

            var ret = _worker.DetailRead(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // 正しくデータが変換されている
            ret.Medicine.YjCode.Is("1234");
            ret.Medicine.ProductName.Is("くすり");
            ret.Medicine.CommonName.Is("こもん");
            ret.Medicine.ApprovalCompanyName.Is("カンパニー");
            ret.Medicine.SaleCompanyName.Is("セール");
            ret.Medicine.Ingredients.Is("abc");
            ret.Medicine.GeneralCode.Is("12");
            ret.Medicine.ActionA.Is("a");
            ret.Medicine.ActionB.Is("b");
            ret.Medicine.ActionC1.Is("c1");
            ret.Medicine.ActionC2.Is("c2");
            ret.Medicine.Interaction.Is("interaction");
            ret.Medicine.Precautions.Is("cautions");
            ret.Medicine.DrugOrFood.Is("food");
            ret.Medicine.FileKeyN.Count.Is(2);
            ret.Medicine.FileKeyN[0].FileKeyReference.Is(dbResult.FileEntityN[0].FILEKEY.ToEncrypedReference());
            ret.Medicine.FileKeyN[0].Sequence.Is(dbResult.FileEntityN[0].SEQUENCE.ToString());
            ret.Medicine.FileKeyN[1].FileKeyReference.Is(dbResult.FileEntityN[1].FILEKEY.ToEncrypedReference());
            ret.Medicine.FileKeyN[1].Sequence.Is(dbResult.FileEntityN[1].SEQUENCE.ToString());
            ret.Medicine.ThumbnailKey.Is(ret.Medicine.FileKeyN[0]);


            // FileKeyNがnullの場合
            dbResult.FileEntityN = null;

            ret = _worker.DetailRead(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            ret.Medicine.FileKeyN.Count.Is(0);
            ret.Medicine.FileKeyN.IsNotNull();
            ret.Medicine.ThumbnailKey.IsNotNull();
        }

        [TestMethod]
        public void 医薬品画像取得でDBエラーが発生した場合は失敗となる()
        {
            var fileKey = Guid.Parse("94c5da04-dd3f-4100-b333-0980ad8bee00");

            var args = new QoMasterMedicineImageReadApiArgs
            {
                FileKey = new QoApiFileKeyItem
                {
                    FileKeyReference = fileKey.ToEncrypedReference()
                }
            };

            // DB取得処理で例外を発生させる
            _repo.Setup(m => m.ReadEthDrugFileEntity(fileKey)).Throws(new Exception());

            var ret = _worker.ImageRead(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
        }

        [TestMethod]
        public void 医薬品画像取得でStorage取得でエラーが発生した場合は失敗となる()
        {
            var fileKey = Guid.Parse("94c5da04-dd3f-4100-b333-0980ad8bee00");

            var args = new QoMasterMedicineImageReadApiArgs
            {
                FileKey = new QoApiFileKeyItem
                {
                    FileKeyReference = fileKey.ToEncrypedReference()
                }
            };

            var entity = new QH_ETHDRUGFILE_MST
            {
                FILEKEY = fileKey,
            };

            // DBからは正常に値を取得できる設定
            _repo.Setup(m => m.ReadEthDrugFileEntity(fileKey)).Returns(entity);

            // Storageアクセスでエラーを発生させる
            _repo.Setup(m => m.ReadEthDrugFileBlob(entity)).Throws(new Exception());

            var ret = _worker.ImageRead(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1004"); // Storageエラー
        }

        [TestMethod]
        public void 医薬品画像取得で正常にデータを取得できる()
        {
            var fileKey = Guid.Parse("94c5da04-dd3f-4100-b333-0980ad8bee00");

            var args = new QoMasterMedicineImageReadApiArgs
            {
                FileKey = new QoApiFileKeyItem
                {
                    FileKeyReference = fileKey.ToEncrypedReference()
                }
            };

            var entity = new QH_ETHDRUGFILE_MST
            {
                FILEKEY = fileKey,
                YJCODE = "123a123"
            };

            // DBからは正常に値を取得できる設定
            _repo.Setup(m => m.ReadEthDrugFileEntity(fileKey)).Returns(entity);

            // Storageからも正常に取得できる設定
            _repo.Setup(m => m.ReadEthDrugFileBlob(entity)).Returns(("imageData","image/jpeg", "123a123.jpg"));

            var ret = _worker.ImageRead(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);

            // 画像情報が正しく取得できている
            ret.Image.FileKeyReference.Is(fileKey.ToEncrypedReference());
            ret.Image.ContentType.Is("image/jpeg");
            ret.Image.Sequence.Is("1");
            ret.Image.OriginalName.Is("123a123.jpg");
            ret.Image.Data.Is("imageData");
        }
    }
}
