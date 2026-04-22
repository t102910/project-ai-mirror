using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設の内容を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LinkageFacilityListReader : QsDbReaderBase, IQsDbDistributedReader<QH_FACILITY_MST, LinkageFacilityListReaderArgs, LinkageFacilityListReaderResults>
    {
        /// <summary>
        /// <see cref="LinkageFacilityListReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageFacilityListReader() : base()
        {
        }


        /// <summary>
        /// LinkageSystemNoから関連子施設のリストを返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="updateDate"></param>
        /// <returns></returns>
        private List<QH_FACILITY_MST> SelectFacilityMst(int linkageSystemNo, DateTime updateDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { 
                    this.CreateParameter(connection, "@p1", linkageSystemNo) ,
                    this.CreateParameter(connection,"@p2" ,updateDate)
                };

                // クエリを作成                
                query.Append("select qh_facility_mst.* FROM QH_FACILITY_MST ");
                query.Append(" INNER JOIN QH_LINKAGESYSTEM_MST ON  QH_FACILITY_MST.PARENTKEY = QH_LINKAGESYSTEM_MST.FACILITYKEY ");
                query.Append(" WHERE(qh_facility_mst.facilitytype = 1) AND (qh_facility_mst.deleteflag = 0) AND (qh_facility_mst.updateddate >= @p2) AND (qh_linkagesystem_mst.deleteflag = 0) AND (qh_linkagesystem_mst.linkagesystemno = @p1) ");
                query.Append(" ORDER BY QH_FACILITY_MST.DISPORDER");
               
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_FACILITY_MST>(connection, null, query.ToString(), @params);
            }
        }


        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LinkageFacilityListReaderResults ExecuteByDistributed(LinkageFacilityListReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkageFacilityListReaderResults result = new LinkageFacilityListReaderResults() { IsSuccess = false };

            result.Result = this.SelectFacilityMst(args.LinkageSystemNo ,args.UpdatedDate );
            result.IsSuccess = true;
            return result;
        }
    }


}
