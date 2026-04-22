using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi
{

    /// <summary>
    /// LinePreRegistの情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class LinePreRegistReaderArgs : QsDbReaderArgsBase<QH_LINEPREREGIST_DAT>
    {

        /// <summary>
        /// LineUserId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LineUserId { get; set; } = string.Empty;


        /// <summary>
        /// <see cref="LinePreRegistReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinePreRegistReaderArgs() : base()
        {
        }
    }


}