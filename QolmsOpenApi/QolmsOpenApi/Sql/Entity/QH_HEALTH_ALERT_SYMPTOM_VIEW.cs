using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_HEALTHRECORDALERT_DATとQH_SYMPTOM_DATを結合したVIEW
    /// アラートと症状のリストを取得するために使用する
    /// </summary>
    public class QH_HEALTH_ALERT_SYMPTOM_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// アラートNo
        /// </summary>
        public long ALERTNO { get; set; }

        /// <summary>
        /// 症状ID
        /// </summary>
        public Guid SYMPTOMID { get; set; } = Guid.Empty;

        /// <summary>
        /// 記録日時
        /// </summary>
        public DateTime RECORDDATE { get; set; }

        /// <summary>
        /// データタイプ
        /// </summary>
        public byte DATATYPE { get; set; }

        /// <summary>
        /// バイタルタイプ
        /// </summary>
        public byte VITALTYPE { get; set; }

        /// <summary>
        /// 値1
        /// </summary>
        public decimal VALUE1 { get; set; }

        /// <summary>
        /// 値2
        /// </summary>
        public decimal VALUE2 { get; set; }

        /// <summary>
        /// 異常値種別
        /// </summary>
        public byte ABNORMALTYPE { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                ALERTNO = reader.GetInt64(0);
                SYMPTOMID = reader.GetGuid(1);
                RECORDDATE = reader.GetDateTime(2);
                DATATYPE = (byte)reader.GetInt32(3);
                VITALTYPE = (byte)reader.GetInt32(4);
                VALUE1 = reader.GetDecimal(5);
                VALUE2 = reader.GetDecimal(6);
                ABNORMALTYPE = (byte)reader.GetInt32(7);

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
        /// <inheritdoc/>
        /// </summary>
        public override bool IsKeysValid()
        {
            return true;
        }
    }
}