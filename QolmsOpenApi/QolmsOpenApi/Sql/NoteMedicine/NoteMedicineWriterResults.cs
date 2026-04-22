using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 
    /// 
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NoteMedicineWriterResults : QsDbWriterResultsBase
    {
        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;


        /// <summary>
        /// 操作キーを取得または設定します。
        /// </summary>
        public Guid ActionKey { get; set; } = Guid.Empty;


        /// <summary>
        /// 登録したデータIDを取得または設定します。
        /// </summary>
        public string DataId { get; set; } = string.Empty;


        /// <summary>
        /// 登録した日付内連番を取得または設定します。
        /// </summary>
        public int Sequence { get; set; } = int.MinValue;


        /// <summary>
        /// <see cref="NoteMedicineWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineWriterResults() : base()
        {
        }
    }


}