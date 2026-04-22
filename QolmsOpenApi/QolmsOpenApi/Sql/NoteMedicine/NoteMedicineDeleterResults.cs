using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 
    /// 
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NoteMedicineDeleterResults : QsDbWriterResultsBase
    {
        #region "Public Property"

        /// <summary>
        /// 削除した薬のキー情報を取得または設定します。
        /// </summary>
        public List<DbMedicineKeyItem> DeletedKeys { get; set; } = new List<DbMedicineKeyItem>();


        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;


        /// <summary>
        /// 操作キーを取得または設定します。
        /// </summary>
        public Guid ActionKey { get; set; } = Guid.Empty;

        #endregion

        #region "Public Constructor"
        /// <summary>
        /// <see cref="NoteMedicineDeleterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineDeleterResults() : base()
        {
        }

        #endregion
    }


}