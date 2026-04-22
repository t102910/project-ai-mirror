using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{ 
    internal sealed class NoteMedicineWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        #region "Public Property"

        /// <summary>
        /// 所有者アカウントキーを取得または設定します。
        /// </summary>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 対象者アカウントキーを取得または設定します。
        /// </summary>
        public Guid ActorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 評価日を取得または設定します。
        /// </summary>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 日付け内連番を取得または設定します。
        /// </summary>
        public int Sequence { get; set; } = int.MinValue;

        /// <summary>
        /// 記録タイプを取得または設定します。
        /// </summary>
        public byte DataType { get; set; } = byte.MinValue;

        /// <summary>
        /// 情報提供元タイプを取得または設定します。
        /// </summary>
        public int OwnerType { get; set; } = int.MinValue;

        /// <summary>
        /// 情報提供元を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 薬局番号を取得または設定します。
        /// </summary>
        public int PharmacyNo { get; set; } = int.MinValue;

        /// <summary>
        /// 薬局受付番号を取得または設定します。
        /// </summary>
        public string ReceiptNo { get; set; } = string.Empty;

        /// <summary>
        /// 医療機関番号を取得または設定します。
        /// </summary>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 処方日を取得または設定します。
        /// </summary>
        public DateTime PrescriptionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 服用開始日（Min)を取得または設定します。
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 服用終了日（Max)を取得または設定します。
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 提供元ファイル名を取得または設定します。
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;
        /// <summary>
        /// お薬手帳セットを取得または設定します。
        /// </summary>
        public string MedicineSet { get; set; } = string.Empty;
        /// <summary>
        /// コルムス共通お薬手帳セットを取得または設定します。
        /// </summary>
        public string ConvertedMedicineSet { get; set; } = string.Empty;

        /// <summary>
        /// 追加コメントセットを取得または設定します。
        /// </summary>
        public string CommentSet { get; set; } = string.Empty;

        /// <summary>
        /// 削除フラグを取得または設定します。
        /// </summary>
        public bool DeleteFlag { get; set; } = false;

        #endregion

        #region "Public Constructor"

        /// <summary>
        /// <see cref="NoteMedicineWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineWriterArgs() : base()
        {
        }
        #endregion
    }
}