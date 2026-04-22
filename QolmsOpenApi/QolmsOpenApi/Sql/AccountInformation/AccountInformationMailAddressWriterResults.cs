using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// パスワードリカバリ用メールアドレスを、
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AccountInformationMailAddressWriterResults : QsDbWriterResultsBase
    {


        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 操作キーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ActionKey { get; set; } = Guid.Empty;



        /// <summary>
        /// <see cref="AccountInformationMailAddressWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountInformationMailAddressWriterResults() : base()
        {
        }
    }


}