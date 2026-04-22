using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi
{

    /// <summary>
    /// SPHR用のバイタル情報 を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SphrHealthRecordReaderArgs : QsDbReaderArgsBase<QH_HEALTHRECORD_DAT>
    {

        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemNo { get; set; } = string.Empty;

        /// <summary>
        /// LinkageSystemId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// VitalTypes を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string VitalTypes { get; set; } = string.Empty;


        /// <summary>
        /// <see cref="SphrHealthRecordReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public SphrHealthRecordReaderArgs() : base()
        {
        }
    }


}