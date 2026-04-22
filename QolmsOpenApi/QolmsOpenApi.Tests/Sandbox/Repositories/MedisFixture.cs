using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QolmsOpenApi.Tests.Workers;
using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.MorphologyParser;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class MedisFixture
    {
        IMedisRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new MedisRepository();
        }

        [TestMethod]
        public void ReadMorphemScoreListTest()
        {
            // 膝関節炎 完全一致
            var words = "膝関節炎";
            var tokens = QsMorphologyParser.AnalyzeDetailed(words);
            var result = _repo.ReadMorphemeScoreList(words, tokens[0].Surface, tokens[0].Reading, 10);
            // 新しいスコア計算:
            // Surface Priority 1.00 * 100 + 完全一致(source) 100 = 200
            // Reading Priority 1.00 * 100 + カナ前方一致 (30 - (カナ長 - 入力カナ長)) = 100 + (30 - 差分)
            

            // 急性心筋梗塞のケース: 「急性心筋梗塞」が「急性心筋梗塞後心房内血栓症」より上位に来ることを検証
            words = "急性心筋梗塞";
            tokens = QsMorphologyParser.AnalyzeDetailed(words);
            result = _repo.ReadMorphemeScoreList(words, tokens[0].Surface, tokens[0].Reading, 100);
            
        }

        [TestMethod]
        public void ReadDiseaseListByScoreViewTest()
        {
            var words = "急性心筋梗塞";
            var tokens = QsMorphologyParser.AnalyzeDetailed(words);
            var result = _repo.ReadMorphemeScoreList(words, tokens[0].Surface, tokens[0].Reading, 50);

            var sortedList = result
               .OrderByDescending(x => x.SCORE)
               .GroupBy(x => x.REFERENCECODE)
               .Select(g => g.First())
               .Take(20)
               .ToList();

            var diseaseList = _repo.ReadDiseaseListByScoreView(sortedList);
        }

        [TestMethod]
        public void スコア確認用()
        {
            var words = "全般性";
            var tokens = QsMorphologyParser.AnalyzeDetailed(words);
            var allMorphemeScoreList = new List<QH_MEDISMORPHEME_SCORE_VIEW>();
            foreach (var token in tokens)
            {
                var scoreList = _repo.ReadMorphemeScoreList(words, token.Surface, token.Reading, 50);
                allMorphemeScoreList.AddRange(scoreList);
            }
            
            // Scoreでソートし、重複を排除して最大取得件数を抽出
            var sortedList = allMorphemeScoreList
                .OrderByDescending(x => x.SCORE)
                .GroupBy(x => x.REFERENCECODE)
                .Select(g => g.First())
                .Take(20)
                .ToList();
        }
    }
}
