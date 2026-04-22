using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// 診察呼び出し順番情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class WaitingOrderListReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 診療科コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DepartmentCode { get; set; } = string.Empty;

        /// <summary>
        /// 医師コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DoctorCode { get; set; } = string.Empty;

        /// <summary>
        /// 医師コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int WaitingNumber { get; set; } = int.MinValue;

        /// <summary>
        /// 医師コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ReserveFlag { get; set; } = true;

        /// <summary>
        /// 医師コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime OrderDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 待ち人数カウントパターンを取得または設定します。
        /// </summary>
        public QsDbWaitingPriorityTypeEnum priorityType { get; set; } = QsDbWaitingPriorityTypeEnum.None;

        /// <summary>
        /// 同日内連番を取得または設定します。
        /// </summary>
        public int SameDaySequence { get; set; } = int.MinValue;

        /// <summary>
        /// <see cref="WaitingOrderListReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public WaitingOrderListReaderArgs() : base()
        {
        }
    }


}