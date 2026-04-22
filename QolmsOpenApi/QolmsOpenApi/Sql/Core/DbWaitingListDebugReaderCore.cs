using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// 順番待ち情報をDBから取得するための機能を提供します。（デバッグ用）
    /// </summary>
    public class DbWaitingListDebugReaderCore: QsDbReaderBase
    {
        /// <summary>
        /// デバッグ用のForeignKeyの最大値を取得する
        /// 990000000001～999999999999がデバッグ用のコードとする
        /// </summary>
        /// <returns></returns>
        public int GetMaxDebugForeignKey()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_WAITINGLIST_DAT>())
            {
                var sql = @"
                    SELECT CAST(COALESCE(MAX(SUBSTRING(FOREIGNKEY,3,10)),0) AS INT) AS MAXKEY
                    FROM QH_WAITINGLIST_DAT
                    WHERE FOREIGNKEY LIKE '99__________'
                ";

                connection.Open();

                return this.ExecuteScalar<int>(connection, null, sql,null);
            }
        }

        /// <summary>
        /// 指定日で最大の受付番号を取得する（数字以外は入ってない想定）
        /// </summary>
        /// <param name="targetDate">指定日</param>
        /// <returns></returns>
        public int GetMaxReceptionNoInDay(DateTime targetDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_WAITINGLIST_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(connection,"@p1", targetDate)
                };

                var sql = @"
                    SELECT CAST(COALESCE(MAX(RECEPTIONNO),0) As int) As MaxNo
                    FROM QH_WAITINGLIST_DAT
                    WHERE WAITINGDATE = @p1
                ";

                connection.Open();

                return this.ExecuteScalar<int>(connection, null, sql, paramList);
            }
        }
    }
}