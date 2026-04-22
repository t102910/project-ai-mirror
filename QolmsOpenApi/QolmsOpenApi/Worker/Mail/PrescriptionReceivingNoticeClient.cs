using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// 薬局へ処方せん受信の通知メールを送信する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PrescriptionReceivingNoticeClient : QoNoticeClientBase<PrescriptionReceivingNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private PrescriptionReceivingNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="PasswordResetNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public PrescriptionReceivingNoticeClient(PrescriptionReceivingNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }


}