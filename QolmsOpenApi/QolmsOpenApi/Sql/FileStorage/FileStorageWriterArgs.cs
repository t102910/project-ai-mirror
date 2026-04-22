
using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{
   
    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FileStorageWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
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
        /// <see cref="FileStorageWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FileStorageWriterArgs() : base()
        {
        }
    }


}