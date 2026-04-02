using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public sealed class LoginModel
    {
        #region "public Property"

        /// <summary>
        /// QOLMSの サイト名を取得します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string QolmsSiteName { get => QjConfiguration.QolmsJotoSiteName; }

        public string UserId = string.Empty;

        public string Password = string.Empty;

        public string PasswordHash = string.Empty;

        public bool RememberId = false;

        public bool RememberLogin = false;

        public QsApiLoginResultTypeEnum LoginResultType = QsApiLoginResultTypeEnum.None;

        public string Message = string.Empty;

        #endregion

        #region "Constructor"

        public LoginModel() : base() { }

        #endregion

        #region "Public Method"

        /// <summary>
        /// インプット モデルの内容を現在のインスタンスに反映します。
        /// </summary>
        /// <param name="inputModel">インプット モデル。</param>
        public void UpdateByInput(LoginModel inputModel)
        {
            if (inputModel != null)
            {
                this.UserId = string.IsNullOrWhiteSpace(inputModel.UserId)? string.Empty: inputModel.UserId.Trim();
                this.Password = string.IsNullOrWhiteSpace(inputModel.Password)? string.Empty: inputModel.Password.Trim();
                this.RememberId = inputModel.RememberId;
                this.RememberLogin = inputModel.RememberLogin;
            }
        }

        #endregion
    }
}