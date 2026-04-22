using System;

using MGF.QOLMS.QolmsDbCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal sealed class LinkageWriterResults : QsDbWriterResultsBase
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
        /// <see cref="LinkageWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageWriterResults() : base()
        {
        }
    }


}