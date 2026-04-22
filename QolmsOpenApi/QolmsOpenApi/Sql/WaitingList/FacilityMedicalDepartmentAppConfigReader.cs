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
    internal sealed class FacilityMedicalDepartmentAppConfigReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, FacilityMedicalDepartmentAppConfigReaderArgs, FacilityMedicalDepartmentAppConfigReaderResults>
    {

        /// <summary>
        /// <see cref="FacilityMedicalDepartmentAppConfigReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentAppConfigReader() : base()
        {
        }


        /// <summary>
        /// 対象の診療科に設定情報を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        private List<QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT> SelectConfig(int linkageSystemNo, string departmentCode)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT>())
            {
                
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", departmentCode),
                };
                
                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT TOP(1) T1.*")
                    .Append(" FROM QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT AS T1, QH_LINKAGESYSTEM_MST AS T3")
                    .Append(" WHERE T1.FACILITYKEY = T3.FACILITYKEY")
                    .Append(" AND T3.LINKAGESYSTEMNO = @P1")
                    .Append(" AND T1.LOCALCODE = @P2");
                
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public FacilityMedicalDepartmentAppConfigReaderResults ExecuteByDistributed(FacilityMedicalDepartmentAppConfigReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new FacilityMedicalDepartmentAppConfigReaderResults() { IsSuccess = false, MedicalDepartMentAppConfigEntity = new QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT()};

            // 一応初期値積んでおく
            var serializer = new QsJsonSerializer();
            var json = new QhFacilityDepartmentAppConfigOfJson() { 
                WaitingPriority = QsDbWaitingPriorityTypeEnum.None,
                ReserveFlag = false,
                WaitNumber = 0,
                AmbignousNumber = 999
            };
            result.MedicalDepartMentAppConfigEntity.VALUE = serializer.Serialize<QhFacilityDepartmentAppConfigOfJson>(json);

            var entity = this.SelectConfig(args.LinkageSystemNo, args.DepartmentCode);
            if (entity.Count == 1)
            {
                result.IsSuccess = true;
                result.MedicalDepartMentAppConfigEntity = entity[0];
            }
            
            return result;
        }
    }


}
