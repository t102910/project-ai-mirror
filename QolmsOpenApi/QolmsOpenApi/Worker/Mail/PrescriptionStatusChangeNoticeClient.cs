using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// 処方箋ステータス変更メールを送信する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
     internal sealed class PrescriptionStatusChangeNoticeClient : QoNoticeClientBase<PrescriptionStatusChangeNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private PrescriptionStatusChangeNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="PrescriptionStatusChangeNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public PrescriptionStatusChangeNoticeClient(PrescriptionStatusChangeNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }


}