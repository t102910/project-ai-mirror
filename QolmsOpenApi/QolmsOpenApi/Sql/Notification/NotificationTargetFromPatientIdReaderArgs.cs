using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi
{

    /// <summary>
    /// プッシュ通知対象アカウントキーと通知元施設キーを、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NotificationTargetFromPatientIdReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// 患者IDを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// LinkageSystemNoを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemNo { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="NotificationTargetFromPatientIdReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NotificationTargetFromPatientIdReaderArgs() : base()
        {
        }
    }


}