using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設のTELを、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class FacilityContactInformationListReader : QsDbReaderBase, IQsDbDistributedReader<QH_FACILITYCONTACTINFORMATION_DAT, FacilityContactInformationListReaderArgs, FacilityContactInformationListReaderResults>
    {
        /// <summary>
        /// <see cref="FacilityContactInformationListReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityContactInformationListReader() : base()
        {
        }


        /// <summary>
        /// facilitykeyから施設のURLリストを返す。
        /// </summary>
        /// <param name="facilitykey"></param>
        /// <returns></returns>
        private List<QH_FACILITYCONTACTINFORMATION_DAT> SelectFacilityMst(Guid facilitykey)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITYCONTACTINFORMATION_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { 
                    this.CreateParameter(connection, "@p1", facilitykey)                 };

                // クエリを作成                
                query.Append("select * FROM QH_FACILITYCONTACTINFORMATION_DAT ");
                query.Append(" WHERE (FACILITYKEY=@p1) AND (deleteflag = 0)  ");
                query.Append(" ORDER BY DISPORDER, CONTACTINFORMATIONNO");
               
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_FACILITYCONTACTINFORMATION_DAT>(connection, null, query.ToString(), @params);
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
        public FacilityContactInformationListReaderResults ExecuteByDistributed(FacilityContactInformationListReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            FacilityContactInformationListReaderResults result = new FacilityContactInformationListReaderResults() { IsSuccess = false };

            result.Result = this.SelectFacilityMst(args.Facilitykey );
            result.IsSuccess = true;
            return result;
        }
    }


}
