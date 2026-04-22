using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設の診療科情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class FacilityMedicalDepartmentListReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, FacilityMedicalDepartmentListReaderArgs, FacilityMedicalDepartmentListReaderResults>
    {

        internal sealed class MedicalDepartmentResult : QsDbEntityBase
        {
           

            public int DEPARTMENTNO { get; set; } = int.MinValue;
            public string DEPARTMENTNAME { get; set; } = string.Empty;
            public string LOCALCODE { get; set; } = string.Empty;
            public string LOCALNAME { get; set; } = string.Empty;


            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                try
                {

                    
                    this.DEPARTMENTNO = reader.GetInt32(0);
                    this.DEPARTMENTNAME = reader.GetString(1);
                    this.LOCALCODE = reader.GetString(2);
                    this.LOCALNAME = reader.GetString(3);

                    this.KeyGuid = Guid.NewGuid();
                    this.DataState = QsDbEntityStateTypeEnum.Unchanged;
                    this.IsEmpty = false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return this;
            }

            public override bool IsKeysValid()
            {
                return true;
            }


        }
        /// <summary>
        /// <see cref="FacilityMedicalDepartmentListReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentListReader() : base()
        {
        }


        /// <summary>
        /// facilitykeyから施設の診療科リストを返す。
        /// </summary>
        /// <param name="facilitykey"></param>
        /// <returns></returns>
        private List<MedicalDepartmentResult> SelectFacilityMst(Guid facilitykey)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITYURL_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { 
                    this.CreateParameter(connection, "@p1", facilitykey)                 };

                // クエリを作成                
                query.Append("select QH_FACILITYMEDICALDEPARTMENT_DAT.DEPARTMENTNO,DEPARTMENTNAME,LOCALCODE,LOCALNAME  ");
                query.Append(" FROM QH_FACILITYMEDICALDEPARTMENT_DAT INNER JOIN QH_MEDICALDEPARTMENT_MST ON QH_FACILITYMEDICALDEPARTMENT_DAT.DEPARTMENTNO = QH_MEDICALDEPARTMENT_MST.DEPARTMENTNO "); 
                query.Append(" WHERE (FACILITYKEY=@p1) AND (QH_FACILITYMEDICALDEPARTMENT_DAT.deleteflag = 0)  ");
                query.Append(" ORDER BY QH_FACILITYMEDICALDEPARTMENT_DAT.DISPORDER, QH_FACILITYMEDICALDEPARTMENT_DAT.DEPARTMENTNO ;");
               
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<MedicalDepartmentResult>(connection, null, query.ToString(), @params);
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
        public FacilityMedicalDepartmentListReaderResults ExecuteByDistributed(FacilityMedicalDepartmentListReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            FacilityMedicalDepartmentListReaderResults result = new FacilityMedicalDepartmentListReaderResults() { IsSuccess = false };

            var entity = this.SelectFacilityMst(args.Facilitykey);
            if (entity != null)
            {
                foreach (var item in entity)
                {
                    result.MedicalDepartmentList.Add(new DbMedicalDepartmentItem()
                    {
                        DepartmentNo = item.DEPARTMENTNO,
                        DepartmentName = item.DEPARTMENTNAME,
                        LocalCode = item.LOCALCODE,
                        LocalName = item.LOCALNAME
                    });
                };
            }
             
            result.IsSuccess = true;
            return result;
        }
    }


}
