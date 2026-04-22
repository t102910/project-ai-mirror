    using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// DB 用のファイル情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbFileStorageItem
    {


        /// <summary>
        /// アカウント キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// ファイル キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid FileKey { get; set; } = Guid.Empty;

        /// <summary>
        /// オリジナル ファイル名を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string OriginalName { get; set; } = string.Empty;

        /// <summary>
        /// MIME タイプを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ContentType { get; set; } = string.Empty;



        /// <summary>
        /// <see cref="DbFileStorageItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbFileStorageItem()
        {
        }
    }


}