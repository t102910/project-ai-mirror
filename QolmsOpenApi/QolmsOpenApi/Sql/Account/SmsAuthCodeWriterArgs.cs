using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class SmsAuthCodeWriterArgs :  QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 対象のEntityを設定する
        /// </summary>
        public QH_SMSAUTHCODE_DAT Entity { get; set; }

        /// <summary>
        /// 物理削除するかどうかを設定する
        /// </summary>
        public bool IsPhysicalDelete { get; set; } = false;
    }
}