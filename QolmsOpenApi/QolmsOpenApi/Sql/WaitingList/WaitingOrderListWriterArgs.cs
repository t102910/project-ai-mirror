using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ログイン日時を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class WaitingOrderListWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// UpsertEntity を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public QH_WAITINGORDERLIST_DAT Entity { get; set; }

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
        /// LinkageSystemId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// 同日内連番を取得または設定します。
        /// </summary>
        public int DjKbn { get; set; } = int.MinValue;

        /// <summary>
        /// <see cref="WaitingOrderListWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public WaitingOrderListWriterArgs() : base()
        {
        }
    }


}