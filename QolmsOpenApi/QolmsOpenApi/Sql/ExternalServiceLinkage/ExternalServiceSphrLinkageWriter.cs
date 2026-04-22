using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class ExternalServiceSphrLinkageWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, ExternalServiceSphrLinkageWriterArgs, ExternalServiceSphrLinkageWriterResults>
    {
        public ExternalServiceSphrLinkageWriterResults ExecuteByDistributed(ExternalServiceSphrLinkageWriterArgs args)
        {
            var result = new ExternalServiceSphrLinkageWriterResults { IsSuccess = false };
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

        private bool Upsert(QH_LINKAGE_DAT entity)
        {

            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var now = DateTime.Now;

                var param = new List<DbParameter>
                {
                    CreateParameter(con, "@accountKey", entity.ACCOUNTKEY),
                    CreateParameter(con, "@linkageSystemNo", entity.LINKAGESYSTEMNO),
                    CreateParameter(con, "@linkageSystemId", entity.LINKAGESYSTEMID),
                    CreateParameter(con, "@dataSet", string.Empty),
                    CreateParameter(con, "@statusType", 2),
                    CreateParameter(con, "@deleteFlag", entity.DELETEFLAG),
                    CreateParameter(con, "@updatedDate", now)
                };

                // データ追加または更新
                var mergeSql = $@"
                    MERGE INTO QH_LINKAGE_DAT AS A
                    USING(
                        SELECT
                        @accountKey AS ACCOUNTKEY, 
                        @linkageSystemNo AS LINKAGESYSTEMNO,
                        @linkageSystemId AS LINKAGESYSTEMID, 
                        @dataSet AS DATASET, 
                        @statusType AS STATUSTYPE, 
                        @deleteFlag AS DELETEFLAG,
                        @updatedDate AS CREATEDDATE, 
                        @updatedDate AS UPDATEDDATE
                    ) AS B
                    ON A.ACCOUNTKEY = B.ACCOUNTKEY
                    AND A.LINKAGESYSTEMNO = B.LINKAGESYSTEMNO
                    WHEN MATCHED THEN
                        UPDATE SET
                        LINKAGESYSTEMID = B.LINKAGESYSTEMID, 
                        DELETEFLAG = B.DELETEFLAG,
                        UPDATEDDATE = B.UPDATEDDATE
                    WHEN NOT MATCHED THEN
                        INSERT
                        (ACCOUNTKEY, LINKAGESYSTEMNO, LINKAGESYSTEMID, DATASET, STATUSTYPE, DELETEFLAG, CREATEDDATE, UPDATEDDATE)
                        VALUES
                        (B.ACCOUNTKEY, B.LINKAGESYSTEMNO, B.LINKAGESYSTEMID, B.DATASET, B.STATUSTYPE, B.DELETEFLAG, B.CREATEDDATE, B.UPDATEDDATE);
                ";

                con.Open();

                var affectedRows = ExecuteNonQuery(con, null, mergeSql, param);

                return affectedRows == 1;
            }            
        }
    }
}