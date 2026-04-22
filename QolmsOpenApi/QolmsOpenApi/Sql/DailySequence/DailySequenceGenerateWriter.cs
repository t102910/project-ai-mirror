using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class DailySequenceGenerateWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, DailySequenceGenerateWriterArgs, DailySequenceGenerateWriterResults>
    {
        public DailySequenceGenerateWriterResults ExecuteByDistributed(DailySequenceGenerateWriterArgs args)
        {
            var result = new DailySequenceGenerateWriterResults { IsSuccess = false };
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            var systemType = args.SystemType == QolmsApiCoreV1.QsApiSystemTypeEnum.None ? 0 : (int)args.SystemType;

            if(!GetNewSequence(systemType, args.FunctionNo, args.TargetDate,out var seq))
            {
                return result;
            }

            // 処理件数
            result.Result = 1;
            // 操作日時
            result.ActionDate = actionDate;
            // 操作キー
            result.ActionKey = actionKey;
            // 生成した番号
            result.Sequence = seq;
            // 成功
            result.IsSuccess = true;

            return result;
        }


        private bool GetNewSequence(int systemType, int functionNo, DateTime targetDate, out int newSeq)
        {
            newSeq = -1;

            using (var con = QsDbManager.CreateDbConnection<QH_DAILYSEQUENCE_MST>())
            {
                var param = new List<DbParameter>
                {
                    CreateParameter(con, "@systemType", systemType),
                    CreateParameter(con, "@functionNo", functionNo),
                    CreateParameter(con, "@targetDate", targetDate.Date)
                };

                // 最新のSEQを取得
                var sql = $@"
                    Select *
                    From {nameof(QH_DAILYSEQUENCE_MST)} WITH(UPDLOCK, ROWLOCK)
                    Where {nameof(QH_DAILYSEQUENCE_MST.SYSTEMTYPE)} = @systemType
                    AND {nameof(QH_DAILYSEQUENCE_MST.FUNCTIONNO)} = @functionNo
                    AND {nameof(QH_DAILYSEQUENCE_MST.TARGETDATE)} = @targetDate;
                ";

                con.Open();

                var result = ExecuteReader<QH_DAILYSEQUENCE_MST>(con, null, sql, param);
                var seq = result.FirstOrDefault()?.SEQ ?? 0;
                newSeq = seq + 1;
            }

            using(var con = QsDbManager.CreateDbConnection<QH_DAILYSEQUENCE_MST>())
            {
                var now = DateTime.Now;

                var param = new List<DbParameter>
                {
                    CreateParameter(con, "@systemType", systemType),
                    CreateParameter(con, "@functionNo", functionNo),
                    CreateParameter(con, "@targetDate", targetDate.Date),
                    CreateParameter(con, "@seq", newSeq),
                    CreateParameter(con, "@updatedDate", now)
                };

                // 新しいSEQを追加または更新
                var mergeSql = $@"
                    MERGE INTO QH_DAILYSEQUENCE_MST AS A
                    USING(
                        SELECT
                        @systemType AS SYSTEMTYPE, 
                        @functionNo As FUNCTIONNO,
                        @targetDate AS TARGETDATE, 
                        @seq AS SEQ, 
                        @updatedDate AS CREATEDDATE, 
                        @updatedDate AS UPDATEDDATE
                    ) AS B
                    ON A.SYSTEMTYPE = B.SYSTEMTYPE
                    AND A.FUNCTIONNO = B.FUNCTIONNO
                    AND A.TARGETDATE = B.TARGETDATE
                    WHEN MATCHED THEN
                        UPDATE SET
                        SEQ = B.SEQ, 
                        UPDATEDDATE = B.UPDATEDDATE
                    WHEN NOT MATCHED THEN
                        INSERT
                        (SYSTEMTYPE, FUNCTIONNO, TARGETDATE, SEQ, CREATEDDATE, UPDATEDDATE)
                        VALUES
                        (B.SYSTEMTYPE, B.FUNCTIONNO, B.TARGETDATE, B.SEQ, B.CREATEDDATE, B.UPDATEDDATE);
                ";

                con.Open();

                var affectedRows = ExecuteNonQuery(con, null, mergeSql, param);

                return affectedRows == 1;
            }     
        }
    }
}