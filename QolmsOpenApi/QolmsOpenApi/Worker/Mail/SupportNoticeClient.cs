using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// お問い合わせ登録メールを送信する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SupportNoticeClient : QoNoticeClientBase<SupportNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SupportNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="SupportNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public SupportNoticeClient(SupportNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }


}