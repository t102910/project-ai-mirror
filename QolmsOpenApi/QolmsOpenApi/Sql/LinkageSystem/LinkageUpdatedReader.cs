using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 連携システムの連携IDからレコード更新日時を、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkageUpdatedReader :
        QsDbReaderBase,
        IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageUpdatedReaderArgs, LinkageUpdatedReaderResults>
    {

        #region "Constructor"

        /// <summary>
        /// <see cref="LinkageUpdatedReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageUpdatedReader() : base() { }

        #endregion

        #region "Private Meshod"

        private DateTime GetUpdatedDate(Guid facilityKey, string linkageSystemId)
        {
            if (facilityKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("linkageSystemNo", "連携システムNOが不正です。");

            if (string.IsNullOrEmpty(linkageSystemId))
                throw new ArgumentOutOfRangeException("linkageSystemId", "連携システムIDが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@facilityKey", facilityKey) ,
                    this.CreateParameter(connection, "@linkageSystemId", linkageSystemId) ,
                };
                bool refIsSuccess = false;

                // クエリを作成
                query.Append("select top (1) t1.updateddate from qh_linkage_dat as t1, qh_linkagesystem_mst as t2");
                query.Append(" where t1.linkagesystemid = @linkageSystemId and t1.linkagesystemno = t2.linkagesystemno and t2.facilityKey = @facilityKey ");
                query.Append(" order by updateddate desc");
                query.Append(";");

                // コネクション オープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<DateTime>(connection, null, query.ToString(), @params, DateTime.MinValue, ref refIsSuccess);
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
        LinkageUpdatedReaderResults IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageUpdatedReaderArgs, LinkageUpdatedReaderResults>.ExecuteByDistributed(LinkageUpdatedReaderArgs args)
        {
            if (args == null) throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkageUpdatedReaderResults result = new LinkageUpdatedReaderResults() { IsSuccess = false };
            result.UpdatedDate = GetUpdatedDate(args.FacilityKey, args.LinkageSystemId);

            result.IsSuccess = result.UpdatedDate != DateTime.MinValue ? true : false;

            return result;
        }

        #endregion
    }
}
