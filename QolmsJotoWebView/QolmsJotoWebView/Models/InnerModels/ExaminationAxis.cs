using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 検査結果表の列項目もしくは行項目を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class ExaminationAxis
    {
        #region Constant

        /// <summary>
        /// 検査項目コードを表す正規表現パターンです。
        /// 医誠会ローカルコード版
        /// </summary>
        private static readonly Regex regexLocalCode = new Regex("^[0-9]{6}$", RegexOptions.IgnoreCase);

        private static readonly Regex regexCode = new Regex("^[0-9A-Z]{17}$", RegexOptions.IgnoreCase);

        #endregion

        #region Variable

        /// <summary>
        /// 列または行を示すキー値を保持します。
        /// 列の場合は「検査実施年月日（YYYYMMDD）_日付内連番」、行の場合は「検査項目コード」になります。
        /// </summary>
        private string _key = string.Empty;

        #endregion

        #region Public Property

        /// <summary>
        /// 列または行を示すキー値を取得します。
        /// 列の場合は「検査実施年月日（YYYYMMDD）_日付内連番」、行の場合は「検査項目コード」になります。
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// ヘッダに表示する1つ目の値を取得または設定します。
        /// 列の場合は「検査実施機関名」、
        /// 行の場合は 種別（グループ）列：「種別名」、項目列：「検査項目名」になります。
        /// </summary>
        public string Header1 { get; set; } = string.Empty;

        /// <summary>
        /// ヘッダに表示する2つ目の値を取得または設定します。
        /// 列の場合は「検査実施日」になります。
        /// </summary>
        public string Header2 { get; set; } = string.Empty;

        /// <summary>
        /// ヘッダに表示する単位を取得または設定します。
        /// </summary>
        public string HeaderUnit { get; set; } = string.Empty;

        /// <summary>
        /// 標準の単位以外を保持しているかを取得または設定します。
        /// </summary>
        public bool HasDifferentUnit { get; set; } = false;

        /// <summary>
        /// ヘッダに表示する基準値を取得または設定します。
        /// </summary>
        public string HeaderStandardValue { get; set; } = string.Empty;

        /// <summary>
        /// 基準値範囲外の結果を保持しているかを取得または設定します。
        /// </summary>
        public bool HasAbnormalValue { get; set; } = false;

        /// <summary>
        /// 検査手帳付随 ファイル 情報の リスト を取得または設定します。
        /// 列の場合のみ有効です。
        /// </summary>
        [Obsolete("実装中")]
        public List<AssociatedFileItem> AssociatedFileN { get; set; } = new List<AssociatedFileItem>();

        /// <summary>
        /// 検査所見・判定 の リスト を取得または設定します。
        /// 列の場合のみ有効です。
        /// </summary>
        public Dictionary<string, ExaminationJudgementItem> ExaminationJudgementN { get; set; } = new Dictionary<string, ExaminationJudgementItem>();

        /// <summary>
        /// 健康年齢を取得または設定します。
        /// 列の場合のみ有効です。
        /// </summary>
        public decimal HealthAge { get; set; } = decimal.MinValue;

        /// <summary>
        /// コメントを取得または設定します。
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        // ''' <summary>
        // ''' 健康年齢の測定対象かどうかを取得または設定します。
        // ''' 列の場合のみ有効です。
        // ''' </summary>
        // public bool HealthAgeCalcFlag { get; set; } = false;

        // ''' <summary>
        // ''' 健康年齢の測定対象の場合に測定に必要な数値を取得または設定します。
        // ''' 列の場合のみ有効です。
        // ''' </summary>
        // public Dictionary<string, string> HealthAgeCalcN { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationAxis" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ExaminationAxis()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="ExaminationAxis" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="key">
        /// 列または行を示すキー値。
        /// 列の場合は「検査実施年月日（YYYYMMDD）_日付内連番」、行の場合は「検査項目コード」になります。
        /// </param>
        /// <param name="header1">
        /// ヘッダに表示する1つ目の値。
        /// 列の場合は「検査実施機関名」、行の場合は「検査項目名」になります。
        /// </param>
        /// <param name="header2">
        /// ヘッダに表示する2つ目の値。
        /// 列の場合は「検査実施年月日（yyyy/M/d）」になります。
        /// </param>
        /// <param name="headerUnit">ツールチップで表示する単位。</param>
        /// <param name="headerSV">ツールチップで表示する基準値。</param>
        /// <param name="healthAge">健康年齢。</param>
        /// <param name="comment">コメント。</param>
        /// <param name="associatedFileN">
        /// 検査手帳付随 ファイル 情報の リスト を取得または設定します（オプショナル、デフォルト = Nothing）。
        /// 列の場合のみ指定してください。
        /// </param>
        /// <param name="examinationJudgementN">
        /// 検査所見・判定 の リスト を取得または設定します（オプショナル、デフォルト = Nothing）。
        /// 列の場合のみ指定してください。
        /// </param>
        [Obsolete("実装中")]
        public ExaminationAxis(
            string key,
            string header1,
            string header2,
            string headerUnit,
            string headerSV,
            decimal healthAge,
            string comment,
            List<AssociatedFileItem> associatedFileN = null,
            Dictionary<string, ExaminationJudgementItem> examinationJudgementN = null
        )
        {
            _key = key;
            Header1 = header1;
            Header2 = header2;
            HeaderUnit = headerUnit;
            HeaderStandardValue = headerSV;
            HealthAge = healthAge;
            Comment = comment;

            if (associatedFileN != null && associatedFileN.Any())
            {
                AssociatedFileN = associatedFileN;
            }
            else
            {
                AssociatedFileN = new List<AssociatedFileItem>();
            }

            if (examinationJudgementN != null && examinationJudgementN.Any())
            {
                ExaminationJudgementN = examinationJudgementN;
            }
            else
            {
                ExaminationJudgementN = new Dictionary<string, ExaminationJudgementItem>();
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 列キーとして使用可能な文字列かチェックします。
        /// </summary>
        /// <param name="fileName">健診CDAファイル名。</param>
        /// <param name="key">列キーとして使用可能な文字列が格納される変数。</param>
        /// <returns>
        /// 使用可能ならTrue、
        /// 使用不可能ならFalse。
        /// </returns>
        public static bool IsColKey(string fileName, ref string key)
        {
            //Dim result As Boolean = False

            key = string.Empty;

            key = fileName;

            //If Not String.IsNullOrWhiteSpace(fileName) Then
            //    Dim parts() As String = IO.Path.GetFileNameWithoutExtension(fileName).Split({"_"}, StringSplitOptions.None)
            //    Dim day As Integer = 0
            //    Dim seq As Integer = 0
            //    Dim id As Byte = 0
            //
            //    If parts.Count() = 10 AndAlso Integer.TryParse(parts(1), day) AndAlso day >= 10101 AndAlso day <= 99991231 AndAlso Integer.TryParse(parts(8), seq) AndAlso Byte.TryParse(parts(9), id) Then
            //        key = String.Format("{0:d8}_{1}_{2}", day, seq, id)
            //        result = True
            //    End If
            //End If

            return true;
        }

        /// <summary>
        /// 行キーとして使用可能な文字列かチェックします。
        /// </summary>
        /// <param name="code">検査項目コード。</param>
        /// <param name="key">行キーとして使用可能な文字列が格納される変数。</param>
        /// <returns>
        /// 使用可能ならTrue、
        /// 使用不可能ならFalse。
        /// </returns>
        public static bool IsRowKey(string code, ref string key)
        {
            bool result = false;

            key = string.Empty;

            if (!string.IsNullOrWhiteSpace(code) && (regexCode.IsMatch(code) || regexLocalCode.IsMatch(code)))
            {
                key = code;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// インスタンスのディープコピーを作成します。
        /// </summary>
        /// <returns>
        /// このインスタンスをディープコピーしたオブジェクト。
        /// </returns>
        public ExaminationAxis Copy()
        {
            ExaminationAxis result = null;

            using (var stream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);

                stream.Position = 0;

                result = (ExaminationAxis)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }

            return result;
        }

        #endregion
    }
}
