using System;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 健診結果付随 ファイル 情報を表します。
    /// この クラス は継承できません。
    /// </summary>
    [Serializable]
    public sealed class AssociatedFileItem
    {
        #region Public Property

        /// <summary>
        /// 連携 システム 番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 連携 システム ID を取得または設定します。
        /// </summary>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// 健診日を取得または設定します。
        /// </summary>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 施設 キー を取得または設定します。
        /// </summary>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 記録 タイプ を取得または設定します。
        /// </summary>
        public QjExaminationDataTypeEnum DataType { get; set; } = QjExaminationDataTypeEnum.None;

        /// <summary>
        /// データ キーを取得または設定します。
        /// </summary>
        public Guid DataKey { get; set; } = Guid.Empty;

        /// <summary>
        /// URL 連携の場合に使用する キー を取得または設定します。
        /// </summary>
        [Obsolete("未使用です。")]
        public string AdditionalKey { get; set; } = string.Empty;

        /// <summary>
        /// ファイル取得用の承認情報を含むJsonの文字列を取得または設定します。
        /// </summary>
        public string FileStorageReferenceJson { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AssociatedFileItem" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AssociatedFileItem()
        {
        }

        #endregion

        #region Public Method

        /// <summary>
        /// ビュー 内に展開する暗号化された ファイル 情報への参照 パラメータ を生成します。
        /// </summary>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="loginAt">ログイン 日時。</param>
        /// <param name="cryptor">
        /// 暗号化および復号化の機能。
        /// 暗号化の種別は <see cref="QsCryptTypeEnum.QolmsWeb" /> を使用してください。
        /// </param>
        /// <returns>
        /// 暗号化された ファイル 情報への参照 パラメータ。
        /// </returns>
        public string ToEncryptedAssociatedFileStorageReference(Guid accountKey, DateTime loginAt, QsCrypt cryptor)
        {
            if (cryptor == null) throw new ArgumentNullException(nameof(cryptor), "暗号化および復号化の機能が null 参照です。");

            return cryptor.EncryptString(
                new AssociatedFileStorageReferenceJsonParameter
                {
                    Accountkey = accountKey.ToApiGuidString(),
                    LoginAt = loginAt.ToApiDateString(),
                    LinkageSystemNo = this.LinkageSystemNo.ToString(),
                    LinkageSystemId = this.LinkageSystemId,
                    RecordDate = this.RecordDate.ToApiDateString(),
                    FacilityKey = this.FacilityKey.ToApiGuidString(),
                    DataKey = this.DataKey.ToApiGuidString(),
                    AdditionalKey = this.AdditionalKey
                }.ToJsonString()
            );
        }

        /// <summary>
        /// ビュー 内に展開する暗号化された ファイル 情報への参照 パラメータ を生成します。
        /// </summary>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="loginAt">ログイン 日時。</param>
        /// <returns>
        /// 暗号化された ファイル 情報への参照 パラメータ。
        /// </returns>
        public string ToEncryptedAssociatedFileStorageReference(Guid accountKey, DateTime loginAt)
        {
            using (var cryptor = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
            {
                return ToEncryptedAssociatedFileStorageReference(accountKey, loginAt, cryptor);
            }
        }

        #endregion
    }
}
