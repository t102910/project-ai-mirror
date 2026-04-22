using MGF.QOLMS.QolmsDbCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
 

    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FileStorageWriterResults : QsDbWriterResultsBase
    {


        /// <summary>
        /// <see cref="FileStorageWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FileStorageWriterResults() : base()
        {
        }
    }


}