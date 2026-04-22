using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 検索項目をもとに連携IDを取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class AccountIdForgetReader :
        QsDbReaderBase,
        IQsDbDistributedReader<MGF_NULL_ENTITY, AccountIdForgetReaderArgs, AccountIdForgetReaderResults>
    {

        #region "Constructor"

        /// <summary>
        /// <see cref="AccountIdForgetReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountIdForgetReader() : base() { }

        #endregion

        #region "Private Meshod"

        private List<QH_PASSWORDMANAGEMENT_DAT> GetAccountId(string mailAddress, string familyName, string givenName, DateTime birthday, byte sex)
        {
            if (string.IsNullOrEmpty(mailAddress))
                throw new ArgumentOutOfRangeException("mailAddress", "メールアドレスが不正です。");

            if (string.IsNullOrEmpty(familyName))
                throw new ArgumentOutOfRangeException("familyName", "漢字姓が不正です。");

            if (string.IsNullOrEmpty(givenName))
                throw new ArgumentOutOfRangeException("givenName", "漢字名が不正です。");

            if (birthday == DateTime.MinValue)
                throw new ArgumentOutOfRangeException("birthday", "生年月日が不正です。");

            if (sex == byte.MinValue)
                throw new ArgumentOutOfRangeException("sex", "性別が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PASSWORDMANAGEMENT_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@mailAddress", mailAddress) ,
                    this.CreateParameter(connection, "@familyName", familyName) ,
                    this.CreateParameter(connection, "@givenName", givenName) ,
                    this.CreateParameter(connection, "@birthday", birthday) ,
                    this.CreateParameter(connection, "@sex", sex) ,
                };

                // クエリを作成
                query.Append(" SELECT T1.*");
                query.Append(" FROM QH_PASSWORDMANAGEMENT_DAT AS T1, QH_ACCOUNTINDEX_DAT AS T2 ");
                query.Append(" WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY");
                query.Append(" AND T1.PASSWORDRECOVERYMAILADDRESS = @mailAddress");
                query.Append(" AND T1.DELETEFLAG = 0");
                query.Append(" AND T2.FAMILYNAME = @familyName");
                query.Append(" AND T2.GIVENNAME = @givenName");
                query.Append(" AND T2.SEXTYPE = @sex");
                query.Append(" AND T2.BIRTHDAY = @birthday");
                query.Append(" AND T2.DELETEFLAG = 0");
                query.Append(";");

                // コネクション オープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_PASSWORDMANAGEMENT_DAT>(connection, null, query.ToString(), @params);
            }
        }

        #endregion
        #region "Public Method"

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        AccountIdForgetReaderResults IQsDbDistributedReader<MGF_NULL_ENTITY, AccountIdForgetReaderArgs, AccountIdForgetReaderResults>.ExecuteByDistributed(AccountIdForgetReaderArgs args)
        {
            if (args == null) throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new AccountIdForgetReaderResults() { IsSuccess = false };
            result.AccountId = new List<string>();

            //検索項目をもとにLinkageSystemIdを検索
            GetAccountId(args.MailAddress, args.FamilyName, args.GivenName, args.Birthday, args.Sex).ForEach(entity => { result.AccountId.Add(entity.USERID); });
            
            result.IsSuccess = true;
            return result;
        }

        #endregion
    }
}
