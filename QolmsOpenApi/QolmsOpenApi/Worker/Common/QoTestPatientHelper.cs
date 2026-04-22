using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// テスト患者判定ヘルパー。
    /// 施設設定に基づいて患者 ID がテスト患者かどうかを判定する。
    /// キャッシュはインスタンス生存期間中のみ有効。
    /// </summary>
    public class QoTestPatientHelper
    {
        readonly IFacilityRepository _facilityRepository;
        readonly Dictionary<Guid, PatientIdCacheEntry> _cache = new Dictionary<Guid, PatientIdCacheEntry>();
        readonly object _cacheLock = new object();

        static readonly Regex IdPatternRegex = new Regex(@"^(\D*)(\d+)(\D*)$", RegexOptions.Compiled);

        /// <summary>
        /// <see cref="QoTestPatientHelper"/> クラスの新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="facilityRepository">施設リポジトリ。</param>
        public QoTestPatientHelper(IFacilityRepository facilityRepository)
        {
            _facilityRepository = facilityRepository;
        }

        /// <summary>
        /// 患者 ID がテスト患者かどうかを判定する。
        /// </summary>
        /// <remarks>
        /// 施設設定が正常に存在する場合はルール判定のみを行い、99999 フォールバックは適用しない。
        /// 設定の未登録・JSON 不正・ルールなしの場合は 99999 プレフィックスで判定する。
        /// </remarks>
        /// <param name="facilityKey">施設キー。</param>
        /// <param name="patientId">患者 ID。</param>
        /// <returns>テスト患者なら true。</returns>
        public bool IsTestPatient(Guid facilityKey, string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return false;
            }

            if (facilityKey == Guid.Empty)
            {
                return false;
            }

            var entry = GetOrLoadCacheEntry(facilityKey);

            if (entry.IsFallbackMode)
            {
                return patientId.StartsWith("99999");
            }

            if (entry.FixedLength > 0 && patientId.Length != entry.FixedLength)
            {
                return false;
            }

            return MatchesAnyRule(patientId, entry.Rules);
        }

        PatientIdCacheEntry GetOrLoadCacheEntry(Guid facilityKey)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(facilityKey, out var cached))
                {
                    return cached;
                }

                var entry = LoadCacheEntry(facilityKey);
                _cache[facilityKey] = entry;
                return entry;
            }
        }

        PatientIdCacheEntry LoadCacheEntry(Guid facilityKey)
        {
            // Repository 例外はここで握らず上位に伝播させる。
            var entity = _facilityRepository.ReadFacilityConfig(facilityKey);
            if (entity == null || string.IsNullOrWhiteSpace(entity.CONFIGSET))
            {
                return new PatientIdCacheEntry { IsFallbackMode = true };
            }

            // JSON 不正・設定解釈エラーのみフォールバック対象とする。
            try
            {
                var configSet = new QsJsonSerializer().Deserialize<QhFacilityConfigSetOfJson>(entity.CONFIGSET);
                if (configSet == null || configSet.PatientIdConfig == null)
                {
                    return new PatientIdCacheEntry { IsFallbackMode = true };
                }

                var patientIdConfig = configSet.PatientIdConfig;
                if (patientIdConfig.TestPatientIdRules == null || patientIdConfig.TestPatientIdRules.Count == 0)
                {
                    return new PatientIdCacheEntry { IsFallbackMode = true };
                }

                var rules = NormalizeRules(patientIdConfig.TestPatientIdRules);
                if (rules.Count == 0)
                {
                    return new PatientIdCacheEntry { IsFallbackMode = true };
                }

                return new PatientIdCacheEntry
                {
                    IsFallbackMode = false,
                    FixedLength = patientIdConfig.FixedLength,
                    Rules = rules
                };
            }
            catch
            {
                return new PatientIdCacheEntry { IsFallbackMode = true };
            }
        }

        static List<NormalizedRule> NormalizeRules(List<QhFacilityTestPatientIdRuleOfJson> rules)
        {
            var result = new List<NormalizedRule>();

            foreach (var rule in rules)
            {
                if (string.IsNullOrWhiteSpace(rule.PatientIdFrom) || string.IsNullOrWhiteSpace(rule.PatientIdTo))
                {
                    continue;
                }

                var fromMatch = IdPatternRegex.Match(rule.PatientIdFrom.Trim());
                var toMatch = IdPatternRegex.Match(rule.PatientIdTo.Trim());

                if (!fromMatch.Success || !toMatch.Success)
                {
                    continue;
                }

                var fromPrefix = fromMatch.Groups[1].Value.ToUpperInvariant();
                var fromSuffix = fromMatch.Groups[3].Value.ToUpperInvariant();
                var toPrefix = toMatch.Groups[1].Value.ToUpperInvariant();
                var toSuffix = toMatch.Groups[3].Value.ToUpperInvariant();

                if (fromPrefix != toPrefix || fromSuffix != toSuffix)
                {
                    continue;
                }

                if (!long.TryParse(fromMatch.Groups[2].Value, out var fromNum) ||
                    !long.TryParse(toMatch.Groups[2].Value, out var toNum))
                {
                    continue;
                }

                result.Add(new NormalizedRule
                {
                    Prefix = fromPrefix,
                    Suffix = fromSuffix,
                    NumericFrom = fromNum,
                    NumericTo = toNum,
                });
            }

            return result;
        }

        static bool MatchesAnyRule(string patientId, List<NormalizedRule> rules)
        {
            var idMatch = IdPatternRegex.Match(patientId.Trim());
            if (!idMatch.Success)
            {
                return false;
            }

            var idPrefix = idMatch.Groups[1].Value.ToUpperInvariant();
            var idSuffix = idMatch.Groups[3].Value.ToUpperInvariant();

            if (!long.TryParse(idMatch.Groups[2].Value, out var idNum))
            {
                return false;
            }

            foreach (var rule in rules)
            {
                if (idPrefix == rule.Prefix &&
                    idSuffix == rule.Suffix &&
                    idNum >= rule.NumericFrom &&
                    idNum <= rule.NumericTo)
                {
                    return true;
                }
            }

            return false;
        }

        class PatientIdCacheEntry
        {
            public bool IsFallbackMode { get; set; }
            public int FixedLength { get; set; }
            public List<NormalizedRule> Rules { get; set; } = new List<NormalizedRule>();
        }

        class NormalizedRule
        {
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public long NumericFrom { get; set; }
            public long NumericTo { get; set; }
        }
    }
}
