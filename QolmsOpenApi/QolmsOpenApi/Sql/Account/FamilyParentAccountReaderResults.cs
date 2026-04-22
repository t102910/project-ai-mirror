
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 
    /// </summary>
    internal sealed class FamilyParentAccountReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 親アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 連携システムIDを取得または設定します。
        /// </summary>
        public string LinkageId { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="FamilyParentAccountReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FamilyParentAccountReaderResults() : base()
        {
        }
    }


}