using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// お薬情報のキー情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class DbMedicineKeyItem
    {


        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        // <summary>
        // 調剤日OR購入日を取得または設定します。
        // </summary>
        // <value></value>
        // <returns></returns>
        // <remarks></remarks>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        // <summary>
        // 日付内連番を取得または設定します。
        // </summary>
        // <value></value>
        // <returns></returns>
        // <remarks></remarks>
        public int Sequence { get; set; } = int.MinValue;

        /// <summary>
        /// データ種別を取得または設定します。
        /// </summary>
        public byte DataType { get; set; } = byte.MinValue;

        /// <summary>
        /// <see cref="DbMedicineKeyItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbMedicineKeyItem()
        {
        }
    }


}