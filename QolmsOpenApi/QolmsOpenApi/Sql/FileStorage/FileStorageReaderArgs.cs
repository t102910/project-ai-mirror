using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FileStorageReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// 所有者アカウント キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 対象者アカウント キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ActorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// ファイル キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid FileKey { get; set; } = Guid.Empty;



        /// <summary>
        /// <see cref="FileStorageReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FileStorageReaderArgs() : base()
        {
        }
    }


}