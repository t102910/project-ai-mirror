using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class QoTestPatientHelperFixture
    {
        Mock<IFacilityRepository> _facilityRepo;
        QoTestPatientHelper _helper;
        Guid _facilityKey;

        [TestInitialize]
        public void Initialize()
        {
            _facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            _facilityRepo = new Mock<IFacilityRepository>();
            _helper = new QoTestPatientHelper(_facilityRepo.Object);
        }

        // ----------------------------------------------------------------
        // 正常系: ルール一致
        // ----------------------------------------------------------------

        [TestMethod]
        public void 数字のみルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "15000").IsTrue();
            _helper.IsTestPatient(_facilityKey, "10000").IsTrue();
            _helper.IsTestPatient(_facilityKey, "19999").IsTrue();
        }

        [TestMethod]
        public void プレフィックス付きルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("A10000", "A19999"));

            _helper.IsTestPatient(_facilityKey, "A15000").IsTrue();
        }

        [TestMethod]
        public void サフィックス付きルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("10000X", "19999X"));

            _helper.IsTestPatient(_facilityKey, "15000X").IsTrue();
        }

        [TestMethod]
        public void プレフィックスとサフィックス付きルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("A10000B", "A19999B"));

            _helper.IsTestPatient(_facilityKey, "A15000B").IsTrue();
        }

        [TestMethod]
        public void プレフィックスとサフィックスは大文字小文字差異があっても一致する()
        {
            SetupConfig(fixedLength: 0, ("A10000", "A19999"));

            // 患者IDが小文字、ルールが大文字
            _helper.IsTestPatient(_facilityKey, "a15000").IsTrue();
        }

        [TestMethod]
        public void ルールが大文字小文字混在でも患者IDと一致する()
        {
            SetupConfig(fixedLength: 0, ("a10000", "a19999"));

            // ルールが小文字、患者IDが大文字
            _helper.IsTestPatient(_facilityKey, "A15000").IsTrue();
        }

        [TestMethod]
        public void プレフィックスとサフィックス両方の大文字小文字差異があっても一致する()
        {
            SetupConfig(fixedLength: 0, ("A10000B", "A19999B"));

            _helper.IsTestPatient(_facilityKey, "a15000b").IsTrue();
        }

        // ----------------------------------------------------------------
        // 正常系: 記号プレフィックス・サフィックス
        // ----------------------------------------------------------------

        [TestMethod]
        public void 記号プレフィックス付きルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("TT-10000", "TT-19999"));

            _helper.IsTestPatient(_facilityKey, "TT-15000").IsTrue();
        }

        [TestMethod]
        public void 記号サフィックス付きルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("10000-X", "19999-X"));

            _helper.IsTestPatient(_facilityKey, "15000-X").IsTrue();
        }

        [TestMethod]
        public void 記号プレフィックスとサフィックス付きルールで範囲内の患者IDはtrue()
        {
            SetupConfig(fixedLength: 0, ("TT-10000-X", "TT-19999-X"));

            _helper.IsTestPatient(_facilityKey, "TT-15000-X").IsTrue();
        }

        [TestMethod]
        public void 記号プレフィックスの英字部分は大文字小文字を区別しない()
        {
            SetupConfig(fixedLength: 0, ("TT-10000", "TT-19999"));

            // ルールは大文字 TT-、患者IDは小文字 tt-
            _helper.IsTestPatient(_facilityKey, "tt-15000").IsTrue();
        }

        // ----------------------------------------------------------------
        // 正常系: FixedLength
        // ----------------------------------------------------------------

        [TestMethod]
        public void FixedLengthが一致すればtrue()
        {
            SetupConfig(fixedLength: 5, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "15000").IsTrue();
        }

        [TestMethod]
        public void FixedLengthが不一致ならfalse()
        {
            SetupConfig(fixedLength: 7, ("10000", "19999"));

            // 長さ5 != 7
            _helper.IsTestPatient(_facilityKey, "15000").IsFalse();
        }

        // ----------------------------------------------------------------
        // 正常系: ルール不一致
        // ----------------------------------------------------------------

        [TestMethod]
        public void プレフィックス不一致ならfalse()
        {
            SetupConfig(fixedLength: 0, ("A10000", "A19999"));

            _helper.IsTestPatient(_facilityKey, "B15000").IsFalse();
        }

        [TestMethod]
        public void サフィックス不一致ならfalse()
        {
            SetupConfig(fixedLength: 0, ("10000X", "19999X"));

            _helper.IsTestPatient(_facilityKey, "15000Y").IsFalse();
        }

        [TestMethod]
        public void 記号プレフィックス不一致ならfalse()
        {
            SetupConfig(fixedLength: 0, ("TT-10000", "TT-19999"));

            _helper.IsTestPatient(_facilityKey, "TT#15000").IsFalse();
        }

        [TestMethod]
        public void 記号サフィックス不一致ならfalse()
        {
            SetupConfig(fixedLength: 0, ("10000-X", "19999-X"));

            _helper.IsTestPatient(_facilityKey, "15000#X").IsFalse();
        }

        [TestMethod]
        public void 記号のみで数字を含まない患者IDは一致しない()
        {
            SetupConfig(fixedLength: 0, ("TT-10000", "TT-19999"));

            // 数字部分がないため正規表現にマッチせず false
            _helper.IsTestPatient(_facilityKey, "TT-ABCDE").IsFalse();
        }

        [TestMethod]
        public void 数字部分が範囲より大きいならfalse()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "20000").IsFalse();
        }

        [TestMethod]
        public void 数字部分が範囲より小さいならfalse()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "9999").IsFalse();
        }

        // ----------------------------------------------------------------
        // 無効ルールの無視
        // ----------------------------------------------------------------

        [TestMethod]
        public void FromとToでプレフィックスが不一致のRuleは無効化されフォールバックになる()
        {
            // A-From / B-To でプレフィックス不一致 -> 有効ルール0件 -> フォールバック
            SetupConfig(fixedLength: 0, ("A10000", "B19999"));

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
            _helper.IsTestPatient(_facilityKey, "15000").IsFalse();
        }

        [TestMethod]
        public void FromとToでサフィックスが不一致のRuleは無効化されフォールバックになる()
        {
            // from suffix X, to suffix Y -> invalid
            SetupConfig(fixedLength: 0, ("10000X", "19999Y"));

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        [TestMethod]
        public void FromとToで記号サフィックスが不一致のRuleは無効化されフォールバックになる()
        {
            // from suffix -X, to suffix -Y -> プレフィックス/サフィックス不一致 -> 無効 -> フォールバック
            SetupConfig(fixedLength: 0, ("10000-X", "19999-Y"));

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        [TestMethod]
        public void 数字パターンに合わない形式のRuleは無効化されフォールバックになる()
        {
            // "INVALID+" は正規表現に合わない -> 無効 -> フォールバック
            SetupRawConfig("{\"PatientIdConfig\":{\"FixedLength\":0,\"TestPatientIdRules\":[{\"PatientIdFrom\":\"INVALID+\",\"PatientIdTo\":\"INVALID+X\"}]}}");

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        // ----------------------------------------------------------------
        // フォールバック (設定不備)
        // ----------------------------------------------------------------

        [TestMethod]
        public void CONFIGSET未登録時に99999フォールバックが有効になる()
        {
            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(_facilityKey))
                .Returns((QH_FACILITYCONFIG_MST)null);

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
            _helper.IsTestPatient(_facilityKey, "10000").IsFalse();
        }

        [TestMethod]
        public void CONFIGSETが空文字の場合にフォールバックが有効になる()
        {
            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(_facilityKey))
                .Returns(new QH_FACILITYCONFIG_MST { CONFIGSET = "" });

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        [TestMethod]
        public void JSON不正時にフォールバックが有効になる()
        {
            SetupRawConfig("{ not valid json }");

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        [TestMethod]
        public void PatientIdConfigがnullの場合にフォールバックが有効になる()
        {
            SetupRawConfig("{\"PatientIdConfig\":null}");

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        [TestMethod]
        public void TestPatientIdRulesが空の場合にフォールバックが有効になる()
        {
            SetupRawConfig("{\"PatientIdConfig\":{\"FixedLength\":0,\"TestPatientIdRules\":[]}}");

            _helper.IsTestPatient(_facilityKey, "99999001").IsTrue();
        }

        [TestMethod]
        public void 設定が正常存在でのルール不一致では99999フォールバックは適用されない()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            // 設定が正常に存在している -> フォールバック不適用
            _helper.IsTestPatient(_facilityKey, "99999001").IsFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Repository例外は握りつぶさず伝播する()
        {
            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(_facilityKey))
                .Throws(new InvalidOperationException("DB接続失敗"));

            _helper.IsTestPatient(_facilityKey, "99999001");
        }

        // ----------------------------------------------------------------
        // 引数バリデーション
        // ----------------------------------------------------------------

        [TestMethod]
        public void 患者IDが空文字の場合はfalse()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "").IsFalse();
        }

        [TestMethod]
        public void 患者IDがwhitespaceの場合はfalse()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "   ").IsFalse();
        }

        [TestMethod]
        public void facilityKeyがGuidEmptyの場合はfalse()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(Guid.Empty, "15000").IsFalse();
        }

        // ----------------------------------------------------------------
        // キャッシュ動作
        // ----------------------------------------------------------------

        [TestMethod]
        public void 同一facilityKeyの2回目以降でRepositoryの呼び出し回数が増えない()
        {
            SetupConfig(fixedLength: 0, ("10000", "19999"));

            _helper.IsTestPatient(_facilityKey, "15000");
            _helper.IsTestPatient(_facilityKey, "15000");
            _helper.IsTestPatient(_facilityKey, "15001");

            _facilityRepo.Verify(r => r.ReadFacilityConfig(_facilityKey), Times.Once);
        }

        [TestMethod]
        public void 異なるfacilityKeyごとに別キャッシュで管理される()
        {
            var key1 = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");
            var key2 = Guid.Parse("22dc3f56-5652-4d08-9147-1575c1723edb");

            var config1 = BuildConfigJson(fixedLength: 0, ("10000", "19999"));
            var config2 = BuildConfigJson(fixedLength: 0, ("20000", "29999"));

            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(key1))
                .Returns(new QH_FACILITYCONFIG_MST { CONFIGSET = config1 });
            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(key2))
                .Returns(new QH_FACILITYCONFIG_MST { CONFIGSET = config2 });

            _helper.IsTestPatient(key1, "15000").IsTrue();
            _helper.IsTestPatient(key1, "25000").IsFalse();

            _helper.IsTestPatient(key2, "25000").IsTrue();
            _helper.IsTestPatient(key2, "15000").IsFalse();

            // それぞれ1回だけ DB アクセスしているはず
            _facilityRepo.Verify(r => r.ReadFacilityConfig(key1), Times.Once);
            _facilityRepo.Verify(r => r.ReadFacilityConfig(key2), Times.Once);
        }

        // ----------------------------------------------------------------
        // ヘルパー
        // ----------------------------------------------------------------

        void SetupConfig(int fixedLength, params (string from, string to)[] rulePairs)
        {
            var json = BuildConfigJson(fixedLength, rulePairs);
            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(_facilityKey))
                .Returns(new QH_FACILITYCONFIG_MST { CONFIGSET = json });
        }

        void SetupRawConfig(string configSetJson)
        {
            _facilityRepo
                .Setup(r => r.ReadFacilityConfig(_facilityKey))
                .Returns(new QH_FACILITYCONFIG_MST { CONFIGSET = configSetJson });
        }

        static string BuildConfigJson(int fixedLength, params (string from, string to)[] rulePairs)
        {
            var rules = new List<QhFacilityTestPatientIdRuleOfJson>();
            foreach (var pair in rulePairs)
            {
                rules.Add(new QhFacilityTestPatientIdRuleOfJson
                {
                    PatientIdFrom = pair.from,
                    PatientIdTo = pair.to,
                });
            }

            var configSet = new QhFacilityConfigSetOfJson
            {
                PatientIdConfig = new QhFacilityPatientIdConfigOfJson
                {
                    FixedLength = fixedLength,
                    TestPatientIdRules = rules,
                },
            };

            return new QsJsonSerializer().Serialize(configSet);
        }
    }
}
