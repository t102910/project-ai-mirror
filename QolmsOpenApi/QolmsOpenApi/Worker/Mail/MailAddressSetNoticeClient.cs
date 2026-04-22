using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ''' メールアドレス設定メール（ユーザIDお知らせ）を送信する機能を提供します。
    /// ''' このクラスは継承できません。
    /// ''' </summary>
    /// ''' <remarks></remarks>
    internal sealed class MailAddressSetNoticeClient : QoNoticeClientBase<MailAddressSetNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private MailAddressSetNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="MailAddressSetNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public MailAddressSetNoticeClient(MailAddressSetNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }

}