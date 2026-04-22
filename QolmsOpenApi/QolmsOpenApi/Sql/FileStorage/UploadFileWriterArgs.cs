
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
    internal sealed class UploadFileWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        public QH_UPLOADFILE_DAT Entity { get; set; }

        /// <summary>
        /// <see cref="UploadFileWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadFileWriterArgs() : base()
        {
        }
    }


}