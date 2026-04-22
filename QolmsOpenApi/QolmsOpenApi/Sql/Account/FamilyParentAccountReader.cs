
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 子アカウント関連のReader
    /// </summary>
    internal sealed class FamilyParentAccountReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, FamilyParentAccountReaderArgs, FamilyParentAccountReaderResults>
    {

        private List<QH_LINKAGE_DAT> GetParentAccountKey(Guid accountkey, int linkageSystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {                
                List<DbParameter> @params = new List<DbParameter>() 
                {
                    this.CreateParameter(connection, "@accountkey", accountkey) ,
                    this.CreateParameter (connection ,"@linkagesystemno", linkageSystemNo),
                };

                // クエリを作成
                var sql = @"
                    SELECT * FROM QH_LINKAGE_DAT
                    WHERE
                    (
                        ACCOUNTKEY = @accountkey
                        OR
                        ACCOUNTKEY = (
                            SELECT TOP(1) ACCOUNTKEY FROM QH_ACCOUNTRELATION_DAT
                            WHERE RELATIONACCOUNTKEY = @accountkey
                            AND RELATIONDIRECTIONTYPE = 1
                            AND RELATIONTYPE = 1
                            AND DELETEFLAG = 0
                        )
                    )
                    AND LINKAGESYSTEMNO = @linkagesystemno
                    AND STATUSTYPE = 2
                    AND DELETEFLAG = 0;
                ";               

                // コネクション オープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_LINKAGE_DAT>(connection, null, this.CreateCommandText(connection, sql), @params);
            }
        }

        /// <summary>
        /// <see cref="FamilyParentAccountReader" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FamilyParentAccountReader() : base()
        {
        }



        public FamilyParentAccountReaderResults ExecuteByDistributed(FamilyParentAccountReaderArgs args)
        {
            FamilyParentAccountReaderResults result = new FamilyParentAccountReaderResults() { IsSuccess = false };

            var entity = GetParentAccountKey(args.AccountKey , args.LinkageSystemNo).FirstOrDefault();
            if(entity!=null )
            {
                result.AccountKey = entity.ACCOUNTKEY;
                result.LinkageId = entity.LINKAGESYSTEMID;
                result.IsSuccess = true;
            }
            
            return result;
        }
    }


}