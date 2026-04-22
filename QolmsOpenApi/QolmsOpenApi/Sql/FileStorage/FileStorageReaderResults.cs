
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FileStorageReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// ファイル情報を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DbFileStorageItem FileStorageItem { get; set; } = new DbFileStorageItem();



        /// <summary>
        /// <see cref="FileStorageReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FileStorageReaderResults() : base()
        {
        }
    }


}