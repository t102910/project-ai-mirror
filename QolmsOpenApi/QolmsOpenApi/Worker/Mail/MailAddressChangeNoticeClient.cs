using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ''' メールアドレス変更メール（メール認証）を送信する機能を提供します。
    /// ''' このクラスは継承できません。
    /// ''' </summary>
    /// ''' <remarks></remarks>
    internal sealed class MailAddressChangeNoticeClient : QoNoticeClientBase<MailAddressChangeNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private MailAddressChangeNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="MailAddressChangeNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public MailAddressChangeNoticeClient(MailAddressChangeNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }

}