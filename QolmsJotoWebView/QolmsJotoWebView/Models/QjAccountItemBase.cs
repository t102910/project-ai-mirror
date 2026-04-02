using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アカウント情報を持つ基本クラスです。
    /// </summary>
    [Serializable()]
    public abstract class QjAccountItemBase
    {
        #region "public "

        /// <summary>
        /// アカウント キー を取得または設定します。
        /// </summary>
        public Guid AccountKey = Guid.Empty;

        /// <summary>
        /// アカウント キー ハッシュ を取得または設定します。
        /// </summary>
        public string AccountKeyHash = string.Empty;

        /// <summary>
        /// 漢字姓を取得または設定します。
        /// </summary>
        public string FamilyName = string.Empty;

        /// <summary>
        /// 漢字 ミドル ネーム を取得または設定します。
        /// </summary>
        public string MiddleName = string.Empty;

        /// <summary>
        /// 漢字名を取得または設定します。
        /// </summary>
        public string GivenName = string.Empty;

        /// <summary>
        /// カナ 姓を取得または設定します。
        /// </summary>
        public string FamilyKanaName = string.Empty;

        /// <summary>
        /// カナ ミドル ネーム を取得または設定します。
        /// </summary>
        public string MiddleKanaName = string.Empty;

        /// <summary>
        /// カナ 名を取得または設定します。
        /// </summary>
        public string GivenKanaName = string.Empty;

        /// <summary>
        /// ローマ 字姓を取得または設定します。
        /// </summary>
        public string FamilyRomanName = string.Empty;

        /// <summary>
        /// ローマ 字 ミドル ネーム を取得または設定します。
        /// </summary>
        public string MiddleRomanName = string.Empty;

        /// <summary>
        /// ローマ 字名を取得または設定します。
        /// </summary>
        public string GivenRomanName = string.Empty;

        /// <summary>
        /// 性別の種別を取得または設定します。
        /// </summary>
        public QjSexTypeEnum SexType = QjSexTypeEnum.None;

        /// <summary>
        /// 生年月日を取得または設定します。
        /// </summary>
        public DateTime Birthday = DateTime.MinValue;

        /// <summary>
        /// 利用規約同意 フラグ を取得または設定します。
        /// </summary>
        public bool AcceptFlag = false;

        /// <summary>
        /// バイタル 情報の種別および バイタル 標準値の種別を キー、
        /// API 用の バイタル 目標値情報を値とする、
        /// バイタル 標準値情報の ディクショナリ を取得または設定します。
        /// </summary>
        public Dictionary<Tuple<QjVitalTypeEnum, QjStandardValueTypeEnum>, QhApiTargetValueItem> StandardValues = new Dictionary<Tuple<QjVitalTypeEnum, QjStandardValueTypeEnum>, QhApiTargetValueItem>();

        /// <summary>
        /// バイタル 情報の種別および バイタル 標準値の種別を キー、
        /// API 用の バイタル 目標値情報を値とする、
        /// バイタル 目標値情報の ディクショナリ を取得または設定します。
        /// </summary>
        public Dictionary<Tuple<QjVitalTypeEnum, QjStandardValueTypeEnum>, QhApiTargetValueItem> TargetValues = new Dictionary<Tuple<QjVitalTypeEnum, QjStandardValueTypeEnum>, QhApiTargetValueItem>();

        /// <summary>
        /// 身長を取得または設定します。
        /// </summary>
        public decimal Height = decimal.MinValue;

        /// <summary>
        /// ビュー 内に展開する暗号化された アカウント キー を取得または設定します。
        /// </summary>
        public string EncryptedAccountKey = string.Empty;


        /// <summary>
        /// 漢字姓名を取得します。
        /// </summary>
        public string Name { get => this.CreateName(); }

        /// <summary>
        /// カナ 姓名を取得します。
        /// </summary>
        public string KanaName { get => this.CreateKanaName(); }

        /// <summary>
        /// ローマ 字姓名を取得します。
        /// </summary>
        public string RomanName { get => this.CreateRomanName(); }

        /// <summary>
        /// 年齢を取得します。
        /// </summary>get
        public int Age { get => this.ToAge(); }

        #endregion

        #region "Constructor"
        public QjAccountItemBase() : base() { }
        #endregion

        #region "Private Method"

        /// <summary>
        /// 漢字姓名を生成します。
        /// </summary>
        /// <returns>漢字姓名</returns>
        private string CreateName()
        {
            var entries = new List<string>() { this.FamilyName, this.MiddleName, this.GivenName }
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            if (entries.Any())
            {
                return string.Join(" ", entries);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// カナ 姓名を生成します。
        /// </summary>
        /// <returns>カナ 姓名。</returns>
        private string CreateKanaName() 
        {
            var entries = new List<string>() { this.FamilyKanaName, this.MiddleKanaName, this.GivenKanaName }
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            if (entries.Any())
            {
                return string.Join(" ", entries);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ローマ 字姓名を生成します。
        /// </summary>
        /// <returns>ローマ 字姓名。</returns>
        private string CreateRomanName() 
        {
            var entries = new List<string>() { this.FamilyRomanName, this.MiddleRomanName, this.GivenRomanName }
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            if (entries.Any())
            {
                return string.Join(" ", entries);
            }
            else
            {
                return string.Empty;
            }
        }

        //todo:DatetimeProvider
        /// <summary>
        /// 生年月日を年齢へ変換します。
        /// </summary>
        /// <returns>
        /// 成功なら 0 以上の年齢、
        /// 失敗なら Integer.MinValue。
        /// </returns>
        private int ToAge() 
        {
            return DateHelper.GetAge(this.Birthday, DateTime.Now);
        }

        /// <summary>
        /// 生年月日より、指定日における年齢を算出します。
        /// </summary>
        /// <param name="birthday">生年月日。</param>
        /// <param name="oneDay">指定日。</param>
        /// <returns>
        /// 成功なら指定日における年齢、失敗なら int.MinValue。
        /// </returns>
        /// <remarks></remarks>
        internal int GetAgeOn(DateTime oneDay)
        {
            return DateHelper.GetAge(this.Birthday, oneDay);
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// アカウント キー ハッシュ を生成します。
        /// </summary>
        /// <param name="accountKey">アカウント キー。</param>
        /// <returns>アカウント キー ハッシュ。</returns>
        public static string CreateAccountKeyHashString(Guid accountKey)
        {
            var result = new StringBuilder();
            using (var algorithm = new SHA256CryptoServiceProvider())
            {
                algorithm.ComputeHash(accountKey.ToByteArray()).ToList().ForEach(i => result.Append(i.ToString("x2")));
            }

            return result.ToString();
        }

        /// <summary>
        /// ビュー 内に展開する暗号化された アカウント キー を生成します。
        /// </summary>
        /// <param name="accountKey">アカウント キー。</param>
        /// <returns> 暗号化された アカウント キー。</returns>
        public static string EncryptAccountKey(Guid accountKey)
        {
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
            {
                return crypt.EncryptString(accountKey.ToApiGuidString());
            }
        }

        #endregion
    }
}