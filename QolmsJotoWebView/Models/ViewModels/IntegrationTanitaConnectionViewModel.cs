using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// ポータルタニタ接続入力モデル
    /// </summary>
    public sealed class IntegrationTanitaConnectionViewModel : IQjModelUpdater<IntegrationTanitaConnectionViewModel>, IValidatableObject
    {
        #region Constant

        /// <summary>
        /// ダミーのセッションIDを表します。
        /// </summary>
        private static readonly string SESSION_ID = new string('Z', 100);

        /// <summary>
        /// ダミーのAPI認証キーを表します。
        /// </summary>
        private static readonly Guid API_AUTHORIZE_KEY = new Guid(new string('F', 32));

        /// <summary>
        /// 漢字姓、漢字名、カナ姓、カナ名、の入力最大文字数を表します。
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
        /// パスワードの最小長を表します。
        /// </summary>
        private const int LENGTH_8 = 8;

        /// <summary>
        /// パスワードの最大長を表します。
        /// </summary>
        private const int LENGTH_32 = 32;

        /// <summary>
        /// パスワードとして使用可能な文字を表す正規表現パターンを表します。
        /// </summary>
        private const string PASSWORD_PATTERN = @"^[A-Za-z0-9!-/:-@≠\[-`{-~]*$";

        #endregion

        #region Public Property

        /// <summary>
        /// ダミーのセッションIDを取得します。
        /// </summary>
        public string SessionId
        {
            get { return IntegrationTanitaConnectionViewModel.SESSION_ID; }
        }

        /// <summary>
        /// ダミーのAPI認証キーを取得します。
        /// </summary>
        public Guid ApiAuthorizeKey
        {
            get { return IntegrationTanitaConnectionViewModel.API_AUTHORIZE_KEY; }
        }

        /// <summary>
        /// IDを取得または設定します。
        /// </summary>
        [DisplayName("ID")]
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// パスワードを取得または設定します。
        /// </summary>
        [DisplayName("パスワード")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 連携IDを取得または設定します。
        /// </summary>
        [DisplayName("連携ID")]
        public string ConnectionID { get; set; } = string.Empty;

        /// <summary>
        /// デバイスのリストを取得または設定します。
        /// </summary>
        [DisplayName("デバイス")]
        public List<DeviceItem> Devices { get; set; } = new List<DeviceItem>();

        /// <summary>
        /// 体組成計のデータ取得するかを取得または設定します。
        /// </summary>
        public bool BodyCompositionMeter { get; set; } = false;

        /// <summary>
        /// 歩数のデータ取得するかを取得または設定します。
        /// </summary>
        public bool Sphygmomanometer { get; set; } = false;

        /// <summary>
        /// 血圧のデータ取得するかを取得または設定します。
        /// </summary>
        public bool Pedometer { get; set; } = false;

        /// <summary>
        /// ALKOOの連携があるかを取得または設定します。
        /// </summary>
        public bool AlkooConnectedFlag { get; set; } = false;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="IntegrationTanitaConnectionViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public IntegrationTanitaConnectionViewModel() : base() { }

        #endregion

        #region Public Method

        /// <summary>
        /// インプットモデルの内容を現在のインスタンスに反映します。
        /// </summary>
        /// <param name="inputModel">インプットモデル。</param>
        public void UpdateByInput(IntegrationTanitaConnectionViewModel inputModel)
        {
            if (inputModel != null)
            {
                this.ID = string.IsNullOrWhiteSpace(inputModel.ID) ? string.Empty : inputModel.ID.Trim();
                this.Password = string.IsNullOrWhiteSpace(inputModel.Password) ? string.Empty : inputModel.Password.Trim();
            }
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

            if (string.IsNullOrWhiteSpace(this.ConnectionID))
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    result.Add(new ValidationResult("IDを入力してください。", new[] { "ID" }));
                }
                else
                {
                    if (this.ID.Length > 64)
                    {
                        result.Add(new ValidationResult("IDは64文字文字以下で入力してください。", new[] { "ID" }));
                    }
                }

                if (string.IsNullOrWhiteSpace(this.Password))
                {
                    result.Add(new ValidationResult("パスワードを入力してください。", new[] { "Password" }));
                }
                else
                {
                    if (this.Password.Length > 16)
                    {
                        result.Add(new ValidationResult("パスワードは16文字文字以下で入力してください。", new[] { "Password" }));
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
