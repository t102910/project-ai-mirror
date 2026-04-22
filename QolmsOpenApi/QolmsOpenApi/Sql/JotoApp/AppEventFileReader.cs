using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// Joto ネイティブアプリのアプリイベントファイル情報を取得します。
    /// </summary>
    internal sealed class AppEventFileReader : QsDbReaderBase, IQsDbDistributedReader<QH_APPEVENTFILE_DAT, AppEventFileReaderArgs, AppEventFileReaderResults>
    {
        /// <summary>
        /// 分散トランザクションを使用してデータベーステーブルから値を取得します。
        /// </summary>
        public AppEventFileReaderResults ExecuteByDistributed(AppEventFileReaderArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            AppEventFileReaderResults result = new AppEventFileReaderResults() { IsSuccess = false };
            // キー不足時はDBアクセスせず失敗結果を返す。
            if (args.EventKey == Guid.Empty || args.FileKey == Guid.Empty)
            {
                return result;
            }

            try
            {
                var entity = this.GetEntity(args.EventKey, args.FileKey);
                if (entity != null && entity.EVENTKEY != Guid.Empty && entity.FILEKEY != Guid.Empty)
                {
                    result.Result = new List<QH_APPEVENTFILE_DAT>() { entity };
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return result;
        }

        private QH_APPEVENTFILE_DAT GetEntity(Guid eventKey, Guid fileKey)
        {
            // 削除されていない行を EventKey + FileKey で1件取得する。
            var sql = @"
                select *
                from qh_appeventfile_dat
                where deleteflag = 0
                  and eventkey = @p1
                  and filekey = @p2;
            ";

            using (var con = QsDbManager.CreateDbConnection<QH_APPEVENTFILE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", eventKey),
                    CreateParameter(con, "@p2", fileKey)
                };

                con.Open();

                return ExecuteReader<QH_APPEVENTFILE_DAT>(
                    con,
                    null,
                    CreateCommandText(con, sql),
                    parameters).FirstOrDefault();
            }
        }
    }
}
