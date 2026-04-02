using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class PortalLocalIdVerificationRegisterInputModel
        : QjHealthPageViewModelBase, IValidatableObject
    {
        #region Constant

        /// <summary>
        /// 診察券番号の入力文字数を表します。
        /// </summary>
        private const int LENGTH_8 = 8;

        /// <summary>
        /// 診察券番号の入力文字数を表します。
        /// </summary>
        private const int LENGTH_12 = 12;

        /// <summary>
        /// 漢字姓、
        /// 漢字名、
        /// カナ姓、
        /// カナ名、
        /// の入力最大文字数を表します。
        /// </summary>
        private const int LENGTH_25 = 25;

        /// <summary>
        /// ユーザーIDの入力最大文字数を表します。
        /// </summary>
        private const int LENGTH_100 = 100;

        /// <summary>
        /// メールアドレスの入力最大文字数を表します。
        /// </summary>
        private const int LENGTH_256 = 256;

        /// <summary>
        /// IDとして使用可能な文字を表す正規表現パターンを表します。
        /// </summary>
        private const string ID_PATTERN = "^[A-Za-z0-9]*$";

        /// <summary>
        /// カナとして使用可能な文字を表す正規表現パターンを表します。
        /// </summary>
        private const string KANA_PATTERN = @"^[\p{IsKatakana}\s]*$";

        /// <summary>
        /// メールアドレスとして使用可能な文字を表す正規表現パターンを表します。
        /// </summary>
        private const string MAILADDRESS_PATTERN = @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z";

        /// <summary>
        /// 必須項目のエラー文字列を表します
        /// </summary>
        private const string REQUIRED_ERRORMESSAGE = "{0}は必須項目です。";

        /// <summary>
        /// 文字列長のエラー文字列を表します
        /// </summary>
        private const string ID_LENGTH_ERRORMESSAGE =
            "{0}は{1}文字で入力してください。{1}文字未満の場合は先頭を0で埋めて入力してください。";

        /// <summary>
        /// 文字列長のエラー文字列を表します
        /// </summary>
        private const string LENGTH_ERRORMESSAGE = "{0}は{1}文字以下で入力してください。";

        /// <summary>
        /// 正規表現のエラー文字列を表します
        /// </summary>
        private const string REGULAREXPRESSION_ERRORMESSAGE = "{0}は{1}で入力してください。";

        /// <summary>
        /// その他のエラー文字列を表します
        /// </summary>
        private const string OTHERS_ERRORMESSAGE = "{0}は{1}で入力してください。";

        /// <summary>
        /// メールアドレスの形式エラー文字列を表します
        /// </summary>
        private const string MAILADDRESS_ERRORMESSAGE = "有効なメールアドレス形式で入力してください。";

        #endregion

        #region Public Property

        /// <summary>
        /// 遷移元の画面番号の種別を取得または設定します。
        /// </summary>
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.None;

        /// <summary>
        /// 連携システム番号 を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 連携システム ID を取得または設定します。
        /// </summary>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// 連携ステータス を取得または設定します。
        /// </summary>
        public byte Status { get; set; } = byte.MinValue;

        /// <summary>
        /// 電話番号を取得または設定します。
        /// </summary>
        [DisplayName("電話番号")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// メールアドレスを取得または設定します。
        /// </summary>
        [DisplayName("メールアドレス")]
        public string MailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 個人情報の更新を取得または設定します。
        /// </summary>
        public bool IdentityUpdateFlag { get; set; } = false;

        /// <summary>
        /// 連携種別フラグを取得または設定します。
        /// </summary>
        [DisplayName("開示許可")]
        public QjRelationContentTypeEnum RelationContentFlags { get; set; }
            = QjRelationContentTypeEnum.None;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PortalLocalIdVerificationRegisterInputModel"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PortalLocalIdVerificationRegisterInputModel()
            : base()
        {
        }

        #endregion

        #region IValidatableObject Support

        /// <summary>
        /// 指定されたオブジェクトが有効かどうかを判断します。
        /// </summary>
        /// <param name="validationContext">検証コンテキスト。</param>
        /// <returns>失敗した検証の情報を保持するコレクション。</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            // 電話番号
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                result.Add(new ValidationResult(
                    string.Format(REQUIRED_ERRORMESSAGE, "電話番号"),
                    new[] { "model.PhoneNumber" }));
            }
            else
            {
                // 数値＋桁数（10か11）
                if (!Regex.IsMatch(PhoneNumber, "^[0-9]+$"))
                {
                    result.Add(new ValidationResult(
                        string.Format(REGULAREXPRESSION_ERRORMESSAGE, "電話番号", "数字のみ"),
                        new[] { "model.PhoneNumber" }));
                }

                if (PhoneNumber.Length < 10 || PhoneNumber.Length > 11)
                {
                    result.Add(new ValidationResult(
                        "市外局番を含む10桁または11桁で入力してください。",
                        new[] { "model.PhoneNumber" }));
                }
            }

            // メールアドレス
            if (string.IsNullOrWhiteSpace(MailAddress))
            {
                result.Add(new ValidationResult(
                    string.Format(REQUIRED_ERRORMESSAGE, "メールアドレス"),
                    new[] { "model.MailAddress" }));
            }
            else
            {
                var mail = new EmailAddressAttribute();

                if (!mail.IsValid(MailAddress) ||
                    !Regex.IsMatch(MailAddress, MAILADDRESS_PATTERN, RegexOptions.IgnoreCase))
                {
                    result.Add(new ValidationResult(
                        MAILADDRESS_ERRORMESSAGE,
                        new[] { "model.MailAddress" }));
                }

                if (MailAddress.Length > LENGTH_256)
                {
                    result.Add(new ValidationResult(
                        string.Format(LENGTH_ERRORMESSAGE, "メールアドレス", LENGTH_256),
                        new[] { "model.MailAddress" }));
                }
            }

            if (RelationContentFlags == QjRelationContentTypeEnum.None)
            {
                result.Add(new ValidationResult(
                    string.Format(REQUIRED_ERRORMESSAGE, "基本情報の開示"),
                    new[] { "model.RelationContentFlags" }));
            }

            return result;
        }

        #endregion
    }
}