using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// ワンタイムコードから利用者の情報を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbOnetimeCodeReaderCore : QsDbReaderBase
    {

        /// <summary>
        /// <see cref="DbOnetimeCodeReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DbOnetimeCodeReaderCore() : base()
        {
        }

        #region "Private Method"

        /// <summary>
        /// ワンタイムコードのエンティティを取得します。
        /// </summary>
        /// <param name="otc"></param>
        /// <returns></returns>
        private List<QH_ELINKONETIMECODE_DAT> ReadOnetimeCodeEntities(string otc)
        {
            if (string.IsNullOrWhiteSpace(otc)) throw new ArgumentOutOfRangeException("otc", "ワンタイムコードが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_ELINKONETIMECODE_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>()  {
                    this.CreateParameter(connection, "@p1", otc),
                    this.CreateParameter(connection, "@p2", DateTime.Now),
                };

                // クエリ作成
                query.Append("select o.*");
                query.Append(" from qh_elinkonetimecode_dat as o");
                query.Append(" where o.onetimecode = @p1");
                query.Append(" and o.requesteddate <= @p2"); // 未使用チェックで、未来に有効になるのがあると困るけど、そういうのは作らないので手抜き
                query.Append(" and o.expires >= @p2");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_ELINKONETIMECODE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        #endregion

        #region "public method"

        /// <summary>
        /// ワンタイムコードからアカウントキーを逆引きします。
        /// </summary>
        /// <param name="otc"></param>
        /// <returns></returns>
        public List<QH_ELINKONETIMECODE_DAT> ReadEntities(string otc)
        {
            return this.ReadOnetimeCodeEntities(otc);
        }

        #endregion
    }
}