using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{ 
    internal sealed class NoteMedicineDeleterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// 所有者アカウントキーを取得または設定します。
        /// </summary>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 対象者アカウントキーを取得または設定します。
        /// </summary>
        public Guid ActorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// お薬手帳データIDを取得または設定します。
        /// </summary>
        public string DataId { get; set; } = string.Empty;

        #endregion

        #region "Public Constructor"

        /// <summary>
        /// <see cref="NoteMedicineDeleterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineDeleterArgs() : base()
        {
        }
        #endregion
    }
}