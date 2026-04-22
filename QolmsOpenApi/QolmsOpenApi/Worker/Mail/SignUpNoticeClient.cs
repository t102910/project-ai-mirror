using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// アカウント登録メールを送信する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SignUpNoticeClient : QoNoticeClientBase<SignUpNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SignUpNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="SignUpNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public SignUpNoticeClient(SignUpNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }

}
