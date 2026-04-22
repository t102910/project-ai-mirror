using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal sealed class LinkagePatientCardWriterResults : QsDbWriterResultsBase
    {

        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 操作キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ActionKey { get; set; } = Guid.Empty;

        /// <summary>
        /// エラーメッセージを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 更新されたEntity
        /// </summary>
        public QH_PATIENTCARD_DAT Entity { get; set; } 

        /// <summary>
        /// <see cref="LinkagePatientCardWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkagePatientCardWriterResults() : base()
        {
        }
    }


}