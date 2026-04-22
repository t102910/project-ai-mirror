using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{ 
    internal sealed class NoteMedicineReaderArgs : QsDbReaderArgsBase<QH_MEDICINE_DAT>
    {
        #region "Public Property"

        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// データ種別を取得または設定します。
        /// </summary>
        public byte DataType { get; set; } = byte.MinValue;

        /// <summary>
        /// 最終アクセス日時を取得または設定します。
        /// </summary>
        public DateTime LastAccessDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ページインデックスを取得または設定します。
        /// </summary>
        public int PageIndex { get; set; } = int.MinValue;

        /// <summary>
        /// ページサイズを取得または設定します。
        /// </summary>
        public int PageSize { get; set; } = int.MinValue;

        #endregion

        #region "Public Constructor"

        /// <summary>
        /// <see cref="NoteMedicineReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineReaderArgs() : base()
        {
        }
        #endregion
    }
}