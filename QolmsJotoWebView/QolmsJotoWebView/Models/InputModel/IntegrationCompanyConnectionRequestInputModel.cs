using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携申請の入力値を保持し、サーバー側の入力検証を行います。
    /// </summary>
    [Serializable]
    public sealed class IntegrationCompanyConnectionRequestInputModel
        : QjHealthPageViewModelBase, IValidatableObject
    {
        private const int Length25 = 25;
        private const int Length40 = 40;
        private const string IdPattern = "^[A-Za-z0-9]*$";
        private const string KanaPattern = @"^[\p{IsKatakana}\s]*$";
        private const string RequiredErrorMessage = "{0}は必須項目です。";
        private const string LengthErrorMessage = "{0}は{1}文字以下で入力してください。";
        private const string RegularExpressionErrorMessage = "{0}は{1}で入力してください。";

        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.None;

        [DisplayName("企業コード")]
        public string FacilityId { get; set; } = string.Empty;

        [DisplayName("社員番号")]
        public string EmployeeNo { get; set; } = string.Empty;

        [DisplayName("姓")]
        public string FamilyName { get; set; } = string.Empty;

        [DisplayName("名")]
        public string GivenName { get; set; } = string.Empty;

        [DisplayName("カナ姓")]
        public string FamilyKanaName { get; set; } = string.Empty;

        [DisplayName("カナ名")]
        public string GivenKanaName { get; set; } = string.Empty;

        [DisplayName("性別")]
        public QjSexTypeEnum SexType { get; set; } = QjSexTypeEnum.None;

        [DisplayName("生年月日")]
        public string BirthYear { get; set; } = string.Empty;

        [DisplayName("生年月日")]
        public string BirthMonth { get; set; } = string.Empty;

        [DisplayName("生年月日")]
        public string BirthDay { get; set; } = string.Empty;

        [DisplayName("開示許可")]
        public QjRelationContentTypeEnum RelationContentFlags { get; set; } = QjRelationContentTypeEnum.None;

        public bool IdentityUpdateFlag { get; set; } = false;

        public bool CompanyConnectedFlag { get; set; } = false;

        /// <summary>
        /// 企業連携申請の入力ルールを検証します。
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            var rxId = new Regex(IdPattern, RegexOptions.Compiled);
            var rxKana = new Regex(KanaPattern, RegexOptions.Compiled);

            if (string.IsNullOrWhiteSpace(FacilityId))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "企業コード"), new[] { "FacilityId" }));
            }
            else if (FacilityId.Length >= 10)
            {
                result.Add(new ValidationResult(string.Format(LengthErrorMessage, "企業コード", "10"), new[] { "FacilityId" }));
            }

            if (string.IsNullOrWhiteSpace(EmployeeNo))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "社員番号"), new[] { "EmployeeNo" }));
            }
            else
            {
                if (!rxId.IsMatch(EmployeeNo))
                {
                    result.Add(new ValidationResult(string.Format(RegularExpressionErrorMessage, "社員番号", "半角英数"), new[] { "EmployeeNo" }));
                }

                if (EmployeeNo.Length >= Length40)
                {
                    result.Add(new ValidationResult(string.Format(LengthErrorMessage, "社員番号", Length40), new[] { "EmployeeNo" }));
                }
            }

            if (string.IsNullOrWhiteSpace(FamilyName))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "姓"), new[] { "FamilyName" }));
            }
            else if (FamilyName.Length > Length25)
            {
                result.Add(new ValidationResult(string.Format(LengthErrorMessage, "姓", Length25), new[] { "FamilyName" }));
            }

            if (string.IsNullOrWhiteSpace(GivenName))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "名"), new[] { "GivenName" }));
            }
            else if (GivenName.Length > Length25)
            {
                result.Add(new ValidationResult(string.Format(LengthErrorMessage, "名", Length25), new[] { "GivenName" }));
            }

            if (string.IsNullOrWhiteSpace(FamilyKanaName))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "カナ姓"), new[] { "FamilyKanaName" }));
            }
            else
            {
                if (!rxKana.IsMatch(FamilyKanaName))
                {
                    result.Add(new ValidationResult(string.Format(RegularExpressionErrorMessage, "カナ姓", "全角カナ文字"), new[] { "FamilyKanaName" }));
                }

                if (FamilyKanaName.Length > Length25)
                {
                    result.Add(new ValidationResult(string.Format(LengthErrorMessage, "カナ姓", Length25), new[] { "FamilyKanaName" }));
                }
            }

            if (string.IsNullOrWhiteSpace(GivenKanaName))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "カナ名"), new[] { "GivenKanaName" }));
            }
            else
            {
                if (!rxKana.IsMatch(GivenKanaName))
                {
                    result.Add(new ValidationResult(string.Format(RegularExpressionErrorMessage, "カナ名", "全角カナ文字"), new[] { "GivenKanaName" }));
                }

                if (GivenKanaName.Length > Length25)
                {
                    result.Add(new ValidationResult(string.Format(LengthErrorMessage, "カナ名", Length25), new[] { "GivenKanaName" }));
                }
            }

            // 年月日の3項目を1つの日付として検証する。
            if (!int.TryParse(BirthYear, out var year)
                || !int.TryParse(BirthMonth, out var month)
                || !int.TryParse(BirthDay, out var day)
                || year < 0
                || month < 1
                || day < 1
                || !DateTime.TryParseExact(
                    year.ToString("d4") + month.ToString("d2") + day.ToString("d2"),
                    "yyyyMMdd",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out var birthday)
                || birthday > DateTime.Now.Date)
            {
                result.Add(new ValidationResult("生年月日が不正です。", new[] { "BirthYear" }));
            }

            if (RelationContentFlags == QjRelationContentTypeEnum.None)
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "開示許可"), new[] { "RelationContentFlags" }));
            }

            return result;
        }
    }
}