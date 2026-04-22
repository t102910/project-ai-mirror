using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// お知らせ（グループ）情報の読み込みパラメータ
    /// </summary>
    internal sealed class NoticeGroupReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
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
        /// ターゲットタイプを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte TargetType { get; set; } = 0;

        /// <summary>
        /// カテゴリ番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte CategoryNo { get; set; } = 0;

        /// <summary>
        /// 既読状態を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte AlreadyRead { get; set; } = 0;

        /// <summary>
        /// ページ番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PageIndex { get; set; } = int.MinValue;

        /// <summary>
        /// ページサイズを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PageSize { get; set; } = int.MinValue;

        /// <summary>
        /// 取得対象の施設キー一覧を取得または設定します。
        /// </summary>
        public List<Guid> FacilityKeyN { get; set; } = new List<Guid>();
        #endregion

        #region "Constructor"
        #endregion

        #region "Private Method"
        #endregion

        #region "Public Method"
        #endregion
    }
}
