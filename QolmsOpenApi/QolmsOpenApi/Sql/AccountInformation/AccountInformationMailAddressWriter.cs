using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// パスワードリカバリ用メールアドレスを、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AccountInformationMailAddressWriter<TEntity> : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, AccountInformationMailAddressWriterArgs, AccountInformationMailAddressWriterResults> where TEntity : QsPasswordManagementDataEntityBase, new()
    {


        /// <summary>
        /// <see cref="AccountInformationMailAddressWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountInformationMailAddressWriter() : base()
        {
        }



        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public AccountInformationMailAddressWriterResults ExecuteByDistributed(AccountInformationMailAddressWriterArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            AccountInformationMailAddressWriterResults result = new AccountInformationMailAddressWriterResults() { IsSuccess = false };
            DbAccountInformationWriterCore accountWriter = new DbAccountInformationWriterCore(args.AuthorKey, args.AuthorKey);
            DateTime actionDate = DateTime.Now;
            Guid actionKey = Guid.NewGuid();
            
            if (accountWriter.WritePasswordManagementEntityByMailAddress<TEntity>(args.PasswordRecoveryMailAddress, actionDate, actionKey))
            {
                // 処理件数
                result.Result = 1;

                // 操作日時
                result.ActionDate = actionDate;

                // 操作キー
                result.ActionKey = actionKey;

                // 成功
                result.IsSuccess = true;
            }
            else
                // 失敗
                throw new InvalidOperationException();

            return result;
        }
    }


}