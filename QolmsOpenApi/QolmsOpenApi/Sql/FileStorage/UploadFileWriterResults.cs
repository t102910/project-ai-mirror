using MGF.QOLMS.QolmsDbCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
 

    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class UploadFileWriterResults : QsDbWriterResultsBase
    {


        /// <summary>
        /// <see cref="UploadFileWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadFileWriterResults() : base()
        {
        }
    }


}