using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class ExternalServiceLinkageWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, ExternalServiceLinkageWriterArgs, ExternalServiceLinkageWriterResults>
    {
        public ExternalServiceLinkageWriterResults ExecuteByDistributed(ExternalServiceLinkageWriterArgs args)
        {
            var result = new ExternalServiceLinkageWriterResults { IsSuccess = false };
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            if (!Upsert(args.Entity))
            {
                return result;
            }

            // 処理件数
            result.Result = 1;
            // 操作日時
            result.ActionDate = actionDate;
            // 操作キー
            result.ActionKey = actionKey;

            // 成功
            result.IsSuccess = true;

            return result;
        }

        private bool Upsert(QH_EXTERNALSERVICELINKAGE_DAT entity)
        {

            using (var con = QsDbManager.CreateDbConnection<QH_EXTERNALSERVICELINKAGE_DAT>())
            {
                var now = DateTime.Now;

                var param = new List<DbParameter>
                {
                    CreateParameter(con, "@accountKey", entity.ACCOUNTKEY),
                    CreateParameter(con, "@externalServiceType", entity.EXTERNALSERVICETYPE),
                    CreateParameter(con, "@targetServiceType", entity.TARGETSERVICETYPE),
                    CreateParameter(con, "@value", entity.VALUE),
                    CreateParameter(con, "@deleteFlag", entity.DELETEFLAG),
                    CreateParameter(con, "@updatedDate", now)
                };

                // データ追加または更新
                var mergeSql = $@"
                    MERGE INTO QH_EXTERNALSERVICELINKAGE_DAT AS A
                    USING(
                        SELECT
                        @accountKey AS ACCOUNTKEY, 
                        @externalServiceType As EXTERNALSERVICETYPE,
                        @targetServiceType AS TARGETSERVICETYPE, 
                        @value AS VALUE, 
                        @deleteFlag AS DELETEFLAG,
                        @updatedDate AS CREATEDDATE, 
                        @updatedDate AS UPDATEDDATE
                    ) AS B
                    ON A.ACCOUNTKEY = B.ACCOUNTKEY
                    AND A.EXTERNALSERVICETYPE = B.EXTERNALSERVICETYPE
                    AND A.TARGETSERVICETYPE = B.TARGETSERVICETYPE
                    WHEN MATCHED THEN
                        UPDATE SET
                        VALUE = B.VALUE, 
                        DELETEFLAG = B.DELETEFLAG,
                        UPDATEDDATE = B.UPDATEDDATE
                    WHEN NOT MATCHED THEN
                        INSERT
                        (ACCOUNTKEY, EXTERNALSERVICETYPE, TARGETSERVICETYPE, VALUE, DELETEFLAG, CREATEDDATE, UPDATEDDATE)
                        VALUES
                        (B.ACCOUNTKEY, B.EXTERNALSERVICETYPE, B.TARGETSERVICETYPE, B.VALUE, B.DELETEFLAG, B.CREATEDDATE, B.UPDATEDDATE);
                ";

                con.Open();

                var affectedRows = ExecuteNonQuery(con, null, mergeSql, param);

                return affectedRows == 1;
            }            
        }
    }
}