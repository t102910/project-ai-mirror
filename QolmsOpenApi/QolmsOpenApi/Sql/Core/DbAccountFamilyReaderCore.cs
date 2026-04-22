using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    
    /// <summary>
    /// 利用者家族情報の情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbAccountFamilyReaderCore : QsDbReaderBase
    {
        /// <summary>
        /// 親となるパブリックアカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _accountKey = Guid.Empty;



        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbAccountFamilyReaderCore()
        {
        }

        /// <summary>
        /// パブリックアカウントキーを指定して、
        /// <see cref="DbAccountFamilyReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">パブリックアカウントキー</param>
        /// <remarks></remarks>
        public DbAccountFamilyReaderCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "対象者アカウントキーが不正です。");
        }



        /// <summary>
        /// 家族情報のリストを取得します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<QH_ACCOUNTINDEX_DAT> ReadAccountFamilyEntityList(bool isIncludeMine = true)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_ACCOUNTINDEX_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { this.CreateParameter(connection, "@p1", this._accountKey) };

                // クエリを作成
                if (isIncludeMine)
                {
                    // 親アカウント
                    query.Append("select i.*, a.privateaccountflag");
                    query.Append(" from qh_accountindex_dat as i");
                    query.Append(" inner join qh_account_mst as a");
                    query.Append(" on a.accountkey = i.accountkey");
                    query.Append(" where a.deleteflag = 0");
                    query.Append(" and i.deleteflag = 0");
                    query.Append(" and i.acceptflag = 1");
                    query.Append(" and a.privateaccountflag = 0");
                    query.Append(" and i.accountkey = @p1");

                    query.Append(" union ");
                }

                // 子アカウント
                query.Append("select i.*, a.privateaccountflag");
                query.Append(" from QH_ACCOUNTRELATION_DAT as r");
                query.Append(" inner join qh_account_mst as a");
                query.Append(" on a.accountkey = r.RELATIONACCOUNTKEY");
                query.Append(" inner join qh_accountindex_dat as i");
                query.Append(" on i.accountkey = r.RELATIONACCOUNTKEY");
                query.Append(" where r.deleteflag = 0");
                query.Append(" and a.deleteflag = 0");
                query.Append(" and i.deleteflag = 0");
                query.Append(" and i.acceptflag = 1");
                query.Append(" and a.privateaccountflag = 1");
                query.Append(" and r.accountkey = @p1");
                // .Append("select i.*, a.privateaccountflag")
                // .Append(" from qh_familyrelationto_dat as r")
                // .Append(" inner join qh_account_mst as a")
                // .Append(" on a.accountkey = r.childaccountkey")
                // .Append(" inner join qh_accountindex_dat as i")
                // .Append(" on i.accountkey = r.childaccountkey")
                // .Append(" where r.deleteflag = 0")
                // .Append(" and a.deleteflag = 0")
                // .Append(" and i.deleteflag = 0")
                // .Append(" and i.acceptflag = 1")
                // .Append(" and a.privateaccountflag = 1")
                // .Append(" and r.accountkey = @p1")
                // unionのソート条件にprivateaccountflagを使用するために列指定する（が、取得しない）
                query.Append(" order by a.privateaccountflag, i.createddate");
                query.Append(";");
           

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_ACCOUNTINDEX_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }



        /// <summary>
        /// 家族情報のリストを取得します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_ACCOUNTINDEX_DAT> ReadAccountFamilyList(bool isIncludeMine = true)
        {
            return this.ReadAccountFamilyEntityList(isIncludeMine);
        }
    }


}