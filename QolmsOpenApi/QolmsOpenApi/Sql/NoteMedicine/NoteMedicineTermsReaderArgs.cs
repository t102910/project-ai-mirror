using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{ 
    internal sealed class NoteMedicineTermsReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// System種別を取得または設定します。
        /// </summary>
        public byte SystemType { get; set; } = byte.MinValue;

        #endregion

        #region "Public Constructor"

        /// <summary>
        /// <see cref="NoteMedicineTermsReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineTermsReaderArgs() : base()
        {
        }
        #endregion
    }
}