using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// QH_NOTICEGROUPTARGET_DAT の既読状態更新を行います。
    /// </summary>
    internal sealed class DbNoticeGroupTargetWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// 対象の既読状態を更新します。
        /// </summary>
        /// <param name="noticeNo">お知らせ番号</param>
        /// <param name="accountKey">対象者アカウントキー</param>
        /// <param name="alreadyReadFlag">既読フラグ</param>
        /// <param name="alreadyReadDate">既読日時</param>
        /// <param name="updatedDate">更新日時</param>
        /// <returns>更新件数が1件の場合 true</returns>
        public bool UpdateNoticeGroupTarget(long noticeNo, Guid accountKey, bool alreadyReadFlag, DateTime alreadyReadDate, DateTime updatedDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_NOTICEGROUPTARGET_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@p1", noticeNo),
                    this.CreateParameter(connection, "@p2", accountKey),
                    this.CreateParameter(connection, "@p3", alreadyReadFlag),
                    this.CreateParameter(connection, "@p4", alreadyReadDate),
                    this.CreateParameter(connection, "@p5", updatedDate)
                };

                query.Append(" update qh_noticegrouptarget_dat");
                query.Append(" set alreadyreadflag = @p3, alreadyreaddate = @p4, updateddate = @p5");
                query.Append(" where noticeno = @p1 and accountkey = @p2 and deleteflag = 0;");

                connection.Open();
                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params) == 1;
            }
        }
    }
}
