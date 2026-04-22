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
    internal sealed class LinkageSystemMasterReader : QsDbReaderBase, IQsDbDistributedReader<QH_LINKAGESYSTEM_MST, LinkageSystemMasterReaderArgs, LinkageSystemMasterReaderResults>
    {
        /// <summary>
        /// <see cref="LinkageSystemMasterReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkageSystemMasterReader() : base()
        {
        }


        /// <summary>
        /// FacilityKey からLinkageSystemNo情報を返す。（リストだが１件しか返らない想定）
        /// </summary>
        /// <param name="facilitykey"></param>
        /// <returns></returns>
        private List<QH_LINKAGESYSTEM_MST> SelectLinkageSystemMst(Guid facilitykey )
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { this.CreateParameter(connection, "@p1", facilitykey) };

                // クエリを作成                
                query.Append("select * FROM QH_LINKAGESYSTEM_MST ");
                query.Append(" WHERE (deleteflag = 0)  AND (FACILITYKEY = @p1 )");
               
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_LINKAGESYSTEM_MST>(connection, null, query.ToString(), @params);
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
        public LinkageSystemMasterReaderResults ExecuteByDistributed(LinkageSystemMasterReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkageSystemMasterReaderResults result = new LinkageSystemMasterReaderResults() { IsSuccess = false };
            result.Result = this.SelectLinkageSystemMst(args.FacilityKey);
            if(result.Result !=null && result.Result.Count==1)
                result.IsSuccess = true;
            return result;
        }
    }


}
