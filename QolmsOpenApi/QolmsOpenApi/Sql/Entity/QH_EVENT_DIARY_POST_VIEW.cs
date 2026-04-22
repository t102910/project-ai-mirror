using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_EVENT_DATから日記情報の投稿数情報を保持するVIEW
    /// </summary>
    public class QH_EVENT_DIARY_POST_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// 当日の日記投稿数
        /// </summary>
        public int TODAY { get; set; }

        /// <summary>
        /// 1日前～2日前の投稿数
        /// </summary>
        public int DAY1TO2 { get; set; }

        /// <summary>
        /// 3日前～6日前の投稿数
        /// </summary>
        public int DAY3TO6 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                TODAY = reader.GetInt32(0);
                DAY1TO2 = reader.GetInt32(1);
                DAY3TO6 = reader.GetInt32(2);
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