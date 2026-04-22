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
    internal sealed class MedicalFacilitySearchReader : QsDbReaderBase, IQsDbDistributedReader<QH_FACILITY_MST, MedicalFacilitySearchReaderArgs, MedicalFacilitySearchReaderResults>
    {
        /// <summary>
        /// <see cref="FacilityMasterReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public MedicalFacilitySearchReader() : base()
        {
        }


        /// <summary>
        /// 医療施設情報を検索。
        /// </summary>
        /// <param name="medicalFacilityCode"></param>
        /// <param name="prefectureCode"></param>
        /// <param name="keyWord"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private List<QH_FACILITY_MST> SelectFacilityMst(string medicalFacilityCode, string prefectureCode, string keyWord, int pageSize, int pageIndex)
        {
            
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", medicalFacilityCode),
                    this.CreateParameter(connection, "@P2", prefectureCode),
                    this.CreateParameter(connection, "@P3", string.Format($"%{keyWord}%")),
                    this.CreateParameter(connection, "@P4", pageSize),
                    this.CreateParameter(connection, "@P5", pageSize * pageIndex),
                    this.CreateParameter(connection, "@P6", false),
                    this.CreateParameter(connection, "@P7", (byte)QH_FACILITY_MST.FacilityTypeEnum.Medical)};

                // クエリ(件数)を作成                
                StringBuilder query = new StringBuilder();
                // クエリを作成                
                query.Append(" SELECT FACILITYKEY");
                query.Append("  ,FACILITYID");
                query.Append("  ,FACILITYNAME");
                query.Append("  ,FACILITYKANANAME");
                query.Append("  ,FACILITYTYPE");
                query.Append("  ,PARENTKEY");
                query.Append("  ,POSTALCODE");
                query.Append("  ,ADDRESS1");
                query.Append("  ,ADDRESS2");
                query.Append("  ,PREFNO");
                query.Append("  ,CITYNO");
                query.Append("  ,TEL");
                query.Append("  ,FAX");
                query.Append("  ,OFFICIALNAME");
                query.Append("  ,MEDICALFACILITYCODE");
                query.Append("  ,ASSOCIATIONCODE");
                query.Append("  ,INSURERNO");
                query.Append(" ,(SELECT COUNT(0) FROM QH_FACILITY_MST WHERE DELETEFLAG = @P6 AND FACILITYTYPE = @P7");
                
                //医療機関番号が設定されている場合、医療機関番号で検索
                if (!string.IsNullOrEmpty(medicalFacilityCode))
                {
                    query.Append(" AND MEDICALFACILITYCODE = @P1");
                }
                else
                {
                    query.Append(" AND PREFNO = @P2");
                    query.Append(" AND (FACILITYNAME LIKE @P3 OR FACILITYKANANAME LIKE @P3 OR OFFICIALNAME LIKE @P3)");
                }
                
                query.Append("  ) AS DISPORDER");
                query.Append("  ,DELETEFLAG");
                query.Append("  ,CREATEDDATE");
                query.Append("  ,UPDATEDDATE");
                query.Append("  FROM QH_FACILITY_MST");
                query.Append(" WHERE DELETEFLAG = @P6 AND FACILITYTYPE = @P7");

                //医療機関番号が設定されている場合、医療機関番号で検索
                if (!string.IsNullOrEmpty(medicalFacilityCode))
                {
                    query.Append(" AND MEDICALFACILITYCODE = @P1");
                }
                else
                {
                    query.Append(" AND PREFNO = @P2");
                    query.Append(" AND (FACILITYNAME LIKE @P3 OR FACILITYKANANAME LIKE @P3 OR OFFICIALNAME LIKE @P3)");
                    query.Append(" ORDER BY FACILITYKANANAME");
                    query.Append(" OFFSET @P5 ROWS");
                    query.Append(" FETCH NEXT @P4 ROWS ONLY");
                }

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
        public MedicalFacilitySearchReaderResults ExecuteByDistributed(MedicalFacilitySearchReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            MedicalFacilitySearchReaderResults result = new MedicalFacilitySearchReaderResults() { IsSuccess = false };
            result.Result = this.SelectFacilityMst(args.MedicalFacilityCode, args.PrefectureCode, args.KeyWord, args.PageSize, args.PageIndex);
            if(result.Result !=null && result.Result.Count > 0)
                result.IsSuccess = true;
            return result;
        }
    }


}
