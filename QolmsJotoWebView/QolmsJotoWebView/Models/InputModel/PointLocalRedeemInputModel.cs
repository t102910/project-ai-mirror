using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「地域ポイント履歴」画面のポイント変換入力モデルを表します。
    /// </summary>
    [Serializable]
    public sealed class PointLocalRedeemInputModel : IValidatableObject
    {
        #region Constant

        private const string REQUIRED_ERROR_MESSAGE = "{0}を入力してください。";
        private const string REGULAREXPRESSION_ERROR_MESSAGE = "{0}は数字で入力してください。";

        #endregion

        /// <summary>
        /// 変換するポイント数を取得または設定します。
        /// </summary>
        [DisplayName("変換ポイント")]
        public string RedeemPoint { get; set; } = string.Empty;

        #region IValidatableObject Support

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(RedeemPoint))
            {
                result.Add(new ValidationResult(
                    string.Format(REQUIRED_ERROR_MESSAGE, "変換ポイント"),
                    new[] { nameof(RedeemPoint) }
                ));
            }
            else if (!Regex.IsMatch(RedeemPoint, "^[0-9]+$"))
            {
                result.Add(new ValidationResult(
                    string.Format(REGULAREXPRESSION_ERROR_MESSAGE, "変換ポイント"),
                    new[] { nameof(RedeemPoint) }
                ));
            }

            return result;
        }

        #endregion
    }
}
