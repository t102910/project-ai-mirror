using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ''' アカウント登録メールを送信する機能を提供します。
    /// ''' このクラスは継承できません。
    /// ''' </summary>
    /// ''' <remarks></remarks>
    internal sealed class PasswordResetNoticeClient : QoNoticeClientBase<PasswordResetNoticeClientArgs>
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private PasswordResetNoticeClient() : base()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// <see cref="PasswordResetNoticeClient" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs"></param>
        /// <remarks></remarks>
        public PasswordResetNoticeClient(PasswordResetNoticeClientArgs noticeArgs) : base(noticeArgs)
        {
        }
    }


}