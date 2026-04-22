using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_MEDISMORPHEME_MST のスコア情報を含むVIEW
    /// (実際にDBにVIEWを作成しているわけではなくあくまでクエリ結果格納用）
    /// </summary>
    public class QH_MEDISMORPHEME_SCORE_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// 参照コード
        /// </summary>
        public string REFERENCECODE { get; set; } = string.Empty;

        /// <summary>
        /// 索引語
        /// </summary>
        public string INDEXTERM { get; set; } = string.Empty;

        /// <summary>
        /// 読み
        /// </summary>
        public string READING { get; set; } = string.Empty;

        /// <summary>
        /// 優先度
        /// </summary>
        public decimal PRIORITY { get; set; } = decimal.MinValue;

        /// <summary>
        /// スコア
        /// </summary>
        public int SCORE { get; set; } = int.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                REFERENCECODE = reader.GetString(0);
                INDEXTERM = reader.GetString(1);
                READING = reader.GetString(2);
                PRIORITY = reader.GetDecimal(3);
                SCORE = reader.GetInt32(4);

                KeyGuid = Guid.NewGuid();
                DataState = QsDbEntityStateTypeEnum.Unchanged;
                IsEmpty = false;
            }
            catch
            {
                throw;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsKeysValid()
        {
            return true;
        }
    }
}