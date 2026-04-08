using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 病院連携申請の入力値を保持し、サーバー側の入力検証を行います。
    /// <see cref="QolmsJotoModel.SetInputModelCache"/> のキャッシュ対象としても使用します。
    /// </summary>
    [Serializable]
    public sealed class IntegrationHospitalConnectionRequestInputModel : QjHealthPageViewModelBase, IValidatableObject
    {
        private const int Length8 = 8;
        private const int Length12 = 12;
        private const int Length25 = 25;
        private const int Length256 = 256;

        private const string IdPattern = "^[A-Za-z0-9]*$";
        private const string KanaPattern = @"^[\p{IsKatakana}\s]*$";
        private const string MailAddressPattern = @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z";

        public int LinkageSystemNo { get; set; } = int.MinValue;

        public string LinkageSystemId { get; set; } = string.Empty;

        public string FamilyName { get; set; } = string.Empty;

        public string GivenName { get; set; } = string.Empty;

        public string FamilyKanaName { get; set; } = string.Empty;

        public string GivenKanaName { get; set; } = string.Empty;

        public QjSexTypeEnum SexType { get; set; } = QjSexTypeEnum.None;

        public string BirthYear { get; set; } = string.Empty;

        public string BirthMonth { get; set; } = string.Empty;

        public string BirthDay { get; set; } = string.Empty;

        public string MailAddress { get; set; } = string.Empty;

        public bool IdentityUpdateFlag { get; set; } = false;

        public QjRelationContentTypeEnum RelationContentFlags { get; set; } = QjRelationContentTypeEnum.None;

        /// <summary>
        /// 病院連携申請の入力ルールを検証します。
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LinkageSystemNo <= 0)
            {
                yield return new ValidationResult("病院情報が不正です。", new[] { "LinkageSystemNo" });
            }

            if (string.IsNullOrWhiteSpace(LinkageSystemId))
            {
                yield return new ValidationResult("診察券番号は必須です。", new[] { "LinkageSystemId" });
            }
            else
            {
                if (!Regex.IsMatch(LinkageSystemId, IdPattern))
                {
                    yield return new ValidationResult("診察券番号は半角英数で入力してください。", new[] { "LinkageSystemId" });
                }

                int requiredLength = (LinkageSystemNo == 47106 || LinkageSystemNo == 47000016) ? Length12 : Length8;
                if (LinkageSystemId.Length != requiredLength)
                {
                    yield return new ValidationResult(string.Format("診察券番号は{0}文字で入力してください。", requiredLength), new[] { "LinkageSystemId" });
                }
            }

            if (string.IsNullOrWhiteSpace(FamilyName))
            {
                yield return new ValidationResult("姓は必須です。", new[] { "FamilyName" });
            }
            else if (FamilyName.Length > Length25)
            {
                yield return new ValidationResult(string.Format("姓は{0}文字以下で入力してください。", Length25), new[] { "FamilyName" });
            }

            if (string.IsNullOrWhiteSpace(GivenName))
            {
                yield return new ValidationResult("名は必須です。", new[] { "GivenName" });
            }
            else if (GivenName.Length > Length25)
            {
                yield return new ValidationResult(string.Format("名は{0}文字以下で入力してください。", Length25), new[] { "GivenName" });
            }

            if (string.IsNullOrWhiteSpace(FamilyKanaName))
            {
                yield return new ValidationResult("カナ姓は必須です。", new[] { "FamilyKanaName" });
            }
            else
            {
                if (!Regex.IsMatch(FamilyKanaName, KanaPattern))
                {
                    yield return new ValidationResult("カナ姓は全角カナ文字で入力してください。", new[] { "FamilyKanaName" });
                }

                if (FamilyKanaName.Length > Length25)
                {
                    yield return new ValidationResult(string.Format("カナ姓は{0}文字以下で入力してください。", Length25), new[] { "FamilyKanaName" });
                }
            }

            if (string.IsNullOrWhiteSpace(GivenKanaName))
            {
                yield return new ValidationResult("カナ名は必須です。", new[] { "GivenKanaName" });
            }
            else
            {
                if (!Regex.IsMatch(GivenKanaName, KanaPattern))
                {
                    yield return new ValidationResult("カナ名は全角カナ文字で入力してください。", new[] { "GivenKanaName" });
                }

                if (GivenKanaName.Length > Length25)
                {
                    yield return new ValidationResult(string.Format("カナ名は{0}文字以下で入力してください。", Length25), new[] { "GivenKanaName" });
                }
            }

            if (SexType == QjSexTypeEnum.None || SexType == QjSexTypeEnum.Other)
            {
                yield return new ValidationResult("性別が不正です。", new[] { "SexType" });
            }

            DateTime birthday;
            if (!int.TryParse(BirthYear, out int y)
                || !int.TryParse(BirthMonth, out int m)
                || !int.TryParse(BirthDay, out int d)
                || y < 1
                || m < 1
                || d < 1
                || !DateTime.TryParseExact(y.ToString("d4") + m.ToString("d2") + d.ToString("d2"), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out birthday)
                || birthday.Date > DateTime.Today)
            {
                yield return new ValidationResult("生年月日が不正です。", new[] { "BirthYear" });
            }

            if (string.IsNullOrWhiteSpace(MailAddress))
            {
                yield return new ValidationResult("メールアドレスは必須です。", new[] { "MailAddress" });
            }
            else
            {
                var mail = new EmailAddressAttribute();
                if (!mail.IsValid(MailAddress) || !Regex.IsMatch(MailAddress, MailAddressPattern, RegexOptions.IgnoreCase))
                {
                    yield return new ValidationResult("有効なメールアドレス形式で入力してください。", new[] { "MailAddress" });
                }

                if (MailAddress.Length > Length256)
                {
                    yield return new ValidationResult(string.Format("メールアドレスは{0}文字以下で入力してください。", Length256), new[] { "MailAddress" });
                }
            }

            if (RelationContentFlags == QjRelationContentTypeEnum.None)
            {
                yield return new ValidationResult("開示許可は必須です。", new[] { "RelationContentFlags" });
            }
        }
    }
}
