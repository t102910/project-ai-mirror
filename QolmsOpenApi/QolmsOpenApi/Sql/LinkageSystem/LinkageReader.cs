using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システム ID を、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageReader :
        QsDbReaderBase,
        IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageReaderArgs, LinkageReaderResults>
    {

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageReader() : base() { }

        #endregion

        #region "Private Meshod"

        private string GetLinkageSystemId(Guid accountKey, int linkageSystemNo, byte statusType)
        {
            if (accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "accountKeyが不正です。");

            if (linkageSystemNo == int.MinValue)
                throw new ArgumentOutOfRangeException("linkageSystemNo", "連携システム番号が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@accountKey", accountKey) ,
                    this.CreateParameter(connection, "@linkageSystemNo", linkageSystemNo) ,
                    this.CreateParameter(connection,"@statusType",statusType) };
                

                // クエリを作成
                query.Append("select linkagesystemid from QH_LINKAGE_DAT ");
                query.Append(" where accountkey = @accountKey and linkagesystemno=@linkageSystemNo and statustype=@statusType ");
                query.Append(" and deleteflag = 0");
                query.Append(";");

                // コネクション オープン
                connection.Open();

                // クエリを実行
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query.ToString();
                    foreach (var p in @params) command.Parameters.Add(p);
                    var scalar = command.ExecuteScalar();

                    if (scalar == null)
                        return null;        // レコードなし
                    if (scalar == DBNull.Value)
                        return null;        // DB上でNULL

                    return scalar.ToString();
                }
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
        LinkageReaderResults IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageReaderArgs, LinkageReaderResults>.ExecuteByDistributed(LinkageReaderArgs args)
        {
            if (args == null) throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkageReaderResults result = new LinkageReaderResults() { IsSuccess = false };

            result.LinkageSystemId = GetLinkageSystemId(args.AccountKey, args.LinkageSystemNo, args.StatusType);

            result.IsSuccess = result.LinkageSystemId != null ? true : false;

            return result;
        }

        #endregion
    }
}
