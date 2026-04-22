using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// QH_SIGNUP_DATへの書き込みを行う
    /// </summary>
    public class DbSignUpWriterCore: QsDbWriterBase
    {
        /// <summary>
        /// 対象のアカウントのレコードを削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public int DeleteSignUpData(Guid accountKey)
        {
            using(var con = QsDbManager.CreateDbConnection<QH_SIGNUP_DAT>())
            {
                var paramList = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", accountKey)
                };

                var sql = $@"
                    Delete From {nameof(QH_SIGNUP_DAT)}
                    Where {nameof(QH_SIGNUP_DAT.ACCOUNTKEY)} = @p1
                    And {nameof(QH_SIGNUP_DAT.DELETEFLAG)} = 0;
                ";

                con.Open();

                return ExecuteNonQuery(con, null, sql, paramList);
            }
        }
    }
}