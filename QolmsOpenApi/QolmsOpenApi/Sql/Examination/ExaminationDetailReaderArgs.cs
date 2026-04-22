using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi
{
    
    /// <summary>
    /// 検査結果の情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class ExaminationDetailReaderArgs : QsDbReaderArgsBase<QH_EXAMINATION_DAT>
    {


        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 検査日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 施設キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// <see cref="ExaminationDetailReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExaminationDetailReaderArgs() : base()
        {
        }
    }


}