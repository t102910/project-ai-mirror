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
    internal sealed class FacilityMasterReader : QsDbReaderBase, IQsDbDistributedReader<QH_FACILITY_MST, FacilityMasterReaderArgs, FacilityMasterReaderResults>
    {
        /// <summary>
        /// <see cref="FacilityMasterReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMasterReader() : base()
        {
        }


        /// <summary>
        /// 医療機関番号からFacility情報を返す。（リストだが１件しか返らない想定）
        /// </summary>
        /// <param name="medicalFacilityCode"></param>
        /// <returns></returns>
        private List<QH_FACILITY_MST> SelectFacilityMst(string medicalFacilityCode )
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { this.CreateParameter(connection, "@p1", medicalFacilityCode) };

                // クエリを作成                
                query.Append("select * FROM QH_FACILITY_MST ");
                query.Append(" WHERE(facilitytype = 1) AND (deleteflag = 0)  AND (MEDICALFACILITYCODE = @p1) ");
               
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_FACILITY_MST>(connection, null, query.ToString(), @params);
            }
        }

        /// <summary>
        /// 連携システム番号からFacility情報を返す。（リストだが１件しか返らない想定）
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        private List<QH_FACILITY_MST> SelectFacilityMst(int linkageSystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { this.CreateParameter(connection, "@p1", linkageSystemNo) };

                // クエリを作成                
                query.Append("select QH_FACILITY_MST.* FROM QH_FACILITY_MST ");
                query.Append(" FROM QH_FACILITY_MST INNER JOIN QH_LINKAGESYSTEM_MST ON QH_FACILITY_MST.FACILITYKEY = QH_LINKAGESYSTEM_MST.FACILITYKEY ");
                query.Append(" WHERE (QH_LINKAGESYSTEM_MST.LINKAGESYSTEMNO =@1) AND (facilitytype = 1) AND (QH_FACILITY_MST.deleteflag = 0)   ");

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
        public FacilityMasterReaderResults ExecuteByDistributed(FacilityMasterReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            FacilityMasterReaderResults result = new FacilityMasterReaderResults() { IsSuccess = false };
            if (!string.IsNullOrEmpty(args.MedicalFacilityCode))
                result.Result = this.SelectFacilityMst(args.MedicalFacilityCode);
            else
                result.Result = this.SelectFacilityMst(args.LinkageSystemNo);
            if(result.Result !=null && result.Result.Count==1)
                result.IsSuccess = true;
            return result;
        }
    }


}
