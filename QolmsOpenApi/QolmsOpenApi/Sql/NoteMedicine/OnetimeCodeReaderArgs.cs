using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{ 
    internal sealed class OnetimeCodeReaderArgs : QsDbReaderArgsBase<QH_ELINKONETIMECODE_DAT>
    {
        #region "Public Property"

        /// <summary>
        /// ワンタイムコードを取得または設定します。
        /// </summary>
        public string OnetimeCode { get; set; } = string.Empty;

        #endregion

        #region "Public Constructor"

        /// <summary>
        /// <see cref="OnetimeCodeReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public OnetimeCodeReaderArgs() : base()
        {
        }
        #endregion
    }
}