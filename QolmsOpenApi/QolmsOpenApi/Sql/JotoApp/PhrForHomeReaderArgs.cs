using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 医療・健康情報ホーム画面用の読み込みパラメータ
    /// </summary>
    internal sealed class PhrForHomeReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        #region "Private Property"
        #endregion

        #region "Public Property"
        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 記録日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime TargetDate { get; set; } = DateTime.MinValue;
        #endregion

        #region "Constructor"
        #endregion

        #region "Private Method"
        #endregion

        #region "Public Method"
        #endregion
    }
}
