using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class MasterDrugDetailReadWorkerFixture
    {
        MasterDrugDetailReadWorker _worker;
        Mock<IOtcDrugRepository> _otcRepo;

        [TestInitialize]
        public void Initialize()
        {
            _otcRepo = new Mock<IOtcDrugRepository>();
            _worker = new MasterDrugDetailReadWorker(_otcRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void ItemCode未設定でエラー()
        {
            var args = GetValidArgs();
            args.ItemCode = ""; // 未入力

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ItemCode)).IsTrue();
            results.Result.Detail.Contains("必須").IsTrue();
        }

        [TestMethod]
        public void OTC情報取得で例外が発生するとエラー()
        {
            var args = GetValidArgs();

            // 例外を起こす
            _otcRepo.Setup(m => m.ReadDrug(args.ItemCode, args.ItemCodeType)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("OTC医薬品情報の取得処理でエアラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 正常に取得できる()
        {
            var args = GetValidArgs();

            var dbItem = new DbOtcDrugDetailItem
            {
                ItemCode = "4987306054769",
                FullWidthMakerName = "大正製薬",
                MakerOfficialItemName = "新ビオフェルミンＳ錠　４５錠",
                FullWidthContent = 45.00M,
                FullWidthContentUnit = "錠",
                FullWidthQuantityUnit = "個",
                ItemFeatures = "おなか大切に＜乳酸菌のくすり＞",
                OtcDrugType = "A",
                DosageFormType = "21",
                PackingStandardName = "４５錠",
                ChildrenType = "0",
                ItemType = "32",
                LastUpdate = new DateTime(2022, 2, 2, 10, 10, 10),
                RequiredReading = "必ずお読みください",
                Features = "＊生きてはたらく乳酸菌。",
                PrefaceCaution = "Hoge",
                ProhibitedMatters = "Fuga",
                Consult = "次の人は服用前に",
                OtherCaution = "その他",
                Indications = "整腸（便通を整える）、軟便、便秘、腹部膨満感",
                Dosages = "次の量を…",
                Ingredient = "９錠（１５歳以上の１日服用量）中...",
                PrecautionForHandling = "［ビン入り品、分包品について］",
                OtherDescriptions = "おなかの弱い方の整腸に",
                FileEntityN = new List<QH_OTCDRUGFILE_MST>
                {
                    new QH_OTCDRUGFILE_MST
                    {
                        FILEKEY = Guid.NewGuid(),
                        SEQUENCE = 1
                    },
                    new QH_OTCDRUGFILE_MST
                    {
                        FILEKEY = Guid.NewGuid(),
                        SEQUENCE = 3
                    }
                }
            };

            // 正常に結果を返す
            _otcRepo.Setup(m => m.ReadDrug(args.ItemCode, args.ItemCodeType)).Returns(dbItem);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 結果がセットされている
            results.OtcDrug.IsNotNull();

            // 正しく変換されている
            results.OtcDrug.ItemCode.Is(dbItem.ItemCode);
            results.OtcDrug.ItemCodeType.Is(args.ItemCodeType);
            results.OtcDrug.MakerName.Is(dbItem.FullWidthMakerName);
            results.OtcDrug.ItemName.Is(dbItem.MakerOfficialItemName);
            results.OtcDrug.ContentQuantity.Is("45.00");
            results.OtcDrug.ContentUnit.Is(dbItem.FullWidthContentUnit);
            results.OtcDrug.ItemUnit.Is(dbItem.FullWidthQuantityUnit);
            results.OtcDrug.ItemFeatures.Is(dbItem.ItemFeatures);
            results.OtcDrug.OtcDrugType.Is(dbItem.OtcDrugType);
            results.OtcDrug.DosageFormType.Is(dbItem.DosageFormType);
            results.OtcDrug.PackingStandardName.Is(dbItem.PackingStandardName);
            results.OtcDrug.ChildrenType.Is(dbItem.ChildrenType);
            results.OtcDrug.ItemType.Is(dbItem.ItemType);
            results.OtcDrug.LastUpdate.Is(dbItem.LastUpdate.ToApiDateString());
            results.OtcDrug.RequiredReading.Is(dbItem.RequiredReading);
            results.OtcDrug.Features.Is(dbItem.Features);
            results.OtcDrug.PrefaceCaution.Is(dbItem.PrefaceCaution);
            results.OtcDrug.ProhibitedMatters.Is(dbItem.ProhibitedMatters);
            results.OtcDrug.Consult.Is(dbItem.Consult);
            results.OtcDrug.OtherCaution.Is(dbItem.OtherCaution);
            results.OtcDrug.Indications.Is(dbItem.Indications);
            results.OtcDrug.Dosages.Is(dbItem.Dosages);
            results.OtcDrug.Ingredient.Is(dbItem.Ingredient);
            results.OtcDrug.PrecautionForHandling.Is(dbItem.PrecautionForHandling);
            results.OtcDrug.OtherDescriptions.Is(dbItem.OtherDescriptions);
            results.OtcDrug.FileKeyN[0].FileKeyReference.Is(dbItem.FileEntityN[0].FILEKEY.ToEncrypedReference());
            results.OtcDrug.FileKeyN[0].Sequence.Is("1");
            results.OtcDrug.FileKeyN[1].FileKeyReference.Is(dbItem.FileEntityN[1].FILEKEY.ToEncrypedReference());
            results.OtcDrug.FileKeyN[1].Sequence.Is("3");
            results.OtcDrug.FileKeyN.Count.Is(2);
        }

        QoMasterDrugDetailReadApiArgs GetValidArgs()
        {
            return new QoMasterDrugDetailReadApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                ItemCode = "4987306054769",
                ItemCodeType = "J",
            };
        }
    }
}
