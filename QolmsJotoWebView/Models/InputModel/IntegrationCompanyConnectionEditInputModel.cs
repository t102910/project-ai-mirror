using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携編集の入力値を保持し、サーバー側の入力検証を行います。
    /// </summary>
    [Serializable]
    public sealed class IntegrationCompanyConnectionEditInputModel
        : QjHealthPageViewModelBase, IValidatableObject
    {
        private const int Length256 = 256;
        private const string MailAddressPattern = @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z";
        private const string RequiredErrorMessage = "{0}は必須項目です。";
        private const string LengthErrorMessage = "{0}は{1}文字以下で入力してください。";
        private const string MailAddressErrorMessage = "有効なメールアドレス形式で入力してください。";

        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.None;

        public int LinkageSystemNo { get; set; } = int.MinValue;

        public string LinkageSystemId { get; set; } = string.Empty;

        public string LinkageSystemName { get; set; } = string.Empty;

        [DisplayName("開示許可")]
        public QjRelationContentTypeEnum RelationContentFlags { get; set; } = QjRelationContentTypeEnum.None;

        [DisplayName("企業連絡用メールアドレス")]
        public string MailAddress { get; set; } = string.Empty;

        public bool CompanyConnectedFlag { get; set; } = false;

        /// <summary>
        /// メールアドレスと開示許可の入力ルールを検証します。
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(MailAddress))
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "メールアドレス"), new[] { "MailAddress" }));
            }
            else
            {
                var mail = new EmailAddressAttribute();

                if (!mail.IsValid(MailAddress) || !Regex.IsMatch(MailAddress, MailAddressPattern, RegexOptions.IgnoreCase))
                {
                    result.Add(new ValidationResult(MailAddressErrorMessage, new[] { "MailAddress" }));
                }

                if (MailAddress.Length > Length256)
                {
                    result.Add(new ValidationResult(string.Format(LengthErrorMessage, "メールアドレス", Length256), new[] { "MailAddress" }));
                }
            }

            if (RelationContentFlags == QjRelationContentTypeEnum.None)
            {
                result.Add(new ValidationResult(string.Format(RequiredErrorMessage, "開示許可"), new[] { "RelationContentFlags" }));
            }

            return result;
        }
    }
}