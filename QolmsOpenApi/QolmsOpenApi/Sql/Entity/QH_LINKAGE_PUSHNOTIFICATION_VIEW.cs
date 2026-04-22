using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_LINKAGE_DATとQH_ACCOUNTRELATION_DAT
    /// を組みあわせて通知用のUserIDを抽出するためのVIEW
    /// (実際にDBにVIEWを作成しているわけではなくあくまでクエリ結果格納用）
    /// </summary>
    public class QH_LINKAGE_PUSHNOTIFICATION_VIEW : QsDbEntityBase
    {

        /// <summary>
        /// 連携システムID
        /// </summary>
        public string LINKAGESYSTEMID { get; set; } = string.Empty;

        /// <summary>
        /// アカウントキー
        /// </summary>
        public Guid ACCOUNTKEY { get; set; } = Guid.Empty;

        /// <summary>
        /// Push通知用UserID
        /// </summary>
        public string NOTIFICATIONUSERID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                LINKAGESYSTEMID = reader.GetString(0);

                ACCOUNTKEY = reader.GetGuid(1);
                NOTIFICATIONUSERID = reader.GetString(2);

                KeyGuid = Guid.NewGuid();
                DataState = QsDbEntityStateTypeEnum.Unchanged;
                IsEmpty = false;
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