using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ログイン日時 を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AccountLogInManagementWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, AccountLogInManagementWriterArgs, AccountLogInManagementWriterResults>
    {

        /// <summary>
        /// <see cref="AccountLogInManagementWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountLogInManagementWriter() : base()
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
        public AccountLogInManagementWriterResults ExecuteByDistributed(AccountLogInManagementWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            //UPDATE実行
            AccountLogInManagementWriterResults result = new AccountLogInManagementWriterResults() { IsSuccess = false };
            DbAccountLogInManagementWriterCore accountWriter = new DbAccountLogInManagementWriterCore(args.AuthorKey, args.ActionDate);
            
            if (accountWriter.UpdateAccountLogInManagement())
            {
                // 成功
                result.IsSuccess = true;
            }

            return result;
        }
    }


}