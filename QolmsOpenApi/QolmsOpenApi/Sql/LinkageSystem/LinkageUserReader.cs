using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システムの連携IDからアカウントキーを、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageUserReader :
        QsDbReaderBase,
        IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageUserReaderArgs, LinkageUserReaderResults>
    {

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageUserReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageUserReader() : base() { }

        #endregion

        #region "Private Meshod"

        private Guid GetAccountKey(int linkageSystemNo, string linkageSystemId, byte statusNo)
        {
            if (linkageSystemNo == int.MinValue)
                throw new ArgumentOutOfRangeException("linkageSystemNo", "連携システムNOが不正です。");

            if (string.IsNullOrEmpty(linkageSystemId))
                throw new ArgumentOutOfRangeException("linkageSystemId", "連携システムIDが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@linkageSystemNo", linkageSystemNo) ,
                    this.CreateParameter(connection, "@linkageSystemId", linkageSystemId) ,
                    this.CreateParameter(connection,"@statustype",statusNo) };
                bool refIsSuccess = false;

                // クエリを作成
                query.Append("select accountkey from qh_linkage_dat");
                query.Append(" where linkagesystemno = @linkageSystemNo and linkagesystemid=@linkageSystemId and statustype=@statustype ");
                query.Append(" and deleteflag = 0");
                query.Append(";");

                // コネクション オープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<Guid>(connection, null, query.ToString(), @params, Guid.Empty, ref refIsSuccess);
            }
        }

        private Guid GetAccountKey(Guid facilityKey, string linkageSystemId, byte statusNo)
        {
            if (facilityKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("facilityKey", "facilityKeyが不正です。");

            if (string.IsNullOrEmpty(linkageSystemId))
                throw new ArgumentOutOfRangeException("linkageSystemId", "連携システムIDが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@facilityKey", facilityKey) ,
                    this.CreateParameter(connection, "@linkageSystemId", linkageSystemId) ,
                    this.CreateParameter(connection,"@statustype",statusNo) };
                bool refIsSuccess = false;

                // クエリを作成
                query.Append("select accountkey from QH_LINKAGE_DAT INNER JOIN QH_LINKAGESYSTEM_MST ON QH_LINKAGE_DAT.LINKAGESYSTEMNO = QH_LINKAGESYSTEM_MST.LINKAGESYSTEMNO ");
                query.Append(" where FACILITYKEY  = @facilityKey and linkagesystemid=@linkageSystemId and statustype=@statustype ");
                query.Append(" and QH_LINKAGE_DAT.deleteflag = 0");
                query.Append(";");

                // コネクション オープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<Guid>(connection, null, query.ToString(), @params, Guid.Empty, ref refIsSuccess);
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
        LinkageUserReaderResults IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageUserReaderArgs, LinkageUserReaderResults>.ExecuteByDistributed(LinkageUserReaderArgs args)
        {
            if (args == null) throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkageUserReaderResults result = new LinkageUserReaderResults() { IsSuccess = false };
            if (args.LinkageSystemNo > 0)
                result.AccountKey = GetAccountKey(args.LinkageSystemNo, args.LinkageSystemId, args.StatusNo);
            else
                result.AccountKey = GetAccountKey(args.FacilityKey, args.LinkageSystemId, args.StatusNo);

            result.IsSuccess = result.AccountKey != Guid.Empty ? true : false;

            return result;
        }

        #endregion
    }
}
