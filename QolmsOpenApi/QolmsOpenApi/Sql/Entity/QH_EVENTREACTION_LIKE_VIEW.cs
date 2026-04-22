using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_EVENTREACTION_DATからいいね数のカウントを保持するVIEW
    /// </summary>
    public class QH_EVENTREACTION_LIKE_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// トータルいいね数
        /// </summary>
        public int TOTAL { get; set; }
        /// <summary>
        /// 当日のいいね数
        /// </summary>
        public int TODAY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                TOTAL = reader.GetInt32(0);
                TODAY = reader.GetInt32(1);
            }
            catch
            {
                throw;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsKeysValid()
        {
            return true;
        }
    }
}