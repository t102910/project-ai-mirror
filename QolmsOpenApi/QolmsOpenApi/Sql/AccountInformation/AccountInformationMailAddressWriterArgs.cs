using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// パスワードリカバリ用メールアドレスを、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AccountInformationMailAddressWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// 所有者アカウント キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// パスワードリカバリ用メールアドレスを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PasswordRecoveryMailAddress { get; set; } = string.Empty;



        /// <summary>
        /// <see cref="AccountInformationMailAddressWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountInformationMailAddressWriterArgs() : base()
        {
        }
    }


}