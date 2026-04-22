using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 
    /// 
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NoteMedicineTermsReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// 利用規約情報を取得または設定します。
        /// </summary>
        public DbTermsItem TermsItem { get; set; } = new DbTermsItem();

        /// <summary>
        /// <see cref="NoteMedicineTermsReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineTermsReaderResults() : base()
        {
        }
    }


}