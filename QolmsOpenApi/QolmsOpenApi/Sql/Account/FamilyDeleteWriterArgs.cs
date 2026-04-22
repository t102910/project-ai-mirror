using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 家族削除処理の引数クラス
    /// </summary>
    public class FamilyDeleteWriterArgs: QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 親アカウントキー
        /// </summary>
        public Guid ParentAccountKey { get; set; }
        /// <summary>
        /// 削除対象の子アカウントキー
        /// </summary>
        public Guid AccountKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid AuthorKey { get; set; }     
    }
}