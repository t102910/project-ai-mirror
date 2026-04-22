using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 診療科設定情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class FacilityMedicalDepartmentConfigReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, FacilityMedicalDepartmentConfigReaderArgs, FacilityMedicalDepartmentConfigReaderResults>
    {

        /// <summary>
        /// <see cref="FacilityMedicalDepartmentConfigReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentConfigReader() : base()
        {
        }


        /// <summary>
        /// 対象の診療科に設定情報を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        private List<QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT> SelectConfig(int linkageSystemNo, string departmentCode)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT>())
            {
                
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", departmentCode),
                };
                
                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT TOP(1) T1.*")
                    //.Append(" FROM QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT AS T1, QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT AS T2, QH_LINKAGESYSTEM_MST AS T3")
                    .Append(" FROM QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT AS T1, QH_LINKAGESYSTEM_MST AS T3")
                    .Append(" WHERE T1.FACILITYKEY = T3.FACILITYKEY")
                    //.Append(" AND T1.FACILITYKEY = T2.FACILITYKEY")
                    //.Append(" AND T1.DEPARTMENTNO = T2.DEPARTMENTNO")
                    //.Append(" AND T1.LOCALCODE = T2.LOCALCODE")
                    .Append(" AND T3.LINKAGESYSTEMNO = @P1")
                    .Append(" AND T1.LOCALCODE = @P2");
                
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public FacilityMedicalDepartmentConfigReaderResults ExecuteByDistributed(FacilityMedicalDepartmentConfigReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new FacilityMedicalDepartmentConfigReaderResults() { IsSuccess = false, MedicalDepartMentConfigEntity = new QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT()};

            var entity = this.SelectConfig(args.LinkageSystemNo, args.DepartmentCode);
            if (entity.Count == 1)
            {
                result.IsSuccess = true;
                result.MedicalDepartMentConfigEntity = entity[0];
            }
            
            return result;
        }
    }


}
