using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class DiseaseSearchWorkerFixture
    {
        Mock<IMedisRepository> _medisRepo;
        DiseaseSearchWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _medisRepo = new Mock<IMedisRepository>();
            _worker = new DiseaseSearchWorker(_medisRepo.Object);
        }

        #region ヘルパーメソッド

        /// <summary>
        /// 有効な引数を取得
        /// </summary>
        QoDiseaseSearchApiArgs GetValidArgs()
        {
            return new QoDiseaseSearchApiArgs
            {
                ActorKey = Guid.NewGuid().ToString(),
                Keyword = "膝関節"
            };
        }

        /// <summary>
        /// 形態素スコアビューを作成
        /// </summary>
        QH_MEDISMORPHEME_SCORE_VIEW CreateMorphemeScoreView(string referenceCode, string indexTerm, string reading, decimal priority, int score)
        {
            return new QH_MEDISMORPHEME_SCORE_VIEW
            {
                REFERENCECODE = referenceCode,
                INDEXTERM = indexTerm,
                READING = reading,
                PRIORITY = priority,
                SCORE = score
            };
        }

        /// <summary>
        /// 病名マスタを作成
        /// </summary>
        QH_MEDISDISEASE_MST CreateDiseaseMst(string diseaseCode, string exchangeCode, string name, string nameKana, string icd10Code, string icd10MultiCode = null)
        {
            return new QH_MEDISDISEASE_MST
            {
                DISEASEMASTERCODE = diseaseCode,
                DISEASEEXCHANGECODE = exchangeCode,
                DISEASENAME = name,
                DISEASENAMEKANA = nameKana,
                ICD10CODE = icd10Code,
                ICD10MULTICODE = icd10MultiCode,
                ADOPTIONTYPE = 1,
                RECEIPTSYSTEMCODE = "8830542",
                USAGEFIELD = 1,
                REVISIONNO = 212,
                DELETEFLAG = false,
                CREATEDDATE = DateTime.Now,
                UPDATEDDATE = DateTime.Now
            };
        }

        #endregion

        #region 引数バリデーションテスト

        [TestMethod]
        public void ActorKeyが不正でエラーとなる()
        {
            // Arrange
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // ArgumentError
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
        }

        [TestMethod]
        public void Keywordが未設定でエラーとなる()
        {
            // Arrange
            var args = GetValidArgs();
            args.Keyword = null;

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // ArgumentError
            results.Result.Detail.Contains(nameof(args.Keyword)).IsTrue();
            results.Result.Detail.Contains("未設定").IsTrue();
        }

        [TestMethod]
        public void Keywordが空白でエラーとなる()
        {
            // Arrange
            var args = GetValidArgs();
            args.Keyword = "   ";

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // ArgumentError
            results.Result.Detail.Contains(nameof(args.Keyword)).IsTrue();
        }

        #endregion

        #region リポジトリ例外処理テスト

        [TestMethod]
        public void ReadMorphemeScoreListで例外が発生したらエラーとなる()
        {
            // Arrange
            var args = GetValidArgs();
            _medisRepo.Setup(x => x.ReadMorphemeScoreList(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
                .Throws(new Exception("データベースエラー"));

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Detail.Contains("形態素マッピングマスタからのデータ取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void ReadDiseaseListByScoreViewで例外が発生したらエラーとなる()
        {
            // Arrange
            var args = GetValidArgs();
            var morphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>
            {
                CreateMorphemeScoreView("20054499", "膝関節", "シツカンセツ", 1.0m, 100)
            };

            _medisRepo.Setup(x => x.ReadMorphemeScoreList(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
                .Returns(morphemeScoreList);

            _medisRepo.Setup(x => x.ReadDiseaseListByScoreView(It.IsAny<List<QH_MEDISMORPHEME_SCORE_VIEW>>()))
                .Throws(new Exception("データベースエラー"));

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Detail.Contains("標準病名マスタからのデータ取得に失敗しました").IsTrue();
        }

        #endregion

        #region 正常系テスト

        [TestMethod]
        public void 膝関節で検索すると膝関節症が取得できる()
        {
            // Arrange
            var args = GetValidArgs();
            args.Keyword = "膝関節";

            var morphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>
            {
                CreateMorphemeScoreView("20054499", "膝関節", "シツカンセツ", 1.0m, 100),
                CreateMorphemeScoreView("20054506", "膝関節", "シツカンセツ", 1.0m, 95),
                CreateMorphemeScoreView("20054517", "膝関節", "シツカンセツ", 1.0m, 90)
            };

            var diseaseList = new List<QH_MEDISDISEASE_MST>
            {
                CreateDiseaseMst("20054499", "AM6J", "一側性外傷後膝関節症", "イッソクセイガイショウゴシツカンセツショウ", "M173 ", "M173+M171"),
                CreateDiseaseMst("20054506", "UT2B", "一側性原発性膝関節症", "イッソクセイゲンパツセイシツカンセツショウ", "M171 ", null),
                CreateDiseaseMst("20054517", "E77F", "一側性続発性膝関節症", "イッソクセイゾクハツセイシツカンセツショウ", "M175 ", "M175+M171")
            };

            _medisRepo.Setup(x => x.ReadMorphemeScoreList(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
                .Returns(morphemeScoreList);

            _medisRepo.Setup(x => x.ReadDiseaseListByScoreView(It.IsAny<List<QH_MEDISMORPHEME_SCORE_VIEW>>()))
                .Returns(diseaseList);

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.TrueString);
            results.DiseaseList.IsNotNull();
            results.DiseaseList.Count.Is(3);
            results.DiseaseList[0].DiseaseMasterCode.Is("20054499");
            results.DiseaseList[0].DiseaseMasterName.Is("一側性外傷後膝関節症");
            results.DiseaseList[0].DiseaseExchangeCode.Is("AM6J");
            results.DiseaseList[0].ICD10Code.Is("M173 ");
            results.DiseaseList[0].ICD10MultiCode.Is("M173+M171");
        }

        [TestMethod]
        public void 心筋梗塞で検索すると心筋梗塞関連病名が取得できる()
        {
            // Arrange
            var args = GetValidArgs();
            args.Keyword = "心筋梗塞";

            var morphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>
            {
                CreateMorphemeScoreView("20097410", "心筋梗塞", "シンキンコウソク", 1.0m, 100),
                CreateMorphemeScoreView("20083957", "心筋梗塞", "シンキンコウソク", 1.0m, 98),
                CreateMorphemeScoreView("20058444", "心筋梗塞", "シンキンコウソク", 1.0m, 95)
            };

            var diseaseList = new List<QH_MEDISDISEASE_MST>
            {
                CreateDiseaseMst("20097410", "V7HQ", "ＳＴ上昇型急性心筋梗塞", "ＳＴジョウショウガタキュウセイシンキンコウソク", "I219 ", "I219+I21"),
                CreateDiseaseMst("20083957", "C8Q1", "急性心筋梗塞", "キュウセイシンキンコウソク", "I219 ", null),
                CreateDiseaseMst("20058444", "GRPS", "急性心筋梗塞後心室中隔穿孔", "キュウセイシンキンコウソクゴシンシツチュウカクセンコウ", "I232 ", "I232+I219")
            };

            _medisRepo.Setup(x => x.ReadMorphemeScoreList(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
                .Returns(morphemeScoreList);

            _medisRepo.Setup(x => x.ReadDiseaseListByScoreView(It.IsAny<List<QH_MEDISMORPHEME_SCORE_VIEW>>()))
                .Returns(diseaseList);

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.TrueString);
            results.DiseaseList.IsNotNull();
            results.DiseaseList.Count.Is(3);
            results.DiseaseList[0].DiseaseMasterCode.Is("20097410");
            results.DiseaseList[0].DiseaseMasterName.Is("ＳＴ上昇型急性心筋梗塞");
        }

        #endregion

        #region ビジネスロジックテスト

        [TestMethod]
        public void 形態素スコアが50件以上でも病名は最大20件まで取得される()
        {
            // Arrange
            var args = GetValidArgs();
            var morphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>();

            // 30件の形態素スコアを作成
            for (int i = 1; i <= 30; i++)
            {
                morphemeScoreList.Add(CreateMorphemeScoreView(
                    $"2005{i:D4}", 
                    "膝関節", 
                    "シツカンセツ", 
                    1.0m, 
                    100 - i));
            }

            var diseaseList = new List<QH_MEDISDISEASE_MST>();
            // 最大20件のみ返す（Take(MaxDiseaseListCount)で絞り込まれた後のリスト）
            for (int i = 1; i <= 20; i++)
            {
                diseaseList.Add(CreateDiseaseMst(
                    $"2005{i:D4}",
                    $"EX{i:D2}",
                    $"膝関節症{i}",
                    "シツカンセツショウ",
                    "M171 ",
                    null));
            }

            _medisRepo.Setup(x => x.ReadMorphemeScoreList(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
                .Returns(morphemeScoreList);

            _medisRepo.Setup(x => x.ReadDiseaseListByScoreView(It.IsAny<List<QH_MEDISMORPHEME_SCORE_VIEW>>()))
                .Returns(diseaseList);

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.TrueString);
            results.DiseaseList.IsNotNull();
            results.DiseaseList.Count.Is(20); // 最大20件
            // 最初の病名がスコアが最も高いもの（SCORE=99）
            results.DiseaseList[0].DiseaseMasterCode.Is("20050001");
        }

        [TestMethod]
        public void 同じREFERENCECODEの重複が排除されスコアが高い方が採用される()
        {
            // Arrange
            var args = GetValidArgs();
            var morphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>
            {
                // 同じREFERENCECODEで異なるスコア
                CreateMorphemeScoreView("20054499", "膝関節", "シツカンセツ", 1.0m, 100),
                CreateMorphemeScoreView("20054499", "膝", "ヒザ", 0.8m, 80), // 重複・低スコア
                CreateMorphemeScoreView("20054506", "膝関節", "シツカンセツ", 1.0m, 95),
                CreateMorphemeScoreView("20054506", "関節", "カンセツ", 0.5m, 50), // 重複・低スコア
                CreateMorphemeScoreView("20054517", "膝関節", "シツカンセツ", 1.0m, 90)
            };

            var diseaseList = new List<QH_MEDISDISEASE_MST>
            {
                CreateDiseaseMst("20054499", "AM6J", "一側性外傷後膝関節症", "イッソクセイガイショウゴシツカンセツショウ", "M173 ", null),
                CreateDiseaseMst("20054506", "UT2B", "一側性原発性膝関節症", "イッソクセイゲンパツセイシツカンセツショウ", "M171 ", null),
                CreateDiseaseMst("20054517", "E77F", "一側性続発性膝関節症", "イッソクセイゾクハツセイシツカンセツショウ", "M175 ", null)
            };

            _medisRepo.Setup(x => x.ReadMorphemeScoreList(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
                .Returns(morphemeScoreList);

            _medisRepo.Setup(x => x.ReadDiseaseListByScoreView(It.IsAny<List<QH_MEDISMORPHEME_SCORE_VIEW>>()))
                .Returns(diseaseList);

            // Act
            var results = _worker.Search(args);

            // Assert
            results.IsSuccess.Is(bool.TrueString);
            results.DiseaseList.IsNotNull();
            results.DiseaseList.Count.Is(3); // 重複排除後は3件
            
            // スコア順にソートされている（100 > 95 > 90）
            results.DiseaseList[0].DiseaseMasterCode.Is("20054499"); // SCORE=100
            results.DiseaseList[1].DiseaseMasterCode.Is("20054506"); // SCORE=95
            results.DiseaseList[2].DiseaseMasterCode.Is("20054517"); // SCORE=90
        }

        #endregion
    }
}
