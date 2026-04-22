using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設の情報を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NoteMedicineReaderResults : QsDbReaderResultsBase<QH_MEDICINE_DAT>
    {
        /// <summary>
        /// 最終アクセス日時以降にデータが追加・修正・削除されたかどうかを取得または設定します。
        /// </summary>
        public bool IsModified { get; set; } = false;

        /// <summary>
        /// 最終アクセス日時を取得または設定します。
        /// </summary>
        public DateTime LastAccessDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ページインデックスを取得または設定します。
        /// </summary>
        public int PageIndex { get; set; } = int.MinValue;

        /// <summary>
        /// 最大ページインデックスを取得または設定します。
        /// </summary>
        public int MaxPageIndex { get; set; } = int.MinValue;

        /// <summary>
        /// <see cref="NoteMedicineReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineReaderResults() : base()
        {
        }
    }


}